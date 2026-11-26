import { init as esmlInit, parse as esmlParse } from "es-module-lexer";
import esbuild from "esbuild";
import { createReadStream, createWriteStream, existsSync, mkdirSync, readFileSync, rmSync, writeFileSync } from "fs";
import { globSync } from "tinyglobby";
import pipe from "node:stream/promises";
import { constants, createBrotliCompress, createGzip } from "node:zlib";
import { dirname, isAbsolute, join, relative, resolve, sep } from "path";

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
        ignore: excludes,
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

    // by default only for an output directory under an esm folder (e.g. wwwroot/esm), where the access control
    // looks for it, not e.g. for the dist/ bundle of an npm package
    const isEsmOutdir = /(^|[\\/])esm([\\/]|$)/i.test(opt.outdir ?? tsbuildDefaults.outdir);
    if (opt.esmManifest ?? ((opt.format == null || opt.format === "esm") && opt.metafile !== false && isEsmOutdir))
        plugins.push(esmManifestPlugin());

    delete opt.compress;
    delete opt.esmManifest;
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

let exitOnStdinCloseEnabled = false;

// NodeScriptRunner sets the variable, and holds the other end of the stdin pipe, so that watchers
// don't outlive the application even if it is terminated abruptly. It is opt-in, as otherwise
// watching with an empty or closed stdin, e.g. a background job, would exit immediately.
function exitOnStdinClose() {
    if (exitOnStdinCloseEnabled || process.env.SERENITY_EXIT_ON_STDIN_CLOSE !== "1")
        return;
    exitOnStdinCloseEnabled = true;
    const exit = () => process.exit(0);
    process.stdin.on("end", exit);
    process.stdin.on("error", exit);
    process.stdin.resume();
}

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
        exitOnStdinClose();
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

const serenityModuleTest = {
    "@serenity-is/corelib": "PropertyGrid",
    "@serenity-is/domwise": "observeSignal",
    "@serenity-is/domwise/jsx-runtime": "jsx",
    "@serenity-is/extensions": "GridEditorBase",
    "@serenity-is/pro.extensions": "EnhancedLayout",
    "@serenity-is/sleekgrid": "SleekGrid"
}

