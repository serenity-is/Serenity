import * as corelib from "@serenity-is/corelib";
import { LanguageGrid } from "../../../Modules/Administration/Language/LanguageGrid";
import initPage from "../../../Modules/Administration/Language/LanguagePage";

describe("LanguagePage", () => {
    it("initializes the grid page", () => {
        const spy = vi.spyOn(corelib, "gridPageInit").mockReturnValue({} as any);
        initPage();
        expect(spy).toHaveBeenCalledWith(LanguageGrid);
    });
});
