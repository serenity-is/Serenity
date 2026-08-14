import vitestDefaults from "test-utils/vitest-defaults";

export default {
    ...vitestDefaults({
        name: "northwind",
        projectRoot: import.meta.dirname
    })
}