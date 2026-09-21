import { DeleteRequest, DeleteResponse, RetrieveRequest, RetrieveResponse } from "@serenity-is/corelib";
import { EntityDialogWrapper, mockAdmin, mockDynamicData, mockFetch, mockRowLookup, unmockFetch } from "test-utils";
import { afterAll, afterEach, beforeAll, describe, expect, it, vi } from "vitest";
import { ProductDialog } from "../../Modules/Product/ProductDialog";
import { CategoryRow, ProductForm, ProductRow, ProductService, SupplierRow } from "../../Modules/ServerTypes/Demo";

beforeAll(() => {
    mockDynamicData();
    mockAdmin();
    mockRowLookup(CategoryRow, [{ CategoryID: 1, CategoryName: "Beverages" }]);
    mockRowLookup(SupplierRow, [{ SupplierID: 1, CompanyName: "Exotic Liquids" }]);
    mockFetch({ "*": () => ({ Entities: [], TotalCount: 0 }) });
});

afterAll(() => {
    unmockFetch();
});

afterEach(() => {
    vi.restoreAllMocks();
    document.body.innerHTML = "";
});

describe("ProductDialog", () => {
    it("wires up form, row and service", () => {
        const dialog = new ProductDialog({});
        expect(dialog["getFormKey"]()).toBe(ProductForm.formKey);
        expect(dialog["getRowDefinition"]()).toBe(ProductRow);
        expect(dialog["getService"]()).toBe(ProductService.baseUrl);
        dialog.destroy();
    });

    it("calls retrieve service on loadById", async () => {
        const fetchSpy = mockFetch({
            [ProductService.Methods.Retrieve]: (info) => {
                expect(info.data).toStrictEqual({ EntityId: 1 } satisfies RetrieveRequest);
                return { Entity: { ProductID: 1, ProductName: "Chai" } } satisfies RetrieveResponse<ProductRow>;
            }
        });
        const dlg = new EntityDialogWrapper(new ProductDialog());
        await new Promise((resolve, reject) => dlg.actual.loadByIdAndOpenDialog(1, void 0, resolve, reject));
        expect(fetchSpy.requests.filter(r => r.url.includes("/Product/Retrieve")).length).toBe(1);
        expect(dlg.actual.entityId).toBe(1);
        expect(dlg.getForm(ProductForm).ProductName.value).toBe("Chai");
    });

    it("calls delete service on delete button click", async () => {
        const dlg = new EntityDialogWrapper(new ProductDialog());
        dlg.actual.loadEntityAndOpenDialog({ ProductID: 1, ProductName: "Chai" });
        const fetchSpy = mockFetch({
            [ProductService.Methods.Delete]: (info) => {
                expect(info.data).toStrictEqual({ EntityId: 1 } satisfies DeleteRequest);
                return {} satisfies DeleteResponse;
            }
        });
        await dlg.clickDeleteButton();
        expect(fetchSpy.requests.filter(r => r.url.includes("/Product/Delete")).length).toBe(1);
    });
});
