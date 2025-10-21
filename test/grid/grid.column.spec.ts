import { Column } from "../../src/core";
import { BasicLayout, Grid } from "../../src/grid";

const getTestColumns = (): Column[] => ([{
    id: 'test_id',
    field: 'test_field',
    name: 'test_name',
}, {
    id: 'test2_id',
    field: 'test2_field',
    name: 'test2_name',
}]);

describe('updateColumnHeader', () => {
    it('should be able to update column header', () => {
        const columns = getTestColumns();
        const grid = new Grid(document.createElement('div'), [], columns, {});

        grid.updateColumnHeader(columns[0].id, 'abc', 'def');

        expect(columns[0].name).toBe('abc');
        expect(columns[0].toolTip).toBe('def');
        expect(grid.getHeaderColumn(columns[0].id).title).toBe('def');
    });

    it('should trigger onBeforeHeaderCellDestroy event before update', () => {
        const columns = getTestColumns();
        columns[0].toolTip = 'test';

        const grid = new Grid(document.createElement('div'), [], columns, {});

        let onBeforeHeaderCellDestroyCalled = false;
        grid.onBeforeHeaderCellDestroy.subscribe((eventData, args) => {
            onBeforeHeaderCellDestroyCalled = true;

            expect(args.column).toBe(columns[0]);
            expect(args.node).toBe(grid.getHeaderColumn(columns[0].id));

            expect(grid.getHeaderColumn(columns[0].id).title).toBe('test');
        });

        grid.updateColumnHeader(columns[0].id, 'abc', 'def');

        expect(onBeforeHeaderCellDestroyCalled).toBe(true);
    });

    it('should trigger onHeaderCellRendered event after update', () => {
        const columns = getTestColumns();
        const grid = new Grid(document.createElement('div'), [], columns, {});

        let onHeaderCellRenderedCalled = false;
        grid.onHeaderCellRendered.subscribe((eventData, args) => {
            onHeaderCellRenderedCalled = true;

            expect(args.column).toBe(columns[0]);
            expect(args.node).toBe(grid.getHeaderColumn(columns[0].id));

            expect(grid.getHeaderColumn(columns[0].id).title).toBe('def');
        });

        grid.updateColumnHeader(columns[0].id, 'abc', 'def');

        expect(onHeaderCellRenderedCalled).toBe(true);
    });

    it('should update innerHTML when passed a function', () => {
        const columns = getTestColumns();
        const grid = new Grid(document.createElement('div'), [], columns, {});

        grid.updateColumnHeader(columns[0].id, () => {
            const i = document.createElement('i');
            const span = document.createElement('span');
            span.id = 'test';
            span.textContent = 'abc';
            i.appendChild(span);
            return i;
        }, 'def');

        expect(grid.getHeaderColumn(columns[0].id).querySelector("#test").innerHTML).toBe('abc');
    });

    it('should update textContent when passed a string', () => {
        const columns = getTestColumns();
        const grid = new Grid(document.createElement('div'), [], columns, {});

        grid.updateColumnHeader(columns[0].id, `<i><span id="test">abc</span></i>`, 'def');

        expect(grid.getHeaderColumn(columns[0].id).querySelector("#test")).toBeNull();
        expect(grid.getHeaderColumn(columns[0].id).textContent).toBe(`<i><span id="test">abc</span></i>`);
    });

    it('should not update column header if grid is not initialized', () => {
        const columns = getTestColumns();
        const grid = new Grid(document.createElement('div'), [], columns, {
            explicitInitialization: true
        });

        grid.updateColumnHeader(columns[0].id, 'foo', 'foo');

        expect(columns[0].name).not.toBe('foo');
        expect(columns[0].toolTip).not.toBe('foo');
        expect(grid.getHeaderColumn(columns[0].id)?.title).not.toBe('foo');
    });
});

describe('getColumnIndex', () => {
    it('should return the column index', () => {
        const columns = getTestColumns();
        const grid = new Grid(document.createElement('div'), [], columns, {});

        expect(grid.getColumnIndex(columns[0].id)).toBe(0);
        expect(grid.getColumnIndex(columns[1].id)).toBe(1);
    });

    it('should return the column index after the index change', () => {
        const columns = getTestColumns();
        const grid = new Grid(document.createElement('div'), [], columns, {});

        grid.setColumns([columns[1], columns[0]]);

        expect(grid.getColumnIndex(columns[0].id)).toBe(1);
        expect(grid.getColumnIndex(columns[1].id)).toBe(0);
    });

    it('should not be aware of the indexes of the not visible columns', () => {
        const columns = getTestColumns();
        columns[0].visible = false;
        const grid = new Grid(document.createElement('div'), [], columns, {});

        expect(grid.getColumnIndex(columns[0].id)).toBeFalsy();
        expect(grid.getColumnIndex(columns[1].id)).toBe(0);
    });
});

