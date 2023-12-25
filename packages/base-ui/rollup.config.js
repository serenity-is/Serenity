import { nodeResolve } from '@rollup/plugin-node-resolve';
import dts from "rollup-plugin-dts";

const externalPackages = ["@serenity-is/base"];
const nodeResolvePlugin = () => nodeResolve({
    resolveOnly: ['@serenity-is/base']
});

export default [
    {
        input: "./src/index.ts",
        output: [{ 
            file: "./dist/index.d.ts",
            format: "es",
            generatedCode: 'es2015'
        }],
        plugins: [
            nodeResolvePlugin(),
            dts({
                respectExternal: true
            }),
        ],
        external: externalPackages
    }
];