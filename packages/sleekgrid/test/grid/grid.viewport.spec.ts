import type { Column } from "../../src/core";
import { SleekGrid } from "../../src/grid";

interface ViewRow {
    first: string;
    second: string;
}

const columns: Column<ViewRow>[] = [
    { id: "first", field: "first", width: 120 },
    { id: "second", field: "second", width: 120 }
];

function createGrid(options: Record<string, unknown> = {}) {
    const container = document.createElement("div");
    container.style.height = "100px";
    container.style.width = "180px";
    document.body.append(container);
    const data = Array.from({ length: 20 }, (_, index) => ({ first: `f${index}`, second: `s${index}` }));
    const grid = new SleekGrid(container, data, columns.map(column => ({ ...column })), {
        minBuffer: 1,
        ...options
    });
    const viewport = grid.getScrollContainerY();
    viewport.style.height = "100px";
    viewport.style.width = "180px";
    Object.defineProperty(viewport, "clientHeight", { configurable: true, value: 100 });
    Object.defineProperty(viewport, "clientWidth", { configurable: true, value: 180 });
    return { container, grid, viewport };
}

describe("SleekGrid viewport and scrolling", () => {
    const grids: SleekGrid<ViewRow>[] = [];

    afterEach(() => {
        while (grids.length)
            grids.pop()?.destroy();
    });

    it("exposes the main scroll containers and updates paging state", () => {
        const { grid, viewport } = createGrid();
        grids.push(grid);

        expect(grid.getScrollContainerX()).toBe(viewport);
        expect(grid.getScrollContainerY()).toBe(viewport);
        grid.updatePagingStatusFromView({ pageSize: 10, pageNum: 1, totalPages: 2 });
        grid.scrollRowToTop(5);

        expect(viewport.scrollTop).toBeGreaterThan(0);
    });

    it("scrolls rows and cells into view and emits viewport changes", () => {
        const { grid, viewport } = createGrid();
        grids.push(grid);
        const viewportChanged = vi.fn();
        grid.onViewportChanged.subscribe(viewportChanged);

        grid.scrollRowIntoView(15, true);
        const afterRowScroll = viewport.scrollTop;
        grid.scrollCellIntoView(15, 1, true);
        grid.scrollColumnIntoView(0);

        expect(afterRowScroll).toBeGreaterThan(0);
        expect(viewportChanged).toHaveBeenCalled();
    });

    it("resizes the canvas and updates viewport information", () => {
        const { grid } = createGrid();
        grids.push(grid);
        const render = vi.spyOn(grid, "render");
        const viewportChanged = vi.fn();
        grid.onViewportChanged.subscribe(viewportChanged);

        grid.resizeCanvas();

        expect(render).toHaveBeenCalled();
        expect(grid.getViewport()).toBeDefined();
    });

    it("normalizes RTL visible-range horizontal positions", () => {
        const { grid } = createGrid({ rtl: true });
        grids.push(grid);

        const range = grid.getVisibleRange(0, -40);
        expect(range.leftPx).toBe(40);
        expect(range.rightPx).toBeGreaterThan(range.leftPx);
    });
});
