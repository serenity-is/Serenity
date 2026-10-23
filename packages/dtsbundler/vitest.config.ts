import { configDefaults, defineConfig } from "vitest/config";

export default defineConfig({
    test: {
        name: "dtsbundler",
        environment: "node",
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