import { ViewportInfo } from "../../src/core";
import { GridOptions } from "../../src/core/gridoptions";
import { BasicLayout } from "../../src/grid/";
import { Grid } from "../../src/grid/grid";

describe('canvas', () => {

    it('should return canvases wrapped with jQuery if jQuery is available', () => {
        function MockJQueryStatic(el: HTMLElement | HTMLElement[]) {
            if (!(this instanceof MockJQueryStatic)) {
                return new (MockJQueryStatic as any)(el);
            }

            this.elements = Array.isArray(el) ? el : [el];
            this.length = this.elements.length;

            for (let i = 0; i < this.length; i++) {
                Object.defineProperty(this, i, {
                    get: () => this.elements[i]
                });
            }

            this.empty = () => this;
            this.on = () => this;
        }

        MockJQueryStatic.fn = {
            mousewheel: function () {
                return this;
            }
        };

        const layoutEngine = new BasicLayout();

        const canvasNodes = [document.createElement('div'), document.createElement('div')];

        const gridOptions: GridOptions = {
            jQuery: MockJQueryStatic as any,
            layoutEngine
        };

        const container = new (MockJQueryStatic as any)(document.createElement("div"));
        const grid = new Grid(container as any, [], [], gridOptions);

        layoutEngine.getRefs = () => {
            return {
                main: { body: { canvas: canvasNodes[0] } },
                end: { body: { canvas: canvasNodes[1] } },
                pinnedEndFirst: 1,
                pinnedStartLast: -Infinity,
                frozenTopLast: -Infinity,
                frozenBottomFirst: Infinity
            };
        };

        const canvases = grid.getCanvases();
        expect(canvases).toBeInstanceOf(MockJQueryStatic);
        expect(canvases.length).toBe(2);
        expect(canvases[0]).toBe(canvasNodes[0]);
        expect(canvases[1]).toBe(canvasNodes[1]);
    });

    it('should return activeCanvasNode as null if its not set', () => {
        const grid = new Grid(document.createElement('div'), [], [], {});

        expect(grid.getActiveCanvasNode()).toBeFalsy();
    });

    it('should return activeCanvasNode after its set with an event', () => {
        const oldGetComputedStyle = window.getComputedStyle;
        window.getComputedStyle = (_el: HTMLElement) => {
            return { height: '1000px', width: '1000px', getPropertyValue: (_prop: string) => 1000 } as any;
        };

        const grid = new Grid(document.createElement('div'), [{ c1: 1, c2: 8 }], [{ field: "c1" }, { field: "c2" }], {
            rowHeight: 20
        });

        window.getComputedStyle = oldGetComputedStyle;

        const cellNode = grid.getCellNode(0, 0)

        grid.getActiveCanvasNode({
            target: cellNode
        });

        expect(grid.getActiveCanvasNode()).toBeTruthy();
    });

    it('should traverse towards root of the document on activeCanvasNode', () => {
        const container = document.createElement('div');
        const canvasNode = document.createElement('div');
        canvasNode.classList.add('grid-canvas');

        canvasNode.appendChild(container); // Canvas -> Container -> Grid

        const grid = new Grid(container, [], [], {});

        grid.getActiveCanvasNode({
            target: container
        });

        expect(grid.getActiveCanvasNode()).toBe(canvasNode);
    });

    it('should not set active canvas if event is null', () => {
        const grid = new Grid(document.createElement('div'), [], [], {});

        grid.getActiveCanvasNode(null);

        expect(grid.getActiveCanvasNode()).toBeFalsy();
    });

    it('should get removed from the document tree on destroy', () => {
        const layoutEngine = new BasicLayout();
        const canvasNodes = [document.createElement('div'), document.createElement('div')];

        const documentFragment = document.createDocumentFragment();
        canvasNodes.forEach(canvasNode => documentFragment.appendChild(canvasNode));
        expect(documentFragment.childNodes.length).toBe(2);

        const grid = new Grid(document.createElement('div'), [], [], { layoutEngine });

        layoutEngine.getRefs = () => {
            return {
                main: { body: { canvas: canvasNodes[0] } },
                end: { body: { canvas: canvasNodes[1] } },
                pinnedEndFirst: 1,
                pinnedStartLast: -Infinity,
                frozenTopLast: -Infinity,
                frozenBottomFirst: Infinity
            };
        };

        expect(canvasNodes[0].parentNode).toBe(documentFragment);
        expect(canvasNodes[1].parentNode).toBe(documentFragment);

        grid.destroy();

        expect(canvasNodes[0].parentNode).toBeFalsy();
        expect(canvasNodes[1].parentNode).toBeFalsy();
    });
});

