import type { Column, ISleekGrid } from "@serenity-is/sleekgrid";
import { Fluent } from "../../base";
import type { FilterLine } from "../filtering/filterline";
import type { FilterStore } from "../filtering/filterstore";
import { tryGetWidget } from "../widgets/widgetutils";
import { getCurrentSettings, omitAllGridPersistenceFlags, restoreSettingsFrom } from "./datagrid-persistence";
import { QuickFilterBar } from "./quickfilterbar";

vi.mock("../../base", async (importOriginal) => {
    const actual = await importOriginal() as typeof import("../../base");
    const Fluent = Object.assign(vi.fn(), actual.Fluent, { trigger: vi.fn() });
    return {
        ...actual,
        Fluent,
        cssEscape: vi.fn((s) => s),
        FilterPanelTexts: { ...actual.FilterPanelTexts, And: "AND" }
    };
});

vi.mock("../widgets/widgetutils", () => ({
    tryGetWidget: vi.fn()
}));

vi.mock("./quickfilterbar", () => ({
    QuickFilterBar: {
        getItemData: vi.fn()
    }
}));

afterEach(() => {
    vi.clearAllMocks();
});

function mockSleekGrid(columns?: any[], sortColumns?: any[]): ISleekGrid {
    return {
        getAllColumns: vi.fn(() => columns || []),
        getSortColumns: vi.fn(() => sortColumns || []),
        invalidate: vi.fn(),
        invalidateColumns: vi.fn(),
        setVisibleColumns: vi.fn(),
        setSortColumns: vi.fn()
    } satisfies Partial<ISleekGrid> as unknown as ISleekGrid;
}

function mockFilterStore(items?: FilterLine[]): FilterStore {
    return {
        get_items: vi.fn(() => items || []),
        raiseChanged: vi.fn()
    } satisfies Partial<FilterStore> as unknown as FilterStore;
}

