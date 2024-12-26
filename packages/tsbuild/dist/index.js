import esbuild from "esbuild";
import { existsSync, readdirSync, statSync, mkdirSync, writeFileSync, rmSync, readFileSync } from "fs";
import { dirname, join, relative, resolve } from "path";
import { globSync } from "glob";

export const defaultEntryPointGlobs = ['Modules/**/*Page.ts', 'Modules/**/*Page.tsx', 'Modules/**/ScriptInit.ts'];

export const importAsGlobalsMapping = {
    "@serenity-is/base": "Serenity",
    "@serenity-is/corelib": "Serenity",
    "@serenity-is/corelib/q": "Q",
    "@serenity-is/corelib/slick": "Slick",
    "@serenity-is/sleekgrid": "Slick",
    "@serenity-is/extensions": "Serenity.Extensions",
    "@serenity-is/pro.extensions": "Serenity"
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
            if (globs === void 0 &&
                cfg.Extends &&
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
            plugins.push(cleanPlugin());
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

    return Object.assign({
        absWorkingDir: resolve('./'),
        bundle: true,
        chunkNames: '_chunks/[name]-[hash]',
        color: true,
        entryPoints: entryPoints,
        format: 'esm',
        keepNames: true,
        logLevel: 'info',
        metafile: true,
        minify: true,
        outbase: "./",
        outdir: 'wwwroot/esm',
        plugins,
        sourcemap: true,
        splitting: splitting,
        target: 'es2017',
        watch: process.argv.slice(2).some(x => x == "--watch"),
    }, opt);
}

export const build = async (opt) => {
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

function scanDir(dir, org) {
    return readdirSync(dir).reduce((files, file) => {
        const absolute = join(dir, file);
        return [...files, ...(statSync(absolute).isDirectory()
            ? scanDir(absolute, org || dir)
            : [relative(org || dir, absolute)])]
    }, []);
}

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

export function cleanPlugin() {
    return {
        name: 'clean',
        setup(build) {
            build.onEnd(result => {
                try {
                    const { outputs } = result.metafile ?? {};
                    if (!outputs || !existsSync(build.initialOptions.outdir))
                        return;

                    const outputFiles = new Set(Object.keys(outputs));
                    scanDir(build.initialOptions.outdir).forEach(file => {
                        if (!file.endsWith('.js') && !file.endsWith('.js.map') && !file.endsWith('.css') && !file.endsWith('.css.map'))
                            return;
                        if (!outputFiles.has(join(build.initialOptions.outdir, file).replace(/\\/g, '/'))) {
                            console.log('esbuild clean: deleting extra file ' + file);
                            rmSync(join(build.initialOptions.outdir, file));
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
                    writeFileSync(file.path, file.text);
                });
            });
        }
    };
}