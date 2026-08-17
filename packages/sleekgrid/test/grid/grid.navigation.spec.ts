import type { Column, IDataView, ItemMetadata } from "../../src/core";
import { SleekGrid } from "../../src/grid";

interface NavigationRow {
    first: string;
    second: string;
    third: string;
}

const columns: Column<NavigationRow>[] = [
    { id: "first", field: "first" },
    { id: "second", field: "second" },
    { id: "third", field: "third" }
];

function createGrid(data: NavigationRow[], options: Record<string, unknown> = {}) {
    const container = document.createElement("div");
    container.style.height = "300px";
    document.body.append(container);
    const grid = new SleekGrid(container, data, columns.map(column => ({ ...column })), {
        renderAllRows: true,
        renderAllCells: true,
        ...options
    });
    return { container, grid };
}

describe("SleekGrid selection and navigation", () => {
    const grids: SleekGrid<NavigationRow>[] = [];

    afterEach(() => {
        while (grids.length)
            grids.pop()?.destroy();
    });

    it("moves the active cell in each direction and reaches top and bottom rows", () => {
        const { grid } = createGrid([
            { first: "a", second: "b", third: "c" },
            { first: "d", second: "e", third: "f" },
            { first: "g", second: "h", third: "i" }
        ]);
        grids.push(grid);
        grid.setActiveCell(1, 1);

        expect(grid.navigateRight()).toBe(true);
        expect(grid.getActiveCell()).toEqual({ row: 1, cell: 2 });
        expect(grid.navigateLeft()).toBe(true);
        expect(grid.getActiveCell()).toEqual({ row: 1, cell: 1 });
        expect(grid.navigateDown()).toBe(true);
        expect(grid.getActiveCell()).toEqual({ row: 2, cell: 1 });
        expect(grid.navigateUp()).toBe(true);
        expect(grid.getActiveCell()).toEqual({ row: 1, cell: 1 });

        grid.navigateTop();
        expect(grid.getActiveCell()).toEqual({ row: 0, cell: 1 });
        grid.navigateBottom();
        expect(grid.getActiveCell()).toEqual({ row: 2, cell: 1 });
    });

    it("supports next and previous navigation across row boundaries", () => {
        const { grid } = createGrid([
            { first: "a", second: "b", third: "c" },
            { first: "d", second: "e", third: "f" }
        ]);
        grids.push(grid);

        grid.setActiveCell(1, 0);
        expect(grid.navigatePrev()).toBe(true);
        expect(grid.getActiveCell()).toEqual({ row: 0, cell: 2 });
        expect(grid.navigateNext()).toBe(true);
        expect(grid.getActiveCell()).toEqual({ row: 1, cell: 0 });
    });

    it("returns false at navigation boundaries and when navigation is disabled", () => {
        const { grid } = createGrid([{ first: "a", second: "b", third: "c" }]);
        grids.push(grid);
        grid.setActiveCell(0, 0);

        expect(grid.navigateLeft()).toBe(false);
        expect(grid.navigateUp()).toBe(false);
        grid.setActiveCell(0, 2);
        expect(grid.navigateRight()).toBe(false);
        expect(grid.navigateDown()).toBe(false);

        const disabled = createGrid([{ first: "a", second: "b", third: "c" }], { enableCellNavigation: false });
        grids.push(disabled.grid);
        expect(disabled.grid.navigateNext()).toBe(false);
        expect(disabled.grid.getActiveCell()).toBeNull();
    });

    it("guards active-cell setters and reset behavior", () => {
        const { grid } = createGrid([{ first: "a", second: "b", third: "c" }]);
        grids.push(grid);
        const changed = vi.fn();
        grid.onActiveCellChanged.subscribe(changed);

        grid.setActiveCell(-1, 0);
        grid.setActiveCell(0, 3);
        grid.setActiveRow(2, 0);
        expect(grid.getActiveCell()).toBeNull();

        grid.setActiveCell(0, 1);
        expect(grid.getActiveCellNode()).toBeTruthy();
        grid.setActiveRow(0, 2, true);
        expect(grid.getActiveCell()?.row).toBe(0);
        grid.resetActiveCell();
        expect(grid.getActiveCell()).toBeNull();
        expect(changed).toHaveBeenCalled();
    });

    it("honors row and column focusable and selectable metadata", () => {
        const metadata: ItemMetadata<NavigationRow>[] = [
            { focusable: false, selectable: false },
            { columns: { second: { focusable: false, selectable: false } } },
            { columns: { 0: { focusable: true, tabbable: false } } }
        ];
        const data: IDataView<NavigationRow> = {
            getLength: () => metadata.length,
            getItem: row => ({ first: `f${row}`, second: `s${row}`, third: `t${row}` }),
            getItemMetadata: row => metadata[row],
            getGrandTotals: () => ({})
        };
        const { grid } = createGrid(data as unknown as NavigationRow[]);
        grids.push(grid);

        expect(grid.canCellBeActive(0, 0)).toBe(false);
        expect(grid.canCellBeSelected(0, 0)).toBe(false);
        expect(grid.canCellBeActive(1, 1)).toBe(false);
        expect(grid.canCellBeSelected(1, 1)).toBe(false);
        expect(grid.canCellBeActive(2, 0, true)).toBe(false);
        expect(grid.canCellBeActive(2, 2)).toBe(true);
        expect(grid.canCellBeActive(-1, 0)).toBe(false);
        expect(grid.canCellBeSelected(3, 0)).toBe(false);
    });
});
