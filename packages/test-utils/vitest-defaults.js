import { execSync } from "child_process";
import { existsSync, readdirSync, readFileSync } from "fs";
import { basename, join, resolve } from "path";
import { fileURLToPath } from 'url';
import { defineConfig, } from "vitest/config";

const testUtils = resolve(join(fileURLToPath(new URL('.', import.meta.url)), './'));
const serenityRoot = resolve(join(testUtils, "../../"));

export default (opt) => {
    
    if ((opt?.dynamicData ?? true) &&
        !existsSync(`${testUtils}/dynamic-data/Columns.Administration.Language.json`) &&
        !existsSync(resolve(`./dynamic-data/Columns.Administration.Language.json`))) {
        if (resolve("./").indexOf('Serene.Web') >= 0 || !tryProject(`${serenityRoot}/..`, "StartSharp"))
            tryProject(`${serenityRoot}/serene`, "Serene");
    }
   
    const provide = {};
    if (opt?.dynamicData ?? true) {
        for (var folder of [join(testUtils, "dynamic-data"), join(opt?.projectRoot ?? resolve("./"), "dynamic-data")]) {
            if (existsSync(folder)) {
                for (var file of readdirSync(folder)) {
                    if (file.endsWith(".json")) {
                        provide["dynamic-data/" + basename(file)] = readFileSync(join(folder, file), "utf8");
                    }
                }
            }
        }
    }

    return defineConfig({
        test: {
            environment: "jsdom",
            globals: true,
            alias: [
                { find: "@serenity-is/corelib", replacement: `${testUtils}/../corelib/dist/index.js` },
                { find: "jsx-dom/min/jsx-dev-runtime", replacement: "jsx-dom/jsx-runtime.js" },
                { find: "jsx-dom/jsx-dev-runtime", replacement: "jsx-dom/jsx-runtime.js" }
            ],
            provide,
            setupFiles: [
                "test-utils/vitest-setup.js"
            ]
        }
    });
}

function tryProject(root, name) {
    const target = "net8.0";
    const folder = `${root}/src/${name}.Web`;
    const csproj = `${folder}/${name}.Web.csproj`;
    if (!existsSync(csproj))
        return false;

    const debugDll = `${folder}/bin/Debug/${target}/${name}.Web.dll`;
    const releaseDll = `${folder}/bin/Release/${target}/${name}.Web.dll`;

    let debugExists = existsSync(debugDll);
    let releaseExists = !debugExists && existsSync(releaseDll);
    if (!debugExists && !releaseExists)
        execSync(`dotnet build ${csproj}`, { timeout: 60000 });

    debugExists = existsSync(debugDll);
    releaseExists = !debugExists && existsSync(releaseDll);
    if (debugExists || releaseExists)
        execSync(`dotnet ${debugExists ? debugDll : releaseDll} dynamic-data`, {
            timeout: 60000,
            cwd: resolve(".").indexOf(name + ".Web") >= 0 ? resolve("./") : testUtils
        });

    return true;
}