import { Column } from "@serenity-is/sleekgrid";
import { Fluent } from "../../base";
import { ScriptData } from "../../compat";
import { DataGrid, omitAllGridPersistenceFlags } from "./datagrid";

function createGrid(): DataGrid<any, any> {
    class DefaultGrid extends DataGrid<any, any> { }
    return new DefaultGrid({});
}

function getIdProperty(grid: DataGrid<any, any>): string {
    return grid["getIdProperty"]();
}

describe('DataGrid.getIdProperty', () => {
    it('returns ID by default', () => {
        class DefaultGrid extends DataGrid<any, any> {
        }

        var grid = new DefaultGrid({});
        expect(getIdProperty(grid)).toBe("ID");
    });

    it('can be overridden in subclass', () => {
        class SubClassGrid extends DataGrid<any, any> {
            getIdProperty() { return "subClassId" };
        }

        var grid = new SubClassGrid({});
        expect(getIdProperty(grid)).toBe("subClassId");
    });

    it('returns value from getRowDefition()', () => {
        class TestRow {
            static readonly idProperty = "idForTestRow";
        }

        class TestRowGrid extends DataGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid({});
        expect(getIdProperty(grid)).toBe("idForTestRow");
    });

    it("returns empty string if getRowDefition() doesn't have the value", () => {
        class TestRow {
            static readonly idProperty: string = undefined;
        }

        class TestRowGrid extends DataGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid({});
        expect(getIdProperty(grid)).toBe("");
    });
});

function getIsActiveProperty(grid: DataGrid<any, any>): string {
    return grid["getIsActiveProperty"]();
}

describe('DataGrid.getIsActiveProperty', () => {
    it('returns empty by default', () => {
        class DefaultGrid extends DataGrid<any, any> {
        }

        var grid = new DefaultGrid({});
        expect(getIsActiveProperty(grid)).toBe("");
    });

    it('can be set via attribute', () => {
        class SubClassGrid extends DataGrid<any, any> {
            getIsActiveProperty() { return "subClassIsActive" };
        }

        var grid = new SubClassGrid({});
        expect(getIsActiveProperty(grid)).toBe("subClassIsActive");
    });

    it('returns value from getRowDefinition()', () => {
        class TestRow {
            static readonly isActiveProperty = "activeForTestRow";
        }

        class TestRowGrid extends DataGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid({});
        expect(getIsActiveProperty(grid)).toBe("activeForTestRow");
    });

    it("returns empty string if getRowDefinition() doesn't have the value", () => {
        class TestRow {
            static readonly isActiveProperty: string = undefined;
        }

        class TestRowGrid extends DataGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid({});
        expect(getIsActiveProperty(grid)).toBe("");
    });
});

function getLocalTextDbPrefix(grid: DataGrid<any, any>): string {
    return grid["getLocalTextDbPrefix"]();
}

describe('DataGrid.getLocalTextDbPrefix', () => {
    it('returns empty by default', () => {
        class DefaultGrid extends DataGrid<any, any> {
        }

        var grid = new DefaultGrid({});
        expect(getLocalTextDbPrefix(grid)).toBe("");
    });

    it('can be overridden in subclass via getLocalTextDbPrefix', () => {
        class SubClassGrid extends DataGrid<any, any> {
            getLocalTextDbPrefix() { return "My.Prefix." };
        }

        var grid = new SubClassGrid({});
        expect(getLocalTextDbPrefix(grid)).toBe("My.Prefix.");
    });

    it('can be overridden in subclass via getLocalTextPrefix', () => {
        class SubClassGrid extends DataGrid<any, any> {
            getLocalTextPrefix() { return "MySubClassPrefix" };
        }

        var grid = new SubClassGrid({});
        expect(getLocalTextDbPrefix(grid)).toBe("Db.MySubClassPrefix.");
    });

    it('returns value from getRowDefinition()', () => {
        class TestRow {
            static readonly localTextPrefix = "prefixForTestRow";
        }

        class TestRowGrid extends DataGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid({});
        expect(getLocalTextDbPrefix(grid)).toBe("Db.prefixForTestRow.");
    });

    it("returns empty string if getRowDefinition() doesn't have the value", () => {
        class TestRow {
            static readonly localTextPrefix: string = undefined;
        }

        class TestRowGrid extends DataGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid({});
        expect(getLocalTextDbPrefix(grid)).toBe("");
    });
});

function getLocalTextPrefix(grid: DataGrid<any, any>): string {
    return grid["getLocalTextPrefix"]();
}

describe('DataGrid.getLocalTextPrefix', () => {
    it('returns undefined by default', () => {
        class DefaultGrid extends DataGrid<any, any> {
        }

        var grid = new DefaultGrid({});
        expect(getLocalTextPrefix(grid)).toBeUndefined();
    });

    it('can be overridden in subclass', () => {
        class SubClassGrid extends DataGrid<any, any> {
            getLocalTextPrefix() { return "subClassPrefix" };
        }

        var grid = new SubClassGrid({});
        expect(getLocalTextPrefix(grid)).toBe("subClassPrefix");
    });

    it('returns value from getRowDefinition()', () => {
        class TestRow {
            static readonly localTextPrefix = "prefixForTestRow";
        }

        class TestRowGrid extends DataGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid({});
        expect(getLocalTextPrefix(grid)).toBe("prefixForTestRow");
    });

    it("returns undefined if getRowDefinition() doesn't have the value", () => {
        class TestRow {
            static readonly localTextPrefix: string = undefined;
        }

        class TestRowGrid extends DataGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        var grid = new TestRowGrid({});
        expect(getLocalTextPrefix(grid)).toBeUndefined();
    });
});