describe('getInitialColumnIndex', () => {
    it('should return the initial column index', () => {
        const layoutEngine = new BasicLayout();
        layoutEngine.reorderViewColumns = (viewCols, _opt) => {
            return [...viewCols].reverse();
        }

        const columns = getTestColumns();
        const grid = new Grid(document.createElement('div'), [], columns, { layoutEngine });

        expect(grid.getInitialColumnIndex(columns[0].id)).toBe(0);
        expect(grid.getInitialColumnIndex(columns[1].id)).toBe(1);

        expect(grid.getColumnIndex(columns[0].id)).toBe(1);
        expect(grid.getColumnIndex(columns[1].id)).toBe(0);
    });

    it('should be aware of the indexes of the not visible columns', () => {
        const layoutEngine = new BasicLayout();
        layoutEngine.reorderViewColumns = (viewCols, _opt) => {
            return [...viewCols].reverse();
        }

        const columns = getTestColumns();
        columns[0].visible = false;
        const grid = new Grid(document.createElement('div'), [], columns, { layoutEngine });

        expect(grid.getInitialColumnIndex(columns[0].id)).toBe(0);
        expect(grid.getInitialColumnIndex(columns[1].id)).toBe(1);

        expect(grid.getColumnIndex(columns[0].id)).toBeFalsy();
        expect(grid.getColumnIndex(columns[1].id)).toBe(0);
    });
});

describe('getInitialColumns', () => {
    it('should return initial columns', () => {
        const columns = getTestColumns();
        const grid = new Grid(document.createElement('div'), [], columns, {});

        expect(grid.getInitialColumns()).toBe(columns);
    });

    it('should return initial columns after the index change', () => {
        const layoutEngine = new BasicLayout();
        layoutEngine.reorderViewColumns = (viewCols, _opt) => {
            return [...viewCols].reverse();
        }

        const columns = getTestColumns();
        const grid = new Grid(document.createElement('div'), [], columns, { layoutEngine });

        expect(grid.getInitialColumns()).toBe(columns);
    });

    it('should return initial columns even though column was not visible', () => {
        const columns = getTestColumns();
        columns[0].visible = false;
        const grid = new Grid(document.createElement('div'), [], columns, {});

        expect(grid.getInitialColumns()).toBe(columns);
    })
});

describe('getColumns', () => {
    it('should return the columns', () => {
        const columns = getTestColumns();
        const grid = new Grid(document.createElement('div'), [], columns, {});

        expect(grid.getColumns()).toStrictEqual(columns);
    });

    it('should return the columns after the index change', () => {
        const layoutEngine = new BasicLayout();
        layoutEngine.reorderViewColumns = (viewCols, _opt) => {
            return [...viewCols].reverse();
        }

        const columns = getTestColumns();
        const grid = new Grid(document.createElement('div'), [], columns, { layoutEngine });


        expect(grid.getColumns()).toStrictEqual([...columns].reverse());
    });

    it('should not return not visible columns', () => {
        const columns = getTestColumns();
        columns[0].visible = false;
        const grid = new Grid(document.createElement('div'), [], columns, {});

        expect(grid.getColumns()).toEqual([columns[1]]);
    });
});

describe('setColumns', () => {
    it('should set initial columns', () => {
        const columns = getTestColumns();
        const grid = new Grid(document.createElement('div'), [], columns, {});

        const newColumns = getTestColumns();
        newColumns[0].name = 'foo';
        grid.setColumns(newColumns);

        expect(grid.getInitialColumns()).toBe(newColumns);
    });

    it('should set columns', () => {
        const columns = getTestColumns();
        const grid = new Grid(document.createElement('div'), [], columns, {});

        const newColumns = getTestColumns();
        newColumns[0].name = 'foo';
        newColumns[1].visible = false;
        grid.setColumns(newColumns);

        expect(grid.getColumns()).toStrictEqual([newColumns[0]]);
    });
});
