/**
 * Bundles a package's declaration entry with @serenity-is/dtsbundler and compares
 * the result against the bundle already committed in that package's `dist/`.
 *
 * The new bundle overwrites the package's `dist/index.d.ts` by default, so the
 * result can be compared with `git diff`. Use --out to write elsewhere instead.
 *
 * Usage:
 *   node scripts/compare.mjs <entry> <tsconfig.json> [flags]
 *
 * Flags mirror dts-bundle-generator's options so every package can be compared with
 * the exact same configuration its `dts` script uses:
 *   --no-banner                    (default: banner is off, like the repo's scripts)
 *   --export-referenced-types      keep `export` on referenced-only types (dbg default)
 *   --inline-declare-globals
 *   --inline-declare-externals
 *   --follow-symlinks
 *   --out <file>                   write elsewhere than <package>/dist/index.d.ts
 *   --full-diff                    print the full unified diff
 */
import { execFileSync } from "node:child_process";
import { existsSync, mkdirSync, mkdtempSync, readFileSync, rmSync, writeFileSync } from "node:fs";
import { tmpdir } from "node:os";
import { basename, dirname, join, resolve } from "node:path";
import process from "node:process";

import { bundleDts } from "../dist/index.js";

const [entryArg, projectArg, ...rest] = process.argv.slice(2);
if (!entryArg || !projectArg) {
    console.error("usage: node scripts/compare.mjs <entry.ts> <tsconfig.json> [--out file] [--full-diff]");
    process.exit(1);
}

const entry = resolve(entryArg);
const project = resolve(projectArg);
// the package that owns the given path (nearest ancestor with a package.json), so that
// nested entry paths (e.g. `src/index.ts` vs `Modules/index.ts`) resolve dist/index.d.ts
function findPackageRoot(start) {
    let dir = dirname(start);
    for (;;) {
        if (existsSync(join(dir, "package.json"))) {
            return dir;
        }
        const parent = dirname(dir);
        if (parent === dir) {
            return dirname(start);
        }
        dir = parent;
    }
}

const packageDir = findPackageRoot(project);
const outIndex = rest.indexOf("--out");
const outFile = outIndex >= 0
    ? resolve(rest[outIndex + 1])
    : join(packageDir, "dist", "index.d.ts");
const fullDiff = rest.includes("--full-diff");

// flags mirror the dts-bundle-generator options used by each package's `dts` script
const exportReferencedTypes = rest.includes("--export-referenced-types"); // opt-in, off by default
const inlineDeclareGlobals = !rest.includes("--no-inline-declare-globals");
const inlineDeclareExternals = !rest.includes("--no-inline-declare-externals");
const followSymlinks = rest.includes("--follow-symlinks");

// Read the existing bundle *before* overwriting it - it is the comparison baseline.
const reference = join(packageDir, "dist", "index.d.ts");
const refText = existsSync(reference) ? readFileSync(reference, "utf8") : null;

const { dts: bundle, timings } = bundleDts({
    entry,
    project,
    outFile,
    followSymlinks,
    exportReferencedTypes,
    inlineDeclareGlobals,
    inlineDeclareExternals,
});

console.log(`entry     ${entry}`);
console.log(`wrote     ${outFile}`);
console.log(`size      ${bundle.length} chars, ${bundle.split("\n").length} lines, ${timings.compile + timings.bundle} ms (compile ${timings.compile}, bundle ${timings.bundle})`);

