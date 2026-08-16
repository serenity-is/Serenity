import { defineConfig } from "vitest/config";

export default defineConfig({
    test: {
        name: "sleekgrid",
        environment: "jsdom",
        globals: true,
        coverage: {
            provider: "v8",
            all: true,
            include: ["src/**/*.{ts,tsx}"],
            reporter: ["text", "html"]
        }
    }
})
