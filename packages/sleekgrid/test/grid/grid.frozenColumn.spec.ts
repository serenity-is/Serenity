import { Column } from "../../src/core/column";
import { SleekGrid } from "../../src/grid/sleekgrid";
import { FrozenLayout } from "../../src/layouts/frozen-layout";

const slickPaneMain = "slick-pane-main";
const slickPaneStart = "slick-pane-start";
const slickPaneTop = "slick-pane-top";
const slickPaneBottom = "slick-pane-bottom";

function threeCols(): Column[] {
    return [{
        id: 'c1',
        field: 'c1'
    }, {
        id: 'c2',
        field: 'c2'
    }, {
        id: 'c3',
        field: 'c3'
    }]
}

function container() {
    return document.body.appendChild(document.createElement('div'));
}

describe('options.frozenColumns', () => {

    it('is ignored when undefined', () => {
        const grid = new SleekGrid(container(), [], threeCols(), {
            enableColumnReorder: false,
            layoutEngine: new FrozenLayout()
        });

        expect(grid.getAllColumns().length).toBe(3);
        expect(grid.getColumns().length).toBe(3);
        expect(grid.getColumns()[0].frozen).toBeUndefined();
    });

    it('is ignored when null', () => {
        const grid = new SleekGrid(container(), [], threeCols(), {
            enableColumnReorder: false,
            frozenColumns: null,
            layoutEngine: new FrozenLayout()
        });

        expect(grid.getAllColumns().length).toBe(3);
        expect(grid.getColumns().length).toBe(3);
        expect(grid.getColumns()[0].frozen).toBeUndefined();
    });

    it('is ignored when less than zero', () => {
        const grid = new SleekGrid(container(), [], threeCols(), {
            enableColumnReorder: false,
            frozenColumns: -1,
            layoutEngine: new FrozenLayout()
        });

        expect(grid.getAllColumns().length).toBe(3);
        expect(grid.getColumns().length).toBe(3);
        expect(grid.getColumns()[0].frozen).toBeUndefined();
    });

    it('is ignored when than zero', () => {
        const grid = new SleekGrid(container(), [], threeCols(), {
            enableColumnReorder: false,
            frozenColumns: 0,
            layoutEngine: new FrozenLayout()
        });

        expect(grid.getAllColumns().length).toBe(3);
        expect(grid.getColumns().length).toBe(3);
        expect(grid.getColumns()[0].frozen).toBeUndefined();
    });

    it('sets first column to frozen when 1 and all cols are visible', () => {
        const grid = new SleekGrid(container(), [], threeCols(), {
            enableColumnReorder: false,
            frozenColumns: 1,
            layoutEngine: new FrozenLayout()
        });

        expect(grid.getAllColumns().length).toBe(3);
        expect(grid.getColumns().length).toBe(3);
        expect(grid.getColumns()[0].frozen).toBe(true);
    });

    it('sets the first visible column to frozen when 1', () => {
        var cols = threeCols();
        cols[0].visible = false;
        const grid = new SleekGrid(container(), [], cols, {
            enableColumnReorder: false,
            frozenColumns: 1,
            layoutEngine: new FrozenLayout()
        });

        expect(grid.getAllColumns().length).toBe(3);
        expect(grid.getColumns().length).toBe(2);
        expect(grid.getColumns()[0].frozen).toBe(true);
        expect(grid.getColumns()[0].id).toBe('c2');
        expect(grid.getColumns()[1].frozen).toBeUndefined();
    });

    it('sets first two columns to frozen when 2 and all cols are visible', () => {
        const grid = new SleekGrid(container(), [], threeCols(), {
            enableColumnReorder: false,
            frozenColumns: 2,
            layoutEngine: new FrozenLayout()
        });

        expect(grid.getAllColumns().length).toBe(3);
        expect(grid.getColumns().length).toBe(3);
        expect(grid.getColumns()[0].frozen).toBe(true);
        expect(grid.getColumns()[1].frozen).toBe(true);
    });

    it('sets the first two visible column to frozen when 2', () => {
        var cols = threeCols();
        cols[0].visible = false;
        const grid = new SleekGrid(container(), [], cols, {
            enableColumnReorder: false,
            frozenColumns: 2,
            layoutEngine: new FrozenLayout()
        });

        expect(grid.getAllColumns().length).toBe(3);
        expect(grid.getColumns().length).toBe(2);
        expect(grid.getColumns()[0].frozen).toBe(true);
        expect(grid.getColumns()[0].id).toBe('c2');
        expect(grid.getColumns()[1].frozen).toBe(true);
        expect(grid.getColumns()[1].id).toBe('c3');
    });

    it('null gets deleted from options after processing', () => {
        const grid = new SleekGrid(container(), [], threeCols(), {
            enableColumnReorder: false,
            frozenColumns: null,
            layoutEngine: new FrozenLayout()
        });

        expect(grid.getOptions().frozenColumns).toBeUndefined();
    });

    it('sets pane visibilities properly', () => {
        const div = container();
        const grid = new SleekGrid(div, [], threeCols(), {
            enableColumnReorder: false,
            frozenColumns: 2,
            layoutEngine: new FrozenLayout()
        });

        //const paneTopLeft = div.querySelector(`.${slickPaneTop}.${slickPaneStart}`) as HTMLDivElement;
        //expect(paneTopLeft).toBeDefined();
        //expect(paneTopLeft.hidden).toBe(false);
//
        //const paneTopRight = div.querySelector(`.${slickPaneTop}.${slickPaneMain}`) as HTMLDivElement;
        //expect(paneTopRight).toBeDefined();
        //expect(paneTopRight.hidden).toBe(false);
//
        //const paneBottomLeft = div.querySelector(`.${slickPaneBottom}.${slickPaneStart}`) as HTMLDivElement;
        //expect(paneBottomLeft).toBeDefined();
        //expect(paneBottomLeft.hidden).toBe(true);
//
        //const paneBottomRight = div.querySelector(`.${slickPaneBottom}.${slickPaneMain}`) as HTMLDivElement;
        //expect(paneBottomRight).toBeDefined();
        //expect(paneBottomRight.hidden).toBe(true);
    });

    it("switches scroll containers when setting frozen columns back to 0 at runtime", () => {
        const div = container();
        const layout = new FrozenLayout();
        const grid = new SleekGrid(div, [], threeCols(), {
            frozenColumns: 2,
            layoutEngine: layout
        });
        const cols = grid.getColumns();
        expect(cols[0].frozen).toBe(true);
        expect(cols[1].frozen).toBe(true);
        expect(cols[2].frozen).toBeFalsy();

        const viewportTopLeft = div.querySelector(`.${slickPaneTop}.${slickPaneStart} > .slick-viewport`) as HTMLDivElement;
        const viewportTopRight = div.querySelector(`.${slickPaneTop}.${slickPaneMain} > .slick-viewport`) as HTMLDivElement;
        expect(viewportTopLeft).toBeDefined();
        expect(viewportTopRight).toBeDefined();
        //expect(grid.getScrollContainerX()).toBe(viewportTopRight);
        //expect(grid.getScrollContainerY()).toBe(viewportTopRight);
        //grid.setOptions({
        //    frozenColumns: 0
        //});
        //expect(grid.getScrollContainerX()).toBe(viewportTopLeft);
        //expect(grid.getScrollContainerY()).toBe(viewportTopLeft);
        //const cols2 = grid.getColumns();
        //expect(cols2[0].frozen).toBeFalsy();
        //expect(cols2[1].frozen).toBeFalsy();
        //expect(cols2[2].frozen).toBeFalsy();
    });

    it("switches scroll containers when setting frozen columns > 0 at runtime", () => {
        const div = container();
        const layout = new FrozenLayout();
        const grid = new SleekGrid(div, [], threeCols(), {
            frozenColumns: 0,
            layoutEngine: layout
        });
        const cols = grid.getColumns();
        expect(cols[0].frozen).toBeFalsy();
        expect(cols[1].frozen).toBeFalsy();
        expect(cols[2].frozen).toBeFalsy();

        const viewportTopLeft = div.querySelector(`.${slickPaneTop}.${slickPaneStart} > .slick-viewport`) as HTMLDivElement;
        const viewportTopRight = div.querySelector(`.${slickPaneTop}.${slickPaneMain} > .slick-viewport`) as HTMLDivElement;
        expect(viewportTopLeft).toBeDefined();
        expect(viewportTopRight).toBeDefined();
       //expect(grid.getScrollContainerX()).toBe(viewportTopLeft);
       //expect(grid.getScrollContainerY()).toBe(viewportTopLeft);
       //grid.setOptions({
       //    frozenColumns: 2
       //});
       //expect(grid.getScrollContainerX()).toBe(viewportTopRight);
       //expect(grid.getScrollContainerY()).toBe(viewportTopRight);
       //const cols2 = grid.getColumns();
       //expect(cols2[0].frozen).toBe(true);
       //expect(cols2[1].frozen).toBe(true);
       //expect(cols2[2].frozen).toBeFalsy();
    });

    it("moves frozen columns to the left on init", () => {
        var cols = threeCols();
        cols[1].frozen = true;
        cols[2].frozen = true;
        const div = container();
        const layout = new FrozenLayout();
        const grid = new SleekGrid(div, [], cols, {
            enableColumnReorder: false,
            layoutEngine: layout
        });
        const cols2 = grid.getColumns();
        expect(cols2[0].id).toBe('c2');
        expect(cols2[0].frozen).toBe(true);
        expect(cols2[1].id).toBe('c3');
        expect(cols2[1].frozen).toBe(true);
        expect(cols2[2].id).toBe('c1');
        expect(cols2[2].frozen).toBeFalsy();
    });
});
