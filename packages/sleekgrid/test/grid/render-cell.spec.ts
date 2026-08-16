import { renderCell } from "../../src/grid/render-cell";
import type { CellRenderArgs } from "../../src/grid/render-args";
import type { CachedRow } from "../../src/grid/internal";

function makeCachedRow(): CachedRow {
    return { cellColSpans: [], cellNodesByColumnIdx: {}, cellRenderQueue: [], cellRenderContent: [] } as CachedRow;
}

function makeGrid(overrides: any = {}) {
    return {
        getColumns: () => [{ id: "c0", field: "c0", cssClass: "" }],
        getDataItemValueForColumn: (item: any, col: any) => item[col.field],
        getFormatter: () => (_ctx: any) => "value",
        ...overrides
    };
}

const noPinned = { frozenBottomFirst: Infinity, frozenTopLast: -1, pinnedStartLast: -1, pinnedEndFirst: Infinity };

function makeArgs(overrides: any = {}) {
    const cachedRow = makeCachedRow();
    return {
        activeCell: -1,
        activeRow: -1,
        cell: 0,
        cellCssClasses: {},
        colMetadata: null,
        colspan: 1,
        grid: makeGrid(),
        item: { c0: "v" },
        row: 0,
        rtl: false,
        frozenPinned: noPinned,
        cachedRow,
        sb: [],
        ...overrides
    } as CellRenderArgs<any>;
}

