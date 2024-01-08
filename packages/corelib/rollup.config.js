import { nodeResolve } from '@rollup/plugin-node-resolve';
import typescript from '@rollup/plugin-typescript';
import fs from 'fs';
import { basename, resolve } from "path";
import { dts } from "rollup-plugin-dts";
import { minify } from "terser";
import sourcemapsPlugin from './build/rollup-sourcemaps.js';

var externalPackages = ["@serenity-is/sleekgrid"];

var globals = {
    'flatpickr': 'flatpickr',
    '@serenity-is/sleekgrid': 'this.Slick = this.Slick || {}'
}

var dtsOutputs = [];

var rxNamespace = /^namespace\s?/;
var rxExport = /^export\s?/;
var rxClassTypeIntfEnum = /^(class|type|interface|enum)\s?/;
var rxDeclareGlobal = /^(declare\s+global.*\s*\{\r?\n)((^\s+.*\r?\n)*)?\}/gm;
var rxRemoveExports = /^export\s*\{[^\}]*\}\s*\;\s*\r?\n/gm;
var rxRemoveImports = /^\s*import\s*\{([\sA-Za-z0-9_,\$]*)\}\s*from\s*['"](.*)['"]\s*\;\s*\r?\n/gm;
var rxReExports = /^\s*export\s*\{([\sA-Za-z0-9_,\$]*)\}\s*from\s*['"](.*)['"]\s*\;\s*\r?\n/gm;
var rxModuleAugmentation = /^(declare\s+module\s*['"]([A-Za-z\/@\-]+)['"]\s*\{\r?\n)((^\s+.*\r?\n)*)?\}/gm;
var referenceTypeCommentsRegex = /^\/\/\/\s*\<reference\s*types\=\".*\r?\n/gm

const replaceTypeRef = function (src, name, rep) {
    return src.replace(new RegExp('([<>:,]\\s*)' + name.replace('$', '\\$') + '([^A-Za-z0-9_\\$])', 'g'), (m, p1, p2) => p1 + rep + p2);
}

function moduleToGlobalName(fromModule) {
    return fromModule == "@serenity-is/sleekgrid" ? 'Slick' :
        ((fromModule == "@serenity-is/base" || fromModule == "@serenity-is/base-ui") ? 'Serenity' : null);
}

const convertModularToGlobal = (src, ns, isTS) => {
    src = src.replace('sourceItem?: PropertyItem', 'sourceItem?: Serenity.PropertyItem');

    var refTypes = [];
    src = src.replace(referenceTypeCommentsRegex, function (match) {
        refTypes.push(match);
        return '';
    });

    src = src.replace(/\r/g, '');
    src = src.replace(rxRemoveExports, '');

    src = src.replace(rxReExports, function (match, exportList, fromModule) {
        var g = moduleToGlobalName(fromModule)
        if (!g) {
            return match;
        }

        var list = [];

        for (var part of exportList.split(',')) {
            var x = part.trim();
            if (ns != g && x) {
                var y = x.split(' as ');

                var equalsToType = g + '.' + (y[1] || y[0]).trim();
                var typeAlias = y[0].trim();

                list.push('export import ' + typeAlias + ' = ' + equalsToType + ';')
            }
        }

        return list.join('\n') + '\n';
    });


    var imports = {};

    src = src.replace(rxRemoveImports, function (match, p1, p2) {
        var g = moduleToGlobalName(p2);

        if (!g) {
            return match;
        }

        imports[g] = imports[g] || [];
        for (var part of p1.split(',')) {
            var x = part.trim();
            if (ns != g && x) {
                var y = x.split(' as ');
                imports[g].push({ alias: (y[1] || y[0]).trim(), name: y[0].trim() });
            }
        }
        return '';
    });

    for (var k in imports) {
        if (k != ns) {
            for (var i of imports[k]) {
                src = replaceTypeRef(src, i.alias, k + '.' + i.name);
            }
        }
    }

    if (ns == 'Serenity') {
        src = src.replace(': typeof executeOnceWhenVisible', ': typeof Q.executeOnceWhenVisible');
        src = src.replace(': typeof executeEverytimeWhenVisible', ': typeof Q.executeEverytimeWhenVisible');
    }

    var moduleAugmentations = [];
    src = src.replace(rxModuleAugmentation, function (match, p1, p2, p3, p4, offset, str) {
        if (p2 == '@serenity-is/sleekgrid')
            moduleAugmentations.push('declare namespace Slick {\n' + p3 + '}\n\n');
        else
            moduleAugmentations.push(match);
        return '';
    });

    var globals = [];
    src = src.replace(rxDeclareGlobal, function (x, y, m1) {
        if (!m1)
            return '';
        var g = m1.replace(/(\r?\n)*$/, '').split('\n')
            .map(s => s.length > 0 && s.charAt(0) == '\t' ? s.substring(1) : (s.substring(0, 4) == '    ' ? s.substring(4) : s))
            .map(s => {
                if (rxNamespace.test(s))
                    return ("declare " + s);
                else if (rxExport.test(s))
                    return "declare " + s.substring(7);
                else
                    return s;
            })
            .join('\n');
        globals.push(g);
        return '';
    });

    if (ns) {
        src = 'declare namespace ' + ns + ' {\n' +
            src.replace(/(\r?\n)*$/, '').split('\n').map(s => {
                if (!s.length)
                    return '';
                if (/^declare\s+/.test(s))
                    return '    ' + (isTS ? 'export ' : '') + s.substring(8);
                if (/^export\sdeclare\s+/.test(s))
                    return '    ' + (isTS ? 'export ' : '') + s.substring(15);
                if (/^export\sinterface\s+/.test(s))
                    return '    ' + (isTS ? 'export ' : '') + s.substring(7);
                if (rxClassTypeIntfEnum.test(s))
                    return '    ' + (isTS ? 'export ' : '') + s;
                return '    ' + s;
            }).join('\n') + '\n}'
    }

    for (var i = 0; i < 2; i++)
        src = src.replace(/^\r?\n\r?\n/gm, '\n');
    src = refTypes.join('') + '\n' + moduleAugmentations.join('') + src + '\n' + globals.join('\n');
    return src;
}

var toGlobal = function (ns, outFile, isTS) {
    return {
        name: 'toGlobal',
        generateBundle(o, b) {
            for (var fileName of Object.keys(b)) {
                if (b[fileName].code && fileName.indexOf('bundle.d.ts') >= 0) {
                    var src = b[fileName].code;
                    src = convertModularToGlobal(src, ns, isTS);
                    if (outFile) {
                        fs.writeFileSync(outFile, src);
                    }
                    else {
                        dtsOutputs.push(src);

                        var toEmit = {
                            type: 'asset',
                            fileName: fileName.replace('bundle.d.ts', 'bundle.global.d.ts'),
                            source: src
                        };
                        this.emitFile(toEmit);
                    }
                }
            }
        }
    }
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

const mergeReferenceTypeComments = (src) => {
    var refTypes = [];
    src = src.replace(referenceTypeCommentsRegex, function (match, offset, str) {
        refTypes.push(match);
        return '';
    });
    refTypes = refTypes.filter((x, i) => refTypes.indexOf(x) == i);
    src = refTypes.join('') + '\n' + src
    return src;
}

const nodeResolvePlugin = () => nodeResolve({
    resolveOnly: ['@serenity-is/base', '@serenity-is/base-ui']
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
            toGlobal('Serenity'),
            {
                name: 'writeFinal',
                writeBundle: {
                    sequential: true,
                    order: 'post',
                    async handler({ dir }) {
                        dtsOutputs.splice(0, 0, convertModularToGlobal(fs.readFileSync("./node_modules/@serenity-is/sleekgrid/dist/index.d.ts").toString(), 'Slick'));
                        dtsOutputs.push(`
import Q = Serenity;

declare namespace Slick {
    namespace Data {
        /** @obsolete use the type exported from @serenity-is/sleekgrid */
        export import GroupItemMetadataProvider = Slick.GroupItemMetadataProvider;
    }
    export import AggregateFormatting = Serenity.AggregateFormatting;
    export import Aggregators = Serenity.Aggregators;
    export import CancellableViewCallback = Serenity.CancellableViewCallback;
    export import Formatter = Serenity.Formatter;
    export import GroupInfo = Serenity.GroupInfo;   
    export import PagerOptions = Serenity.PagerOptions;
    export import PagingInfo = Serenity.PagingInfo;
    export import PagingOptions = Serenity.PagingOptions;
    export import RemoteView = Serenity.RemoteView;
    export import RemoteViewAjaxCallback = Serenity.RemoteViewAjaxCallback;
    export import RemoteViewFilter = Serenity.RemoteViewFilter;
    export import RemoteViewOptions = Serenity.RemoteViewOptions;
    export import RemoteViewProcessCallback = Serenity.RemoteViewProcessCallback;
    export import SummaryOptions = Serenity.SummaryOptions;
}
`);

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
                        var src = dtsOutputs.join('\n').replace(/\r/g, '');
                        src = mergeReferenceTypeComments(src);
                        writeIfDifferent('./wwwroot/index.global.d.ts', src);
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