function threeCols(): Slick.Column[] {
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
        const grid = new Slick.Grid($('<div/>').appendTo(document.body), [], threeCols(), {
            enableColumnReorder: false
        });

        expect(grid.getColumns().length).toBe(3);
        expect(grid.getViewColumns().length).toBe(3);
        expect(grid.getColumns()[0].fixedTo).toBeUndefined();
    });

    it('is ignored when null', () => {
        const grid = new Slick.Grid($('<div/>').appendTo(document.body), [], threeCols(), {
            enableColumnReorder: false,
            frozenColumn: null
        });

        expect(grid.getColumns().length).toBe(3);
        expect(grid.getViewColumns().length).toBe(3);
        expect(grid.getColumns()[0].fixedTo).toBeUndefined();
    });

    it('is ignored when less than zero', () => {
        const grid = new Slick.Grid($('<div/>').appendTo(document.body), [], threeCols(), {
            enableColumnReorder: false,
            frozenColumn: -1
        });

        expect(grid.getColumns().length).toBe(3);
        expect(grid.getViewColumns().length).toBe(3);
        expect(grid.getColumns()[0].fixedTo).toBeUndefined();
    });

    it('sets first column to fixed start when 0 and all cols are visible', () => {
        const grid = new Slick.Grid($('<div/>').appendTo(document.body), [], threeCols(), {
            enableColumnReorder: false,
            frozenColumn: 0
        });

        expect(grid.getColumns().length).toBe(3);
        expect(grid.getViewColumns().length).toBe(3);
        expect(grid.getViewColumns()[0].fixedTo).toBe('start');
    });

    it('sets the first visible column to fixed start when 0', () => {
        var cols = threeCols();
        cols[0].visible = false;
        const grid = new Slick.Grid($('<div/>').appendTo(document.body), [], cols , {
            enableColumnReorder: false,
            frozenColumn: 0
        });

        expect(grid.getColumns().length).toBe(3);
        expect(grid.getViewColumns().length).toBe(2);
        expect(grid.getViewColumns()[0].fixedTo).toBe('start');
        expect(grid.getViewColumns()[0].id).toBe('c2');
        expect(grid.getViewColumns()[1].fixedTo).toBeUndefined();
    });    

    it('sets first two columns to fixed start when 1 and all cols are visible', () => {
        const grid = new Slick.Grid($('<div/>').appendTo(document.body), [], threeCols(), {
            enableColumnReorder: false,
            frozenColumn: 1
        });

        expect(grid.getColumns().length).toBe(3);
        expect(grid.getViewColumns().length).toBe(3);
        expect(grid.getViewColumns()[0].fixedTo).toBe('start');
        expect(grid.getViewColumns()[1].fixedTo).toBe('start');
    });

    it('sets the first two visible column to fixed start when 1', () => {
        var cols = threeCols();
        cols[0].visible = false;
        const grid = new Slick.Grid($('<div/>').appendTo(document.body), [], cols , {
            enableColumnReorder: false,
            frozenColumn: 1
        });

        expect(grid.getColumns().length).toBe(3);
        expect(grid.getViewColumns().length).toBe(2);
        expect(grid.getViewColumns()[0].fixedTo).toBe('start');
        expect(grid.getViewColumns()[0].id).toBe('c2');
        expect(grid.getViewColumns()[1].fixedTo).toBe('start');
        expect(grid.getViewColumns()[1].id).toBe('c3');
    });

    it('gets deleted from options after processing', () => {
        const grid = new Slick.Grid($('<div/>').appendTo(document.body), [], threeCols(), {
            enableColumnReorder: false,
            frozenColumn: 2
        });

        console.log(JSON.stringify(grid.getOptions()));
        expect(grid.getOptions().frozenColumn).toBeUndefined();
    });

});
