import { afterEach, describe, expect, it, vi } from "vitest";
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
        // each row also has the cell that fills the rest of its width, which is no column's
        expect(container.querySelectorAll(".slick-cell:not(.sg-fill)")).toHaveLength(6);
        expect(container.querySelectorAll(".slick-cell.sg-fill")).toHaveLength(3);
        expect(grid.getRenderedRange()).toMatchObject({ top: 0, bottom: 2 });
    });

    it("fills each row with a last cell that belongs to no column, unless fillRows is off", () => {
        const data = [{ first: "a", second: "b" }, { first: "c", second: "d" }];
        const { container, grid } = createGrid(data);
        grids.push(grid);

        expect(container.classList.contains("sg-fill-rows")).toBe(true);
        const row = container.querySelector<HTMLElement>(".slick-row");
        const filler = row.lastElementChild as HTMLElement;
        expect(filler.classList.contains("sg-fill")).toBe(true);
        expect(filler.getAttribute("aria-hidden")).toBe("true");
        // the cells of the row are still the ones of the columns
        expect(grid.getCellNode(0, 0).textContent).toBe("a");
        expect(grid.getCellNode(0, 1).textContent).toBe("b");
        expect(grid.getCellFromEvent({ target: filler })).toBeNull();

        grid.setOptions({ fillRows: false });
        grid.render();
        expect(container.classList.contains("sg-fill-rows")).toBe(false);
        expect(container.querySelectorAll(".slick-cell.sg-fill")).toHaveLength(0);
        expect(grid.getCellNode(1, 1).textContent).toBe("d");
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
