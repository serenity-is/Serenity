import { CellRange, EventEmitter } from "../../src/core";
import { SelectionModel } from "../../src/grid";
import { Grid } from "../../src/grid/grid";

it('should selectionModel init with grid', () => {
    const grid = new Grid(document.createElement('div'), [], [], {});

    let selectionModelInitGrid: Grid | null = null;
    const selectionModel: SelectionModel = {
        init: (grid: Grid) => {
            selectionModelInitGrid = grid;
        },
        setSelectedRanges: (_ranges: CellRange[]) => {
        },
        refreshSelections: () => {
        },
        onSelectedRangesChanged: new EventEmitter<CellRange[]>()
    };

    grid.setSelectionModel(selectionModel);

    expect(selectionModelInitGrid).toBe(grid);
});

it('should subscribe to selectionModel.onSelectedRangesChanged', () => {
    const grid = new Grid(document.createElement('div'), [], [], {});

    let selectionModelOnSelectedRangesSubscribed = false;

    const eventEmitter = new EventEmitter<CellRange[]>();
    eventEmitter.subscribe = () => {
        selectionModelOnSelectedRangesSubscribed = true;
    };

    const selectionModel: SelectionModel = {
        init: (_grid: Grid) => {},
        setSelectedRanges: (_ranges: CellRange[]) => {
        },
        refreshSelections: () => {
        },
        onSelectedRangesChanged: eventEmitter
    };

    grid.setSelectionModel(selectionModel);
    expect(selectionModelOnSelectedRangesSubscribed).toBe(true);
});

it('should unsubscribe from selectionModel.onSelectedRangesChanged on destroy', () => {
    const grid = new Grid(document.createElement('div'), [], [], {});

    let selectionModelOnSelectedRangesUnsubscribed = false;

    const eventEmitter = new EventEmitter<CellRange[]>();
    eventEmitter.unsubscribe = () => {
        selectionModelOnSelectedRangesUnsubscribed = true;
    };

    const selectionModel: SelectionModel = {
        init: (_grid: Grid) => {},
        setSelectedRanges: (_ranges: CellRange[]) => {
        },
        refreshSelections: () => {
        },
        onSelectedRangesChanged: eventEmitter
    };

    grid.setSelectionModel(selectionModel);
    grid.destroy();
    expect(selectionModelOnSelectedRangesUnsubscribed).toBe(true);
});

it('should unsubscribe from selectionModel.onSelectedRangesChanged when setting a new selectionModel', () => {
    const grid = new Grid(document.createElement('div'), [], [], {});

    let selectionModelOnSelectedRangesUnsubscribed = false;

    const eventEmitter = new EventEmitter<CellRange[]>();
    eventEmitter.unsubscribe = () => {
        selectionModelOnSelectedRangesUnsubscribed = true;
    };

    const selectionModel: SelectionModel = {
        init: (_grid: Grid) => {},
        setSelectedRanges: (_ranges: CellRange[]) => {
        },
        refreshSelections: () => {
        },
        onSelectedRangesChanged: eventEmitter
    };

    grid.setSelectionModel(selectionModel);
    grid.setSelectionModel({
        init: (_grid: Grid) => {},
        setSelectedRanges: (_ranges: CellRange[]) => {
        },
        refreshSelections: () => {
        },
        onSelectedRangesChanged: new EventEmitter<CellRange[]>()
    });
    expect(selectionModelOnSelectedRangesUnsubscribed).toBe(true);
});

it('should return current selectionModel on getSelectionModel', () => {
    const grid = new Grid(document.createElement('div'), [], [], {});

    const selectionModel: SelectionModel = {
        init: (_grid: Grid) => {},
        setSelectedRanges: (_ranges: CellRange[]) => {
        },
        refreshSelections: () => {
        },
        onSelectedRangesChanged: new EventEmitter<CellRange[]>()
    };

    grid.setSelectionModel(selectionModel);
    expect(grid.getSelectionModel()).toBe(selectionModel);
});