describe("getCurrentSettings", () => {
    it("omits all settings when all flags are false", () => {
        const grid = mockSleekGrid();
        const settings = getCurrentSettings({
            filterStore: null,
            flags: omitAllGridPersistenceFlags,
            includeDeletedToggle: document.createElement("div"),
            quickFiltersDiv: document.createElement("div"),
            sleekGrid: grid,
            toolbarNode: null,
            uniqueName: "TestGrid"
        });

        expect(settings).toEqual({ flags: omitAllGridPersistenceFlags });
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
            quickFiltersDiv: document.createElement("div"),
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
            quickFiltersDiv: document.createElement("div"),
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
            quickFiltersDiv: document.createElement("div"),
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
            quickFiltersDiv: document.createElement("div"),
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
            quickFiltersDiv: document.createElement("div"),
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
        });

        const grid = mockSleekGrid();
        const settings = getCurrentSettings({
            filterStore: null,
            flags: { ...omitAllGridPersistenceFlags, quickSearch: true },
            includeDeletedToggle: document.createElement("div"),
            quickFiltersDiv: document.createElement("div"),
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
            quickFiltersDiv: quickFiltersDiv,
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
            quickFiltersDiv: document.createElement("div"),
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
            quickFiltersDiv: document.createElement("div"),
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
            quickFiltersDiv: document.createElement("div"),
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
            quickFiltersDiv: document.createElement("div"),
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
            quickFiltersDiv: document.createElement("div"),
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

function restoreSettingsArgs(opt: Partial<Parameters<typeof restoreSettingsFrom>[0]>) {
    const defaultGrid = mockSleekGrid();
    return {
        filterStore: null,
        includeDeletedToggle: null,
        quickFiltersDiv: null,
        sleekGrid: defaultGrid,
        toolbarNode: null,
        uniqueName: "TestGrid",
        view: { sortBy: [] } as any,
        canShowColumn: vi.fn(() => true),
        ...opt
    } as Parameters<typeof restoreSettingsFrom>[0];
};

describe("restoreSettingsFrom", () => {
    it("does not fail when persisted flags are null and restore flags includes sort", () => {
        const columns = [
            { id: "col1" },
            { id: "col2" },
            { id: "col3" }
        ];
        const grid = mockSleekGrid(columns);
        restoreSettingsFrom(restoreSettingsArgs({
            sleekGrid: grid,
            flags: {
                ...omitAllGridPersistenceFlags,
                sortColumns: true
            },
            settings: {
                columns: [],
                flags: null
            }
        }));
    });

    it("restores column pinning when flag is true", () => {
        const columns: Column[] = [
            { id: "col1" },
            { id: "col2" },
            { id: "col3" }
        ];
        const grid = mockSleekGrid(columns);
        restoreSettingsFrom(restoreSettingsArgs({
            sleekGrid: grid,
            flags: { ...omitAllGridPersistenceFlags, columnPinning: true },
            settings: {
                columns: [
                    { id: "col1", pin: "start" },
                    { id: "col2", pin: false },
                    { id: "col3", pin: "end" }
                ]
            }
        }));

        expect(columns[0].frozen).toBe("start");
        expect(columns[1].frozen).toBe(null);
        expect(columns[2].frozen).toBe("end");
    });

    it("does not restore column pinning when flag is false", () => {
        const columns: Column[] = [
            { id: "col1" },
            { id: "col2" }
        ];
        const grid = mockSleekGrid(columns);
        restoreSettingsFrom(restoreSettingsArgs({
            sleekGrid: grid,
            flags: { ...omitAllGridPersistenceFlags, columnPinning: false },
            settings: {
                columns: [
                    { id: "col1", pin: "start" },
                    { id: "col2", pin: "end" }
                ]
            }
        }));

        expect(columns[0].frozen).toBeUndefined();
        expect(columns[1].frozen).toBeUndefined();
    });

    it("restores column widths when flag is true", () => {
        const columns: Column[] = [
            { id: "col1" },
            { id: "col2" }
        ];
        const grid = mockSleekGrid(columns);
        restoreSettingsFrom(restoreSettingsArgs({
            sleekGrid: grid,
            flags: { ...omitAllGridPersistenceFlags, columnWidths: true },
            settings: {
                columns: [
                    { id: "col1", width: 100 },
                    { id: "col2", width: 200 }
                ]
            }
        }));

        expect(columns[0].width).toBe(100);
        expect(columns[1].width).toBe(200);
    });

    it("does not restore column widths when flag is false", () => {
        const columns: Column[] = [
            { id: "col1" },
            { id: "col2" }
        ];
        const grid = mockSleekGrid(columns);
        restoreSettingsFrom(restoreSettingsArgs({
            sleekGrid: grid,
            flags: { ...omitAllGridPersistenceFlags, columnWidths: false },
            settings: {
                columns: [
                    { id: "col1", width: 100 },
                    { id: "col2", width: 200 }
                ]
            }
        }));

        expect(columns[0].width).toBeUndefined();
        expect(columns[1].width).toBeUndefined();
    });

    it("restores sort columns when flag is true", () => {
        const columns: Column[] = [
            { id: "col1" },
            { id: "col2" },
            { id: "col3" }
        ];
        const grid = mockSleekGrid(columns);
        const view = { sortBy: [] } as any;
        restoreSettingsFrom(restoreSettingsArgs({
            sleekGrid: grid,
            view,
            flags: { ...omitAllGridPersistenceFlags, sortColumns: true },
            settings: {
                columns: [
                    { id: "col1", sort: -2 },
                    { id: "col2", sort: 1 },
                    { id: "col3" }
                ]
            }
        }));

        expect(grid.setSortColumns).toHaveBeenCalledWith([
            { columnId: "col2", sortAsc: true },
            { columnId: "col1", sortAsc: false }
        ]);
        expect(view.sortBy).toEqual(["col2", "col1 DESC"]);
    });

    it("does not restore sort columns when flag is false", () => {
        const columns: Column[] = [
            { id: "col1" },
            { id: "col2" }
        ];
        const grid = mockSleekGrid(columns);
        const view = { sortBy: [] } as any;
        restoreSettingsFrom(restoreSettingsArgs({
            sleekGrid: grid,
            view,
            flags: { ...omitAllGridPersistenceFlags, sortColumns: false },
            settings: {
                columns: [
                    { id: "col1", sort: 1 },
                    { id: "col2", sort: -1 }
                ]
            }
        }));

        expect(grid.setSortColumns).not.toHaveBeenCalled();
        expect(view.sortBy).toEqual([]);
    });

    it("restores column visibility when flag is true", () => {
        const columns: Column[] = [
            { id: "col1" },
            { id: "col2" },
            { id: "col3" }
        ];
        const grid = mockSleekGrid(columns);
        restoreSettingsFrom(restoreSettingsArgs({
            sleekGrid: grid,
            flags: { ...omitAllGridPersistenceFlags, columnVisibility: true },
            settings: {
                columns: [
                    { id: "col1", visible: true },
                    { id: "col2", visible: false },
                    { id: "col3", visible: true }
                ]
            }
        }));

        expect(grid.setVisibleColumns).toHaveBeenCalledWith(["col1", "col3"], { notify: false });
    });

    it("does not restore column visibility when flag is false", () => {
        const columns = [
            { id: "col1" },
            { id: "col2" }
        ];
        const grid = mockSleekGrid(columns);
        restoreSettingsFrom(restoreSettingsArgs({
            sleekGrid: grid,
            flags: { ...omitAllGridPersistenceFlags, columnVisibility: false },
            settings: {
                columns: [
                    { id: "col1", visible: true },
                    { id: "col2", visible: false }
                ]
            }
        }));

        expect(grid.setVisibleColumns).not.toHaveBeenCalled();
        expect(grid.invalidateColumns).toHaveBeenCalled();
        expect(grid.invalidate).toHaveBeenCalled();
    });

    it("restores filter items when flag is true", () => {
        const filterItems: FilterLine[] = [
            { field: "field1", operator: "eq", criteria: ["value1"] }
        ];
        const filterStore = mockFilterStore([]);
        restoreSettingsFrom(restoreSettingsArgs({
            filterStore,
            flags: { ...omitAllGridPersistenceFlags, filterItems: true },
            settings: {
                filterItems
            }
        }));

        expect(filterStore.get_items()).toEqual(filterItems);
        expect(filterStore.raiseChanged).toHaveBeenCalled();
    });

    it("does not restore filter items when flag is false", () => {
        const filterItems: FilterLine[] = [
            { field: "field1", operator: "eq", criteria: ["value1"] }
        ];
        const filterStore = mockFilterStore([]);
        restoreSettingsFrom(restoreSettingsArgs({
            filterStore,
            flags: { ...omitAllGridPersistenceFlags, filterItems: false },
            settings: {
                filterItems
            }
        }));

        expect(filterStore.get_items()).toEqual([]);
        expect(filterStore.raiseChanged).not.toHaveBeenCalled();
    });

    it("restores include deleted when flag is true", () => {
        const includeDeletedToggle = document.createElement("div");
        const link = document.createElement("a");
        includeDeletedToggle.appendChild(link);

        restoreSettingsFrom(restoreSettingsArgs({
            includeDeletedToggle,
            flags: { ...omitAllGridPersistenceFlags, includeDeleted: true },
            settings: {
                includeDeleted: true
            }
        }));

        expect(Fluent.trigger).toHaveBeenCalledWith(link, "click");
    });

    it("does not restore include deleted when flag is false", () => {
        vi.clearAllMocks();
        const includeDeletedToggle = document.createElement("div");
        const link = document.createElement("a");
        includeDeletedToggle.appendChild(link);

        restoreSettingsFrom(restoreSettingsArgs({
            includeDeletedToggle,
            flags: { ...omitAllGridPersistenceFlags, includeDeleted: false },
            settings: {
                includeDeleted: true
            }
        }));

        expect(Fluent.trigger).not.toHaveBeenCalled();
    });

    it("restores quick filters when flag is true", () => {
        const quickFiltersDiv = document.createElement("div");
        const filterItem = document.createElement("div");
        filterItem.className = "quick-filter-item";
        filterItem.setAttribute("data-qffield", "field1");
        quickFiltersDiv.appendChild(filterItem);

        const widget = {};
        vi.mocked(tryGetWidget).mockReturnValue(widget as any);
        vi.mocked(QuickFilterBar.getItemData).mockReturnValue({
            loadState: vi.fn()
        });

        restoreSettingsFrom(restoreSettingsArgs({
            quickFiltersDiv,
            flags: { ...omitAllGridPersistenceFlags, quickFilters: true },
            settings: {
                quickFilters: { field1: "value1" }
            }
        }));

        expect(tryGetWidget).toHaveBeenCalledWith('#TestGrid_QuickFilter_field1');
        expect(QuickFilterBar.getItemData).toHaveBeenCalledWith(filterItem);
    });

    it("does not restore quick filters when flag is false", () => {
        const quickFiltersDiv = document.createElement("div");
        const filterItem = document.createElement("div");
        filterItem.className = "quick-filter-item";
        filterItem.setAttribute("data-qffield", "field1");
        quickFiltersDiv.appendChild(filterItem);

        restoreSettingsFrom(restoreSettingsArgs({
            quickFiltersDiv: quickFiltersDiv,
            flags: { ...omitAllGridPersistenceFlags, quickFilters: false },
            settings: {
                quickFilters: { field1: "value1" }
            }
        }));

        expect(QuickFilterBar.getItemData).not.toHaveBeenCalled();
    });

    it("restores quick search when flag is true", () => {
        const toolbarNode = document.createElement("div");
        const qsInput = document.createElement("input");
        qsInput.className = "s-QuickSearchInput";
        toolbarNode.appendChild(qsInput);

        const qsWidget = {
            restoreState: vi.fn()
        };
        vi.mocked(tryGetWidget).mockReturnValue(qsWidget as any);

        restoreSettingsFrom(restoreSettingsArgs({
            toolbarNode,
            flags: { ...omitAllGridPersistenceFlags, quickSearch: true },
            settings: {
                quickSearchText: "search text",
                quickSearchField: { name: "field1", title: "Field 1" }
            }
        }));

        expect(qsWidget.restoreState).toHaveBeenCalledWith("search text", { name: "field1", title: "Field 1" });
    });

    it("does not restore quick search when flag is false", () => {
        const toolbarNode = document.createElement("div");
        const qsInput = document.createElement("input");
        qsInput.className = "s-QuickSearchInput";
        toolbarNode.appendChild(qsInput);

        const qsWidget = {
            restoreState: vi.fn()
        };
        vi.mocked(tryGetWidget).mockReturnValue(qsWidget as any);

        restoreSettingsFrom(restoreSettingsArgs({
            toolbarNode,
            flags: { ...omitAllGridPersistenceFlags, quickSearch: false },
            settings: {
                quickSearchText: "search text",
                quickSearchField: { name: "field1", title: "Field 1" }
            }
        }));

        expect(qsWidget.restoreState).not.toHaveBeenCalled();
    });
});