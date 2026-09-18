import * as corelib from "@serenity-is/corelib";
import { ProductColumns, ProductDialog, ProductRow, ProductService } from "@serenity-is/demo.northwind";
import { mockDynamicData, mockFetch, mockGridSize, unmockFetch } from "test-utils";
import initPage, { GridFilteredByCriteria } from "../../Modules/Grids/GridFilteredByCriteria/GridFilteredByCriteriaPage";

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

describe("GridFilteredByCriteriaPage", () => {
    it("initializes the grid page", () => {
        const spy = vi.spyOn(corelib, "gridPageInit").mockReturnValue({} as any);
        initPage();
        expect(spy).toHaveBeenCalledWith(GridFilteredByCriteria);
    });
});

describe("GridFilteredByCriteria", () => {
    it("wires up columns, dialog, row and service", () => {
        const grid = new GridFilteredByCriteria({});
        expect(grid["getColumnsKey"]()).toBe(ProductColumns.columnsKey);
        expect(grid["getDialogType"]()).toBe(ProductDialog);
        expect(grid["getRowDefinition"]()).toBe(ProductRow);
        expect(grid["getService"]()).toBe(ProductService.baseUrl);
        grid.destroy();
    });

    it("adds a criteria to the list request", () => {
        const grid = new GridFilteredByCriteria({});
        grid["view"].params = {} as any;
        grid["setViewParams"]();
        expect(grid["view"].params.Criteria).toBeTruthy();
        grid.destroy();
    });
});
