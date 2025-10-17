
import { writeIfChanged } from "build-utils";
import process from 'process';

const hasDtsArg = process.argv.includes('dts') || process.argv.includes('--dts');
if (hasDtsArg) {
    const dtsBundler = (await import("build-utils/dts-bundler.js"));
    dtsBundler.dtsBundle();
    process.exit(0);
}

const defaults = {
    bundle: true,
    entryPoints: ['./src/index.ts'],
    color: true,
    logLevel: 'info',
    target: 'es2020',
    plugins: [writeIfChanged()]
}

const esbuild = await import("esbuild");

const esmIndex = {
    ...defaults,
    format: 'esm',
    minify: false,
    outfile: './dist/index.js'
}

const esmJsxRuntime = {
    ...defaults,
    format: 'esm',
    sourcemap: false,
    bundle: false,
    minify: false,
    entryPoints: ['./src/jsx-runtime.ts'],
    outfile: './dist/jsx-runtime.js'
}

const esmJsxDevRuntime = {
    ...esmJsxRuntime,
    outfile: './dist/jsx-dev-runtime.js'
}

const buildList = [
    esmIndex,
    esmJsxRuntime,
    esmJsxDevRuntime
];

for (const buildItem of buildList) {
    await esbuild.build({
        ...buildItem,
    }).catch(() => process.exit());
}
