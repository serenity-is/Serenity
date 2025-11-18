import type { ISleekGrid } from "@serenity-is/sleekgrid";
import { Fluent } from "../../base";
import type { FilterStore } from "../filtering/filterstore";
import type { FilterLine } from "../filtering/filterline";
import { tryGetWidget } from "../widgets/widgetutils";
import { QuickFilterBar } from "./quickfilterbar";
import { getCurrentSettings, omitAllGridPersistenceFlags, defaultGridPersistenceFlags } from "./datagrid-persistence";

vi.mock("../widgets/widgetutils", () => ({
    tryGetWidget: vi.fn()
}));

vi.mock("./quickfilterbar", () => ({
    QuickFilterBar: {
        getItemData: vi.fn()
    }
}));

function mockSleekGrid(columns?: any[], sortColumns?: any[]): ISleekGrid {
    return {
        getAllColumns: vi.fn(() => columns || []),
        getSortColumns: vi.fn(() => sortColumns || []),
        invalidate: vi.fn(),
        invalidateColumns: vi.fn(),
        setVisibleColumns: vi.fn()
    } satisfies Partial<ISleekGrid> as unknown as ISleekGrid;
}

function mockFilterStore(items?: FilterLine[]): FilterStore {
    return {
        get_items: vi.fn(() => items || [])
    } as any;
}