export function importAsGlobalsPlugin(mapping) {
    const escRe = (s) => s.replace(/[-\/\\^$*+?.()|[\]{}]/g, "\\$&");
    const filter = new RegExp(Object.keys(mapping).map((mod) => `^${escRe(mod)}$`).join("|"));
    let serenityModuleContents;
    return {
        name: "global-imports",
        setup(build) {
            build.onResolve({ filter }, (args) => {
                if (args.namespace === "skip-external-global") {
                    return;
                }
                if (!mapping[args.path])
                    throw new Error("Unknown global: " + args.path);
                return { path: mapping[args.path], namespace: "external-global" };
            });
            build.onLoad(
                { filter: /.*/, namespace: "external-global" },
                async (args) => {
                    if (args.path === "Serenity") {
                        if (serenityModuleContents === void 0) {
                            const initOpt = build?.initialOptions || {};
                            const absWorkingDir = initOpt.absWorkingDir || process.cwd();
                            const allNames = new Set();
                            const nameByModule = {};
                            const moduleTest = {};
                            try {
                                await esmlInit;
                                for (const [mod, ns] of Object.entries(mapping)) {
                                    if (ns === "Serenity") {
                                        const resResult = await build.resolve(mod, {
                                            resolveDir: absWorkingDir,
                                            kind: "import-statement",
                                            namespace: "skip-external-global"
                                        });
                                        if (resResult?.errors?.length || !resResult?.path)
                                            continue;

                                        let path = resResult.path;
                                        if (resResult.external) {
                                            path = null;
                                            let currentPath = absWorkingDir;
                                            const actualMod = mod == "@serenity-is/domwise/jsx-runtime" ? "@serenity-is/domwise" : mod;
                                            const actualFile = mod == "@serenity-is/domwise/jsx-runtime" ? "dist/jsx-runtime.js" : "dist/index.js";
                                            let tries = 0;
                                            while (tries++ < 100) {
                                                const nmPath = join(currentPath, "node_modules", actualMod, actualFile);
                                                if (existsSync(nmPath)) {
                                                    path = nmPath;
                                                    break;
                                                }
                                                currentPath = dirname(currentPath);
                                                if (!currentPath.length)
                                                    break;
                                            }
                                        }
                                        if (!path)
                                            continue;
                                        const source = readFileSync(path, "utf8");
                                        const [_, exports] = esmlParse(source);
                                        nameByModule[mod] ??= new Set();
                                        if (!moduleTest[mod] && serenityModuleTest[mod]) {
                                            moduleTest[mod] = serenityModuleTest[mod];
                                        }
                                        for (const exp of exports) {
                                            if (exp.n !== "default" && exp.n && exp.n.length > 0) {
                                                allNames.add(exp.n);
                                                nameByModule[mod].add(exp.n);
                                            }
                                        }
                                    }
                                }

                                if (allNames.size > 0) {
                                    const exportNames = Array.from(allNames).sort().join(", ");
                                    const sb = [];
                                    sb.push(`if (typeof Serenity === "undefined") globalThis.Serenity = Object.create(null);`);
                                    if (initOpt.format === "esm" && Object.keys(moduleTest).length > 0) {
                                        sb.push(`
const t = ${JSON.stringify(moduleTest)};
for (const [k, v] of Object.entries(t)) {
    if (v in Serenity) continue;
    try {
        const m = await import(k);
        for (const n of Object.keys(m)) {
            if (!(n in Serenity)) Serenity[n] = m[n];
        }
    } catch (e) { console.warn(e); }
}`);
                                    }

                                    sb.push(`const { ${exportNames} } = Serenity;`);
                                    sb.push(`export { ${exportNames} };`);
                                    serenityModuleContents = sb.join("\n");
                                }
                            }
                            catch (e) {
                                console.warn(`esbuild external-globals plugin: failed to parse exports for global import`, e);
                                serenityModuleContents = null;
                                // continue with commonjs export
                            }
                        }

                        if (serenityModuleContents) {
                            return {
                                contents: serenityModuleContents,
                                loader: "js"
                            };
                        }
                    }

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
export function cleanPlugin(opt) {
    opt = Object.assign({}, cleanPluginDefaults, opt ?? {});
    return {
        name: "clean",
        setup(build2) {
            build2.onEnd((result) => {
                try {
                    // this runs on every rebuild while watching, when the current directory
                    // might not be the one the build started from, so don't depend on it
                    const workDir = build2.initialOptions.absWorkingDir ?? process.cwd();
                    const outdir = resolve(workDir, build2.initialOptions.outdir);
                    const { outputs } = result.metafile ?? {};
                    if (!outputs || !existsSync(outdir))
                        return;
                    const normalize = (x) => x.replace(/\\/g, "/");
                    // metafile output paths are relative to the working directory
                    const outputFiles = new Set(Object.keys(outputs).map((x) => normalize(resolve(workDir, x))));
                    const existingFiles = safeGlobSync(opt.globs || [], {
                        root: outdir
                    }).map((x) => normalize(join(outdir, x)));
                    existingFiles.forEach((file) => {
                        if (!outputFiles.has(file)) {
                            if (opt.logDeletedFiles ?? true)
                                console.log(`esbuild clean: \x1B[33mdeleting extra file ${normalize(relative(workDir, file))}\x1B[0m`);
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

export const esmManifestFileName = "build.esm-manifest";

/**
 * Builds the ESM manifest from an esbuild metafile: the entry points, and for each output file under
 * the output directory, the indexes of the entry points that reach it via static or dynamic imports.
 * A source map has the same entries as the file it belongs to. Files no entry point reaches have an
 * empty list. Paths are relative to the output directory, with / separators. Pass the entryPoints of
 * the build to tell the entry points from dynamically imported modules more precisely.
 */
export function buildEsmManifest(metafile, outdir, workDir, entryPoints) {
    const normalize = (x) => x.replace(/\\/g, "/");
    const outputs = metafile?.outputs ?? {};
    const absOut = normalize(resolve(workDir, outdir));
    const toRelative = (key) => {
        const abs = normalize(resolve(workDir, key));
        return abs.startsWith(absOut + "/") ? abs.substring(absOut.length + 1) : null;
    };

    const keys = Object.keys(outputs);
    // the css bundle of a js entry point may also have its entryPoint set, but it is not a separate entry
    const cssBundles = new Set(keys.map((key) => outputs[key].cssBundle).filter((x) => x));
    // dynamically imported modules are implicit entry points with their entryPoint set, but they
    // are not entries, they belong to the entries that import them
    let isEntry;
    if (entryPoints?.length) {
        const explicit = new Set(entryPoints.map((x) => typeof x === "string" ? x : x?.in).filter((x) => x)
            .map((x) => normalize(resolve(workDir, x))));
        isEntry = (key) => explicit.has(normalize(resolve(workDir, outputs[key].entryPoint)));
    } else {
        const dynamicTargets = new Set();
        const staticTargets = new Set();
        for (const key of keys) {
            for (const imp of outputs[key].imports ?? [])
                (imp.kind === "dynamic-import" ? dynamicTargets : staticTargets).add(imp.path);
        }
        isEntry = (key) => !dynamicTargets.has(key) || staticTargets.has(key);
    }
    const entryKeys = keys.filter((key) => outputs[key].entryPoint && !key.endsWith(".map") &&
        !cssBundles.has(key) && toRelative(key) != null && isEntry(key)).sort();
    const reachedBy = new Map(keys.map((key) => [key, new Set()]));

    entryKeys.forEach((entryKey, index) => {
        const stack = [entryKey];
        const visited = new Set();
        while (stack.length) {
            const key = stack.pop();
            if (visited.has(key))
                continue;
            visited.add(key);
            reachedBy.get(key)?.add(index);
            const output = outputs[key];
            if (!output)
                continue;
            for (const imp of output.imports ?? []) {
                if (!imp.external && imp.path && outputs[imp.path])
                    stack.push(imp.path);
            }
            if (output.cssBundle && outputs[output.cssBundle])
                stack.push(output.cssBundle);
        }
    });

    const entries = [];
    for (const key of entryKeys) {
        const rel = toRelative(key);
        if (rel != null)
            entries.push(rel);
    }

    const files = {};
    for (const key of [...keys].sort()) {
        const rel = toRelative(key);
        if (rel == null)
            continue;
        const owner = key.endsWith(".map") && outputs[key.substring(0, key.length - 4)] ? key.substring(0, key.length - 4) : key;
        files[rel] = [...(reachedBy.get(owner) ?? [])].sort((a, b) => a - b);
    }

    return { version: 1, entries, files };
}

/**
 * Plugin that writes build.esm-manifest to the output directory, which the server side ESM access
 * control uses to decide which files an entry point (e.g. a page) needs. Written only if changed.
 */
export function esmManifestPlugin() {
    return {
        name: "esm-manifest",
        setup(build2) {
            build2.onEnd((result) => {
                try {
                    if (!result.metafile || result.errors?.length)
                        return;
                    const workDir = build2.initialOptions.absWorkingDir ?? process.cwd();
                    const outdir = build2.initialOptions.outdir;
                    if (!outdir)
                        return;
                    const entryPoints = Array.isArray(build2.initialOptions.entryPoints) ? build2.initialOptions.entryPoints : null;
                    const manifest = buildEsmManifest(result.metafile, outdir, workDir, entryPoints);
                    const absOut = resolve(workDir, outdir);
                    const file = join(absOut, esmManifestFileName);
                    const content = JSON.stringify(manifest);
                    if (existsSync(file) && readFileSync(file, "utf8") === content)
                        return;
                    mkdirSync(absOut, { recursive: true });
                    writeFileSync(file, content);
                } catch (e) {
                    console.error(`esm manifest: ${e}`);
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
