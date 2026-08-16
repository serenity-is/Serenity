import { CellRange } from "../../src/core";
import { RowSelectionModel } from "../../src/plugins/rowselectionmodel";
import { SleekGrid } from "../../src/grid/sleekgrid";

function makeData(n: number) {
    const data: any[] = [];
    for (let i = 0; i < n; i++)
        data.push({ id: i, c1: "r" + i, c2: "c" + i });
    return data;
}

function makeCols() {
    return [
        { id: "c1", name: "c1", field: "c1" },
        { id: "c2", name: "c2", field: "c2" }
    ];
}

function setup(n = 10, options: any = {}) {
    const container = document.createElement("div");
    container.style.height = "100px";
    document.body.appendChild(container);
    const grid = new SleekGrid(container, makeData(n), makeCols(), options);
    const model = new RowSelectionModel();
    grid.setSelectionModel(model);
    return { container, grid, model };
}

function fireKey(grid: SleekGrid, which: number, opts: any = {}) {
    const ev = new KeyboardEvent("keydown", { bubbles: true, cancelable: true, ...opts });
    Object.defineProperty(ev, "which", { value: which, configurable: true });
    grid.getCanvasNode().dispatchEvent(ev);
    return ev;
}

function fireClick(grid: SleekGrid, row: number, cell: number, opts: any = {}) {
    const cellNode = grid.getCellNode(row, cell);
    const ev = new MouseEvent("click", { bubbles: true, cancelable: true, ...opts });
    cellNode.dispatchEvent(ev);
    return ev;
}

describe("RowSelectionModel defaults", () => {
    it("selectActiveRow defaults to true", () => {
        expect(RowSelectionModel.defaults.selectActiveRow).toBe(true);
    });
});

describe("RowSelectionModel init/destroy", () => {
    it("subscribes to grid events on init and unsubscribes on destroy", () => {
        const { grid, model } = setup();
        grid.setActiveCell(1, 0);
        expect(model.getSelectedRows()).toEqual([1]);

        model.destroy();
        grid.setActiveCell(2, 0);
        expect(model.getSelectedRows()).toEqual([1]);
        grid.destroy();
    });
});

describe("RowSelectionModel active cell change", () => {
    it("selects the active row when selectActiveRow is true", () => {
        const { grid, model } = setup();
        grid.setActiveCell(2, 0);
        expect(model.getSelectedRows()).toEqual([2]);
        expect(model.getSelectedRanges()).toEqual([new CellRange(2, 0, 2, 1)]);
        grid.destroy();
    });

    it("does not select the active row when selectActiveRow is false", () => {
        const { grid } = setup(10, {});
        const m = new RowSelectionModel({ selectActiveRow: false });
        grid.setSelectionModel(m);
        grid.setActiveCell(2, 0);
        expect(grid.getSelectedRows()).toEqual([]);
        expect(m.getSelectedRanges()).toBeUndefined();
        grid.destroy();
    });

    it("does nothing when the active cell is reset to null", () => {
        const { grid, model } = setup();
        grid.setActiveCell(2, 0);
        expect(model.getSelectedRows()).toEqual([2]);
        grid.resetActiveCell();
        expect(model.getSelectedRows()).toEqual([2]);
        grid.destroy();
    });
});

describe("RowSelectionModel keydown", () => {
    it("ignores keydown when there is no active cell", () => {
        const { grid } = setup();
        fireKey(grid, 40, { shiftKey: true });
        expect(grid.getSelectedRows()).toEqual([]);
        grid.destroy();
    });

    it("ignores keydown without shift", () => {
        const { grid, model } = setup();
        grid.setActiveCell(2, 0);
        fireKey(grid, 40, {});
        expect(model.getSelectedRows()).toEqual([2]);
        grid.destroy();
    });

    it("ignores keydown with ctrl", () => {
        const { grid, model } = setup();
        grid.setActiveCell(2, 0);
        fireKey(grid, 40, { shiftKey: true, ctrlKey: true });
        expect(model.getSelectedRows()).toEqual([2]);
        grid.destroy();
    });

    it("ignores keydown with alt", () => {
        const { grid, model } = setup();
        grid.setActiveCell(2, 0);
        fireKey(grid, 40, { shiftKey: true, altKey: true });
        expect(model.getSelectedRows()).toEqual([2]);
        grid.destroy();
    });

    it("ignores keydown with meta", () => {
        const { grid, model } = setup();
        grid.setActiveCell(2, 0);
        fireKey(grid, 40, { shiftKey: true, metaKey: true });
        expect(model.getSelectedRows()).toEqual([2]);
        grid.destroy();
    });

    it("ignores keydown with a non-arrow key", () => {
        const { grid, model } = setup();
        grid.setActiveCell(2, 0);
        fireKey(grid, 65, { shiftKey: true });
        expect(model.getSelectedRows()).toEqual([2]);
        grid.destroy();
    });

    it("extends selection down with shift+down when nothing is selected", () => {
        const { grid, model } = setup();
        grid.setActiveCell(2, 0);
        model.setSelectedRanges([]);
        fireKey(grid, 40, { shiftKey: true });
        expect(model.getSelectedRows()).toEqual([2, 3]);
        grid.destroy();
    });

    it("extends selection up with shift+up when nothing is selected", () => {
        const { grid, model } = setup();
        grid.setActiveCell(2, 0);
        model.setSelectedRanges([]);
        fireKey(grid, 38, { shiftKey: true });
        expect(model.getSelectedRows()).toEqual([1, 2]);
        grid.destroy();
    });

    it("extends selection down with shift+down from an existing selection", () => {
        const { grid, model } = setup();
        grid.setActiveCell(2, 0);
        model.setSelectedRows([2, 3]);
        fireKey(grid, 40, { shiftKey: true });
        expect(model.getSelectedRows()).toEqual([2, 3, 4]);
        grid.destroy();
    });

    it("shrinks selection up with shift+up from an existing selection", () => {
        const { grid, model } = setup();
        grid.setActiveCell(2, 0);
        model.setSelectedRows([2, 3]);
        fireKey(grid, 38, { shiftKey: true });
        expect(model.getSelectedRows()).toEqual([2]);
        grid.destroy();
    });

    it("keeps the anchor row when shift+up with active row above the selection bottom", () => {
        const { grid, model } = setup();
        grid.setActiveCell(3, 0);
        model.setSelectedRows([5]);
        fireKey(grid, 38, { shiftKey: true });
        expect(model.getSelectedRows()).toEqual([4, 5]);
        grid.destroy();
    });

    it("does not change selection when the new active row would be out of bounds", () => {
        const { grid, model } = setup();
        grid.setActiveCell(0, 0);
        model.setSelectedRows([0]);
        fireKey(grid, 38, { shiftKey: true });
        expect(model.getSelectedRows()).toEqual([0]);
        grid.destroy();
    });

    it("prevents default and stops propagation on an eligible key", () => {
        const { grid, container } = setup();
        grid.setActiveCell(2, 0);
        let containerKeydown = 0;
        container.addEventListener("keydown", () => containerKeydown++);
        const ev = fireKey(grid, 40, { shiftKey: true });
        expect(ev.defaultPrevented).toBe(true);
        expect(containerKeydown).toBe(0);
        grid.destroy();
    });
});

