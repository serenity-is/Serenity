import { beforeEach, describe, expect, it, vi } from "vitest";
import { GridRadioSelectionMixin } from "./gridradioselectionmixin";
import { GridRowSelectionMixin } from "./gridrowselectionmixin";

describe("GridRowSelectionMixin", () => {
    let mockGrid: any;
    let gridObj: any;
    let viewObj: any;
    let onClickHandler: Function;
    let onHeaderClickHandler: Function;
    let onKeyDownHandler: Function;

    beforeEach(() => {
        onClickHandler = null as any;
        onHeaderClickHandler = null as any;
        onKeyDownHandler = null as any;
        gridObj = {
            onClick: { subscribe: vi.fn((h: any) => { onClickHandler = h; }), unsubscribe: vi.fn() },
            onHeaderClick: { subscribe: vi.fn((h: any) => { onHeaderClickHandler = h; }), unsubscribe: vi.fn() },
            onKeyDown: { subscribe: vi.fn((h: any) => { onKeyDownHandler = h; }), unsubscribe: vi.fn() },
            getActiveCell: vi.fn(() => null),
            getColumnIndex: vi.fn(() => 0),
            getColumns: vi.fn(() => [{ id: GridRowSelectionMixin.selectColumnId, format: GridRowSelectionMixin.formatSelectItem }]),
            invalidate: vi.fn(),
            updateRow: vi.fn()
        };
        viewObj = {
            getIdPropertyName: vi.fn(() => "id"),
            getItem: vi.fn(),
            getLength: vi.fn(() => 0),
            getItems: vi.fn(() => []),
            setItems: vi.fn(),
            populate: vi.fn(),
            onRowsChanged: { subscribe: vi.fn(), unsubscribe: vi.fn() }
        };
        mockGrid = {
            getGrid: vi.fn(() => gridObj),
            getView: vi.fn(() => viewObj),
            getElement: vi.fn(() => document.createElement("div"))
        };
    });

    describe("constructor", () => {
        it("subscribes to grid events", () => {
            const mixin = new GridRowSelectionMixin(mockGrid);
            expect(mockGrid.getGrid().onClick.subscribe).toHaveBeenCalled();
            expect(mockGrid.getGrid().onHeaderClick.subscribe).toHaveBeenCalled();
        });

        it("subscribes to onRowsChanged when available", () => {
            const mixin = new GridRowSelectionMixin(mockGrid);
            expect(viewObj.onRowsChanged.subscribe).toHaveBeenCalled();
        });
    });

    describe("tryGetInstance", () => {
        it("finds the mixin of a grid, and forgets it when it is destroyed", () => {
            const mixin = new GridRowSelectionMixin(mockGrid);

            expect(GridRowSelectionMixin.tryGetInstance(mockGrid)).toBe(mixin);
            expect(GridRowSelectionMixin.tryGetInstance(gridObj)).toBe(mixin);

            mixin.destroy();

            expect(GridRowSelectionMixin.tryGetInstance(mockGrid)).toBeNull();
            expect(GridRowSelectionMixin.tryGetInstance(gridObj)).toBeNull();
        });

        it("returns null for a grid without a mixin", () => {
            expect(GridRowSelectionMixin.tryGetInstance(mockGrid)).toBeNull();
        });

        it("replaces the mixin a page created, instead of leaving its handlers on the grid", () => {
            // a page may create its own mixin although the grid makes one for the select column, and the
            // handlers of the one before it would toggle a check box twice
            const pageMixin = new GridRowSelectionMixin(mockGrid);
            const subscribeCount = gridObj.onClick.subscribe.mock.calls.length;

            const gridMixin = new GridRowSelectionMixin(mockGrid);

            expect(GridRowSelectionMixin.tryGetInstance(mockGrid)).toBe(gridMixin);
            // the handlers of the one before it are off the grid, and the includes of it are gone
            expect(gridObj.onClick.unsubscribe).toHaveBeenCalledWith(
                gridObj.onClick.subscribe.mock.calls[subscribeCount - 1][0]);
            expect((pageMixin as any).grid).toBeUndefined();

            gridMixin.destroy();
        });

        it("can be destroyed twice, as a page may destroy the mixin that a newer one replaced", () => {
            const pageMixin = new GridRowSelectionMixin(mockGrid);
            const gridMixin = new GridRowSelectionMixin(mockGrid);

            expect(() => pageMixin.destroy()).not.toThrow();
            expect(() => pageMixin.destroy()).not.toThrow();
            // the mixin that is in use is still there
            expect(GridRowSelectionMixin.tryGetInstance(mockGrid)).toBe(gridMixin);

            gridMixin.destroy();
        });

        it("keeps the subscribers of the mixin it replaces, as the rows are the same ones", () => {
            const pageMixin = new GridRowSelectionMixin(mockGrid);
            const changed = vi.fn();
            pageMixin.onSelectionChanged.subscribe(changed);
            const notified = pageMixin.onSelectionChanged;

            // the new mixin tells the subscribers of the one it replaces, and the one that goes is given
            // an empty one, so that its destroy clears nothing that is still in use
            const gridMixin = new GridRowSelectionMixin(mockGrid);
            gridMixin.selectKeys(["1"]);

            expect(changed).toHaveBeenCalledWith(expect.objectContaining({ count: 1 }));
            expect(gridMixin.onSelectionChanged).toBe(notified);
            expect(pageMixin.onSelectionChanged).not.toBe(notified);

            gridMixin.destroy();
        });
    });

    describe("the state of the checked rows", () => {
        it("tells the keys, their number, and how many of them the page has", () => {
            viewObj.getLength = vi.fn(() => 3);
            viewObj.getItem = vi.fn((row: number) => [{ id: 1 }, { id: 2 }, { id: 3 }][row]);
            const mixin = new GridRowSelectionMixin(mockGrid);

            mixin.selectKeys(["1", "2", "3", "9"]);

            expect(mixin.getSelectionState()).toEqual({
                keys: ["1", "2", "3", "9"],
                count: 4,
                onPage: 3,
                selectableOnPage: 3
            });
        });

        it("keeps the rows of a group or of the totals out of the page counts", () => {
            viewObj.getLength = vi.fn(() => 3);
            viewObj.getItem = vi.fn((row: number) => [
                { id: 1 },
                { __group: true, __nonDataRow: true, key: "g" },
                { __groupTotals: true, __nonDataRow: true, id: 2 }
            ][row]);
            const mixin = new GridRowSelectionMixin(mockGrid);

            mixin.selectKeys(["1", "2"]);

            expect(mixin.getSelectionState()).toMatchObject({ count: 2, onPage: 1, selectableOnPage: 1 });
        });

        it("is notified to the subscribers after a change", () => {
            viewObj.getLength = vi.fn(() => 1);
            viewObj.getItem = vi.fn(() => ({ id: 1 }));
            const mixin = new GridRowSelectionMixin(mockGrid);
            const changed = vi.fn();
            mixin.onSelectionChanged.subscribe(changed);

            mixin.selectKeys(["1"]);
            expect(changed).toHaveBeenLastCalledWith(expect.objectContaining({ count: 1, onPage: 1 }));

            mixin.clear();
            expect(changed).toHaveBeenLastCalledWith(expect.objectContaining({ count: 0, onPage: 0 }));

            mixin.destroy();
            mixin.updateSelectAll();
            expect(changed).toHaveBeenCalledTimes(2);
        });
    });

    describe("the selected rows", () => {
        it("adds the checked-row class through the metadata of the view, keeping the classes it gives", () => {
            let callback: any;
            viewObj.getItemMetadataCallback = vi.fn(() => (item: any) => ({ cssClasses: "noted" }));
            viewObj.setItemMetadataCallback = vi.fn((value: any) => { callback = value; });
            const mixin = new GridRowSelectionMixin(mockGrid);

            mixin.selectKeys(["42"]);

            expect(callback({ id: 42 }, 0)).toEqual({ cssClasses: "noted checked-row" });
            expect(callback({ id: 7 }, 1)).toEqual({ cssClasses: "noted" });
        });

        it("uses the checked-row class alone when the view has no metadata of its own", () => {
            let callback: any;
            viewObj.setItemMetadataCallback = vi.fn((value: any) => { callback = value; });
            const mixin = new GridRowSelectionMixin(mockGrid);

            mixin.selectKeys(["42"]);

            expect(callback({ id: 42 }, 0)).toEqual({ cssClasses: GridRowSelectionMixin.checkedRowClass });
        });

        it("tells the rows of a group or of the totals apart, which have no id to select", () => {
            let callback: any;
            viewObj.setItemMetadataCallback = vi.fn((value: any) => { callback = value; });
            const mixin = new GridRowSelectionMixin(mockGrid);

            mixin.selectKeys(["42"]);

            expect(callback({ __group: true, key: "a" }, 0)).toBeUndefined();
            expect(callback({ __groupTotals: true, id: 42 }, 1)).toEqual({ cssClasses: GridRowSelectionMixin.checkedRowClass });
        });

        it("gives the view its own metadata back when the mixin is destroyed", () => {
            const previous = vi.fn();
            viewObj.getItemMetadataCallback = vi.fn(() => previous);
            viewObj.setItemMetadataCallback = vi.fn();
            const mixin = new GridRowSelectionMixin(mockGrid);

            mixin.destroy();

            expect(viewObj.setItemMetadataCallback).toHaveBeenLastCalledWith(previous);
        });
    });

    describe("destroy", () => {
        it("unsubscribes events and cleans up", () => {
            const mixin = new GridRowSelectionMixin(mockGrid);
            mixin.destroy();
            expect(mockGrid.getGrid().onClick.unsubscribe).toHaveBeenCalled();
            expect(mockGrid.getGrid().onHeaderClick.unsubscribe).toHaveBeenCalled();
            expect((mixin as any).grid).toBeUndefined();
        });

        it("handles missing onRowsChanged gracefully", () => {
            const mixin = new GridRowSelectionMixin(mockGrid);
            (mixin as any).grid = {
                getGrid: () => gridObj,
                getView: () => ({ onRowsChanged: null })
            };
            expect(() => mixin.destroy()).not.toThrow();
        });
    });

    describe("handleGridClick (onClick handler)", () => {
        it("selects item when clicking select-item", () => {
            viewObj.getItem = vi.fn(() => ({ id: 42 }));
            viewObj.getLength = vi.fn(() => 2);
            const mixin = new GridRowSelectionMixin(mockGrid);

            const e = {
                target: { classList: { contains: (c: string) => c === 'select-item' } },
                preventDefault: vi.fn()
            } as any;
            onClickHandler(e, { row: 0 });

            expect(e.preventDefault).toHaveBeenCalled();
            expect((mixin as any).include["42"]).toBe(true);
            expect(gridObj.invalidate).toHaveBeenCalled();
        });

        it("deselects item when clicking already selected item", () => {
            viewObj.getItem = vi.fn(() => ({ id: 42 }));
            viewObj.getLength = vi.fn(() => 1);
            const mixin = new GridRowSelectionMixin(mockGrid);
            (mixin as any).include = { "42": true };

            const e = {
                target: { classList: { contains: (c: string) => c === 'select-item' } },
                preventDefault: vi.fn()
            } as any;
            onClickHandler(e, { row: 0 });

            expect('42' in (mixin as any).include).toBe(false);
        });

        it("ignores the click on the select cell of a group row", () => {
            // the group cell starts after the select column, so group rows render that cell too
            viewObj.getItem = vi.fn(() => ({ __nonDataRow: true } as any));
            const mixin = new GridRowSelectionMixin(mockGrid);

            const e = {
                target: { classList: { contains: (c: string) => c === 'select-item' } },
                preventDefault: vi.fn()
            } as any;

            expect(() => onClickHandler(e, { row: 0 })).not.toThrow();
            expect((mixin as any).include).toEqual({});
            expect(gridObj.invalidate).not.toHaveBeenCalled();
        });

        it("does nothing when clicking non-select-item", () => {
            const mixin = new GridRowSelectionMixin(mockGrid);
            const e = {
                target: { classList: { contains: (c: string) => false } },
                preventDefault: vi.fn()
            } as any;
            onClickHandler(e, { row: 0 });

            expect(e.preventDefault).not.toHaveBeenCalled();
            expect(gridObj.updateRow).not.toHaveBeenCalled();
        });

        it("treats prototype property names as ordinary keys", () => {
            viewObj.getItem = vi.fn(() => ({ id: "constructor" }));
            viewObj.getLength = vi.fn(() => 1);
            const mixin = new GridRowSelectionMixin(mockGrid);

            const e = {
                target: { classList: { contains: (c: string) => c === 'select-item' } },
                preventDefault: vi.fn()
            } as any;
            onClickHandler(e, { row: 0 });

            expect((mixin as any).include["constructor"]).toBe(true);
        });
    });

    describe("handleHeaderClick (onHeaderClick handler)", () => {
        it("selects all items when header clicked and none selected", () => {
            const items = [{ id: 1 }, { id: 2 }];
            viewObj.getItems = vi.fn(() => items);
            const mixin = new GridRowSelectionMixin(mockGrid);

            const e = {
                target: { classList: { contains: (c: string) => c === 'select-all-items' } },
                preventDefault: vi.fn()
            } as any;
            onHeaderClickHandler(e);

            expect(e.preventDefault).toHaveBeenCalled();
            expect((mixin as any).include["1"]).toBe(true);
            expect((mixin as any).include["2"]).toBe(true);
        });

        it("deselects all when header clicked and some selected", () => {
            const items = [{ id: 1 }, { id: 2 }];
            viewObj.getItems = vi.fn(() => items);
            const mixin = new GridRowSelectionMixin(mockGrid);
            (mixin as any).include = { "1": true };

            const e = {
                target: { classList: { contains: (c: string) => c === 'select-all-items' } },
                preventDefault: vi.fn()
            } as any;
            onHeaderClickHandler(e);

            expect(Object.keys((mixin as any).include).length).toBe(0);
        });

        it("does nothing when default is prevented", () => {
            const mixin = new GridRowSelectionMixin(mockGrid);
            const e = {
                target: { classList: { contains: (c: string) => true } },
                preventDefault: vi.fn(),
                defaultPrevented: true
            } as any;
            onHeaderClickHandler(e);

            expect(e.preventDefault).not.toHaveBeenCalled();
        });

        it("does nothing when clicking non-select-all-items", () => {
            const mixin = new GridRowSelectionMixin(mockGrid);
            const e = {
                target: { classList: { contains: (c: string) => false } },
                preventDefault: vi.fn()
            } as any;
            onHeaderClickHandler(e);

            expect(e.preventDefault).not.toHaveBeenCalled();
        });
    });

    describe("updateSelectAll", () => {
        it("toggles checked class when all items selectable", () => {
            const element = document.createElement("div");
            const headerDiv = document.createElement("div");
            headerDiv.className = "select-all-header";
            const nameDiv = document.createElement("div");
            nameDiv.className = "slick-column-name";
            const button = document.createElement("a");
            button.className = "select-all-items";
            nameDiv.appendChild(button);
            headerDiv.appendChild(nameDiv);
            element.appendChild(headerDiv);

            mockGrid.getElement = vi.fn(() => element);
            mockGrid.getView = vi.fn(() => ({
                getIdPropertyName: vi.fn(() => "id"),
                getItems: vi.fn(() => [{ id: 1 }, { id: 2 }])
            }));

            const mixin = new GridRowSelectionMixin(mockGrid);
            (mixin as any).include = { "1": true, "2": true };

            mixin.updateSelectAll();
            expect(button.classList.contains("checked")).toBe(true);
        });

        it("removes checked class when not all items selected", () => {
            const element = document.createElement("div");
            const headerDiv = document.createElement("div");
            headerDiv.className = "select-all-header";
            const nameDiv = document.createElement("div");
            nameDiv.className = "slick-column-name";
            const button = document.createElement("a");
            button.className = "select-all-items";
            button.classList.add("checked");
            nameDiv.appendChild(button);
            headerDiv.appendChild(nameDiv);
            element.appendChild(headerDiv);

            mockGrid.getElement = vi.fn(() => element);
            mockGrid.getView = vi.fn(() => ({
                getIdPropertyName: vi.fn(() => "id"),
                getItems: vi.fn(() => [{ id: 1 }, { id: 2 }])
            }));

            const mixin = new GridRowSelectionMixin(mockGrid);
            (mixin as any).include = { "1": true };

            mixin.updateSelectAll();
            expect(button.classList.contains("checked")).toBe(false);
        });

        it("does nothing if no select-all button", () => {
            const mixin = new GridRowSelectionMixin(mockGrid);
            expect(() => mixin.updateSelectAll()).not.toThrow();
        });
    });

    describe("clear", () => {
        it("clears all selections and repaints the rows", () => {
            const mixin = new GridRowSelectionMixin(mockGrid);
            (mixin as any).include = { "1": true, "2": true };
            gridObj.invalidate.mockClear();
            mixin.clear();

            expect((mixin as any).include).toEqual({});
            // the rows have to be painted again, or they would keep their check boxes and their tint
            expect(gridObj.invalidate).toHaveBeenCalled();
        });

        it("does nothing after it is destroyed", () => {
            const mixin = new GridRowSelectionMixin(mockGrid);
            (mixin as any).include = { "1": true };
            mixin.destroy();
            gridObj.invalidate.mockClear();

            expect(() => mixin.clear()).not.toThrow();
            expect(gridObj.invalidate).not.toHaveBeenCalled();
        });
    });

    describe("resetCheckedAndRefresh", () => {
        it("resets include and repopulates view", () => {
            const mixin = new GridRowSelectionMixin(mockGrid);
            (mixin as any).include = { "1": true };
            mixin.resetCheckedAndRefresh();
            expect((mixin as any).include).toEqual({});
            expect(viewObj.populate).toHaveBeenCalled();
        });
    });

    describe("selectKeys", () => {
        it("selects multiple keys", () => {
            const mixin = new GridRowSelectionMixin(mockGrid);
            mixin.selectKeys(["1", "2", "3"]);
            expect((mixin as any).include).toEqual({ "1": true, "2": true, "3": true });
        });
    });

    describe("getSelectedKeys", () => {
        it("returns all selected keys", () => {
            const mixin = new GridRowSelectionMixin(mockGrid);
            (mixin as any).include = { "1": true, "2": true };
            expect(mixin.getSelectedKeys()).toEqual(["1", "2"]);
        });

        it("returns empty array when nothing selected", () => {
            const mixin = new GridRowSelectionMixin(mockGrid);
            expect(mixin.getSelectedKeys()).toEqual([]);
        });
    });

    describe("getSelectedAsInt32", () => {
        it("returns selected keys as integers", () => {
            const mixin = new GridRowSelectionMixin(mockGrid);
            (mixin as any).include = { "42": true, "100": true };
            expect(mixin.getSelectedAsInt32()).toEqual([42, 100]);
        });

        it("returns empty array when nothing selected", () => {
            const mixin = new GridRowSelectionMixin(mockGrid);
            expect(mixin.getSelectedAsInt32()).toEqual([]);
        });
    });

    describe("getSelectedAsInt64", () => {
        it("returns selected keys as integers", () => {
            const mixin = new GridRowSelectionMixin(mockGrid);
            (mixin as any).include = { "42": true };
            expect(mixin.getSelectedAsInt64()).toEqual([42]);
        });
    });

    describe("setSelectedKeys", () => {
        it("clears and sets multiple keys", () => {
            const mixin = new GridRowSelectionMixin(mockGrid);
            mixin.setSelectedKeys(["a", "b"]);
            expect((mixin as any).include).toEqual({ "a": true, "b": true });
        });
    });

    describe("the column of the check boxes", () => {
        it("is told apart from the others by its format, not by its id", () => {
            const column = GridRowSelectionMixin.createSelectColumn();

            expect(GridRowSelectionMixin.isSelectColumn(column)).toBe(true);
            expect(column.id).toBe("__select__");
            expect(column.format).toBe(GridRowSelectionMixin.formatSelectItem);

            // the id alone is not enough, e.g. for a column that a page adds with the same id and no format
            expect(GridRowSelectionMixin.isSelectColumn({ id: "__select__", field: "__select__" })).toBe(false);
            expect(GridRowSelectionMixin.isSelectColumn({ id: "Name", field: "Name", format: () => "" })).toBe(false);
            expect(GridRowSelectionMixin.isSelectColumn(null)).toBe(false);
        });

        it("is also found in a grid that added it before, without the format of the mixin", () => {
            // the columns that the pages add themselves have the header of the select all check box
            expect(GridRowSelectionMixin.isSelectColumn({
                id: "__select__", field: "__select__", headerCssClass: "select-all-header"
            })).toBe(true);

            // the id alone is not enough, e.g. for a column that a page adds with the same id and no header
            expect(GridRowSelectionMixin.isSelectColumn({
                id: "__select__", field: "__select__", headerCssClass: "some-header"
            })).toBe(false);
        });

        it("is not the column of the radio buttons, which has an id of its own", () => {
            expect(GridRowSelectionMixin.isSelectColumn(
                GridRadioSelectionMixin.createSelectColumn(() => null))).toBe(false);
        });
    });

    describe("createSelectColumn", () => {
        it("returns a column with format function", () => {
            const mixin = new GridRowSelectionMixin(mockGrid);
            const getMixin = vi.fn(() => mixin);
            const column = GridRowSelectionMixin.createSelectColumn(getMixin);

            expect(column.id).toBe(GridRowSelectionMixin.selectColumnId);
            expect(column.sortable).toBe(false);
            expect(column.headerCssClass).toBe("select-all-header");
            // the group cell and the indentation of the data rows start after this column
            expect(column.skipGroupCell).toBe(true);
        });

        it("finds the mixin of the grid that is rendered, when it is not given a getter", () => {
            const mixin = new GridRowSelectionMixin(mockGrid);
            mixin.selectKeys(["1"]);
            const column = GridRowSelectionMixin.createSelectColumn({ frozen: "start" });

            expect(column.frozen).toBe("start");

            const checked = column.format({ item: { id: 1 }, grid: gridObj } as any) as HTMLElement;
            expect(checked.className).toContain("select-item");
            expect(checked.className).toContain("checked");

            const unchecked = column.format({ item: { id: 2 }, grid: gridObj } as any) as HTMLElement;
            expect(unchecked.className).toContain("select-item");
            expect(unchecked.className).not.toContain("checked");

            // a grid without a mixin shows no checkbox
            expect(column.format({ item: { id: 1 }, grid: {} } as any)).toBe("");
        });

        it("format returns empty string when mixin is null", () => {
            const getMixin = vi.fn(() => null);
            const column = GridRowSelectionMixin.createSelectColumn(getMixin);
            const result = column.format({ item: { id: 1 }, grid: gridObj } as any);
            expect(result).toBe("");
        });

        it("format returns empty string for the rows of a group", () => {
            const mixin = new GridRowSelectionMixin(mockGrid);
            const getMixin = vi.fn(() => mixin);
            const column = GridRowSelectionMixin.createSelectColumn(getMixin);
            const result = column.format({ item: { __nonDataRow: true }, grid: gridObj } as any);
            expect(result).toBe("");
        });

        it("format returns empty string when item is not selectable", () => {
            const mixin = new GridRowSelectionMixin(mockGrid, { selectable: () => false });
            const getMixin = vi.fn(() => mixin);
            const column = GridRowSelectionMixin.createSelectColumn(getMixin);
            const result = column.format({ item: { id: 1 }, grid: gridObj } as any);
            expect(result).toBe("");
        });

        it("format returns checked span when item is selected", () => {
            const mixin = new GridRowSelectionMixin(mockGrid);
            (mixin as any).include = { "1": true };
            const getMixin = vi.fn(() => mixin);
            const column = GridRowSelectionMixin.createSelectColumn(getMixin);
            const result = column.format({ item: { id: 1 }, grid: gridObj } as any);
            expect(result).toBeTruthy();
            expect((result as any).classList.contains('checked')).toBe(true);
        });

        it("format returns unchecked span when item is not selected", () => {
            const mixin = new GridRowSelectionMixin(mockGrid);
            const getMixin = vi.fn(() => mixin);
            const column = GridRowSelectionMixin.createSelectColumn(getMixin);
            const result = column.format({ item: { id: 2 }, grid: gridObj } as any);
            expect(result).toBeTruthy();
            expect((result as any).classList.contains('checked')).toBe(false);
        });

        it("format does not treat prototype property names as checked", () => {
            const mixin = new GridRowSelectionMixin(mockGrid);
            const getMixin = vi.fn(() => mixin);
            const column = GridRowSelectionMixin.createSelectColumn(getMixin);
            const result = column.format({ item: { id: "constructor" }, grid: gridObj } as any);
            expect(result).toBeTruthy();
            expect((result as any).classList.contains('checked')).toBe(false);
        });
    });

    describe("group row selection", () => {
        function makeGroup(value: string, rows: any[], groups: any[] = [], count = rows.length) {
            return { __group: true, __nonDataRow: true, value, rows, groups, count, level: 0 } as any;
        }

        function selectItemClick() {
            return {
                target: { classList: { contains: (c: string) => c === 'select-item' } },
                preventDefault: vi.fn()
            } as any;
        }

        it("selects all the rows of a group and then deselects them", () => {
            const group = makeGroup("A", [{ id: 1 }, { id: 2 }]);
            viewObj.getItem = vi.fn(() => group);
            const mixin = new GridRowSelectionMixin(mockGrid);
            const e = selectItemClick();

            onClickHandler(e, { row: 0 });
            expect((mixin as any).include).toEqual({ "1": true, "2": true });
            expect(gridObj.invalidate).toHaveBeenCalled();

            onClickHandler(e, { row: 0 });
            expect((mixin as any).include).toEqual({});
        });

        it("selects the rows of the subgroups, without repeating them", () => {
            const child1 = makeGroup("A/1", [{ id: 1 }]);
            const child2 = makeGroup("A/2", [{ id: 2 }, { id: 3 }]);
            // the rows of a group also include the rows of its subgroups, as the view builds them
            const group = makeGroup("A", [{ id: 1 }, { id: 2 }, { id: 3 }], [child1, child2], 3);
            viewObj.getItem = vi.fn(() => group);
            const mixin = new GridRowSelectionMixin(mockGrid);

            onClickHandler(selectItemClick(), { row: 0 });

            expect((mixin as any).include).toEqual({ "1": true, "2": true, "3": true });
        });

        it("does not select the rows that can't be selected", () => {
            const group = makeGroup("A", [{ id: 1, locked: true }, { id: 2 }]);
            viewObj.getItem = vi.fn(() => group);
            const mixin = new GridRowSelectionMixin(mockGrid, { selectable: (x: any) => !x.locked });

            onClickHandler(selectItemClick(), { row: 0 });

            expect((mixin as any).include).toEqual({ "2": true });
        });

        it("does nothing when the rows of the group are not known", () => {
            // a virtual scroll view knows the number of the rows of a group, but not the rows
            const group = makeGroup("A", [], [], 4000);
            viewObj.getItem = vi.fn(() => group);
            const mixin = new GridRowSelectionMixin(mockGrid);

            onClickHandler(selectItemClick(), { row: 0 });

            expect((mixin as any).include).toEqual({});
            expect(gridObj.invalidate).not.toHaveBeenCalled();
        });

        it("renders the state of the group selection in the select column", () => {
            const group = makeGroup("A", [{ id: 1 }, { id: 2 }]);
            const mixin = new GridRowSelectionMixin(mockGrid, { selectGroups: true });
            const column = GridRowSelectionMixin.createSelectColumn(() => mixin);
            const format = column.format as any;

            expect(format({ item: group, grid: gridObj }).className).toContain("check-box");
            expect(format({ item: group, grid: gridObj }).className).not.toContain("checked");
            expect(format({ item: group, grid: gridObj }).className).not.toContain("partial");

            (mixin as any).include = { "1": true };
            expect(format({ item: group, grid: gridObj }).className).toContain("partial");

            (mixin as any).include = { "1": true, "2": true };
            expect(format({ item: group, grid: gridObj }).className).toContain("checked");
        });

        it("hides the checkbox of the groups when selectGroups is false", () => {
            const group = makeGroup("A", [{ id: 1 }]);
            const mixin = new GridRowSelectionMixin(mockGrid, { selectGroups: false });
            const column = GridRowSelectionMixin.createSelectColumn(() => mixin);

            expect((column.format as any)({ item: group })).toBe("");
        });

        it("selects the rows of a group that the view has, when the group has more of them", () => {
            // the view has two of the ten rows of the group, as it pages them, and the checkbox is about
            // the ones that are there, like the select all of the header is about the rows of the page
            const group = makeGroup("A", [{ id: 1 }, { id: 2 }], [], 10);
            viewObj.getItem = vi.fn(() => group);
            const mixin = new GridRowSelectionMixin(mockGrid);
            const column = GridRowSelectionMixin.createSelectColumn(() => mixin);
            const format = column.format as any;

            expect(format({ item: group, grid: gridObj }).className).toContain("check-box");

            onClickHandler(selectItemClick(), { row: 0 });
            expect((mixin as any).include).toEqual({ "1": true, "2": true });
            expect(format({ item: group, grid: gridObj }).className).toContain("checked");
        });

        it("hides the checkbox of a group whose rows are not known", () => {
            const group = makeGroup("A", [], [], 100);
            const mixin = new GridRowSelectionMixin(mockGrid);
            const column = GridRowSelectionMixin.createSelectColumn(() => mixin);

            expect((column.format as any)({ item: group })).toBe("");
        });
    });

    describe("keyboard selection", () => {
        function keyEvent(key: string) {
            return { key, preventDefault: vi.fn(), stopImmediatePropagation: vi.fn() } as any;
        }

        it("toggles the checkbox of the active row on space", () => {
            viewObj.getItem = vi.fn(() => ({ id: 42 }));
            const mixin = new GridRowSelectionMixin(mockGrid);
            gridObj.getActiveCell.mockReturnValue({ row: 0, cell: 0 });

            const e = keyEvent(" ");
            onKeyDownHandler(e);

            expect((mixin as any).include).toEqual({ "42": true });
            expect(e.preventDefault).toHaveBeenCalled();
            expect(gridObj.invalidate).toHaveBeenCalled();

            onKeyDownHandler(keyEvent(" "));
            expect((mixin as any).include).toEqual({});
        });

        it("ignores the space key when the active cell is not the select column", () => {
            viewObj.getItem = vi.fn(() => ({ id: 42 }));
            new GridRowSelectionMixin(mockGrid);
            gridObj.getActiveCell.mockReturnValue({ row: 0, cell: 3 });

            const e = keyEvent(" ");
            onKeyDownHandler(e);

            expect(e.preventDefault).not.toHaveBeenCalled();
            expect(gridObj.invalidate).not.toHaveBeenCalled();
        });

        it("ignores keys other than space, and grids without a select column", () => {
            viewObj.getItem = vi.fn(() => ({ id: 42 }));
            new GridRowSelectionMixin(mockGrid);
            gridObj.getActiveCell.mockReturnValue({ row: 0, cell: 0 });

            const enter = keyEvent("Enter");
            onKeyDownHandler(enter);
            expect(enter.preventDefault).not.toHaveBeenCalled();

            gridObj.getColumns.mockReturnValue([]);
            const space = keyEvent(" ");
            onKeyDownHandler(space);
            expect(space.preventDefault).not.toHaveBeenCalled();
        });

        it("toggles all the rows of a group on space", () => {
            const group = { __group: true, __nonDataRow: true, rows: [{ id: 1 }, { id: 2 }], count: 2 } as any;
            viewObj.getItem = vi.fn(() => group);
            const mixin = new GridRowSelectionMixin(mockGrid);
            gridObj.getActiveCell.mockReturnValue({ row: 0, cell: 0 });

            onKeyDownHandler(keyEvent(" "));

            expect((mixin as any).include).toEqual({ "1": true, "2": true });
        });
    });
});
