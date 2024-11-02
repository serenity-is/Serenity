import { join, resolve } from "path";
import { fileURLToPath } from 'url';

const testUtils = resolve(join(fileURLToPath(new URL('.', import.meta.url)), './'));
const serenityRoot = resolve(join(testUtils, "../../"));

export default () => ({
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
});