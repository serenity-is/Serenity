import { Column, formatterContext } from "@serenity-is/sleekgrid";
import { describe, expect, it, vi } from "vitest";
import { Fluent } from "../../base";
import { ScriptData } from "../../compat";
import { GridRowSelectionMixin } from "../helpers/gridrowselectionmixin";
import { DataGrid, omitAllGridPersistenceFlags } from "./datagrid";
import { dataGridDefaults } from "./datagrid-defaults";
import { GridStyle } from "./gridstyle";

function createGrid(): DataGrid<any, any> {
    class DefaultGrid extends DataGrid<any, any> { }
    return new DefaultGrid({});
}

function getIdProperty(grid: DataGrid<any, any>): string {
    return grid["getIdProperty"]();
}

describe('DataGrid.ensureRowSelection', () => {
    function gridWithColumns(columns: Column[]): DataGrid<any, any> {
        class TestGrid extends DataGrid<any, any> { }
        const grid = new TestGrid({});
        const event = () => ({ subscribe: vi.fn(), unsubscribe: vi.fn() });
        (grid as any)._grid = {
            getColumns: () => columns,
            onClick: event(),
            onHeaderClick: event(),
            onKeyDown: event()
        };
        (grid.view as any).idProperty = "Id";
        return grid;
    }

    it("creates the mixin for the column of the check boxes of a columns class", () => {
        const grid = gridWithColumns([
            GridRowSelectionMixin.createSelectColumn(),
            { id: "Name", field: "Name" } as Column
        ]);

        grid["initRowSelector"]();

        expect(GridRowSelectionMixin.tryGetInstance(grid)).toBeInstanceOf(GridRowSelectionMixin);

        // a second call does not create another one
        const first = GridRowSelectionMixin.tryGetInstance(grid);
        grid["initRowSelector"]();
        expect(GridRowSelectionMixin.tryGetInstance(grid)).toBe(first);

        first.destroy();
    });

    it("does nothing for a grid without the column", () => {
        const plain = gridWithColumns([{ id: "Name", field: "Name" } as Column]);
        plain["initRowSelector"]();
        expect(GridRowSelectionMixin.tryGetInstance(plain)).toBeNull();
    });
});

describe('DataGrid.getIdProperty', () => {
    it('returns ID by default', () => {
        class DefaultGrid extends DataGrid<any, any> {
        }

        const grid = new DefaultGrid({});
        expect(getIdProperty(grid)).toBe("ID");
    });

    it('can be overridden in subclass', () => {
        class SubClassGrid extends DataGrid<any, any> {
            getIdProperty() { return "subClassId" };
        }

        const grid = new SubClassGrid({});
        expect(getIdProperty(grid)).toBe("subClassId");
    });

    it('returns value from getRowDefition()', () => {
        class TestRow {
            static readonly idProperty = "idForTestRow";
        }

        class TestRowGrid extends DataGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        const grid = new TestRowGrid({});
        expect(getIdProperty(grid)).toBe("idForTestRow");
    });

    it("returns empty string if getRowDefition() doesn't have the value", () => {
        class TestRow {
            static readonly idProperty: string = undefined;
        }

        class TestRowGrid extends DataGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        const grid = new TestRowGrid({});
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

        const grid = new DefaultGrid({});
        expect(getIsActiveProperty(grid)).toBe("");
    });

    it('can be set via attribute', () => {
        class SubClassGrid extends DataGrid<any, any> {
            getIsActiveProperty() { return "subClassIsActive" };
        }

        const grid = new SubClassGrid({});
        expect(getIsActiveProperty(grid)).toBe("subClassIsActive");
    });

    it('returns value from getRowDefinition()', () => {
        class TestRow {
            static readonly isActiveProperty = "activeForTestRow";
        }

        class TestRowGrid extends DataGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        const grid = new TestRowGrid({});
        expect(getIsActiveProperty(grid)).toBe("activeForTestRow");
    });

    it("returns empty string if getRowDefinition() doesn't have the value", () => {
        class TestRow {
            static readonly isActiveProperty: string = undefined;
        }

        class TestRowGrid extends DataGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        const grid = new TestRowGrid({});
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

        const grid = new DefaultGrid({});
        expect(getLocalTextDbPrefix(grid)).toBe("");
    });

    it('can be overridden in subclass via getLocalTextDbPrefix', () => {
        class SubClassGrid extends DataGrid<any, any> {
            getLocalTextDbPrefix() { return "My.Prefix." };
        }

        const grid = new SubClassGrid({});
        expect(getLocalTextDbPrefix(grid)).toBe("My.Prefix.");
    });

    it('can be overridden in subclass via getLocalTextPrefix', () => {
        class SubClassGrid extends DataGrid<any, any> {
            getLocalTextPrefix() { return "MySubClassPrefix" };
        }

        const grid = new SubClassGrid({});
        expect(getLocalTextDbPrefix(grid)).toBe("Db.MySubClassPrefix.");
    });

    it('returns value from getRowDefinition()', () => {
        class TestRow {
            static readonly localTextPrefix = "prefixForTestRow";
        }

        class TestRowGrid extends DataGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        const grid = new TestRowGrid({});
        expect(getLocalTextDbPrefix(grid)).toBe("Db.prefixForTestRow.");
    });

    it("returns empty string if getRowDefinition() doesn't have the value", () => {
        class TestRow {
            static readonly localTextPrefix: string = undefined;
        }

        class TestRowGrid extends DataGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        const grid = new TestRowGrid({});
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

        const grid = new DefaultGrid({});
        expect(getLocalTextPrefix(grid)).toBeUndefined();
    });

    it('can be overridden in subclass', () => {
        class SubClassGrid extends DataGrid<any, any> {
            getLocalTextPrefix() { return "subClassPrefix" };
        }

        const grid = new SubClassGrid({});
        expect(getLocalTextPrefix(grid)).toBe("subClassPrefix");
    });

    it('returns value from getRowDefinition()', () => {
        class TestRow {
            static readonly localTextPrefix = "prefixForTestRow";
        }

        class TestRowGrid extends DataGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        const grid = new TestRowGrid({});
        expect(getLocalTextPrefix(grid)).toBe("prefixForTestRow");
    });

    it("returns undefined if getRowDefinition() doesn't have the value", () => {
        class TestRow {
            static readonly localTextPrefix: string = undefined;
        }

        class TestRowGrid extends DataGrid<any, any> {
            getRowDefinition() { return TestRow; }
        }

        const grid = new TestRowGrid({});
        expect(getLocalTextPrefix(grid)).toBeUndefined();
    });
});

