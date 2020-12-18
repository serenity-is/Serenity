import ts from "@wessberg/rollup-plugin-ts";
import {terser} from "rollup-plugin-terser";
import pkg from "./package.json";
import {builtinModules} from "module";

export default [
	{
		input: "CoreLib/CoreLib.ts",
		output: [
			{
				file: 'dist/Serenity.CoreLib.js',
				format: "iife",
				sourcemap: true,
				name: "window",
				extend: true
			},
			{
				file: 'dist/Serenity.CoreLib.min.js',
				format: "iife",
				sourcemap: false,
				name: "window",
				extend: true,
				plugins: [
					terser()
				]
			},
			{
				file: 'dist/Serenity.CoreLib.esm.js',
				format: "esm",
				sourcemap: true,
			},
			{
				file: 'dist/Serenity.CoreLib.esm.min.js',
				format: "esm",
				sourcemap: false,
				plugins: [
					terser()
				]
			}
		],
		plugins: [
			ts({
				tsconfig: 'CoreLib/tsconfig.json'
			}),
			{
				name: 'stripExportsForGlobal',
				generateBundle(o, b) {
					for (var k of Object.keys(b)) {
						if (/CoreLib\.d\.ts/.test(k)) {
							var src = b[k].source;
							src = src.replace(/^(declare\s+global.*\s*\{\r?\n)((^\s+.*\r?\n)*)\}/gmi, function(x, y, z) {
								var r = z.indexOf('\r') >= 0;
								return z.replace(/\r/g, '').split('\n')
									.map(s => s.length > 0 && s.charAt(0) == '\t' ? s.substring(1) : (s.substring(0, 4) == '    ' ? s.substring(4) : s))
									.map(s => /^namespace/.test(s) ? ("declare " + s) : s)
									.join(r ? '\r\n' : '\n');
							});

							b[k].source = src.replace(/^export\s*\{[^\}]*\}\s*\;/gmi, '');
						}
					}
				}
			}
		],
		external: [
			...builtinModules,
			...(pkg.dependencies == null ? [] : Object.keys(pkg.dependencies)),
			...(pkg.devDependencies == null ? [] : Object.keys(pkg.devDependencies)),
			...(pkg.peerDependencies == null ? [] : Object.keys(pkg.peerDependencies))
		]
	}
];