import typescript from '@rollup/plugin-typescript';
import { minify } from "terser";
import fs from 'fs';
import { builtinModules } from "module";
import dts from "rollup-plugin-dts";
import { basename, resolve } from "path";

var pkg = JSON.parse(fs.readFileSync('./package.json', 'utf8'));

const external = [
    ...builtinModules,
    ...(pkg.dependencies == null ? [] : Object.keys(pkg.dependencies)),
    ...(pkg.devDependencies == null ? [] : Object.keys(pkg.devDependencies)),
    ...(pkg.peerDependencies == null ? [] : Object.keys(pkg.peerDependencies))
]

var globals = {
    'flatpickr': 'flatpickr',
    'tslib': 'this.window || this',
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
var rxReferenceTypes = /^\/\/\/\s*\<reference\s*types\=\".*\r?\n/gm
var rxModuleAugmentation = /^(declare\s+module\s*['"]([A-Za-z\/@\-]+)['"]\s*\{\r?\n)((^\s+.*\r?\n)*)?\}/gm;

const replaceTypeRef = function(src, name, rep) {
    return src.replace(new RegExp('([<>:,]\\s*)' + name.replace('$', '\\$') + '([^A-Za-z0-9_\\$])', 'g'), (m, p1, p2) => p1 + rep + p2);
}

function moduleToGlobalName(fromModule) {
    return (fromModule == "@serenity-is/sleekgrid" || fromModule.endsWith('./slick') || fromModule == "@serenity-is/corelib/slick") ? 'Slick' : 
        (fromModule == "@serenity-is/corelib/q" || fromModule == './q' || fromModule.endsWith("../q") || fromModule.indexOf('/q/') >= 0) ? "Q" : null;
}

const convertModularToGlobal = (src, ns, isTS) => {
    src = src.replace(/: Event;/g, ': Slick.Event;');
    src = src.replace(/: Event</g, ': Slick.Event<');

    var refTypes = [];
    src = src.replace(rxReferenceTypes, function (match) {
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

    if (ns == 'Slick')
        src = replaceTypeRef(src, 'Event', 'Slick.Event');

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
                    return '    ' + (isTS ? 'export ': '') + s;
                return '    ' + s;
            }).join('\n') + '\n}'
    }

    src = src.replace(/^\r?\n\r?\n/gm, '\n');
    src = src.replace(/^\r?\n\r?\n/gm, '\n');
    src = refTypes.join('') + '\n' + moduleAugmentations.join('') + src + '\n' + globals.join('\n');    
    return src;
}

var toGlobal = function (ns, outFile, isTS) {
    return {
        name: 'toGlobal',
        generateBundle(o, b) {
            for (var fileName of Object.keys(b)) {
				if (b[fileName].code && fileName.indexOf('.bundle.d.ts') >= 0) {
                    var src = b[fileName].code;
                    src = convertModularToGlobal(src, ns, isTS);
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
                    src = src.replace(/^(\s*)exports\.([A-Za-z_]+)\s*=\s*(.+?);/gm, function (match, grp1, grp2, grp3) {
                        if (grp2.charAt(0) == '_' && grp2.charAt(1) == '_')
                            return grp1 + "exports." + grp2 + " = exports." + grp2 + " || " + grp3 + ";";
                        return grp1 + "exports." + grp2 + " = exports." + grp2 + " || {}; extend(exports." + grp2 + ", " + grp3 + ");";
                    });
                    b[fileName].code = src;
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
                file: './out/Serenity.CoreLib.js',
                format: "iife",
                sourcemap: true,
                sourcemapExcludeSources: false,
                name: "window",
                extend: true,
                freeze: false,
                banner: fs.readFileSync('./node_modules/tslib/tslib.js',
                    'utf8').replace(/^\uFEFF/, '') + '\n',
                globals
            }
        ],
        plugins: [
            typescript({
                tsconfig: 'tsconfig.json',
                outDir: './out',
                sourceRoot: resolve('./corelib'),
                exclude: ["**/*.spec.ts", "**/*.spec.tsx"],
            }),
            extendGlobals()
        ],
        external
    },
    {
        input: "./out/q/index.d.ts",
        output: [{
            file: "./out/q/index.bundle.d.ts"
        }],
        plugins: [
            dts(), 
            toGlobal('Q')
        ]
    },
    {
        input: "./out/slick/index.d.ts",
        output: [{ 
            file: "./out/slick/index.bundle.d.ts",
            format: "es"
        }],
        plugins: [
            dts(), 
            toGlobal('Slick')
        ],
        external: ['../q', '../../q', '@serenity-is/corelib/q', ...external]
    },
    {
        input: "./out/index.d.ts",
        output: [{ 
            file: "./out/index.bundle.d.ts",
            format: "es"
        }],
        plugins: [
            dts(),
            toGlobal('Serenity'),
            {
                name: 'writeFinal',
                writeBundle: {
                    sequential: true,
                    order: 'post',
                    async handler({ dir }) {
                        // inject tslib
                        dtsOutputs.splice(0, 0, fs.readFileSync('./node_modules/tslib/tslib.d.ts',
                            'utf8').replace(/^\uFEFF/, '').replace(/^[ \t]*export declare/gm, 'declare'));
                        // inject sleekgrid typings after q
                        dtsOutputs.splice(2, 0, convertModularToGlobal(fs.readFileSync("./node_modules/@serenity-is/sleekgrid/dist/index.d.ts").toString(), 'Slick'));

                        var src = dtsOutputs.join('\n').replace(/\r/g, '');
                        src = mergeRefTypes(src);
                    
                        fs.writeFileSync('./out/Serenity.CoreLib.d.ts', src);
                        await minifyScript('./out/Serenity.CoreLib.js');
                        !fs.existsSync('./dist') && fs.mkdirSync('./dist');
                        !fs.existsSync('./dist/q') && fs.mkdirSync('./dist/q');
                        fs.copyFileSync('./out/q/index.bundle.d.ts', './dist/q/index.d.ts');
                        !fs.existsSync('./dist/slick') && fs.mkdirSync('./dist/slick');
                        fs.copyFileSync('./out/slick/index.bundle.d.ts', './dist/slick/index.d.ts');
                        fs.copyFileSync('./out/index.bundle.d.ts', './dist/index.d.ts');

                        const wwwroot = '../../src/Serenity.Scripts/wwwroot';
                        fs.copyFileSync('./out/Serenity.CoreLib.min.js', `${wwwroot}/Serenity.CoreLib.min.js`);
                        fs.copyFileSync('./out/Serenity.CoreLib.min.js.map', `${wwwroot}/Serenity.CoreLib.min.js.map`);
                        fs.copyFileSync('./out/Serenity.CoreLib.d.ts', `${wwwroot}/Serenity.CoreLib.d.ts`);
                        fs.copyFileSync('./out/Serenity.CoreLib.js', `${wwwroot}/Serenity.CoreLib.js`);
                        fs.copyFileSync('./out/Serenity.CoreLib.js.map', `${wwwroot}/Serenity.CoreLib.js.map`);
                    }
                }
            }
        ],
        external: ['./q', '../q', '../../q', './slick', '../../slick', '../slick', '@serenity-is/corelib/q', '@serenity-is/corelib/slick', ...external]
    }
];