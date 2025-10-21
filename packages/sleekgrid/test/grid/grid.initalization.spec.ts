import { Column, columnDefaults } from "../../src/core/column";
import { gridDefaults, GridOptions } from "../../src/core/gridoptions";
import { BasicLayout, LayoutHost } from "../../src/grid/";
import { Grid } from "../../src/grid/grid";

function threeCols(): Column[] {
    return [{
        id: 'c1',
        name: 'c1',
        field: 'c1'
    }, {
        id: 'c2',
        name: 'c2',
        field: 'c2'
    }, {
        id: 'c3',
        name: 'c3',
        field: 'c3'
    }]
}

describe('Grid columns', () => {
    it('should set column defaults from colDefaults', () => {
        if (Object.keys(columnDefaults).length === 0)
            return;

        const gridColumns = threeCols();
        new Grid(document.createElement("div"), [], gridColumns, {});

        expect(gridColumns).not.toStrictEqual(threeCols());

        for (const col of gridColumns)
            expect(col).toMatchObject(columnDefaults);
    });

    it('should set default width of the columns using gridDefaults', () => {
        if (gridDefaults.defaultColumnWidth === undefined)
            return;

        const gridColumns = threeCols();
        new Grid(document.createElement("div"), [], gridColumns, {});

        expect(gridColumns).not.toStrictEqual(threeCols());

        for (const col of gridColumns)
            expect(col.width).toBe(gridDefaults.defaultColumnWidth);
    });

    it('should set default width of the columns using gridOptions.defaultColumnWidth', () => {
        const gridColumns = threeCols();
        new Grid(document.createElement("div"), [], gridColumns, { defaultColumnWidth: 123 });

        for (const col of gridColumns)
            expect(col.width).toBe(123);
    });

    it('should set default width of the columns using columnDefaults.width', () => {
        const gridColumns = threeCols();
        new Grid(document.createElement("div"), [], gridColumns, { defaultColumnWidth: undefined });

        for (const col of gridColumns)
            expect(col.width).toBe(columnDefaults.width);
    });

    it('should not override already existing properties on the columns', () => {
        const colDefaultFirstProperty = Object.keys(columnDefaults).filter(x => x !== "width")[0];

        const gridColumns = threeCols();
        gridColumns[0].width = 123;

        if (colDefaultFirstProperty)
            (gridColumns[0] as any)[colDefaultFirstProperty] = "123";

        new Grid(document.createElement("div"), [], gridColumns, { defaultColumnWidth: 456 });

        expect(gridColumns[0].width).toBe(123);
        if (colDefaultFirstProperty)
            expect((gridColumns[0] as any)[colDefaultFirstProperty]).toBe("123");

        for (let i = 1; i < gridColumns.length; i++) {
            expect(gridColumns[i].width).toBe(456);
            if (colDefaultFirstProperty)
                expect((gridColumns[i] as any)[colDefaultFirstProperty]).toBe((columnDefaults as any)[colDefaultFirstProperty]);
        }
    });
});

