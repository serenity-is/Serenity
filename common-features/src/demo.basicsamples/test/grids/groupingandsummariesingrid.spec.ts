import * as corelib from "@serenity-is/corelib";
import { ProductColumns, ProductDialog, ProductRow, ProductService } from "@serenity-is/demo.northwind";
import { mockDynamicData, mockFetch, mockGridSize, unmockFetch } from "test-utils";
import { afterEach, beforeAll, beforeEach, describe, expect, it, vi } from "vitest";
import initPage, { GroupingAndSummariesInGrid } from "../../Modules/Grids/GroupingAndSummariesInGrid/GroupingAndSummariesInGridPage";

beforeAll(() => {
    mockDynamicData();
    mockGridSize();
});

beforeEach(() => {
    mockFetch({ "*": () => ({ Entities: [], TotalCount: 0 }) });
});

afterEach(() => {
    unmockFetch();
    vi.restoreAllMocks();
    document.body.innerHTML = "";
});

describe("GroupingAndSummariesInGridPage", () => {
    it("initializes the grid page", () => {
        const spy = vi.spyOn(corelib, "gridPageInit").mockReturnValue({} as any);
        initPage();
        expect(spy).toHaveBeenCalledWith(GroupingAndSummariesInGrid);
    });
});

describe("GroupingAndSummariesInGrid", () => {
    it("wires up columns, dialog, row and service", () => {
        const grid = new GroupingAndSummariesInGrid({});
        expect(grid["getColumnsKey"]()).toBe(ProductColumns.columnsKey);
        expect(grid["getDialogType"]()).toBe(ProductDialog);
        expect(grid["getRowDefinition"]()).toBe(ProductRow);
        expect(grid["getService"]()).toBe(ProductService.baseUrl);
        grid.destroy();
    });

    it("enables footer row and disables pager", () => {
        const grid = new GroupingAndSummariesInGrid({});
        expect(grid["getSlickOptions"]().showFooterRow).toBe(true);
        expect(grid["usePager"]()).toBe(false);
        grid.destroy();
    });

    it("groups by category, then category and supplier, then none", () => {
        const grid = new GroupingAndSummariesInGrid({});
        const setGrouping = vi.spyOn(grid["view"], "setGrouping").mockImplementation(() => ({} as any));
        const buttons = grid["getButtons"]();
        expect(buttons.length).toBe(3);

        buttons[0].onClick();
        expect(setGrouping).toHaveBeenLastCalledWith([expect.objectContaining({ getter: "CategoryName" })]);

        buttons[1].onClick();
        expect(setGrouping).toHaveBeenLastCalledWith([
            expect.objectContaining({ getter: "CategoryName" }),
            expect.objectContaining({ getter: "SupplierCompanyName" })
        ]);

        buttons[2].onClick();
        expect(setGrouping).toHaveBeenLastCalledWith([]);
        grid.destroy();
    });

    it("provides group total formatters using max and avg values", () => {
        const grid = new GroupingAndSummariesInGrid({});
        const columns = new ProductColumns(grid["createColumns"]());
        expect(columns.UnitsOnOrder.groupTotalsFormat({ escape: (x: any) => x, item: { max: { UnitsOnOrder: 12 } } } as any))
            .toBe("max: 12");
        expect(columns.UnitsOnOrder.groupTotalsFormat({ escape: (x: any) => x, item: {} } as any)).toBe("");
        expect(columns.ReorderLevel.groupTotalsFormat({ escape: (x: any) => x, item: { avg: { ReorderLevel: 3 } } } as any))
            .toContain("avg: 3");
        expect(columns.ReorderLevel.groupTotalsFormat({ escape: (x: any) => x, item: {} } as any)).toBe("");
        grid.destroy();
    });

    it("formats the grouping headers", () => {
        const grid = new GroupingAndSummariesInGrid({});
        let grouping: any;
        vi.spyOn(grid["view"], "setGrouping").mockImplementation((g: any) => { grouping = g; return {} as any; });
        const buttons = grid["getButtons"]();

        buttons[0].onClick();
        expect(grouping[0].format({ escape: (x: any) => x, item: { value: "Beverages", count: 3 } }))
            .toBe("Category: Beverages (3 items)");

        buttons[1].onClick();
        expect(grouping[1].format({ escape: (x: any) => x, item: { value: "Exotic", count: 2 } }))
            .toBe("Supplier: Exotic (2 items)");
        grid.destroy();
    });
});
