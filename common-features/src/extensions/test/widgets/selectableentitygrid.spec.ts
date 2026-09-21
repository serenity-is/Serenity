import { mockFetch, unmockFetch } from "test-utils";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { SelectableEntityGrid } from "../../Modules/Widgets/SelectableEntityGrid";

describe("SelectableEntityGrid", () => {
    beforeEach(() => {
        mockFetch({ "*": () => ({}) });
    });

    afterEach(() => {
        unmockFetch();
        vi.restoreAllMocks();
    });

    it("enables text selection and cell navigation in slick options", () => {
        const grid = new SelectableEntityGrid({});
        const opt = grid["getSlickOptions"]();
        expect(opt.enableTextSelectionOnCells).toBe(true);
        expect(opt.selectedCellCssClass).toBe("slick-row-selected");
        expect(opt.enableCellNavigation).toBe(true);
        grid.destroy();
    });

    it("sets a row selection model on the sleek grid", () => {
        const grid = new SelectableEntityGrid({});
        const setSelectionModelSpy = vi.spyOn(grid.sleekGrid, "setSelectionModel");
        grid["initSleekGrid"]();
        expect(setSelectionModelSpy).toHaveBeenCalledTimes(1);
        grid.destroy();
    });
});
