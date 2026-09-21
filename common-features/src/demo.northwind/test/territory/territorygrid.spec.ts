import { mockDynamicData, mockFetch, mockGridSize, unmockFetch } from "test-utils";
import { afterEach, beforeAll, beforeEach, describe, expect, it, vi } from "vitest";
import { TerritoryDialog } from "../../Modules/Territory/TerritoryDialog";
import { TerritoryGrid } from "../../Modules/Territory/TerritoryGrid";
import { TerritoryColumns, TerritoryRow, TerritoryService } from "../../Modules/ServerTypes/Demo";

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

describe("TerritoryGrid", () => {
    it("wires up columns, dialog, row and service", () => {
        const grid = new TerritoryGrid({});
        expect(grid["getColumnsKey"]()).toBe(TerritoryColumns.columnsKey);
        expect(grid["getDialogType"]()).toBe(TerritoryDialog);
        expect(grid["getRowDefinition"]()).toBe(TerritoryRow);
        expect(grid["getService"]()).toBe(TerritoryService.baseUrl);
        grid.destroy();
    });
});