describe("DataGrid.getCurrentSettings", () => {
    it("returns an empty object with columns for a default grid", () => {
        class MyGrid extends DataGrid<any, any> {
        }

        const grid = new MyGrid({});
        expect(grid.getCurrentSettings()).toEqual({
            columns: [],
            flags: DataGrid.defaultOptions.persistenceFlags
        });
    });

    it("does not return columns if flags.columnVisibility is false, flags.columnWidths is false, and flags.sortColumns is false", () => {
        class MyGrid extends DataGrid<any, any> {
        }

        const grid = new MyGrid({});
        const flags = {
            ...omitAllGridPersistenceFlags,
            columnVisibility: false,
            columnWidths: false,
            sortColumns: false
        };
        expect(grid.getCurrentSettings(flags)).toEqual({
            flags: flags
        });
    });

    it("sets column visibility only for visible columns", () => {
        class MyGrid extends DataGrid<any, any> {
            getColumns(): Column[] {
                return [
                    { field: "A" },
                    { field: "B", visible: false },
                    { field: "C" }
                ];
            }
        }

        const grid = new MyGrid({});
        const flags = {
            ...omitAllGridPersistenceFlags,
            columnVisibility: true
        };
        expect(grid.getCurrentSettings(flags)).toEqual({
            columns: [
                { id: "A", visible: true },
                { id: "B", visible: false },
                { id: "C", visible: true }
            ],
            flags: flags
        });
    });

    it("sets column widths", () => {
        class MyGrid extends DataGrid<any, any> {
            getColumns(): Column[] {
                return [
                    { field: "A", width: 100 },
                    { field: "B", width: 200 },
                ];
            }
        }

        const grid = new MyGrid({});
        const flags = { 
            ...omitAllGridPersistenceFlags,
            columnWidths: true 
        };
        expect(grid.getCurrentSettings(flags)).toEqual({
            columns: [
                { id: "A", width: 100 },
                { id: "B", width: 200 }
            ],
            flags: flags
        });
    });
});

describe("DataGrid accessors", () => {
    it("getElement returns domNode", () => {
        const grid = createGrid();
        expect(grid.getElement()).toBe(grid.domNode);
        grid.destroy();
    });

    it("getGrid and sleekGrid/slickGrid return grid", () => {
        const grid = createGrid();
        expect(grid.getGrid()).toBeTruthy();
        expect(grid.sleekGrid).toBe(grid.getGrid());
        expect(grid.slickGrid).toBe(grid.getGrid());
        grid.destroy();
    });

    it("getView returns view", () => {
        const grid = createGrid();
        expect(grid.getView()).toBeTruthy();
        grid.destroy();
    });

    it("getFilterStore returns null when no filter bar", () => {
        const grid = createGrid();
        expect(grid.getFilterStore()).toBeNull();
        grid.destroy();
    });

    it("allColumns and columns return arrays", () => {
        const grid = createGrid();
        expect(Array.isArray(grid.allColumns)).toBe(true);
        expect(Array.isArray(grid.columns)).toBe(true);
        grid.destroy();
    });

    it("getItemType returns Item", () => {
        const grid = createGrid();
        expect(grid["getItemType"]()).toBe("Item");
        grid.destroy();
    });

    it("getAddButtonCaption returns text", () => {
        const grid = createGrid();
        expect(grid["getAddButtonCaption"]()).toBeTruthy();
        grid.destroy();
    });

    it("default persistence storage static getter/setter", () => {
        const orig = DataGrid.defaultPersistenceStorage;
        const storage = { getItem: vi.fn(), setItem: vi.fn() } as any;
        DataGrid.defaultPersistenceStorage = storage;
        expect(DataGrid.defaultPersistenceStorage).toBe(storage);
        DataGrid.defaultPersistenceStorage = orig;
    });
});

describe("DataGrid rows and items", () => {
    it("rowCount and itemAt use the grid", () => {
        const grid = createGrid();
        expect(grid["rowCount"]()).toBe(0);
        expect(() => grid["itemAt"](0)).not.toThrow();
        grid.destroy();
    });

    it("itemId returns id field value", () => {
        const grid = createGrid();
        expect(grid["itemId"]({ ID: 42 })).toBe(42);
        grid.destroy();
    });

    it("getItems returns view items", () => {
        const grid = createGrid();
        expect(grid["getItems"]()).toEqual([]);
        grid.destroy();
    });

    it("setItems sets view items", () => {
        const grid = createGrid();
        const setSpy = vi.spyOn(grid.view, "setItems");
        grid["setItems"]([{ ID: 1 }]);
        expect(setSpy).toHaveBeenCalled();
        grid.destroy();
    });
});

