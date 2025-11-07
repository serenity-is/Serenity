import { ViewportInfo } from "../../src/core";
import { GridOptions } from "../../src/core/gridoptions";
import { BasicLayout, SleekGrid } from "../../src/grid/";

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

        const gridOptions: GridOptions = {
            jQuery: MockJQueryStatic as any,
            layoutEngine
        };

        const container = new (MockJQueryStatic as any)(document.createElement("div"));
        const grid = new SleekGrid(container as any, [], [], gridOptions);

        const canvases = grid.getCanvases();
        expect(canvases).toBeInstanceOf(MockJQueryStatic);
        expect(canvases.length).toBe(1);
    });

    it('should return activeCanvasNode as null if its not set', () => {
        const grid = new SleekGrid(document.createElement('div'), [], [], {});

        expect(grid.getActiveCanvasNode()).toBeFalsy();
    });

    it('should return activeCanvasNode after its set with an event', () => {
        const oldGetComputedStyle = window.getComputedStyle;
        window.getComputedStyle = (_el: HTMLElement) => {
            return { height: '1000px', width: '1000px', getPropertyValue: (_prop: string) => 1000 } as any;
        };

        const grid = new SleekGrid(document.createElement('div'), [{ c1: 1, c2: 8 }], [{ field: "c1" }, { field: "c2" }], {
            rowHeight: 20
        });


        const cellNode = grid.getCellNode(0, 0)
        window.getComputedStyle = oldGetComputedStyle;

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

        const grid = new SleekGrid(container, [], [], {});

        grid.getActiveCanvasNode({
            target: container
        });

        expect(grid.getActiveCanvasNode()).toBe(canvasNode);
    });

    it('should not set active canvas if event is null', () => {
        const grid = new SleekGrid(document.createElement('div'), [], [], {});

        grid.getActiveCanvasNode(null);

        expect(grid.getActiveCanvasNode()).toBeFalsy();
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
            if (el.matches(".slick-viewport.sg-body"))
                return { height: viewportHeight + 'px' } as any;

            return oldGetComputedStyles(el);
        }

        new SleekGrid(document.createElement('div'), data, columns, gridOptions);
        const viewportWithVScroll = getViewportInfo();

        viewportHeight = dataHeight + 1;
        new SleekGrid(document.createElement('div'), data, columns, gridOptions);
        const viewportWithoutVScroll = getViewportInfo(); /* reference of this function has changed */

        window.getComputedStyle = oldGetComputedStyles;

        expect(viewportWithVScroll.hasVScroll).toBe(true);
        expect(viewportWithoutVScroll.hasVScroll).toBe(false);
    });

    it('should return activeViewportNode as null if its not set', () => {
        const grid = new SleekGrid(document.createElement('div'), [], [], {});

        expect(grid.getActiveViewportNode()).toBeFalsy();
    });

    it('should return activeViewportNode after its set with an event', () => {
        const oldGetComputedStyle = window.getComputedStyle;
        window.getComputedStyle = (_el: HTMLElement) => {
            return { height: '1000px', width: '1000px', getPropertyValue: (_prop: string) => 1000 } as any;
        };

        const grid = new SleekGrid(document.createElement('div'), [{ c1: 1, c2: 8 }], [{ field: "c1" }, { field: "c2" }], {
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

        const grid = new SleekGrid(container, [], [], {});

        grid.getActiveViewportNode({
            target: container
        });

        expect(grid.getActiveViewportNode()).toBe(viewportNode);
    });

    it('should not set active viewport if event is null', () => {
        const grid = new SleekGrid(document.createElement('div'), [], [], {});

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
            asserted = true;
            oldLayoutEngineInit.call(layoutEngine, layoutHost);
        };

        new SleekGrid(document.createElement('div'), [], [], { layoutEngine });

        if (!asserted)
            throw "assertion not made";
    });
});
