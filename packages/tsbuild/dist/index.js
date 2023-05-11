import esbuild from "esbuild";
import { existsSync, readdirSync, statSync, mkdirSync, writeFileSync, rmSync, readFileSync } from "fs";
import { join, relative, resolve } from "path";
import { exit } from "process";
import { globSync } from "glob";

export function checkIfTrigger() {
    if (process.argv.slice(2).some(x => x == "--trigger")) {
        if (existsSync('typings/serenity.corelib/_trigger.ts'))
            rmSync('typings/serenity.corelib/_trigger.ts')
        else {
            if (!existsSync('typings/serenity.corelib/'))
                mkdirSync('typings/serenity.corelib/');
            writeFileSync('typings/serenity.corelib/_trigger.ts', '// for triggering build');
        }
        exit(0);
    }
}

export const importAsGlobalsMapping = {
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
        var globs;
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
            }
        }

        if (globs == null && !entryPointsRegEx) {
            globs = ['Modules/**/*Page.ts', 'Modules/**/*Page.tsx', 'Modules/**/ScriptInit.ts'];
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

    delete opt.clean;
    delete opt.importAsGlobals;

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
        target: 'es6',
        watch: process.argv.slice(2).some(x => x == "--watch"),
    }, opt);
}

export const build = async (opt) => {
    var opt = esbuildOptions(opt);

    if (opt.watch) {
        // this somehow resolves the issue that when debugging is stopped
        // in Visual Studio, the node process stays alive
        setInterval(() => {
            process.stdout.write("");
        }, 5000);
    }

    await esbuild.build(esbuildOptions(opt));
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
                        if (!file.endsWith('.js') && !file.endsWith('.js.map'))
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