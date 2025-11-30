import esbuild from "esbuild";
import { type GlobOptions } from "glob";
export declare const defaultEntryPointGlobs: string[];
/**
 * Default mapping for importing modules as globals. corelib, domwise, extensions, pro.extensions, sleekgrid
 * are all mapped to Serenity global namespace */
export declare const importAsGlobalsMapping: Record<string, string>;
export declare function safeGlobSync(globs: string[], options?: Omit<GlobOptions, "ignore">): string[];
/** Default esbuild options used by TSBuild */
export declare const esbuildDefaults: Partial<import("esbuild").BuildOptions>;
export interface TSBuildOptions extends Partial<import("esbuild").BuildOptions> {
    /** Enable bundling of dependencies, default is true */
    bundle?: boolean;
    /** Chunk names to generate when code splitting is enabled. Default is '_chunks/[name]-[hash]' */
    chunkNames?: string;
    /** True to enable the clean plugin. Default is true if splitting is true. */
    clean?: boolean | CleanPluginOptions;
    /** Options for compressing output files. If specified, enables compression. Currently only available when writeIfChanged plugin is enabled */
    compress?: CompressOptions;
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
    target?: string;
    /** True to watch, default is calculated from process args and true if it contains --watch */
    watch?: boolean;
    /** Write output files only if contents have changed. Default is true. */
    writeIfChanged?: boolean;
}
/** Processes passed TSBuildOptions options and converts it to options suitable for esbuild */
export declare const esbuildOptions: (opt: TSBuildOptions) => import("esbuild").BuildOptions;
/** Calls esbuild with passed options. By default, this is used to generate files under wwwroot/esm/ from entry points under Modules/
 * but this can be changed by passing outdir and outbase, and other options. */
export declare const build: (opt: TSBuildOptions) => Promise<void>;
/** Plugin for importing modules as globals */
export declare function importAsGlobalsPlugin(mapping: Record<string, string>): {
    name: string;
    setup(build: esbuild.PluginBuild): void;
};
/** Default options for cleanPlugin */
export declare const cleanPluginDefaults: {
    globs: string[];
    logDeletedFiles: boolean;
};
/** Options for cleanPlugin */
export interface CleanPluginOptions {
    /** Glob patterns to include for cleaning. Default is ['** /*.js', '** /*.js.map', '** /*.css', '** /*.css.map'] */
    globs?: string[];
    /** Whether to log deleted files to console. Default is true */
    logDeletedFiles?: boolean;
}
/** Plugin for cleaning output directory based on passed globs */
export declare function cleanPlugin(opt: CleanPluginOptions): {
    name: string;
    setup(build: esbuild.PluginBuild): void;
};
export interface CompressOptions {
    brotli?: boolean | {
        quality?: number;
    };
    gzip?: boolean | {
        level?: number;
    };
    extensions?: string[];
}
/** Plugin for writing files only if changed */
export declare function writeIfChanged(opt?: CompressOptions): {
    name: string;
    setup(build: esbuild.PluginBuild): void;
};
/** Copies files from node_modules to outdir (wwwroot/npm by default). Paths are relative to node_modules. */
export declare function npmCopy(paths: string[]): void;