describe("RowSelectionModel click", () => {
    it("returns false when the cell cannot be active", () => {
        const cols = [
            { id: "c1", name: "c1", field: "c1", focusable: false },
            { id: "c2", name: "c2", field: "c2" }
        ];
        const container = document.createElement("div");
        container.style.height = "100px";
        document.body.appendChild(container);
        const grid = new SleekGrid(container, makeData(10), cols, {});
        const model = new RowSelectionModel();
        grid.setSelectionModel(model);
        grid.setActiveCell(0, 1);
        expect(model.getSelectedRows()).toEqual([0]);
        fireClick(grid, 0, 0, { ctrlKey: true });
        expect(model.getSelectedRows()).toEqual([0]);
        grid.destroy();
        container.remove();
    });

    it("does not multi-select when multiSelect is disabled", () => {
        const { grid, model } = setup(10, { multiSelect: false });
        grid.setActiveCell(0, 0);
        fireClick(grid, 1, 0, { ctrlKey: true });
        // the click moves the active cell (selecting row 1) but does not extend a multi-selection
        expect(model.getSelectedRows()).toEqual([1]);
        grid.destroy();
    });

    it("does not multi-select when no modifier key is pressed", () => {
        const { grid, model } = setup();
        grid.setActiveCell(0, 0);
        fireClick(grid, 1, 0, {});
        // the click moves the active cell (selecting row 1) but does not extend a multi-selection
        expect(model.getSelectedRows()).toEqual([1]);
        grid.destroy();
    });

    it("adds a row to the selection with ctrl+click", () => {
        const { grid, model } = setup();
        grid.setActiveCell(0, 0);
        fireClick(grid, 1, 0, { ctrlKey: true });
        expect(model.getSelectedRows()).toEqual([0, 1]);
        grid.destroy();
    });

    it("removes a row from the selection with ctrl+click on a selected row", () => {
        const { grid, model } = setup();
        grid.setActiveCell(0, 0);
        fireClick(grid, 1, 0, { ctrlKey: true });
        expect(model.getSelectedRows()).toEqual([0, 1]);
        fireClick(grid, 1, 0, { ctrlKey: true });
        expect(model.getSelectedRows()).toEqual([0]);
        grid.destroy();
    });

    it("adds a row to the selection with meta+click", () => {
        const { grid, model } = setup();
        grid.setActiveCell(0, 0);
        fireClick(grid, 1, 0, { metaKey: true });
        expect(model.getSelectedRows()).toEqual([0, 1]);
        grid.destroy();
    });

    it("extends the selection with shift+click", () => {
        const { grid, model } = setup();
        grid.setActiveCell(0, 0);
        fireClick(grid, 2, 0, { shiftKey: true });
        expect(model.getSelectedRows()).toEqual([1, 2, 0]);
        grid.destroy();
    });
});

describe("RowSelectionModel ranges API", () => {
    it("returns early when both old and new selections are empty", () => {
        const { grid, model } = setup();
        let notified = 0;
        model.onSelectedRangesChanged.subscribe(() => notified++);
        model.setSelectedRanges([]);
        expect(notified).toBe(0);
        grid.destroy();
    });

    it("notifies onSelectedRangesChanged when setting ranges", () => {
        const { grid, model } = setup();
        let notified: CellRange[] | null = null;
        model.onSelectedRangesChanged.subscribe((_e, ranges) => { notified = ranges; });
        model.setSelectedRanges([new CellRange(1, 0, 1, 1)]);
        expect(notified).toEqual([new CellRange(1, 0, 1, 1)]);
        expect(model.getSelectedRanges()).toEqual([new CellRange(1, 0, 1, 1)]);
        grid.destroy();
    });

    it("converts rows to ranges with setSelectedRows", () => {
        const { grid, model } = setup();
        model.setSelectedRows([1, 2]);
        expect(model.getSelectedRanges()).toEqual([
            new CellRange(1, 0, 1, 1),
            new CellRange(2, 0, 2, 1)
        ]);
        expect(model.getSelectedRows()).toEqual([1, 2]);
        grid.destroy();
    });
});
