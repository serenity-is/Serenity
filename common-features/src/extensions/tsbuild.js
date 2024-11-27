import { build } from "@serenity-is/tsbuild";

const buildOpt = {
    entryPoints: ['./Modules/index.ts'],
    outbase: './Modules/',
    splitting: false,
    clean: false
}

// wwwroot/esm/**/*.js: ESM entry points for own pages
await build({
});

// dist/index.js: ESM bundle for NPM references
await build(Object.assign({}, buildOpt, {
    external: [
        '@serenity-is/*'
    ],
    outdir: 'dist/',
    plugins: []
}));

// wwwroot/index.js: Global script (e.g. ~/Serenity.Extensions/index.js include in appsettings.bundles.json)
await build(Object.assign({}, buildOpt, {
    format: 'iife',
    footer: {
        js: 'Serenity.Extensions = Serenity.Extensions || {}; Object.assign(Serenity.Extensions, Serenity._); delete Serenity._;'
    },
    globalName: 'Serenity._',
    outdir: 'wwwroot/'
}));
