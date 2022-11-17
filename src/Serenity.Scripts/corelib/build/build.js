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
    { ...compatCore, entryPoints: [`${sleekSrc}/core/index.ts`], outfile: `${assetsSlick}/slick.core.js`, sourcemap: false },
    { ...compatGrid, entryPoints: [`${sleekSrc}/grid/index.ts`], outfile: `${assetsSlick}/slick.grid.js`, sourcemap: false },
    { ...compatLayoutsFrozen, entryPoints: [`${sleekSrc}/layouts/frozenlayout.ts`], outfile: `${assetsSlick}/layouts/slick.frozenlayout.js`, sourcemap: false }
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
    external: ['../q', '../slick', './q', './slick'],
    minify: true
});

var corelibIndex = readFileSync("./dist/index.js", "utf8");
corelibIndex = corelibIndex.replace(/(["'])\.\.\/q/g, '$1.\\/q').replace(/(["'])\.\.\/slick/g, '$1.\\/slick');
writeFileSync("./dist/index.js", corelibIndex, "utf8");

//await esbuild.build({
//    ...coreLibBase,
//    entryPoints: [
//        'index.ts'
//    ],
//    external: ['./q', './slick', '../q', '../slick'],
//    minify: true
//});