import esbuild from "esbuild";
import { existsSync, mkdirSync, readFileSync, rmSync, writeFileSync } from "fs";
import { globSync } from "glob";
import { dirname, join, resolve } from "path";

export const defaultEntryPointGlobs = ['Modules/**/*Page.ts', 'Modules/**/*Page.tsx', 'Modules/**/ScriptInit.ts', 'Modules/**/*.mts'];

export const importAsGlobalsMapping = {
    "@serenity-is/corelib": "Serenity",
    "@serenity-is/domwise": "Serenity",
    "@serenity-is/domwise/jsx-runtime": "Serenity",
    "@serenity-is/extensions": "Serenity",
    "@serenity-is/pro.extensions": "Serenity",
    "@serenity-is/sleekgrid": "Serenity"
}

export const esbuildDefaults = {
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
    splitting: true,
    sourceRoot: "Modules",
    target: 'es2017'
}

export const esbuildOptions = (opt) => {

    opt = Object.assign({}, opt);

    var entryPointsRegEx;
    if (opt.entryPointsRegEx !== undefined) {
        entryPointsRegEx = opt.entryPointsRegEx;
        delete opt.entryPointsRegEx;
    }

    var entryPointRoots = ['Modules'];
    if (opt.entryPointRoots !== undefined) {
        entryPointRoots = opt.entryPointRoots;
        delete opt.entryPointRoots;
    }


    var entryPoints = opt.entryPoints;
    if (entryPoints === void 0) {
        let globs;
        if (existsSync('sergen.json')) {
            var json = readFileSync('sergen.json', 'utf8').trim();
            var cfg = JSON.parse(json || {});
            globs = cfg?.TSBuild?.EntryPoints;
            if (globs != null && globs[0] === '+') {
                globs = [...defaultEntryPointGlobs, ...globs.slice(1)];
            }
            if (globs === void 0 &&
                typeof cfg.Extends == "string" &&
                existsSync(cfg.Extends)) {
                json = readFileSync(cfg.Extends, 'utf8').trim();
                cfg = JSON.parse(json || {});
                globs = cfg?.TSBuild?.EntryPoints;
                if (globs != null && globs[0] === '+') {
                    globs = [...defaultEntryPointGlobs, ...globs.slice(1)];
                }
            }
        }

        if (globs == null && !entryPointsRegEx) {
            globs = defaultEntryPointGlobs;
        }

        if (globs != null) {
            globs = globs.filter(x => x.indexOf("..") < 0);
            var include = globs.filter(x => !x.startsWith('!'));
            var exclude = globs.filter(x => x.startsWith('!')).map(x => x.substring(1));
            exclude.push(".git/**");
            exclude.push("App_Data/**");
            exclude.push("bin/**");
            exclude.push("obj/**");
            exclude.push("node_modules/**");
            exclude.push("**/node_modules/**");

            entryPoints = globSync(include, {
                ignore: exclude,
                nodir: true,
                matchBase: true
            });
        }
        else {
            entryPoints = [];

            function scanDir(dir, org) {
                return readdirSync(dir).reduce((files, file) => {
                    const absolute = join(dir, file);
                    return [...files, ...(statSync(absolute).isDirectory()
                        ? scanDir(absolute, org || dir)
                        : [relative(org || dir, absolute)])]
                }, []);
            }

            entryPointRoots.forEach(root =>
                scanDir(root)
                    .filter(p => p.match(entryPointsRegEx))
                    .forEach(p => entryPoints.push(root + '/' + p)));
        }
    }

    var splitting = opt.splitting;
    if (splitting === undefined)
        splitting = !process.argv.slice(2).some(x => x == "--nosplit");

    var plugins = opt.plugins;
    if (plugins === undefined) {
        plugins = [];
        if ((opt.clean === undefined && splitting) || opt.clean)
            plugins.push(cleanPlugin(opt.clean === true ? {} : opt.clean));
        if (opt.importAsGlobals === undefined || opt.importAsGlobals)
            plugins.push(importAsGlobalsPlugin(opt.importAsGlobals ?? importAsGlobalsMapping));
    }

    if (opt.write === undefined && opt.writeIfChanged === undefined || opt.writeIfChanged) {
        plugins.push(writeIfChanged());
        opt.write = false;
    }

    delete opt.clean;
    delete opt.importAsGlobals;
    delete opt.writeIfChanged;

    return {
        ...esbuildDefaults,
        absWorkingDir: resolve('./'),
        entryPoints,
        plugins,
        splitting,
        watch: process.argv.slice(2).some(x => x == "--watch"),
        ...opt
    };
}

