import esbuild from "esbuild";
import { createRequire } from 'node:module';
import { join, resolve } from "path";
import { fileURLToPath } from 'url';

const root = resolve(join(fileURLToPath(new URL('.', import.meta.url)), '../'));

const require = createRequire(import.meta.url);

var esbuildOptions = {
    absWorkingDir: resolve(root),
    bundle: true,
    color: true,
    chunkNames: 'chunks/[name]-[hash]',
    format: 'esm',
    logLevel: 'info',
    outdir: 'dist',
    sourcemap: true,
    splitting: false,
    target: 'es2015'
}

await esbuild.build({
    ...esbuildOptions,
    entryPoints: [
        'src/index.ts'
    ],
    outbase: 'src',
    minify: false
});