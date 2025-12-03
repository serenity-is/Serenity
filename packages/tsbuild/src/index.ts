import esbuild from "esbuild";
// @ts-ignore
import { existsSync, mkdirSync, readFileSync, rmSync, writeFileSync } from "fs";
import { globSync, type GlobOptions } from "glob";
// @ts-ignore
import { constants, createBrotliCompress, createGzip } from "node:zlib";
// @ts-ignore
import { dirname, isAbsolute, join, resolve, sep } from "path";
// @ts-ignore
import pipe from "node:stream/promises"
// @ts-ignore
import fs from "fs";

export const defaultEntryPointGlobs = ['Modules/**/*Page.ts', 'Modules/**/*Page.tsx', 'Modules/**/ScriptInit.ts', 'Modules/**/*.mts'];

/**
 * Default mapping for importing modules as globals. corelib, domwise, extensions, pro.extensions, sleekgrid 
 * are all mapped to Serenity global namespace */
export const importAsGlobalsMapping: Record<string, string> = {
    "@serenity-is/corelib": "Serenity",
    "@serenity-is/domwise": "Serenity",
    "@serenity-is/domwise/jsx-runtime": "Serenity",
    "@serenity-is/extensions": "Serenity",
    "@serenity-is/pro.extensions": "Serenity",
    "@serenity-is/sleekgrid": "Serenity"
}

export function safeGlobSync(globs: string[], options: Omit<GlobOptions, "ignore"> = {}): string[] {
    // @ts-ignore
    const root = options.root || process.cwd();

    const normalizePattern = (pattern: string) => {
        let normalized = pattern.replace(/\\/g, '/');

        if (normalized.startsWith('/')) {
            // anchored to root
            normalized = '.' + normalized;
        } else if (!normalized.includes('/')) {
            // no slashes, match anywhere
            normalized = '**/' + normalized;
        }

        // check for invalid patterns
        if (isAbsolute(normalized) || normalized.includes('..') || /^[a-zA-Z]:/.test(normalized)) {
            throw new Error(`Invalid pattern: ${pattern}`);
        }

        return normalized;
    };

    globs = globs.map(x => x?.trim()).filter(x => x.length > 0);
    const includes = globs.filter(x => !x.startsWith('!')).map(normalizePattern);
    const excludes = globs.filter(x => x.startsWith('!')).map(x => normalizePattern(x.substring(1)));

    const results = globSync(includes, {
        nodir: true,
        ignore: excludes,
        matchBase: true,
        cwd: root,
        ...options
    });

    // filter results to ensure under root
    const resolvedRoot = resolve(root);
    return (results as string[]).filter((file: string) => {
        const fullPath = resolve(root, file);
        return fullPath.startsWith(resolvedRoot + sep) || fullPath === resolvedRoot;
    });
}

/** Default esbuild options used by TSBuild */
export const tsbuildDefaults: Partial<import("esbuild").BuildOptions> = {
    assetNames: 'assets/[name]-[hash]',
    bundle: true,
    chunkNames: '_chunks/[name]-[hash]',
    color: true,
    format: 'esm',
    jsxSideEffects: true,
    keepNames: true,
    loader: {
        '.woff2': 'file',
        '.woff': 'file',
        '.ttf': 'file',
        '.eot': 'file',
        '.svg': 'file',
        '.png': 'file',
        '.jpg': 'file',
        '.jpeg': 'file',
        '.gif': 'file',
        '.webp': 'file',
    },
    logLevel: 'info',
    metafile: true,
    minify: true,
    outbase: "./",
    outdir: 'wwwroot/esm',
    sourcemap: true,
    target: 'es2017'
}

export interface TSBuildOptions extends Partial<import("esbuild").BuildOptions> {
    /** Enable building of global iife bundles from Modules/Common/esm/bundles/*-bundle(.css|.ts) files to wwwroot/esm/bundles/. Default is false.
      * If set to an object, uses the passed options for building global bundles. */
    buildGlobalBundles?: boolean | TSBuildOptions;

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
    importAsGlobals?: Record<string, string> | null;

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

    /* Javascript target for output files. Default is es2017. */
    target?: string;

    /** True to watch, default is calculated from process args and true if it contains --watch */
    watch?: boolean;

    /** Write output files only if contents have changed. Default is true. */
    writeIfChanged?: boolean;
}

function isSplittingEnabled(opt: TSBuildOptions): boolean {
    if (opt.splitting !== undefined) {
        return !!opt.splitting;
    }
    return (opt.format == null || opt.format === 'esm') && 
        !((globalThis as any).process.argv as string[]).slice(2).some(x => x == "--nosplit");
}

