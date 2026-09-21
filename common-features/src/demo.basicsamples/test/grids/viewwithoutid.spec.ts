import * as corelib from "@serenity-is/corelib";
import { SalesByCategoryColumns, SalesByCategoryRow, SalesByCategoryService } from "@serenity-is/demo.northwind";
import { mockDynamicData, mockFetch, mockGridSize, unmockFetch } from "test-utils";
import { afterEach, beforeAll, beforeEach, describe, expect, it, vi } from "vitest";
import initPage, { ViewWithoutIDGrid } from "../../Modules/Grids/ViewWithoutID/ViewWithoutIDPage";

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

describe("ViewWithoutIDPage", () => {
    it("initializes the grid page", () => {
        const spy = vi.spyOn(corelib, "gridPageInit").mockReturnValue({} as any);
        initPage();
        expect(spy).toHaveBeenCalledWith(ViewWithoutIDGrid);
    });
});

describe("ViewWithoutIDGrid", () => {
    it("wires up columns, id/name/localText and service", () => {
        const grid = new ViewWithoutIDGrid({});
        expect(grid["getColumnsKey"]()).toBe(SalesByCategoryColumns.columnsKey);
        expect(grid["getIdProperty"]()).toBe("__id");
        expect(grid["getNameProperty"]()).toBe(SalesByCategoryRow.nameProperty);
        expect(grid["getLocalTextPrefix"]()).toBe(SalesByCategoryRow.localTextPrefix);
        expect(grid["getService"]()).toBe(SalesByCategoryService.baseUrl);
        expect(grid["getButtons"]()).toEqual([]);
        grid.destroy();
    });

    it("assigns auto incrementing __id to entities", () => {
        const grid = new ViewWithoutIDGrid({});
        const response = { Entities: [{ CategoryName: "A" }, { CategoryName: "B" }] } as any;
        const processed = grid["onViewProcessData"](response);
        expect((processed.Entities[0] as any).__id).toBe(1);
        expect((processed.Entities[1] as any).__id).toBe(2);
        const second = grid["onViewProcessData"]({ Entities: [{}] } as any);
        expect((second.Entities[0] as any).__id).toBe(3);
        grid.destroy();
    });
});
