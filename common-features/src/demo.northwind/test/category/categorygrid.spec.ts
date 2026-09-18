import { mockDynamicData, mockFetch, mockGridSize, unmockFetch } from "test-utils";
import { CategoryDialog } from "../../Modules/Category/CategoryDialog";
import { CategoryGrid } from "../../Modules/Category/CategoryGrid";
import { CategoryColumns, CategoryRow, CategoryService } from "../../Modules/ServerTypes/Demo";

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

describe("CategoryGrid", () => {
    it("wires up columns, dialog, row and service", () => {
        const grid = new CategoryGrid({});
        expect(grid["getColumnsKey"]()).toBe(CategoryColumns.columnsKey);
        expect(grid["getDialogType"]()).toBe(CategoryDialog);
        expect(grid["getRowDefinition"]()).toBe(CategoryRow);
        expect(grid["getService"]()).toBe(CategoryService.baseUrl);
        grid.destroy();
    });
});