function cleanPluginOptions(opt: TSBuildOptions): CleanPluginOptions | null {
    if (opt.plugins === undefined)
        return null;

    if ((opt.clean === undefined && isSplittingEnabled(opt)) || opt.clean)
        return opt.clean === true ? {} : (opt.clean ?? {})

    return null;
}

/** Processes passed TSBuildOptions options and converts it to options suitable for esbuild */
export const esbuildOptions = (opt: TSBuildOptions): import("esbuild").BuildOptions => {

    opt = Object.assign({}, opt);

    if ((opt as any).entryPointsRegex !== undefined ||
        (opt as any).entryPointRoots !== undefined) {
        throw new Error("TSBuildOptions.entryPointsRegex and entryPointRoots are deprecated, use TSBuild:EntryPoints in sergen.json.");
    }

    let entryPoints = opt.entryPoints as string[];
    if (entryPoints === void 0) {
        let globs;
        if (existsSync('sergen.json')) {
            var json = readFileSync('sergen.json', 'utf8').trim();
            var cfg = JSON.parse(json || "{}");
            globs = cfg?.TSBuild?.EntryPoints as string[];
            if (globs != null && globs[0] === '+') {
                globs = [...defaultEntryPointGlobs, ...globs.slice(1)];
            }
            if (globs === void 0 &&
                typeof cfg.Extends == "string" &&
                existsSync(cfg.Extends)) {
                json = readFileSync(cfg.Extends, 'utf8').trim();
                cfg = JSON.parse(json || "{}");
                globs = cfg?.TSBuild?.EntryPoints as string[];
                if (globs != null && globs[0] === '+') {
                    globs = [...defaultEntryPointGlobs, ...globs.slice(1)];
                }
            }
        }

        if (globs == null) {
            globs = defaultEntryPointGlobs;
        }

        if (globs != null) {
            globs.push("!.git/**");
            globs.push("!App_Data/**");
            globs.push("!bin/**");
            globs.push("!obj/**");
            globs.push("!node_modules/**");

            entryPoints = safeGlobSync(globs);
        }
    }

    const splitting = isSplittingEnabled(opt);

    let plugins = opt.plugins;
    if (plugins === undefined) {
        plugins = [];
        const cleanOpt = cleanPluginOptions(opt);
        if (cleanOpt != null)
            plugins.push(cleanPlugin(cleanOpt));
        if (opt.importAsGlobals === undefined || opt.importAsGlobals)
            plugins.push(importAsGlobalsPlugin(opt.importAsGlobals ?? importAsGlobalsMapping));
    }

    if ((opt.write === undefined && opt.writeIfChanged === undefined) || opt.writeIfChanged) {
        plugins.push(writeIfChanged(opt.compress));
        opt.write = false;
    }

    delete opt.compress;
    delete opt.clean;
    delete opt.importAsGlobals;
    delete opt.writeIfChanged;

    if (opt.sourceRoot === undefined) {
        if (existsSync('package.json')) {
            let pkgId = JSON.parse(readFileSync('package.json', 'utf8').trim() || "{}").name;
            if (pkgId.startsWith("@serenity-is/")) {
                opt.sourceRoot = "https://packages.serenity.is/" + pkgId.substring(13) + "/Modules/";
            }
        }
        opt.sourceRoot ??= "Modules";
    }

    return {
        ...tsbuildDefaults,
        absWorkingDir: resolve('./'),
        entryPoints,
        plugins,
        splitting,
        // @ts-ignore
        watch: process.argv.slice(2).some(x => x == "--watch"),
        ...opt
    };
}

/** Default options for global iife bundle builds which is used when buildGlobalBundles is true or is an object */
export const tsbuildGlobalBundleDefaults: Partial<TSBuildOptions> = {
    entryPoints: [
        "Modules/Common/bundles/*-bundle.ts",
        "Modules/Common/bundles/*-bundle.css",
        "Modules/Common/bundles/*-bundle.rtl.css",
    ],
    format: "iife",
    importAsGlobals: null,
    outdir: "wwwroot/esm/bundles/",
    outbase: "Modules/Common/bundles",
    watch: false,
}

/** Calls esbuild with passed options. By default, this is used to generate files under wwwroot/esm/ from entry points under Modules/
 * but this can be changed by passing outdir and outbase, and other options. */
