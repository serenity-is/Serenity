#!/usr/bin/env node
import * as fs from 'node:fs';
import * as path from 'node:path';
import process from 'node:process';

import { API, type Diagnostic, type RawCompilerOptions } from 'typescript/sync';

import { bundleDts, findConfigFrom, type DtsBundlerOptions } from './index';

const USAGE = `Usage: dtsbundler [entry] [options]

Bundle TypeScript declarations into a single .d.ts file.

Defaults:
  entry   src/index.ts, index.ts or Modules/index.ts -- whichever of those exists
          (they must not be ambiguous; otherwise pass the input file explicitly)
  output  dist/index.d.ts         (relative to the current directory)
  project auto-discovered from the entry ('--project' to override)

Options:
  -o, --out-file                   File name of the generated d.ts
      --project                    Path to the tsconfig.json file used for the compilation
      --no-check                   Skip validation of the generated d.ts file
      --no-inline-declare-global   Leave out the \`declare global\` blocks of inlined files
                                   (they are included by default)
      --no-inline-declare-externals
                                   Leave out the \`declare module "..."\` augmentation blocks of
                                   inlined files, external ones only (included by default)
      --follow-symlinks            Resolve symlinks to their real (pnpm store) location
      --export-referenced-types    Also export the declarations that are only referenced by the
                                   public API instead of exported by the entry (default: off)
      --external-inlines           Package names to inline the typings of. May be repeated,
                                   or given as space separated values (repeat or use \`--\` if a
                                   value could be mistaken for a file path)
  -v, --verbose                    Enable verbose logging
      --silent                     Disable any logging except errors
      --version                    Show version number
  -h, --help                       Show this help

Not (yet) supported, they are rejected with an error:
  --sort, --fail-on-class, --respect-preserve-const-enum, --external-imports,
  --external-types, --umd-module-name, --config

Examples:
  dtsbundler                          # entry + dist/index.d.ts by convention
  dtsbundler src/index.ts -o dist/bundle.d.ts
  dtsbundler Modules/index.ts --no-inline-declare-global`;

/**
 * Entry files we look for when no positional argument is given, in order of preference.
 * Only used when exactly one of them exists, so an ambiguous folder never guesses.
 */
const DEFAULT_ENTRY_CANDIDATES = ['src/index.ts', 'index.ts', 'Modules/index.ts'];
/** Output file used when `--out-file` is not given. */
const DEFAULT_OUT_FILE = 'dist/index.d.ts';

const UNSUPPORTED = new Set([
	'sort',
	'fail-on-class',
	'respect-preserve-const-enum',
	'external-imports',
	'external-types',
	'umd-module-name',
	'config',
]);

type FlagKind = 'boolean' | 'string' | 'array';

const FLAGS: Record<string, { kind: FlagKind; alias?: string }> = {
	'help': { kind: 'boolean', alias: 'h' },
	'version': { kind: 'boolean' },
	'verbose': { kind: 'boolean', alias: 'v' },
	'silent': { kind: 'boolean' },
	'check': { kind: 'boolean' },
	'inline-declare-global': { kind: 'boolean' },
	'inline-declare-externals': { kind: 'boolean' },
	'follow-symlinks': { kind: 'boolean' },
	'export-referenced-types': { kind: 'boolean' },
	'out-file': { kind: 'string', alias: 'o' },
	'project': { kind: 'string' },
	'external-inlines': { kind: 'array' },
};

const ALIASES = new Map(Object.entries(FLAGS)
	.filter(([, spec]) => spec.alias !== undefined)
	.map(([name, spec]) => [spec.alias!, name]));

interface ParsedArgs {
	booleans: Map<string, boolean>;
	strings: Map<string, string>;
	arrays: Map<string, string[]>;
	positionals: string[];
}

const BOOLEAN_VALUES = new Map<string, boolean>([
	['true', true], ['false', false], ['1', true], ['0', false],
]);

function parseBoolean(raw: string, flag: string): boolean {
	const value = BOOLEAN_VALUES.get(raw);
	if (value === undefined) {
		throw new Error(`Option --${flag} expects true or false, got "${raw}"`);
	}
	return value;
}

/**
 * Minimal `yargs`-compatible parser: `--flag`, `--no-flag`, `--flag=value`,
 * `--flag value`, `-o value`, repeatable array flags and `--` as the
 * "everything after this is a positional" terminator.
 */