export const build = async (opt) => {
    if (opt?.npmCopy !== false &&
        existsSync('appsettings.bundles.json')) {
        const bundlesJson = readFileSync('appsettings.bundles.json', 'utf8').trim();
        const bundlesCfg = JSON.parse(bundlesJson || {});
        const bundles = Object.values(bundlesCfg?.CssBundling?.Bundles || {}).concat(Object.values(bundlesCfg?.ScriptBundling?.Bundles || {}));
        let paths = [];
        Object.values(bundles).filter(x => x?.length).forEach(bundle => {
            paths.push(...bundle.filter(f => f?.startsWith('~/npm/')).map(f => f.substring(5)));
        });
        paths = paths.filter((v, i, a) => a.indexOf(v) === i); // unique
        if (paths.length) {
            npmCopy(paths);
        }
    }

    delete opt?.npmCopy;

    opt = esbuildOptions(opt);

    if (opt.watch) {
        // this somehow resolves the issue that when debugging is stopped
        // in Visual Studio, the node process stays alive
        setInterval(() => {
            process.stdout.write("");
        }, 5000);

        delete opt.watch;
        const context = await esbuild.context(opt);
        await context.watch();
    }
    else {
        delete opt.watch;
        await esbuild.build(opt);
    }
};

// https://github.com/evanw/esbuild/issues/337
export function importAsGlobalsPlugin(mapping) {
    const escRe = (s) => s.replace(/[-\/\\^$*+?.()|[\]{}]/g, "\\$&");
    const filter = new RegExp(Object.keys(mapping).map((mod) =>
        `^${escRe(mod)}$`).join("|"));

    return {
        name: "global-imports",
        setup(build) {
            build.onResolve({ filter }, (args) => {
                if (!mapping[args.path])
                    throw new Error("Unknown global: " + args.path);
                return { path: mapping[args.path], namespace: "external-global" };
            });

            build.onLoad({ filter: /.*/, namespace: "external-global" },
                async (args) => {
                    return { contents: `module.exports = ${args.path};`, loader: "js" };
                });
        }
    };
}

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

export function cleanPlugin(opt) {
    opt = Object.assign({}, cleanPluginDefaults, opt ?? {});
    return {
        name: 'clean',
        setup(build) {
            build.onEnd(result => {
                try {
                    const outdir = build.initialOptions.outdir;
                    const { outputs } = result.metafile ?? {};
                    if (!outputs || !existsSync(outdir))
                        return;

                    const outputFiles = new Set(Object.keys(outputs).map(x => x.replace(/\\/g, '/')));
                    const globs = opt.globs.filter(x => x.indexOf("..") < 0);

                    const existingFiles = globSync(globs.filter(x => !x.startsWith('!')), {
                        cwd: build.initialOptions.outdir,
                        ignore: globs.filter(x => x.startsWith('!')).map(x => x.substring(1)),
                        nodir: true,
                        matchBase: true
                    }).map(x => join(outdir, x).replace(/\\/g, '/'));

                    existingFiles.forEach(file => {
                        if (!outputFiles.has(file)) {
                            if (opt.logDeletedFiles ?? true)
                                console.log('esbuild clean: deleting extra file ' + file);
                            rmSync(file);
                        }
                    });
                } catch (e) {
                    console.error(`esbuild clean: ${e}`);
                }
            });
        }
    }
}

export function checkIfTrigger() {
    // nop
}

export function writeIfChanged() {
    return {
        name: "write-if-changed",
        setup(build) {
            build.onEnd(result => {
                result.outputFiles?.forEach(file => {
                    if (existsSync(file.path)) {
                        const old = readFileSync(file.path);
                        if (old.equals(file.contents))
                            return;
                    }
                    else {
                        mkdirSync(dirname(file.path), { recursive: true });
                    }
                    writeFileSync(file.path, file.contents);
                });
            });
        }
    };
}

export function npmCopy(paths) {
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