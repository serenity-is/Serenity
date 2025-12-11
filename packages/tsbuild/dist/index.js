import esbuild from "esbuild";
import { createReadStream, createWriteStream, existsSync, mkdirSync, readFileSync, rmSync, writeFileSync } from "fs";
import { globSync } from "glob";
import pipe from "node:stream/promises";
import { constants, createBrotliCompress, createGzip } from "node:zlib";
import { dirname, isAbsolute, join, resolve, sep } from "path";

export const defaultEntryPointGlobs = [
    "Modules/**/*Page.ts",
    "Modules/**/*Page.tsx",
    "Modules/**/ScriptInit.ts",
    "Modules/**/*.mts"
];

export const importAsGlobalsMapping = {
    "@serenity-is/corelib": "Serenity",
    "@serenity-is/domwise": "Serenity",
    "@serenity-is/domwise/jsx-runtime": "Serenity",
    "@serenity-is/extensions": "Serenity",
    "@serenity-is/pro.extensions": "Serenity",
    "@serenity-is/sleekgrid": "Serenity"
};

export function safeGlobSync(globs, options = {}) {
    const root = options.root || process.cwd();
    const normalizePattern = (pattern) => {
        let normalized = pattern.replace(/\\/g, "/");
        if (normalized.startsWith("/")) {
            normalized = "." + normalized;
        } else if (!normalized.includes("/")) {
            normalized = "**/" + normalized;
        }
        if (isAbsolute(normalized) || normalized.includes("..") || /^[a-zA-Z]:/.test(normalized)) {
            throw new Error(`Invalid pattern: ${pattern}`);
        }
        return normalized;
    };
    globs = globs.map((x) => x?.trim()).filter((x) => x.length > 0);
    const includes = globs.filter((x) => !x.startsWith("!")).map(normalizePattern);
    const excludes = globs.filter((x) => x.startsWith("!")).map((x) => normalizePattern(x.substring(1)));
    const results = globSync(includes, {
        nodir: true,
        ignore: excludes,
        matchBase: true,
        cwd: root,
        ...options
    });
    const resolvedRoot = resolve(root);
    return results.filter((file) => {
        const fullPath = resolve(root, file);
        return fullPath.startsWith(resolvedRoot + sep) || fullPath === resolvedRoot;
    });
}

export const tsbuildDefaults = {
    assetNames: "assets/[name]-[hash]",
    bundle: true,
    chunkNames: "_chunks/[name]-[hash]",
    color: true,
    format: "esm",
    jsxSideEffects: true,
    keepNames: true,
    lineLimit: 1000,
    loader: {
        ".woff2": "file",
        ".woff": "file",
        ".ttf": "file",
        ".eot": "file",
        ".svg": "file",
        ".png": "file",
        ".jpg": "file",
        ".jpeg": "file",
        ".gif": "file",
        ".webp": "file"
    },
    logLevel: "info",
    logOverride: {
        "empty-glob": "silent"
    },
    metafile: true,
    minify: true,
    outbase: "./",
    outdir: "wwwroot/esm",
    sourcemap: true,
    target: "es2022"
};

function isSplittingEnabled(opt) {
    if (opt.splitting !== void 0) {
        return !!opt.splitting;
    }
    return (opt.format == null || opt.format === "esm") && !globalThis.process.argv.slice(2).some((x) => x == "--nosplit");
}

function cleanPluginOptions(opt) {
    if (opt.plugins !== void 0)
        return null;
    if ((opt.clean === void 0 && isSplittingEnabled(opt)) || opt.clean)
        return opt.clean === true ? {} : opt.clean ?? {};
    return null;
}

