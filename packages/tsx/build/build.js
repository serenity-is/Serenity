import esbuild from "esbuild";
import { join, resolve } from "path";
import { fileURLToPath } from "url";

const root = resolve(join(fileURLToPath(new URL('.', import.meta.url)), '../'));

await esbuild.build({
    absWorkingDir: resolve(root),
    bundle: true,
    color: true,
    entryPoints: [
        'src/index.ts'
    ],
    format: 'esm',
    logLevel: 'info',
    minify: true,
    outbase: 'src',
    outdir: 'dist',
    sourcemap: true,
    target: 'es2017'
});