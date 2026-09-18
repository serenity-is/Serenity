import vitestDefaults from "test-utils/vitest-defaults";

const defaults = vitestDefaults({
    name: "extensions",
    projectRoot: import.meta.dirname
});

export default {
    ...defaults,
    test: {
        ...defaults.test,
        coverage: {
            provider: "v8",
            all: true,
            include: ["Modules/**/*.{ts,tsx}"],
            reporter: ["text", "html"]
        }
    }
}
