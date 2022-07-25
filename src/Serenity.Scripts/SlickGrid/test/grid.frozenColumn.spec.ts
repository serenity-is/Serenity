import { Column } from "../column";
import { Grid } from "../grid";

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

describe('options.frozenColumn', () => {

    it('is ignored when undefined', () => {
        const grid = new Grid($('<div/>').appendTo(document.body), [], threeCols(), {
            enableColumnReorder: false
        });

        expect(grid.getInitialColumns().length).toBe(3);
        expect(grid.getColumns().length).toBe(3);
        expect(grid.getColumns()[0].frozen).toBeUndefined();
    });

    it('is ignored when null', () => {
        const grid = new Grid($('<div/>').appendTo(document.body), [], threeCols(), {
            enableColumnReorder: false,
            frozenColumn: null
        });

        expect(grid.getInitialColumns().length).toBe(3);
        expect(grid.getColumns().length).toBe(3);
        expect(grid.getColumns()[0].frozen).toBeUndefined();
    });

    it('is ignored when less than zero', () => {
        const grid = new Grid($('<div/>').appendTo(document.body), [], threeCols(), {
            enableColumnReorder: false,
            frozenColumn: -1
        });

        expect(grid.getInitialColumns().length).toBe(3);
        expect(grid.getColumns().length).toBe(3);
        expect(grid.getColumns()[0].frozen).toBeUndefined();
    });

    it('sets first column to frozen when 0 and all cols are visible', () => {
        const grid = new Grid($('<div/>').appendTo(document.body), [], threeCols(), {
            enableColumnReorder: false,
            frozenColumn: 0
        });

        expect(grid.getInitialColumns().length).toBe(3);
        expect(grid.getColumns().length).toBe(3);
        expect(grid.getColumns()[0].frozen).toBe(true);
    });

    it('sets the first visible column to frozen when 0', () => {
        var cols = threeCols();
        cols[0].visible = false;
        const grid = new Grid($('<div/>').appendTo(document.body), [], cols , {
            enableColumnReorder: false,
            frozenColumn: 0
        });

        expect(grid.getInitialColumns().length).toBe(3);
        expect(grid.getColumns().length).toBe(2);
        expect(grid.getColumns()[0].frozen).toBe(true);
        expect(grid.getColumns()[0].id).toBe('c2');
        expect(grid.getColumns()[1].frozen).toBeUndefined();
    });    

    it('sets first two columns to frozen when 1 and all cols are visible', () => {
        const grid = new Grid($('<div/>').appendTo(document.body), [], threeCols(), {
            enableColumnReorder: false,
            frozenColumn: 1
        });

        expect(grid.getInitialColumns().length).toBe(3);
        expect(grid.getColumns().length).toBe(3);
        expect(grid.getColumns()[0].frozen).toBe(true);
        expect(grid.getColumns()[1].frozen).toBe(true);
    });

    it('sets the first two visible column to frozen when 1', () => {
        var cols = threeCols();
        cols[0].visible = false;
        const grid = new Grid($('<div/>').appendTo(document.body), [], cols , {
            enableColumnReorder: false,
            frozenColumn: 1
        });

        expect(grid.getInitialColumns().length).toBe(3);
        expect(grid.getColumns().length).toBe(2);
        expect(grid.getColumns()[0].frozen).toBe(true);
        expect(grid.getColumns()[0].id).toBe('c2');
        expect(grid.getColumns()[1].frozen).toBe(true);
        expect(grid.getColumns()[1].id).toBe('c3');
    });

    it('null gets deleted from options after processing', () => {
        const grid = new Grid($('<div/>').appendTo(document.body), [], threeCols(), {
            enableColumnReorder: false,
            frozenColumn: null
        });

        expect(grid.getOptions().frozenColumn).toBeUndefined();
    });


});
