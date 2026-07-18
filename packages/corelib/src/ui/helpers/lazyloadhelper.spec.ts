import { executeEverytimeWhenVisible, executeOnceWhenVisible } from "../../compat";
import { LazyLoadHelper } from "./lazyloadhelper";

describe("LazyLoadHelper", () => {
    it("executeOnceWhenShown is an alias for executeOnceWhenVisible", () => {
        expect(LazyLoadHelper.executeOnceWhenShown).toBe(executeOnceWhenVisible);
    });

    it("executeEverytimeWhenShown is an alias for executeEverytimeWhenVisible", () => {
        expect(LazyLoadHelper.executeEverytimeWhenShown).toBe(executeEverytimeWhenVisible);
    });
});
