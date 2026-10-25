import { serenityTestOptions } from "../test-utils/vitest-defaults.js";
import { configDefaults, defineConfig } from "vitest/config";

export default defineConfig({
    test: {
        name: "domwise",
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
        pool: "vmThreads"
    }
})