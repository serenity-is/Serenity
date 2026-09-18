import * as corelib from "@serenity-is/corelib";
import { TranslationGrid } from "../../../Modules/Administration/Translation/TranslationGrid";
import initPage from "../../../Modules/Administration/Translation/TranslationPage";

describe("TranslationPage", () => {
    it("initializes the grid page", () => {
        const spy = vi.spyOn(corelib, "gridPageInit").mockReturnValue({} as any);
        initPage();
        expect(spy).toHaveBeenCalledWith(TranslationGrid);
    });
});
