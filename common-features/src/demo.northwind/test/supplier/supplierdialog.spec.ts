import { DeleteRequest, DeleteResponse, RetrieveRequest, RetrieveResponse, SaveRequest, SaveResponse } from "@serenity-is/corelib";
import { EntityDialogWrapper, mockAdmin, mockDynamicData, mockFetch, typeText, unmockFetch } from "test-utils";
import { SupplierDialog } from "../../Modules/Supplier/SupplierDialog";
import { SupplierForm, SupplierRow, SupplierService } from "../../Modules/ServerTypes/Demo";

beforeAll(() => {
    mockDynamicData();
    mockAdmin();
});

afterEach(() => {
    unmockFetch();
});

describe("SupplierDialog", () => {
    it("wires up form, row and service", () => {
        const dialog = new SupplierDialog({});
        expect(dialog["getFormKey"]()).toBe(SupplierForm.formKey);
        expect(dialog["getRowDefinition"]()).toBe(SupplierRow);
        expect(dialog["getService"]()).toBe(SupplierService.baseUrl);
        dialog.destroy();
    });

    it("calls retrieve service on loadById", async () => {
        const fetchSpy = mockFetch({
            [SupplierService.Methods.Retrieve]: (info) => {
                expect(info.data).toStrictEqual({ EntityId: 1 } satisfies RetrieveRequest);
                return { Entity: { SupplierID: 1, CompanyName: "Exotic Liquids", ContactName: "Charlotte" } } satisfies RetrieveResponse<SupplierRow>;
            }
        });
        const dlg = new EntityDialogWrapper(new SupplierDialog());
        await new Promise((resolve, reject) => dlg.actual.loadByIdAndOpenDialog(1, void 0, resolve, reject));
        expect(fetchSpy.requests.length).toBe(1);
        expect(dlg.actual.entityId).toBe(1);
        const form = dlg.getForm(SupplierForm);
        expect(form.CompanyName.value).toBe("Exotic Liquids");
        expect(form.ContactName.value).toBe("Charlotte");
    });

    it("calls create service on save button click for new mode", async () => {
        const dlg = new EntityDialogWrapper(new SupplierDialog());
        dlg.actual.loadNewAndOpenDialog();
        const form = dlg.getForm(SupplierForm);
        typeText(form.CompanyName, "Exotic Liquids");
        typeText(form.ContactName, "Charlotte");
        const fetchSpy = mockFetch({
            [SupplierService.Methods.Create]: (info) => {
                expect((info.data as SaveRequest<SupplierRow>).Entity).toMatchObject({ CompanyName: "Exotic Liquids", ContactName: "Charlotte" });
                return { EntityId: 1 } satisfies SaveResponse;
            }
        });
        await dlg.clickSaveButton();
        expect(fetchSpy.requests.length).toBe(1);
    });

    it("calls update service on save button click for edit mode", async () => {
        const dlg = new EntityDialogWrapper(new SupplierDialog());
        dlg.actual.loadEntityAndOpenDialog({ SupplierID: 1, CompanyName: "Exotic Liquids", ContactName: "Charlotte" });
        typeText(dlg.getForm(SupplierForm).CompanyName, "Exotic Foods");
        const fetchSpy = mockFetch({
            [SupplierService.Methods.Update]: (info) => {
                expect((info.data as SaveRequest<SupplierRow>).EntityId).toBe(1);
                expect((info.data as SaveRequest<SupplierRow>).Entity).toMatchObject({ SupplierID: 1, CompanyName: "Exotic Foods", ContactName: "Charlotte" });
                return { EntityId: 1 } satisfies SaveResponse;
            }
        });
        await dlg.clickSaveButton();
        expect(fetchSpy.requests.length).toBe(1);
    });

    it("calls delete service on delete button click", async () => {
        const dlg = new EntityDialogWrapper(new SupplierDialog());
        dlg.actual.loadEntityAndOpenDialog({ SupplierID: 1, CompanyName: "Exotic Liquids" });
        const fetchSpy = mockFetch({
            [SupplierService.Methods.Delete]: (info) => {
                expect(info.data).toStrictEqual({ EntityId: 1 } satisfies DeleteRequest);
                return {} satisfies DeleteResponse;
            }
        });
        await dlg.clickDeleteButton();
        expect(fetchSpy.requests.length).toBe(1);
    });
});
