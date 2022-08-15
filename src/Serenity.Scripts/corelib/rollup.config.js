import typescript from '@rollup/plugin-typescript';
import { minify } from "terser";
import fs from 'fs';
import pkg from "./package.json";
import { builtinModules } from "module";
import dts from "rollup-plugin-dts";

var globals = {
    'jquery': '$',
    'flatpickr': 'flatpickr',
    'tslib': 'tslib',
    '@serenity-is/sleekgrid': 'this.Slick = this.Slick || {}'
}

var dtsOutputs = [];

var rxNamespace = /^namespace\s?/;
var rxExport = /^export\s?/;
var rxDeclare = /^declare\s?/;
var rxClassTypeIntfEnum = /^(class|type|interface|enum)\s?/;
var rxDeclareGlobal = /^(declare\s+global.*\s*\{\r?\n)((^\s+.*\r?\n)*)?\}/gm;
var rxRemoveExports = /^export\s*\{[^\}]*\}\s*\;\s*\r?\n/gm;
var rxRemoveImports = /^\s*import\s*\{([\sA-Za-z0-9_,\$]*)\}\s*from\s*['"](.*)['"]\s*\;\s*\r?\n/gm;
var rxReferenceTypes = /^\/\/\/\s*\<reference\s*types\=\".*\r?\n/gm
var rxModuleAugmentation = /^(declare\s+module\s*['"]([A-Za-z\/@\-]+)['"]\s*\{\r?\n)((^\s+.*\r?\n)*)?\}/gm;

const replaceTypeRef = function(src, name, rep) {
    return src.replace(new RegExp('(>|:|,)\\s*' + name.replace('$', '\\$') + '([^A-Za-z0-9_\\$])', 'g'), (m, p1, p2) => p1 + ' ' + rep + p2);
}

var toGlobal = function (ns, outFile, isTS) {
    return {
        name: 'toGlobal',
        generateBundle(o, b) {
            for (var fileName of Object.keys(b)) {
				if (b[fileName].code && fileName.indexOf('.bundle.d.ts') >= 0) {
                    var src = b[fileName].code;

                    src = src.replace(/: Event;/g, ': Slick.Event;');
                    src = src.replace(/: Event</g, ': Slick.Event<');

                    var refTypes = [];
                    src = src.replace(rxReferenceTypes, function (match) {
                        refTypes.push(match);
                        return '';
                    });

                    src = src.replace(/\r/g, '');
                    src = src.replace(rxRemoveExports, '');

                    var imports = {};

                    src = src.replace(rxRemoveImports, function (match, p1, p2) {
                        var g = p2 == "@serenity-is/sleekgrid" || p2.endsWith('./slick') ? 'Slick' : 
                            (p2 == './q' || p2.endsWith("../q") || p2.indexOf('/q/') >= 0) ? "Q" : null;

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
                                if (i.alias.indexOf('$') >= 0)
                                    debugger;
                                src = replaceTypeRef(src, i.alias, k + '.' + i.name);
                            }
                        }
                    }

                    if (ns == 'Slick')
                        src = replaceTypeRef(src, 'Event', 'Slick.Event');

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
                                if (rxDeclare.test(s))
                                    return '    ' + (isTS ? 'export ' : '') + s.substring(8);
                                else if (rxClassTypeIntfEnum.test(s))
                                    return '    ' + (isTS ? 'export ': '') + s;
                                else
                                    return '    ' + s;
                            }).join('\n') + '\n}'
                    }

                    src = src.replace(/^\r?\n\r?\n/gm, '\n');
                    src = src.replace(/^\r?\n\r?\n/gm, '\n');
                    src = refTypes.join('') + '\n' + moduleAugmentations.join('') + src + '\n' + globals.join('\n');

                    if (outFile) {
                        if (outFile.indexOf('Slick') >= 0)
                            src = src.replace(/SlickEvent/g, )
                        fs.writeFileSync(outFile, src);
                    }
                    else {
                        dtsOutputs.push(src);

                        var toEmit = {
                            type: 'asset',
                            fileName: fileName.replace('.bundle.d.ts', '.global.d.ts'),
                            source: src
                        };
                        this.emitFile(toEmit);
                    }
                }
            }
        }
    }
}

var extendGlobals = function () {
    return {
        name: 'extendGlobals',
        generateBundle(o, b) {
            for (var fileName of Object.keys(b)) {
                var code = b[fileName].code || b[fileName].source;

                if (code && fileName.indexOf('.js') >= 0) {
                    var src = code;
                    src = fs.readFileSync('./node_modules/tslib/tslib.js',
                        'utf8').replace(/^\uFEFF/, '') + '\n' + src;
                    src = src.replace(/^(\s*)exports\.([A-Za-z_]+)\s*=\s*(.+?);/gm, function (match, grp1, grp2, grp3) {
                        if (grp2.charAt(0) == '_' && grp2.charAt(1) == '_')
                            return grp1 + "exports." + grp2 + " = exports." + grp2 + " || " + grp3 + ";";
                        return grp1 + "exports." + grp2 + " = exports." + grp2 + " || {}; extend(exports." + grp2 + ", " + grp3 + ");";
                    });
                    src = src.replace(/,\s*tslib/g, '');
                    src = src.replace(/tslib\.__/g, '__');
                    b[fileName].code = src;
                }
                else if (code && /(Globals\/.*|Globals|Globals\..*)\.d\.ts$/i.test(fileName) && code.indexOf('declare global') < 0) {
                    dtsOutputs.push(code);
                }
            }
        }
    }
}

