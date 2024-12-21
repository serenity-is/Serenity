import vitestDefaults from "test-utils/vitest-defaults";
import { resolve } from "node:path";

export default {
    ...vitestDefaults({
        name: "serene",
        projectRoot: resolve(__dirname)
    })
}