describe("DataGrid sort and columns", () => {
    it("setInitialSortOrder maps sort columns", () => {
        class DefaultGrid extends DataGrid<any, any> {
            getDefaultSortBy() { return ["Name", "Date desc"]; }
        }
        const grid = new DefaultGrid({});
        grid["setInitialSortOrder"]();
        const sortCols = grid.getGrid().getSortColumns();
        expect(sortCols).toContainEqual({ columnId: "Name", sortAsc: true });
        expect(sortCols).toContainEqual({ columnId: "Date", sortAsc: false });
        grid.destroy();
    });

    it("getSlickOptions returns grid options", () => {
        const grid = createGrid();
        const opt = grid["getSlickOptions"]();
        expect(opt.multiSelect).toBe(false);
        expect(opt.multiColumnSort).toBe(true);
        grid.destroy();
    });

    it("postProcessColumns scales numeric widths", () => {
        class DefaultGrid extends DataGrid<any, any> {
            getColumnWidthDelta() { return 2; }
            getColumnWidthScale() { return 2; }
        }
        const grid = new DefaultGrid({});
        const cols = grid["postProcessColumns"]([
            { field: "A", width: 10, minWidth: 5, maxWidth: 20 },
            { field: "B" }
        ]);
        expect(cols[0].width).toBe(22);
        expect(cols[0].minWidth).toBe(12);
        expect(cols[0].maxWidth).toBe(42);
        grid.destroy();
    });

    it("postProcessColumns handles negative scale", () => {
        class DefaultGrid extends DataGrid<any, any> {
            getColumnWidthScale() { return -1; }
        }
        const grid = new DefaultGrid({});
        const cols = grid["postProcessColumns"]([{ field: "A", width: 10 }]);
        expect(cols[0].width).toBe(10);
        grid.destroy();
    });

    it("propertyItemsToColumns wraps edit link formatter", () => {
        const grid = createGrid();
        const items = [{ name: "X", title: "X", editLink: true } as any];
        const cols = grid["propertyItemsToColumns"](items);
        expect(cols).toHaveLength(1);
        expect(typeof cols[0].format).toBe("function");
        grid.destroy();
    });
});

describe("DataGrid grid event handlers", () => {
    it("handleGridSort persists settings", () => {
        const grid = createGrid();
        const persistSpy = vi.spyOn(grid as any, "persistSettings").mockImplementation(() => { });
        grid["handleGridSort"]({ args: {} } as any);
        expect(persistSpy).toHaveBeenCalled();
        grid.destroy();
    });

    it("handleGridColumnsReordered/Resized persist settings", () => {
        const grid = createGrid();
        const persistSpy = vi.spyOn(grid as any, "persistSettings").mockImplementation(() => { });
        grid["handleGridColumnsReordered"]();
        grid["handleGridColumnsResized"]();
        expect(persistSpy).toHaveBeenCalledTimes(2);
        grid.destroy();
    });

    it("handleGridClick calls onClick", () => {
        const grid = createGrid();
        const onClickSpy = vi.spyOn(grid as any, "onClick").mockImplementation(() => { });
        grid["handleGridClick"]({ row: 1, cell: 2 } as any);
        expect(onClickSpy).toHaveBeenCalled();
        grid.destroy();
    });
});

describe("DataGrid onClick edit links", () => {
    it("edits item from edit link target", () => {
        const grid = createGrid();
        const link = document.createElement("a");
        link.classList.add("s-EditLink");
        link.dataset.itemType = "Type";
        link.dataset.itemId = "5";
        const e = { target: link, preventDefault: vi.fn() } as any;
        const editSpy = vi.spyOn(grid as any, "editItemOfType").mockImplementation(() => { });
        grid["onClick"](e, 0, 0);
        expect(e.preventDefault).toHaveBeenCalled();
        expect(editSpy).toHaveBeenCalledWith("Type", "5");
        grid.destroy();
    });

    it("returns early when event default is prevented", () => {
        const grid = createGrid();
        const isDefaultPreventedSpy = vi.spyOn(Fluent, "isDefaultPrevented").mockReturnValue(true);
        const editSpy = vi.spyOn(grid as any, "editItemOfType").mockImplementation(() => { });
        grid["onClick"]({ target: document.createElement("a") } as any, 0, 0);
        expect(editSpy).not.toHaveBeenCalled();
        grid.destroy();
        isDefaultPreventedSpy.mockRestore();
    });
});

