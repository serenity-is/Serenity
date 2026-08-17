import type { Column } from "../../src/core";
import { SleekGrid } from "../../src/grid";

interface RowData {
    first: string;
    second: string;
}

const columns: Column<RowData>[] = [
    { id: "first", field: "first", name: "First" },
    { id: "second", field: "second", name: "Second" }
];

function createGrid(data: RowData[], options: Record<string, unknown> = {}) {
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

describe("SleekGrid rendering and invalidation", () => {
    const grids: SleekGrid<RowData>[] = [];

    afterEach(() => {
        while (grids.length)
            grids.pop()?.destroy();
    });

    it("renders all rows and cells when requested", () => {
        const data = [
            { first: "a", second: "b" },
            { first: "c", second: "d" },
            { first: "e", second: "f" }
        ];
        const { container, grid } = createGrid(data);
        grids.push(grid);

        expect(container.querySelectorAll(".slick-row")).toHaveLength(3);
        expect(container.querySelectorAll(".slick-cell")).toHaveLength(6);
        expect(grid.getRenderedRange()).toMatchObject({ top: 0, bottom: 2 });
    });

    it("updates a rendered cell through updateCell", () => {
        const data = [{ first: "before", second: "value" }];
        const { grid } = createGrid(data);
        grids.push(grid);
        const cell = grid.getCellNode(0, 0);

        data[0].first = "after";
        grid.updateCell(0, 0);

        expect(cell.textContent).toBe("after");
    });

    it("updates all rendered cells in a row through updateRow", () => {
        const data = [{ first: "before", second: "value" }];
        const { grid } = createGrid(data);
        grids.push(grid);

        data[0].first = "after";
        data[0].second = "changed";
        grid.updateRow(0);

        expect(grid.getCellNode(0, 0)?.textContent).toBe("after");
        expect(grid.getCellNode(0, 1)?.textContent).toBe("changed");
    });

    it("invalidates rows and rerenders the grid", () => {
        const data = [{ first: "before", second: "value" }];
        const { grid } = createGrid(data);
        grids.push(grid);
        const render = vi.spyOn(grid, "render");

        grid.invalidateRow(0);
        grid.invalidateRows([0]);
        grid.invalidateAllRows();
        grid.invalidate();

        expect(render).toHaveBeenCalledOnce();
        expect(grid.getCellNode(0, 0)).toBeTruthy();
    });

    it("clamps visible and rendered ranges to the data and canvas", () => {
        const { grid } = createGrid([
            { first: "a", second: "b" },
            { first: "c", second: "d" }
        ]);
        grids.push(grid);

        expect(grid.getVisibleRange(-100, -20)).toMatchObject({ top: -4, leftPx: -20 });
        expect(grid.getRenderedRange()).toMatchObject({ top: 0, bottom: 1, leftPx: 0 });
    });
});
