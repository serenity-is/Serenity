import * as fs from 'node:fs';
import * as path from 'node:path';

import { bundle } from './bundler';
import { compileDts } from './pipeline';
import type { BundleStageOptions, DtsBundlerOptions } from './types';

export type { DtsBundlerOptions };

/**
 * Finds the closest `tsconfig.json` at or above a file's directory.
 * Throws when there is none, since the declaration emit needs compiler options.
 */
export function findConfigFrom(entry: string): string {
	let dir = path.dirname(path.resolve(entry));
	for (;;) {
		const candidate = path.join(dir, 'tsconfig.json');
		if (fs.existsSync(candidate)) {
			return candidate;
		}
		const parent = path.dirname(dir);
		if (parent === dir) {
			break;
		}
		dir = parent;
	}
	throw new Error(`Cannot find tsconfig.json starting from "${entry}"`);
}

function toStageOptions(options: DtsBundlerOptions): BundleStageOptions {
	return {
		followSymlinks: options.followSymlinks ?? false,
		inlineDeclareGlobals: options.inlineDeclareGlobals ?? true,
		inlineDeclareExternals: options.inlineDeclareExternals ?? true,
		exportReferencedTypes: options.exportReferencedTypes ?? false,
		externalInlines: options.externalInlines ?? [],
	};
}

/**
 * Result of {@link bundleDts}.
 */
export interface BundleResult {
	/** The bundled declarations. */
	dts: string;

	/** Absolute path the bundle was written to, when `outFile` was given. */
	outFile: string | undefined;

	/**
	 * Whether the output file was actually written. `false` when the file
	 * already contained exactly this bundle (nothing is touched in that case),
	 * or when no `outFile` was requested.
	 */
	written: boolean;

	/** Wall clock timings in milliseconds, per pipeline stage. */
	timings: {
		/** Declaration emit + program creation for the declaration program. */
		compile: number;
		/** The checker-driven crawl that produces the bundle text. */
		bundle: number;
	};
}

/** Writes only when the content differs, so unchanged builds do not touch the file. */
function writeIfChanged(fileName: string, text: string): boolean {
	if (fs.existsSync(fileName) && fs.readFileSync(fileName, 'utf8') === text) {
		return false;
	}
	fs.mkdirSync(path.dirname(fileName), { recursive: true });
	fs.writeFileSync(fileName, text);
	return true;
}

/**
 * Bundles the entry point's declaration files into a single `.d.ts` string.
 *
 * The pipeline runs entirely on the TypeScript 7 (native/tsgo) API:
 * - `api.createProgram` over the sources
 * - `program.getDeclarationEmit` for in-process declaration emit
 * - a second program over the emitted declarations, then a checker-driven
 *   crawl that inlines local declarations, imports external libraries, and
 *   resolves name collisions.
 *
 * When `outFile` is given the bundle is written to it, unless the file already
 * has exactly that content.
 */
export function bundleDts(options: DtsBundlerOptions): BundleResult {
	const tsconfigPath = options.project ?? findConfigFrom(options.entry);

	const compileStarted = performance.now();
	const compiled = compileDts(options.entry, tsconfigPath, {
		followSymlinks: options.followSymlinks,
	});
	const compile = performance.now() - compileStarted;

	try {
		const bundleStarted = performance.now();
		const dts = bundle({
			program: compiled.program,
			entryPath: compiled.entryPath,
			options: toStageOptions(options),
		});
		const bundleMs = performance.now() - bundleStarted;

		const outFile = options.outFile === undefined ? undefined : path.resolve(options.outFile);
		const written = outFile !== undefined && writeIfChanged(outFile, dts + '\n');

		return {
			dts,
			outFile,
			written,
			timings: { compile: Math.round(compile), bundle: Math.round(bundleMs) },
		};
	} finally {
		compiled.api.close();
	}
}