function parseArgs(argv: readonly string[]): ParsedArgs {
	const result: ParsedArgs = { booleans: new Map(), strings: new Map(), arrays: new Map(), positionals: [] };

	let onlyPositionals = false;
	for (let i = 0; i < argv.length; i++) {
		const arg = argv[i];
		if (onlyPositionals || !arg.startsWith('-') || arg === '-') {
			result.positionals.push(arg);
			continue;
		}
		if (arg === '--') {
			onlyPositionals = true;
			continue;
		}

		const isLong = arg.startsWith('--');
		const raw = isLong ? arg.slice(2) : arg.slice(1);
		const eq = raw.indexOf('=');
		let name = eq === -1 ? raw : raw.slice(0, eq);
		const inlineValue = eq === -1 ? undefined : raw.slice(eq + 1);

		// `-o=dist/x.d.ts` / `-odist/x.d.ts` are not supported, `-o dist/x.d.ts` is
		if (!isLong) {
			const longName = ALIASES.get(name);
			if (longName === undefined) {
				throw new Error(`Unknown option "-${name}"`);
			}
			name = longName;
		}

		// `--no-foo` turns the boolean `foo` off
		let negated = false;
		if (isLong && name.startsWith('no-')) {
			const positive = name.slice(3);
			if (FLAGS[positive]?.kind === 'boolean') {
				name = positive;
				negated = true;
			}
		}

		const spec = FLAGS[name];
		if (spec === undefined) {
			if (UNSUPPORTED.has(name)) {
				throw new Error(`Option "--${name}" is not supported by @serenity-is/dtsbundler yet`);
			}
			throw new Error(`Unknown option "${isLong ? '--' : '-'}${name}"`);
		}

		switch (spec.kind) {
			case 'boolean': {
				const value = inlineValue !== undefined
					? parseBoolean(inlineValue, name)
					: !negated;
				if (negated && inlineValue !== undefined) {
					throw new Error(`Option --no-${name} does not take a value`);
				}
				result.booleans.set(name, value);
				break;
			}
			case 'string': {
				const value = inlineValue ?? argv[++i];
				if (value === undefined) {
					throw new Error(`Option --${name} requires a value`);
				}
				result.strings.set(name, value);
				break;
			}
			case 'array': {
				const values: string[] = result.arrays.get(name) ?? [];
				if (inlineValue !== undefined) {
					values.push(inlineValue);
				}
				else if (argv[i + 1] !== undefined && !argv[i + 1].startsWith('-')) {
					values.push(argv[++i]);
					// keep consuming `--external-inlines a b`, but stop at something that
					// looks like the entry file, so `--external-inlines foo src/index.ts` works
					while (argv[i + 1] !== undefined && !argv[i + 1].startsWith('-')
						&& !fs.existsSync(argv[i + 1])) {
						values.push(argv[++i]);
					}
				}
				else {
					throw new Error(`Option --${name} requires at least one value`);
				}
				result.arrays.set(name, values);
				break;
			}
		}
	}
	return result;
}

function readVersion(): string {
	try {
		const pkg = JSON.parse(fs.readFileSync(new URL('../package.json', import.meta.url), 'utf8')) as { version?: string };
		return pkg.version ?? '0.0.0';
	}
	catch {
		return '0.0.0';
	}
}

/**
 * Resolves the entry file: an explicit positional wins; otherwise the single
 * conventional entry that exists next to the current directory.
 */
function resolveEntry(entryArg: string | undefined): string {
	if (entryArg !== undefined) {
		const entry = path.resolve(entryArg);
		if (!fs.existsSync(entry)) {
			throw new Error(`Input file "${entryArg}" does not exist`);
		}
		return entry;
	}

	const cwd = process.cwd();
	const found = DEFAULT_ENTRY_CANDIDATES.filter((candidate) => fs.existsSync(path.join(cwd, candidate)));
	if (found.length === 1) {
		return path.resolve(cwd, found[0]);
	}

	const candidates = DEFAULT_ENTRY_CANDIDATES.join(', ');
	if (found.length === 0) {
		throw new Error(`Cannot find an entry file, looked for ${candidates}. Pass the input file explicitly`);
	}
	throw new Error(`Found more than one possible entry file (${found.join(', ')}), pass the input file to use explicitly`);
}

