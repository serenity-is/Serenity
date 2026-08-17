import { CellRange, EventEmitter, type Column, type Editor, type EditorClass, type EditorOptions, type GridPlugin, type IDataView, type ItemMetadata, type SelectionModel, type ValidationResult } from "../../src/core";
import { SleekGrid } from "../../src/grid";

interface Row {
    first: string;
    second: string;
}

const baseColumns: Column<Row>[] = [
    { id: "first", field: "first", name: "First", sortable: true, width: 100 },
    { id: "second", field: "second", name: "Second", sortable: true, width: 100 }
];

class TestEditor implements Editor {
    static validation: ValidationResult = { valid: true };
    readonly input: HTMLInputElement;
    private originalValue = "";
    private item: Row;
    constructor(options: EditorOptions) {
        this.input = document.createElement("input");
        options.container?.append(this.input);
    }
    destroy(): void { this.input.remove(); }
    loadValue(item: Row): void { this.item = item; this.originalValue = item.first; this.input.value = item.first; }
    serializeValue(): string { return this.input.value; }
    applyValue(item: Row, value: string): void { item.first = value; }
    isValueChanged(): boolean { return this.input.value !== this.originalValue; }
    validate(): ValidationResult { return TestEditor.validation; }
    focus(): void { this.input.focus(); }
}
const editorClass = TestEditor as unknown as EditorClass;

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

