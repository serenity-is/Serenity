import vitestDefaults from "test-utils/vitest-defaults";

export default {
    ...vitestDefaults({
        name: "serene",
        projectRoot: import.meta.dirname
    })
}