describe("DataGrid.getCurrentSettings", () => {
    it("identifies persistence triggered by a column resize", () => {
        const grid = createGrid();
        vi.spyOn(grid as any, "getPersistenceStorage").mockReturnValue({ setItem: vi.fn() });
        const resized: boolean[] = [];
        grid.onPersistence.subscribe(event => {
            if (event.persisting && !event.after)
                resized.push(event.columnResized);
        });

        grid.sleekGrid.columnsResized();
        grid.persistSettings();

        expect(resized).toEqual([true, false]);
        grid.destroy();
    });

    it("lets persistence handlers adjust flags before capturing settings", () => {
        const grid = createGrid();
        grid.sleekGrid.setColumns([{ id: "A", width: 120 }]);
        const flags = { columnVisibility: true };
        const phases: boolean[] = [];
        grid.onPersistence.subscribe(event => {
            if (event.persisting) {
                phases.push(event.after);
                if (!event.after)
                    event.flagsToUse.columnWidths = false;
            }
        });

        const settings = grid.getCurrentSettings(flags);

        expect(phases).toEqual([false, true]);
        expect(settings.flags.columnWidths).toBe(false);
        expect(settings.columns).toEqual([{ id: "A", visible: true, pin: false }]);
        expect(flags).toEqual({ columnVisibility: true });
        grid.destroy();
    });

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
        expect((grid as any).slickGrid).toBe(grid.getGrid());
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

    it("restoreSettings with settings calls restoreSettingsFrom with snapshot defaults", () => {
        const grid = createGrid();
        const restoreFromSpy = vi.spyOn(grid as any, "restoreSettingsFrom").mockImplementation(() => { });
        grid["restoreSettings"]({ columns: [] } as any, {} as any);
        expect(restoreFromSpy).toHaveBeenCalledWith({ columns: [] }, {});
        grid.destroy();
    });

    it("restoreSettings flags automatic storage restores and leaves restore options to be defaulted after persistence handlers", () => {
        const grid = createGrid();
        const settings = { flags: { columnVisibility: true }, columns: [] };
        vi.spyOn(grid as any, "getPersistedSettings").mockReturnValue(settings);
        const restoreFromSpy = vi.spyOn(grid as any, "restoreSettingsFrom").mockImplementation(() => { });

        grid["restoreSettings"]();

        expect(restoreFromSpy).toHaveBeenCalledWith(settings, {
            restoreOptions: { newVisibleColumns: undefined, onRevisionMismatch: undefined }
        });
        grid.destroy();
    });

    it("automatic restores default restore options after persistence handlers, which can set their own", () => {
        const grid = createGrid();
        grid["propertyItemsData"] = { items: [], additionalItems: [], revision: 2 };
        vi.spyOn(grid.getGrid(), "getAllColumns").mockReturnValue([{ id: "A" }, { id: "B" }]);
        const setVisibleColumns = vi.spyOn(grid.getGrid(), "setVisibleColumns").mockImplementation(() => { });
        const settings = { columnsRevision: 1, flags: { columnVisibility: true }, columns: [{ id: "A", visible: true }] };
        vi.spyOn(grid as any, "getPersistedSettings").mockReturnValue(settings);
        grid.onPersistence.subscribe(event => {
            if (event.restoring && !event.after)
                (event.flagsToUse.restoreOptions ??= {}).onRevisionMismatch ??= "merge";
        });

        grid["restoreSettings"]();

        expect(setVisibleColumns).toHaveBeenCalledWith(["A", "B"], { notify: false });
        grid.destroy();
    });

    it("restoreSettings permits disabling new column merging for automatic storage restores", () => {
        const grid = createGrid();
        const settings = { flags: { columnVisibility: true }, columns: [] };
        vi.spyOn(grid as any, "getPersistedSettings").mockReturnValue(settings);
        vi.spyOn(grid as any, "gridPersistenceFlags").mockReturnValue({
            restoreOptions: { newVisibleColumns: false }, columnWidths: false
        });
        const restoreFromSpy = vi.spyOn(grid as any, "restoreSettingsFrom").mockImplementation(() => { });

        grid["restoreSettings"]();

        expect(restoreFromSpy).toHaveBeenCalledWith(settings, {
            restoreOptions: { newVisibleColumns: false, onRevisionMismatch: undefined }, columnWidths: false
        });
        grid.destroy();
    });

    it("merges automatic restore flags with grid defaults and honors explicit restore options", async () => {
        const grid = createGrid();
        const settings = { flags: { columnVisibility: true }, columns: [] };
        vi.spyOn(grid as any, "getPersistedSettings").mockResolvedValue(settings);
        vi.spyOn(grid as any, "gridPersistenceFlags").mockReturnValue({
            restoreOptions: { newVisibleColumns: false }, columnWidths: false
        });
        const restoreFromSpy = vi.spyOn(grid as any, "restoreSettingsFrom").mockImplementation(() => { });
        const flags = { restoreOptions: { newVisibleColumns: true }, sortColumns: false };

        await grid["restoreSettings"](null, flags);

        expect(restoreFromSpy).toHaveBeenCalledWith(settings, {
            restoreOptions: { newVisibleColumns: true, onRevisionMismatch: undefined }, columnWidths: false, sortColumns: false
        });
        expect(flags).toEqual({ restoreOptions: { newVisibleColumns: true }, sortColumns: false });
        grid.destroy();
    });

    it("inherits null automatic restore options from grid defaults", () => {
        const grid = createGrid();
        const settings = { flags: { columnVisibility: true }, columns: [] };
        vi.spyOn(grid as any, "getPersistedSettings").mockReturnValue(settings);
        vi.spyOn(grid as any, "gridPersistenceFlags").mockReturnValue({
            restoreOptions: { newVisibleColumns: false, onRevisionMismatch: "merge" }
        });
        const restoreFromSpy = vi.spyOn(grid as any, "restoreSettingsFrom").mockImplementation(() => { });

        grid["restoreSettings"](null, { restoreOptions: { newVisibleColumns: null, onRevisionMismatch: null } });

        expect(restoreFromSpy).toHaveBeenCalledWith(settings, {
            restoreOptions: { newVisibleColumns: false, onRevisionMismatch: "merge" }
        });
        grid.destroy();
    });

    it.each([
        { name: "drops flags", overrideFlags: undefined, expectedVisible: ["A"], expectedWidth: 250, reset: false },
        { name: "supplies partial flags", overrideFlags: { columnWidths: false }, expectedVisible: ["A"], expectedWidth: 120, reset: false },
        { name: "overrides a restore option", overrideFlags: { columnWidths: false, restoreOptions: { newVisibleColumns: true } }, expectedVisible: ["A", "B"], expectedWidth: 120, reset: false },
        { name: "inherits a null restore option", overrideFlags: { columnWidths: false, restoreOptions: { onRevisionMismatch: null } }, expectedVisible: ["A"], expectedWidth: 120, reset: false },
        { name: "overrides revision handling", overrideFlags: { columnWidths: false, restoreOptions: { onRevisionMismatch: "reset" } }, expectedVisible: ["A", "B"], expectedWidth: 120, reset: true }
    ])("preserves automatic flags when a restore override $name", ({ overrideFlags, expectedVisible, expectedWidth, reset }) => {
        const grid = createGrid();
        grid["propertyItemsData"] = { items: [], additionalItems: [], revision: 2 };
        grid.sleekGrid.setColumns([{ id: "A", width: 120 }, { id: "B", width: 140 }]);
        grid["initialSettings"] = grid.getCurrentSettings();
        vi.spyOn(grid as any, "gridPersistenceFlags").mockReturnValue({
            restoreOptions: { newVisibleColumns: false, onRevisionMismatch: "merge" }
        });
        vi.spyOn(grid as any, "getPersistedSettings").mockReturnValue({
            columnsRevision: 1,
            flags: { columnVisibility: true, columnWidths: true },
            columns: [{ id: "A", visible: true, width: 250 }]
        });
        const restoreFrom = grid["restoreSettingsFrom"].bind(grid);
        vi.spyOn(grid as any, "restoreSettingsFrom").mockImplementation(settings => restoreFrom(settings, overrideFlags as any));
        let resetLayout: boolean;
        grid.onPersistence.subscribe(event => {
            if (event.restoring && event.after)
                resetLayout = event.columnLayoutReset;
        });

        grid["restoreSettings"]();

        expect(resetLayout).toBe(reset);
        expect(grid.columns.map(column => column.id)).toEqual(expectedVisible);
        expect(grid.allColumns.find(column => column.id === "A").width).toBe(expectedWidth);
        grid.destroy();
    });

    it("reports effective automatic restore options after persistence handlers", () => {
        const grid = createGrid();
        vi.spyOn(grid as any, "getPersistedSettings").mockReturnValue({ columns: [] });
        const options: any[] = [];
        grid.onPersistence.subscribe(event => {
            if (event.restoring)
                options.push({ ...event.flagsToUse.restoreOptions });
        });

        grid["restoreSettings"]();

        expect(options).toEqual([
            { newVisibleColumns: undefined, onRevisionMismatch: undefined },
            { newVisibleColumns: true, onRevisionMismatch: "reset" }
        ]);
        grid.destroy();
    });

    it("uses snapshot semantics for direct restores even when the grid enables new column merging", () => {
        const grid = createGrid();
        vi.spyOn(grid as any, "gridPersistenceFlags").mockReturnValue({ restoreOptions: { newVisibleColumns: true } });
        vi.spyOn(grid.getGrid(), "getAllColumns").mockReturnValue([{ id: "A" }, { id: "B" }]);
        const setVisibleColumns = vi.spyOn(grid.getGrid(), "setVisibleColumns").mockImplementation(() => { });
        const settings = { flags: { columnVisibility: true }, columns: [{ id: "A", visible: true }] };

        grid["restoreSettingsFrom"](settings);
        expect(setVisibleColumns).toHaveBeenLastCalledWith(["A"], { notify: false });

        grid["restoreSettingsFrom"](settings, { restoreOptions: { newVisibleColumns: true } });
        expect(setVisibleColumns).toHaveBeenLastCalledWith(["A", "B"], { notify: false });
        grid.destroy();
    });

    it("merges new columns according to initial visibility after restoring a saved view", () => {
        const grid = createGrid();
        grid.sleekGrid.setColumns([
            { id: "A" },
            { id: "B" },
            { id: "C", visible: false }
        ]);
        grid["initialSettings"] = grid.getCurrentSettings();
        grid["restoreSettingsFrom"]({
            flags: { columnVisibility: true },
            columns: [{ id: "A", visible: true }, { id: "C", visible: true }]
        });
        expect(grid.columns.map(column => column.id)).toEqual(["A", "C"]);

        vi.spyOn(grid as any, "getPersistedSettings").mockReturnValue({
            flags: { columnVisibility: true },
            columns: [{ id: "A", visible: true }]
        });
        grid["restoreSettings"]();

        expect(grid.columns.map(column => column.id)).toEqual(["A", "B"]);
        grid.destroy();
    });

    it("merges new columns in initial order after a saved view reorders the grid", () => {
        const grid = createGrid();
        grid.sleekGrid.setColumns([{ id: "A" }, { id: "B" }, { id: "C" }]);
        grid["initialSettings"] = grid.getCurrentSettings();
        grid["restoreSettingsFrom"]({
            flags: { columnVisibility: true },
            columns: [{ id: "B", visible: true }, { id: "C", visible: true }, { id: "A", visible: true }]
        });
        expect(grid.columns.map(column => column.id)).toEqual(["B", "C", "A"]);
        vi.spyOn(grid as any, "getPersistedSettings").mockReturnValue({
            flags: { columnVisibility: true },
            columns: [{ id: "A", visible: true }, { id: "C", visible: true }]
        });

        grid["restoreSettings"]();

        expect(grid.columns.map(column => column.id)).toEqual(["A", "B", "C"]);
        grid.destroy();
    });

    it("allows persistence event handlers to set restore options", () => {
        const grid = createGrid();
        vi.spyOn(grid.getGrid(), "getAllColumns").mockReturnValue([{ id: "A" }, { id: "B" }]);
        const setVisibleColumns = vi.spyOn(grid.getGrid(), "setVisibleColumns").mockImplementation(() => { });
        grid.onPersistence.subscribe(event => {
            if (event.restoring && !event.after)
                event.flagsToUse.restoreOptions = { newVisibleColumns: true };
        });

        grid["restoreSettingsFrom"]({ flags: { columnVisibility: true }, columns: [{ id: "A", visible: true }] });

        expect(setVisibleColumns).toHaveBeenCalledWith(["A", "B"], { notify: false });
        grid.destroy();
    });

    it("reads columnsRevision from propertyItemsData and resets on automatic restore mismatch", () => {
        const grid = createGrid();
        grid["propertyItemsData"] = { items: [], additionalItems: [], revision: 2 };
        expect(grid["getColumnsRevision"]()).toBe(2);

        vi.spyOn(grid.getGrid(), "getAllColumns").mockReturnValue([{ id: "A" }, { id: "B" }]);
        const setVisibleColumns = vi.spyOn(grid.getGrid(), "setVisibleColumns").mockImplementation(() => { });
        const settings = { columnsRevision: 1, flags: { columnVisibility: true }, columns: [{ id: "A", visible: true }] };

        // Direct restore ignores revision mismatch
        grid["restoreSettingsFrom"](settings);
        expect(setVisibleColumns).not.toHaveBeenCalledWith(["A", "B"], expect.anything());
        expect(setVisibleColumns).toHaveBeenCalledWith(["A"], { notify: false });

        // Automatic restore resets column layout on revision mismatch
        setVisibleColumns.mockClear();
        vi.spyOn(grid as any, "getPersistedSettings").mockReturnValue(settings);
        grid["restoreSettings"]();
        expect(setVisibleColumns).not.toHaveBeenCalled();

        // Persist captures current columnsRevision
        expect(grid.getCurrentSettings().columnsRevision).toBe(2);
        grid.destroy();
    });

    it("uses independent mismatch defaults for automatic and direct restores", () => {
        const grid = createGrid();
        grid["propertyItemsData"] = { items: [], additionalItems: [], revision: 2 };
        vi.spyOn(grid.getGrid(), "getAllColumns").mockReturnValue([{ id: "A" }, { id: "B" }]);
        const setVisibleColumns = vi.spyOn(grid.getGrid(), "setVisibleColumns").mockImplementation(() => { });
        const settings = { columnsRevision: 1, flags: { columnVisibility: true }, columns: [{ id: "A", visible: true }] };

        grid["restoreSettingsFrom"](settings, { restoreOptions: { newVisibleColumns: true } });
        expect(setVisibleColumns).toHaveBeenCalledWith(["A", "B"], { notify: false });

        setVisibleColumns.mockClear();
        vi.spyOn(grid as any, "gridPersistenceFlags").mockReturnValue({ restoreOptions: { newVisibleColumns: false } });
        vi.spyOn(grid as any, "getPersistedSettings").mockReturnValue(settings);
        grid["restoreSettings"]();
        expect(setVisibleColumns).not.toHaveBeenCalled();
        grid.destroy();
    });

    it("restores the initial column layout and saved sorting on a revision mismatch", () => {
        const grid = createGrid();
        grid["propertyItemsData"] = { items: [], additionalItems: [], revision: 2 };
        grid.sleekGrid.setColumns([
            { id: "A", width: 120, frozen: "start" },
            { id: "B", width: 150 },
            { id: "C", width: 160, visible: false }
        ]);
        grid["initialSettings"] = grid.getCurrentSettings();
        grid.sleekGrid.reorderColumns(["B", "C", "A"], { setVisible: ["B", "C"], notify: false });
        grid.allColumns.find(column => column.id === "A").width = 400;
        grid.allColumns.find(column => column.id === "A").frozen = null;
        grid.allColumns.find(column => column.id === "B").frozen = "end";

        grid["restoreSettingsFrom"]({
            columnsRevision: 1,
            flags: { columnVisibility: true, columnWidths: true, columnPinning: true, sortColumns: true },
            columns: [{ id: "B", visible: true, width: 300, pin: "end", sort: -1 }]
        }, { restoreOptions: { onRevisionMismatch: "reset" } });

        expect(grid.allColumns.map(column => column.id)).toEqual(["A", "B", "C"]);
        expect(grid.columns.map(column => column.id)).toEqual(["A", "B"]);
        expect(grid.allColumns.map(column => column.width)).toEqual([120, 150, 160]);
        expect(grid.allColumns.map(column => column.frozen)).toEqual(["start", null, null]);
        expect(grid.view.sortBy).toEqual(["B DESC"]);
        expect(grid.sleekGrid.getSortColumns()).toEqual([{ columnId: "B", sortAsc: false }]);
        grid.destroy();
    });

    it("reports whether column widths were restored or discarded on revision mismatch", () => {
        const grid = createGrid();
        grid["propertyItemsData"] = { items: [], additionalItems: [], revision: 2 };
        grid.sleekGrid.setColumns([{ id: "A", width: 120 }]);
        grid["initialSettings"] = grid.getCurrentSettings();
        const outcomes: { columnLayoutReset: boolean, columnWidthsRestored: boolean }[] = [];
        grid.onPersistence.subscribe(event => {
            if (event.restoring && event.after)
                outcomes.push({ columnLayoutReset: event.columnLayoutReset, columnWidthsRestored: event.columnWidthsRestored });
        });
        const settings = { columnsRevision: 1, flags: { columnWidths: true }, columns: [{ id: "A", width: 200 }] };

        grid["restoreSettingsFrom"](settings, { restoreOptions: { onRevisionMismatch: "reset" } });
        grid["restoreSettingsFrom"](settings, { restoreOptions: { onRevisionMismatch: "merge" } });
        grid["restoreSettingsFrom"](settings, { columnWidths: false });

        expect(outcomes).toEqual([
            { columnLayoutReset: true, columnWidthsRestored: false },
            { columnLayoutReset: false, columnWidthsRestored: true },
            { columnLayoutReset: false, columnWidthsRestored: false }
        ]);
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

    it("getViewOptions has serverAggregates off by default, and from dataGridDefaults, which a grid can override", () => {
        class DefaultGrid extends DataGrid<any, any> { }
        class OverridingGrid extends DataGrid<any, any> {
            protected override getViewOptions() {
                return { ...super.getViewOptions(), serverAggregates: false };
            }
        }

        const grid = new DefaultGrid({});
        const overriding = new OverridingGrid({});
        try {
            expect(grid["getViewOptions"]().serverAggregates).toBe(false);

            dataGridDefaults.serverAggregates = true;
            expect(grid["getViewOptions"]().serverAggregates).toBe(true);
            expect(overriding["getViewOptions"]().serverAggregates).toBe(false);
        }
        finally {
            dataGridDefaults.serverAggregates = null;
            grid.destroy();
            overriding.destroy();
        }
    });

    it("getViewOptions has displayTotalsRow from dataGridDefaults, which a grid can override", () => {
        class DefaultGrid extends DataGrid<any, any> { }
        class OverridingGrid extends DataGrid<any, any> {
            protected override getViewOptions() {
                return { ...super.getViewOptions(), displayTotalsRow: true };
            }
        }

        const grid = new DefaultGrid({});
        const overriding = new OverridingGrid({});
        try {
            // without a default, the views keep their own, which is a totals row
            expect(grid["getViewOptions"]().displayTotalsRow).toBeNull();

            // e.g. what the application template sets, to show the totals on the group rows
            dataGridDefaults.displayTotalsRow = "groupheader";
            expect(grid["getViewOptions"]().displayTotalsRow).toBe("groupheader");
            expect(overriding["getViewOptions"]().displayTotalsRow).toBe(true);
        }
        finally {
            dataGridDefaults.displayTotalsRow = null;
            grid.destroy();
            overriding.destroy();
        }
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

describe("DataGrid destroy", () => {
    it("stops the view, so the rows it asked for that arrive later do not reach the destroyed grid", () => {
        const grid = createGrid();
        const view = grid.view;
        const onProcessData = vi.fn(() => ({ Entities: [] }));

        view.onProcessData = onProcessData;
        grid.destroy();

        expect(grid.view).toBeNull();
        expect(grid.sleekGrid).toBeNull();

        // the response of a request that was made before the grid was destroyed
        expect(view.addData({ TotalCount: 1, Entities: [{ ID: 1 }] })).toBe(false);
        expect(onProcessData).not.toHaveBeenCalled();
    });
});

describe("DataGrid grid style", () => {
    const styleClasses = ["slick-no-odd-even", "slick-no-cell-border", "slick-no-header-chrome"];
    const classesOf = (grid: DataGrid<any, any>) => styleClasses.filter(x => grid.domNode.classList.contains(x));

    it("has none of the classes by default, so the grid looks as its theme says", () => {
        expect(classesOf(createGrid())).toEqual([]);
    });

    it("adds the classes of the members that getInitialGridStyle returns while the grid is created", () => {
        class LinesGrid extends DataGrid<any, any> {
            protected override getInitialGridStyle() { return GridStyle.Lines; }
        }
        expect(classesOf(new LinesGrid({}))).toEqual(["slick-no-odd-even", "slick-no-header-chrome"]);

        class PlainGrid extends DataGrid<any, any> {
            protected override getInitialGridStyle() { return GridStyle.Plain; }
        }
        expect(classesOf(new PlainGrid({}))).toEqual(styleClasses);
    });

    it("can read the style from the options of the grid", () => {
        class OptionGrid extends DataGrid<any, { style?: GridStyle }> {
            protected override getInitialGridStyle() { return this.options.style; }
        }
        expect(classesOf(new OptionGrid({ style: GridStyle.NoCellBorders }))).toEqual(["slick-no-cell-border"]);
    });

    it("changes the classes when the style is set again, and removes only the ones it added", () => {
        const grid = createGrid();
        grid.domNode.classList.add("slick-no-cell-border");
        grid.setGridStyle(GridStyle.NoStripes);
        expect(classesOf(grid)).toEqual(["slick-no-odd-even", "slick-no-cell-border"]);
        grid.setGridStyle(GridStyle.NoHeaderChrome);
        expect(classesOf(grid)).toEqual(["slick-no-cell-border", "slick-no-header-chrome"]);
        grid.setGridStyle(null);
        expect(classesOf(grid)).toEqual(["slick-no-cell-border"]);
    });

    it("keeps a class that was on the element before, which the style also has", () => {
        const grid = createGrid();
        grid.domNode.classList.add("slick-no-odd-even");
        grid.setGridStyle(null);
        expect(classesOf(grid)).toEqual(["slick-no-odd-even"]);
    });

    it("accepts the number as a text, as the enum editor stores it", () => {
        const grid = createGrid();
        grid.setGridStyle("5" as any);
        expect(classesOf(grid)).toEqual(["slick-no-odd-even", "slick-no-header-chrome"]);
    });
});

describe("DataGrid edit links", () => {
    // OrderID, Customer and Date link to the order of the row, Employee to an employee of its own
    const items = () => [
        { name: "OrderID", title: "OrderID", editLink: true },
        { name: "Customer", title: "Customer", editLink: true },
        { name: "Date", title: "Date", editLink: true },
        { name: "Employee", title: "Employee", editLink: true, editLinkItemType: "Northwind.Employee", editLinkIdField: "EmployeeID" },
        { name: "Freight", title: "Freight" }
    ] as any[];

    function setup(editLinks: "all" | "primary", grid?: DataGrid<any, any>) {
        grid ??= createGrid();
        vi.spyOn(grid as any, "getEditLinks").mockReturnValue(editLinks);
        const columns = grid["propertyItemsToColumns"](items());
        grid.sleekGrid.setColumns(columns);
        return grid;
    }

    /** The css class of the link that the column renders for a row, after the grid has the given visible columns. */
    function linkClass(grid: DataGrid<any, any>, id: string): string {
        const column = grid.allColumns.find(x => x.id === id);
        const link = column.format(formatterContext({
            column, item: { ID: 1, EmployeeID: 2, [id]: "x" }, value: "x"
        })) as HTMLElement;
        return link.className;
    }

    const quiet = (grid: DataGrid<any, any>, id: string) => linkClass(grid, id).split(" ").includes("s-EditLink-quiet");

    it("are all links by default", () => {
        const grid = setup("all");
        expect(["OrderID", "Customer", "Date", "Employee"].map(x => quiet(grid, x))).toEqual([false, false, false, false]);
        grid.destroy();
    });

    it("are links by default, as editLinks of the defaults is null", () => {
        const grid = createGrid();
        expect(dataGridDefaults.editLinks).toBeNull();
        expect(grid["getEditLinks"]()).toBe("all");
        try {
            dataGridDefaults.editLinks = "primary";
            expect(grid["getEditLinks"]()).toBe("primary");
        }
        finally {
            dataGridDefaults.editLinks = null;
            grid.destroy();
        }
    });

    it("with primary, are quiet except the first visible one to the record of the row and the ones to other records", () => {
        const grid = setup("primary");
        expect(quiet(grid, "OrderID")).toBe(false);
        expect(quiet(grid, "Customer")).toBe(true);
        expect(quiet(grid, "Date")).toBe(true);
        expect(quiet(grid, "Employee")).toBe(false);
        expect(linkClass(grid, "Customer")).toContain("s-EditLink");
        grid.destroy();
    });

    it("with primary, pass the primary to the next one when the first is hidden, and back when it is shown", () => {
        const grid = setup("primary");
        grid.sleekGrid.reorderColumns(["Customer", "Date", "Employee", "Freight"], { setVisible: ["Customer", "Date", "Employee", "Freight"] });
        expect(quiet(grid, "Customer")).toBe(false);
        expect(quiet(grid, "Date")).toBe(true);

        grid.sleekGrid.reorderColumns(["OrderID", "Customer", "Date", "Employee", "Freight"], { setVisible: ["OrderID", "Customer", "Date", "Employee", "Freight"] });
        expect(quiet(grid, "OrderID")).toBe(false);
        expect(quiet(grid, "Customer")).toBe(true);
        grid.destroy();
    });

    it("with primary, follow the order the columns are shown in", () => {
        const grid = setup("primary");
        grid.sleekGrid.reorderColumns(["Date", "OrderID", "Customer", "Employee", "Freight"]);
        expect(quiet(grid, "Date")).toBe(false);
        expect(quiet(grid, "OrderID")).toBe(true);
        grid.destroy();
    });

    it("with primary, keep the css class of the EditLink attribute", () => {
        const grid = createGrid();
        vi.spyOn(grid as any, "getEditLinks").mockReturnValue("primary");
        const columns = grid["propertyItemsToColumns"]([
            { name: "A", title: "A", editLink: true },
            { name: "B", title: "B", editLink: true, editLinkCssClass: "my-link" }
        ] as any[]);
        grid.sleekGrid.setColumns(columns);
        expect(linkClass(grid, "B").split(" ")).toEqual(expect.arrayContaining(["s-EditLink", "my-link", "s-EditLink-quiet"]));
        grid.destroy();
    });

    it("with primary, let a grid pick its primary column", () => {
        class PickGrid extends DataGrid<any, any> {
            protected override getPrimaryEditLinkColumn(columns: Column[]) {
                return columns.find(x => x.id === "Customer");
            }
        }
        const grid = setup("primary", new PickGrid({}));
        expect(quiet(grid, "OrderID")).toBe(true);
        expect(quiet(grid, "Customer")).toBe(false);
        grid.destroy();
    });
});

describe("DataGrid edit links set quiet by their attribute", () => {
    function setup(editLinks: "all" | "primary", items: any[]) {
        const grid = createGrid();
        vi.spyOn(grid as any, "getEditLinks").mockReturnValue(editLinks);
        grid.sleekGrid.setColumns(grid["propertyItemsToColumns"](items.map(x => ({ title: x.name, editLink: true, ...x }))));
        return grid;
    }

    const quiet = (grid: DataGrid<any, any>, id: string) => {
        const column = grid.allColumns.find(x => x.id === id);
        const link = column.format(formatterContext({ column, item: { ID: 1, EmployeeID: 2 }, value: "x" })) as HTMLElement;
        return link.classList.contains("s-EditLink-quiet");
    };

    it("are quiet or full links as it says, whatever the editLinks of the grid", () => {
        for (const mode of ["all", "primary"] as const) {
            const grid = setup(mode, [
                { name: "A", editLinkQuiet: true },
                { name: "B", editLinkQuiet: false },
                { name: "C", editLinkQuiet: false },
                { name: "Employee", editLinkItemType: "Northwind.Employee", editLinkIdField: "EmployeeID", editLinkQuiet: true }
            ]);
            expect([quiet(grid, "A"), quiet(grid, "B"), quiet(grid, "C"), quiet(grid, "Employee")]).toEqual([true, false, false, true]);
            grid.destroy();
        }
    });

    it("with primary, a quiet one is not the primary, so the next one is", () => {
        const grid = setup("primary", [{ name: "A", editLinkQuiet: true }, { name: "B" }, { name: "C" }]);
        expect([quiet(grid, "A"), quiet(grid, "B"), quiet(grid, "C")]).toEqual([true, false, true]);
        grid.destroy();
    });

    it("with primary, a full one is the primary when it comes first, so the ones after it are quiet", () => {
        const grid = setup("primary", [{ name: "A" }, { name: "B", editLinkQuiet: false }, { name: "C" }]);
        grid.sleekGrid.reorderColumns(["B", "A", "C"]);
        expect([quiet(grid, "B"), quiet(grid, "A"), quiet(grid, "C")]).toEqual([false, true, true]);

        // when it comes after one that the grid decides for, that one is the primary
        grid.sleekGrid.reorderColumns(["A", "B", "C"]);
        expect([quiet(grid, "A"), quiet(grid, "B"), quiet(grid, "C")]).toEqual([false, false, true]);
        grid.destroy();
    });
});