/** One "signature" per top-level construct, normalized so ordering can be compared. */
function signatures(text) {
    const signaturePrefix = /^(?:export\s+)?(?:declare\s+)?(?:default\s+)?(?:abstract\s+)?(?:const\s+)?(interface|type|class|namespace|module|function|enum|var|let|const|import|export)\b/;
    const result = [];
    let depth = 0;
    for (const rawLine of text.split("\n")) {
        const line = rawLine.trim();
        if (line !== "" && !line.startsWith("*") && !line.startsWith("/*") && !line.startsWith("//")) {
            if (depth === 0 && signaturePrefix.test(line)) {
                result.push(line
                    .replace(/^export\s+/, "")
                    .replace(/^declare\s+/, "")
                    .replace(/\{.*$/, "")
                    .replace(/\s+/g, " ")
                    .trim());
            }
            if (!line.startsWith("//") && !line.startsWith("*") && !line.startsWith("/*")) {
                depth += (rawLine.match(/[{([]/g) ?? []).length - (rawLine.match(/[})\]]/g) ?? []).length;
                if (depth < 0) {
                    depth = 0;
                }
            }
        }
    }
    return result;
}

if (refText === null) {
    console.log(`\nno reference bundle at ${reference}, skipping comparison`);
    process.exit(0);
}

const refSignatures = signatures(refText);
const newSignatures = signatures(bundle);

const refSet = new Set(refSignatures);
const newSet = new Set(newSignatures);
const missing = refSignatures.filter((s) => !newSet.has(s));
const extra = newSignatures.filter((s) => !refSet.has(s));
const common = refSignatures.filter((s) => newSet.has(s));
const orderMatches = common.filter((s, index) => newSignatures.indexOf(s) === index || newSignatures.indexOf(s) >= 0).length;

console.log(`\nreference ${refText.length} chars, ${refSignatures.length} top-level declarations`);
console.log(`new       ${bundle.length} chars, ${newSignatures.length} top-level declarations`);
console.log(`matched   ${common.length}/${refSignatures.length}`);
console.log(`shared    ${common.length} common; first ${Math.min(common.length, 10)}: ${common.slice(0, 10).join(" | ")}`);
void orderMatches;

if (common.length !== 0) {
    const newCommonSequence = newSignatures.filter((s) => refSet.has(s));
    const sameOrder = common.every((s, index) => newCommonSequence[index] === s);
    console.log(`order     ${sameOrder ? "identical to reference" : "DIFFERS from reference (see signature diff below)"}`);
}

const listDiff = (title, list) => {
    if (list.length === 0) {
        return;
    }
    console.log(`\n${title} (${list.length}):`);
    for (const signature of list.slice(0, 25)) {
        console.log(`  ${signature}`);
    }
    if (list.length > 25) {
        console.log(`  ... and ${list.length - 25} more`);
    }
};

listDiff("only in reference", missing);
listDiff("only in new bundle", extra);

const temp = mkdtempSync(join(tmpdir(), "dtsbundler-signatures-"));
try {
    const refSignaturesFile = join(temp, "reference.txt");
    const newSignaturesFile = join(temp, "new.txt");
    writeFileSync(refSignaturesFile, refSignatures.join("\n") + "\n");
    writeFileSync(newSignaturesFile, newSignatures.join("\n") + "\n");
    let diff = "";
    try {
        diff = execFileSync("git", ["diff", "--no-index", "--unified=1", "--", refSignaturesFile, newSignaturesFile], { encoding: "utf8" });
    }
    catch (error) {
        diff = error.stdout ?? "";
    }
    if (diff !== "") {
        console.log("\ntop-level declaration ordering diff (- reference, + new):\n");
        console.log(diff.split("\n").filter((line) => !line.startsWith("diff --git") && !line.startsWith("index ") && !line.startsWith("--- ") && !line.startsWith("+++ ")).join("\n"));
    }
}
finally {
    rmSync(temp, { recursive: true, force: true });
}

if (fullDiff) {
    const fullTemp = mkdtempSync(join(tmpdir(), "dtsbundler-full-"));
    try {
        const refFile = join(fullTemp, "reference.d.ts");
        const newFile = join(fullTemp, "new.d.ts");
        writeFileSync(refFile, refText);
        writeFileSync(newFile, bundle + "\n");
        try {
            console.log(execFileSync("git", ["diff", "--no-index", "--", refFile, newFile], { encoding: "utf8" }));
        }
        catch (error) {
            console.log(error.stdout ?? "");
        }
    }
    finally {
        rmSync(fullTemp, { recursive: true, force: true });
    }
}