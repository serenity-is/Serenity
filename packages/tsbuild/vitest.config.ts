import { configDefaults, defineConfig } from "vitest/config";

export default defineConfig({
    test: {
        name: "tsbuild",
        coverage: {
            exclude: [
                ...(configDefaults?.coverage?.exclude ?? []),
                "build/**",
                "src/test/**",
                "src/index.ts"
            ],
            excludeAfterRemap: true
        },
        globals: true
    }
})