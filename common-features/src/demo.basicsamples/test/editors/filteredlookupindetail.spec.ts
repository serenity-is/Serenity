import * as corelib from "@serenity-is/corelib";
import { OrderDetailDialog, OrderGrid, OrderRow, OrderService, ProductRow } from "@serenity-is/demo.northwind";
import { EntityDialogWrapper, mockAdmin, mockDynamicData, mockFetch, mockGridSize, unmockFetch } from "test-utils";
import { afterAll, afterEach, beforeAll, describe, expect, it, vi } from "vitest";
import initPage, {
    FilteredLookupDetailEditor,
    FilteredLookupInDetailDialog,
    FilteredLookupInDetailGrid,
    FilteredLookupOrderDetailDialog
} from "../../Modules/Editors/FilteredLookupInDetail/FilteredLookupInDetailPage";
import { FilteredLookupInDetailForm } from "../../Modules/ServerTypes/Demo";

beforeAll(() => {
    mockDynamicData();
    mockAdmin();
    mockGridSize();
    mockFetch({ "*": () => ({ Entities: [], TotalCount: 0 }) });
});

afterAll(() => {
    unmockFetch();
});

afterEach(() => {
    vi.restoreAllMocks();
    document.body.innerHTML = "";
});

describe("FilteredLookupInDetailPage", () => {
    it("initializes the grid page", () => {
        const spy = vi.spyOn(corelib, "gridPageInit").mockReturnValue({} as any);
        initPage();
        expect(spy).toHaveBeenCalledWith(FilteredLookupInDetailGrid);
    });
});

describe("FilteredLookupInDetailGrid", () => {
    it("extends the order grid and uses the custom dialog", () => {
        const grid = new FilteredLookupInDetailGrid({});
        expect(grid instanceof OrderGrid).toBe(true);
        expect(grid["getDialogType"]()).toBe(FilteredLookupInDetailDialog);
        grid.destroy();
    });
});

describe("FilteredLookupOrderDetailDialog", () => {
    it("sets the product cascade field and value", () => {
        const dialog = new FilteredLookupOrderDetailDialog({});
        expect(dialog instanceof OrderDetailDialog).toBe(true);
        expect(dialog["form"].ProductID.cascadeField).toBe(ProductRow.Fields.CategoryID);
        dialog.categoryID = 3;
        dialog["beforeLoadEntity"]({});
        expect(dialog["form"].ProductID.cascadeValue).toBe(3);
        dialog.destroy();
    });
});

describe("FilteredLookupDetailEditor", () => {
    it("uses the custom detail dialog", () => {
        const editor = new FilteredLookupDetailEditor({});
        expect(editor["getDialogType"]()).toBe(FilteredLookupOrderDetailDialog);
        editor.destroy();
    });

    it("passes the category id to the detail dialog", () => {
        const editor = new FilteredLookupDetailEditor({});
        editor.categoryID = 7;
        const dialog = new FilteredLookupOrderDetailDialog({});
        editor["initEntityDialog"](editor["getItemType"](), dialog);
        expect(dialog.categoryID).toBe(7);
        dialog.destroy();
        editor.destroy();
    });
});

describe("FilteredLookupInDetailDialog", () => {
    it("wires up form, row and service", () => {
        const dialog = new FilteredLookupInDetailDialog({});
        expect(dialog["getFormKey"]()).toBe(FilteredLookupInDetailForm.formKey);
        expect(dialog["getRowDefinition"]()).toBe(OrderRow);
        expect(dialog["getService"]()).toBe(OrderService.baseUrl);
        dialog.destroy();
    });

    it("passes the category id to the detail list on change", () => {
        const wrapper = new EntityDialogWrapper(new FilteredLookupInDetailDialog({}));
        const form = wrapper.actual["form"];
        form.CategoryID.element.val("2");
        form.CategoryID.element.trigger("change");
        expect(form.DetailList.categoryID).toBe(2);
        wrapper.actual.destroy();
    });
});