export const esbuildOptions = (opt) => {
    opt = Object.assign({}, opt);
    if (opt.entryPointsRegex !== void 0 || opt.entryPointRoots !== void 0) {
        throw new Error("TSBuildOptions.entryPointsRegex and entryPointRoots are deprecated, use TSBuild:EntryPoints in sergen.json.");
    }
    let entryPoints = opt.entryPoints;
    if (entryPoints === void 0) {
        let globs;
        if (existsSync("sergen.json")) {
            var json = readFileSync("sergen.json", "utf8").trim();
            var cfg = JSON.parse(json || "{}");
            globs = cfg?.TSBuild?.EntryPoints;
            if (globs != null && globs[0] === "+") {
                globs = [...defaultEntryPointGlobs, ...globs.slice(1)];
            }
            if (globs === void 0 && typeof cfg.Extends == "string" && existsSync(cfg.Extends)) {
                json = readFileSync(cfg.Extends, "utf8").trim();
                cfg = JSON.parse(json || "{}");
                globs = cfg?.TSBuild?.EntryPoints;
                if (globs != null && globs[0] === "+") {
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
    
    if (plugins === void 0) {
        plugins = [];
    
        // clean and import plugins are only added if user didn't provide custom plugins
        const cleanOpt = cleanPluginOptions(opt);
        if (cleanOpt != null)
            plugins.push(cleanPlugin(cleanOpt));
                
        if (opt.importAsGlobals === void 0 || opt.importAsGlobals)
            plugins.push(importAsGlobalsPlugin(opt.importAsGlobals ?? importAsGlobalsMapping));
    }
    else {
        if (opt.clean)
            console.warn("TSBuildOptions.clean is ignored when custom plugins are provided.");
        if (opt.importAsGlobals)
            console.warn("TSBuildOptions.importAsGlobals is ignored when custom plugins are provided.");
    }

    if ((opt.write === void 0 && opt.writeIfChanged === void 0) || opt.writeIfChanged) {
        plugins.push(writeIfChanged(opt.compress));
        opt.write = false;
    }

    delete opt.compress;
    delete opt.clean;
    delete opt.importAsGlobals;
    delete opt.writeIfChanged;
    if (opt.sourceRoot === void 0) {
        if (existsSync("package.json")) {
            let pkgId = JSON.parse(readFileSync("package.json", "utf8").trim() || "{}").name;
            if (pkgId.startsWith("@serenity-is/")) {
                opt.sourceRoot = "https://packages.serenity.is/" + pkgId.substring(13) + "/Modules/";
            }
        }
        opt.sourceRoot ??= "Modules";
    }
    return {
        ...tsbuildDefaults,
        absWorkingDir: resolve("./"),
        entryPoints,
        plugins,
        splitting,
        // @ts-ignore
        watch: process.argv.slice(2).some((x) => x == "--watch"),
        ...opt
    };
};

export const tsbuildGlobalBundleDefaults = {
    entryPoints: [
        "Modules/Common/bundles/*.bundle.ts",
        "Modules/Common/bundles/*-bundle.ts",
        "Modules/Common/bundles/*.bundle.css",
        "Modules/Common/bundles/*-bundle.css",
        "Modules/Common/bundles/*.bundle.rtl.css",
        "Modules/Common/bundles/*-bundle.rtl.css"
    ],
    external: [
        "@serenity-is/tiptap"
    ],
    format: "iife",
    importAsGlobals: null,
    outdir: "wwwroot/esm/bundles/",
    outbase: "Modules/Common/bundles",
    watch: false
};

export const build = async (opt) => {
    if (opt.buildGlobalBundles) {
        const buildGlobalBundles = {
            ...tsbuildGlobalBundleDefaults,
            ...opt.buildGlobalBundles === true ? {} : opt.buildGlobalBundles
        };
        delete opt.buildGlobalBundles;
        console.log("\x1B[32mBuilding global bundles...\x1B[0m");
        await build(buildGlobalBundles);
        let cleanOpt = cleanPluginOptions(opt);
        if (cleanOpt != null) {
            opt.clean = {
                ...cleanOpt,
                globs: [
                    "!./bundles/**",
                    ...cleanOpt.globs ?? cleanPluginDefaults.globs
                ]
            };
        }
    }
    if (opt?.npmCopy !== false && existsSync("appsettings.bundles.json")) {
        const bundlesJson = readFileSync("appsettings.bundles.json", "utf8").trim();
        const bundlesCfg = JSON.parse(bundlesJson || "{}");
        const bundles = Object.values(bundlesCfg?.CssBundling?.Bundles || {}).concat(Object.values(bundlesCfg?.ScriptBundling?.Bundles || {}));
        let paths = [];
        Object.values(bundles).filter((x) => x?.length).forEach((bundle) => {
            paths.push(...bundle.filter((f) => f?.startsWith("~/npm/")).map((f) => f.substring(5)));
        });
        paths = paths.filter((v, i, a) => a.indexOf(v) === i);
        if (paths.length) {
            npmCopy(paths);
        }
    }
    delete opt?.npmCopy;
    const esopt = esbuildOptions(opt);
    if (esopt.watch) {
        setInterval(() => {
            process.stdout.write("");
        }, 5e3);
        delete esopt.watch;
        const context = await esbuild.context(esopt);
        await context.watch();
    } else {
        delete esopt.watch;
        await esbuild.build(esopt);
    }
};

export function importAsGlobalsPlugin(mapping) {
    const escRe = (s) => s.replace(/[-\/\\^$*+?.()|[\]{}]/g, "\\$&");
    const filter = new RegExp(Object.keys(mapping).map((mod) => `^${escRe(mod)}$`).join("|"));
    return {
        name: "global-imports",
        setup(build2) {
            build2.onResolve({ filter }, (args) => {
                if (!mapping[args.path])
                    throw new Error("Unknown global: " + args.path);
                return { path: mapping[args.path], namespace: "external-global" };
            });
            build2.onLoad(
                { filter: /.*/, namespace: "external-global" },
                async (args) => {
                    return { contents: `module.exports = ${args.path};`, loader: "js" };
                }
            );
        }
    };
}

export const cleanPluginDefaults = {
    globs: [
        "*.css",
        "*.css.map",
        "*.js",
        "*.js.map",
        "*.jpg",
        "*.png",
        "*.gif",
        "*.svg",
        "*.woff",
        "*.woff2",
        "*.ttf",
        "*.eot"
    ],
    logDeletedFiles: true
};
function cleanPlugin(opt) {
    opt = Object.assign({}, cleanPluginDefaults, opt ?? {});
    return {
        name: "clean",
        setup(build2) {
            build2.onEnd((result) => {
                try {
                    const outdir = build2.initialOptions.outdir;
                    const { outputs } = result.metafile ?? {};
                    if (!outputs || !existsSync(outdir))
                        return;
                    const outputFiles = new Set(Object.keys(outputs).map((x) => x.replace(/\\/g, "/")));
                    const existingFiles = safeGlobSync(opt.globs || [], {
                        cwd: outdir
                    }).map((x) => join(outdir, x).replace(/\\/g, "/"));
                    existingFiles.forEach((file) => {
                        if (!outputFiles.has(file)) {
                            if (opt.logDeletedFiles ?? true)
                                console.log(`esbuild clean: \x1B[33mdeleting extra file ${file}\x1B[0m`);
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
    };
}

export function writeIfChanged(opt) {
    const compressExtensions = opt?.extensions ?? [".css", ".js", ".svg", ".json"];
    return {
        name: "write-if-changed",
        setup(build2) {
            build2.onEnd(async (result) => {
                const start = (/* @__PURE__ */ new Date()).getTime();
                let compressed = 0;
                let written = 0;
                let checkedFiles = 0;
                let outputFiles = result.outputFiles || [];
                for (const file of outputFiles) {
                    checkedFiles++;
                    const compressFile = async () => {
                        if ((opt?.brotli || opt?.gzip) && compressExtensions.some((ext) => file.path?.endsWith(ext))) {
                            if (opt.brotli) {
                                await pipe.pipeline(
                                    createReadStream(file.path),
                                    createBrotliCompress({
                                        [constants.BROTLI_PARAM_QUALITY]: typeof opt.brotli === "object" && opt.brotli.quality || 4
                                    }),
                                    createWriteStream(`${file.path}.br`)
                                );
                                compressed++;
                            }
                            if (opt.gzip) {
                                await pipe.pipeline(
                                    createReadStream(file.path),
                                    createGzip({ level: typeof opt.gzip === "object" && opt.gzip.level || 7 }),
                                    createWriteStream(`${file.path}.gz`)
                                );
                                compressed++;
                            }
                        }
                    };
                    if (existsSync(file.path)) {
                        const old = readFileSync(file.path);
                        if (old.equals(file.contents)) {
                            if (opt?.brotli && !existsSync(file.path + ".br") || opt?.gzip && !existsSync(file.path + ".gz")) {
                                await compressFile();
                            }
                            continue;
                        }
                    } else {
                        mkdirSync(dirname(file.path), { recursive: true });
                    }
                    writeFileSync(file.path, file.contents);
                    written++;
                    await compressFile();
                }
                const end = (/* @__PURE__ */ new Date()).getTime();
                if (written > 0 || compressed > 0)
                    console.log(`esbuild write: \x1B[32mChecked ${checkedFiles} output files in ${end - start} ms, changed ${written}, compressed ${compressed}\x1B[0m`);
                else
                    console.log(`esbuild write: \x1B[32mChecked ${checkedFiles} output files in ${end - start} ms, none changed\x1B[0m`);
                await Promise.resolve();
            });
        }
    };
}

export function npmCopy(paths) {
    paths.forEach((path) => {
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
            } else {
                mkdirSync(dirname(dstfile), { recursive: true });
            }
            console.log(`Copying ${srcFile} to ${dstfile}`);
            writeFileSync(dstfile, srcContent);
        })();
        const js = path.endsWith(".js");
        if (js && !path.endsWith(".min.js") || path.endsWith(".css") && !path.endsWith(".min.css")) {
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
