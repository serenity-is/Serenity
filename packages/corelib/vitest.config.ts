import { configDefaults, defineConfig } from "vitest/config";

export default defineConfig({
    test: {
        name: "corelib",
        environment: "jsdom",
        browser: {
            provider: 'playwright',
            instances: [{
                browser: "chromium",
            }]
        },
        coverage: {
            exclude: [
                ...(configDefaults?.coverage?.exclude ?? []),
                "build/**",
                "src/index.ts"
            ],
            excludeAfterRemap: true
        },
        globals: true
    }
})