describe("getCurrentSettings", () => {
    it("omits all settings when all flags are false", () => {
        const grid = mockSleekGrid();
        const settings = getCurrentSettings({
            filterStore: null,
            flags: omitAllGridPersistenceFlags,
            includeDeletedToggle: document.createElement("div"),
            quickFiltersDiv: Fluent(document.createElement("div")),
            sleekGrid: grid,
            toolbarNode: null,
            uniqueName: "TestGrid"
        });

        expect(settings).toEqual({ flags: omitAllGridPersistenceFlags});
    });

    it("captures column visibility when flag is true", () => {
        const columns = [
            { id: "col1", visible: true },
            { id: "col2", visible: false },
            { id: "col3", visible: true }
        ];
        const grid = mockSleekGrid(columns);
        const flags = { ...omitAllGridPersistenceFlags, columnVisibility: true };
        const settings = getCurrentSettings({
            filterStore: null,
            flags: flags,
            includeDeletedToggle: document.createElement("div"),
            quickFiltersDiv: Fluent(document.createElement("div")),
            sleekGrid: grid,
            toolbarNode: null,
            uniqueName: "TestGrid"
        });

        expect(settings.columns).toEqual([
            { id: "col1", visible: true },
            { id: "col2", visible: false },
            { id: "col3", visible: true }
        ]);
    });

    it("captures column widths when flag is true", () => {
        const columns = [
            { id: "col1", width: 100 },
            { id: "col2", width: 200 },
            { id: "col3", width: 150 }
        ];
        const grid = mockSleekGrid(columns);
        const settings = getCurrentSettings({
            filterStore: null,
            flags: { ...omitAllGridPersistenceFlags, columnWidths: true },
            includeDeletedToggle: document.createElement("div"),
            quickFiltersDiv: Fluent(document.createElement("div")),
            sleekGrid: grid,
            toolbarNode: null,
            uniqueName: "TestGrid"
        });

        expect(settings.columns).toEqual([
            { id: "col1", width: 100 },
            { id: "col2", width: 200 },
            { id: "col3", width: 150 }
        ]);
    });

    it("captures column pinning when flag is true", () => {
        const columns = [
            { id: "col1", frozen: false },
            { id: "col2", frozen: true },
            { id: "col3", frozen: "end" }
        ];
        const grid = mockSleekGrid(columns);
        const settings = getCurrentSettings({
            filterStore: null,
            flags: { ...omitAllGridPersistenceFlags, columnPinning: true },
            includeDeletedToggle: document.createElement("div"),
            quickFiltersDiv: Fluent(document.createElement("div")),
            sleekGrid: grid,
            toolbarNode: null,
            uniqueName: "TestGrid"
        });

        expect(settings.columns).toEqual([
            { id: "col1", pin: false },
            { id: "col2", pin: "start" },
            { id: "col3", pin: "end" }
        ]);
    });

    it("captures sort columns when flag is true", () => {
        const columns = [
            { id: "col1" },
            { id: "col2" },
            { id: "col3" }
        ];
        const sortColumns = [
            { columnId: "col2", sortAsc: true },
            { columnId: "col1", sortAsc: false }
        ];
        const grid = mockSleekGrid(columns, sortColumns);
        const settings = getCurrentSettings({
            filterStore: null,
            flags: { ...omitAllGridPersistenceFlags, sortColumns: true },
            includeDeletedToggle: document.createElement("div"),
            quickFiltersDiv: Fluent(document.createElement("div")),
            sleekGrid: grid,
            toolbarNode: null,
            uniqueName: "TestGrid"
        });

        expect(settings.columns).toEqual([
            { id: "col1", sort: -2 },
            { id: "col2", sort: 1 },
            { id: "col3" }
        ]);
    });

    it("captures filter items when flag is true", () => {
        const filterItems: FilterLine[] = [
            { field: "field1", operator: "eq", criteria: ["value1"] },
            { field: "field2", operator: "contains", criteria: ["value2"] }
        ];
        const filterStore = mockFilterStore(filterItems);
        const grid = mockSleekGrid();
        const settings = getCurrentSettings({
            filterStore,
            flags: { ...omitAllGridPersistenceFlags, filterItems: true },
            includeDeletedToggle: document.createElement("div"),
            quickFiltersDiv: Fluent(document.createElement("div")),
            sleekGrid: grid,
            toolbarNode: null,
            uniqueName: "TestGrid"
        });

        expect(settings.filterItems).toEqual(filterItems);
    });

    it("captures quick search when flag is true", () => {
        const toolbarNode = document.createElement("div");
        const qsInput = document.createElement("input");
        qsInput.className = "s-QuickSearchInput";
        qsInput.value = "search text";
        toolbarNode.appendChild(qsInput);

        // Mock tryGetWidget to return a QuickSearchInput-like object
        vi.mocked(tryGetWidget).mockReturnValue({
            get_field: vi.fn(() => ({ name: "field1", title: "Field 1" })),
            domNode: qsInput
        } as any);

        const grid = mockSleekGrid();
        const settings = getCurrentSettings({
            filterStore: null,
            flags: { ...omitAllGridPersistenceFlags, quickSearch: true },
            includeDeletedToggle: document.createElement("div"),
            quickFiltersDiv: Fluent(document.createElement("div")),
            sleekGrid: grid,
            toolbarNode,
            uniqueName: "TestGrid"
        });

        expect(settings.quickSearchField).toEqual({ name: "field1", title: "Field 1" });
        expect(settings.quickSearchText).toBe("search text");
    });

    it("captures quick filters when flag is true", () => {
        const quickFiltersDiv = document.createElement("div");
        const filterItem = document.createElement("div");
        filterItem.className = "quick-filter-item";
        filterItem.setAttribute("data-qffield", "field1");
        const label = document.createElement("div");
        label.className = "quick-filter-label";
        label.textContent = "Field 1";
        filterItem.appendChild(label);
        quickFiltersDiv.appendChild(filterItem);

        // Mock tryGetWidget to return a widget
        vi.mocked(tryGetWidget).mockReturnValue({} as any);

        // Mock QuickFilterBar.getItemData to return saveState function
        vi.mocked(QuickFilterBar.getItemData).mockReturnValue({
            saveState: vi.fn(() => "filter value")
        });

        const grid = mockSleekGrid();
        const settings = getCurrentSettings({
            filterStore: null,
            flags: { ...omitAllGridPersistenceFlags, quickFilters: true },
            includeDeletedToggle: document.createElement("div"),
            quickFiltersDiv: Fluent(quickFiltersDiv),
            sleekGrid: grid,
            toolbarNode: null,
            uniqueName: "TestGrid"
        });

        expect(settings.quickFilters).toEqual({ field1: "filter value" });
    });

    it("captures include deleted when flag is true", () => {
        const includeDeletedToggle = document.createElement("div");
        includeDeletedToggle.className = "pressed";

        const grid = mockSleekGrid();
        const settings = getCurrentSettings({
            filterStore: null,
            flags: { ...omitAllGridPersistenceFlags, includeDeleted: true },
            includeDeletedToggle,
            quickFiltersDiv: Fluent(document.createElement("div")),
            sleekGrid: grid,
            toolbarNode: null,
            uniqueName: "TestGrid"
        });

        expect(settings.includeDeleted).toBe(true);
    });

    it("uses default flags when no flags are provided", () => {
        const columns = [
            { id: "col1", visible: true, width: 100 },
            { id: "col2", visible: false, width: 200 }
        ];
        const grid = mockSleekGrid(columns);
        const settings = getCurrentSettings({
            filterStore: null,
            flags: {}, // No flags provided
            includeDeletedToggle: document.createElement("div"),
            quickFiltersDiv: Fluent(document.createElement("div")),
            sleekGrid: grid,
            toolbarNode: null,
            uniqueName: "TestGrid"
        });

        // Should use defaults: persist columns, filters, etc. but not quickSearch/quickFilterText
        expect(settings.columns).toBeDefined();
        expect(settings.columns!.length).toBe(2);
        expect(settings.columns![0]).toEqual({ id: "col1", visible: true, width: 100, pin: false });
        expect(settings.columns![1]).toEqual({ id: "col2", visible: false, width: 200, pin: false });
        expect(settings.quickSearchField).toBeUndefined();
        expect(settings.quickSearchText).toBeUndefined();
    });

    it("respects default flags for quick search (should not persist by default)", () => {
        const toolbarNode = document.createElement("div");
        const qsInput = document.createElement("input");
        qsInput.className = "s-QuickSearchInput";
        qsInput.value = "search text";
        toolbarNode.appendChild(qsInput);

        vi.mocked(tryGetWidget).mockReturnValue({
            get_field: vi.fn(() => ({ name: "field1", title: "Field 1" })),
            domNode: qsInput
        } as any);

        const grid = mockSleekGrid();
        const settings = getCurrentSettings({
            filterStore: null,
            flags: {}, // No flags provided - should use defaults
            includeDeletedToggle: document.createElement("div"),
            quickFiltersDiv: Fluent(document.createElement("div")),
            sleekGrid: grid,
            toolbarNode,
            uniqueName: "TestGrid"
        });

        // quickSearch defaults to false, so should not be persisted
        expect(settings.quickSearchField).toBeUndefined();
        expect(settings.quickSearchText).toBeUndefined();
    });

    it("allows overriding defaults with explicit flags", () => {
        const columns = [
            { id: "col1", visible: true, width: 100 }
        ];
        const grid = mockSleekGrid(columns);
        const settings = getCurrentSettings({
            filterStore: null,
            flags: { columnWidths: false, quickSearch: true }, // Override defaults
            includeDeletedToggle: document.createElement("div"),
            quickFiltersDiv: Fluent(document.createElement("div")),
            sleekGrid: grid,
            toolbarNode: null,
            uniqueName: "TestGrid"
        });

        // columnWidths overridden to false, so width should not be persisted
        // columnPinning still defaults to true, so dummy pin is added
        expect(settings.columns![0]).toEqual({ id: "col1", visible: true, pin: false });
        expect(settings.columns![0].width).toBeUndefined();

        // quickSearch overridden to true, but no toolbar so no search data
        expect(settings.quickSearchField).toBeUndefined();
    });

    it("merges partial flags with defaults correctly", () => {
        const columns = [
            { id: "col1", visible: true, width: 100 }
        ];
        const grid = mockSleekGrid(columns);
        const settings = getCurrentSettings({
            filterStore: null,
            flags: { columnVisibility: false }, // Only override visibility
            includeDeletedToggle: document.createElement("div"),
            quickFiltersDiv: Fluent(document.createElement("div")),
            sleekGrid: grid,
            toolbarNode: null,
            uniqueName: "TestGrid"
        });

        // columnVisibility overridden to false, so visible should not be persisted
        // columnWidths should still use default (true), so width should be persisted
        // columnPinning defaults to true, so dummy pin is added for only first column
        expect(settings.columns![0]).toEqual({ id: "col1", width: 100, pin: false });
        expect(settings.columns![0].visible).toBeUndefined();
    });
});