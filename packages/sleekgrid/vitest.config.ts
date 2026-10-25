import { serenityTestOptions } from "../test-utils/vitest-defaults.js";
import { defineConfig } from "vitest/config";

export default defineConfig({
    test: {
        name: "sleekgrid",
        ...serenityTestOptions(import.meta.dirname),
        environment: "jsdom",
        coverage: {
            include: ["src/**/*.{ts,tsx}"],
            reporter: ["json", "html", "text"]
        },
        pool: "vmThreads"
    }
})
