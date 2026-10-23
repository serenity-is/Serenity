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
    logLevel: 'info',
    minify: false,
    platform: 'node',
    target: 'es2022',
    outdir: 'dist',
    plugins: [writeIfChanged()],
    sourcemap: false,
    external: [
        // provided by the consumer, not bundled
        'typescript'
    ]
}

const esmIndex = {
    ...defaults,
    entryPoints: [{
        in: './src/index.ts', out: 'index'
    }],
}

const esmCli = {
    ...defaults,
    entryPoints: [{
        // the `#!` line is kept from the source, so no banner is needed here
        in: './src/cli.ts', out: 'cli'
    }],
}

const buildList = [];

buildList.push(
    esmIndex,
    esmCli
)

for (const buildItem of buildList) {
    await esbuild.build({
        ...buildItem,
    }).catch(() => process.exit());
}