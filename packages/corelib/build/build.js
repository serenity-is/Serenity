import {
    compatCore, compatDataGroupItemMetadataProvider, compatEditors,
    compatFormatters, compatGrid,
    compatLayoutsFrozen, compatPluginsAutoTooltips
} from "@serenity-is/sleekgrid/build/defines";
import esbuild from "esbuild";
import { createRequire } from 'node:module';
import { join, resolve } from "path";
import { fileURLToPath } from 'url';

const root = resolve(join(fileURLToPath(new URL('.', import.meta.url)), '../'));

const require = createRequire(import.meta.url);
const sleekRoot = resolve(join(require.resolve('@serenity-is/sleekgrid/build/defines'), '..', '..'));
const assetsSlick = resolve(join(root, '..', '..', 'src', 'Serenity.Assets', 'wwwroot', 'Scripts', 'SlickGrid'));

const minify = true;
for (var esmOpt of [
    { ...compatCore, minify },
    { ...compatGrid, minify },
    { ...compatLayoutsFrozen },
    { ...compatEditors },
    { ...compatFormatters },
    { ...compatDataGroupItemMetadataProvider, minify },
    { ...compatPluginsAutoTooltips, minify }
]) {
    var shouldMinify = esmOpt.minify;

    esmOpt = { 
        ...esmOpt, 
        absWorkingDir: sleekRoot,
        outfile: esmOpt.outfile.replace('./dist/compat/', assetsSlick + '/').replace('/plugins/', '/Plugins/'),
        minify: false,
        sourcemap: false
    };

    esbuild.build(esmOpt).catch(() => process.exit());

    if (shouldMinify) {
        await esbuild.build({
            ...esmOpt,
            minify: true,
            outfile: esmOpt.outfile.replace(/\.js$/, '.min.js')
        }).catch(() => process.exit());
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
    external: ['@serenity-is/corelib/q', '@serenity-is/corelib/slick', '@serenity-is/sleekgrid' ],
    minify: true
});