describe('options', () => {
    it('should be able to set jQuery from options', () => {
        function MockJQueryStatic(el: HTMLElement) {

            if (!(this instanceof MockJQueryStatic)) {
                return new (MockJQueryStatic as any)(el);
            }

            this.el = el;
            this.empty = function () { return this };
            this.on = function () { return this }
            this.remove = function () { return this }
        }

        Object.defineProperty(MockJQueryStatic.prototype, 0, {
            get: function () {
                return this.el;
            },
            enumerable: false,
            configurable: true
        });

        MockJQueryStatic.fn = {
            mousewheel: function () {
                return this;
            }
        };

        const gridOptions: GridOptions = {
            jQuery: MockJQueryStatic as any
        };

        const container = MockJQueryStatic(document.createElement("div"));
        const grid = new Grid(container as any, [], [], gridOptions);

        expect(grid.getContainerNode()).toBe(container[0]);
        expect(gridOptions.jQuery).toStrictEqual(MockJQueryStatic);
    });

    it('should throw an error if container is null', () => {
        expect(() => new Grid(null, [], [], {})).toThrow();
    });

    it('should add slick-container class to the container', () => {
        const container = document.createElement("div");
        new Grid(container, [], [], {});

        expect(container.classList.contains("slick-container")).toBe(true);
    });

    it('should set default options', () => {
        const gridOptions: GridOptions = {}
        const grid = new Grid(document.createElement("div"), [], [], gridOptions);

        expect(grid.getOptions()).toMatchObject(gridDefaults);
    });

    it('should not override options with defaults', () => {
        const gridOptions: GridOptions = {
            rowHeight: (gridDefaults.rowHeight ?? 0) + 1
        };

        const grid = new Grid(document.createElement("div"), [], [], gridOptions);

        expect(grid.getOptions().rowHeight).toBe(gridOptions.rowHeight);
    });

    it('should set rtl to true if body has rtl class', () => {
        document.body.classList.add("rtl");
        const grid = new Grid(document.createElement("div"), [], [], {});

        expect(grid.getOptions().rtl).toBe(true);
        document.body.classList.remove("rtl");
    });

    it('should set rtl to true if container has rtl direction', () => {
        const oldGetComputedStyle = window.getComputedStyle;
        window.getComputedStyle = () => {
            return {
                direction: "rtl",
                getPropertyValue: () => null as any
            } as any;
        };

        const grid = new Grid(document.createElement("div"), [], [], {});
        window.getComputedStyle = oldGetComputedStyle;

        expect(grid.getOptions().rtl).toBe(true);
    });

    it('should set options.leaveSpaceForNewRows to false if autoHeight is true', () => {
        const gridOptions: GridOptions = {
            autoHeight: true,
            leaveSpaceForNewRows: true
        };

        const grid = new Grid(document.createElement("div"), [], [], gridOptions);

        expect(grid.getOptions().leaveSpaceForNewRows).toBe(false);
    });

    it('should not initialize grid if explicitInitialization is true', () => {
        const gridOptions: GridOptions = {
            explicitInitialization: true
        };

        const oldInit = Grid.prototype.init;
        Grid.prototype.init = () => {
            throw new Error("Grid was initialized");
        };

        new Grid(document.createElement("div"), [{ "c1": 1 }], [{ field: "c1" }], gridOptions);
        Grid.prototype.init = oldInit;
    });
});

describe('layout', () => {
    it('should empty innerHTML of the container', () => {
        const container = document.createElement("div");
        container.innerHTML = "foo 123";

        new Grid(container, [], [], {});

        expect(container.innerHTML).not.toContain("foo 123");
    });

    it('should set overflow to hidden', () => {
        const container = document.createElement("div");
        new Grid(container, [], [], {});

        expect(container.style.overflow).toBe("hidden");
    });

    it('should set outline to 0', () => {
        const container = document.createElement("div");
        new Grid(container, [], [], {});

        expect(container.style.outline).toBe(globalThis.jsdom ? "0" : "0px");
    });

    it('should add uid as a class to the container', () => {
        const container = document.createElement("div");
        const grid = new Grid(container, [], [], {});

        const uid = (grid as any).uid as string;

        if (!uid)
            return;

        expect(container.classList.contains(uid)).toBe(true);
    });

    it('should set position to relative if its not relative or absolute or fixed', () => {
        const oldGetComputedStyle = window.getComputedStyle;
        window.getComputedStyle = () => {
            return {
                position: "static",
                getPropertyValue: () => null as any
            } as any;
        }

        const container = document.createElement("div");
        new Grid(container, [], [], {});

        window.getComputedStyle = oldGetComputedStyle;

        expect(container.style.position).toBe("relative");
    });

    it('should append two elements with slick-focus-sink classes to the container', () => {
        const container = document.createElement("div");
        new Grid(container, [], [], {});

        expect(container.querySelectorAll(".slick-focus-sink")).toHaveLength(2);
    });

    it('should append slick-grouping-panel to the container if groupingPanel is true', () => {
        const container = document.createElement("div");
        new Grid(container, [], [], { groupingPanel: true });

        expect(container.querySelector(".slick-grouping-panel")).not.toBeNull();
    });

    it('should append slick-preheader-panel if groupingPanel and preHeaderPanel are true', () => {
        const container = document.createElement("div");
        new Grid(container, [], [], { groupingPanel: true, createPreHeaderPanel: true });

        expect(container.querySelector(".slick-grouping-panel")).not.toBeNull();
        expect(container.querySelector(".slick-preheader-panel")).not.toBeNull();
    });

    it('should not append slick-preheader-panel if groupingPanel is false', () => {
        const container = document.createElement("div");
        new Grid(container, [], [], { groupingPanel: false, createPreHeaderPanel: true });

        expect(container.querySelector(".slick-grouping-panel")).toBeNull();
        expect(container.querySelector(".slick-preheader-panel")).toBeNull();
    });

    it('should hide slick-grouping-panel if showGroupingPanel is false', () => {
        const container = document.createElement("div");
        new Grid(container, [], [], { groupingPanel: true, showGroupingPanel: false });

        expect(container.querySelector<HTMLElement>(".slick-grouping-panel")?.classList.contains("slick-hidden")).toBe(true);
    });

    it('should bind grid instance to the layout functions', () => {
        let layoutHostGrid: LayoutHost = null;

        const layoutEngine = new BasicLayout();
        const oldInit = layoutEngine.init;

        layoutEngine.init = function (grid: LayoutHost) {
            layoutHostGrid = grid;

            oldInit(grid);
        };

        const container = document.createElement("div");
        const grid = new Grid(container, [{ c1: 1 }], [{ field: "c1" }], { layoutEngine: layoutEngine });

        expect(layoutHostGrid.getCellFromPoint(0, 0)).toEqual(grid.getCellFromPoint(0, 0));
        expect(layoutHostGrid.getColumns()).toEqual(grid.getColumns());
        expect(layoutHostGrid.getContainerNode()).toEqual(grid.getContainerNode());
        expect(layoutHostGrid.getDataLength()).toEqual(grid.getDataLength());
        expect(layoutHostGrid.getDataLength()).toEqual(grid.getDataLength());
        expect(layoutHostGrid.getOptions()).toEqual(grid.getOptions());
    });

    it('should add viewport classes to every viewport node', () => {
        const layoutEngine = new BasicLayout();
        const viewportNodes = [document.createElement("div"), document.createElement("div")];

        layoutEngine.getViewportNodes = () => {
            return viewportNodes;
        };

        const container = document.createElement("div");
        new Grid(container, [], [], {
            layoutEngine: layoutEngine,
            viewportClass: "test-viewport-class"
        });

        expect(viewportNodes[0].classList.contains("test-viewport-class")).toBe(true);
        expect(viewportNodes[1].classList.contains("test-viewport-class")).toBe(true);
    });
});

