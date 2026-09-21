import { mockDynamicData, mockFetch, mockGridSize, unmockFetch } from "test-utils";
import { afterEach, beforeAll, beforeEach, describe, expect, it, vi } from "vitest";
import { RegionDialog } from "../../Modules/Region/RegionDialog";
import { RegionGrid } from "../../Modules/Region/RegionGrid";
import { RegionColumns, RegionRow, RegionService } from "../../Modules/ServerTypes/Demo";

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

describe("RegionGrid", () => {
    it("wires up columns, dialog, row and service", () => {
        const grid = new RegionGrid({});
        expect(grid["getColumnsKey"]()).toBe(RegionColumns.columnsKey);
        expect(grid["getDialogType"]()).toBe(RegionDialog);
        expect(grid["getRowDefinition"]()).toBe(RegionRow);
        expect(grid["getService"]()).toBe(RegionService.baseUrl);
        grid.destroy();
    });
});
