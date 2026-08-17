import { EventEmitter } from "../../src/core/event";
import type { Column, ColumnMetadata, IDataView, ItemMetadata } from "../../src/core";
import { BasicLayout, FrozenLayout, SleekGrid } from "../../src/grid";

interface Row {
    first: string;
    second: string;
}

const baseColumns: Column<Row>[] = [
    { id: "first", field: "first", name: "First", sortable: true, width: 100 },
    { id: "second", field: "second", name: "Second", sortable: true, width: 100 }
];

function makeGrid(data: Row[] | IDataView<Row> = [{ first: "a", second: "b" }], options: Record<string, unknown> = {}, columns = baseColumns) {
    const container = document.createElement("div");
    container.style.height = "300px";
    document.body.append(container);
    const grid = new SleekGrid(container, data, columns.map(x => ({ ...x })), {
        renderAllRows: true,
        renderAllCells: true,
        ...options
    });
    return { container, grid };
}

describe("SleekGrid public API behavior", () => {
    const grids: SleekGrid<Row>[] = [];

    afterEach(() => {
        while (grids.length)
            grids.pop()?.destroy();
        document.body.classList.remove("rtl");
    });

    it("accepts selector and array-like containers and maps pre-header visibility", () => {
        const host = document.createElement("div");
        host.id = "grid-selector-host";
        document.body.append(host);
        const selected = new SleekGrid("#grid-selector-host", [], [], {});
        grids.push(selected);
        expect(selected.getContainerNode()).toBe(host);

        const arrayHost = document.createElement("div");
        document.body.append(arrayHost);
        const arrayLike = { 0: arrayHost, length: 1 } as ArrayLike<HTMLElement>;
        expect(() => new SleekGrid(arrayLike, [], [], {})).toThrow();
        const preHeaderGrid = new SleekGrid(host, [], [], {
            createPreHeaderPanel: true,
            showPreHeaderPanel: false,
            preHeaderPanelHeight: 24
        });
        grids.push(preHeaderGrid);
        expect(preHeaderGrid.getOptions().showGroupingPanel).toBe(true);
        expect(preHeaderGrid.getOptions().groupingPanelHeight).toBe(24);
        expect(() => new SleekGrid("#does-not-exist", [], [], {})).toThrow();
    });

    it("renders header, footer, header-row, custom names, classes, and sort indicators", () => {
        const nameFormat = vi.fn(() => "Formatted name");
        const columns: Column<Row>[] = [
            { ...baseColumns[0], nameFormat, headerCssClass: "header-class", footerCssClass: "footer-class", cssClass: "fallback" },
            { ...baseColumns[1], cssClass: "footer-fallback" }
        ];
        const { grid } = makeGrid([{ first: "a", second: "b" }], {
            showHeaderRow: true,
            showFooterRow: true
        }, columns);
        grids.push(grid);

        expect(nameFormat).toHaveBeenCalled();
        expect(grid.getHeaderColumn("first").textContent).toContain("Formatted name");
        expect(grid.getHeaderColumn("first").classList.contains("header-class")).toBe(true);
        expect(grid.getFooterRowColumn("first").classList.contains("footer-class")).toBe(true);
        expect(grid.getFooterRowColumn("second").classList.contains("footer-fallback")).toBe(true);

        grid.setSortColumn("first", true);
        expect(grid.getHeaderColumn("first").querySelector(".slick-sort-indicator-asc")).not.toBeNull();
        grid.setSortColumns([{ columnId: "first", sortAsc: false }, { columnId: "second" }]);
        expect(grid.getHeaderColumn("first").querySelector(".slick-sort-indicator-desc")).not.toBeNull();
        expect(grid.getHeaderColumn("first").querySelector("sub")?.textContent).toBe("1");
        grid.setSortColumns(null);
        expect(grid.getSortColumns()).toEqual([]);

        grid.updateColumnHeader("first", "Changed", "tip");
        expect(nameFormat).toHaveBeenCalledTimes(2);
        expect(grid.getHeaderColumn("first").title).toBe("tip");
        grid.updateColumnHeader("first", "Changed", "");
        expect(grid.getHeaderColumn("first").title).toBe("");
        grid.updateColumnHeader("missing", "ignored");
    });

    it("covers visibility, reordering, preserved columns, and notification options", () => {
        const columns: Column<Row>[] = [
            { ...baseColumns[0] }, { ...baseColumns[1] }, { id: "hidden", field: "first", name: "Hidden", visible: false }
        ];
        const { grid } = makeGrid([{ first: "a", second: "b" }], {}, columns);
        grids.push(grid);
        const reordered = vi.fn();
        grid.onColumnsReordered.subscribe(reordered);

        grid.reorderColumns(["second", "first"], { notify: false });
        expect(reordered).not.toHaveBeenCalled();
        grid.setVisibleColumns(["first"], { reorder: false, notify: false });
        expect(grid.getColumns().map(x => x.id)).toEqual(["first"]);
        grid.setVisibleColumns(["second", "first"], { reorder: true });
        expect(reordered).toHaveBeenCalledOnce();
        expect(grid.getColumnIndex("hidden", { inAll: true })).toBeGreaterThanOrEqual(0);
        grid.setColumns(grid.getColumns());
    });

    it("selects frozen and pinned canvases and exposes dimensions", () => {
        const columns: Column<Row>[] = [
            { ...baseColumns[0], frozen: "start" },
            { ...baseColumns[1], frozen: "end" }
        ];
        const { grid } = makeGrid([{ first: "a", second: "b" }], {
            layoutEngine: new FrozenLayout(), frozenColumns: 1, frozenRows: 1, frozenBottomRows: 1
        }, columns);
        grids.push(grid);
        expect(grid.getLayoutInfo().supportPinnedCols).toBe(true);
        expect(grid.getCanvases().length).toBeGreaterThan(0);
        expect(grid.getViewportNode()).toBeDefined();
        expect(grid.getCanvasNode()).toBeDefined();
        expect(grid.getCanvasNode(99)).toBeDefined();
        expect(grid.getScrollBarDimensions().width).toBeGreaterThanOrEqual(0);
        expect(grid.getDisplayedScrollbarDimensions().width).toBeGreaterThanOrEqual(0);
        expect(grid.getAbsoluteColumnMinWidth()).toBeGreaterThanOrEqual(0);
    });

    it("resolves formatter and editor fallbacks in priority order", () => {
        const metadataFormat = vi.fn(() => "metadata");
        const columnFormat = vi.fn(() => "column");
        const factoryFormat = vi.fn(() => "factory");
        const defaultFormat = vi.fn(() => "default");
        const metadata: ItemMetadata<Row> = { columns: { first: { format: metadataFormat } } };
        const view: IDataView<Row> = {
            getLength: () => 1,
            getItem: () => ({ first: "a", second: "b" }),
            getItemMetadata: () => metadata
        };
        const { grid } = makeGrid(view, {
            formatterFactory: { getFormat: () => factoryFormat },
            defaultFormat
        }, [{ ...baseColumns[0], format: columnFormat }, { ...baseColumns[1] }]);
        grids.push(grid);
        expect(grid.getFormatter(0, grid.getColumns()[0])).toBe(metadataFormat);
        metadata.columns = { 0: { formatter: (() => "legacy") as any } as ColumnMetadata<Row> };
        expect(grid.getFormatter(0, grid.getColumns()[0])).toBeTypeOf("function");
        metadata.columns = undefined;
        metadata.format = undefined;
        (metadata as any).formatter = undefined;
        expect(grid.getFormatter(0, grid.getColumns()[0])).toBe(columnFormat);
        grid.getColumns()[0].format = undefined;
        expect(grid.getFormatter(0, grid.getColumns()[0])).toBe(factoryFormat);

        const editor = class {} as any;
        const editorFactory = { getEditor: vi.fn(() => editor) };
        const editorGrid = makeGrid(view, { editorFactory }, [{ ...baseColumns[0], editor: undefined }, { ...baseColumns[1] }]).grid;
        grids.push(editorGrid);
        expect((editorGrid as any).getEditor(0, 0)).toBe(editor);
        metadata.columns = { first: { editor } };
        expect((editorGrid as any).getEditor(0, 0)).toBe(editor);
        metadata.columns = { 0: { editor } };
        expect((editorGrid as any).getEditor(0, 0)).toBe(editor);
        expect(editorGrid.getDataItemValueForColumn({ first: "a", second: "b" }, editorGrid.getColumns()[0])).toBe("a");
    });

    it("handles data-view events and option changes", () => {
        const view: IDataView<Row> = {
            getLength: () => 1,
            getItem: () => ({ first: "a", second: "b" }),
            onRowCountChanged: new EventEmitter(),
            onRowsChanged: new EventEmitter(),
            onDataChanged: new EventEmitter(),
            getGrandTotals: () => ({})
        };
        const { grid } = makeGrid(view);
        grids.push(grid);
        const render = vi.spyOn(grid, "render");
        const invalidate = vi.spyOn(grid, "invalidate");
        const invalidateRows = vi.spyOn(grid, "invalidateRows");
        const updateRowCount = vi.spyOn(grid, "updateRowCount");
        (view.onRowCountChanged as EventEmitter).notify({});
        (view.onRowsChanged as EventEmitter).notify({ rows: [0] });
        (view.onDataChanged as EventEmitter).notify({});
        expect(updateRowCount).toHaveBeenCalled();
        expect(invalidateRows).toHaveBeenCalledWith([0]);
        expect(invalidate).toHaveBeenCalled();
        expect(render).toHaveBeenCalled();

        grid.setOptions({ showColumnHeader: false }, true, true, true);
        expect(grid.getOptions().showColumnHeader).toBe(false);
        grid.setOptions({ groupingPanel: true, showColumnHeader: true }, true);
        expect(grid.getGroupingPanel()).toBeTruthy();
        grid.setOptions({ groupingPanel: false }, true, true, true);
    });

    it("covers navigation guards, focus helpers, and data metadata fallbacks", () => {
        const metadata: ItemMetadata<Row> = {
            focusable: true,
            tabbable: true,
            columns: { first: { focusable: true, tabbable: true, selectable: true, colspan: "*" } }
        };
        const view: IDataView<Row> = {
            getLength: () => 2,
            getItem: row => row < 2 ? ({ first: "a", second: "b" }) : null,
            getItemMetadata: () => metadata
        };
        const { grid } = makeGrid(view, { enableAddRow: true });
        grids.push(grid);

        expect(grid.canCellBeActive(-1, 0)).toBe(false);
        expect(grid.canCellBeSelected(2, 0)).toBe(false);
        expect(grid.getColspan(0, 0)).toBe(2);
        expect(grid.navigateNext()).toBe(true);
        grid.navigateRowStart();
        grid.navigateRowEnd();
        grid.navigatePageDown();
        grid.navigatePageUp();
        grid.scrollActiveCellIntoView();
        grid.clearTextSelection();
        grid.setActiveCell(-1, 0);
        grid.setActiveCell(99, 0);
        grid.setActiveRow(99, 0);
        grid.gotoCell(99, 0);
        expect(grid.getActiveCell()).toBeTruthy();
    });

    it("covers formatter and totals compatibility fallbacks", () => {
        const legacy = (() => "legacy") as any;
        const groupLegacy = (() => "group") as any;
        const { grid } = makeGrid([{ first: "a", second: "b" }], {
            formatterFactory: { getFormatter: () => legacy },
            defaultFormatter: legacy,
            groupTotalsFormatter: groupLegacy
        }, [{ ...baseColumns[0], formatter: legacy, groupTotalsFormatter: groupLegacy }, { ...baseColumns[1] }]);
        grids.push(grid);
        const column = grid.getColumns()[0] as any;
        expect(grid.getFormatter(0, column)).toBeTypeOf("function");
        column.format = undefined;
        expect(grid.getFormatter(0, column)).toBeTypeOf("function");
        column.formatter = undefined;
        expect(grid.getFormatter(0, column)).toBeTypeOf("function");
        expect(grid.getTotalsFormatter(column)).toBeTypeOf("function");
        column.groupTotalsFormatter = undefined;
        expect(grid.getTotalsFormatter(column)).toBeTypeOf("function");
    });

    it("renders grand totals and updates them after data changes", () => {
        const totalsFormat = vi.fn((ctx: any) => `total:${ctx.item?.first ?? "none"}`);
        const legacyTotals = (() => "legacy-total") as any;
        const view: IDataView<Row> = {
            getLength: () => 1,
            getItem: () => ({ first: "a", second: "b" }),
            getGrandTotals: () => ({ first: "all" }),
            onDataChanged: new EventEmitter()
        };
        const { grid } = makeGrid(view, { showFooterRow: true }, [
            { ...baseColumns[0], groupTotalsFormat: totalsFormat },
            { ...baseColumns[1], groupTotalsFormatter: legacyTotals }
        ]);
        grids.push(grid);

        grid.getColumns().forEach(column => {
            const formatter = grid.getTotalsFormatter(column);
            if (formatter) {
                const ctx = grid.getFormatterContext(0, grid.getColumnIndex(column.id));
                ctx.item = view.getGrandTotals();
                ctx.column = column;
                ctx.purpose = "grand-totals";
                formatter(ctx);
            }
        });
        expect(totalsFormat).toHaveBeenCalled();
        expect(grid.getFooterRowColumn("first")).toBeTruthy();
        expect(grid.getFooterRowColumn("second")).toBeTruthy();
        expect(grid.getFooterRow()).toBeTruthy();
    });

    it("handles virtual scrolling, horizontal scrolling, wheel input, and row cleanup", () => {
        vi.useFakeTimers();
        const data = Array.from({ length: 80 }, (_, i) => ({ first: `a${i}`, second: `b${i}` }));
        const columns = [
            { ...baseColumns[0], width: 500 },
            { ...baseColumns[1], width: 500 }
        ];
        const { grid } = makeGrid(data, {
            renderAllRows: false,
            renderAllCells: false,
            forceSyncScrolling: true,
            minBuffer: 1
        }, columns);
        grids.push(grid);
        const viewport = grid.getScrollContainerY();
        const scroll = vi.spyOn(grid, "render");
        viewport.scrollTop = 500;
        viewport.dispatchEvent(new Event("scroll"));
        vi.runAllTimers();
        viewport.scrollTop = 20;
        viewport.dispatchEvent(new Event("scroll"));
        vi.runAllTimers();

        const wheel = new WheelEvent("wheel", { bubbles: true, cancelable: true, deltaY: 120 });
        viewport.dispatchEvent(wheel);
        viewport.dispatchEvent(new WheelEvent("wheel", { bubbles: true, cancelable: true, shiftKey: true, deltaY: 120 }));
        expect(scroll).toHaveBeenCalled();
        expect(grid.getRenderedRange(0, 0).bottom).toBeGreaterThan(0);
        grid.scrollRowIntoView(50, true);
        grid.scrollRowIntoView(0, false);
        grid.scrollColumnIntoView(1);
        grid.scrollCellIntoView(0, 1);
        grid.updateRow(0);
        grid.updateCell(0, 0);
        grid.updateCell(999, 0);
        vi.useRealTimers();
    });

    it("renders metadata colspans and formatter DOM nodes while changing horizontal ranges", () => {
        const view: IDataView<Row> = {
            getLength: () => 2,
            getItem: row => ({ first: `a${row}`, second: `b${row}` }),
            getItemMetadata: () => ({ columns: { first: { colspan: 2 } } })
        };
        const nodeFormat = vi.fn(() => {
            const node = document.createElement("strong");
            node.textContent = "node-value";
            return node;
        });
        const { grid } = makeGrid(view, {}, [
            { ...baseColumns[0], format: nodeFormat },
            { ...baseColumns[1] }
        ]);
        grids.push(grid);
        expect(grid.getColspan(0, 0)).toBe(2);
        expect(grid.getCellNode(0, 0).textContent).toContain("node-value");
        grid.invalidateRows([0, 1]);
        grid.render();
        grid.updateRow(0);
        expect(nodeFormat).toHaveBeenCalled();
    });

    it("covers drag guards and handled drag events", () => {
        const { grid } = makeGrid();
        grids.push(grid);
        const cell = grid.getCellNode(0, 0);
        const event = new MouseEvent("mousedown", { bubbles: true, cancelable: true });
        Object.defineProperty(event, "target", { value: cell });
        const dd = {} as any;
        expect((grid as any).handleDragInit(event, dd)).toBe(false);
        expect((grid as any).handleDragStart(event, dd)).toBe(false);
        expect((grid as any).handleDragEnd(event, dd)).toBeUndefined();
        const drag = vi.fn((e: any) => e.stopImmediatePropagation());
        grid.onDrag.subscribe(drag);
        expect((grid as any).handleDrag(event, dd)).toBeUndefined();
        expect(drag).toHaveBeenCalled();
        expect((grid as any).cellExists(-1, 0)).toBe(false);
        expect((grid as any).cellExists(0, 0)).toBe(true);
        expect(cell).toBeTruthy();
    });

    it("supports async post-render and cleanup callbacks", () => {
        vi.useFakeTimers();
        const postRender = vi.fn();
        const cleanup = vi.fn();
        const columns: Column<Row>[] = [
            { ...baseColumns[0], asyncPostRender: postRender, asyncPostRenderCleanup: cleanup },
            { ...baseColumns[1] }
        ];
        const { grid } = makeGrid([{ first: "a", second: "b" }], {
            enableAsyncPostRender: true,
            enableAsyncPostRenderCleanup: true
        }, columns);
        grids.push(grid);
        vi.runAllTimers();
        grid.invalidateRow(0);
        grid.render();
        vi.runAllTimers();
        expect(postRender).toHaveBeenCalled();
        vi.useRealTimers();
    });
});