describe('data event bindings', () => {
    it('should bind onRowCountChanged event of the data', () => {
        let onRowCountChangedSubscribeCalls = 0;
        const data = {
            onRowCountChanged: {
                subscribe: () => { onRowCountChangedSubscribeCalls++; }
            }
        };

        new Grid(document.createElement("div"), data, [], {});

        expect(onRowCountChangedSubscribeCalls).toBe(1);
    });

    it('should bind onRowsChanged event of the data', () => {
        let onRowsChangedSubscribeCalls = 0;
        const data = {
            onRowsChanged: {
                subscribe: () => { onRowsChangedSubscribeCalls++; }
            }
        };

        new Grid(document.createElement("div"), data, [], {});

        expect(onRowsChangedSubscribeCalls).toBe(1);
    });

    it('should bind onDataChanged event of the data', () => {
        let onDataChangedSubscribeCalls = 0;
        const data = {
            onDataChanged: {
                subscribe: () => { onDataChangedSubscribeCalls++; }
            }
        };

        new Grid(document.createElement("div"), data, [], {});

        expect(onDataChangedSubscribeCalls).toBe(1);
    });

    it('should unbind onRowCountChanged event of the data', () => {
        let onRowCountChangedUnsubscribeCalls = 0;
        const data = {
            onRowCountChanged: {
                subscribe: () => { },
                unsubscribe: () => { onRowCountChangedUnsubscribeCalls++; }
            }
        };

        const grid = new Grid(document.createElement("div"), data, [], {});
        grid.destroy();

        expect(onRowCountChangedUnsubscribeCalls).toBe(1);
    });

    it('should unbind onRowsChanged event of the data', () => {
        let onRowsChangedUnsubscribeCalls = 0;
        const data = {
            onRowsChanged: {
                subscribe: () => { },
                unsubscribe: () => { onRowsChangedUnsubscribeCalls++; }
            }
        };

        const grid = new Grid(document.createElement("div"), data, [], {});
        grid.destroy();

        expect(onRowsChangedUnsubscribeCalls).toBe(1);
    });

    it('should unbind onDataChanged event of the data', () => {
        let onDataChangedUnsubscribeCalls = 0;
        const data = {
            onDataChanged: {
                subscribe: () => { },
                unsubscribe: () => { onDataChangedUnsubscribeCalls++; }
            }
        };

        const grid = new Grid(document.createElement("div"), data, [], {});
        grid.destroy();

        expect(onDataChangedUnsubscribeCalls).toBe(1);
    });
});
