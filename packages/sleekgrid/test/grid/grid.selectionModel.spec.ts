import { CellRange, EventEmitter, type ISleekGrid, type SelectionModel } from "../../src/core";
import { SleekGrid } from "../../src/grid/sleekgrid";

it('should selectionModel init with grid', () => {
    const grid = new SleekGrid(document.createElement('div'), [], [], {});

    let selectionModelInitGrid: ISleekGrid | null = null;
    const selectionModel: SelectionModel = {
        init: (grid: ISleekGrid) => {
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
    const grid = new SleekGrid(document.createElement('div'), [], [], {});

    let selectionModelOnSelectedRangesSubscribed = false;

    const eventEmitter = new EventEmitter<CellRange[]>();
    eventEmitter.subscribe = () => {
        selectionModelOnSelectedRangesSubscribed = true;
    };

    const selectionModel: SelectionModel = {
        init: (_grid: ISleekGrid) => {},
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
    const grid = new SleekGrid(document.createElement('div'), [], [], {});

    let selectionModelOnSelectedRangesUnsubscribed = false;

    const eventEmitter = new EventEmitter<CellRange[]>();
    eventEmitter.unsubscribe = () => {
        selectionModelOnSelectedRangesUnsubscribed = true;
    };

    const selectionModel: SelectionModel = {
        init: (_grid: ISleekGrid) => {},
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
    const grid = new SleekGrid(document.createElement('div'), [], [], {});

    let selectionModelOnSelectedRangesUnsubscribed = false;

    const eventEmitter = new EventEmitter<CellRange[]>();
    eventEmitter.unsubscribe = () => {
        selectionModelOnSelectedRangesUnsubscribed = true;
    };

    const selectionModel: SelectionModel = {
        init: (_grid: ISleekGrid) => {},
        setSelectedRanges: (_ranges: CellRange[]) => {
        },
        refreshSelections: () => {
        },
        onSelectedRangesChanged: eventEmitter
    };

    grid.setSelectionModel(selectionModel);
    grid.setSelectionModel({
        init: (_grid: ISleekGrid) => {},
        setSelectedRanges: (_ranges: CellRange[]) => {
        },
        refreshSelections: () => {
        },
        onSelectedRangesChanged: new EventEmitter<CellRange[]>()
    });
    expect(selectionModelOnSelectedRangesUnsubscribed).toBe(true);
});

it('should return current selectionModel on getSelectionModel', () => {
    const grid = new SleekGrid(document.createElement('div'), [], [], {});

    const selectionModel: SelectionModel = {
        init: (_grid: ISleekGrid) => {},
        setSelectedRanges: (_ranges: CellRange[]) => {
        },
        refreshSelections: () => {
        },
        onSelectedRangesChanged: new EventEmitter<CellRange[]>()
    };

    grid.setSelectionModel(selectionModel);
    expect(grid.getSelectionModel()).toBe(selectionModel);
});