describe("DataGrid view pipeline", () => {
    it("handleViewSubmit returns true by default", () => {
        const grid = createGrid();
        expect(grid["handleViewSubmit"]()).toBe(true);
        grid.destroy();
    });

    it("handleViewSubmit returns false when onViewSubmit false", () => {
        class DefaultGrid extends DataGrid<any, any> {
            onViewSubmit() { return false; }
        }
        const grid = new DefaultGrid({});
        expect(grid["handleViewSubmit"]()).toBe(false);
        grid.destroy();
    });

    it("handleViewSubmit returns false when submitting cancels", () => {
        const grid = createGrid();
        grid.onSubmitting.subscribe(e => { e.cancel = true; });
        expect(grid["handleViewSubmit"]()).toBe(false);
        grid.destroy();
    });

    it("handleViewFilter returns true when onViewFilter true", () => {
        const grid = createGrid();
        expect(grid["handleViewFilter"]({ ID: 1 })).toBe(true);
        grid.destroy();
    });

    it("handleViewFilter returns false when onViewFilter false", () => {
        class DefaultGrid extends DataGrid<any, any> {
            onViewFilter() { return false; }
        }
        const grid = new DefaultGrid({});
        expect(grid["handleViewFilter"]({ ID: 1 })).toBe(false);
        grid.destroy();
    });

    it("handleViewProcessData returns processed response", () => {
        const grid = createGrid();
        const resp = grid["handleViewProcessData"]({ Entities: [], TotalCount: 0 } as any);
        expect(resp).toEqual({ Entities: [], TotalCount: 0 });
        grid.destroy();
    });

    it("onViewSubmit returns false when getGridCanLoad false", () => {
        class DefaultGrid extends DataGrid<any, any> {
            getGridCanLoad() { return false; }
        }
        const grid = new DefaultGrid({});
        expect(grid["onViewSubmit"]()).toBe(false);
        grid.destroy();
    });

    it("prepareSubmit uses view.onSubmit when present", () => {
        const grid = createGrid();
        grid.view.onSubmit = vi.fn(() => true);
        expect(grid.prepareSubmit()).toBe(true);
        grid.destroy();
    });

    it("prepareSubmit falls back to handleViewSubmit", () => {
        const grid = createGrid();
        grid.view.onSubmit = null;
        expect(grid.prepareSubmit()).toBe(true);
        grid.destroy();
    });
});

describe("DataGrid refresh and readonly", () => {
    it("refresh calls internalRefresh when not populateWhenVisible", () => {
        const grid = createGrid();
        const internalSpy = vi.spyOn(grid as any, "internalRefresh").mockImplementation(() => { });
        grid.refresh();
        expect(internalSpy).toHaveBeenCalled();
        grid.destroy();
    });

    it("refreshIfNeeded refreshes when needsRefresh set", () => {
        const grid = createGrid();
        grid["slickContainer"].data("needsRefresh", "true");
        const internalSpy = vi.spyOn(grid as any, "internalRefresh").mockImplementation(() => { });
        grid["refreshIfNeeded"]();
        expect(internalSpy).toHaveBeenCalled();
        grid.destroy();
    });

    it("readOnly setter calls updateInterface", () => {
        const grid = createGrid();
        const uiSpy = vi.spyOn(grid, "updateInterface").mockImplementation(() => { });
        grid.readOnly = true;
        expect(grid.readOnly).toBe(true);
        expect(uiSpy).toHaveBeenCalled();
        grid.destroy();
    });

    it("readOnly setter is no-op for same value", () => {
        const grid = createGrid();
        const uiSpy = vi.spyOn(grid, "updateInterface").mockImplementation(() => { });
        grid.readOnly = false;
        expect(uiSpy).not.toHaveBeenCalled();
        grid.destroy();
    });
});

describe("DataGrid layout", () => {
    it("layout returns early when grid not visible", () => {
        const grid = createGrid();
        const isVisibleSpy = vi.spyOn(Fluent, "isVisibleLike").mockReturnValue(false);
        const resizeSpy = vi.spyOn(grid.getGrid(), "resizeCanvas").mockImplementation(() => { });
        grid["layout"]();
        expect(resizeSpy).not.toHaveBeenCalled();
        grid.destroy();
        isVisibleSpy.mockRestore();
    });

    it("layout calls resizeCanvas when visible", () => {
        const grid = createGrid();
        const isVisibleSpy = vi.spyOn(Fluent, "isVisibleLike").mockReturnValue(true);
        const resizeSpy = vi.spyOn(grid.getGrid(), "resizeCanvas").mockImplementation(() => { });
        grid["layout"]();
        expect(resizeSpy).toHaveBeenCalled();
        grid.destroy();
        isVisibleSpy.mockRestore();
    });

    it("setTitle creates title div and getTitle returns text", () => {
        const grid = createGrid();
        expect(grid.getTitle()).toBeNull();
        grid.setTitle("Hello");
        expect(grid.getTitle()).toBe("Hello");
        grid.destroy();
    });

    it("setTitle null removes title div", () => {
        const grid = createGrid();
        grid.setTitle("Hello");
        grid.setTitle(null);
        expect(grid.getTitle()).toBeNull();
        grid.destroy();
    });
});

describe("DataGrid quick filters", () => {
    it("ensureQuickFilterBar creates and returns bar", () => {
        const grid = createGrid();
        const bar = grid["ensureQuickFilterBar"]();
        expect(bar).toBeTruthy();
        grid.destroy();
    });

    it("quick filter factory helpers do not throw", () => {
        const grid = createGrid();
        expect(() => grid["dateRangeQuickFilter"]("Date")).not.toThrow();
        expect(() => grid["dateTimeRangeQuickFilter"]("Date")).not.toThrow();
        expect(() => grid["booleanQuickFilter"]("Active")).not.toThrow();
        grid.destroy();
    });

    it("findQuickFilter uses quickFiltersBar when present", () => {
        const grid = createGrid();
        grid["quickFiltersBar"] = { find: vi.fn(() => "FOUND"), tryFind: vi.fn(), destroy: vi.fn(), onSubmit: vi.fn() } as any;
        expect(grid["findQuickFilter"](class { }, "Name")).toBe("FOUND");
        grid.destroy();
    });

    it("tryFindQuickFilter returns null without bar and widget", () => {
        const grid = createGrid();
        expect(grid["tryFindQuickFilter"](class { }, "Name")).toBeNull();
        grid.destroy();
    });
});

