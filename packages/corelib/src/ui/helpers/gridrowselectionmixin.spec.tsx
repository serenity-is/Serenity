import { beforeEach, describe, expect, it, vi } from "vitest";
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
        it("clears all selections and updates select all", () => {
            const mixin = new GridRowSelectionMixin(mockGrid);
            (mixin as any).include = { "1": true, "2": true };
            mixin.clear();
            expect((mixin as any).include).toEqual({});
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

    describe("createSelectColumn", () => {
        it("returns a column with format function", () => {
            const mixin = new GridRowSelectionMixin(mockGrid);
            const getMixin = vi.fn(() => mixin);
            const column = GridRowSelectionMixin.createSelectColumn(getMixin);

            expect(column.id).toBe("__select__");
            expect(column.sortable).toBe(false);
            expect(column.headerCssClass).toBe("select-all-header");
            // the group cell and the indentation of the data rows start after this column
            expect(column.skipGroupCell).toBe(true);
        });

        it("format returns empty string when mixin is null", () => {
            const getMixin = vi.fn(() => null);
            const column = GridRowSelectionMixin.createSelectColumn(getMixin);
            const result = column.format({ item: { id: 1 } } as any);
            expect(result).toBe("");
        });

        it("format returns empty string for the rows of a group", () => {
            const mixin = new GridRowSelectionMixin(mockGrid);
            const getMixin = vi.fn(() => mixin);
            const column = GridRowSelectionMixin.createSelectColumn(getMixin);
            const result = column.format({ item: { __nonDataRow: true } } as any);
            expect(result).toBe("");
        });

        it("format returns empty string when item is not selectable", () => {
            const mixin = new GridRowSelectionMixin(mockGrid, { selectable: () => false });
            const getMixin = vi.fn(() => mixin);
            const column = GridRowSelectionMixin.createSelectColumn(getMixin);
            const result = column.format({ item: { id: 1 } } as any);
            expect(result).toBe("");
        });

        it("format returns checked span when item is selected", () => {
            const mixin = new GridRowSelectionMixin(mockGrid);
            (mixin as any).include = { "1": true };
            const getMixin = vi.fn(() => mixin);
            const column = GridRowSelectionMixin.createSelectColumn(getMixin);
            const result = column.format({ item: { id: 1 } } as any);
            expect(result).toBeTruthy();
            expect((result as any).classList.contains('checked')).toBe(true);
        });

        it("format returns unchecked span when item is not selected", () => {
            const mixin = new GridRowSelectionMixin(mockGrid);
            const getMixin = vi.fn(() => mixin);
            const column = GridRowSelectionMixin.createSelectColumn(getMixin);
            const result = column.format({ item: { id: 2 } } as any);
            expect(result).toBeTruthy();
            expect((result as any).classList.contains('checked')).toBe(false);
        });

        it("format does not treat prototype property names as checked", () => {
            const mixin = new GridRowSelectionMixin(mockGrid);
            const getMixin = vi.fn(() => mixin);
            const column = GridRowSelectionMixin.createSelectColumn(getMixin);
            const result = column.format({ item: { id: "constructor" } } as any);
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

            expect(format({ item: group }).className).toContain("check-box");
            expect(format({ item: group }).className).not.toContain("checked");
            expect(format({ item: group }).className).not.toContain("partial");

            (mixin as any).include = { "1": true };
            expect(format({ item: group }).className).toContain("partial");

            (mixin as any).include = { "1": true, "2": true };
            expect(format({ item: group }).className).toContain("checked");
        });

        it("hides the checkbox of the groups when selectGroups is false", () => {
            const group = makeGroup("A", [{ id: 1 }]);
            const mixin = new GridRowSelectionMixin(mockGrid, { selectGroups: false });
            const column = GridRowSelectionMixin.createSelectColumn(() => mixin);

            expect((column.format as any)({ item: group })).toBe("");
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

            gridObj.getColumnIndex.mockReturnValue(-1);
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
