import esbuild from "esbuild";
import { compatCore, compatGrid } from "@serenity-is/sleekgrid/build/defines.js";
import { resolve } from "path";

const src = 'node_modules/@serenity-is/sleekgrid/src';
const out = '../Serenity.Assets/wwwroot/Scripts/SlickGrid';

for (var esmOpt of [
    { ...compatCore, entryPoints: [`${src}/core/index.ts`], outfile: `${out}/slick.core.js`, sourcemap: false },
    { ...compatGrid, entryPoints: [`${src}/grid/index.ts`], outfile: `${out}/slick.grid.js`, sourcemap: false }
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
    absWorkingDir: resolve('./CoreLib'),
    bundle: true,
    color: true,
    chunkNames: 'chunks/[name]-[hash]',
    format: 'esm',
    logLevel: 'info',
    outbase: ".",
    outdir: resolve('./dist'),
    sourcemap: true,
    splitting: false,
    target: 'es6'
}

await esbuild.build({
    ...coreLibBase,
    entryPoints: [
        'q.ts',
        'slick.ts',
        'index.ts'
    ],
    plugins: [localImport(/.\/q$/, './q.js'), localImport(/.\/slick$/, './slick.js')],
    splitting: false,
    minify: true
});