function formatDiagnostic(diagnostic: Diagnostic): string {
	const location = diagnostic.fileName !== undefined && diagnostic.startPosition !== undefined
		? `${diagnostic.fileName}(${diagnostic.startPosition.line + 1},${diagnostic.startPosition.character + 1}): `
		: '';
	return `${location}error TS${diagnostic.code}: ${diagnostic.text}`;
}

/**
 * Validates the generated bundle by type-checking it, like
 * dts-bundle-generator does unless `--no-check` is passed.
 */
function checkBundle(outFile: string, project: string | undefined): string[] {
	const api = new API({});
	try {
		const options: RawCompilerOptions = {};
		if (project !== undefined) {
			Object.assign(options, api.parseConfigFile(project).options);
		}
		const program = api.createProgram([outFile], {
			...options,
			declaration: true,
			emitDeclarationOnly: true,
			noEmit: true,
		});
		const diagnostics = [...program.getSyntacticDiagnostics(), ...program.getSemanticDiagnostics()];
		return diagnostics
			.filter((diagnostic) => diagnostic.category === 1 /* DiagnosticCategory.Error */)
			.map(formatDiagnostic);
	}
	finally {
		api.close();
	}
}

/** Windows prints backslashes in paths, the log reads better with forward slashes. */
const posix = (text: string) => text.replaceAll('\\', '/');

function run(argv: readonly string[]): number {
	const args = parseArgs(argv);

	if (args.booleans.get('help')) {
		console.log(USAGE);
		return 0;
	}
	if (args.booleans.get('version')) {
		console.log(readVersion());
		return 0;
	}

	const silent = args.booleans.get('silent') ?? false;
	const verbose = args.booleans.get('verbose') ?? false;
	if (silent && verbose) {
		throw new Error('Cannot use both --silent and --verbose options at the same time');
	}
	const log = (message: string): void => {
		if (!silent) {
			console.log(message);
		}
	};

	if (args.positionals.length > 1) {
		throw new Error('Multiple entry files are not supported yet, specify a single input file');
	}

	const entry = resolveEntry(args.positionals[0]);

	const projectArg = args.strings.get('project');
	const project = projectArg === undefined ? findConfigFrom(entry) : path.resolve(projectArg);
	const outFile = path.resolve(args.strings.get('out-file') ?? DEFAULT_OUT_FILE);

	const externalInlines = args.arrays.get('external-inlines') ?? [];
	const check = args.booleans.get('check') ?? true;

	const options: DtsBundlerOptions = {
		entry,
		project,
		outFile,
		followSymlinks: args.booleans.get('follow-symlinks') ?? false,
		inlineDeclareGlobals: args.booleans.get('inline-declare-global') ?? true,
		inlineDeclareExternals: args.booleans.get('inline-declare-externals') ?? true,
		exportReferencedTypes: args.booleans.get('export-referenced-types') ?? false,
		externalInlines,
	};

	if (verbose) {
		log(`entry      ${entry}`);
		log(`project    ${project}`);
		log(`out-file   ${outFile}`);
		log(`options    ${JSON.stringify({ ...options, entry: undefined, project: undefined, outFile: undefined }, undefined, 4)}`);
	}

	const result = bundleDts(options);

	const checkStarted = performance.now();
	if (check) {
		const errors = checkBundle(outFile, project);
		if (errors.length !== 0) {
			throw new Error(`Generated "${outFile}" has errors:\n${errors.join('\n')}`);
		}
	}
	const checkMs = Math.round(performance.now() - checkStarted);

	const action = result.written ? 'Writing' : 'Unchanged';
	const timings = [
		`compile ${result.timings.compile} ms`,
		`bundle ${result.timings.bundle} ms`,
		check ? `check ${checkMs} ms` : `check skipped`,
	];
	log(`${action} ${posix(path.relative(process.cwd(), entry))} -> ${posix(path.relative(process.cwd(), outFile))} (${timings.join(', ')})`);

	return 0;
}

try {
	process.exitCode = run(process.argv.slice(2));
}
catch (e) {
	console.error(`Error: ${e instanceof Error ? e.message : String(e)}`);
	process.exitCode = 1;
}
