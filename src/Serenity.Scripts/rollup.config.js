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
				plugins: {
					generateBundle: function(o, b) {
						for (var k of Object.keys(b)) {
							if (/\.d\.ts/.test(k))
								b[k].source = b[k].source + '// i m here!';				
						}
					}
				}
			},
			{
				file: 'dist/Serenity.CoreLib.esm.js',
				format: "esm",
				sourcemap: true,
			},
			{
				file: 'dist/Serenity.CoreLib.esm.min.js',
				format: "esm",
				sourcemap: true,
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
				name: 'myfixexport',
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