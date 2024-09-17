import { nodeResolve } from '@rollup/plugin-node-resolve';
import { dts } from "rollup-plugin-dts";

export default [{
    input: "./src/index.ts",
    output: [{
        file: "./dist/index.d.ts",
        format: "es"
    }],
    plugins: [
        nodeResolve({
            resolveOnly: ['@serenity-is/base', '@serenity-is/base-ui', 'jsx-dom']
        }),
        dts({
            respectExternal: true
        })
    ],
    external: [
        "@serenity-is/sleekgrid"
    ]
}];