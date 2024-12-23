import { defineProject } from "vitest/config";

export default defineProject({
    test: {
        name: "corelib",
        environment: "jsdom",
        browser: {
            provider: 'playwright',
            instances: [{
                browser: "chromium",
            }]
        },
        globals: true
    }
})