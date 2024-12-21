import { defineProject } from "vitest/config";

export default defineProject({
    test: {
        name: "corelib",
        environment: "jsdom",
        alias: {
            "jsx-dom/min/jsx-dev-runtime": "jsx-dom/jsx-runtime.js",
            "jsx-dom/jsx-dev-runtime": "jsx-dom/jsx-runtime.js",
        },
        browser: {
            provider: 'playwright',
            name: 'chromium',
        },
        globals: true
    }
})