import vitestDefaults from "test-utils/vitest-defaults";

export default {
    ...vitestDefaults({
        name: "extensions",
        projectRoot: import.meta.dirname
    })
}