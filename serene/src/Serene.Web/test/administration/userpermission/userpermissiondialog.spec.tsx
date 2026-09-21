import { setScriptData } from "@serenity-is/corelib";
import { mockAdmin, mockDynamicData, mockFetch, unmockFetch } from "test-utils";
import { afterEach, beforeAll, beforeEach, describe, expect, it, vi } from "vitest";
import { UserPermissionDialog } from "../../../Modules/Administration/UserPermission/UserPermissionDialog";
import { UserPermissionService } from "../../../Modules/ServerTypes/Administration";
import { RemoteDataKeys } from "../../../Modules/ServerTypes/RemoteDataKeys";

beforeAll(() => {
    mockDynamicData();
    mockAdmin();
});

beforeEach(() => {
    setScriptData("RemoteData." + RemoteDataKeys.Administration.PermissionKeys, ["Administration:", "Administration:Users"]);
    setScriptData("RemoteData." + RemoteDataKeys.Administration.ImplicitPermissions, { "Administration:Users": [] });
    mockFetch({
        [UserPermissionService.Methods.List]: () => ({ Entities: [{ PermissionKey: "Administration:Users", Granted: true }] }),
        [UserPermissionService.Methods.ListRolePermissions]: () => ({ Entities: ["Administration:Users"] }),
        [UserPermissionService.Methods.Update]: () => ({})
    });
});

afterEach(() => {
    unmockFetch();
    vi.restoreAllMocks();
    document.body.innerHTML = "";
});

describe("UserPermissionDialog", () => {
    it("lists user and role permissions and saves on ok", async () => {
        await UserPermissionDialog({ userID: 1, username: "admin" });

        const dialog = document.querySelector(".s-UserPermissionDialog");
        expect(dialog).toBeTruthy();

        const okButton = dialog.querySelector(".btn-info") as HTMLElement;
        expect(okButton).toBeTruthy();
        okButton.click();

        await new Promise(resolve => setTimeout(resolve, 20));
        const fetchSpy = (window.fetch as any);
        expect(fetchSpy.requests.some((x: any) => x.url.indexOf("Update") >= 0)).toBe(true);
    });
});
