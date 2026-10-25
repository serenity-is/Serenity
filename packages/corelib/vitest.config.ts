import { serenityTestOptions } from "../test-utils/vitest-defaults.js";
import { configDefaults, defineConfig } from "vitest/config";

export default defineConfig({
    test: {
        name: "corelib",
        ...serenityTestOptions(import.meta.dirname),
        environment: "jsdom",
        coverage: {
            exclude: [
                ...(configDefaults?.coverage?.exclude ?? []),
                "build/**",
                "src/test/**",
                "src/index.ts"
            ],
            excludeAfterRemap: true
        },
        execArgv: [
            Number((globalThis as any).process.versions.node.split('.')[0]) >= 25 ? '--no-webstorage' : null
        ].filter(x => x != null),
        pool: "vmThreads"
    }
})