
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
    color: true,
    format: 'esm',
    jsxSideEffects: true,
    logLevel: 'info',
    minify: false,
    target: 'es2022',
    outdir: 'dist',
    plugins: [writeIfChanged()],
    sourcemap: true,
    sourceRoot: "https://packages.serenity.is/domwise/src/"
}

const esmIndex = {
    ...defaults,
    entryPoints: [{
        in: './src/index.ts', out: 'index'
    }],
}

const esmJsxRuntime = {
    ...defaults,
    bundle: false,
    entryPoints: [{
        in: './src/jsx-runtime.ts', out: 'jsx-runtime'
    }]
}

const buildList = [];

buildList.push(
    esmIndex,
    esmJsxRuntime
)

for (const buildItem of buildList) {
    await esbuild.build({
        ...buildItem,
    }).catch(() => process.exit());
}
