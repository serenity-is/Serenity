import * as corelib from "@serenity-is/corelib";
import { UserGrid } from "../../../Modules/Administration/User/UserGrid";
import initPage from "../../../Modules/Administration/User/UserPage";

describe("UserPage", () => {
    it("initializes the grid page", () => {
        const spy = vi.spyOn(corelib, "gridPageInit").mockReturnValue({} as any);
        initPage();
        expect(spy).toHaveBeenCalledWith(UserGrid);
    });
});
