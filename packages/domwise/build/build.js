
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

const watch = process.argv.includes('--watch');

// NodeScriptRunner sets the variable and holds the other end of the stdin pipe, so that the
// watcher doesn't outlive the application even if it is terminated abruptly
if (watch && process.env.SERENITY_EXIT_ON_STDIN_CLOSE === "1") {
    process.stdin.on("end", () => process.exit(0));
    process.stdin.on("error", () => process.exit(0));
    process.stdin.resume();
}

async function run(options) {
    if (watch)
        await (await esbuild.context(options)).watch();
    else
        await esbuild.build(options);
}

for (const buildItem of buildList) {
    await run({
        ...buildItem,
    }).catch(() => process.exit());
}
