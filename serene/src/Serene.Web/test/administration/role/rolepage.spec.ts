import * as corelib from "@serenity-is/corelib";
import { describe, expect, it, vi } from "vitest";
import { RoleGrid } from "../../../Modules/Administration/Role/RoleGrid";
import initPage from "../../../Modules/Administration/Role/RolePage";

describe("RolePage", () => {
    it("initializes the grid page", () => {
        const spy = vi.spyOn(corelib, "gridPageInit").mockReturnValue({} as any);
        initPage();
        expect(spy).toHaveBeenCalledWith(RoleGrid);
    });
});
