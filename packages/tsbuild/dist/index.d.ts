export const importAsGlobalsMapping: Record<string, string>;

export interface TSBuildOptions {
    /** Enable bundling of dependencies, default is true */
    bundle?: boolean;

    /** Chunk names to generate when code splitting is enabled. Default is '_chunks/[name]-[hash]' */
    chunkNames?: string[];

    /** True to enable the clean plugin. Default is true if splitting is true. */
    clean?: boolean;

    /**
     * Determines the set of entry points that should be passed to the esbuild. 
     * Only use to specify full paths of entry points manually if you calculated them yourself.
     * Prefer specifying entry point globs in sergen.json under TSBuild:EntryPoints which 
     * defaults to ['Modules/** /*Page.ts', 'Modules/** /*Page.tsx', 'Modules/** /ScriptInit.ts'] */
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

    /** Base directory for calculating output file locations in output directory. Default is "./" */
    outbase?: string;

    /** Base output directory. Default is wwwroot/esm */
    outdir?: boolean;

    /** True to enable code splitting. Default is true unless --nosplit is passed in process arguments. */
    splitting?: boolean;

    /** Set of plugins for esbuild */
    plugins?: any[];

    /** Should source maps be generated. Default is true. */
    sourcemap?: boolean;

    /* Javascript target for output files. Default is es6. */
    target?: string;

    /** True to watch, default is calculated from process args and true if it contains --watch */
    watch?: boolean;
}

/** Processes passed options and converts it to options suitable for esbuild */
export const esbuildOptions: (opt: TSBuildOptions) => any;
export const build: (opt: TSBuildOptions) => Promise<void>;
export function importAsGlobalsPlugin(mapping: Record<string, string>): any;
export function cleanPlugin(): any;