describe('viewport', () => {
    it('should set viewportInfo hasVScroll based on row count and rowHeight if autoHeight is false', () => {
        const layoutEngine = new BasicLayout();
        let getViewportInfo: () => ViewportInfo;

        const oldLayoutEngineInit = layoutEngine.init;
        layoutEngine.init = function (layoutHost) {
            getViewportInfo = layoutHost.getViewportInfo;
            oldLayoutEngineInit.call(layoutEngine, layoutHost);
        }

        const gridOptions: GridOptions = {
            layoutEngine,
            autoHeight: false,
            rowHeight: 20
        };

        const data = [
            { c1: 1 },
            { c1: 2 },
            { c1: 3 }
        ];

        const columns = [
            { field: "c1" }
        ];

        const dataHeight = data.length * gridOptions.rowHeight;
        let viewportHeight = 10;

        const oldGetComputedStyles = window.getComputedStyle;
        window.getComputedStyle = (el: HTMLElement) => {
            if (el === layoutEngine.getRefs().main.body.viewport)
                return { height: viewportHeight + 'px' } as any;

            return oldGetComputedStyles(el);
        }

        new Grid(document.createElement('div'), data, columns, gridOptions);
        const viewportWithVScroll = getViewportInfo();

        viewportHeight = dataHeight + 1;
        new Grid(document.createElement('div'), data, columns, gridOptions);
        const viewportWithoutVScroll = getViewportInfo(); /* reference of this function has changed */

        window.getComputedStyle = oldGetComputedStyles;

        expect(viewportWithVScroll.hasVScroll).toBe(true);
        expect(viewportWithoutVScroll.hasVScroll).toBe(false);
    });

    it('should return activeViewportNode as null if its not set', () => {
        const grid = new Grid(document.createElement('div'), [], [], {});

        expect(grid.getActiveViewportNode()).toBeFalsy();
    });

    it('should return activeViewportNode after its set with an event', () => {
        const oldGetComputedStyle = window.getComputedStyle;
        window.getComputedStyle = (_el: HTMLElement) => {
            return { height: '1000px', width: '1000px', getPropertyValue: (_prop: string) => 1000 } as any;
        };

        const grid = new Grid(document.createElement('div'), [{ c1: 1, c2: 8 }], [{ field: "c1" }, { field: "c2" }], {
            rowHeight: 20
        });

        window.getComputedStyle = oldGetComputedStyle;

        const cellNode = grid.getCellNode(0, 0)

        grid.getActiveViewportNode({
            target: cellNode
        });

        expect(grid.getActiveViewportNode()).toBeTruthy();
    });

    it('should traverse towards root of the document on activeViewportNode', () => {
        const container = document.createElement('div');
        const viewportNode = document.createElement('div');
        viewportNode.classList.add('slick-viewport');

        viewportNode.appendChild(container); // Viewport -> Container -> Grid

        const grid = new Grid(container, [], [], {});

        grid.getActiveViewportNode({
            target: container
        });

        expect(grid.getActiveViewportNode()).toBe(viewportNode);
    });

    it('should not set active viewport if event is null', () => {
        const grid = new Grid(document.createElement('div'), [], [], {});

        grid.getActiveCanvasNode(null);

        expect(grid.getActiveViewportNode()).toBeFalsy();
    });

    it('should return viewportInfo.width on getAvailableWidth if there viewportInfo hasVScroll is falsy', () => {
        const layoutEngine = new BasicLayout();

        const oldLayoutEngineInit = layoutEngine.init;
        let asserted = false;
        layoutEngine.init = function (layoutHost) {
            layoutHost.getViewportInfo().width = 123;
            expect(layoutHost.getViewportInfo().hasVScroll).toBeFalsy();
            expect(layoutHost.getAvailableWidth()).toBe(123);
            asserted = true;
            oldLayoutEngineInit.call(layoutEngine, layoutHost);
        };

        new Grid(document.createElement('div'), [], [], { layoutEngine });

        if (!asserted)
            throw "assertion not made";
    });

    it('should return viewportInfo.width - scrollbar width on getAvailableWidth if there viewportInfo hasVScroll is truthy', () => {
        const layoutEngine = new BasicLayout();

        const oldLayoutEngineInit = layoutEngine.init;
        let asserted = false;
        layoutEngine.init = function (layoutHost) {
            layoutHost.getViewportInfo().width = 123;
            layoutHost.getViewportInfo().hasVScroll = true;

            layoutHost.getScrollDims().width = 10;

            expect(layoutHost.getAvailableWidth()).toBe(123 - layoutHost.getScrollDims().width);

            asserted = true;
            oldLayoutEngineInit.call(layoutEngine, layoutHost);
        };

        new Grid(document.createElement('div'), [], [], { layoutEngine });

        if (!asserted)
            throw "assertion not made";
    });
});

