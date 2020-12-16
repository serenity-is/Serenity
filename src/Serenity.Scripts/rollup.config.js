import dts from "rollup-plugin-dts";

export default [
  {
    input: 'dist/index.js',
    output: {
      file: 'dist/Serenity.CoreLib.js',
      format: 'umd',
	  name: 'window'
    }
  },
  {
    input: 'dist/index.d.ts',
	output: [{ file: "dist/Serenity.CoreLib.d.ts", format: "umd" }],
	plugins: [dts()]
  }
]