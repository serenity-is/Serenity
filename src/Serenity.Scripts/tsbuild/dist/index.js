import esbuild from "esbuild";
import { existsSync, readdirSync, statSync, mkdirSync, writeFileSync, rmSync } from "fs";
import { join, relative, resolve } from "path";
import { exit } from "process";

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
    
    var entryPointsRegEx = /(Page|ScriptInit)\.ts$/;
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
    if (entryPoints === undefined) {
        entryPoints = [];
        entryPointRoots.forEach(root => 
            scanDir(root)
                .filter(p => p.match(entryPointsRegEx))
                .forEach(p => entryPoints.push(root + '/' + p)));
    }
    
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
        sourcemap: true,
        splitting: !process.argv.slice(2).some(x => x == "--nosplit"),
        target: 'es6',
        watch: process.argv.slice(2).some(x => x == "--watch"),
        plugins: [
            cleanPlugin(),
            importAsGlobalsPlugin(opt.importAsGlobalsMapping ?? importAsGlobalsMapping)
        ]
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
                return { path: args.path, namespace: "external-global" };
            });

            build.onLoad({ filter, namespace: "external-global" },
                async (args) => {
                    const global = mapping[args.path];
                    return { contents: `module.exports = ${global};`, loader: "js" };
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
                        if (!outputFiles.has(join(build.initialOptions.outdir, file).replace(/\\/g, '/')))
                            rmSync(join(build.initialOptions.outdir, file));
                    });
                } catch (e) {
                    console.error(`esbuild clean: ${e}`);
                }
            });
        }
    }
}