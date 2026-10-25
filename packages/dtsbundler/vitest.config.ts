import { serenityTestOptions } from "../test-utils/vitest-defaults.js";
import { configDefaults, defineConfig } from "vitest/config";

export default defineConfig({
    test: {
        name: "dtsbundler",
        ...serenityTestOptions(import.meta.dirname),
        environment: "node",
        // most tests spawn the CLI, which compiles TypeScript in a child process,
        // so they need more headroom than the shared default
        testTimeout: 30_000,
        hookTimeout: 30_000,
        coverage: {
            exclude: [
                ...(configDefaults?.coverage?.exclude ?? []),
                "build/**",
                "test/fixtures/**",
                // executed in a child process by test/cli.spec.ts, so it never shows up here
                "src/cli.ts"
            ],
            excludeAfterRemap: true
        },
        pool: "vmThreads"
    }
})