import esbuild from "esbuild";
import { join, resolve } from "path";
import { fileURLToPath } from 'url';

const root = resolve(join(fileURLToPath(new URL('.', import.meta.url)), '../'));

var coreLibBase = {
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
    ...coreLibBase,
    entryPoints: [
        'src/index.ts'
    ],
    outbase: 'src',
    external: ['@serenity-is/sleekgrid'],
    minify: false,
    sourcemap: false
});