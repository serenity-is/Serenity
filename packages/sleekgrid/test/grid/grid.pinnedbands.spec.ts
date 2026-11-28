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

describe("the active row highlight", () => {
    it("is off by default, and the container tells when it is asked for", () => {
        const { el, grid } = createGrid();

        expect(el.classList.contains("slick-highlight-active-row")).toBe(false);

        grid.setOptions({ highlightActiveRow: true });
        expect(el.classList.contains("slick-highlight-active-row")).toBe(true);

        grid.setOptions({ highlightActiveRow: false });
        expect(el.classList.contains("slick-highlight-active-row")).toBe(false);
    });
});

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

describe("the vertical scrollbar after the columns pinned to the end", () => {
    // a container in a test has no size, so the grid can not pin columns, or tell whether it scrolls, on its own
    function withPinnedEnd(grid: SleekGrid, pinnedEndFirst: number, hasVScroll: boolean, barWidth: number) {
        const g = grid as any;
        g._refs = Object.assign(Object.create(Object.getPrototypeOf(g._refs)), g._refs, { pinnedEndFirst });
        g._viewportInfo.hasVScroll = hasVScroll;
        g._scrollDims = { width: barWidth, height: barWidth };
        return g.updateEndPaneScrollbar() as boolean;
    }

    it("is in the end pane of a grid with columns pinned to the end, which the main pane reaches under", () => {
        const { el, grid } = createGrid();

        expect(withPinnedEnd(grid, 2, true, 15)).toBe(true);
        expect(el.classList.contains("sg-vbar-end")).toBe(true);
        expect(el.style.getPropertyValue("--sg-vbar-hide")).toBe("15px");
        expect(el.style.getPropertyValue("--sg-vbar-pad")).toBe("0px");
        expect(el.style.getPropertyValue("--sg-fill-trim")).toBe("15px");
        // nothing changed, so the panes need no measuring again
        expect(withPinnedEnd(grid, 2, true, 15)).toBe(false);

        // without a vertical scroll, there is no scrollbar to cover
        withPinnedEnd(grid, 2, false, 15);
        expect(el.style.getPropertyValue("--sg-vbar-hide")).toBe("0px");
    });

    it("widens the canvas of the main pane for an overlay scrollbar, which takes no room", () => {
        const { el, grid } = createGrid();
        withPinnedEnd(grid, 2, true, 0);
        expect(el.style.getPropertyValue("--sg-vbar-hide")).toBe("12px");
        expect(el.style.getPropertyValue("--sg-vbar-pad")).toBe("12px");
    });

    it("stays in the main pane without columns pinned to the end, with autoHeight, or with the option off", () => {
        const { el, grid } = createGrid();
        withPinnedEnd(grid, Infinity, true, 15);
        expect(el.classList.contains("sg-vbar-end")).toBe(false);
        // the scrollbar of the main pane is left out of the width of the cell that fills the rows
        expect(el.style.getPropertyValue("--sg-fill-trim")).toBe("15px");
        withPinnedEnd(grid, Infinity, false, 15);
        expect(el.style.getPropertyValue("--sg-fill-trim")).toBe("0px");

        withPinnedEnd(grid, 2, true, 15);
        expect(el.classList.contains("sg-vbar-end")).toBe(true);
        grid.setOptions({ endPaneScrollbar: false });
        withPinnedEnd(grid, 2, true, 15);
        expect(el.classList.contains("sg-vbar-end")).toBe(false);

        grid.setOptions({ endPaneScrollbar: true, autoHeight: true });
        withPinnedEnd(grid, 2, true, 15);
        expect(el.classList.contains("sg-vbar-end")).toBe(false);
    });

    it("scrolls the main pane when a pinned pane is scrolled, e.g. with the scrollbar of the end pane", () => {
        const { grid } = createGrid();
        const g = grid as any;
        const main = grid.getScrollContainerY();
        // the pinned panes of a grid in a test, as it pins no columns without a size, are whichever it made
        const end = (g.getViewports() as HTMLElement[]).find(vp => vp !== main && vp.classList.contains("sg-body"));
        expect(end).toBeTruthy();
        expect(end).not.toBe(main);

        end.scrollTop = 40;
        end.dispatchEvent(new Event("scroll"));
        expect(main.scrollTop).toBe(40);

        // a position the grid gave the pane itself is not followed back
        g._scrollTop = 40;
        main.scrollTop = 80;
        end.scrollTop = 40;
        end.dispatchEvent(new Event("scroll"));
        expect(main.scrollTop).toBe(80);
    });
});

describe("the scroll edges of the container", () => {
    it("tells whether the main pane is scrolled from its start and whether there is more towards its end", () => {
        const { el, grid } = createGrid();
        const main = grid.getScrollContainerX();
        const set = (left: number, client: number, width: number) => {
            Object.defineProperty(main, "scrollLeft", { configurable: true, value: left });
            Object.defineProperty(main, "clientWidth", { configurable: true, value: client });
            Object.defineProperty(main, "scrollWidth", { configurable: true, value: width });
            (grid as any).updateScrollEdges();
        };

        set(0, 100, 300);
        expect(el.classList.contains("sg-scrolled-start")).toBe(false);
        expect(el.classList.contains("sg-scrolled-end")).toBe(true);

        set(100, 100, 300);
        expect(el.classList.contains("sg-scrolled-start")).toBe(true);
        expect(el.classList.contains("sg-scrolled-end")).toBe(true);

        set(200, 100, 300);
        expect(el.classList.contains("sg-scrolled-end")).toBe(false);

        // a right to left grid scrolls with negative positions
        set(-200, 100, 300);
        expect(el.classList.contains("sg-scrolled-start")).toBe(true);
        expect(el.classList.contains("sg-scrolled-end")).toBe(false);

        set(0, 300, 300);
        expect(el.classList.contains("sg-scrolled-start")).toBe(false);
        expect(el.classList.contains("sg-scrolled-end")).toBe(false);
    });
});