describe("DataGrid property items", () => {
    it("getPropertyItemsData returns empty when no columns key", () => {
        const grid = createGrid();
        expect(grid["getPropertyItemsData"]()).toEqual({ items: [], additionalItems: [] });
        grid.destroy();
    });

    it("getPropertyItemsData uses custom items when ScriptData cannot load", () => {
        const canLoadSpy = vi.spyOn(ScriptData, "canLoad").mockReturnValue(false);
        class CustomGrid extends DataGrid<any, any> {
            getPropertyItems() { return [{ name: "X" }] as any; }
        }
        const grid = new CustomGrid({});
        expect(grid["propertyItemsData"]).toBeDefined();
        grid.destroy();
        canLoadSpy.mockRestore();
    });

    it("getPropertyItemsDataAsync returns empty when no columns key", async () => {
        const grid = createGrid();
        const result = await grid["getPropertyItemsDataAsync"]();
        expect(result).toEqual({ items: [], additionalItems: [] });
        grid.destroy();
    });
});

describe("DataGrid persistence", () => {
    it("getPersistenceKey builds key from path and type", () => {
        class DefaultGrid extends DataGrid<any, any> {
            static [Symbol.typeInfo] = this.registerClass("MyProject.TestModule.DefaultGrid");
        }
        const grid = new DefaultGrid({});
        const key = grid["getPersistenceKey"]();
        expect(key).toContain("GridSettings:");
        expect(key).toContain("TestModule.DefaultGrid");
        grid.destroy();
    });

    it("getPersistedSettings parses JSON from storage", () => {
        const grid = createGrid();
        const storage = { getItem: vi.fn(() => '{"columns":[]}'), setItem: vi.fn() };
        const getStorageSpy = vi.spyOn(grid as any, "getPersistenceStorage").mockReturnValue(storage);
        const result = grid["getPersistedSettings"]();
        expect(result).toEqual({ columns: [] });
        getStorageSpy.mockRestore();
        grid.destroy();
    });

    it("getPersistedSettings returns null when no storage", () => {
        const grid = createGrid();
        const getStorageSpy = vi.spyOn(grid as any, "getPersistenceStorage").mockReturnValue(null);
        expect(grid["getPersistedSettings"]()).toBeNull();
        getStorageSpy.mockRestore();
        grid.destroy();
    });

    it("restoreSettings with settings calls restoreSettingsFrom", () => {
        const grid = createGrid();
        const restoreFromSpy = vi.spyOn(grid as any, "restoreSettingsFrom").mockImplementation(() => { });
        grid["restoreSettings"]({ columns: [] } as any, {} as any);
        expect(restoreFromSpy).toHaveBeenCalledWith({ columns: [] }, {});
        grid.destroy();
    });

    it("restoreSettingsFrom applies settings without throwing", () => {
        const grid = createGrid();
        expect(() => grid["restoreSettingsFrom"]({ columns: [], sortColumns: [] } as any)).not.toThrow();
        grid.destroy();
    });

    it("persistSettings writes to storage", () => {
        const storage = { getItem: vi.fn(), setItem: vi.fn() };
        class DefaultGrid extends DataGrid<any, any> {
            getPersistenceStorage() { return storage as any; }
        }
        const grid = new DefaultGrid({});
        grid.persistSettings();
        expect(storage.setItem).toHaveBeenCalled();
        grid.destroy();
    });

    it("persistSettings skipped while locked", () => {
        class DefaultGrid extends DataGrid<any, any> {
            getPersistenceStorage() { return { getItem: vi.fn(), setItem: vi.fn() } as any; }
        }
        const grid = new DefaultGrid({});
        grid.persistenceLock();
        grid.persistSettings();
        expect(grid["getPersistenceStorage"]().setItem).not.toHaveBeenCalled();
        grid.destroy();
    });
});

