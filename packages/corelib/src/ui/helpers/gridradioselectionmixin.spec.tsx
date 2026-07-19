import { GridRadioSelectionMixin } from "./gridradioselectionmixin";

describe("GridRadioSelectionMixin", () => {
    let mockGrid: any;
    let gridObj: any;
    let viewObj: any;
    let subscribeHandler: Function;

    beforeEach(() => {
        subscribeHandler = null as any;
        gridObj = {
            onClick: { subscribe: vi.fn((handler: any) => { subscribeHandler = handler; }) },
            updateRow: vi.fn()
        };
        viewObj = {
            getIdPropertyName: vi.fn(() => "id"),
            getItem: vi.fn(),
            getLength: vi.fn(() => 0)
        };
        mockGrid = {
            getGrid: vi.fn(() => gridObj),
            getView: vi.fn(() => viewObj)
        };
    });

    describe("constructor", () => {
        it("subscribes to grid click event", () => {
            const mixin = new GridRadioSelectionMixin(mockGrid);
            expect(mockGrid.getGrid().onClick.subscribe).toHaveBeenCalled();
        });

        it("stores options", () => {
            const selectable = vi.fn();
            const mixin = new GridRadioSelectionMixin(mockGrid, { selectable });
            expect((mixin as any).options.selectable).toBe(selectable);
        });

        it("uses empty options when not provided", () => {
            const mixin = new GridRadioSelectionMixin(mockGrid);
            expect((mixin as any).options).toEqual({});
        });
    });

    describe("click handler", () => {
        it("selects item when clicking rad-select-item", () => {
            viewObj.getItem = vi.fn(() => ({ id: 42 }));
            viewObj.getLength = vi.fn(() => 2);
            const mixin = new GridRadioSelectionMixin(mockGrid);

            const e = { target: { classList: { contains: (c: string) => c === 'rad-select-item' } }, preventDefault: vi.fn() } as any;
            subscribeHandler(e, { row: 0 });

            expect(e.preventDefault).toHaveBeenCalled();
            expect((mixin as any).include["42"]).toBe(true);
            expect(gridObj.updateRow).toHaveBeenCalledWith(0);
            expect(gridObj.updateRow).toHaveBeenCalledWith(1);
        });

        it("deselects when clicking already selected item (toggle off)", () => {
            viewObj.getItem = vi.fn(() => ({ id: 42 }));
            viewObj.getLength = vi.fn(() => 1);
            const mixin = new GridRadioSelectionMixin(mockGrid);
            (mixin as any).include = { "42": true };

            const e = { target: { classList: { contains: (c: string) => c === 'rad-select-item' } }, preventDefault: vi.fn() } as any;
            subscribeHandler(e, { row: 0 });

            // Already selected: clearKeys removes all entries, nothing re-added
            expect((mixin as any).include).toEqual({});
        });

        it("does nothing when clicking non-rad-select-item", () => {
            const mixin = new GridRadioSelectionMixin(mockGrid);
            const e = { target: { classList: { contains: (c: string) => false } }, preventDefault: vi.fn() } as any;
            subscribeHandler(e, { row: 0 });

            expect(e.preventDefault).not.toHaveBeenCalled();
            expect(gridObj.updateRow).not.toHaveBeenCalled();
        });

        it("does nothing when item is not selectable", () => {
            viewObj.getItem = vi.fn(() => ({ id: 42 }));
            const mixin = new GridRadioSelectionMixin(mockGrid, {
                selectable: () => false
            });

            const e = { target: { classList: { contains: (c: string) => c === 'rad-select-item' } }, preventDefault: vi.fn() } as any;
            subscribeHandler(e, { row: 0 });

            expect(e.preventDefault).toHaveBeenCalled();
            expect((mixin as any).include).toEqual({});
        });
    });

    describe("clear", () => {
        it("resets include map", () => {
            const mixin = new GridRadioSelectionMixin(mockGrid);
            (mixin as any).include = { "1": true };
            mixin.clear();
            expect((mixin as any).include).toEqual({});
        });
    });

    describe("resetCheckedAndRefresh", () => {
        it("resets and repopulates", () => {
            const populateFn = vi.fn();
            (mockGrid.getView() as any).populate = populateFn;
            const mixin = new GridRadioSelectionMixin(mockGrid);
            (mixin as any).include = { "1": true };
            mixin.resetCheckedAndRefresh();
            expect((mixin as any).include).toEqual({});
            expect(populateFn).toHaveBeenCalled();
        });
    });

    describe("getSelectedKey", () => {
        it("returns selected key", () => {
            const mixin = new GridRadioSelectionMixin(mockGrid);
            (mixin as any).include = { "42": true };
            expect(mixin.getSelectedKey()).toBe("42");
        });

        it("returns null when nothing selected", () => {
            const mixin = new GridRadioSelectionMixin(mockGrid);
            expect(mixin.getSelectedKey()).toBeNull();
        });
    });

    describe("getSelectedAsInt32", () => {
        it("returns selected as int", () => {
            const mixin = new GridRadioSelectionMixin(mockGrid);
            (mixin as any).include = { "42": true };
            expect(mixin.getSelectedAsInt32()).toBe(42);
        });

        it("returns null when nothing selected", () => {
            const mixin = new GridRadioSelectionMixin(mockGrid);
            expect(mixin.getSelectedAsInt32()).toBeNull();
        });
    });

    describe("getSelectedAsInt64", () => {
        it("returns selected as int", () => {
            const mixin = new GridRadioSelectionMixin(mockGrid);
            (mixin as any).include = { "42": true };
            expect(mixin.getSelectedAsInt64()).toBe(42);
        });

        it("returns null when nothing selected", () => {
            const mixin = new GridRadioSelectionMixin(mockGrid);
            expect(mixin.getSelectedAsInt64()).toBeNull();
        });
    });

    describe("setSelectedKey", () => {
        it("clears and sets the selected key", () => {
            const mixin = new GridRadioSelectionMixin(mockGrid);
            (mixin as any).include = { "old": true };
            mixin.setSelectedKey("newKey");
            expect((mixin as any).include).toEqual({ newKey: true });
        });
    });

    describe("isSelectable", () => {
        it("returns true when no selectable option", () => {
            const mixin = new GridRadioSelectionMixin(mockGrid);
            const result = (mixin as any).isSelectable({ id: 1 });
            expect(result).toBe(true);
        });

        it("returns false when item is null", () => {
            const mixin = new GridRadioSelectionMixin(mockGrid);
            const result = (mixin as any).isSelectable(null);
            expect(result).toBeFalsy();
        });

        it("delegates to selectable option when provided", () => {
            const selectable = vi.fn(() => false);
            const mixin = new GridRadioSelectionMixin(mockGrid, { selectable });
            const item = { id: 1 };
            const result = (mixin as any).isSelectable(item);
            expect(selectable).toHaveBeenCalledWith(item);
            expect(result).toBe(false);
        });
    });

    describe("createSelectColumn", () => {
        it("returns a column with format function", () => {
            const mixin = new GridRadioSelectionMixin(mockGrid);
            const getMixin = vi.fn(() => mixin);
            const column = GridRadioSelectionMixin.createSelectColumn(getMixin);

            expect(column.id).toBe("__select__");
            expect(column.sortable).toBe(false);
            expect(typeof column.nameFormat).toBe("function");
            expect((column.nameFormat as any)()).toBe("");
        });

        it("format returns empty string when mixin is null", () => {
            const getMixin = vi.fn(() => null);
            const column = GridRadioSelectionMixin.createSelectColumn(getMixin);
            const result = column.format({ item: { id: 1 } } as any);
            expect(result).toBe("");
        });

        it("format returns empty string when item is not selectable", () => {
            const mixin = new GridRadioSelectionMixin(mockGrid, { selectable: () => false });
            const getMixin = vi.fn(() => mixin);
            const column = GridRadioSelectionMixin.createSelectColumn(getMixin);
            const result = column.format({ item: { id: 1 } } as any);
            expect(result).toBe("");
        });

        it("format returns radio input when item is selectable", () => {
            const mixin = new GridRadioSelectionMixin(mockGrid);
            (mixin as any).include = { "1": true };
            const getMixin = vi.fn(() => mixin);
            const column = GridRadioSelectionMixin.createSelectColumn(getMixin);
            const result = column.format({ item: { id: 1 } } as any);
            expect(result).toBeTruthy();
            expect((result as any).type).toBe("radio");
        });

        it("format returns unchecked radio when item is not in include", () => {
            const mixin = new GridRadioSelectionMixin(mockGrid);
            const getMixin = vi.fn(() => mixin);
            const column = GridRadioSelectionMixin.createSelectColumn(getMixin);
            const result = column.format({ item: { id: 2 } } as any);
            expect(result).toBeTruthy();
            expect((result as any).type).toBe("radio");
            expect((result as any).checked).toBeFalsy();
        });
    });
});
