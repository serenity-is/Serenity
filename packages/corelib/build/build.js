import esbuild from "esbuild";
import { existsSync, mkdirSync, readFileSync, writeFileSync } from "fs";
import { dirname, join, resolve } from "path";
import { fileURLToPath } from 'url';

const root = resolve(join(fileURLToPath(new URL('.', import.meta.url)), '../'));

const coreLibBase = {
    absWorkingDir: resolve(root),
    bundle: true,
    color: true,
    chunkNames: 'chunks/[name]-[hash]',
    entryPoints: [
        'src/index.ts'
    ],
    keepNames: true,
    logLevel: 'info',
    minify: false,
    outbase: 'src',
    sourcemap: true,
    splitting: false,
    target: 'es2015'
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

function writeIfChanged() {
    return {
        name: "write-if-changed",
        setup(build) {
            const write = build.initialOptions.write;
            build.initialOptions.write = false;
            build.onEnd(result => {
                if (!(write === undefined || write))
                    return;
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

await esbuild.build({
    ...coreLibBase,
    external: ['@serenity-is/sleekgrid'],
    format: 'esm',
    outdir: 'dist',
    sourcemap: false
});

const corelibGlobalBase = {
    ...coreLibBase,
    banner: {
        js: 'var Slick = Slick || {};'
    },
    footer: {
        js: `(function (me) {
    if (!me.Q)
        me.Q = me.Serenity;
    else if (me.Q !== me.Serenity) {
        Object.keys(me.Q).forEach(function(key) {
            if (me.Q[key] != null &&
                me.Serenity[key] == null) {
                me.Serenity[key] = me.Q[key];
            }
        });
        me.Q = me.Serenity;
    }
    me.Slick = me.Slick || {};
    ['Aggregators', 'AggregateFormatting'].forEach(function(x) {
        me.Slick[x] = me.Slick[x] || {};
        Object.assign(me.Slick[x], Serenity[x]);
    });
    ['RemoteView'].forEach(function(x) {
        me.Slick[x] = Serenity[x];
    });
})(this);`
    },
    format: 'iife',
    globalName: 'Serenity',
    outfile: 'wwwroot/index.global.js'
};

await esbuild.build({
    ...corelibGlobalBase,
    minify: false,
    outfile: 'wwwroot/index.global.js',
    plugins: [importAsGlobalsPlugin({
        "@serenity-is/sleekgrid": "Slick"
    }), writeIfChanged()]
});

await esbuild.build({
    ...corelibGlobalBase,
    lineLimit: 1000,
    minify: true,
    outfile: 'wwwroot/index.global.min.js',
    plugins: [importAsGlobalsPlugin({
        "@serenity-is/sleekgrid": "Slick"
    }), writeIfChanged()]
});