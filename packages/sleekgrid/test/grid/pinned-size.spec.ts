import { afterEach, describe, expect, it } from "vitest";
import { SleekGrid } from "../../src/grid";
import type { Column } from "../../src/core";
import { FrozenLayout } from "../../src/layouts/frozen-layout";

describe("the pinned columns of a grid that changes size", () => {
    const grids: SleekGrid<any>[] = [];

    afterEach(() => {
        while (grids.length)
            grids.pop()?.destroy();
        document.body.replaceChildren();
    });

    function makeGrid(width: number) {
        const host = document.createElement("div");
        host.style.height = "300px";
        host.style.width = width + "px";
        document.body.append(host);
        const cols: Column[] = [
            { id: "a", field: "a", name: "A", width: 200, frozen: true },
            { id: "b", field: "b", name: "B", width: 120 },
            { id: "c", field: "c", name: "C", width: 120 },
            { id: "d", field: "d", name: "D", width: 120 }
        ];
        const grid = new SleekGrid(host, [{ a: 1, b: 2, c: 3, d: 4 }], cols, { renderAllRows: true, layoutEngine: () => new FrozenLayout() });
        grids.push(grid);
        return { grid, host };
    }

    it("are limited by the width of the grid, as it is when the columns are set", () => {
        const { grid } = makeGrid(250);
        grid.resizeCanvas();

        // 200 + 120 + 120 + 120 do not fit in 250, so the main band would be left without room for the columns it has to show
        expect(grid.getLayoutInfo().pinnedStartCols).toBe(0);
    });

    it("come back when the grid is sized again, in one go, e.g. when it is shown after being hidden", () => {
        const { grid, host } = makeGrid(250);
        grid.resizeCanvas();
        expect(grid.getLayoutInfo().pinnedStartCols).toBe(0);

        // the width is measured before the pinned columns are limited, so one resize is enough to pin them again
        host.style.width = "1200px";
        grid.resizeCanvas();

        expect(grid.getLayoutInfo().pinnedStartCols).toBe(1);
        expect((grid as any).getViewportInfo().width).toBe(1200);
    });
});
