import vitestDefaults from "test-utils/vitest-defaults";
import { resolve } from "node:path";

export default {
    ...vitestDefaults({
        projectRoot: resolve(__dirname),
        name: "northwind"
    })
}