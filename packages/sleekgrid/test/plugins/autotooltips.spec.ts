import { AutoTooltips } from "../../src/plugins/autotooltips";

describe("AutoTooltips.defaults", () => {
    it("enableForCells true", () => {
        expect(AutoTooltips.defaults.enableForCells).toBe(true);
    });

    it("enableForHeaders false", () => {
        expect(AutoTooltips.defaults.enableForHeaderCells).toBeFalsy();
    });

    it("maxToolTipLength null", () => {
        expect(AutoTooltips.defaults.maxToolTipLength).toBe(null);
    });

    it("replaceExisting true", () => {
        expect(AutoTooltips.defaults.replaceExisting).toBe(true);
    });
});

function mockGrid() {
    var grid = {
        node: {
            title: <string>null,
            textContent: "text-content",
            clientWidth: 10,
            scrollWidth: 20
        },
        onMouseEnterList: <any[]>[],
        onMouseEnter: {
            subscribe: function(f: any) { grid.onMouseEnterList.push(f); },
            unsubscribe: function(f: any) {
                var idx = grid.onMouseEnterList.indexOf(f);
                expect(idx >= 0).toBe(true);
                grid.onMouseEnterList.splice(idx, 1);
            }
        },
        onHeaderMouseEnterList: <any[]>[],
        onHeaderMouseEnter: {
            subscribe: function(f: any) { grid.onHeaderMouseEnterList.push(f); },
            unsubscribe: function(f: any) {
                var idx = grid.onHeaderMouseEnterList.indexOf(f);
                expect(idx >= 0).toBe(true);
                grid.onHeaderMouseEnterList.splice(idx, 1);
            }
        },
        getCellFromEvent: function(e: any) {
            grid.getCellFromEventCalls++;
            return e.cell === undefined ? { row: 7, cell: 3 } : e.cell;
        },
        getCellFromEventCalls: 0,
        getCellNode: function(row: number, cell: number) {
            grid.getCellNodeCalls++;
            return row == 7 && cell == 3 ? grid.node : null
        },
        getCellNodeCalls: 0
    };
    return grid;
}

describe("AutoTooltips.enableForCells", () => {

    it("attaches to onMouseEnter when enableForCells not specified", () => {
        var plugin = new AutoTooltips();
        var grid = mockGrid();
        plugin.init(grid as any);
        expect(grid.onMouseEnterList.length).toBe(1);
        plugin.destroy();
        expect(grid.onMouseEnterList.length).toBe(0);
    });

    it("attaches to onMouseEnter when enableForCells is true", () => {
        var plugin = new AutoTooltips({ enableForCells: true });
        var grid = mockGrid();
        plugin.init(grid as any);
        expect(grid.onMouseEnterList.length).toBe(1);
        plugin.destroy();
        expect(grid.onMouseEnterList.length).toBe(0);
    });

    it("does not attach to onMouseEnter when enableForCells is false", () => {
        var plugin = new AutoTooltips({ enableForCells: false });
        var grid = mockGrid();
        plugin.init(grid as any);
        expect(grid.onMouseEnterList.length).toBe(0);
        plugin.destroy();
        expect(grid.onMouseEnterList.length).toBe(0);
    });

});

describe("AutoTooltips.enableForHeaderCells", () => {

    it("attaches to onHeaderMouseEnter when enableForCells is true", () => {
        var plugin = new AutoTooltips({ enableForHeaderCells: true });
        var grid = mockGrid();
        plugin.init(grid as any);
        expect(grid.onHeaderMouseEnterList.length).toBe(1);
        plugin.destroy();
        expect(grid.onHeaderMouseEnterList.length).toBe(0);
    });

    it("does not attache to onHeaderMouseEnter when enableForHeaderCells not specified", () => {
        var plugin = new AutoTooltips();
        var grid = mockGrid();
        plugin.init(grid as any);
        expect(grid.onHeaderMouseEnterList.length).toBe(0);
        plugin.destroy();
        expect(grid.onHeaderMouseEnterList.length).toBe(0);
    });

    it("does not attach to onHeaderMouseEnter when enableForHeaderCells is false", () => {
        var plugin = new AutoTooltips({ enableForHeaderCells: false });
        var grid = mockGrid();
        plugin.init(grid as any);
        expect(grid.onHeaderMouseEnterList.length).toBe(0);
        plugin.destroy();
        expect(grid.onHeaderMouseEnterList.length).toBe(0);
    });

});

