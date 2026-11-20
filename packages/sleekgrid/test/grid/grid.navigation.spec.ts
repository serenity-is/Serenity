import { afterEach, describe, expect, it, vi } from "vitest";
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

describe("SleekGrid row navigation (enableCellNavigation: \"row\")", () => {
    const grids: SleekGrid<NavigationRow>[] = [];

    afterEach(() => {
        while (grids.length)
            grids.pop()?.destroy();
    });

    const data = () => [
        { first: "a", second: "b", third: "c" },
        { first: "d", second: "e", third: "f" },
        { first: "g", second: "h", third: "i" }
    ];

    function rowGrid() {
        const { container, grid } = createGrid(data(), { enableCellNavigation: "row", enableTabKeyNavigation: true });
        grids.push(grid);
        const key = (k: string, opts: Record<string, unknown> = {}) => {
            const event = new KeyboardEvent("keydown", { key: k, bubbles: true, cancelable: true, ...opts });
            grid.getCanvasNode().dispatchEvent(event);
            return event;
        };
        return { container, grid, key };
    }

    it("marks the container, and unmarks it for the other values", () => {
        const { container, grid } = rowGrid();
        expect(container.classList.contains("slick-row-navigation")).toBe(true);

        grid.setOptions({ enableCellNavigation: true });
        expect(container.classList.contains("slick-row-navigation")).toBe(false);
    });

    it("keeps the active cell API working, as the cursor is the row of the active cell", () => {
        const { grid } = rowGrid();
        grid.setActiveCell(1, 2);
        expect(grid.getActiveCell()).toEqual({ row: 1, cell: 2 });
        expect(grid.canCellBeActive(0, 0)).toBe(true);
    });

    it("moves between the rows with Up and Down, keeping the column", () => {
        const { grid } = rowGrid();
        grid.setActiveCell(1, 1);

        expect(grid.navigateDown()).toBe(true);
        expect(grid.getActiveCell()).toEqual({ row: 2, cell: 1 });
        expect(grid.navigateUp()).toBe(true);
        expect(grid.navigateUp()).toBe(true);
        expect(grid.getActiveCell()).toEqual({ row: 0, cell: 1 });
    });

    it("does not move between the cells of a row", () => {
        const { grid } = rowGrid();
        grid.setActiveCell(1, 1);

        expect(grid.navigateNext()).toBe(false);
        expect(grid.navigatePrev()).toBe(false);
        expect(grid.navigateRowStart()).toBe(false);
        expect(grid.navigateRowEnd()).toBe(false);
        expect(grid.getActiveCell()).toEqual({ row: 1, cell: 1 });

        // Left and Right scroll the columns instead, which is a handled key that leaves the cursor alone
        expect(grid.navigateLeft()).toBe(true);
        expect(grid.navigateRight()).toBe(true);
        expect(grid.getActiveCell()).toEqual({ row: 1, cell: 1 });
    });

    it("leaves Tab to the browser, so it moves the focus out of the grid", () => {
        const { grid, key } = rowGrid();
        grid.setActiveCell(1, 1);

        expect(key("Tab").defaultPrevented).toBe(false);
        expect(key("Tab", { shiftKey: true }).defaultPrevented).toBe(false);
        expect(grid.getActiveCell()).toEqual({ row: 1, cell: 1 });
    });

    it("goes to the first and the last row with Home and End", () => {
        const { grid, key } = rowGrid();
        grid.setActiveCell(1, 1);

        expect(key("End").defaultPrevented).toBe(true);
        expect(grid.getActiveCell()).toEqual({ row: 2, cell: 1 });
        expect(key("Home").defaultPrevented).toBe(true);
        expect(grid.getActiveCell()).toEqual({ row: 0, cell: 1 });
    });

    it("starts the cursor at the first row on Down, or the last row on Up, when no row has it", () => {
        const first = rowGrid();
        expect(first.grid.getActiveCell()).toBeNull();
        expect(first.grid.navigateDown()).toBe(true);
        expect(first.grid.getActiveCell()).toEqual({ row: 0, cell: 0 });

        const last = rowGrid();
        expect(last.grid.navigateUp()).toBe(true);
        expect(last.grid.getActiveCell()).toEqual({ row: 2, cell: 0 });
    });

    it("still moves between the cells, and does not start the cursor on Down, with cell navigation", () => {
        const { grid } = createGrid(data(), { enableCellNavigation: true });
        grids.push(grid);
        expect(grid.navigateDown()).toBe(false);

        grid.setActiveCell(1, 1);
        expect(grid.navigateRight()).toBe(true);
        expect(grid.getActiveCell()).toEqual({ row: 1, cell: 2 });
    });
});
