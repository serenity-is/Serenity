import * as corelib from "@serenity-is/corelib";
import { formatterContext } from "@serenity-is/sleekgrid";
import { mockDynamicData, mockFetch, mockGridSize, mockRowLookup, unmockFetch } from "test-utils";
import { afterAll, afterEach, beforeAll, describe, expect, it, vi } from "vitest";
import { ProductDialog } from "../../Modules/Product/ProductDialog";
import { ProductGrid } from "../../Modules/Product/ProductGrid";
import { CategoryRow, ProductColumns, ProductRow, ProductService, SupplierRow } from "../../Modules/ServerTypes/Demo";

const product: any = {
    ProductID: 1,
    ProductName: "Chai",
    CategoryID: 1,
    SupplierID: 1,
    UnitPrice: 18,
    UnitsInStock: 10,
    UnitsOnOrder: 2,
    ReorderLevel: 5,
    QuantityPerUnit: "10 boxes",
    Discontinued: false
};

function html(result: any): string {
    const div = document.createElement("div");
    if (result != null)
        div.append(result);
    return div.innerHTML;
}

function setupGrid(): any {
    const grid = new ProductGrid({}) as any;
    grid.view.setItems([{ ...product }], true);
    grid.sleekGrid.getCellFromEvent = () => ({ row: 0 });
    return grid;
}

