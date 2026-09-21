import * as corelib from "@serenity-is/corelib";
import { SupplierColumns, SupplierDialog, SupplierRow, SupplierService } from "@serenity-is/demo.northwind";
import { mockDynamicData, mockFetch, mockGridSize, unmockFetch } from "test-utils";
import { afterEach, beforeAll, beforeEach, describe, expect, it, vi } from "vitest";
import initPage, { RowSelectionGrid } from "../../Modules/Grids/EnablingRowSelection/EnablingRowSelectionPage";

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

describe("EnablingRowSelectionPage", () => {
    it("initializes the grid page", () => {
        const spy = vi.spyOn(corelib, "gridPageInit").mockReturnValue({} as any);
        initPage();
        expect(spy).toHaveBeenCalledWith(RowSelectionGrid);
    });
});

describe("RowSelectionGrid", () => {
    it("wires up columns, dialog, row and service", () => {
        const grid = new RowSelectionGrid({});
        expect(grid["getColumnsKey"]()).toBe(SupplierColumns.columnsKey);
        expect(grid["getDialogType"]()).toBe(SupplierDialog);
        expect(grid["getRowDefinition"]()).toBe(SupplierRow);
        expect(grid["getService"]()).toBe(SupplierService.baseUrl);
        grid.destroy();
    });
});