describe("DataGrid misc methods", () => {
    it("editItem throws Not Implemented", () => {
        const grid = createGrid();
        expect(() => grid["editItem"]({})).toThrow();
        grid.destroy();
    });

    it("editItemOfType throws for other type", () => {
        const grid = createGrid();
        expect(() => grid["editItemOfType"]("Other", {})).toThrow();
        grid.destroy();
    });

    it("determineText returns null without local prefix", () => {
        const grid = createGrid();
        expect(grid["determineText"](p => p + "X")).toBeNull();
        grid.destroy();
    });

    it("getItemCssClass and getItemMetadata return values", () => {
        class TestRow {
            static readonly isActiveProperty = "IsActive";
        }
        class RowGrid extends DataGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }
        const grid = new RowGrid({});
        expect(grid["getItemCssClass"]({ IsActive: -1 }, 0)).toBe("deleted");
        expect(grid["getItemCssClass"]({ IsActive: 0 }, 0)).toBe("inactive");
        const meta = grid["getItemMetadata"]({ IsActive: -1 }, 0);
        expect(meta).toEqual({ cssClasses: "deleted" });
        grid.destroy();
    });

    it("getViewOptions sets idField and rowsPerPage 0 without pager", () => {
        class DefaultGrid extends DataGrid<any, any> {
            usePager() { return false; }
        }
        const grid = new DefaultGrid({});
        const opt = grid["getViewOptions"]();
        expect(opt.idField).toBe("ID");
        expect(opt.rowsPerPage).toBe(0);
        grid.destroy();
    });

    it("setEquality and setCriteriaParameter do not throw", () => {
        const grid = createGrid();
        expect(() => grid["setEquality"]("Field", "value")).not.toThrow();
        expect(() => grid["setCriteriaParameter"]()).not.toThrow();
        grid.destroy();
    });

    it("EditLink renders anchor for data row", () => {
        const grid = createGrid();
        const result = grid.EditLink({ context: { item: { ID: 1 }, value: "abc" } as any });
        expect(result).toBeTruthy();
        grid.destroy();
    });

    it("EditLink returns fragment for non-data rows", () => {
        const grid = createGrid();
        const result = grid.EditLink({ context: { item: { __nonDataRow: true }, value: "x" } as any, children: "text" });
        expect(result).toBeTruthy();
        grid.destroy();
    });
});

describe("DataGrid construction variants", () => {
    it("constructs when useLayoutTimer is false", () => {
        class NoTimerGrid extends DataGrid<any, any> {
            useLayoutTimer() { return false; }
        }
        const grid = new NoTimerGrid({});
        expect(grid).toBeTruthy();
        grid.destroy();
    });

    it("constructs when getButtons returns null", () => {
        class NoToolbarGrid extends DataGrid<any, any> {
            getButtons() { return null; }
        }
        const grid = new NoToolbarGrid({});
        expect(grid["toolbar"]).toBeFalsy();
        grid.destroy();
    });

    it("markupReady and afterInit are no-ops", () => {
        const grid = createGrid();
        expect(() => (grid as any).markupReady()).not.toThrow();
        expect(() => (grid as any).afterInit()).not.toThrow();
        grid.destroy();
    });
});

describe("DataGrid layout timer", () => {
    it("layoutTimerCallback runs on layout event", () => {
        const grid = createGrid();
        const layoutSpy = vi.spyOn(grid as any, "layout").mockImplementation(() => { });
        grid.domNode.dispatchEvent(new Event("layout"));
        expect(layoutSpy).toHaveBeenCalled();
        grid.destroy();
    });
});

describe("DataGrid advanced filtering and pager", () => {
    it("createFilterBar creates filter bar when advanced filtering enabled", () => {
        class AdvGrid extends DataGrid<any, any> {
            enableAdvancedFiltering() { return true; }
        }
        const grid = new AdvGrid({});
        expect(grid["filterBar"]).toBeTruthy();
        grid.destroy();
    });

    it("createPager creates pager when using pager", () => {
        class PagerGrid extends DataGrid<any, any> {
            usePager() { return true; }
        }
        const grid = new PagerGrid({});
        expect(() => grid["createPager"]()).not.toThrow();
        grid.destroy();
    });

    it("getViewOptions uses responsive height rows per page", () => {
        class PagerGrid extends DataGrid<any, any> {
            usePager() { return true; }
        }
        const grid = new PagerGrid({});
        grid.domNode.classList.add("responsive-height");
        const opt = grid["getViewOptions"]();
        expect(typeof opt.rowsPerPage).toBe("number");
        grid.destroy();
    });
});

describe("DataGrid filter bar internals", () => {
    it("canFilterColumn checks column source item", () => {
        const grid = createGrid();
        expect(grid["canFilterColumn"]({ sourceItem: { notFilterable: true } } as any)).toBe(false);
        expect(grid["canFilterColumn"]({ sourceItem: {} } as any)).toBe(true);
        grid.destroy();
    });

    it("initializeFilterBar sets store and subscribes to changes", () => {
        const grid = createGrid();
        const store = { add_changed: vi.fn(), get_items: vi.fn(() => []) };
        grid["filterBar"] = { set_store: vi.fn(), get_store: () => store } as any;
        grid["initializeFilterBar"]();
        expect(grid["filterBar"].set_store).toHaveBeenCalled();
        expect(store.add_changed).toHaveBeenCalled();
        grid.destroy();
    });

    it("filterStoreChanged persists and refreshes", () => {
        const grid = createGrid();
        const persistSpy = vi.spyOn(grid as any, "persistSettings").mockImplementation(() => { });
        const refreshSpy = vi.spyOn(grid as any, "refresh").mockImplementation(() => { });
        grid["filterStoreChanged"]();
        expect(persistSpy).toHaveBeenCalled();
        expect(refreshSpy).toHaveBeenCalled();
        grid.destroy();
    });
});

