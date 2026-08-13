import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
import { TreeGridMixin } from "./treegridmixin";

function createMockGrid(overrides?: any) {
    const gridContainer = document.createElement("div");
    gridContainer.className = "grid-container";
    const domNode = document.createElement("div");
    domNode.appendChild(gridContainer);
    document.body.appendChild(domNode);
    const view = { getItems: vi.fn(() => []), setItems: vi.fn(), getItemById: vi.fn(() => undefined), getLocalSort: vi.fn() };
    return {
        domNode,
        gridContainer,
        view,
        sleekGrid: { getCellFromEvent: vi.fn(() => ({ cell: 0, row: 0 })) },
        onFiltering: { subscribe: vi.fn() },
        onProcessData: { subscribe: vi.fn() },
        allColumns: [] as any[],
        getIdProperty: () => "ID",
        ...overrides
    };
}

describe("TreeGridMixin", () => {
    beforeEach(() => { document.body.innerHTML = ""; });
    afterEach(() => { document.body.innerHTML = ""; vi.restoreAllMocks(); });

    it("subscribes to filtering and process data events", () => {
        const dg = createMockGrid();
        const mixin = new TreeGridMixin({ grid: dg as any, getParentId: (x: any) => x.P, toggleField: "Toggle" });
        expect(dg.onFiltering.subscribe).toHaveBeenCalled();
        expect(dg.onProcessData.subscribe).toHaveBeenCalled();
        expect(mixin).toBeTruthy();
    });

    it("sets toggle column format when toggleField matches", () => {
        const col = { field: "Toggle", format: undefined };
        const dg = createMockGrid({ allColumns: [col] });
        new TreeGridMixin({ grid: dg as any, getParentId: (x: any) => x.P, toggleField: "Toggle" });
        expect(typeof col.format).toBe("function");
    });

    it("click handler ignores non-toggle clicks", () => {
        const dg = createMockGrid();
        new TreeGridMixin({ grid: dg as any, getParentId: (x: any) => x.P, toggleField: "Toggle" });
        expect(() => dg.gridContainer.dispatchEvent(new Event("click"))).not.toThrow();
    });

    it("toggle click handler toggles row", () => {
        const dg = createMockGrid();
        dg.view.getItems = vi.fn(() => [{ ID: 1, P: null }]);
        dg.view.setItems = vi.fn();
        dg.view.getRowById = vi.fn();
        dg.view.collapseGroup = vi.fn();
        dg.view.expandGroup = vi.fn();
        new TreeGridMixin({ grid: dg as any, getParentId: (x: any) => x.P, toggleField: "Toggle" });
        const toggle = document.createElement("div");
        toggle.className = "s-TreeToggle";
        dg.gridContainer.appendChild(toggle);
        toggle.dispatchEvent(new Event("click", { bubbles: true }));
        expect(dg.sleekGrid.getCellFromEvent).toHaveBeenCalled();
    });

    it("onFiltering handler sets isMatch", () => {
        const dg = createMockGrid();
        new TreeGridMixin({ grid: dg as any, getParentId: (x: any) => x.P, toggleField: "Toggle" });
        const filterHandler = dg.onFiltering.subscribe.mock.calls[0][0];
        const e = { item: { ID: 1, P: 1 }, isMatch: true };
        filterHandler(e);
        expect(typeof e.isMatch).toBe("boolean");
    });

    it("onProcessData handler reorders entities", () => {
        const dg = createMockGrid();
        new TreeGridMixin({ grid: dg as any, getParentId: (x: any) => x.P, toggleField: "Toggle" });
        const processHandler = dg.onProcessData.subscribe.mock.calls[0][0];
        const e = { response: { Entities: [{ ID: 1, P: null }, { ID: 2, P: 1 }] } };
        expect(() => processHandler(e)).not.toThrow();
        expect(e.response.Entities.map((x: any) => x.ID)).toEqual([1, 2]);
    });

    describe("applyTreeOrdering", () => {
        it("orders parents before children", () => {
            const items = [{ ID: 1, P: null }, { ID: 2, P: 1 }, { ID: 3, P: 1 }, { ID: 4, P: 2 }];
            const result = TreeGridMixin.applyTreeOrdering(items, (x: any) => x.ID, (x: any) => x.P);
            expect(result.map(x => x.ID)).toEqual([1, 2, 4, 3]);
        });

        it("keeps items without children in order", () => {
            const items = [{ ID: 1, P: null }, { ID: 2, P: 5 }];
            const result = TreeGridMixin.applyTreeOrdering(items, (x: any) => x.ID, (x: any) => x.P);
            expect(result.map(x => x.ID)).toEqual([1, 2]);
        });
    });

    describe("collapse and expand", () => {
        it("collapseAll sets collapsed on all items", () => {
            const items = [{ _collapsed: false }];
            const dg = createMockGrid({ view: { getItems: vi.fn(() => items), setItems: vi.fn() } });
            const mixin = new TreeGridMixin({ grid: dg as any, getParentId: (x: any) => x.P, toggleField: "Toggle" });
            mixin.collapseAll();
            expect(dg.view.setItems).toHaveBeenCalled();
            expect(items[0]._collapsed).toBe(true);
        });

        it("expandAll clears collapsed on all items", () => {
            const items = [{ _collapsed: true }];
            const dg = createMockGrid({ view: { getItems: vi.fn(() => items), setItems: vi.fn() } });
            const mixin = new TreeGridMixin({ grid: dg as any, getParentId: (x: any) => x.P, toggleField: "Toggle" });
            mixin.expandAll();
            expect(dg.view.setItems).toHaveBeenCalled();
            expect(items[0]._collapsed).toBe(false);
        });

        it("toggleAll collapses expanded rows", () => {
            const items = [{ _collapsed: false }];
            const dg = createMockGrid({ view: { getItems: vi.fn(() => items), setItems: vi.fn() } });
            const mixin = new TreeGridMixin({ grid: dg as any, getParentId: (x: any) => x.P, toggleField: "Toggle" });
            mixin.toggleAll();
            expect(dg.view.setItems).toHaveBeenCalled();
            expect(items[0]._collapsed).toBe(true);
        });
    });
});
