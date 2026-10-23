#!/usr/bin/env node
/*
 * cleanup.mjs - safely remove regenerable build output / installed packages.
 *
 * Unlike `git clean -xfd`, this only removes well-known, regenerable folders:
 *
 *   **\/node_modules/     any depth
 *   **\/.vs/              any depth
 *   **\/obj/              any depth
 *   **\/*.Web/bin/        "bin" directly under a "*.Web" folder
 *   artifacts/            explicit path; also Serenity/artifacts, so it works from
 *                         the StartSharp workspace root as well
 *
 * Workspace selection: the root is the current working directory, so the same
 * script cleans either workspace, depending on where `pnpm cleanup` is run:
 *   P:\startsharp            -> the whole StartSharp workspace (incl. Serenity/)
 *   P:\startsharp\Serenity   -> the Serenity workspace only
 *
 * Notes:
 *  - Symlinks / NTFS junctions are never followed, so pnpm links or junctions
 *    can never cause deletions outside the repo.
 *  - .git folders are not traversed.
 *  - Use `--dry-run` to preview.
 */
import fs from 'node:fs';
import path from 'node:path';

const TARGET_NAMES = new Set(['node_modules', '.vs', 'obj']);
const WEB_BIN_PARENT = /\.web$/i;
const SKIP_NAMES = new Set(['.git']);
const WORKSPACE_MARKERS = ['pnpm-workspace.yaml', 'package.json'];
const EXTRA_PATHS = ['artifacts', 'Serenity/artifacts'];

const args = process.argv.slice(2);
const options = { dryRun: false, root: process.cwd(), rootExplicit: false };

for (let i = 0; i < args.length; i++) {
    const arg = args[i];
    if (arg === '-n' || arg === '--dry-run') options.dryRun = true;
    else if (arg === '--root') {
        options.root = path.resolve(args[++i] ?? '.');
        options.rootExplicit = true;
    }
    else if (arg === '-h' || arg === '--help') {
        console.log(`Usage: node build/cleanup.mjs [--dry-run] [--root <dir>]

Removes node_modules, .vs, obj, *.Web/bin and artifacts.
  -n, --dry-run   only list what would be removed
      --root      workspace root (default: the current directory)`);
        process.exit(0);
    }
    else {
        console.error(`Unknown argument: ${arg}`);
        process.exit(2);
    }
}

const root = options.root;
if (!fs.statSync(root, { throwIfNoEntry: false })?.isDirectory()) {
    console.error(`Root folder not found: ${root}`);
    process.exit(2);
}

// Guard against being run from an arbitrary folder: only clean a pnpm workspace
if (!options.rootExplicit && !WORKSPACE_MARKERS.some((m) => fs.existsSync(path.join(root, m)))) {
    console.error(`${root} does not look like a pnpm workspace root (no ${WORKSPACE_MARKERS.join(' / ')}).\n` +
        'Run it from a workspace root, e.g. `pnpm cleanup`, or pass --root <dir>.');
    process.exit(2);
}

const rel = (full) => path.relative(root, full) || '.';
const kindOf = (name, parentName) => {
    const lower = name.toLowerCase();
    if (TARGET_NAMES.has(lower)) return lower === 'node_modules' ? 'node_modules' : name;
    if (lower === 'bin' && WEB_BIN_PARENT.test(parentName ?? '')) return '*.Web/bin';
    return null;
};

/** Collect target directories, never descending into a matched or linked folder. */
function collect(rootDir) {
    const found = [];
    const stack = [rootDir];

    while (stack.length) {
        const dir = stack.pop();
        let entries;
        try {
            entries = fs.readdirSync(dir, { withFileTypes: true });
        }
        catch (err) {
            console.warn(`  ! cannot read ${rel(dir)}: ${err.code}`);
            continue;
        }

        for (const entry of entries) {
            if (!entry.isDirectory() || entry.isSymbolicLink()) continue;
            if (SKIP_NAMES.has(entry.name)) continue;

            const full = path.join(dir, entry.name);
            const kind = kindOf(entry.name, path.basename(dir));
            if (kind) found.push({ kind, full });
            else stack.push(full);
        }
    }

    for (const extra of EXTRA_PATHS) {
        const full = path.join(rootDir, extra);
        if (fs.statSync(full, { throwIfNoEntry: false })?.isDirectory()) found.push({ kind: extra, full });
    }

    // Drop targets already covered by a parent target (e.g. obj inside Serenity/artifacts)
    const kept = [];
    for (const target of found.sort((a, b) => a.full.length - b.full.length)) {
        const covered = kept.some((k) => target.full.startsWith(k.full + path.sep));
        if (!covered) kept.push(target);
    }
    return kept;
}

/** fs.rmSync, with a chmod based fallback for read-only files on Windows. */
function removeTree(target) {
    try {
        fs.rmSync(target, { recursive: true, force: true, maxRetries: 3, retryDelay: 100 });
        return true;
    }
    catch (err) {
        if (!['EPERM', 'EACCES', 'EBUSY', 'ENOTEMPTY'].includes(err.code)) throw err;
    }

    removeReadOnly(target);
    return true;
}

function removeReadOnly(target) {
    const stat = fs.lstatSync(target, { throwIfNoEntry: false });
    if (!stat) return;

    if (stat.isDirectory()) {
        for (const entry of fs.readdirSync(target)) removeReadOnly(path.join(target, entry));
        try { fs.rmdirSync(target); }
        catch { fs.chmodSync(target, 0o666); fs.rmdirSync(target); }
    }
    else {
        try { fs.chmodSync(target, 0o666); } catch { /* ignore */ }
        fs.unlinkSync(target);
    }
}

console.log(`Cleaning ${root}${options.dryRun ? ' (dry run)' : ''}`);

const targets = collect(root);
const counts = {};
let removed = 0;
let failed = 0;

// Deepest first, so a child never blocks a parent
for (const target of targets.sort((a, b) => b.full.length - a.full.length)) {
    const display = rel(target.full);
    if (options.dryRun) {
        console.log(`  would remove  ${display}`);
        continue;
    }

    try {
        removeTree(target.full);
        console.log(`  removed  ${display}`);
        removed++;
        counts[target.kind] = (counts[target.kind] ?? 0) + 1;
    }
    catch (err) {
        failed++;
        console.warn(`  ! failed  ${display}: ${err.message}`);
    }
}

if (targets.length === 0) {
    console.log('Nothing to clean.');
}
else if (options.dryRun) {
    console.log(`\nWould remove ${targets.length} directories. Re-run without --dry-run to delete them.`);
}
else {
    const details = Object.entries(counts).map(([kind, count]) => `${kind}: ${count}`).join(', ');
    console.log(`\nRemoved ${removed} of ${targets.length} directories${details ? ` (${details})` : ''}.`);
}

process.exit(failed > 0 ? 1 : 0);
