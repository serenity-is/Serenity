/**
 * Options for {@link bundleDts}.
 *
 * Modelled on the `dts-bundle-generator` options this workspace used before, but
 * not command line compatible with it: some are renamed, some defaults are
 * inverted, and unsupported ones are rejected. See `--help` for the CLI surface.
 */
export interface DtsBundlerOptions {
    /**
     * Path to the input file. Can be a `.ts` source (it will be compiled to
     * declarations in-process) or an already-emitted `.d.ts` file.
     */
    entry: string;
    /**
     * Path to the `tsconfig.json` to use. When omitted, one is auto-discovered
     * from the entry's directory and its parents.
     */
    project?: string;
    /**
     * When set, the generated bundle is written to this file instead of being
     * returned as a string.
     */
    outFile?: string;
    /**
     * Resolve symlinked paths to their real location (e.g. into the pnpm store)
     * instead of using them as given.
     *
     * Off by default, and there is rarely a reason to turn it on: a workspace
     * dependency (`"pkg": "workspace:*"`) is a symlink to a sibling folder, so
     * resolving it makes the dependency look like a local source file and it gets
     * **inlined** into the bundle instead of being kept as an `import`.
     */
    followSymlinks?: boolean;
    /**
     * Carry over the `declare global { ... }` blocks of inlined files.
     *
     * On by default: those blocks are pure augmentations and nothing references
     * them by name, so dropping them loses global declarations without any error.
     * Set to `false` (`--no-inline-declare-global`) to leave them out.
     */
    inlineDeclareGlobals?: boolean;
    /**
     * Carry over the `declare module "package" { ... }` augmentation blocks of
     * inlined files. Relative ones (`declare module "./x"`) are not affected;
     * they are promoted to top-level declarations instead.
     *
     * On by default, for the same reason as {@link inlineDeclareGlobals}.
     */
    inlineDeclareExternals?: boolean;
    /**
     * Also export the declarations that the bundle only contains because the
     * public API references them. Off by default, so the bundle exports exactly
     * what the entry exports and nothing else.
     *
     * The declarations themselves are always included either way; this only
     * decides whether they get an `export` keyword.
     */
    exportReferencedTypes?: boolean;
    /**
     * Package names whose declaration files should be inlined into the bundle
     * instead of being imported (like `--external-inlines`), e.g.
     * `["@preact/signals-core"]`.
     */
    externalInlines?: string[];
}
/**
 * Finds the closest `tsconfig.json` at or above a file's directory.
 * Throws when there is none, since the declaration emit needs compiler options.
 */
export declare function findConfigFrom(entry: string): string;
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
export declare function bundleDts(options: DtsBundlerOptions): BundleResult;

export {};
