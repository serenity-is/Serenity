import { GridRowSelectionMixin } from "./gridrowselectionmixin";

describe("GridRowSelectionMixin", () => {
    let mockGrid: any;
    let gridObj: any;
    let viewObj: any;
    let onClickHandler: Function;
    let onHeaderClickHandler: Function;

    beforeEach(() => {
        onClickHandler = null as any;
        onHeaderClickHandler = null as any;
        gridObj = {
            onClick: { subscribe: vi.fn((h: any) => { onClickHandler = h; }), unsubscribe: vi.fn() },
            onHeaderClick: { subscribe: vi.fn((h: any) => { onHeaderClickHandler = h; }), unsubscribe: vi.fn() },
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
            expect(gridObj.updateRow).toHaveBeenCalledWith(0);
            expect(gridObj.updateRow).toHaveBeenCalledWith(1);
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
            expect(gridObj.updateRow).not.toHaveBeenCalled();
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
    });
});
