import { DecimalEditor } from "@serenity-is/corelib";
import { OrderDetailRow, ProductRow } from "@serenity-is/demo.northwind";
import { GridEditorDialog } from "@serenity-is/extensions";
import { EntityDialogWrapper, mockAdmin, mockDynamicData, mockFetch, mockGridSize, mockRowLookup, unmockFetch } from "test-utils";
import { afterAll, afterEach, beforeAll, describe, expect, it, vi } from "vitest";
import initPage, { ChangingLookupTextDialog, ChangingLookupTextEditor } from "../../Modules/Editors/ChangingLookupText/ChangingLookupTextPage";
import { ChangingLookupTextForm } from "../../Modules/ServerTypes/Demo";

beforeAll(() => {
    mockDynamicData();
    mockAdmin();
    mockGridSize();
    mockRowLookup(ProductRow, [{ ProductID: 1, ProductName: "Chai", UnitPrice: 18.5, UnitsInStock: 5, SupplierCompanyName: "Exotic" }]);
    mockFetch({ "*": () => ({ Entities: [], TotalCount: 0 }) });
});

afterAll(() => {
    unmockFetch();
});

afterEach(() => {
    vi.restoreAllMocks();
    document.body.innerHTML = "";
});

describe("ChangingLookupTextPage", () => {
    it("initializes the dialog page", () => {
        const open = vi.spyOn(ChangingLookupTextDialog.prototype, "loadNewAndOpenDialog").mockImplementation((() => ({})) as any);
        initPage();
        expect(open).toHaveBeenCalled();
    });
});

describe("ChangingLookupTextEditor", () => {
    it("uses the product lookup key", () => {
        const editor = new ChangingLookupTextEditor({});
        expect(editor["getLookupKey"]()).toBe(ProductRow.lookupKey);
        editor.destroy();
    });

    it("appends price, stock and supplier to the item text", () => {
        const editor = new ChangingLookupTextEditor({});
        const text = editor["getItemText"](
            { ProductID: 1, ProductName: "Chai", UnitPrice: 18.5, UnitsInStock: 5, SupplierCompanyName: "Exotic" } as ProductRow,
            { textField: "ProductName" } as any);
        expect(text).toContain("Chai");
        expect(text).toContain("$18.50");
        expect(text).toContain("5 in stock");
        expect(text).toContain("Exotic");
        editor.destroy();
    });

    it("reports out of stock and unknown supplier", () => {
        const editor = new ChangingLookupTextEditor({});
        const text = editor["getItemText"](
            { ProductID: 1, ProductName: "Chai", UnitPrice: 1, UnitsInStock: 0 } as ProductRow,
            { textField: "ProductName" } as any);
        expect(text).toContain("out of stock");
        expect(text).toContain("Unknown");
        editor.destroy();
    });
});

describe("ChangingLookupTextDialog", () => {
    it("extends the grid editor dialog and wires form", () => {
        const wrapper = new EntityDialogWrapper(new ChangingLookupTextDialog({}));
        expect(wrapper.actual instanceof GridEditorDialog).toBe(true);
        expect(wrapper.actual["getFormKey"]()).toBe(ChangingLookupTextForm.formKey);
        expect(wrapper.actual["getLocalTextPrefix"]()).toBe(OrderDetailRow.localTextPrefix);
        expect(wrapper.actual["getDialogOptions"]().modal).toBe(false);
        wrapper.actual.destroy();
    });

    it("hides apply changes and save and close buttons", () => {
        const wrapper = new EntityDialogWrapper(new ChangingLookupTextDialog({}));
        const hide = vi.spyOn(wrapper.actual["toolbar"], "findButton").mockReturnValue({ hide: vi.fn() } as any);
        wrapper.actual["updateInterface"]();
        expect(hide).toHaveBeenCalledWith("apply-changes-button");
        expect(hide).toHaveBeenCalledWith("save-and-close-button");
        wrapper.actual.destroy();
    });

    it("updates unit price when a product is selected", async () => {
        const wrapper = new EntityDialogWrapper(new ChangingLookupTextDialog({}));
        const form = wrapper.actual["form"] as ChangingLookupTextForm;
        form.ProductID.value = 1 as any;
        form.ProductID.element.trigger("change");
        await new Promise(resolve => setTimeout(resolve, 20));
        expect(form.UnitPrice.value).toBe(18.5);
        wrapper.actual.destroy();
    });

    it("does not update unit price when the product is cleared", async () => {
        const wrapper = new EntityDialogWrapper(new ChangingLookupTextDialog({}));
        const form = wrapper.actual["form"] as ChangingLookupTextForm;
        form.UnitPrice.value = 99 as any;
        form.ProductID.value = null;
        form.ProductID.element.trigger("change");
        await new Promise(resolve => setTimeout(resolve, 20));
        expect(form.UnitPrice.value).toBe(99);
        wrapper.actual.destroy();
    });

    it("validates that discount is not higher than total price", () => {
        let rule: any;
        vi.spyOn(DecimalEditor.prototype, "addValidationRule").mockImplementation((_uniqueName: string, r: any) => { rule = r; });
        const wrapper = new EntityDialogWrapper(new ChangingLookupTextDialog({}));
        const form = wrapper.actual["form"] as ChangingLookupTextForm;
        form.UnitPrice.value = 10 as any;
        form.Quantity.value = 2;
        form.Discount.value = 25;
        expect(rule()).toContain("Discount can't be higher than total price!");
        form.Discount.value = 1;
        expect(rule()).toBeUndefined();
        wrapper.actual.destroy();
    });
});
