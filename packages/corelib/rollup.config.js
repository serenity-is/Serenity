import { nodeResolve } from '@rollup/plugin-node-resolve';
import { basename, resolve } from "path";
import { dts } from "rollup-plugin-dts";
import { minify } from "terser";
import fs from 'fs';
import sourcemapsPlugin from './build/rollup-sourcemaps.js';
import typescript from '@rollup/plugin-typescript';

var externalPackages = ["@serenity-is/sleekgrid"];

var globals = {
    'flatpickr': 'flatpickr',
    '@serenity-is/sleekgrid': 'this.Slick = this.Slick || {}'
}

async function minifyScript(fileName) {
    var minified = await minify({ [basename(fileName)]: fs.readFileSync(fileName, 'utf8') }, {
        mangle: true,
        sourceMap: {
            content: fs.existsSync(fileName + '.map') ? fs.readFileSync(fileName + '.map', 'utf8') : undefined,
            filename: fileName.replace(/\.js$/, '.min.js').replace(/\.\/out\//g, ''),
            url: fileName.replace(/\.js$/, '.min.js.map').replace(/\.\/out\//g, '')
        },
        format: {
            beautify: false,
            max_line_len: 1000
        }
    });
    fs.writeFileSync(fileName.replace(/\.js$/, '.min.js'), minified.code);
    fs.writeFileSync(fileName.replace(/\.js$/, '.min.js.map'), minified.map);
}

const nodeResolvePlugin = () => nodeResolve({
    resolveOnly: ['@serenity-is/base', '@serenity-is/base-ui', 'jsx-dom']
});

export default [
    {
        input: "src/index.ts",
        output: [
            {
                file: './out/index.global.js',
                format: "iife",
                sourcemap: true,
                sourcemapExcludeSources: false,
                name: "Serenity",
                generatedCode: 'es2015',
                extend: true,
                freeze: false,
                footer: `(function (me) {
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
})(this);`,
                globals
            }
        ],
        plugins: [
            nodeResolvePlugin(),
            typescript({
                tsconfig: 'tsconfig.json',
                resolveJsonModule: true,
                outDir: './out',
                sourceRoot: resolve('./corelib'),
                exclude: ["**/*.spec.ts", "**/*.spec.tsx"],
            }),
            sourcemapsPlugin()
        ],
        external: externalPackages
    },
    {
        input: "./out/index.d.ts",
        output: [{
            file: "./out/index.bundle.d.ts",
            format: "es"
        }],
        plugins: [
            nodeResolvePlugin(),
            dts({
                respectExternal: true
            }),
            {
                name: 'writeFinal',
                writeBundle: {
                    sequential: true,
                    order: 'post',
                    async handler({ dir }) {
                        function writeIfDifferent(target, content) {
                            if (!fs.existsSync(target) ||
                                fs.readFileSync(target, 'utf8') != content) {
                                fs.writeFileSync(target, content);
                            }
                        }

                        function copyIfDifferent(source, target) {
                            writeIfDifferent(target, fs.readFileSync(source, 'utf8'));
                        }

                        !fs.existsSync('./dist') && fs.mkdirSync('./dist');
                        copyIfDifferent('./out/index.bundle.d.ts', './dist/index.d.ts');
                        !fs.existsSync('./wwwroot') && fs.mkdirSync('./wwwroot');
                        copyIfDifferent('./out/index.global.js', './wwwroot/index.global.js');
                        copyIfDifferent('./out/index.global.js.map', './wwwroot/index.global.js.map');
                        await minifyScript('./out/index.global.js');
                        copyIfDifferent('./out/index.global.min.js', './wwwroot/index.global.min.js');
                        copyIfDifferent('./out/index.global.min.js.map', './wwwroot/index.global.min.js.map');
                    }
                }
            }
        ],
        external: externalPackages
    }
];