describe("SleekGrid interaction behavior", () => {
    const grids: SleekGrid<Row>[] = [];

    afterEach(() => {
        while (grids.length)
            grids.pop()?.destroy();
        document.body.classList.remove("rtl");
    });

    it("covers pre-header panel fallbacks and ltr class", () => {
        const host = document.createElement("div");
        document.body.append(host);
        const grid = new SleekGrid(host, [], [], {
            createPreHeaderPanel: true,
            preHeaderPanelHeight: 30,
            showPreHeaderPanel: true
        });
        grids.push(grid);
        expect(grid.getOptions().groupingPanelHeight).toBe(30);
        expect(grid.getOptions().showGroupingPanel).toBe(true);
        expect(host.classList.contains("ltr")).toBe(true);
    });

    it("binds wheel handlers on webkit macintosh", () => {
        const original = navigator.userAgent;
        Object.defineProperty(navigator, "userAgent", {
            value: "Mozilla/5.0 (Macintosh; Intel Mac OS X) AppleWebKit/537.36",
            configurable: true
        });
        const { grid } = makeGrid();
        grids.push(grid);
        expect(grid.getContainerNode()).toBeTruthy();
        Object.defineProperty(navigator, "userAgent", { value: original, configurable: true });
    });

    it("sets and replaces selection models", () => {
        const { grid } = makeGrid();
        grids.push(grid);
        const makeModel = (): SelectionModel => ({
            init: () => { },
            setSelectedRanges: () => { },
            refreshSelections: () => { },
            onSelectedRangesChanged: new EventEmitter<CellRange[]>()
        });
        grid.setSelectionModel(makeModel());
        grid.setSelectionModel(makeModel());
        expect(grid.getSelectionModel()).toBeTruthy();
        expect(grid.getDisplayedScrollbarDimensions()).toBeDefined();
        expect(grid.getAbsoluteColumnMinWidth()).toBeGreaterThanOrEqual(0);
    });

    it("registers, finds, and unregisters plugins", () => {
        const { grid } = makeGrid();
        grids.push(grid);
        const plugin: GridPlugin = {
            init: () => { },
            destroy: () => { },
            pluginName: "p1"
        };
        grid.registerPlugin(plugin);
        expect(grid.getPluginByName("p1")).toBe(plugin);
        grid.unregisterPlugin(plugin);
        expect(grid.getPluginByName("p1")).toBeUndefined();
        grid.unregisterPlugin(plugin);
    });

    it("destroys a grid with async post render enabled", () => {
        const { grid } = makeGrid([{ first: "a", second: "b" }], {
            enableAsyncPostRender: true,
            enableAsyncPostRenderCleanup: true
        });
        grid.destroy();
    });

    it("covers setOptions branches", () => {
        const { grid } = makeGrid();
        grids.push(grid);
        grid.setOptions({ groupingPanel: true });
        grid.setOptions({ groupingPanel: false });
        grid.setOptions({ showColumnHeader: false });
        grid.setOptions({ enableAddRow: true });
        grid.setOptions({ columns: [{ id: "first", field: "first", name: "First", width: 100 }] });
        grid.setOptions({ autoHeight: true });
        grid.setOptions({ editorLock: undefined });
    });

    it("covers visibility setters", () => {
        const { grid } = makeGrid();
        grids.push(grid);
        grid.setTopPanelVisibility(true);
        grid.setTopPanelVisibility(false);
        grid.setColumnHeaderVisibility(false);
        grid.setColumnHeaderVisibility(true);
        grid.setFooterRowVisibility(true);
        grid.setFooterRowVisibility(false);
        grid.setGroupingPanelVisibility(true);
        grid.setGroupingPanelVisibility(false);
        grid.setPreHeaderPanelVisibility(true);
        grid.setHeaderRowVisibility(true);
        grid.setHeaderRowVisibility(false);
    });

    it("covers getFormatter fallback branches", () => {
        const metadata: ItemMetadata<Row> = { format: (() => "meta") as any };
        const view: IDataView<Row> = {
            getLength: () => 1,
            getItem: () => ({ first: "a", second: "b" }),
            getItemMetadata: () => metadata
        };
        const { grid } = makeGrid(view, {
            formatterFactory: { getFormatter: () => (() => "compat") as any },
            defaultFormatter: (() => "def") as any
        }, [{ ...baseColumns[0], format: undefined }, { ...baseColumns[1] }]);
        grids.push(grid);
        expect(grid.getFormatter(0, grid.getColumns()[0])).toBeTypeOf("function");
        metadata.format = undefined;
        (metadata as any).formatter = undefined;
        expect(grid.getFormatter(0, grid.getColumns()[0])).toBeTypeOf("function");
    });

    it("covers getEditor branches", () => {
        const editor = class { } as any;
        const metadata: ItemMetadata<Row> = { columns: { first: { editor } } };
        const view: IDataView<Row> = {
            getLength: () => 1,
            getItem: () => ({ first: "a", second: "b" }),
            getItemMetadata: () => metadata
        };
        const { grid } = makeGrid(view, { editorFactory: { getEditor: () => editor } });
        grids.push(grid);
        expect((grid as any).getEditor(0, 0)).toBe(editor);
        metadata.columns = { 0: { editor } };
        expect((grid as any).getEditor(0, 0)).toBe(editor);
        metadata.columns = undefined;
        expect((grid as any).getEditor(0, 0)).toBe(editor);
    });

    it("covers cell css styles add/remove/set", () => {
        const { grid } = makeGrid();
        grids.push(grid);
        grid.addCellCssStyles("k", { 0: { first: "cls" } });
        expect(() => grid.addCellCssStyles("k", {})).toThrow();
        grid.setCellCssStyles("k", { 0: { first: "cls2" } });
        expect(grid.getCellCssStyles("k")).toBeTruthy();
        grid.removeCellCssStyles("k");
        grid.removeCellCssStyles("k");
    });

    it("flashes a cell", () => {
        vi.useFakeTimers();
        const { grid } = makeGrid();
        grids.push(grid);
        grid.flashCell(0, 0);
        grid.flashCell(999, 0);
        vi.runAllTimers();
        vi.useRealTimers();
    });

    it("covers drag handlers", () => {
        const { grid } = makeGrid();
        grids.push(grid);
        const cell = grid.getCellNode(0, 0);
        const event = new MouseEvent("mousedown", { bubbles: true });
        Object.defineProperty(event, "target", { value: cell });
        const dd = {} as any;
        expect((grid as any).handleDragInit(event, dd)).toBe(false);
        expect((grid as any).handleDragStart(event, dd)).toBe(false);
        (grid as any).handleDragEnd(event, dd);
        grid.onDragInit.subscribe((e: any) => { e.stopImmediatePropagation(); return true; });
        expect((grid as any).handleDragInit(event, dd)).toBe(true);
        grid.onDragStart.subscribe((e: any) => e.stopImmediatePropagation());
        expect((grid as any).handleDragStart(event, dd)).toBe(true);
        grid.onDrag.subscribe(() => "rv");
        expect((grid as any).handleDrag(event, dd)).toBe("rv");
    });

    it("covers cell/node/point lookup helpers", () => {
        const { grid } = makeGrid();
        grids.push(grid);
        expect(grid.getCellFromPoint(10, 10)).toBeDefined();
        const cellNode = grid.getCellNode(0, 0);
        expect(grid.getCellFromNode(cellNode)).toBe(0);
        expect(grid.getCellFromNode(null)).toBeNull();
        expect(grid.getColumnFromNode(cellNode)).toBe(grid.getColumns()[0]);
        expect(grid.getColumnFromNode(null)).toBeNull();
        expect(grid.getRowFromNode(cellNode.parentElement)).toBe(0);
        expect(grid.getRowFromNode(null)).toBeNull();
        const ev = new MouseEvent("click", { bubbles: true });
        Object.defineProperty(ev, "target", { value: cellNode });
        expect(grid.getCellFromEvent(ev)).toEqual({ row: 0, cell: 0 });
        expect(grid.getCellNodeBox(0, 0)).toBeDefined();
        expect(grid.getCellNodeBox(999, 0)).toBeNull();
        grid.focus();
        grid.scrollCellIntoView(0, 1);
        grid.scrollColumnIntoView(1);
    });

    it("covers header mouse and context events", () => {
        const { grid } = makeGrid();
        grids.push(grid);
        const header = grid.getHeaderColumn("first");
        const mouseEnter = vi.fn();
        const mouseLeave = vi.fn();
        const ctx = vi.fn();
        const click = vi.fn();
        grid.onHeaderMouseEnter.subscribe(mouseEnter);
        grid.onHeaderMouseLeave.subscribe(mouseLeave);
        grid.onHeaderContextMenu.subscribe(ctx);
        grid.onHeaderClick.subscribe(click);
        header.dispatchEvent(new MouseEvent("mouseenter", { bubbles: true }));
        header.dispatchEvent(new MouseEvent("mouseleave", { bubbles: true }));
        header.dispatchEvent(new MouseEvent("contextmenu", { bubbles: true }));
        header.dispatchEvent(new MouseEvent("click", { bubbles: true }));
        expect(mouseEnter).toHaveBeenCalled();
        expect(mouseLeave).toHaveBeenCalled();
        expect(ctx).toHaveBeenCalled();
        expect(click).toHaveBeenCalled();
    });

    it("covers cell mouse enter/leave and context/dblclick handlers", () => {
        const { grid } = makeGrid();
        grids.push(grid);
        const cell = grid.getCellNode(0, 0);
        const enter = vi.fn();
        const leave = vi.fn();
        const ctx = vi.fn();
        const dbl = vi.fn();
        grid.onMouseEnter.subscribe(enter);
        grid.onMouseLeave.subscribe(leave);
        grid.onContextMenu.subscribe(ctx);
        grid.onDblClick.subscribe(dbl);
        cell.dispatchEvent(new MouseEvent("mouseenter", { bubbles: true }));
        cell.dispatchEvent(new MouseEvent("mouseleave", { bubbles: true }));
        cell.dispatchEvent(new MouseEvent("contextmenu", { bubbles: true }));
        cell.dispatchEvent(new MouseEvent("dblclick", { bubbles: true }));
        expect(enter).toHaveBeenCalled();
        expect(leave).toHaveBeenCalled();
        expect(ctx).toHaveBeenCalled();
        expect(dbl).toHaveBeenCalled();
    });

    it("covers handleClick with active cell selection", () => {
        const { grid } = makeGrid([{ first: "a", second: "b" }], { enableCellNavigation: true });
        grids.push(grid);
        const cell = grid.getCellNode(0, 0);
        const click = vi.fn();
        grid.onClick.subscribe(click);
        cell.dispatchEvent(new MouseEvent("click", { bubbles: true }));
        expect(click).toHaveBeenCalled();
    });

    it("covers keyboard navigation handlers", () => {
        const { grid } = makeGrid([{ first: "a", second: "b" }, { first: "c", second: "d" }], {
            enableCellNavigation: true,
            editable: true
        });
        grids.push(grid);
        const canvas = grid.getCanvasNode();
        const key = (k: string, opts: Record<string, unknown> = {}) =>
            canvas.dispatchEvent(new KeyboardEvent("keydown", { key: k, bubbles: true, cancelable: true, ...opts }));

        grid.setActiveCell(0, 0);
        key("Home");
        key("End");
        key("Home", { ctrlKey: true });
        key("End", { ctrlKey: true });
        key("PageDown");
        key("PageUp");
        key("ArrowLeft");
        key("ArrowRight");
        key("ArrowUp");
        key("ArrowDown");
        key("Tab");
        key("Tab", { shiftKey: true });
        key("Enter");
        key("Escape");
        key("A");
    });

    it("covers commitCurrentEdit validation error", () => {
        const { grid } = makeGrid([{ first: "a", second: "b" }], { editable: true }, [
            { ...baseColumns[0], editor: editorClass },
            { ...baseColumns[1] }
        ]);
        grids.push(grid);
        grid.setActiveCell(0, 0);
        grid.editActiveCell();
        const current = grid.getCellEditor() as any;
        current.isValueChanged = () => true;
        current.validate = () => ({ valid: false });
        const validation = vi.fn();
        grid.onValidationError.subscribe(validation);
        expect(grid.commitCurrentEdit()).toBe(false);
        expect(validation).toHaveBeenCalled();
        grid.cancelCurrentEdit();
    });

    it("covers commitCurrentEdit add-new-row path", () => {
        const { grid } = makeGrid([{ first: "a", second: "b" }], { editable: true, enableAddRow: true }, [
            { ...baseColumns[0], editor: editorClass },
            { ...baseColumns[1] }
        ]);
        grids.push(grid);
        const addNew = vi.fn();
        grid.onAddNewRow.subscribe(addNew);
        grid.setActiveCell(1, 0);
        grid.editActiveCell();
        const current = grid.getCellEditor() as any;
        current.isValueChanged = () => true;
        current.validate = () => ({ valid: true });
        expect(grid.commitCurrentEdit()).toBe(true);
        expect(addNew).toHaveBeenCalled();
    });

    it("covers autosizeColumns and setColumns preserve-initial", () => {
        const { grid } = makeGrid([{ first: "a", second: "b" }], { forceFitColumns: true });
        grids.push(grid);
        grid.autosizeColumns();
        grid.setVisibleColumns(["first"], { reorder: false, notify: false });
        grid.setColumns(grid.getColumns());
        grid.setColumns([{ id: "first", field: "first", name: "First", width: 100 }]);
    });

    it("covers updateColumnHeader with nameFormat function", () => {
        const nameFormat = vi.fn(() => "Formatted");
        const { grid } = makeGrid([{ first: "a", second: "b" }], {}, [
            { ...baseColumns[0], nameFormat },
            { ...baseColumns[1] }
        ]);
        grids.push(grid);
        grid.updateColumnHeader("first", nameFormat as any, "tip");
        expect(nameFormat).toHaveBeenCalled();
    });

    it("covers footer row creation with css classes", () => {
        const { grid } = makeGrid([{ first: "a", second: "b" }], { showFooterRow: true }, [
            { ...baseColumns[0], footerCssClass: "f1" },
            { ...baseColumns[1], cssClass: "f2" }
        ]);
        grids.push(grid);
        expect(grid.getFooterRowColumn("first").classList.contains("f1")).toBe(true);
        expect(grid.getFooterRowColumn("second").classList.contains("f2")).toBe(true);
    });

    it("covers updateGrandTotals via footer row", () => {
        const totalsFormat = vi.fn(() => "T");
        const { grid } = makeGrid([{ first: "a", second: "b" }], { showFooterRow: true }, [
            { ...baseColumns[0], groupTotalsFormat: totalsFormat },
            { ...baseColumns[1] }
        ]);
        grids.push(grid);
        grid.invalidate();
        expect(totalsFormat).toHaveBeenCalled();
    });

    it("covers handleMouseWheel branches directly", () => {
        const { grid } = makeGrid();
        grids.push(grid);
        const h = (grid as any).handleMouseWheel.bind(grid);
        h({ wheelDelta: 120, preventDefault: () => { } });
        h({ detail: 3, preventDefault: () => { } });
        h({ axis: 1, HORIZONTAL_AXIS: 1, wheelDelta: 120, preventDefault: () => { } });
        h({ wheelDeltaY: 120, shiftKey: true, preventDefault: () => { } });
        h({ wheelDeltaY: 120, wheelDeltaX: 120, preventDefault: () => { } });
        h({ wheelDeltaX: 120, preventDefault: () => { } });
    });

    it("covers handleScroll forceSync and scroll branches", () => {
        vi.useFakeTimers();
        const data = Array.from({ length: 80 }, (_, i) => ({ first: `a${i}`, second: `b${i}` }));
        const { grid } = makeGrid(data, {
            renderAllRows: false,
            renderAllCells: false,
            forceSyncScrolling: true,
            minBuffer: 1
        });
        grids.push(grid);
        const viewport = grid.getScrollContainerY();
        viewport.scrollTop = 500;
        viewport.dispatchEvent(new Event("scroll"));
        vi.runAllTimers();
        viewport.scrollTop = 20;
        viewport.dispatchEvent(new Event("scroll"));
        vi.runAllTimers();
        (grid as any).handleScroll({ forceSync: true });
        vi.useRealTimers();
    });

    it("covers async post process cleanup queue", () => {
        vi.useFakeTimers();
        const cleanup = vi.fn();
        const postRender = vi.fn();
        const { grid } = makeGrid([{ first: "a", second: "b" }], {
            enableAsyncPostRender: true,
            enableAsyncPostRenderCleanup: true,
            asyncPostRenderDelay: 0,
            asyncPostCleanupDelay: 0
        }, [
            { ...baseColumns[0], asyncPostRender: postRender, asyncPostRenderCleanup: cleanup },
            { ...baseColumns[1] }
        ]);
        grids.push(grid);
        vi.runAllTimers();
        grid.invalidateRow(0);
        grid.render();
        vi.runAllTimers();
        expect(postRender).toHaveBeenCalled();
        vi.useRealTimers();
    });

    it("covers scrollTo and scrollRowIntoView paging", () => {
        vi.useFakeTimers();
        const data = Array.from({ length: 80 }, (_, i) => ({ first: `a${i}`, second: `b${i}` }));
        const { grid } = makeGrid(data, {
            renderAllRows: false,
            renderAllCells: false,
            minBuffer: 1
        });
        grids.push(grid);
        grid.scrollRowIntoView(50, true);
        grid.scrollRowIntoView(0, false);
        grid.scrollRowToTop(10);
        grid.navigatePageDown();
        grid.navigatePageUp();
        grid.navigateTop();
        grid.navigateBottom();
        vi.runAllTimers();
        vi.useRealTimers();
    });

    it("covers getVisibleRange and getRenderedRange with virtual scrolling", () => {
        vi.useFakeTimers();
        const data = Array.from({ length: 100 }, (_, i) => ({ first: `a${i}`, second: `b${i}` }));
        const { grid } = makeGrid(data, {
            renderAllRows: false,
            renderAllCells: false,
            minBuffer: 1
        });
        grids.push(grid);
        const viewport = grid.getScrollContainerY();
        viewport.scrollTop = 1000;
        viewport.dispatchEvent(new Event("scroll"));
        vi.runAllTimers();
        expect(grid.getRenderedRange(0, 0).bottom).toBeGreaterThan(0);
        grid.render();
        vi.useRealTimers();
    });

    it("covers setupColumnReorder with Sortable", () => {
        const destroy = vi.fn();
        const instances: any[] = [];
        (globalThis as any).Sortable = {
            create: (el: any, opts: any) => {
                const inst = { el, opts, toArray: () => ["first", "second"], destroy };
                instances.push(inst);
                return inst;
            }
        };
        const { grid } = makeGrid([{ first: "a", second: "b" }], { enableColumnReorder: true });
        grids.push(grid);
        expect(instances.length).toBeGreaterThan(0);
        const inst = instances[0];
        inst.opts.onStart({ item: {}, originalEvent: { pageX: 0 } });
        inst.opts.onEnd({ item: document.createElement("div"), originalEvent: {}, stopPropagation: () => { } });
        delete (globalThis as any).Sortable;
    });

    it("covers comprehensive getFormatter fallback chain", () => {
        const metadata: ItemMetadata<Row> = { format: (() => "meta") as any };
        const view: IDataView<Row> = {
            getLength: () => 1,
            getItem: () => ({ first: "a", second: "b" }),
            getItemMetadata: () => metadata
        };
        const { grid } = makeGrid(view, {
            formatterFactory: { getFormat: () => (() => "factory") as any },
            defaultFormat: (() => "default") as any
        }, [{ ...baseColumns[0], format: undefined }, { ...baseColumns[1] }]);
        grids.push(grid);
        const col = grid.getColumns()[0];
        expect(grid.getFormatter(0, col)).toBeTypeOf("function"); // metadata.format
        metadata.format = undefined;
        (metadata as any).formatter = (() => "meta-compat") as any;
        expect(grid.getFormatter(0, col)).toBeTypeOf("function"); // metadata.formatter
        (metadata as any).formatter = undefined;
        (col as any).formatter = (() => "col-compat") as any;
        expect(grid.getFormatter(0, col)).toBeTypeOf("function"); // column.formatter
        (col as any).formatter = undefined;
        expect(grid.getFormatter(0, col)).toBeTypeOf("function"); // factory.getFormat
        (grid.getOptions() as any).formatterFactory = { getFormat: () => null };
        expect(grid.getFormatter(0, col)).toBeTypeOf("function"); // defaultFormat
        (grid.getOptions() as any).defaultFormat = undefined;
        (grid.getOptions() as any).defaultFormatter = (() => "def-compat") as any;
        expect(grid.getFormatter(0, col)).toBeTypeOf("function"); // defaultFormatter
        (grid.getOptions() as any).defaultFormatter = undefined;
        expect(grid.getFormatter(0, col)).toBeTypeOf("function"); // defaultColumnFormat
    });

    it("covers getEditor column.editor and factory branches", () => {
        const editor = class { } as any;
        const { grid } = makeGrid([{ first: "a", second: "b" }], {
            editorFactory: { getEditor: () => editor }
        }, [{ ...baseColumns[0], editor }, { ...baseColumns[1] }]);
        grids.push(grid);
        expect((grid as any).getEditor(0, 0)).toBe(editor); // column.editor
        grid.getColumns()[0].editor = undefined;
        expect((grid as any).getEditor(0, 0)).toBe(editor); // editorFactory
        grid.getColumns()[0].editor = undefined;
        (grid.getOptions() as any).editorFactory = undefined;
        expect((grid as any).getEditor(0, 0)).toBeUndefined();
    });

    it("covers handleScroll page-switching branches", () => {
        vi.useFakeTimers();
        const data = Array.from({ length: 500 }, (_, i) => ({ first: `a${i}`, second: `b${i}` }));
        const { grid } = makeGrid(data, {
            renderAllRows: false,
            renderAllCells: false,
            minBuffer: 1
        });
        grids.push(grid);
        const viewport = grid.getScrollContainerY();
        viewport.scrollTop = 5000;
        viewport.dispatchEvent(new Event("scroll"));
        vi.runAllTimers();
        viewport.scrollTop = 0;
        viewport.dispatchEvent(new Event("scroll"));
        vi.runAllTimers();
        vi.useRealTimers();
    });

    it("covers canCellBeActive and canCellBeSelected metadata branches", () => {
        const metadata: ItemMetadata<Row> = {
            columns: { first: { focusable: true, tabbable: false, selectable: true } }
        };
        const view: IDataView<Row> = {
            getLength: () => 1,
            getItem: () => ({ first: "a", second: "b" }),
            getItemMetadata: () => metadata
        };
        const { grid } = makeGrid(view, { enableCellNavigation: true }, [
            { ...baseColumns[0], focusable: true, tabbable: true },
            { ...baseColumns[1] }
        ]);
        grids.push(grid);
        // colsMetadata[cols[cell].id].focusable branch
        expect(grid.canCellBeActive(0, 0, true)).toBe(false); // tabbable false
        expect(grid.canCellBeActive(0, 0)).toBe(true);
        // colsMetadata[cell].focusable branch
        metadata.columns = { 0: { focusable: true, tabbable: false } };
        expect(grid.canCellBeActive(0, 0, true)).toBe(false);
        expect(grid.canCellBeActive(0, 0)).toBe(true);
        // cols[cell].focusable branch
        metadata.columns = undefined;
        expect(grid.canCellBeActive(0, 0, true)).toBe(true); // column tabbable true
        expect(grid.canCellBeActive(0, 0)).toBe(true);
        // canCellBeSelected branches
        metadata.columns = { first: { selectable: false } };
        expect(grid.canCellBeSelected(0, 0)).toBe(false);
        metadata.columns = undefined;
        (metadata as any).selectable = false;
        expect(grid.canCellBeSelected(0, 0)).toBe(false);
        (metadata as any).selectable = undefined;
        expect(grid.canCellBeSelected(0, 0)).toBe(true); // column selectable default
    });

    it("covers setSelectedRows and getSelectedRows", () => {
        const { grid } = makeGrid();
        grids.push(grid);
        const model: SelectionModel = {
            init: () => { },
            setSelectedRanges: (ranges: CellRange[]) => {
                (grid as any)._selectedRows = ranges.map(r => r.fromRow);
            },
            refreshSelections: () => { },
            onSelectedRangesChanged: new EventEmitter<CellRange[]>()
        };
        grid.setSelectionModel(model);
        grid.setSelectedRows([0]);
        expect(grid.getSelectedRows()).toEqual([0]);
    });

    it("covers getters, extractor, and missing selection model errors", () => {
        const extractor = vi.fn(() => "extracted");
        const { grid } = makeGrid([], {
            showTopPanel: true,
            dataItemColumnValueExtractor: extractor
        });
        grids.push(grid);

        expect(grid.getEditorFactory()).toBe(grid.getOptions().editorFactory);
        expect(grid.getEditController()).toBeDefined();
        expect(grid.getTopPanel()).toBeDefined();
        expect(grid.getDataItemValueForColumn({ first: "a", second: "b" }, grid.getColumns()[0])).toBe("extracted");
        expect(extractor).toHaveBeenCalled();

        const noSelection = makeGrid().grid;
        grids.push(noSelection);
        expect(() => noSelection.getSelectedRows()).toThrow();
        expect(() => noSelection.setSelectedRows([0])).toThrow();
    });

    it("delegates committed edits to editCommandHandler", () => {
        const handler = vi.fn();
        const { grid } = makeGrid([{ first: "a", second: "b" }], {
            editable: true,
            editCommandHandler: handler
        }, [{ ...baseColumns[0], editor: editorClass }, { ...baseColumns[1] }]);
        grids.push(grid);

        grid.setActiveCell(0, 0);
        grid.editActiveCell();
        const editor = grid.getCellEditor() as any;
        editor.isValueChanged = () => true;
        editor.validate = () => ({ valid: true });
        editor.serializeValue = () => "changed";

        expect(grid.commitCurrentEdit()).toBe(true);
        expect(handler).toHaveBeenCalledOnce();
    });

    it("loads editors asynchronously and invokes editor focus callbacks", () => {
        vi.useFakeTimers();
        const { grid } = makeGrid([{ first: "a", second: "b" }], {
            editable: true,
            asyncEditorLoading: true,
            asyncEditorLoadDelay: 0
        }, [{ ...baseColumns[0], editor: editorClass }, { ...baseColumns[1] }]);
        grids.push(grid);

        grid.setActiveCell(0, 0);
        (grid as any).setActiveCellInternal(grid.getCellNode(0, 0), true);
        vi.runAllTimers();
        expect(grid.getCellEditor()).toBeTruthy();
        expect((grid as any).getActiveCellPosition()).toBeDefined();
        expect(grid.getGridPosition()).toBeDefined();
        (grid as any).commitEditAndSetFocus();

        grid.setActiveCell(0, 0);
        grid.editActiveCell();
        (grid as any).cancelEditAndSetFocus();
        vi.useRealTimers();
    });

    it("synchronizes header row scrolling and ignores throttled scrolls", () => {
        const { grid } = makeGrid([{ first: "a", second: "b" }], {
            showHeaderRow: true,
            showFooterRow: true
        });
        grids.push(grid);

        const headerScroll = grid.getHeaderRow().parentElement as HTMLElement;
        const scrollContainer = grid.getScrollContainerX();
        Object.defineProperty(headerScroll, "scrollLeft", { value: 40, writable: true, configurable: true });
        headerScroll.dispatchEvent(new Event("scroll"));
        expect(scrollContainer.scrollLeft).toBe(40);

        (grid as any)._ignoreScrollUntil = Date.now() + 10000;
        Object.defineProperty(headerScroll, "scrollLeft", { value: 80, writable: true, configurable: true });
        headerScroll.dispatchEvent(new Event("scroll"));
        expect(scrollContainer.scrollLeft).toBe(40);
    });
});
