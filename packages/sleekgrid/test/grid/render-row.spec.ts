import { renderRow } from "../../src/grid/render-row";
import type { RowRenderArgs } from "../../src/grid/render-args";
import type { CachedRow } from "../../src/grid/internal";

function cols() {
    return [{ id: "c0", field: "c0" }, { id: "c1", field: "c1" }, { id: "c2", field: "c2" }];
}

function makeCachedRow(): CachedRow {
    return { cellColSpans: [], cellNodesByColumnIdx: {}, cellRenderQueue: [], cellRenderContent: [] } as CachedRow;
}

function makeGrid(overrides: any = {}) {
    return {
        getColumns: () => cols(),
        getDataLength: () => 1,
        getOptions: () => ({ addNewRowCssClass: "new-row" }),
        getData: () => ({ getItemMetadata: () => null }),
        getDataItemValueForColumn: (item: any, col: any) => item[col.field],
        getFormatter: () => () => "X",
        ...overrides
    };
}

const noPinned = { frozenBottomFirst: Infinity, frozenTopLast: -1, pinnedStartLast: -1, pinnedEndFirst: Infinity };

function makeArgs(overrides: any = {}) {
    return {
        activeCell: -1,
        activeRow: -1,
        colLeft: [0, 100, 200],
        colRight: [100, 200, 300],
        frozenPinned: noPinned,
        grid: makeGrid(),
        item: { c0: "a", c1: "b", c2: "c" },
        row: 0,
        rtl: false,
        range: { top: 0, bottom: 0, leftPx: 0, rightPx: 1000 },
        sbCenter: [],
        sbEnd: [],
        sbStart: [],
        getRowTop: () => 0,
        cachedRow: makeCachedRow(),
        ...overrides
    } as RowRenderArgs<any>;
}

