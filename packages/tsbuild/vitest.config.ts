import { serenityTestOptions } from "../test-utils/vitest-defaults.js";
import { configDefaults, defineConfig } from "vitest/config";

export default defineConfig({
    test: {
        name: "tsbuild",
        ...serenityTestOptions(import.meta.dirname),
        coverage: {
            exclude: [
                ...(configDefaults?.coverage?.exclude ?? []),
                "build/**",
                "src/test/**",
                "src/index.ts"
            ],
            excludeAfterRemap: true
        }
    }
})