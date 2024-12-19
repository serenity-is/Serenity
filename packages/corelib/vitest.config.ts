import { defineConfig } from "vitest/config";

export default defineConfig({
    test: {
        environment: "jsdom",
        alias: {
            "jsx-dom/min/jsx-dev-runtime": "jsx-dom/jsx-runtime.js",
            "jsx-dom/jsx-dev-runtime": "jsx-dom/jsx-runtime.js",
        },
        globals: true
    }
})