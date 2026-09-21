import { defineConfig } from "vitest/config";

export default defineConfig({
    test: {
        name: "sleekgrid",
        environment: "jsdom",
        coverage: {
            include: ["src/**/*.{ts,tsx}"],
            reporter: ["json", "html", "text"]
        },
        pool: "vmThreads"
    }
})