describe('headers', () => {
    it('should return first header from the layout on getHeader', () => {
        const layoutEngine = new BasicLayout();
        const headerCols = [document.createElement('div'), document.createElement('div')];


        const grid = new Grid(document.createElement('div'), [], [], { layoutEngine });
        layoutEngine.getRefs = () => {
            return {
                start: { headerCols: headerCols[1], body: null },
                main: { headerCols: headerCols[0], body: null },
                pinnedEndFirst: Infinity,
                pinnedStartLast: 0,
                frozenTopLast: -Infinity,
                frozenBottomFirst: Infinity
            }
        };

        expect(grid.getHeader()).toBe(headerCols[0]);
    });

    describe('getHeaderColumn', () => {
        it('should return specific header column', () => {
            const layoutEngine = new BasicLayout();
            const headerCols = [document.createElement('div'), document.createElement('div')];

            layoutEngine.getHeaderColumn = (cell) => headerCols[cell];

            const grid = new Grid(document.createElement('div'), [], [], { layoutEngine });

            expect(grid.getHeaderColumn(1)).toBe(headerCols[1]);
        });

        it('should return null if header column is not found', () => {
            const layoutEngine = new BasicLayout();

            layoutEngine.getHeaderColumn = () => null;

            const grid = new Grid(document.createElement('div'), [], [], { layoutEngine });

            expect(grid.getHeaderColumn(1)).toBeNull();
        });
    });

    it('should return first row header from the layout on getHeaderRow', () => {
        const layoutEngine = new BasicLayout();
        const headerCols = [document.createElement('div'), document.createElement('div')];
        const headerParent = document.createElement('div'); // There are events that are bound to the parent of the header
        headerCols.forEach(col => headerParent.appendChild(col));

        const grid = new Grid(document.createElement('div'), [], [], { layoutEngine });
        layoutEngine.getRefs = () => {
            return {
                start: { headerRowCols: headerCols[1], body: null },
                main: { headerRowCols: headerCols[0], body: null },
                pinnedEndFirst: Infinity,
                pinnedStartLast: 0,
                frozenTopLast: -Infinity,
                frozenBottomFirst: Infinity


            }
        }

        expect(grid.getHeaderRow()).toBe(headerCols[0]);
    });

    describe('getHeaderRowColumn', () => {
        it('should return specific header row column', () => {
            const layoutEngine = new BasicLayout();
            const headerCols = [document.createElement('div'), document.createElement('div')];
            const headerParent = document.createElement('div');
            headerCols.forEach(col => headerParent.appendChild(col));

            layoutEngine.getHeaderRowColumn = (cell) => headerCols[cell];

            const grid = new Grid(document.createElement('div'), [], [], { layoutEngine });

            expect(grid.getHeaderRowColumn(1)).toBe(headerCols[1]);
        });

        it('should return null if header row column is not found', () => {
            const layoutEngine = new BasicLayout();

            layoutEngine.getHeaderRowColumn = () => null;

            const grid = new Grid(document.createElement('div'), [], [], { layoutEngine });

            expect(grid.getHeaderRowColumn(1)).toBeNull();
        });
    });

    it('should return first footer row col from the layout on getFooterRow', () => {
        const layoutEngine = new BasicLayout();
        const footerCols = [document.createElement('div'), document.createElement('div')];
        const footerParent = document.createElement('div');
        footerCols.forEach(col => footerParent.appendChild(col));

        const grid = new Grid(document.createElement('div'), [], [], { layoutEngine });

        layoutEngine.getRefs = () => {
            return {
                start: { footerRowCols: footerCols[1], body: null },
                main: { footerRowCols: footerCols[0], body: null },
                pinnedEndFirst: Infinity,
                pinnedStartLast: 0,
                frozenTopLast: -Infinity,
                frozenBottomFirst: Infinity
            }
        };

        expect(grid.getFooterRow()).toBe(footerCols[0]);
    });

    describe('getFooterRowColumn', () => {
        it('should return specific footer row column', () => {
            const layoutEngine = new BasicLayout();
            const footerCols = [document.createElement('div'), document.createElement('div')];
            const footerParent = document.createElement('div');
            footerCols.forEach(col => footerParent.appendChild(col));

            layoutEngine.getFooterRowColumn = (cell) => footerCols[cell];

            const grid = new Grid(document.createElement('div'), [], [], { layoutEngine });

            expect(grid.getFooterRowColumn(1)).toBe(footerCols[1]);
        });

        it('should return null if footer row column is not found', () => {
            const layoutEngine = new BasicLayout();

            layoutEngine.getFooterRowColumn = () => null;

            const grid = new Grid(document.createElement('div'), [], [], { layoutEngine });

            expect(grid.getFooterRowColumn(1)).toBeNull();
        });
    });
});
