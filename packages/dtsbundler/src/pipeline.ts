import * as path from 'node:path';

import {
	API,
	type Program,
	type RawCompilerOptions,
} from 'typescript/sync';
import {
	serverFS,
	type FileSystemCallbacks,
} from 'typescript/fs';

const NODE_MODULES_RE = /[\\/]node_modules[\\/]/;

export function normPath(p: string): string {
	return p.replace(/\\/g, '/');
}

function changeExtensionToDts(fileName: string): string {
	const p = normPath(fileName);
	if (p.endsWith('.d.ts') || p.endsWith('.d.mts') || p.endsWith('.d.cts')) {
		return fileName;
	}
	for (const [ext, dtsExt] of [['.mts', '.d.mts'], ['.cts', '.d.cts'], ['.tsx', '.d.ts'], ['.ts', '.d.ts']] as const) {
		if (p.endsWith(ext)) {
			return p.slice(0, -ext.length) + dtsExt;
		}
	}
	return fileName;
}

const SOURCE_EXTENSIONS = ['.ts', '.tsx', '.mts', '.cts'];
const DECLARATION_EXTENSIONS = ['.d.ts', '.d.mts', '.d.cts'];

/** Whether a file is a local implementation file whose declarations we have to emit. */
function isLocalSourceFile(fileName: string): boolean {
	const path = normPath(fileName);
	if (NODE_MODULES_RE.test(path) || DECLARATION_EXTENSIONS.some((ext) => path.endsWith(ext))) {
		return false;
	}
	return SOURCE_EXTENSIONS.some((ext) => path.endsWith(ext));
}

/**
 * Result of {@link compileDts}: a program built purely over the entry's
 * declaration files, ready for the bundling stage.
 */
export interface CompiledDts {
	/** The API instance that owns the d.ts program (caller must dispose). */
	api: API;
	/** The program over the entry and its declaration files. */
	program: Program;
	/** Absolute path of the entry declaration file within the program. */
	entryPath: string;
}

export interface CompileDtsOptions {
	followSymlinks?: boolean;
}

/**
 * Builds a program over the entry's declarations.
 *
 * - For a `.ts` entry, declaration emit is performed in-process via
 *   `program.getDeclarationEmit` and the emitted `.d.ts` files are served on
 *   a virtual host filesystem (the original `.ts` files are hidden so module
 *   resolution picks the declarations).
 * - For a `.d.ts` entry, the program is created over it directly.
 */
export function compileDts(entryPath: string, tsconfigPath: string, options: CompileDtsOptions = {}): CompiledDts {
	entryPath = path.resolve(entryPath);
	const firstApi = new API({});

	try {
		const parsed = firstApi.parseConfigFile(tsconfigPath);
		if (parsed.errors && parsed.errors.length !== 0) {
			const errors = parsed.errors.map(errorText).join('\n');
			throw new Error(`Error while parsing config file "${tsconfigPath}":\n${errors}`);
		}

		if (entryPath.endsWith('.d.ts') || entryPath.endsWith('.d.mts') || entryPath.endsWith('.d.cts')) {
			// already a declaration entry - no emit stage needed
			const api = new API({
				fs: makeFsCallbacks(new Map(), new Set(), options.followSymlinks ?? false),
			});
			const program = api.createProgram([entryPath], declarationProgramOptions(parsed.options, false));
			return { api, program, entryPath };
		}

		const sourceOptions = declarationProgramOptions(parsed.options, true);
		const sourceProgram = firstApi.createProgram([entryPath], sourceOptions);

		const localSources = sourceProgram.getSourceFileNames().filter(isLocalSourceFile);
		const emit = sourceProgram.getDeclarationEmit([...localSources]);
		if (emit.emitSkipped) {
			const errors = emit.diagnostics.map(errorText).join('\n');
			throw new Error(`Declaration emit failed for "${entryPath}":\n${errors}`);
		}

		// emitted .d.ts path -> text, plus the set of .ts sources we hide
		const dtsFiles = new Map<string, string>();
		for (const [file, out] of emit.outputFiles) {
			dtsFiles.set(normPath(file), out.text);
		}

		const hiddenTs = new Set(localSources.map(normPath));
		const entryDts = normPath(changeExtensionToDts(entryPath));
		if (!dtsFiles.has(entryDts)) {
			throw new Error(`Cannot find emitted declaration for entry "${entryPath}" (expected ${entryDts})`);
		}

		const api = new API({
			fs: makeFsCallbacks(dtsFiles, hiddenTs, options.followSymlinks ?? false),
		});
		const program = api.createProgram([entryDts], declarationProgramOptions(parsed.options, false));

		return { api, program, entryPath: entryDts };
	} finally {
		firstApi.close();
	}
}

function declarationProgramOptions(options: RawCompilerOptions | Record<string, unknown>, emitDeclarations: boolean): RawCompilerOptions {
	// `getDeclarationEmit`/`createProgram` require `declaration: true`;
	// outDir/rootDir relocation needs to be cleared so emitted paths mirror inputs.
	const result: RawCompilerOptions = {
		...options,
		noEmit: !emitDeclarations,
		declaration: true,
		emitDeclarationOnly: emitDeclarations,
	};
	// Note: `isolatedDeclarations` is deliberately not forced here. `getDeclarationEmit`
	// infers types for us (verified byte-identical output with and without the flag), and
	// turning it on would only impose the explicit-annotation requirement on consumers.
	// clear relocation and project-build options
	delete result.outDir;
	delete result.rootDir;
	delete result.declarationDir;
	delete result.incremental;
	delete result.tsBuildInfoFile;
	delete result.composite;
	return result;
}

function errorText(d: { text?: string; messageText?: string }): string {
	return d.text ?? d.messageText ?? String(d);
}

function makeFsCallbacks(dtsFiles: ReadonlyMap<string, string>, hiddenTs: ReadonlySet<string>, followSymlinks: boolean): FileSystemCallbacks {
	const readFile: FileSystemCallbacks['readFile'] = (fileName) => {
		const key = normPath(fileName);
		const hit = dtsFiles.get(key);
		if (hit !== undefined) {
			return hit;
		}
		if (hiddenTs.has(key)) {
			// report as "does not exist" so resolution picks the .d.ts sibling
			return undefined;
		}
		return serverFS.useOS;
	};
	return {
		readFile,
		fileExists: (fileName) => {
			const key = normPath(fileName);
			if (dtsFiles.has(key)) {
				return true;
			}
			if (hiddenTs.has(key)) {
				return false;
			}
			return serverFS.useOS;
		},
		directoryExists: () => serverFS.useOS,
		getAccessibleEntries: () => serverFS.useOS,
		realpath: followSymlinks ? () => serverFS.useOS : () => serverFS.identity,
		stat: serverFS.useOS,
		writeFile: serverFS.noop,
		removeFile: serverFS.useOS,
	};
}