var writeMinJS = function () {
    return {
        name: 'writeMinJS',
        generateBundle: async function (o, b) {
            var self = this;
            Object.keys(b).forEach(async function (fileName) {
                if (b[fileName].code && /\.js$/i.test(fileName) >= 0) {
                    var src = b[fileName].code;
                    var minified = await minify(src, {
                        mangle: true,
                        format: {
                            beautify: false,
                            max_line_len: 1000
                        }
                    });
                    var toEmit = {
                        type: 'asset',
                        fileName: fileName.replace(/\.js$/, '.min.js'),
                        source: minified.code
                    };
                    self.emitFile(toEmit);
                }
            });
        }
    }
}

const external = [
    ...builtinModules,
    ...(pkg.dependencies == null ? [] : Object.keys(pkg.dependencies)),
    ...(pkg.devDependencies == null ? [] : Object.keys(pkg.devDependencies)),
    ...(pkg.peerDependencies == null ? [] : Object.keys(pkg.peerDependencies))
]

const mergeRefTypes = (src) => {
    var refTypes = [];
    src = src.replace(rxReferenceTypes, function (match, offset, str) {
        refTypes.push(match);
        return '';
    });
    refTypes = refTypes.filter((x, i) => refTypes.indexOf(x) == i);
    src = refTypes.join('') + '\n' + src
    return src;
}

export default [
    {
        input: "src/corelib.ts",
        output: [
            {
                file: '../dist/Serenity.CoreLib.js',
                format: "iife",
                sourcemap: true,
                name: "window",
                extend: true,
                freeze: false,
                globals
            }
        ],
        plugins: [
            typescript({
                tsconfig: 'src/tsconfig.json',
                outDir: '../dist'
            }),
            extendGlobals(),
            writeMinJS()
        ],
        external
    },
    {
        input: "../dist/q/index.d.ts",
        output: [{
            file: "../dist/q/index.bundle.d.ts"
        }],
        plugins: [
            dts(), 
            toGlobal('Q')
        ]
    },
    {
        input: "./node_modules/@serenity-is/sleekgrid/src/index.ts",
        output: [{ 
            file: "../dist/sleekgrid/index.js",
            format: "iife",
            name: "Slick",
            extend: true,
            freeze: false,
            globals
        }],
        plugins: [
            typescript({
                tsconfig: './node_modules/@serenity-is/sleekgrid/src/tsconfig.json',
                outDir: '../dist/sleekgrid'
            })
        ]
    },
    {
        input: "../dist/sleekgrid/index.d.ts",
        output: [{ 
            file: "../dist/sleekgrid/index.bundle.d.ts", 
            format: "es"
        }],
        plugins: [
            dts(), 
            toGlobal('Slick')
        ]
    },    
    {
        input: "../dist/slick/index.d.ts",
        output: [{ 
            file: "../dist/slick/index.bundle.d.ts",
            format: "es"
        }],
        plugins: [
            dts(), 
            toGlobal('Slick')
        ],
        external: ['../q', ...external]
    },
    {
        input: "../dist/serenity/index.d.ts",
        output: [{ 
            file: "../dist/serenity/index.bundle.d.ts", 
            format: "es"
        }],
        plugins: [
            dts(), 
            toGlobal('Serenity'),
            {
                name: 'writeFinal',
                generateBundle: function () {
                    dtsOutputs.splice(0, 0, fs.readFileSync('./node_modules/tslib/tslib.d.ts',
                        'utf8').replace(/^\uFEFF/, '').replace(/^[ \t]*export declare/gm, 'declare'));

                    var src = dtsOutputs.join('\n').replace(/\r/g, '');
                    src = mergeRefTypes(src);

                    fs.writeFileSync('../dist/Serenity.CoreLib.d.ts', src);
                    fs.copyFileSync('../dist/Serenity.CoreLib.d.ts', '../wwwroot/Serenity.CoreLib.d.ts');
                    fs.copyFileSync('../dist/Serenity.CoreLib.js', '../wwwroot/Serenity.CoreLib.js');
                    fs.copyFileSync('../dist/Serenity.CoreLib.js.map', '../wwwroot/Serenity.CoreLib.js.map');
                    fs.copyFileSync('../dist/Serenity.CoreLib.min.js', '../wwwroot/Serenity.CoreLib.min.js');
                }
            }
        ],
        external: ['../q', ...external]
    }
];