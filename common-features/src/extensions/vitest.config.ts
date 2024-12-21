import vitestDefaults from "test-utils/vitest-defaults";

export default {
    ...vitestDefaults({
        projectRoot: __dirname,
        name: "extensions"
    })
}