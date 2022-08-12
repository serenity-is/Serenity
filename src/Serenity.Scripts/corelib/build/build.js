import esbuild from "esbuild";
import { fileURLToPath } from 'url';
import { compatCore, compatGrid, compatLayoutsFrozen } from "../node_modules/@serenity-is/sleekgrid/build/defines.js";
import { join, resolve } from "path";

const root = resolve(join(fileURLToPath(new URL('.', import.meta.url)), '../'));

const src = resolve(join(root, 'node_modules', '@serenity-is', 'sleekgrid', 'src'));
const assetsSlick = resolve(join(root, '..', '..', 'Serenity.Assets', 'wwwroot', 'Scripts', 'SlickGrid'));

for (var esmOpt of [
    { ...compatCore, entryPoints: [`${src}/core/index.ts`], outfile: `${assetsSlick}/slick.core.js`, sourcemap: false },
    { ...compatGrid, entryPoints: [`${src}/grid/index.ts`], outfile: `${assetsSlick}/slick.grid.js`, sourcemap: false },
    { ...compatLayoutsFrozen, entryPoints: [`${src}/layouts/frozenlayout.ts`], outfile: `${assetsSlick}/layouts/slick.frozenlayout.js`, sourcemap: false }
]) {
    esbuild.build(esmOpt).catch(() => process.exit());
}

let localImport = function (filter, pkg) {
    return {
        name: 'import-local',
        setup(build) {
            build.onResolve({ filter: filter }, args => {
                return { path: pkg, external: true }
            })
        }
    }
}

var coreLibBase = {
    absWorkingDir: resolve(join(root, 'src')),
    bundle: true,
    color: true,
    chunkNames: 'chunks/[name]-[hash]',
    format: 'esm',
    logLevel: 'info',
    outbase: ".",
    outdir: '../dist',
    sourcemap: true,
    splitting: false,
    target: 'es6'
}

await esbuild.build({
    ...coreLibBase,
    entryPoints: [
        'q/index.ts',
        'slick/index.ts',
        'index.ts'
    ],
    external: ['../q', '../slick'],
    splitting: false,
    minify: true
});