export const build = async (opt: TSBuildOptions) => {
    if (opt.buildGlobalBundles) {
        const buildGlobalBundles: TSBuildOptions = {
            ...tsbuildGlobalBundleDefaults,
            ...(opt.buildGlobalBundles === true ? {} : opt.buildGlobalBundles)
        };
        delete opt.buildGlobalBundles;
        console.log("\x1b[32mBuilding global bundles...\x1b[0m");
        await build(buildGlobalBundles);

        let cleanOpt = cleanPluginOptions(opt);
        if (cleanOpt != null) {
            opt.clean = {
                ...cleanOpt,
                globs: [
                    "!./bundles/**",
                    ...cleanOpt.globs ?? cleanPluginDefaults.globs
                ]
            }
        }
    }

    if (opt?.npmCopy !== false &&
        existsSync('appsettings.bundles.json')) {
        const bundlesJson = readFileSync('appsettings.bundles.json', 'utf8').trim();
        const bundlesCfg = JSON.parse(bundlesJson || "{}");
        const bundles: string[][] = (Object.values(bundlesCfg?.CssBundling?.Bundles || {}) as string[][])
            .concat(Object.values(bundlesCfg?.ScriptBundling?.Bundles || {}) as string[][]);
        let paths: string[] = [];
        Object.values(bundles).filter(x => x?.length).forEach(bundle => {
            paths.push(...bundle.filter(f => f?.startsWith('~/npm/')).map(f => f.substring(5)));
        });
        paths = paths.filter((v, i, a) => a.indexOf(v) === i); // unique
        if (paths.length) {
            npmCopy(paths);
        }
    }

    delete opt?.npmCopy;

    const esopt: import("esbuild").BuildOptions = esbuildOptions(opt);

    if ((esopt as any).watch) {
        // this somehow resolves the issue that when debugging is stopped
        // in Visual Studio, the node process stays alive
        setInterval(() => {
            // @ts-ignore
            process.stdout.write("");
        }, 5000);

        delete (esopt as any).watch;
        const context = await esbuild.context(esopt);
        await context.watch();
    }
    else {
        delete (esopt as any).watch;
        await esbuild.build(esopt);
    }
};

// https://github.com/evanw/esbuild/issues/337
/** Plugin for importing modules as globals */
export function importAsGlobalsPlugin(mapping: Record<string, string>) {
    const escRe = (s: string) => s.replace(/[-\/\\^$*+?.()|[\]{}]/g, "\\$&");
    const filter = new RegExp(Object.keys(mapping).map((mod) =>
        `^${escRe(mod)}$`).join("|"));

    return {
        name: "global-imports",
        setup(build: esbuild.PluginBuild) {
            build.onResolve({ filter }, (args) => {
                if (!mapping[args.path])
                    throw new Error("Unknown global: " + args.path);
                return { path: mapping[args.path], namespace: "external-global" };
            });

            build.onLoad({ filter: /.*/, namespace: "external-global" },
                async (args: esbuild.OnLoadArgs) => {
                    return { contents: `module.exports = ${args.path};`, loader: "js" };
                });
        }
    };
}

/** Default options for cleanPlugin */
export const cleanPluginDefaults = {
    globs: [
        '*.css',
        '*.css.map',
        '*.js',
        '*.js.map',
        '*.jpg',
        '*.png',
        '*.gif',
        '*.svg',
        '*.woff',
        '*.woff2',
        '*.ttf',
        '*.eot'
    ],
    logDeletedFiles: true
}

/** Options for cleanPlugin */
export interface CleanPluginOptions {
    /** Glob patterns to include for cleaning. Default is ['** /*.js', '** /*.js.map', '** /*.css', '** /*.css.map'] */
    globs?: string[];

    /** Whether to log deleted files to console. Default is true */
    logDeletedFiles?: boolean;
}

/** Plugin for cleaning output directory based on passed globs */
export function cleanPlugin(opt: CleanPluginOptions) {
    opt = Object.assign({}, cleanPluginDefaults, opt ?? {});
    return {
        name: 'clean',
        setup(build: esbuild.PluginBuild) {
            build.onEnd(result => {
                try {
                    const outdir = build.initialOptions.outdir as string;
                    const { outputs } = result.metafile ?? {};
                    if (!outputs || !existsSync(outdir))
                        return;

                    const outputFiles = new Set(Object.keys(outputs).map(x => x.replace(/\\/g, '/')));

                    const existingFiles = safeGlobSync(opt.globs || [], {
                        cwd: outdir,
                    }).map(x => join(outdir, x).replace(/\\/g, '/'));

                    existingFiles.forEach(file => {
                        if (!outputFiles.has(file)) {
                            if (opt.logDeletedFiles ?? true)
                                console.log(`esbuild clean: \x1b[33mdeleting extra file ${file}\x1b[0m`);
                            rmSync(file);
                            if (existsSync(file + ".gz")) {
                                rmSync(file + ".gz");
                            }
                            if (existsSync(file + ".br")) {
                                rmSync(file + ".br");
                            }
                        }
                    });
                } catch (e) {
                    console.error(`esbuild clean: ${e}`);
                }
            });
        }
    }
}

