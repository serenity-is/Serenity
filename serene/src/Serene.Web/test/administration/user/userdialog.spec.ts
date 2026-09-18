import { DeleteRequest, DeleteResponse, RetrieveRequest, RetrieveResponse, SaveRequest, SaveResponse } from "@serenity-is/corelib";
import { EntityDialogWrapper, mockAdmin, mockDynamicData, mockFetch, mockRowLookup, typeText, unmockFetch } from "test-utils";
import { UserDialog } from "../../../Modules/Administration/User/UserDialog";
import * as UserPermissionModule from "../../../Modules/Administration/UserPermission/UserPermissionDialog";
import { RoleRow, UserForm, UserRow, UserService } from "../../../Modules/ServerTypes/Administration";

beforeAll(() => {
    mockDynamicData();
    mockAdmin();
});

beforeEach(() => {
    mockRowLookup(RoleRow, [{ RoleId: 1, RoleName: "Admin" }] as any);
});

afterEach(() => {
    unmockFetch();
    vi.restoreAllMocks();
    document.body.innerHTML = "";
});

describe("UserDialog", () => {
    it("can load new entity and open", () => {
        const dlg = new UserDialog();
        dlg.loadNewAndOpenDialog();
        expect(dlg["toolbar"].findButton("edit-permissions-button").hasClass("disabled")).toBe(true);
    });

    it("toggles password required state on new versus loaded entity", () => {
        const dlg = new UserDialog();
        dlg.loadNewAndOpenDialog();
        const form = new UserForm(dlg.idPrefix);
        expect(form.Password.element.hasClass("required")).toBe(true);
        expect(form.PasswordConfirm.element.hasClass("required")).toBe(true);

        dlg.loadEntityAndOpenDialog({ UserId: 1, Username: "admin" });
        expect(form.Password.element.hasClass("required")).toBe(false);
        expect(form.PasswordConfirm.element.hasClass("required")).toBe(false);
        expect(dlg["toolbar"].findButton("edit-permissions-button").hasClass("disabled")).toBe(false);
    });

    it("calls retrieve service on loadById", async () => {
        const fetchSpy = mockFetch({
            [UserService.Methods.Retrieve]: (info) => {
                expect(info.data).toStrictEqual({ EntityId: 7 } satisfies RetrieveRequest);
                return { Entity: { UserId: 7, Username: "john" } } satisfies RetrieveResponse<UserRow>;
            }
        });
        const dlg = new EntityDialogWrapper(new UserDialog());
        await new Promise((resolve, reject) => dlg.actual.loadByIdAndOpenDialog(7, void 0, resolve, reject));
        expect(fetchSpy.requests.length).toBe(1);
        expect(new UserForm(dlg.actual.idPrefix).Username.value).toBe("john");
    });

    it("calls create service on save for new mode", async () => {
        const dlg = new EntityDialogWrapper(new UserDialog());
        dlg.actual.loadNewAndOpenDialog();
        const form = dlg.getForm(UserForm);
        typeText(form.Username, "john");
        typeText(form.DisplayName, "John");
        typeText(form.Password, "123456");
        typeText(form.PasswordConfirm, "123456");
        const fetchSpy = mockFetch({
            [UserService.Methods.Create]: () => ({ EntityId: 1 } satisfies SaveResponse)
        });
        await dlg.clickSaveButton();
        expect(fetchSpy.requests.length).toBe(1);
    });

    it("calls update service on save for edit mode", async () => {
        const dlg = new EntityDialogWrapper(new UserDialog());
        dlg.actual.loadEntityAndOpenDialog({ UserId: 7, Username: "john", DisplayName: "John" });
        const form = dlg.getForm(UserForm);
        typeText(form.Username, "john2");
        const fetchSpy = mockFetch({
            [UserService.Methods.Update]: (info) => {
                expect(info.data).toMatchObject({ EntityId: 7 } satisfies Partial<SaveRequest<UserRow>>);
                return { EntityId: 7 } satisfies SaveResponse;
            }
        });
        await dlg.clickSaveButton();
        expect(fetchSpy.requests.length).toBe(1);
    });

    it("calls delete service on delete", async () => {
        const dlg = new EntityDialogWrapper(new UserDialog());
        dlg.actual.loadEntityAndOpenDialog({ UserId: 7, Username: "john" });
        const fetchSpy = mockFetch({
            [UserService.Methods.Delete]: (info) => {
                expect(info.data).toStrictEqual({ EntityId: 7 } satisfies DeleteRequest);
                return {} satisfies DeleteResponse;
            }
        });
        await dlg.clickDeleteButton();
        expect(fetchSpy.requests.length).toBe(1);
    });

    it("toggles password confirm required when password changes", () => {
        const dlg = new UserDialog();
        dlg.loadNewAndOpenDialog();
        const form = new UserForm(dlg.idPrefix);
        typeText(form.Password, "secret");
        expect(form.PasswordConfirm.element.hasClass("required")).toBe(true);
    });

    it("validates password length and confirmation", () => {
        const dlg = new UserDialog();
        dlg.loadNewAndOpenDialog();
        const form = new UserForm(dlg.idPrefix);
        typeText(form.Username, "john");
        typeText(form.DisplayName, "John");
        typeText(form.Password, "123");
        typeText(form.PasswordConfirm, "123");
        expect(dlg["validateForm"]()).toBe(false);

        typeText(form.Password, "123456");
        typeText(form.PasswordConfirm, "654321");
        expect(dlg["validateForm"]()).toBe(false);

        typeText(form.PasswordConfirm, "123456");
        expect(dlg["validateForm"]()).toBe(true);
    });

    it("opens user permission dialog from toolbar", () => {
        const spy = vi.spyOn(UserPermissionModule, "UserPermissionDialog").mockResolvedValue(undefined);
        const dlg = new UserDialog();
        dlg.loadEntityAndOpenDialog({ UserId: 7, Username: "john" });
        dlg["toolbar"].findButton("edit-permissions-button").click();
        expect(spy).toHaveBeenCalledWith({ userID: 7, username: "john" });
    });
});
