import { mockDynamicData, mockFetch, mockGridSize, unmockFetch } from "test-utils";
import { afterEach, beforeAll, beforeEach, describe, expect, it, vi } from "vitest";
import { ShipperColumns, ShipperRow, ShipperService } from "../../Modules/ServerTypes/Demo";
import { ShipperDialog } from "../../Modules/Shipper/ShipperDialog";
import { ShipperGrid } from "../../Modules/Shipper/ShipperGrid";

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

describe("ShipperGrid", () => {
    it("wires up columns, dialog, row and service", () => {
        const grid = new ShipperGrid({});
        expect(grid["getColumnsKey"]()).toBe(ShipperColumns.columnsKey);
        expect(grid["getDialogType"]()).toBe(ShipperDialog);
        expect(grid["getRowDefinition"]()).toBe(ShipperRow);
        expect(grid["getService"]()).toBe(ShipperService.baseUrl);
        grid.destroy();
    });
});