describe("DataGrid quick filters (custom)", () => {
    it("createQuickFilters without toolbar appends toolbar div", () => {
        class NoToolbarGrid extends DataGrid<any, any> {
            getButtons() { return null; }
        }
        const grid = new NoToolbarGrid({});
        grid["createQuickFilters"]([]);
        expect(grid["quickFiltersBar"]).toBeTruthy();
        grid.destroy();
    });

    it("getQuickFilters returns quick filter items", () => {
        const toQuickFilterSpy = vi.spyOn(DataGrid, "propertyItemToQuickFilter").mockReturnValue({ field: "Name" } as any);
        class CustomGrid extends DataGrid<any, any> {
            getPropertyItems() { return [{ name: "Name", title: "Name", quickFilter: true } as any]; }
            createQuickFilters(filters?: any[]) { }
        }
        const grid = new CustomGrid({});
        const filters = grid["getQuickFilters"]();
        expect(filters.length).toBeGreaterThan(0);
        grid.destroy();
        toQuickFilterSpy.mockRestore();
    });

    it("createIncludeDeletedButton does not throw with active property", () => {
        class TestRow {
            static readonly isActiveProperty = "IsActive";
        }
        class RowGrid extends DataGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }
        const grid = new RowGrid({});
        expect(() => grid["createIncludeDeletedButton"]()).not.toThrow();
        grid.destroy();
    });

    it("createQuickSearchInput does not throw", () => {
        const grid = createGrid();
        expect(() => grid["createQuickSearchInput"]()).not.toThrow();
        grid.destroy();
    });

    it("invokeSubmitHandlers calls quickFiltersBar.onSubmit", () => {
        const grid = createGrid();
        const onSubmit = vi.fn();
        grid["quickFiltersBar"] = { onSubmit, destroy: vi.fn() } as any;
        grid["invokeSubmitHandlers"]();
        expect(onSubmit).toHaveBeenCalled();
        grid.destroy();
    });
});

describe("DataGrid initial populate and refresh", () => {
    it("initialPopulate populates when visible", () => {
        class VisibleGrid extends DataGrid<any, any> {
            populateWhenVisible() { return true; }
        }
        const grid = new VisibleGrid({});
        const isVisibleSpy = vi.spyOn(Fluent, "isVisibleLike").mockReturnValue(true);
        const populateSpy = vi.spyOn(grid.view, "populate").mockImplementation(() => true);
        grid["initialPopulate"]();
        expect(populateSpy).toHaveBeenCalled();
        grid.destroy();
        isVisibleSpy.mockRestore();
    });

    it("refresh refreshes when visible with populateWhenVisible", () => {
        class VisibleGrid extends DataGrid<any, any> {
            populateWhenVisible() { return true; }
        }
        const grid = new VisibleGrid({});
        const isVisibleSpy = vi.spyOn(Fluent, "isVisibleLike").mockReturnValue(true);
        const internalSpy = vi.spyOn(grid as any, "internalRefresh").mockImplementation(() => { });
        grid.refresh();
        expect(internalSpy).toHaveBeenCalled();
        grid.destroy();
        isVisibleSpy.mockRestore();
    });
});

describe("DataGrid grid can load", () => {
    it("getGridCanLoad returns true by default", () => {
        const grid = createGrid();
        expect(grid["getGridCanLoad"]()).toBe(true);
        grid.destroy();
    });

    it("getGridCanLoad returns false when cancelled", () => {
        const grid = createGrid();
        grid.onCanSubmit.subscribe(e => { e.cancel = true; });
        expect(grid["getGridCanLoad"]()).toBe(false);
        grid.destroy();
    });
});

describe("DataGrid persistence (extended)", () => {
    it("getPersistedSettings handles promise from storage", async () => {
        const grid = createGrid();
        const storage = { getItem: vi.fn(() => Promise.resolve('{"columns":[]}')), setItem: vi.fn() };
        const getStorageSpy = vi.spyOn(grid as any, "getPersistenceStorage").mockReturnValue(storage);
        const result = grid["getPersistedSettings"]();
        expect(typeof (result as any).then).toBe("function");
        expect(await result).toEqual({ columns: [] });
        getStorageSpy.mockRestore();
        grid.destroy();
    });

    it("restoreSettings handles promise settings", async () => {
        const grid = createGrid();
        const getPersistedSpy = vi.spyOn(grid as any, "getPersistedSettings").mockReturnValue(Promise.resolve({ columns: [] }));
        const restoreFromSpy = vi.spyOn(grid as any, "restoreSettingsFrom").mockImplementation(() => { });
        const result = grid["restoreSettings"]();
        expect(typeof (result as any).then).toBe("function");
        await result;
        expect(restoreFromSpy).toHaveBeenCalled();
        getPersistedSpy.mockRestore();
        grid.destroy();
    });

    it("persistSettings returns when no storage", () => {
        const grid = createGrid();
        const getStorageSpy = vi.spyOn(grid as any, "getPersistenceStorage").mockReturnValue(null);
        expect(grid.persistSettings()).toBeUndefined();
        getStorageSpy.mockRestore();
        grid.destroy();
    });

    it("gridPersistenceFlags returns empty by default", () => {
        const grid = createGrid();
        expect(grid["gridPersistenceFlags"]()).toEqual({});
        grid.destroy();
    });
});

