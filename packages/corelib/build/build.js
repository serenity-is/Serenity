import esbuild from "esbuild";
import { createRequire } from 'node:module';
import { join, resolve } from "path";
import { existsSync, readFileSync, writeFileSync } from "node:fs";
import { fileURLToPath } from 'url';

const root = resolve(join(fileURLToPath(new URL('.', import.meta.url)), '../'));

const require = createRequire(import.meta.url);
const sleekRoot = resolve(join(require.resolve('@serenity-is/sleekgrid/build/defines'), '..', '..'));
const assetsSlick = resolve(join(root, '..', '..', 'src', 'Serenity.Assets', 'wwwroot', 'Scripts', 'SlickGrid'));

function writeIfDifferent(target, content) {
    if (!existsSync(target) ||
        readFileSync(target, 'utf8') != content) {
        writeFileSync(target, content);
    }
}

function copyIfDifferent(source, target) {
    var content = readFileSync(source, 'utf8').replace(/^\/\/#\s*sourceMappingURL=.*\.map\s*$/mg, '');
    writeIfDifferent(target, content);
}

const minify = true;
for (var esmOpt of [
    { file: 'layouts/slick.frozenlayout.js' },
    { file: 'plugins/slick.autotooltips.js', minify },
    { file: 'plugins/slick.rowmovemanager.js' },
    { file: 'plugins/slick.rowselectionmodel.js' },
    { file: 'slick.core.js', minify },
    { file: 'slick.editors.js' },
    { file: 'slick.formatters.js' },
    { file: 'slick.grid.js', minify },
    { file: 'slick.groupitemmetadataprovider.js', minify }
]) {
    var shouldMinify = esmOpt.minify;
    var sourceFile = join(sleekRoot, 'dist/compat/' + esmOpt.file);
    var targetFile = join(assetsSlick, esmOpt.file).replace('/plugins/', '/Plugins/');
    copyIfDifferent(sourceFile, targetFile);
    if (shouldMinify)
        copyIfDifferent(sourceFile.replace(/\.js$/, '.min.js'), targetFile.replace(/\.js$/, '.min.js'));
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
    target: 'es2015'
}

await esbuild.build({
    ...coreLibBase,
    entryPoints: [
        'src/index.ts'
    ],
    outbase: 'src',
    external: ['@serenity-is/sleekgrid'],
    minify: true
});