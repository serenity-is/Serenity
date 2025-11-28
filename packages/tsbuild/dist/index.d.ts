export const defaultEntryPointGlobs: string[];
export const importAsGlobalsMapping: Record<string, string>;


export interface TSBuildOptions extends Partial<import("esbuild").BuildOptions> {
    /** Enable bundling of dependencies, default is true */
    bundle?: boolean;

    /** Chunk names to generate when code splitting is enabled. Default is '_chunks/[name]-[hash]' */
    chunkNames?: string;

    /** True to enable the clean plugin. Default is true if splitting is true. */
    clean?: boolean | CleanPluginOptions;

    /**
     * Determines the set of entry points that should be passed to the esbuild. 
     * Only use to specify full paths of entry points manually if you calculated them yourself.
     * Prefer specifying entry point globs in sergen.json under TSBuild:EntryPoints which supports
     * globs and defaults to ['Modules/** /*Page.ts', 'Modules/** /*Page.tsx', 'Modules/** /ScriptInit.ts'] */
    entryPoints?: string[];

    /**
     * A set of mappings to pass to the importAsGlobalsPlugin. If this is undefined or any object and the plugins 
     * is not specified, importAsGlobals plugin is enabled */
    importAsGlobals?: Record<string, string>;

    /**
     * True to enable metafile generation by esbuild. Default is true.
     * If this is false, clean plugin won't work properly.
     */
    metafile?: boolean;

    /** True to enable minification. Default is true. */
    minify?: boolean;

    /** False to not call npmCopy automatically for entries in appsettings.bundles.json that start with `~/npm/`. Default is true.*/
    npmCopy?: boolean;

    /** Base directory for calculating output file locations in output directory. Default is "./" */
    outbase?: string;

    /** Base output directory. Default is wwwroot/esm */
    outdir?: string;

    /** True to enable code splitting. Default is true unless --nosplit is passed in process arguments. */
    splitting?: boolean;

    /** Should source maps be generated. Default is true. */
    sourcemap?: boolean;

    /* Javascript target for output files. Default is es6. */
    target?: string;

    /** True to watch, default is calculated from process args and true if it contains --watch */
    watch?: boolean;
}

/** Default esbuild options used by TSBuild */
export const esbuildDefaults: import("esbuild").BuildOptions;

/** Processes passed options and converts it to options suitable for esbuild */
export const esbuildOptions: (opt: TSBuildOptions) => any;

/** Calls esbuild with passed options. By default, this is used to generate files under wwwroot/esm/ from entry points under Modules/
 * but this can be changed by passing outdir and outbase, and other options. */
export const build: (opt: TSBuildOptions) => Promise<void>;

/** Builds bundles from entry points under Modules/Common/bundles/*-bundle.ts and -bundle.css globs. By default, this is used to generate files under wwwroot/esm/bundles/
 * but this can be changed by passing outdir and outbase, and other options. */
export const buildBundles: (opt: TSBuildOptions) => Promise<void>;

/** Plugin for importing modules as globals */
export function importAsGlobalsPlugin(mapping: Record<string, string>): any;

/** Plugin for writing files only if changed */
export function writeIfChanged(): any;

/** Plugin for cleaning output directory based on passed globs */
export function cleanPlugin(options?: CleanPluginOptions): any;

export interface CleanPluginOptions {
    /** Glob patterns to include for cleaning. Default is ['** /*.js', '** /*.js.map', '** /*.css', '** /*.css.map'] */
    globs?: string[];

    /** Whether to log deleted files to console. Default is true */
    logDeletedFiles?: boolean;
}

/** Default options for cleanPlugin */
export const cleanPluginDefaults: CleanPluginOptions;

/** Copies files from node_modules to outdir (wwwroot/npm by default). Paths are relative to node_modules. */
export function npmCopy(paths: string[], outdir?: string): void;