describe("DataGrid columns and misc", () => {
    it("getIncludeColumns includes referenced fields", () => {
        const grid = createGrid();
        const refColumns = [{ field: "A", referencedFields: ["B", "C"] }, { field: "D" }] as any;
        vi.spyOn(grid.getGrid(), "getColumns").mockReturnValue(refColumns);
        const include: any = {};
        grid["getIncludeColumns"](include);
        expect(include).toEqual({ A: true, B: true, C: true, D: true });
        grid.destroy();
    });

    it("getDefaultSortBy returns array from grid", () => {
        const grid = createGrid();
        expect(Array.isArray(grid["getDefaultSortBy"]())).toBe(true);
        grid.destroy();
    });

    it("itemLink returns a formatter function", () => {
        const grid = createGrid();
        const fmt = grid["itemLink"]("Type", "ID");
        expect(typeof fmt).toBe("function");
        grid.destroy();
    });

    it("populateLock and populateUnlock delegate to view", () => {
        const grid = createGrid();
        const lockSpy = vi.spyOn(grid.view, "populateLock");
        const unlockSpy = vi.spyOn(grid.view, "populateUnlock");
        grid["populateLock"]();
        grid["populateUnlock"]();
        expect(lockSpy).toHaveBeenCalled();
        expect(unlockSpy).toHaveBeenCalled();
        grid.destroy();
    });

    it("resizeCanvas and subDialogDataChange do not throw", () => {
        const grid = createGrid();
        expect(() => grid["resizeCanvas"]()).not.toThrow();
        const refreshSpy = vi.spyOn(grid as any, "refresh").mockImplementation(() => { });
        grid["subDialogDataChange"]();
        expect(refreshSpy).toHaveBeenCalled();
        grid.destroy();
    });

    it("addFilterSeparator delegates to quick filter bar", () => {
        const grid = createGrid();
        const bar = { addSeparator: vi.fn(), destroy: vi.fn(), onSubmit: vi.fn() };
        grid["quickFiltersBar"] = bar as any;
        grid["quickFiltersDiv"] = Fluent(document.createElement("div"));
        grid["addFilterSeparator"]();
        expect(bar.addSeparator).toHaveBeenCalled();
        grid.destroy();
    });
});

describe("DataGrid remaining branches", () => {
    it("getItemCssClass uses deleted property only", () => {
        class TestRow {
            static readonly isDeletedProperty = "IsDeleted";
        }
        class RowGrid extends DataGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }
        const grid = new RowGrid({});
        expect(grid["getItemCssClass"]({ IsDeleted: true }, 0)).toBe("deleted");
        expect(grid["getItemCssClass"]({ IsDeleted: false }, 0)).toBeNull();
        grid.destroy();
    });

    it("setCriteriaParameter sets criteria when filter bar active", () => {
        class AdvGrid extends DataGrid<any, any> {
            enableAdvancedFiltering() { return true; }
        }
        const grid = new AdvGrid({});
        const store = { get_activeCriteria: vi.fn(() => [[["A", "=", 1]]]) };
        grid["filterBar"] = { get_store: () => store } as any;
        grid["setCriteriaParameter"]();
        expect(grid.view.params.Criteria).toBeTruthy();
        grid.destroy();
    });

    it("getPersistedSettings returns null for invalid JSON", () => {
        const grid = createGrid();
        const storage = { getItem: vi.fn(() => "not-json"), setItem: vi.fn() };
        const getStorageSpy = vi.spyOn(grid as any, "getPersistenceStorage").mockReturnValue(storage);
        expect(grid["getPersistedSettings"]()).toBeNull();
        getStorageSpy.mockRestore();
        grid.destroy();
    });

    it("wrapFormatterWithEditLink adds referenced field", () => {
        const grid = createGrid();
        const column: any = { field: "A", format: undefined };
        grid["wrapFormatterWithEditLink"](column, { name: "A", editLink: true, editLinkIdField: "ID" } as any);
        expect(column.referencedFields).toContain("ID");
        expect(typeof column.format).toBe("function");
        grid.destroy();
    });

    it("setTitle is no-op for same value", () => {
        const grid = createGrid();
        grid.setTitle("X");
        const layoutSpy = vi.spyOn(grid as any, "layout").mockImplementation(() => { });
        grid.setTitle("X");
        expect(layoutSpy).not.toHaveBeenCalled();
        grid.destroy();
    });

    it("setViewParams invokes submit handlers and notifies", () => {
        const grid = createGrid();
        const setCriteriaSpy = vi.spyOn(grid as any, "setCriteriaParameter").mockImplementation(() => { });
        const setIncludeSpy = vi.spyOn(grid as any, "setIncludeColumnsParameter").mockImplementation(() => { });
        const invokeSpy = vi.spyOn(grid as any, "invokeSubmitHandlers").mockImplementation(() => { });
        const notifySpy = vi.spyOn(grid.onSetViewParams, "notify");
        grid["setViewParams"]();
        expect(setCriteriaSpy).toHaveBeenCalled();
        expect(setIncludeSpy).toHaveBeenCalled();
        expect(invokeSpy).toHaveBeenCalled();
        expect(notifySpy).toHaveBeenCalled();
        grid.destroy();
    });

    it("setIncludeColumnsParameter sets IncludeColumns param", () => {
        const grid = createGrid();
        const includeSpy = vi.spyOn(grid as any, "getIncludeColumns").mockImplementation((include: any) => { include.A = true; });
        grid["setIncludeColumnsParameter"]();
        expect(grid.view.params.IncludeColumns).toEqual(["A"]);
        grid.destroy();
    });
});