describe('renderCell', () => {
    it('renders a basic cell with the formatter text', () => {
        const args = makeArgs();
        renderCell(args);
        expect(args.sb.join("")).toBe('<div class="slick-cell l0 r0">value</div>');
        expect(args.cachedRow.cellColSpans[0]).toBe(1);
        expect(args.cachedRow.cellRenderQueue).toEqual([0]);
        expect(args.cachedRow.cellRenderContent).toEqual([undefined]);
    });

    it('uses rtl class ordering when rtl is enabled', () => {
        const args = makeArgs({ rtl: true });
        renderCell(args);
        expect(args.sb.join("")).toContain('class="slick-cell r0 l0"');
    });

    it('computes the last cell index from the colspan', () => {
        const grid = makeGrid({
            getColumns: () => [{ id: "c0" }, { id: "c1" }, { id: "c2" }, { id: "c3" }]
        });
        const args = makeArgs({ grid, cell: 1, colspan: 2 });
        renderCell(args);
        expect(args.sb.join("")).toContain('class="slick-cell l1 r2"');
    });

    it('appends the column cssClass', () => {
        const grid = makeGrid({
            getColumns: () => [{ id: "c0", field: "c0", cssClass: "my-css" }]
        });
        const args = makeArgs({ grid });
        renderCell(args);
        expect(args.sb.join("")).toContain('slick-cell l0 r0 my-css');
    });

    it('marks pinned-start cells as frozen pinned-start', () => {
        const frozenPinned = { ...noPinned, pinnedStartLast: 0 };
        const args = makeArgs({ frozenPinned, cell: 0 });
        renderCell(args);
        expect(args.sb.join("")).toContain('slick-cell l0 r0 frozen pinned-start');
    });

    it('marks pinned-end cells as frozen pinned-end', () => {
        const frozenPinned = { ...noPinned, pinnedEndFirst: 0 };
        const args = makeArgs({ frozenPinned, cell: 0 });
        renderCell(args);
        expect(args.sb.join("")).toContain('slick-cell l0 r0 frozen pinned-end');
    });

    it('marks the active cell', () => {
        const args = makeArgs({ activeCell: 0, activeRow: 0 });
        renderCell(args);
        expect(args.sb.join("")).toContain('slick-cell l0 r0 active');
    });

    it('appends the column metadata cssClass', () => {
        const args = makeArgs({ colMetadata: { cssClass: "meta-cls" } });
        renderCell(args);
        expect(args.sb.join("")).toContain(' meta-cls');
    });

    it('appends css classes from the cellCssClasses hash', () => {
        const cellCssClasses = { fmt: { 0: { c0: "fmt-cls" } } };
        const args = makeArgs({ cellCssClasses });
        renderCell(args);
        expect(args.sb.join("")).toContain(' fmt-cls');
    });

    it('renders an empty div when the formatter returns a Node and stores it in content', () => {
        const el = document.createElement("span");
        const grid = makeGrid({ getFormatter: () => () => el });
        const args = makeArgs({ grid });
        renderCell(args);
        expect(args.sb.join("")).toBe('<div class="slick-cell l0 r0"></div>');
        expect(args.cachedRow.cellRenderContent[0]).toBe(el);
    });

    it('renders an empty div for a DocumentFragment result and stores it', () => {
        const frag = document.createDocumentFragment();
        frag.appendChild(document.createElement("i"));
        const grid = makeGrid({ getFormatter: () => () => frag });
        const args = makeArgs({ grid });
        renderCell(args);
        expect(args.sb.join("")).toBe('<div class="slick-cell l0 r0"></div>');
        expect(args.cachedRow.cellRenderContent[0]).toBe(frag);
    });

    it('renders an empty div when the formatter returns undefined', () => {
        const grid = makeGrid({ getFormatter: () => () => undefined });
        const args = makeArgs({ grid });
        renderCell(args);
        expect(args.sb.join("")).toBe('<div class="slick-cell l0 r0"></div>');
        expect(args.cachedRow.cellRenderContent[0]).toBeUndefined();
    });

    it('escapes a string result when html rendering is disabled', () => {
        const grid = makeGrid({
            getFormatter: () => () => "<b>bold</b>"
        });
        const args = makeArgs({ grid });
        renderCell(args);
        expect(args.sb.join("")).toContain("&lt;b&gt;bold&lt;/b&gt;");
    });

    it('sanitizes a string result when html rendering is enabled', () => {
        const grid = makeGrid({
            getOptions: () => ({ enableHtmlRendering: true, sanitizer: (s: string) => "SAN:" + s }),
            getFormatter: () => () => "<b>bold</b>"
        });
        const args = makeArgs({ grid });
        renderCell(args);
        // sanitizer output is inserted as raw html
        expect(args.sb.join("")).toContain("SAN:<b>bold</b>");
    });

    it('renders addClass, addAttrs and tooltip from the formatter context', () => {
        const grid = makeGrid({
            getFormatter: () => (ctx: any) => {
                ctx.addClass = "cls-a";
                ctx.addAttrs = { title: "t1", "data-x": "2" };
                ctx.tooltip = "tip";
                return "val";
            }
        });
        const args = makeArgs({ grid });
        renderCell(args);
        const html = args.sb.join("");
        expect(html).toContain('slick-cell l0 r0 cls-a');
        expect(html).toContain('data-fmtcls="cls-a"');
        expect(html).toContain('title="t1"');
        expect(html).toContain('data-x="2"');
        expect(html).toContain('data-fmtatt="title,data-x"');
        expect(html).toContain('tooltip="tip"');
    });

    it('renders addAttrs even when addClass and tooltip are absent (bug fix)', () => {
        const grid = makeGrid({
            getFormatter: () => (ctx: any) => {
                ctx.addAttrs = { title: "hello" };
                return "val";
            }
        });
        const args = makeArgs({ grid });
        renderCell(args);
        const html = args.sb.join("");
        expect(html).toContain('title="hello"');
        expect(html).toContain('data-fmtatt="title"');
    });

    it('renders addClass without addAttrs/tooltip', () => {
        const grid = makeGrid({
            getFormatter: () => (ctx: any) => {
                ctx.addClass = "only-cls";
                return "val";
            }
        });
        const args = makeArgs({ grid });
        renderCell(args);
        const html = args.sb.join("");
        expect(html).toContain('only-cls');
        expect(html).toContain('data-fmtcls="only-cls"');
        expect(html).not.toContain('data-fmtatt');
    });

    it('renders a tooltip without addClass/addAttrs', () => {
        const grid = makeGrid({
            getFormatter: () => (ctx: any) => {
                ctx.tooltip = "only-tip";
                return "val";
            }
        });
        const args = makeArgs({ grid });
        renderCell(args);
        expect(args.sb.join("")).toContain('tooltip="only-tip"');
    });

    it('does not call the formatter when item is null (add new row)', () => {
        const formatter = vi.fn(() => "should-not-appear");
        const grid = makeGrid({ getFormatter: () => formatter });
        const args = makeArgs({ grid, item: null });
        renderCell(args);
        expect(formatter).not.toHaveBeenCalled();
        expect(args.sb.join("")).toBe('<div class="slick-cell l0 r0"></div>');
    });
});
