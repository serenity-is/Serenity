import { beforeEach, describe, expect, it, vi } from "vitest";
import { GridRowSelectionMixin } from "./gridrowselectionmixin";

describe("GridRowSelectionMixin", () => {
    let mockGrid: any;
    let element: HTMLElement;
    let gridObj: any;
    let viewObj: any;
    let onClickHandler: Function;
    let onHeaderClickHandler: Function;

    beforeEach(() => {
        onClickHandler = null as any;
        onHeaderClickHandler = null as any;
        element = document.createElement("div");
        gridObj = {
            onClick: { subscribe: vi.fn((h: any) => { onClickHandler = h; }), unsubscribe: vi.fn() },
            onHeaderClick: { subscribe: vi.fn((h: any) => { onHeaderClickHandler = h; }), unsubscribe: vi.fn() },
            invalidateRows: vi.fn(),
            invalidateAllRows: vi.fn(),
            render: vi.fn(),
            getCellNode: vi.fn()
        };
        viewObj = {
            getIdPropertyName: vi.fn(() => "id"),
            getItem: vi.fn(),
            getLength: vi.fn(() => 0),
            getItems: vi.fn(() => []),
            setItems: vi.fn(),
            populate: vi.fn(),
            getItemMetadataCallback: vi.fn(),
            setItemMetadataCallback: vi.fn(),
            onRowsChanged: { subscribe: vi.fn(), unsubscribe: vi.fn() }
        };
        mockGrid = {
            getGrid: vi.fn(() => gridObj),
            getView: vi.fn(() => viewObj),
            getElement: vi.fn(() => element)
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

        it("restores the original item metadata callback", () => {
            const original = vi.fn();
            viewObj.getItemMetadataCallback = vi.fn(() => original);
            const mixin = new GridRowSelectionMixin(mockGrid);
            mixin.destroy();
            expect(viewObj.setItemMetadataCallback).toHaveBeenLastCalledWith(original);
        });

        it("handles missing onRowsChanged gracefully", () => {
            const mixin = new GridRowSelectionMixin(mockGrid);
            (mixin as any).grid = {
                getElement: () => element,
                getGrid: () => gridObj,
                getView: () => ({ onRowsChanged: null, setItemMetadataCallback: vi.fn() })
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
            expect(gridObj.invalidateRows).toHaveBeenCalled();
            expect(gridObj.render).toHaveBeenCalled();
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

        it("does nothing when clicking non-select-item", () => {
            const mixin = new GridRowSelectionMixin(mockGrid);
            const e = {
                target: { classList: { contains: (c: string) => false } },
                preventDefault: vi.fn()
            } as any;
            onClickHandler(e, { row: 0 });

            expect(e.preventDefault).not.toHaveBeenCalled();
            expect(gridObj.render).not.toHaveBeenCalled();
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
                ...viewObj,
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
                ...viewObj,
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

    describe("range selection", () => {
        const items = [{ id: 1 }, { id: 2 }, { id: 3 }, { id: 4 }, { id: 5 }];

        function click(row: number, shiftKey = false) {
            const target = document.createElement("input");
            target.className = "select-item";
            onClickHandler({ target, shiftKey, row, preventDefault: vi.fn() });
        }

        beforeEach(() => {
            viewObj.getItem = vi.fn((i: number) => items[i]);
            viewObj.getLength = vi.fn(() => items.length);
        });

        it("selects rows between the anchor and the shift-clicked row", () => {
            const mixin = new GridRowSelectionMixin(mockGrid);
            click(1);
            click(3, true);
            expect(mixin.getSelectedKeys()).toEqual(["2", "3", "4"]);
            expect(gridObj.invalidateRows).toHaveBeenLastCalledWith([1, 2, 3]);
        });

        it("deselects the range when the clicked row is selected", () => {
            const mixin = new GridRowSelectionMixin(mockGrid);
            mixin.selectKeys(["1", "2", "3", "4", "5"]);
            click(3);
            click(0, true);
            expect(mixin.getSelectedKeys()).toEqual(["5"]);
        });

        it("skips rows that are not selectable in a range", () => {
            const mixin = new GridRowSelectionMixin(mockGrid, { selectable: x => x.id != 3 });
            click(0);
            click(4, true);
            expect(mixin.getSelectedKeys()).toEqual(["1", "2", "4", "5"]);
        });

        it("toggles a single row when shift is held without an anchor", () => {
            const mixin = new GridRowSelectionMixin(mockGrid);
            click(2, true);
            expect(mixin.getSelectedKeys()).toEqual(["3"]);
        });

        it("resets the anchor on clear", () => {
            const mixin = new GridRowSelectionMixin(mockGrid);
            click(0);
            mixin.clear();
            click(3, true);
            expect(mixin.getSelectedKeys()).toEqual(["4"]);
        });

        it("resets the anchor on select all", () => {
            viewObj.getItems = vi.fn(() => items);
            const mixin = new GridRowSelectionMixin(mockGrid);
            click(0);
            const header = document.createElement("input");
            header.className = "select-all-items";
            onHeaderClickHandler({ target: header, preventDefault: vi.fn() });
            click(3, true);
            expect(mixin.getSelectedKeys()).toEqual(["4"]);
        });

        it("prevents text selection on shift mousedown on a row checkbox", () => {
            new GridRowSelectionMixin(mockGrid);
            const box = element.appendChild(document.createElement("input"));
            box.className = "select-item";
            const e = new MouseEvent("mousedown", { shiftKey: true, bubbles: true, cancelable: true });
            box.dispatchEvent(e);
            expect(e.defaultPrevented).toBe(true);
        });
    });

    describe("select all checkbox", () => {
        function createHeader() {
            const header = document.createElement("div");
            header.className = "select-all-header";
            const name = document.createElement("div");
            name.className = "slick-column-name";
            const box = document.createElement("input");
            box.type = "checkbox";
            box.className = "select-all-items";
            name.appendChild(box);
            header.appendChild(name);
            element.appendChild(header);
            return box;
        }

        beforeEach(() => {
            viewObj.getItems = vi.fn(() => [{ id: 1 }, { id: 2 }, { id: 3 }]);
        });

        it("is clear when nothing is selected", () => {
            const box = createHeader();
            new GridRowSelectionMixin(mockGrid).updateSelectAll();
            expect(box.checked).toBe(false);
            expect(box.indeterminate).toBe(false);
        });

        it("is indeterminate when some rows are selected", () => {
            const box = createHeader();
            const mixin = new GridRowSelectionMixin(mockGrid);
            mixin.selectKeys(["1"]);
            expect(box.checked).toBe(false);
            expect(box.indeterminate).toBe(true);
        });

        it("is checked when all rows are selected", () => {
            const box = createHeader();
            const mixin = new GridRowSelectionMixin(mockGrid);
            mixin.selectKeys(["1", "2", "3"]);
            expect(box.checked).toBe(true);
            expect(box.indeterminate).toBe(false);
        });

        it("is checked when all selectable rows are selected", () => {
            const box = createHeader();
            const mixin = new GridRowSelectionMixin(mockGrid, { selectable: x => x.id != 3 });
            mixin.selectKeys(["1", "2"]);
            expect(box.checked).toBe(true);
        });

        it("ignores selected keys that are not in the view", () => {
            const box = createHeader();
            const mixin = new GridRowSelectionMixin(mockGrid);
            mixin.selectKeys(["9"]);
            expect(box.checked).toBe(false);
            expect(box.indeterminate).toBe(false);
        });
    });

    describe("selected row", () => {
        it("adds the selected css class to metadata of selected items", () => {
            const base = vi.fn(() => ({ cssClasses: "x" }));
            viewObj.getItemMetadataCallback = vi.fn(() => base);
            const mixin = new GridRowSelectionMixin(mockGrid);
            mixin.selectKeys(["1"]);
            const callback = viewObj.setItemMetadataCallback.mock.calls[0][0];
            expect(callback({ id: 1 }, 0)).toEqual({ cssClasses: "x selected" });
            expect(callback({ id: 2 }, 1)).toEqual({ cssClasses: "x" });
        });

        it("renders labeled checkboxes", () => {
            const mixin = new GridRowSelectionMixin(mockGrid);
            const column = GridRowSelectionMixin.createSelectColumn(() => mixin);
            const box = column.format({ item: { id: 1 }, row: 2 } as any) as HTMLInputElement;
            expect(box.type).toBe("checkbox");
            expect(box.getAttribute("aria-label")).toBe("Select row");
            expect((column.nameFormat({} as any) as HTMLInputElement).getAttribute("aria-label")).toBe("Select all rows");
        });
    });

    describe("keyboard", () => {
        function key(target: HTMLElement, key: string) {
            const e = new KeyboardEvent("keydown", { key, bubbles: true, cancelable: true });
            target.dispatchEvent(e);
            return e;
        }

        beforeEach(() => {
            viewObj.getLength = vi.fn(() => 1);
        });

        it("clears the selection on Escape", () => {
            const mixin = new GridRowSelectionMixin(mockGrid);
            mixin.selectKeys(["1", "2"]);
            key(element, "Escape");
            expect(mixin.getSelectedKeys()).toEqual([]);
            expect(gridObj.invalidateAllRows).toHaveBeenCalled();
        });

        it("keeps the selection on Escape inside a text input", () => {
            const mixin = new GridRowSelectionMixin(mockGrid);
            mixin.selectKeys(["1"]);
            const input = element.appendChild(document.createElement("input"));
            key(input, "Escape");
            expect(mixin.getSelectedKeys()).toEqual(["1"]);
        });

        it("does not stop Escape when nothing is selected", () => {
            new GridRowSelectionMixin(mockGrid);
            const onKeyDown = vi.fn();
            document.body.addEventListener("keydown", onKeyDown);
            document.body.appendChild(element);
            key(element, "Escape");
            document.body.removeChild(element);
            document.body.removeEventListener("keydown", onKeyDown);
            expect(onKeyDown).toHaveBeenCalled();
        });

        it("toggles the checkbox of the focused cell on Space", () => {
            new GridRowSelectionMixin(mockGrid);
            const cell = element.appendChild(document.createElement("div"));
            cell.className = "slick-cell";
            const box = cell.appendChild(document.createElement("input"));
            box.className = "select-item";
            const click = vi.fn();
            box.addEventListener("click", click);
            const e = key(cell, " ");
            expect(click).toHaveBeenCalled();
            expect(e.defaultPrevented).toBe(true);
        });

        it("stops listening after destroy", () => {
            const mixin = new GridRowSelectionMixin(mockGrid);
            const removeSpy = vi.spyOn(element, "removeEventListener");
            mixin.destroy();
            expect(removeSpy).toHaveBeenCalledWith("keydown", expect.any(Function));
            expect(removeSpy).toHaveBeenCalledWith("mousedown", expect.any(Function));
        });
    });

    describe("selection bar", () => {
        let toolbar: HTMLElement;

        beforeEach(() => {
            toolbar = element.appendChild(document.createElement("div"));
            toolbar.className = "grid-toolbar";
        });

        it("is not created by default", () => {
            const mixin = new GridRowSelectionMixin(mockGrid);
            mixin.selectKeys(["1"]);
            expect(toolbar.querySelector(".selection-bar")).toBeNull();
        });

        it("shows the selected count when rows are selected", () => {
            const mixin = new GridRowSelectionMixin(mockGrid, { selectionBar: true });
            mixin.selectKeys(["1", "2"]);
            const bar = toolbar.querySelector<HTMLElement>(".selection-bar");
            expect(bar.hidden).toBe(false);
            expect(bar.querySelector(".selection-count").textContent).toBe("2 selected");
        });

        it("shows how many of the selected rows are on the page", () => {
            viewObj.getLength = vi.fn(() => 1);
            viewObj.getItem = vi.fn(() => ({ id: 1 }));
            const mixin = new GridRowSelectionMixin(mockGrid, { selectionBar: true });
            mixin.selectKeys(["1", "7", "9"]);
            const bar = toolbar.querySelector<HTMLElement>(".selection-bar");
            expect(bar.querySelector(".selection-count").textContent).toBe("3 selected");
            expect(bar.querySelector(".selection-page").textContent).toBe("1 on this page");
            mixin.clear();
            mixin.selectKeys(["1"]);
            expect(bar.querySelector(".selection-page").textContent).toBe("");
        });

        it("is hidden when the selection is empty", () => {
            const mixin = new GridRowSelectionMixin(mockGrid, { selectionBar: true });
            mixin.selectKeys(["1"]);
            mixin.clear();
            expect(toolbar.querySelector<HTMLElement>(".selection-bar").hidden).toBe(true);
        });

        it("clears the selection with the Clear button", () => {
            viewObj.getLength = vi.fn(() => 1);
            const mixin = new GridRowSelectionMixin(mockGrid, { selectionBar: true });
            mixin.selectKeys(["1"]);
            toolbar.querySelector<HTMLElement>(".selection-clear").click();
            expect(mixin.getSelectedKeys()).toEqual([]);
            expect(toolbar.querySelector<HTMLElement>(".selection-bar").hidden).toBe(true);
            expect(gridObj.invalidateAllRows).toHaveBeenCalled();
        });

        it("is removed on destroy", () => {
            const mixin = new GridRowSelectionMixin(mockGrid, { selectionBar: true });
            mixin.selectKeys(["1"]);
            mixin.destroy();
            expect(toolbar.querySelector(".selection-bar")).toBeNull();
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
        });

        it("format returns empty string when mixin is null", () => {
            const getMixin = vi.fn(() => null);
            const column = GridRowSelectionMixin.createSelectColumn(getMixin);
            const result = column.format({ item: { id: 1 } } as any);
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
});
