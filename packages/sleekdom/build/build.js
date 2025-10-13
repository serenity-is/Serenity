
import esbuild from "esbuild";
import { existsSync, mkdirSync, readFileSync, writeFileSync } from "fs";
import { dirname } from "path";
import process from 'process';

function writeIfChanged() {
    return {
        name: "write-if-changed",
        setup(build) {
            const write = build.initialOptions.write;
            build.initialOptions.write = false;
            build.onEnd(result => {
                if (!(write === undefined || write))
                    return;
                result.outputFiles?.forEach(file => {
                    if (existsSync(file.path)) {
                        const old = readFileSync(file.path);
                        if (old.equals(file.contents))
                            return;
                    }
                    else {
                        mkdirSync(dirname(file.path), { recursive: true });
                    }
                    writeFileSync(file.path, file.text);
                });
            });
        }
    };
}

const defaults = {
    bundle: true,
    entryPoints: ['./src/index.ts'],
    color: true,
    logLevel: 'info',
    target: 'es2015',
    plugins: [writeIfChanged()]
}

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
const buildList = [];

buildList.push(
    esmIndex,
    esmJsxRuntime,
    esmJsxDevRuntime
)

for (const buildItem of buildList) {
    await esbuild.build({
        ...buildItem,
    }).catch(() => process.exit());
}
