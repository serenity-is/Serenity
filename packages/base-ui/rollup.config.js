import dts from "rollup-plugin-dts";

export default [
    {
        input: "./src/index.ts",
        output: [{ 
            file: "./dist/index.d.ts",
            format: "es",
            generatedCode: 'es2015'
        }],
        plugins: [
            dts()
        ]
    }
];