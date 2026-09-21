import { DeleteRequest, DeleteResponse, Dialog, RetrieveRequest, RetrieveResponse, SaveRequest, SaveResponse } from "@serenity-is/corelib";
import { EntityDialogWrapper, mockAdmin, mockDynamicData, mockFetch, typeText, unmockFetch } from "test-utils";
import { afterEach, beforeAll, describe, expect, it, vi } from "vitest";
import { RoleDialog } from "../../../Modules/Administration/Role/RoleDialog";
import * as RolePermissionModule from "../../../Modules/Administration/RolePermission/RolePermissionDialog";
import { RoleForm, RoleRow, RoleService } from "../../../Modules/ServerTypes/Administration";

beforeAll(() => {
    mockDynamicData();
    mockAdmin();
});

afterEach(() => {
    unmockFetch();
});

describe("RoleDialog", () => {
    it("can load new entity and open", () => {
        const dlg = new RoleDialog();
        dlg.loadNewAndOpenDialog();
        const instance = Dialog.getInstance(dlg.element);
        expect(instance).toBeTruthy();
        expect(instance.getDialogNode()).toBeTruthy();
    });

    it("calls retrieve service on loadById", async () => {
        const fetchSpy = mockFetch({
            [RoleService.Methods.Retrieve]: (info) => {
                expect(info.data).toStrictEqual({
                    EntityId: 7
                } satisfies RetrieveRequest);
                return {
                    Entity: {
                        RoleId: 7,
                        RoleName: "MyRole"
                    }
                } satisfies RetrieveResponse<RoleRow>;
            }
        });
        const dlg = new EntityDialogWrapper(new RoleDialog());
        await new Promise((resolve, reject) => dlg.actual.loadByIdAndOpenDialog(7, void 0, resolve, reject));
        expect(fetchSpy.requests.length).toBe(1);
        expect(dlg.actual.entityId).toBe(7);
        const form = dlg.getForm(RoleForm);
        expect(form.RoleName.value).toBe("MyRole");
    });

    it("calls create service on save button click for new mode", async () => {
        const dlg = new EntityDialogWrapper(new RoleDialog());
        dlg.actual.loadNewAndOpenDialog();
        const form = dlg.getForm(RoleForm);
        typeText(form.RoleName, "MyRole");
        const fetchSpy = mockFetch({
            [RoleService.Methods.Create]: (info) => {
                expect(info.data).toStrictEqual({
                    Entity: {
                        RoleName: "MyRole"
                    }
                } satisfies SaveRequest<RoleRow>);
                return { EntityId: 1 };
            }
        });
        await dlg.clickSaveButton();
        expect(fetchSpy.requests.length).toBe(1);
    });

    it("calls update service on save button click for edit mode", async () => {
        const dlg = new EntityDialogWrapper(new RoleDialog());
        dlg.actual.loadEntityAndOpenDialog({
            RoleId: 7,
            RoleName: "MyRole"
        });

        const form = dlg.getForm(RoleForm);
        typeText(form.RoleName, "UpdatedMyRole");

        const fetchSpy = mockFetch({
            [RoleService.Methods.Update]: (info) => {
                expect(info.data).toStrictEqual({
                    EntityId: 7,
                    Entity: {
                        RoleId: 7,
                        RoleName: "UpdatedMyRole"
                    }
                } satisfies SaveRequest<RoleRow>);
                return { EntityId: 7 } satisfies SaveResponse;
            }
        });
        await dlg.clickSaveButton();
        expect(fetchSpy.requests.length).toBe(1);
    });

    it("calls delete service on delete button click", async () => {
        const dlg = new EntityDialogWrapper(new RoleDialog());
        dlg.actual.loadEntityAndOpenDialog({
            RoleId: 7,
            RoleName: "MyRole"
        });

        const fetchSpy = mockFetch({
            [RoleService.Methods.Delete]: (info) => {
                expect(info.data).toStrictEqual({
                    EntityId: 7
                } satisfies DeleteRequest);
                return {} satisfies DeleteResponse;
            }
        });
        await dlg.clickDeleteButton();
        expect(fetchSpy.requests.length).toBe(1);
    });

    it("disables the edit-permissions button for a new entity and enables it when loaded", () => {
        const dlg = new EntityDialogWrapper(new RoleDialog());
        dlg.actual.loadNewAndOpenDialog();
        expect(dlg.actual["toolbar"].findButton("edit-permissions").hasClass("disabled")).toBe(true);

        dlg.actual.loadEntityAndOpenDialog({ RoleId: 7, RoleName: "MyRole" });
        expect(dlg.actual["toolbar"].findButton("edit-permissions").hasClass("disabled")).toBe(false);
    });

    it("opens the role permission dialog from the edit-permissions button", () => {
        const spy = vi.spyOn(RolePermissionModule, "RolePermissionDialog").mockResolvedValue(undefined);
        const dlg = new EntityDialogWrapper(new RoleDialog());
        dlg.actual.loadEntityAndOpenDialog({ RoleId: 7, RoleName: "MyRole" });
        dlg.actual["toolbar"].findButton("edit-permissions").click();
        expect(spy).toHaveBeenCalledWith({ roleID: 7, roleName: "MyRole" });
    });

});