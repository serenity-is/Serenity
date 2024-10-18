import { build } from "@serenity-is/tsbuild";

// wwwroot/esm/**/*.js: ESM entry points for own pages
await build({
});

// dist/index.js: ESM bundle for NPM references
await build({
    clean: false,
    entryPoints: ['./Modules/index.ts'],
    external: [
        '@serenity-is/*'
    ],
    outbase: './Modules/',
    outdir: 'dist/',
    plugins: [],
    splitting: false
});