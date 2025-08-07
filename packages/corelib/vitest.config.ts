import { defineConfig } from "vitest/config";

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
            excludeAfterRemap: true
        },
        globals: true
    }
})