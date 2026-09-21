import { setScriptData } from "@serenity-is/corelib";
import { mockAdmin, mockDynamicData, mockFetch, unmockFetch } from "test-utils";
import { afterEach, beforeAll, beforeEach, describe, expect, it, vi } from "vitest";
import { RolePermissionDialog } from "../../../Modules/Administration/RolePermission/RolePermissionDialog";
import { RolePermissionService } from "../../../Modules/ServerTypes/Administration";
import { RemoteDataKeys } from "../../../Modules/ServerTypes/RemoteDataKeys";

beforeAll(() => {
    mockDynamicData();
    mockAdmin();
});

beforeEach(() => {
    setScriptData("RemoteData." + RemoteDataKeys.Administration.PermissionKeys, ["Administration:", "Administration:Users"]);
    setScriptData("RemoteData." + RemoteDataKeys.Administration.ImplicitPermissions, {});
    mockFetch({
        [RolePermissionService.Methods.List]: () => ({ Entities: [{ PermissionKey: "Administration:Users" }] }),
        [RolePermissionService.Methods.Update]: () => ({})
    });
});

afterEach(() => {
    unmockFetch();
    vi.restoreAllMocks();
    document.body.innerHTML = "";
});

describe("RolePermissionDialog", () => {
    it("lists permissions and saves on ok", async () => {
        await RolePermissionDialog({ roleID: 7, roleName: "MyRole" });

        const dialog = document.querySelector(".s-RolePermissionDialog");
        expect(dialog).toBeTruthy();

        const okButton = dialog.querySelector(".btn-info") as HTMLElement;
        expect(okButton).toBeTruthy();
        okButton.click();

        await new Promise(resolve => setTimeout(resolve, 20));
        const fetchSpy = (window.fetch as any);
        expect(fetchSpy.requests.some((x: any) => x.url.indexOf("Update") >= 0)).toBe(true);
    });
});
