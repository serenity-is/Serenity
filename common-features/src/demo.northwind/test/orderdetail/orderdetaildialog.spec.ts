import { DecimalEditor, ServiceLookupEditor } from "@serenity-is/corelib";
import { mockDynamicData, mockFetch, mockGridSize, mockRowLookup, unmockFetch } from "test-utils";
import { afterEach, beforeAll, beforeEach, describe, expect, it, vi } from "vitest";
import { OrderDetailDialog } from "../../Modules/OrderDetail/OrderDetailDialog";
import { OrderDetailForm, OrderDetailRow, OrderDetailService, ProductRow } from "../../Modules/ServerTypes/Demo";

beforeAll(() => {
    mockDynamicData();
    mockGridSize();
    mockRowLookup(ProductRow, [
        { ProductID: 1, ProductName: "Chai", UnitPrice: 18 },
        { ProductID: 2, ProductName: "Chang", UnitPrice: 19 }
    ]);
});

beforeEach(() => {
    mockFetch({ "*": () => ({ Entities: [], TotalCount: 0 }) });
});

afterEach(() => {
    unmockFetch();
    vi.restoreAllMocks();
    document.body.innerHTML = "";
});

describe("OrderDetailDialog", () => {
    it("wires up form, row and service", () => {
        const dialog = new OrderDetailDialog({});
        expect(dialog["getFormKey"]()).toBe(OrderDetailForm.formKey);
        expect(dialog["getRowDefinition"]()).toBe(OrderDetailRow);
        expect(dialog["getService"]()).toBe(OrderDetailService.baseUrl);
        dialog.destroy();
    });

    it("sets the unit price when the product changes", async () => {
        let selectCb: any;
        vi.spyOn(ServiceLookupEditor.prototype as any, "changeSelect2").mockImplementation((cb: any) => { selectCb = cb; });
        let ruleCb: any;
        vi.spyOn(DecimalEditor.prototype as any, "addValidationRule").mockImplementation((...args: any[]) => { ruleCb = args[args.length - 1]; });

        const dialog = new OrderDetailDialog({});
        const form: any = dialog["form"];
        const productEditor: any = { value: "1" };
        const unitPrice: any = { value: null };
        const quantity: any = { value: 2 };
        const discount: any = { value: 100 };
        Object.defineProperties(form, {
            ProductID: { value: productEditor, configurable: true },
            UnitPrice: { value: unitPrice, configurable: true },
            Quantity: { value: quantity, configurable: true },
            Discount: { value: discount, configurable: true }
        });

        await selectCb({});
        expect(unitPrice.value).toBe(18);

        expect(ruleCb()).toBeTruthy();
        discount.value = 1;
        expect(ruleCb()).toBeUndefined();
        dialog.destroy();
    });
});
