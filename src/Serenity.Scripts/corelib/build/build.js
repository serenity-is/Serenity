import esbuild from "esbuild";
import { fileURLToPath } from 'url';
import { compatCore, compatGrid, compatLayoutsFrozen } from "@serenity-is/sleekgrid/build/defines";
import { join, resolve } from "path";
import { createRequire } from 'node:module';
import { readFileSync, writeFileSync } from "fs";

const root = resolve(join(fileURLToPath(new URL('.', import.meta.url)), '../'));

const require = createRequire(import.meta.url);
const sleekSrc = resolve(join(require.resolve('@serenity-is/sleekgrid/build/defines'), '..', '..', 'src'));
const assetsSlick = resolve(join(root, '..', '..', 'Serenity.Assets', 'wwwroot', 'Scripts', 'SlickGrid'));

for (var esmOpt of [
    { ...compatCore, absWorkingDir: sleekSrc, entryPoints: [`core/index.ts`], outfile: `${assetsSlick}/slick.core.js`, sourcemap: false },
    { ...compatGrid, absWorkingDir: sleekSrc, entryPoints: [`grid/index.ts`], outfile: `${assetsSlick}/slick.grid.js`, sourcemap: false },
    { ...compatLayoutsFrozen, absWorkingDir: sleekSrc, entryPoints: [`layouts/frozenlayout.ts`], outfile: `${assetsSlick}/layouts/slick.frozenlayout.js`, sourcemap: false }
]) {
    esbuild.build(esmOpt).catch(() => process.exit());
}

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
    target: 'es6'
}

await esbuild.build({
    ...coreLibBase,
    entryPoints: [
        'src/q/index.ts',
        'src/slick/index.ts',
        'src/index.ts'
    ],
    outbase: 'src',
    external: ['@serenity-is/corelib/q', '@serenity-is/corelib/slick', '@serenity-is/sleekgrid' ],
    minify: true
});