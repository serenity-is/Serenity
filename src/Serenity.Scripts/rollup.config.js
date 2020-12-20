import typescript from '@rollup/plugin-typescript';
//import {terser} from "rollup-plugin-terser";
import path from 'path';
import fs from 'fs';
import pkg from "./package.json";
import {builtinModules} from "module";
import dts from "rollup-plugin-dts";

var globals = {
	'jquery': '$',
	'flatpickr': 'flatpickr'
}

function normalizePath(fileName) {
    return fileName.split(path.win32.sep).join(path.posix.sep);
}

var outputs = [];

var rxNamespace = /^namespace\s?/;
var rxExport = /^export\s?/;
var rxDeclare = /^declare\s?/;
var rxDeclareGlobal = /^(declare\s+global.*\s*\{\r?\n)((^\s+.*\r?\n)*)?\}/gm;
var rxRemoveExports = /^export\s*\{[^\}]*\}\s*\;\s*\r?\n/gm;
var rxReferenceTypes = /^\/\/\/\s*\<reference\s*types\=\".*\r?\n/gm

var toGlobal = function(ns) {
	return {
		name: 'toGlobal',
		generateBundle(o, b) {
			for (var fileName of Object.keys(b)) {
				if (b[fileName].code && fileName.indexOf('.bundle.d.ts') >= 0) {
					var src = b[fileName].code;
					src = src.replace('\r', '');
					src = src.replace(rxRemoveExports, '');

					var refTypes = [];
					src = src.replace(rxReferenceTypes, function(match, offset, str) {
						refTypes.push(match);
						return '';
					});

					var globals = [];
					src = src.replace(rxDeclareGlobal, function(x, y, m1) {
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
									return '    ' + s.substring(8);
								else
									return '    ' + s;
							}).join('\n') + '\n}'
					}

					src = refTypes.join('') + '\n' + src + '\n' + globals.join('\n');

					outputs.push(src);

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

var extendGlobals = function() {
	return {
		name: 'extendGlobals',
		generateBundle(o, b) {
			for (var fileName of Object.keys(b)) {
				if (b[fileName].code && fileName.indexOf('.js') >= 0) {
					var src = b[fileName].code;
					src = src.replace(/^(\s*)exports\.([A-Za-z_]+)\s*=\s*(.+?);/gm, function(match, grp1, grp2, grp3) {
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

export default [
	{
		input: "CoreLib/CoreLib.ts",
		output: [
			{
				file: 'dist/Serenity.CoreLib.js',
				format: "iife",
				sourcemap: true,
				name: "window",
				extend: true,
				freeze: false,
				globals,
				plugins: [
					extendGlobals()
				]
			}/*,
			{
				file: 'dist/Serenity.CoreLib.min.js',
				format: "iife",
				sourcemap: false,
				name: "window",
				extend: true,
				plugins: [
					terser()
				],
				globals
			}*/
		],
		plugins: [
			typescript({
				tsconfig: 'CoreLib/tsconfig.json',
				outDir: 'CoreLib/built'
			})
		],
		external: [
			...builtinModules,
			...(pkg.dependencies == null ? [] : Object.keys(pkg.dependencies)),
			...(pkg.devDependencies == null ? [] : Object.keys(pkg.devDependencies)),
			...(pkg.peerDependencies == null ? [] : Object.keys(pkg.peerDependencies))
		]
	},
	{
		input: "./dist/built/Q/indexAll.d.ts",
		output: [{ file: "./dist/built/Q/indexAll.bundle.d.ts", format: "es" }],
		plugins: [dts(), toGlobal('Q')],
	},
	{
		input: "./dist/built/Serenity/index.d.ts",
		output: [{ file: "./dist/built/Serenity/index.bundle.d.ts", format: "es" }],
		plugins: [dts(), toGlobal('Serenity')],
	},
	{
		input: "./dist/built/Decorators/index.d.ts",
		output: [{ file: "./dist/built/Decorators/index.bundle.d.ts", format: "es" }],
		plugins: [dts(), toGlobal('Serenity.Decorators')],
	},
	{
		input: "./dist/built/Slick/index.d.ts",
		output: [{ file: "./dist/built/Slick/index.bundle.d.ts", format: "es" }],
		plugins: [dts(), toGlobal('Slick')]
	},
	{
		input: "./dist/built/Globals/index.d.ts",
		output: [{ file: "./dist/built/Globals/index.bundle.d.ts", format: "es" }],
		plugins: [dts(), toGlobal(null), {
			name: 'writeFinal',
			generateBundle: function() {
				var src = outputs.join('\n');
				var refTypes = [];
				src = src.replace(rxReferenceTypes, function(match, offset, str) {
					refTypes.push(match);
					return '';
				});
				refTypes = refTypes.filter((x, i) => refTypes.indexOf(x) == i);
				src = refTypes.join('') + '\n' + src
				fs.writeFileSync('./dist/Serenity.CoreLib.d.ts', src);
			}
		}],
	}
];