export interface CompressOptions {
    brotli?: boolean | {
        quality?: number;
    },
    gzip?: boolean | {
        level?: number;
    },
    extensions?: string[];
}

/** Plugin for writing files only if changed */
export function writeIfChanged(opt?: CompressOptions) {
    const compressExtensions = opt?.extensions ?? ['.css', '.js', '.svg', '.json'];

    return {
        name: "write-if-changed",
        setup(build: esbuild.PluginBuild) {
            build.onEnd(async result => {
                const start = new Date().getTime();
                let compressed: number = 0;
                let written: number = 0;
                let checkedFiles: number = 0;
                let outputFiles = result.outputFiles || [];
                for (const file of outputFiles) {

                    checkedFiles++;
                    const compressFile = async () => {
                        if ((opt?.brotli || opt?.gzip) &&
                            compressExtensions.some(ext => file.path?.endsWith(ext))) {
                            if (opt.brotli) {
                                await pipe.pipeline(
                                    fs.createReadStream(file.path),
                                    createBrotliCompress({
                                        [constants.BROTLI_PARAM_QUALITY]: (typeof opt.brotli === "object" && opt.brotli.quality) || 4
                                    }),
                                    fs.createWriteStream(`${file.path}.br`));
                                compressed++;
                            }
                            if (opt.gzip) {
                                await pipe.pipeline(
                                    fs.createReadStream(file.path),
                                    createGzip({ level: (typeof opt.gzip === "object" && opt.gzip.level) || 7 }),
                                    fs.createWriteStream(`${file.path}.gz`));
                                compressed++;
                            }
                        }
                    };

                    if (existsSync(file.path)) {
                        const old = readFileSync(file.path);
                        if (old.equals(file.contents)) {
                            if ((opt?.brotli && !existsSync(file.path + '.br')) ||
                                (opt?.gzip && !existsSync(file.path + '.gz'))) {
                                await compressFile();
                            }
                            continue;
                        }
                    }
                    else {
                        mkdirSync(dirname(file.path), { recursive: true });
                    }
                    writeFileSync(file.path, file.contents);
                    written++;
                    await compressFile();
                }

                const end = new Date().getTime();
                if (written > 0 || compressed > 0)
                    console.log(`esbuild write: \x1b[32mChecked ${checkedFiles} output files in ${end - start} ms, changed ${written}, compressed ${compressed}\x1b[0m`);
                else
                    console.log(`esbuild write: \x1b[32mChecked ${checkedFiles} output files in ${end - start} ms, none changed\x1b[0m`);

                await Promise.resolve();
            });
        }
    };
}

/** Copies files from node_modules to outdir (wwwroot/npm by default). Paths are relative to node_modules. */
export function npmCopy(paths: string[]) {
    paths.forEach(path => {
        const srcFile = join("node_modules", path);
        const dstfile = join("wwwroot/npm", path);
        if (!existsSync(srcFile)) {
            console.warn(`Source file not found: ${srcFile}`);
            return;
        }

        (function () {
            const srcContent = readFileSync(srcFile);
            if (existsSync(dstfile)) {
                if (readFileSync(dstfile).equals(srcContent))
                    return;
            }
            else {
                mkdirSync(dirname(dstfile), { recursive: true });
            }
            console.log(`Copying ${srcFile} to ${dstfile}`);
            writeFileSync(dstfile, srcContent);
        })();

        const js = path.endsWith(".js");
        if ((js && !path.endsWith(".min.js")) ||
            (path.endsWith(".css") && !path.endsWith(".min.css"))) {
            const ext = js ? ".min.js" : ".min.css";
            const srcMinFile = srcFile.substring(0, srcFile.length - (js ? 3 : 4)) + ext;
            const dstMinFile = dstfile.substring(0, dstfile.length - (js ? 3 : 4)) + ext;
            if (existsSync(srcMinFile)) {
                const srcMinContent = readFileSync(srcMinFile);
                if (existsSync(dstMinFile) && readFileSync(dstMinFile).equals(srcMinContent))
                    return;
                console.log(`Copying ${srcMinFile} to ${dstMinFile}`);
                writeFileSync(dstMinFile, srcMinContent);
            }
        }
    });
}