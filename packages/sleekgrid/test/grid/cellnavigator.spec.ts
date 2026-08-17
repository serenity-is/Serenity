import { CellNavigator, type CellNavigatorHost } from "../../src/grid/cellnavigator";

interface HostOptions {
    focusable?: boolean[][];
    tabbable?: boolean[][];
    spans?: number[][];
    rtl?: boolean;
}

function createHost(rowCount: number, columnCount: number, options: HostOptions = {}): CellNavigatorHost & { tabbingDirection: number } {
    return {
        tabbingDirection: 0,
        getColumnCount: () => columnCount,
        getRowCount: () => rowCount,
        getColspan: (row, cell) => options.spans?.[row]?.[cell] ?? 1,
        canCellBeActive: (row, cell, tab) => tab
            ? options.tabbable?.[row]?.[cell] ?? options.focusable?.[row]?.[cell] ?? true
            : options.focusable?.[row]?.[cell] ?? true,
        setTabbingDirection(direction) {
            this.tabbingDirection = direction;
        },
        isRTL: () => options.rtl ?? false
    };
}

describe("CellNavigator", () => {
    it("navigates horizontally, vertically, and to row boundaries", () => {
        const host = createHost(3, 3, {
            focusable: [
                [true, false, true],
                [true, true, true],
                [false, true, true]
            ]
        });
        const navigator = new CellNavigator(host);

        expect(navigator.navigate("right", 0, 0, 0)).toEqual({ row: 0, cell: 2, posX: 2 });
        expect(host.tabbingDirection).toBe(1);
        expect(navigator.navigate("left", 0, 2, 2)).toEqual({ row: 0, cell: 0, posX: 0 });
        expect(host.tabbingDirection).toBe(-1);
        expect(navigator.navigate("down", 0, 0, 0)).toEqual({ row: 1, cell: 0, posX: 0 });
        expect(navigator.navigate("up", 2, 1, 1)).toEqual({ row: 1, cell: 1, posX: 1 });
        expect(navigator.navigate("home", 0, 1, 1)).toEqual({ row: 0, cell: 0, posX: 0 });
        expect(navigator.navigate("end", 0, 0, 0)).toEqual({ row: 0, cell: 2, posX: 2 });
    });

    it("skips cells covered by colspans and inaccessible cells", () => {
        const host = createHost(2, 4, {
            focusable: [
                [true, false, false, true],
                [false, true, false, false]
            ],
            spans: [
                [2, 1, 1, 1],
                [1, 2, 1, 1]
            ]
        });
        const navigator = new CellNavigator(host);

        expect(navigator.navigate("right", 0, 0, 0)).toEqual({ row: 0, cell: 3, posX: 3 });
        expect(navigator.navigate("down", 0, 0, 1)).toEqual({ row: 1, cell: 1, posX: 1 });
        expect(navigator.navigate("home", 1, 0, 0)).toEqual({ row: 1, cell: 1, posX: 1 });
        expect(navigator.navigate("end", 1, 0, 0)).toEqual({ row: 1, cell: 1, posX: 1 });
    });

    it("handles next and previous traversal including initial focus", () => {
        const host = createHost(2, 3, {
            focusable: [[true, false, true], [false, true, true]],
            tabbable: [[true, false, false], [false, true, true]]
        });
        const navigator = new CellNavigator(host);

        expect(navigator.navigate("next", null, null, null)).toEqual({ row: 0, cell: 0, posX: 0 });
        expect(host.tabbingDirection).toBe(1);
        expect(navigator.navigate("next", 0, 0, 0)).toEqual({ row: 1, cell: 1, posX: 1 });
        expect(navigator.navigate("prev", null, null, null)).toEqual({ row: 1, cell: 2, posX: 2 });
        expect(host.tabbingDirection).toBe(-1);
        expect(navigator.navigate("prev", 1, 1, 1)).toEqual({ row: 0, cell: 0, posX: 0 });
    });

    it("returns null at navigation boundaries and maps RTL directions", () => {
        const host = createHost(1, 2, { rtl: true });
        const navigator = new CellNavigator(host);

        expect(navigator.navigate("left", 0, 0, 0)).toEqual({ row: 0, cell: 1, posX: 1 });
        expect(host.tabbingDirection).toBe(1);
        expect(navigator.navigate("right", 0, 1, 1)).toEqual({ row: 0, cell: 0, posX: 0 });
        expect(host.tabbingDirection).toBe(-1);
        expect(navigator.navigate("up", 0, 0, 0)).toBeNull();
        expect(navigator.navigate("down", 0, 1, 1)).toBeNull();
        expect(navigator.navigate("next", 0, 1, 1)).toBeNull();
        expect(navigator.navigate("prev", 0, 0, 0)).toBeNull();
    });
});
