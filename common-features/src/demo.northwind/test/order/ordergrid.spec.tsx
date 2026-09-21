import { ReportHelper } from "@serenity-is/extensions";
import { formatterContext } from "@serenity-is/sleekgrid";
import { mockDynamicData, mockFetch, mockGridSize, unmockFetch } from "test-utils";
import { afterAll, afterEach, beforeAll, describe, expect, it, vi } from "vitest";
import { OrderDialog } from "../../Modules/Order/OrderDialog";
import { OrderGrid } from "../../Modules/Order/OrderGrid";
import { OrderColumns, OrderRow, OrderService } from "../../Modules/ServerTypes/Demo";

beforeAll(() => {
    mockDynamicData();
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

describe("OrderGrid", () => {
    it("wires up columns, dialog, row and service", () => {
        const grid = new OrderGrid({});
        expect(grid["getColumnsKey"]()).toBe(OrderColumns.columnsKey);
        expect(grid["getDialogType"]()).toBe(OrderDialog);
        expect(grid["getRowDefinition"]()).toBe(OrderRow);
        expect(grid["getService"]()).toBe(OrderService.baseUrl);
        grid.destroy();
    });

    it("adds export and save buttons", () => {
        const grid = new OrderGrid({});
        const classes = grid["getButtons"]().map(x => x.cssClass);
        expect(classes).toContain("export-xlsx-button");
        expect(classes).toContain("export-pdf-button");
        grid.destroy();
    });

    it("formats the print invoice column", () => {
        const grid = new OrderGrid({});
        const columns = grid["createColumns"]();
        const invoice = columns.find(x => x.id === OrderColumns.Fields.PrintInvoice);
        expect(invoice).toBeTruthy();
        expect(typeof invoice.format).toBe("function");
        expect(invoice.format(formatterContext({ value: null }))).toBeTruthy();
        grid.destroy();
    });

    it("adds a product quick filter and applies its handler", () => {
        const grid = new OrderGrid({});
        const filters = grid["getQuickFilters"]();
        const productFilter = filters.find(x => x.field === "ProductID");
        expect(productFilter).toBeTruthy();
        productFilter.handler({ value: 7 } as any);
        expect((grid.view.params as any).ProductID).toBe(7);
        grid.destroy();
    });

    it("creates the shipping state quick filter", () => {
        const grid = new OrderGrid({});
        expect(() => grid["createQuickFilters"]()).not.toThrow();
        grid.destroy();
    });

    it("sets the shipping state filter value", () => {
        const grid = new OrderGrid({});
        (grid as any).shippingStateFilter = { value: null };
        grid.set_shippingState(1);
        expect((grid as any).shippingStateFilter.value).toBe("1");
        grid.set_shippingState(null);
        expect((grid as any).shippingStateFilter.value).toBe("");
        grid.destroy();
    });

    it("executes the print invoice report on inline action click", () => {
        const execute = vi.spyOn(ReportHelper, "execute").mockImplementation(() => { });
        const grid = new OrderGrid({});
        vi.spyOn(Object.getPrototypeOf(OrderGrid.prototype), "onClick").mockImplementation(() => { });
        (grid as any).itemAt = () => ({ OrderID: 5 });
        const anchor = document.createElement("a");
        anchor.className = "inline-action";
        anchor.setAttribute("data-action", "print-invoice");
        const e = new MouseEvent("click", { bubbles: true, cancelable: true });
        anchor.dispatchEvent(e);
        grid["onClick"](e, 0, 0);
        expect(execute).toHaveBeenCalledWith(expect.objectContaining({ reportKey: "Northwind.OrderDetail" }));
        grid.destroy();
    });

    it("ignores inline action clicks when default is prevented", () => {
        const grid = new OrderGrid({});
        vi.spyOn(Object.getPrototypeOf(OrderGrid.prototype), "onClick").mockImplementation(() => { });
        const execute = vi.spyOn(ReportHelper, "execute").mockImplementation(() => { });
        const anchor = document.createElement("a");
        anchor.className = "inline-action";
        anchor.setAttribute("data-action", "print-invoice");
        const e = new MouseEvent("click", { bubbles: true, cancelable: true });
        anchor.dispatchEvent(e);
        e.preventDefault();
        grid["onClick"](e, 0, 0);
        expect(execute).not.toHaveBeenCalled();
        grid.destroy();
    });

    it("uses the customer equality filter when adding", () => {
        const grid = new OrderGrid({});
        (grid.view.params as any).EqualityFilter = { CustomerID: "A" };
        const edit = vi.spyOn(grid as any, "editItem").mockImplementation(() => { });
        expect(() => grid["addButtonClick"]()).not.toThrow();
        expect(edit).toHaveBeenCalledWith({ CustomerID: "A" });
        grid.destroy();
    });

    it("adds an order without a customer equality filter", () => {
        const grid = new OrderGrid({});
        const edit = vi.spyOn(grid as any, "editItem").mockImplementation(() => { });
        grid["addButtonClick"]();
        expect(edit).toHaveBeenCalledWith({ CustomerID: null });
        grid.destroy();
    });
});