describe('renderRow', () => {
    it('renders the row wrapper with even class and all cells', () => {
        const args = makeArgs();
        renderRow(args);
        expect(args.sbCenter[0]).toBe('<div class="slick-row even" data-row="0">');
        expect(args.sbCenter[args.sbCenter.length - 1]).toBe("</div>");
        expect(args.sbCenter.join("")).toContain('class="slick-cell l0 r0"');
        expect(args.sbCenter.join("")).toContain('class="slick-cell l1 r1"');
        expect(args.sbCenter.join("")).toContain('class="slick-cell l2 r2"');
    });

    it('marks odd rows with the odd class', () => {
        const args = makeArgs({ row: 1 });
        renderRow(args);
        expect(args.sbCenter[0]).toBe('<div class="slick-row odd" data-row="1">');
    });

    it('marks the active row', () => {
        const args = makeArgs({ activeRow: 0 });
        renderRow(args);
        expect(args.sbCenter[0]).toBe('<div class="slick-row active even" data-row="0">');
    });

    it('marks frozen top rows', () => {
        const frozenPinned = { ...noPinned, frozenTopLast: 0 };
        const args = makeArgs({ frozenPinned, row: 0 });
        renderRow(args);
        expect(args.sbCenter[0]).toBe('<div class="slick-row frozen even" data-row="0">');
    });

    it('marks frozen bottom rows', () => {
        const frozenPinned = { ...noPinned, frozenBottomFirst: 1 };
        const args = makeArgs({ frozenPinned, row: 1 });
        renderRow(args);
        expect(args.sbCenter[0]).toBe('<div class="slick-row frozen odd" data-row="1">');
    });

    it('marks loading rows when the item is missing', () => {
        const args = makeArgs({ item: undefined });
        renderRow(args);
        expect(args.sbCenter[0]).toBe('<div class="slick-row loading even" data-row="0">');
    });

    it('adds the add-new-row css class for negative rows', () => {
        const args = makeArgs({ item: null, row: -1, activeRow: -2, frozenPinned: { ...noPinned, frozenTopLast: -2 } });
        renderRow(args);
        expect(args.sbCenter[0]).toBe('<div class="slick-row loading even new-row" data-row="-1">');
    });

    it('appends item metadata cssClasses', () => {
        const grid = makeGrid({
            getData: () => ({ getItemMetadata: () => ({ cssClasses: "meta-row" }) })
        });
        const args = makeArgs({ grid });
        renderRow(args);
        expect(args.sbCenter[0]).toContain("slick-row even meta-row");
    });

    it('pushes the row wrapper into sbStart when there are pinned-start columns', () => {
        const frozenPinned = { ...noPinned, pinnedStartLast: 0 };
        const args = makeArgs({ frozenPinned });
        renderRow(args);
        expect(args.sbStart[0]).toContain('class="slick-row even"');
        expect(args.sbStart[args.sbStart.length - 1]).toBe("</div>");
    });

    it('pushes the row wrapper into sbEnd when there are pinned-end columns', () => {
        const frozenPinned = { ...noPinned, pinnedEndFirst: 2 };
        const args = makeArgs({ frozenPinned });
        renderRow(args);
        expect(args.sbEnd[0]).toContain('class="slick-row even"');
        expect(args.sbEnd[args.sbEnd.length - 1]).toBe("</div>");
    });

    it('renders pinned-start and pinned-end cells into their own builders', () => {
        const frozenPinned = { ...noPinned, pinnedStartLast: 0, pinnedEndFirst: 2 };
        const args = makeArgs({ frozenPinned, colLeft: [0, 100, 0], colRight: [100, 200, 100] });
        renderRow(args);
        expect(args.sbStart.join("")).toContain('class="slick-cell l0 r0');
        expect(args.sbEnd.join("")).toContain('class="slick-cell l2 r2');
        expect(args.sbCenter.join("")).toContain('class="slick-cell l1 r1');
    });

    it('respects a metadata colspan on a column', () => {
        const grid = makeGrid({
            getData: () => ({ getItemMetadata: () => ({ columns: { c0: { colspan: 2 } } }) })
        });
        const args = makeArgs({ grid });
        renderRow(args);
        expect(args.cachedRow.cellColSpans[0]).toBe(2);
    });

    it('resolves a wildcard colspan to the remaining column count', () => {
        const grid = makeGrid({
            getData: () => ({ getItemMetadata: () => ({ columns: { c0: { colspan: "*" } } }) })
        });
        const args = makeArgs({ grid });
        renderRow(args);
        expect(args.cachedRow.cellColSpans[0]).toBe(3);
    });

    it('skips columns outside the horizontal viewport when there are no pinned-end columns', () => {
        const args = makeArgs({ range: { top: 0, bottom: 0, leftPx: 0, rightPx: 50 } });
        renderRow(args);
        const html = args.sbCenter.join("");
        expect(html).toContain('class="slick-cell l0 r0"');
        expect(html).not.toContain('class="slick-cell l1');
        expect(html).not.toContain('class="slick-cell l2');
    });

    it('still renders pinned-end columns when a center column is off-screen to the right (bug fix)', () => {
        const frozenPinned = { ...noPinned, pinnedEndFirst: 2 };
        const args = makeArgs({
            frozenPinned,
            colLeft: [0, 100, 0],
            colRight: [100, 200, 100],
            range: { top: 0, bottom: 0, leftPx: 0, rightPx: 50 }
        });
        renderRow(args);
        // center c1 (colLeft 100 > 50) is skipped, but pinned-end c2 must render
        expect(args.sbCenter.join("")).toContain('class="slick-cell l0 r0"');
        expect(args.sbCenter.join("")).not.toContain('class="slick-cell l1');
        expect(args.sbEnd.join("")).toContain('class="slick-cell l2 r2');
    });

    it('does not render the same cell twice when it is pinned and in range', () => {
        const frozenPinned = { ...noPinned, pinnedStartLast: 0 };
        const args = makeArgs({ frozenPinned, colLeft: [0, 100, 200], colRight: [100, 200, 300] });
        renderRow(args);
        const start = args.sbStart.join("");
        const center = args.sbCenter.join("");
        expect(start).toContain('slick-cell l0 r0');
        expect(center).not.toContain('slick-cell l0');
        expect(center).toContain('slick-cell l1 r1');
    });
});
