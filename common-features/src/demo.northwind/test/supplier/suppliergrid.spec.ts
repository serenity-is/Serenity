import { mockDynamicData, mockFetch, mockGridSize, unmockFetch } from "test-utils";
import { SupplierDialog } from "../../Modules/Supplier/SupplierDialog";
import { SupplierGrid } from "../../Modules/Supplier/SupplierGrid";
import { SupplierColumns, SupplierRow, SupplierService } from "../../Modules/ServerTypes/Demo";

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

describe("SupplierGrid", () => {
    it("wires up columns, dialog, row and service", () => {
        const grid = new SupplierGrid({});
        expect(grid["getColumnsKey"]()).toBe(SupplierColumns.columnsKey);
        expect(grid["getDialogType"]()).toBe(SupplierDialog);
        expect(grid["getRowDefinition"]()).toBe(SupplierRow);
        expect(grid["getService"]()).toBe(SupplierService.baseUrl);
        grid.destroy();
    });
});
