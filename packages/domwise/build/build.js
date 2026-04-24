
import esbuild from "esbuild";
import { existsSync, mkdirSync, readFileSync, writeFileSync } from "fs";
import { dirname } from "path";
import process from 'process';
import { build, rolldown } from "rolldown";

function writeIfChangedPlugin() {
    return {
        name: "rolldown-plugin-write-if-changed",
        generateBundle: function(outputOptions, bundle, isWrite) {
            const filenames = Object.keys(bundle);
            for (const filename in filenames) {
                const outputItem = bundle[filename];
                const filePath = outputOptions.dir ? `${outputOptions.dir}/${filename}` : filename;
                if (existsSync(filePath)) {
                    let newContent = outputItem.type === 'asset' ? outputItem.source : outputItem.code;
                    if (newContent instanceof Uint8Array) {
                        newContent = Buffer.from(newContent).toString('utf8');
                    }
                    if (readFileSync(filePath, 'utf8') === newContent) {
                        delete bundle[filename];
                    }
                }
            }
        }
    };
}

const defaults = {
    logLevel: 'info',
    output: {
        minify: false,
        dir: "dist",
        sourcemap: true,
        sourcemapBaseUrl: "https://packages.serenity.is/domwise/src/"
    },
    plugins: [writeIfChangedPlugin()]
}

const esmIndex = {
    ...defaults,
    input: {
        'index': 'src/index.ts'
    }
}

const esmJsxRuntime = {
    ...defaults,
    input: {
        'jsx-runtime': 'src/jsx-runtime.ts'
    }
}

const buildList = [];

buildList.push(
    esmIndex,
    esmJsxRuntime
)

for (const buildItem of buildList) {
	try {
		await build(buildItem);
	} catch (error) {
		console.error(error);
        process.exit(1);
	}
}
