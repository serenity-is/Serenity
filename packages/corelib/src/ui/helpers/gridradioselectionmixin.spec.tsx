import { GridRadioSelectionMixin } from "./gridradioselectionmixin";

describe("GridRadioSelectionMixin", () => {
    let mockGrid: any;
    let gridObj: any;
    let viewObj: any;

    beforeEach(() => {
        gridObj = {
            onClick: { subscribe: vi.fn() },
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

    it("constructor subscribes to grid click event", () => {
        const mixin = new GridRadioSelectionMixin(mockGrid);
        expect(mockGrid.getGrid().onClick.subscribe).toHaveBeenCalled();
    });

    it("clear resets include map", () => {
        const mixin = new GridRadioSelectionMixin(mockGrid);
        (mixin as any).include = { "1": true };
        mixin.clear();
        expect((mixin as any).include).toEqual({});
    });

    it("resetCheckedAndRefresh resets and repopulates", () => {
        const populateFn = vi.fn();
        (mockGrid.getView() as any).populate = populateFn;
        const mixin = new GridRadioSelectionMixin(mockGrid);
        (mixin as any).include = { "1": true };
        mixin.resetCheckedAndRefresh();
        expect((mixin as any).include).toEqual({});
        expect(populateFn).toHaveBeenCalled();
    });

    it("getSelectedKey returns selected key", () => {
        const mixin = new GridRadioSelectionMixin(mockGrid);
        (mixin as any).include = { "42": true };
        expect(mixin.getSelectedKey()).toBe("42");
    });

    it("getSelectedKey returns null when nothing selected", () => {
        const mixin = new GridRadioSelectionMixin(mockGrid);
        expect(mixin.getSelectedKey()).toBeNull();
    });

    it("getSelectedAsInt32 returns selected as int", () => {
        const mixin = new GridRadioSelectionMixin(mockGrid);
        (mixin as any).include = { "42": true };
        expect(mixin.getSelectedAsInt32()).toBe(42);
    });

    it("getSelectedAsInt32 returns null when nothing selected", () => {
        const mixin = new GridRadioSelectionMixin(mockGrid);
        expect(mixin.getSelectedAsInt32()).toBeNull();
    });

    it("getSelectedAsInt64 returns selected as int", () => {
        const mixin = new GridRadioSelectionMixin(mockGrid);
        (mixin as any).include = { "42": true };
        expect(mixin.getSelectedAsInt64()).toBe(42);
    });

    it("getSelectedAsInt64 returns null when nothing selected", () => {
        const mixin = new GridRadioSelectionMixin(mockGrid);
        expect(mixin.getSelectedAsInt64()).toBeNull();
    });
});
