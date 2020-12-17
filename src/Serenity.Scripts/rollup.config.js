import ts from "@wessberg/rollup-plugin-ts";
import pkg from "./package.json";
import {builtinModules} from "module";

export default [
	{
		input: "CoreLib/CoreLib.ts",
		output: [
			{
				file: 'dist/Serenity.CoreLib.js',
				format: "umd",
				sourcemap: true,
				name: "window"
			}
		],
		plugins: [
			ts({
				tsconfig: 'CoreLib/tsconfig.json'
			}),
			{
				name: 'myfixexport',
				generateBundle: function(o, b) {
					for (var k of Object.keys(b)) {
						if (/\.d\.ts/.test(k))
							b[k].source = b[k].source + '// i m here!';				
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