describe("AutoTooltips cell mouse enter event handler", () => {
    it("ignores when no cell at event pos", () => {
        var plugin = new AutoTooltips({ enableForCells: true });
        var grid = mockGrid();
        plugin.init(grid as any);
        var handler = grid.onMouseEnterList[0];
        expect(handler).toBeDefined();
        handler({ cell: null });
        expect(grid.getCellFromEventCalls).toBe(1);
        expect(grid.getCellNodeCalls).toBe(0);
        expect(grid.node.title).toBe(null);
    });

    it("ignores when no cell node at event pos", () => {
        var plugin = new AutoTooltips({ enableForCells: true });
        var grid = mockGrid();
        plugin.init(grid as any);
        var handler = grid.onMouseEnterList[0];
        expect(handler).toBeDefined();
        handler({ cell: { row: 999, cell: 9999 } });
        expect(grid.getCellFromEventCalls).toBe(1);
        expect(grid.getCellNodeCalls).toBe(1);
        expect(grid.node.title).toBe(null);
    });

    it("ignores when node has title and replaceExisting is false", () => {
        var plugin = new AutoTooltips({ enableForCells: true, replaceExisting: false });
        var grid = mockGrid();
        grid.node.title = "hasTitle";
        plugin.init(grid as any);
        var handler = grid.onMouseEnterList[0];
        expect(handler).toBeDefined();
        handler({});
        expect(grid.getCellFromEventCalls).toBe(1);
        expect(grid.getCellNodeCalls).toBe(1);
        expect(grid.node.title).toBe("hasTitle");
    });

    it("sets title to empty string when there is no overflow", () => {
        var plugin = new AutoTooltips({ enableForCells: true });
        var grid = mockGrid();
        grid.node.title = "title-before";
        grid.node.textContent = "nooverflow-text";
        grid.node.scrollWidth = grid.node.clientWidth;
        plugin.init(grid as any);
        var handler = grid.onMouseEnterList[0];
        expect(handler).toBeDefined();
        handler({});
        expect(grid.node.title).toBe("");
    });

    it("applies textContent as title when there is overflow", () => {
        var plugin = new AutoTooltips({ enableForCells: true });
        var grid = mockGrid();
        grid.node.textContent = "overflowed-text";
        grid.node.scrollWidth = grid.node.clientWidth + 10;
        plugin.init(grid as any);
        var handler = grid.onMouseEnterList[0];
        expect(handler).toBeDefined();
        handler({});
        expect(grid.node.title).toBe("overflowed-text");
    });

    it("truncates the title with an ellipsis when maxToolTipLength is exceeded", () => {
        var plugin = new AutoTooltips({ enableForCells: true, maxToolTipLength: 10 });
        var grid = mockGrid();
        grid.node.textContent = "this is a very long text";
        grid.node.scrollWidth = grid.node.clientWidth + 10;
        plugin.init(grid as any);
        var handler = grid.onMouseEnterList[0];
        expect(handler).toBeDefined();
        handler({});
        expect(grid.node.title).toBe("this is...");
    });

    it("does not truncate when maxToolTipLength is set but text fits", () => {
        var plugin = new AutoTooltips({ enableForCells: true, maxToolTipLength: 100 });
        var grid = mockGrid();
        grid.node.textContent = "short-text";
        grid.node.scrollWidth = grid.node.clientWidth + 10;
        plugin.init(grid as any);
        var handler = grid.onMouseEnterList[0];
        expect(handler).toBeDefined();
        handler({});
        expect(grid.node.title).toBe("short-text");
    });

});

function makeHeaderCol(overrides: any = {}) {
    const col = document.createElement("div");
    col.className = "slick-header-column";
    col.textContent = "header-text";
    Object.defineProperty(col, "clientWidth", { value: 10, configurable: true });
    Object.defineProperty(col, "scrollWidth", { value: overrides.scrollWidth ?? 20, configurable: true });
    return col;
}

describe("AutoTooltips header mouse enter event handler", () => {

    it("sets title to column name when header overflows and column has no toolTip", () => {
        var plugin = new AutoTooltips({ enableForHeaderCells: true });
        var grid = mockGrid();
        plugin.init(grid as any);
        var handler = grid.onHeaderMouseEnterList[0];
        expect(handler).toBeDefined();
        const headerCol = makeHeaderCol();
        handler({ column: { name: "colname" }, target: headerCol });
        expect(headerCol.title).toBe("colname");
    });

    it("sets title to empty string when header does not overflow", () => {
        var plugin = new AutoTooltips({ enableForHeaderCells: true });
        var grid = mockGrid();
        plugin.init(grid as any);
        var handler = grid.onHeaderMouseEnterList[0];
        expect(handler).toBeDefined();
        const headerCol = makeHeaderCol({ scrollWidth: 10 });
        handler({ column: { name: "colname" }, target: headerCol });
        expect(headerCol.title).toBe("");
    });

    it("sets title to empty string when column name is not a string", () => {
        var plugin = new AutoTooltips({ enableForHeaderCells: true });
        var grid = mockGrid();
        plugin.init(grid as any);
        var handler = grid.onHeaderMouseEnterList[0];
        expect(handler).toBeDefined();
        const headerCol = makeHeaderCol();
        handler({ column: { name: 123 }, target: headerCol });
        expect(headerCol.title).toBe("");
    });

    it("does nothing when column already has a toolTip", () => {
        var plugin = new AutoTooltips({ enableForHeaderCells: true });
        var grid = mockGrid();
        plugin.init(grid as any);
        var handler = grid.onHeaderMouseEnterList[0];
        expect(handler).toBeDefined();
        const headerCol = makeHeaderCol();
        handler({ column: { name: "colname", toolTip: "existing" }, target: headerCol });
        expect(headerCol.title).toBe("");
    });

    it("does nothing when there is no column", () => {
        var plugin = new AutoTooltips({ enableForHeaderCells: true });
        var grid = mockGrid();
        plugin.init(grid as any);
        var handler = grid.onHeaderMouseEnterList[0];
        expect(handler).toBeDefined();
        const headerCol = makeHeaderCol();
        handler({ column: null, target: headerCol });
        expect(headerCol.title).toBe("");
    });

    it("does nothing when no header column node is found", () => {
        var plugin = new AutoTooltips({ enableForHeaderCells: true });
        var grid = mockGrid();
        plugin.init(grid as any);
        var handler = grid.onHeaderMouseEnterList[0];
        expect(handler).toBeDefined();
        const target = document.createElement("div");
        handler({ column: { name: "colname" }, target });
        expect(target.title).toBe("");
    });

});