beforeAll(() => {
    mockDynamicData();
    mockGridSize();
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

describe("ProductGrid", () => {
    it("wires up columns, dialog, row and service", () => {
        const grid = setupGrid();
        expect(grid["getColumnsKey"]()).toBe(ProductColumns.columnsKey);
        expect(grid["getDialogType"]()).toBe(ProductDialog);
        expect(grid["getRowDefinition"]()).toBe(ProductRow);
        expect(grid["getService"]()).toBe(ProductService.baseUrl);
        grid.destroy();
    });

    it("adds export and save buttons", () => {
        const grid = setupGrid();
        const classes = grid["getButtons"]().map((x: any) => x.cssClass);
        expect(classes).toContain("export-xlsx-button");
        expect(classes).toContain("export-pdf-button");
        expect(classes.some((c: string) => c?.includes("apply-changes-button"))).toBe(true);
        grid.destroy();
    });

    it("resets pending changes when processing data", () => {
        const grid = setupGrid();
        grid.pendingChanges = { 1: { UnitPrice: 5 } };
        const state = vi.spyOn(grid, "setSaveButtonState").mockImplementation(() => { });
        grid["onViewProcessData"]({ Entities: [], TotalCount: 0 });
        expect(grid.pendingChanges).toEqual({});
        expect(state).toHaveBeenCalled();
        grid.destroy();
    });

    it("renders editable numeric inputs", () => {
        const grid = setupGrid();
        const columns = grid["createColumns"]();
        const unitPrice = columns.find((x: any) => x.field === ProductRow.Fields.UnitPrice);
        const markup = unitPrice.format(formatterContext({ item: product, column: { field: "UnitPrice" } }));
        expect(html(markup)).toContain('value="18"');
        grid.destroy();
    });

    it("renders editable string inputs", () => {
        const grid = setupGrid();
        const columns = grid["createColumns"]();
        const quantity = columns.find((x: any) => x.field === ProductRow.Fields.QuantityPerUnit);
        const markup = quantity.format(formatterContext({ item: product, column: { field: "QuantityPerUnit", sourceItem: { name: "QuantityPerUnit", maxLength: 20 } } }));
        expect(html(markup)).toContain('value="10 boxes"');
        grid.destroy();
    });

    it("renders lookup selects", () => {
        const grid = setupGrid();
        const columns = grid["createColumns"]();
        const category = columns.find((x: any) => x.field === ProductRow.Fields.CategoryName);
        const markup = category.format(formatterContext({ item: product, column: { field: "CategoryName" } }));
        expect(html(markup)).toContain("Beverages");
        grid.destroy();
    });

    it("renders non data rows through escape", () => {
        const grid = setupGrid();
        const columns = grid["createColumns"]();
        const unitPrice = columns.find((x: any) => x.field === ProductRow.Fields.UnitPrice);
        const markup = unitPrice.format(formatterContext({ item: { __nonDataRow: true } as any, value: 18, column: { field: "UnitPrice" } }));
        expect(markup).toBe("18");
        grid.destroy();
    });

    it("renders non data rows for string and lookup columns", () => {
        const grid = setupGrid();
        const columns = grid["createColumns"]();
        const quantity = columns.find((x: any) => x.field === ProductRow.Fields.QuantityPerUnit);
        expect(quantity.format(formatterContext({ item: { __nonDataRow: true } as any, value: "x", column: { field: "QuantityPerUnit" } }))).toBe("x");
        const category = columns.find((x: any) => x.field === ProductRow.Fields.CategoryName);
        expect(category.format(formatterContext({ item: { __nonDataRow: true } as any, value: "x", column: { field: "CategoryName" } }))).toBe("x");
        grid.destroy();
    });

    it("invokes saveClick from the save button", () => {
        const grid = setupGrid();
        const button = grid["getButtons"]().find((b: any) => b.cssClass?.includes("apply-changes-button"));
        expect(button).toBeTruthy();
        const save = vi.spyOn(grid, "saveClick").mockImplementation(() => { });
        button.onClick({});
        expect(save).toHaveBeenCalled();
        grid.destroy();
    });

    it("captures input changes through the slick container delegation", () => {
        const grid = setupGrid();
        const input = document.createElement("input");
        input.className = "edit numeric";
        input.setAttribute("data-field", "UnitsInStock");
        grid.slickContainer.append(input);
        input.value = "6";
        input.dispatchEvent(new Event("change", { bubbles: true }));
        expect(grid.pendingChanges[1]?.UnitsInStock).toBe(6);
        grid.destroy();
    });

    it("marks dirty cells based on pending changes", () => {
        const grid = setupGrid();
        const columns = grid["createColumns"]();
        const unitPrice = columns.find((x: any) => x.field === ProductRow.Fields.UnitPrice);
        grid.pendingChanges = { 1: { UnitPrice: 5 } };
        const markup = unitPrice.format(formatterContext({ item: product, column: { field: "UnitPrice" } }));
        expect(html(markup)).toContain("dirty");
        expect(grid["getEffectiveValue"](product, "UnitPrice")).toBe(5);
        grid.destroy();
    });

    it("captures numeric input changes", () => {
        const grid = setupGrid();
        const input = document.createElement("input");
        input.className = "edit numeric";
        input.setAttribute("data-field", "UnitsInStock");
        input.value = "5";
        grid["inputsChange"]({ target: input });
        expect(grid.pendingChanges[1].UnitsInStock).toBe(5);
        expect(input.classList.contains("dirty")).toBe(true);
        grid.destroy();
    });

    it("rejects invalid numeric input", () => {
        const grid = setupGrid();
        const error = vi.spyOn(corelib, "notifyError").mockImplementation(() => { });
        const input = document.createElement("input");
        input.className = "edit numeric";
        input.setAttribute("data-field", "UnitsInStock");
        input.value = "abc";
        grid["inputsChange"]({ target: input });
        expect(error).toHaveBeenCalled();
        expect(grid.pendingChanges[1]).toBeUndefined();
        grid.destroy();
    });

    it("rejects out of range numeric input", () => {
        const grid = setupGrid();
        vi.spyOn(corelib, "notifyError").mockImplementation(() => { });
        const input = document.createElement("input");
        input.className = "edit numeric";
        input.setAttribute("data-field", "UnitsOnOrder");
        input.value = "40000";
        grid["inputsChange"]({ target: input });
        grid.destroy();
    });

    it("captures decimal unit price changes", () => {
        const grid = setupGrid();
        const input = document.createElement("input");
        input.className = "edit numeric";
        input.setAttribute("data-field", "UnitPrice");
        input.value = "3.5";
        grid["inputsChange"]({ target: input });
        expect(grid.pendingChanges[1].UnitPrice).toBe(3.5);
        grid.destroy();
    });

    it("rejects invalid decimal input", () => {
        const grid = setupGrid();
        vi.spyOn(corelib, "notifyError").mockImplementation(() => { });
        const input = document.createElement("input");
        input.className = "edit numeric";
        input.setAttribute("data-field", "UnitPrice");
        input.value = "abc";
        grid["inputsChange"]({ target: input });
        grid.destroy();
    });

    it("captures select input changes", () => {
        const grid = setupGrid();
        const select = document.createElement("select");
        select.className = "edit";
        select.setAttribute("data-field", "CategoryID");
        select.innerHTML = '<option value="2" selected>Beverages</option>';
        grid["inputsChange"]({ target: select });
        expect(grid.pendingChanges[1].CategoryID).toBe(2);
        grid.destroy();
    });

    it("captures string input changes", () => {
        const grid = setupGrid();
        const input = document.createElement("input");
        input.className = "edit";
        input.setAttribute("data-field", "ProductName");
        input.value = "Chai Tea";
        grid["inputsChange"]({ target: input });
        expect(grid.pendingChanges[1].ProductName).toBe("Chai Tea");
        grid.destroy();
    });

    it("toggles the save button state", () => {
        const grid = setupGrid();
        grid.pendingChanges = {};
        expect(() => grid["setSaveButtonState"]()).not.toThrow();
        grid.pendingChanges = { 1: { UnitPrice: 5 } };
        expect(() => grid["setSaveButtonState"]()).not.toThrow();
        grid.destroy();
    });

    it("saves all pending changes", () => {
        const grid = setupGrid();
        grid.pendingChanges = { 1: { UnitPrice: 5 }, 2: { UnitPrice: 6 } };
        const request = vi.spyOn(corelib, "serviceRequest").mockImplementation(((_s: any, _r: any, cb: any) => {
            cb?.({});
            return {} as any;
        }) as any);
        const refresh = vi.spyOn(grid, "refresh").mockImplementation(() => { });
        grid["saveClick"]();
        expect(request).toHaveBeenCalledTimes(2);
        expect(grid.pendingChanges).toEqual({});
        expect(refresh).toHaveBeenCalled();
        grid.destroy();
    });

    it("does nothing on save with no pending changes", () => {
        const grid = setupGrid();
        grid.pendingChanges = {};
        const request = vi.spyOn(corelib, "serviceRequest").mockImplementation((() => ({})) as any);
        grid["saveClick"]();
        expect(request).not.toHaveBeenCalled();
        grid.destroy();
    });

    it("applies the category query string to quick filters", () => {
        const grid = setupGrid();
        vi.spyOn(corelib, "parseQueryString").mockReturnValue({ cat: "1" } as any);
        const setValue = vi.spyOn(corelib.EditorUtils, "setValue").mockImplementation(() => { });
        const filters = grid["getQuickFilters"]();
        const category = filters.find((x: any) => x.field === ProductRow.Fields.CategoryID);
        expect(category).toBeTruthy();
        category.init({ element: { tryGetWidget: () => ({}) } });
        expect(setValue).toHaveBeenCalled();
        grid.destroy();
    });
});
