import { execSync } from "child_process";
import { existsSync } from "fs";
import { join, resolve } from "path";
import { fileURLToPath } from 'url';

const testUtils = resolve(join(fileURLToPath(new URL('.', import.meta.url)), './'));
const serenityRoot = resolve(join(testUtils, "../../"));

export default (opt) => {

    if ((opt?.dynamicData ?? true) &&
        !existsSync(`${testUtils}/dynamic-data/Columns.Administration.Language.json`) &&
        !existsSync(resolve(`./dynamic-data/Columns.Administration.Language.json`))) {

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

        if (resolve("./").indexOf('Serene.Web') >= 0 || !tryProject(`${serenityRoot}/..`, "StartSharp"))
            tryProject(`${serenityRoot}/serene`, "Serene");
    }

    return {
        coveragePathIgnorePatterns: [
            "<rootDir>/node_modules/",
            "/src/Serenity.Assets/"
        ],
        extensionsToTreatAsEsm: ['.ts', '.tsx'],
        moduleNameMapper: {
            "^@serenity-is/(.*)$": ["<rootDir>/node_modules/@serenity-is/$1", "<rootDir>/../node_modules/@serenity-is/$1", "<rootDir>/../../node_modules/@serenity-is/$1"],
            "^dynamic-data/(.*)$": ["<rootDir>/dynamic-data/$1", `${testUtils}/dynamic-data/$1`]
        },
        setupFiles: [
            `${testUtils}/jest-setup.js`,
        ],
        setupFilesAfterEnv: [
            `${testUtils}/jest-setup-afterenv.js`
        ],
        testEnvironment: `${testUtils}/jsdom-global.js`,
        testMatch: [
            "<rootDir>/test/**/*.spec.ts*",
            "<rootDir>/src/**/*.spec.ts*"
        ],
        transform: {
            '\\.css$': `${testUtils}/jest-css-workaround.cjs`,
            "^.+\.(t|j)sx?$": [`${serenityRoot}/node_modules/@swc/jest`, {
                jsc: {
                    parser: {
                        syntax: "typescript",
                        decorators: true,
                        tsx: true
                    },
                    keepClassNames: true,
                    transform: {
                        react: {
                            runtime: 'automatic',
                            importSource: 'jsx-dom'
                        }
                    }
                },
                module: {
                    type: "commonjs"
                }
            }]
        },
        transformIgnorePatterns: []
    }
}