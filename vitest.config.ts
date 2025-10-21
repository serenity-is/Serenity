import { defineConfig } from "vitest/config";

export default defineConfig({
    test: {
        projects: [
            "packages/*/vitest.config.ts",
            "common-features/*/vitest.config.ts",
            "serene/src/Serene.Web/vitest.config.ts"
        ]
    }
});