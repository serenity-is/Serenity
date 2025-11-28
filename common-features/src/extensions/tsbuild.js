import { build, writeIfChanged } from "@serenity-is/tsbuild";
import { build as esbuild } from "esbuild";
import { existsSync, readFileSync, writeFileSync } from "fs";

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
    external: [
        '@serenity-is/*'
    ],
    format: 'iife',
    footer: {
        js: `
Serenity = Serenity || {}; 
if (Serenity.initGlobalMappings) { 
    Serenity.initGlobalMappings({ extensions: Serenity._ }); 
} else { 
    Object.assign(Serenity, Serenity._);
    Serenity.Extensions = Serenity.Extensions || Serenity._; 
    delete Serenity._;
}
`
    },
    globalName: 'Serenity._',
    outdir: 'wwwroot/'
}));

await esbuild({
    assetNames: 'assets/[name]-[hash]',
    bundle: true,
    entryPoints: [
        { in: "wwwroot/style/bundle.css", out: "common-theme" }
    ],
    legalComments: "inline",
    loader: {
        ".jpg": "file"
    },
    outdir: "wwwroot/",
    write: false,
    plugins: [
        writeIfChanged()
    ],
    target: "es6",
});
