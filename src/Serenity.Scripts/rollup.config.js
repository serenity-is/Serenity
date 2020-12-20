import typescript from '@rollup/plugin-typescript';
import {terser} from "rollup-plugin-terser";
import pkg from "./package.json";
import {builtinModules} from "module";
import dts from "rollup-plugin-dts";

var globals = {
	'jquery': '$',
	'flatpickr': 'flatpickr'
}

export default [
	{
		input: "CoreLib/index.ts",
		output: [
			{
				file: 'dist/Serenity.CoreLib.js',
				format: "iife",
				sourcemap: true,
				name: "window",
				extend: true,
				globals
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
				],
				globals
			}*/
		],
		plugins: [
			typescript({
				tsconfig: 'CoreLib/tsconfig.json',
				outDir: 'CoreLib/built'
			}),
			{
				name: 'stripExportsForGlobal',
				generateBundle(o, b) {
					for (var fileName of Object.keys(b)) {
						/*var result = /^Serenity\.CoreLib\.([A-Za-z]*)\.d\.ts$/.exec(fileName);
						if (result && result.length > 1) {
							var ns = result[1];
							var src = b[fileName].source;
							if (src) {
								src = src + '\nexport as namespace ' + ns + ';';
								b[fileName].source = src;
							}
						}
							var src = b[k].source;
							src = src.replace(/^(declare\s+global.*\s*\{\r?\n)((^\s+.*\r?\n)*)\}/gmi, function(x, y, z) {
								var r = z.indexOf('\r') >= 0;
								return z.replace(/\r/g, '').split('\n')
									.map(s => s.length > 0 && s.charAt(0) == '\t' ? s.substring(1) : (s.substring(0, 4) == '    ' ? s.substring(4) : s))
									.map(s => /^namespace/.test(s) ? ("declare " + s) : s)
									.join(r ? '\r\n' : '\n');
							});

							b[k].source = src.replace(/^export\s*\{[^\}]*\}\s*\;/gmi, '');
						}*/
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
	},
	{
		input: "./dist/built/index.Q.d.ts",
		output: [{ file: "./dist/built/global.Q.d.ts", format: "es" }],
		plugins: [dts()],
	},
	{
		input: "./dist/built/index.Serenity.d.ts",
		output: [{ file: "./dist/built/global.Serenity.d.ts", format: "es" }],
		plugins: [dts()],
	},
	{
		input: "./dist/built/index.Slick.d.ts",
		output: [{ file: "./dist/built/global.Slick.d.ts", format: "es" }],
		plugins: [dts()],
	}
];