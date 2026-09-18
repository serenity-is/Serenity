import { DeleteRequest, DeleteResponse, RetrieveRequest, RetrieveResponse, SaveRequest, SaveResponse } from "@serenity-is/corelib";
import { EntityDialogWrapper, mockAdmin, mockDynamicData, mockFetch, typeText, unmockFetch } from "test-utils";
import { CategoryDialog } from "../../Modules/Category/CategoryDialog";
import { CategoryForm, CategoryRow, CategoryService } from "../../Modules/ServerTypes/Demo";

beforeAll(() => {
    mockDynamicData();
    mockAdmin();
});

afterEach(() => {
    unmockFetch();
});

describe("CategoryDialog", () => {
    it("can load new entity and open", () => {
        const dialog = new CategoryDialog();
        dialog.loadNewAndOpenDialog();
        expect(dialog.dialogOpen).toBeTruthy();
        expect(dialog["getFormKey"]()).toBe(CategoryForm.formKey);
        expect(dialog["getRowDefinition"]()).toBe(CategoryRow);
        expect(dialog["getService"]()).toBe(CategoryService.baseUrl);
    });

    it("calls retrieve service on loadById", async () => {
        const fetchSpy = mockFetch({
            [CategoryService.Methods.Retrieve]: (info) => {
                expect(info.data).toStrictEqual({ EntityId: 1 } satisfies RetrieveRequest);
                return {
                    Entity: { CategoryID: 1, CategoryName: "Beverages", Description: "Soft drinks" }
                } satisfies RetrieveResponse<CategoryRow>;
            }
        });
        const dlg = new EntityDialogWrapper(new CategoryDialog());
        await new Promise((resolve, reject) => dlg.actual.loadByIdAndOpenDialog(1, void 0, resolve, reject));
        expect(fetchSpy.requests.length).toBe(1);
        expect(dlg.actual.entityId).toBe(1);
        const form = dlg.getForm(CategoryForm);
        expect(form.CategoryName.value).toBe("Beverages");
    });

    it("calls create service on save button click for new mode", async () => {
        const dlg = new EntityDialogWrapper(new CategoryDialog());
        dlg.actual.loadNewAndOpenDialog();
        const form = dlg.getForm(CategoryForm);
        typeText(form.CategoryName, "Beverages");
        typeText(form.Description, "Soft drinks");
        const fetchSpy = mockFetch({
            [CategoryService.Methods.Create]: (info) => {
                expect(info.data).toStrictEqual({
                    Entity: { CategoryName: "Beverages", Description: "Soft drinks" }
                } satisfies SaveRequest<CategoryRow>);
                return { EntityId: 1 } satisfies SaveResponse;
            }
        });
        await dlg.clickSaveButton();
        expect(fetchSpy.requests.length).toBe(1);
    });

    it("calls update service on save button click for edit mode", async () => {
        const dlg = new EntityDialogWrapper(new CategoryDialog());
        dlg.actual.loadEntityAndOpenDialog({ CategoryID: 1, CategoryName: "Beverages", Description: "Soft drinks" });
        const form = dlg.getForm(CategoryForm);
        typeText(form.CategoryName, "Drinks");
        const fetchSpy = mockFetch({
            [CategoryService.Methods.Update]: (info) => {
                expect(info.data).toStrictEqual({
                    EntityId: 1,
                    Entity: { CategoryID: 1, CategoryName: "Drinks", Description: "Soft drinks" }
                } satisfies SaveRequest<CategoryRow>);
                return { EntityId: 1 } satisfies SaveResponse;
            }
        });
        await dlg.clickSaveButton();
        expect(fetchSpy.requests.length).toBe(1);
    });

    it("calls delete service on delete button click", async () => {
        const dlg = new EntityDialogWrapper(new CategoryDialog());
        dlg.actual.loadEntityAndOpenDialog({ CategoryID: 1, CategoryName: "Beverages" });
        const fetchSpy = mockFetch({
            [CategoryService.Methods.Delete]: (info) => {
                expect(info.data).toStrictEqual({ EntityId: 1 } satisfies DeleteRequest);
                return {} satisfies DeleteResponse;
            }
        });
        await dlg.clickDeleteButton();
        expect(fetchSpy.requests.length).toBe(1);
    });
});
