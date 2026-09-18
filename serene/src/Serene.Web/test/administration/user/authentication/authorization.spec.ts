import { setScriptData } from "@serenity-is/corelib";
import { mockAdmin } from "test-utils";
import { hasPermission, userDefinition } from "../../../../Modules/Administration/User/Authentication/Authorization";

describe("Authorization", () => {
    it("returns the current user definition", () => {
        mockAdmin();
        expect(userDefinition()).toMatchObject({ Username: "admin" });
    });

    it("grants all permissions to admin", () => {
        mockAdmin();
        expect(hasPermission("Some:Permission")).toBe(true);
    });

    it("checks permissions for non-admin users", () => {
        setScriptData("RemoteData.UserData", { Username: "john", Permissions: { "Some:Permission": true } });
        expect(userDefinition().Username).toBe("john");
        expect(hasPermission("Some:Permission")).toBe(true);
        expect(hasPermission("Other:Permission")).toBe(false);
    });
});
