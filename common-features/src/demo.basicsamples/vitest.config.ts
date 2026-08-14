import vitestDefaults from "test-utils/vitest-defaults";

export default {
    ...vitestDefaults({
        name: "basicsamples",
        projectRoot: import.meta.dirname
    })
}