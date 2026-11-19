import { describe, expect, it } from "vitest";
import { Column } from "../../src/core/column";
import { SleekGrid } from "../../src/grid/sleekgrid";
import { FrozenLayout } from "../../src/layouts/frozen-layout";

function columns(): Column[] {
    return [{ id: "c1", field: "c1" }, { id: "c2", field: "c2" }, { id: "c3", field: "c3" }];
}

function createGrid() {
    const el = document.body.appendChild(document.createElement("div"));
    const grid = new SleekGrid(el, [], columns(), {
        enableColumnReorder: false,
        layoutEngine: new FrozenLayout()
    });
    return { el, grid, signals: (grid as any)._signals };
}

describe("the frozen and pinned parts of the container", () => {
    it("tells whether the pinned bands and the frozen panes are there, as the layout pins and freezes", () => {
        // the columns are pinned by the layout, from their size, so the counts are set on its signals here,
        // as a container in a test has no size
        const { el, signals } = createGrid();

        expect(el.classList.contains("slick-container")).toBe(true);
        expect(el.classList.contains("sg-has-start")).toBe(false);
        expect(el.classList.contains("sg-has-end")).toBe(false);

        signals.pinnedStartCols.value = 2;
        expect(el.classList.contains("sg-has-start")).toBe(true);
        expect(el.classList.contains("sg-has-end")).toBe(false);

        signals.pinnedEndCols.value = 1;
        expect(el.classList.contains("sg-has-end")).toBe(true);

        signals.pinnedStartCols.value = 0;
        expect(el.classList.contains("sg-has-start")).toBe(false);
        expect(el.classList.contains("sg-has-end")).toBe(true);

        signals.pinnedEndCols.value = 0;
        expect(el.classList.contains("sg-has-end")).toBe(false);
    });

    it("tells whether the top and bottom frozen panes are there", () => {
        const { el, signals } = createGrid();

        expect(el.classList.contains("sg-has-top")).toBe(false);
        expect(el.classList.contains("sg-has-bottom")).toBe(false);

        signals.frozenTopRows.value = 2;
        expect(el.classList.contains("sg-has-top")).toBe(true);
        expect(el.classList.contains("sg-has-bottom")).toBe(false);

        signals.frozenBottomRows.value = 1;
        expect(el.classList.contains("sg-has-bottom")).toBe(true);

        signals.frozenTopRows.value = 0;
        expect(el.classList.contains("sg-has-top")).toBe(false);
        expect(el.classList.contains("sg-has-bottom")).toBe(true);
    });
});
