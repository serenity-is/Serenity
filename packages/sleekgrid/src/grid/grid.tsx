import { signal } from "@serenity-is/signals";
import { CellRange, CellStylesHash, Column, ColumnFormat, ColumnMetadata, ColumnSort, EditCommand, EditController, Editor, EditorClass, EditorHost, EditorLock, EventData, EventEmitter, FormatterContext, FormatterResult, IEventData, IGroupTotals, ItemMetadata, Position, RowCell, ViewRange, ViewportInfo, addClass, applyFormatterResultToCellNode, columnDefaults, convertCompatFormatter, defaultColumnFormat, escapeHtml, formatterContext, initializeColumns, parsePx, preClickClassName, removeClass } from "../core";
import { GridOptions, gridDefaults } from "../core/gridoptions";
import { IDataView } from "../core/idataview";
import { BasicLayout } from "./basiclayout";
import { CellNavigator } from "./cellnavigator";
import { Draggable } from "./draggable";
import { ArgsAddNewRow, ArgsCell, ArgsCellChange, ArgsCellEdit, ArgsColumn, ArgsColumnNode, ArgsCssStyle, ArgsEditorDestroy, ArgsGrid, ArgsScroll, ArgsSelectedRowsChange, ArgsSort, ArgsValidationError } from "./eventargs";
import { CachedRow, PostProcessCleanupEntry, absBox, autosizeColumns, calcMinMaxPageXOnDragStart, getInnerWidth, getMaxSupportedCssHeight, getScrollBarDimensions, getVBoxDelta, shrinkOrStretchColumn, simpleArrayEquals, sortToDesiredOrderAndKeepRest } from "./internal";
import { LayoutEngine, type GridOptionSignals } from "./layout";
import { IPlugin, SelectionModel } from "./types";

export class Grid<TItem = any> implements EditorHost {
    declare private _absoluteColMinWidth: number;
    declare private _activeCanvasNode: HTMLElement;
    declare private _activeCell: number;
    declare private _activeCellNode: HTMLElement;
    declare private _activePosX: number;
    declare private _activeRow: number;
    declare private _activeViewportNode: HTMLElement;
    private _cellCssClasses: Record<string, CellStylesHash> = {};
    private _cellHeightDiff: number = 0;
    private _cellWidthDiff: number = 0;
    declare private _cellNavigator: CellNavigator;
    declare private _colById: { [key: string]: number };
    declare private _colDefaults: Partial<Column>;
    private _colLeft: number[] = [];
    private _colRight: number[] = [];
    declare private _cols: Column<TItem>[];
    declare private _columnCssRulesL: any;
    declare private _columnCssRulesR: any;
    declare private _currentEditor: Editor;
    declare private _data: IDataView<TItem> | TItem[];
    declare private _draggableInstance: { destroy: () => void };
    declare private _editController: EditController;
    declare private _emptyNode: (node: Element) => void;
    private _headerColumnWidthDiff: number = 0;
    declare private _hEditorLoader: number;
    declare private _hPostRender: number;
    declare private _hPostRenderCleanup: number;
    declare private _hRender: number;
    private _ignoreScrollUntil: number = 0;
    declare private _initColById: { [key: string]: number };
    declare private _initCols: Column<TItem>[];
    declare private _initialized: any;
    declare private _jQuery: any;
    declare private _jumpinessCoefficient: number;
    declare private _lastRenderTime: number;
    declare private _layout: LayoutEngine;
    declare private _numberOfPages: number;
    declare private _options: GridOptions<TItem>;
    private _optionSignals: GridOptionSignals = {
        showTopPanel: signal<boolean>(),
        showColumnHeader: signal<boolean>(),
        showHeaderRow: signal<boolean>(),
        showFooterRow: signal<boolean>(),
    };
    private _page: number = 0;
    declare private _pageHeight: number;
    private _pageOffset: number = 0;
    private _pagingActive: boolean = false;
    private _pagingIsLastPage: boolean = false;
    private _plugins: IPlugin[] = [];
    declare private _postCleanupActive: boolean;
    private _postProcessCleanupQueue: PostProcessCleanupEntry[] = [];
    private _postProcessedRows: { [row: number]: { [cell: number]: string } } = {};
    declare private _postProcessFromRow: number;
    private _postProcessGroupId: number = 0;
    declare private _postProcessToRow: number;
    declare private _postRenderActive: boolean;
    declare private _removeNode: (node: Element) => void;
    private _rowsCache: { [key: number]: CachedRow } = {};
    declare private _scrollDims: { width: number, height: number };
    private _scrollLeft: number = 0;
    private _scrollLeftPrev: number = 0;
    private _scrollLeftRendered: number = 0;
    private _scrollTop: number = 0;
    private _scrollTopPrev: number = 0;
    private _scrollTopRendered: number = 0;
    private _selectedRows: number[] = [];
    declare private _selectionModel: SelectionModel;
    declare private _serializedEditorValue: any;
    private _sortColumns: ColumnSort[] = [];
    declare private _styleNode: HTMLStyleElement;
    declare private _stylesheet: any;
    private _tabbingDirection: number = 1;
    private _uid: string = "sleekgrid_" + Math.round(1000000 * Math.random());
    private _viewportInfo: ViewportInfo = {} as any;
    private _vScrollDir: number = 1;

    private _boundAncestorScroll: HTMLElement[] = [];
    declare private _container: HTMLElement;
    declare private _focusSink1: HTMLElement;
    declare private _focusSink2: HTMLElement;
    declare private _groupingPanel: HTMLElement;

    readonly onActiveCellChanged = new EventEmitter<ArgsCell>();
    readonly onActiveCellPositionChanged = new EventEmitter<ArgsGrid>();
    readonly onAddNewRow = new EventEmitter<ArgsAddNewRow>();
    readonly onBeforeCellEditorDestroy = new EventEmitter<ArgsEditorDestroy>();
    readonly onBeforeDestroy = new EventEmitter<ArgsGrid>();
    readonly onBeforeEditCell = new EventEmitter<ArgsCellEdit>();
    readonly onBeforeFooterRowCellDestroy = new EventEmitter<ArgsColumnNode>();
    readonly onBeforeHeaderCellDestroy = new EventEmitter<ArgsColumnNode>();
    readonly onBeforeHeaderRowCellDestroy = new EventEmitter<ArgsColumnNode>();
    readonly onCellChange = new EventEmitter<ArgsCellChange>();
    readonly onCellCssStylesChanged = new EventEmitter<ArgsCssStyle>();
    readonly onClick = new EventEmitter<ArgsCell, MouseEvent>();
    readonly onColumnsReordered = new EventEmitter<ArgsGrid>();
    readonly onColumnsResized = new EventEmitter<ArgsGrid>();
    readonly onCompositeEditorChange = new EventEmitter<ArgsGrid>();
    readonly onContextMenu = new EventEmitter<ArgsGrid, UIEvent>();
    readonly onDblClick = new EventEmitter<ArgsCell, MouseEvent>();
    readonly onDrag = new EventEmitter<ArgsGrid, UIEvent>();
    readonly onDragEnd = new EventEmitter<ArgsGrid, UIEvent>();
    readonly onDragInit = new EventEmitter<ArgsGrid, UIEvent>();
    readonly onDragStart = new EventEmitter<ArgsGrid, UIEvent>();
    readonly onFooterRowCellRendered = new EventEmitter<ArgsColumnNode>();
    readonly onHeaderCellRendered = new EventEmitter<ArgsColumnNode>();
    readonly onHeaderClick = new EventEmitter<ArgsColumn>();
    readonly onHeaderContextMenu = new EventEmitter<ArgsColumn>();
    readonly onHeaderMouseEnter = new EventEmitter<ArgsColumn, MouseEvent>();
    readonly onHeaderMouseLeave = new EventEmitter<ArgsColumn, MouseEvent>();
    readonly onHeaderRowCellRendered = new EventEmitter<ArgsColumnNode>();
    readonly onKeyDown = new EventEmitter<ArgsCell, KeyboardEvent>();
    readonly onMouseEnter = new EventEmitter<ArgsGrid, MouseEvent>();
    readonly onMouseLeave = new EventEmitter<ArgsGrid, MouseEvent>();
    readonly onScroll = new EventEmitter<ArgsScroll>();
    readonly onSelectedRowsChanged = new EventEmitter<ArgsSelectedRowsChange>();
    readonly onSort = new EventEmitter<ArgsSort>();
    readonly onValidationError = new EventEmitter<ArgsValidationError>();
    readonly onViewportChanged = new EventEmitter<ArgsGrid>();

    constructor(container: string | HTMLElement | ArrayLike<HTMLElement>, data: any, columns: Column<TItem>[], options: GridOptions<TItem>) {

        this._data = data;
        this._colDefaults = Object.assign({}, columnDefaults);

        this._options = options = Object.assign({}, gridDefaults, options);
        // @ts-ignore
        options.jQuery = this._jQuery = options.jQuery === void 0 ? (typeof jQuery !== "undefined" ? jQuery : void 0) : options.jQuery;
        // @ts-ignore
        options.sanitizer = options.sanitizer === void 0 ? formatterContext().sanitizer : options.sanitizer;

        if (this._jQuery && container instanceof (this._jQuery as any))
            this._container = (container as any)[0];
        else if (container instanceof Element)
            this._container = container as HTMLElement;
        else if (typeof container === "string")
            this._container = document.querySelector(container);
        else if (container.length)
            container = container[0];

        if (this._container == null) {
            throw new Error("SleekGrid requires a valid container, " + container + " does not exist in the DOM.");
        }

        this._container.classList.add('slick-container');

        this._emptyNode = options.emptyNode ?? (this._jQuery ? (function (node: Element) { this(node).empty(); }).bind(this._jQuery) : (function (node: Element) { node.innerHTML = ""; }));
        this._removeNode = options.removeNode ?? (this._jQuery ? (function (node: Element) { this(node).remove(); }).bind(this._jQuery) : (function (node: Element) { node.remove(); }));

        if (options?.createPreHeaderPanel) {
            // for compat, as draggable grouping plugin expects preHeaderPanel for grouping
            options.groupingPanel ??= true;
            if (options.groupingPanelHeight == null && options.preHeaderPanelHeight != null)
                options.groupingPanelHeight = options.preHeaderPanelHeight;
            if (options.showGroupingPanel == null && options.showPreHeaderPanel != null)
                options.showGroupingPanel = options.showPreHeaderPanel;
        }

        this._options.rtl = this._options.rtl ??
            (document.body.classList.contains('rtl') || (typeof getComputedStyle != "undefined" &&
                getComputedStyle(this._container).direction == 'rtl'));

        if (this._options.rtl)
            this._container.classList.add('rtl');
        else
            this._container.classList.add('ltr');

        this.validateAndEnforceOptions();
        this.setOptionSignals();
        this._colDefaults.width = options.defaultColumnWidth;

        this._editController = {
            "commitCurrentEdit": this.commitCurrentEdit.bind(this),
            "cancelCurrentEdit": this.cancelCurrentEdit.bind(this)
        };

        if (this._jQuery)
            this._jQuery(this._container).empty();
        else
            this._container.innerHTML = '';

        this._container.style.overflow = "hidden";
        this._container.style.outline = "0";
        this._container.classList.add(this._uid);

        // set up a positioning container if needed
        if (!/relative|absolute|fixed/.test(getComputedStyle(this._container).position)) {
            this._container.style.position = "relative";
        }

        this._container.appendChild(<div class="slick-focus-sink" tabindex={0} ref={el => this._focusSink1 = el} />);

        this._layout = typeof options.layoutEngine === "function" ? options.layoutEngine() : (options.layoutEngine ?? new BasicLayout());
        this.setInitialCols(columns);
        this._scrollDims = getScrollBarDimensions();

        if (options.groupingPanel) {
            this.createGroupingPanel();
        }

        this._layout.init({
            cleanUpAndRenderCells: this.cleanUpAndRenderCells.bind(this),
            bindAncestorScroll: this.bindAncestorScroll.bind(this),
            getAvailableWidth: this.getAvailableWidth.bind(this),
            getCellFromPoint: this.getCellFromPoint.bind(this),
            getColumnCssRules: this.getColumnCssRules.bind(this),
            getColumns: this.getColumns.bind(this),
            getInitialColumns: this.getInitialColumns.bind(this),
            getContainerNode: this.getContainerNode.bind(this),
            getDataLength: this.getDataLength.bind(this),
            getOptions: this.getOptions.bind(this),
            getOptionSignals: () => this._optionSignals,
            getRowFromNode: this.getRowFromNode.bind(this),
            getScrollDims: this.getScrollBarDimensions.bind(this),
            getScrollLeft: () => this._scrollLeft,
            getScrollTop: () => this._scrollTop,
            getViewportInfo: () => this._viewportInfo,
            renderRows: this.renderRows.bind(this)
        });

        this._container.append(this._focusSink2 = this._focusSink1.cloneNode() as HTMLElement);

        if (options.viewportClass)
            this.getViewports().forEach(vp => addClass(vp, options.viewportClass));

        if (!options.explicitInitialization) {
            this.init();
        }

        this.bindToData();
    }

    private createGroupingPanel() {
        if (this._groupingPanel)
            return;

        this._groupingPanel = <div class={["slick-grouping-panel", !this._options.showGroupingPanel && "slick-hidden"]}>
            {this._options.createPreHeaderPanel && <div class="slick-preheader-panel" />}
        </div> as HTMLElement;

        this._focusSink1?.insertAdjacentElement("afterend", this._groupingPanel);
    }

    private bindAncestorScroll(elem: HTMLElement) {
        if (this._jQuery)
            this._jQuery(elem).on('scroll', this.handleActiveCellPositionChange);
        else
            elem.addEventListener('scroll', this.handleActiveCellPositionChange);
        this._boundAncestorScroll.push(elem);
    }

    init(): void {
        if (this._initialized)
            return;

        this._initialized = true;

        this.calcViewportSize();

        // header columns and cells may have different padding/border skewing width calculations (box-sizing, hello?)
        // calculate the diff so we can set consistent sizes
        this.measureCellPaddingAndBorder();

        var viewports = this.getViewports();

        if (this._jQuery && !this._options.enableTextSelectionOnCells) {
            // disable text selection in grid cells except in input and textarea elements
            // (this is IE-specific, because selectstart event will only fire in IE)
            this._jQuery(viewports).on("selectstart.ui", () => {
                return this._jQuery(this).is("input,textarea");
            });
        }

        this._layout.setPaneVisibility();
        this._layout.setScroller();
        this.setOverflow();

        this.updateViewColLeftRight();
        this.createColumnHeaders();
        this.createColumnFooters();
        this.setupColumnSort();
        this.createCssRules();
        this.resizeCanvas();
        this._layout.bindAncestorScrollEvents();

        const onEvent = <K extends keyof HTMLElementEventMap>(el: HTMLElement, type: K,
            listener: (this: HTMLElement, ev: HTMLElementEventMap[K]) => any) => {
            if (this._jQuery)
                this._jQuery(el).on(type, listener as any);
            else
                el.addEventListener(type, listener);
        }

        onEvent(this._container, "resize", this.resizeCanvas);

        viewports.forEach(vp => {
            var scrollTicking = false;
            onEvent(vp, "scroll", (e) => {
                if (!scrollTicking) {
                    scrollTicking = true;

                    window.requestAnimationFrame(() => {
                        this.handleScroll();
                        scrollTicking = false;
                    });
                }
            });
        });

        const handleMouseWheel = this.handleMouseWheel.bind(this);
        viewports.forEach(vp => {
            onEvent(vp, "wheel", handleMouseWheel);
            onEvent(vp, "mousewheel" as any, handleMouseWheel);
        });

        this._layout.getHeaderCols().forEach(hs => {
            hs.onselectstart = () => false;
            onEvent(hs, "contextmenu", this.handleHeaderContextMenu.bind(this));
            onEvent(hs, "click", this.handleHeaderClick.bind(this));
            if (this._jQuery) {
                this._jQuery(hs)
                    .on('mouseenter', '.slick-header-column', this.handleHeaderMouseEnter.bind(this))
                    .on('mouseleave', '.slick-header-column', this.handleHeaderMouseLeave.bind(this));
            }
            else {
                // need to reimplement this similar to jquery events
                hs.addEventListener("mouseenter", e => (e.target as HTMLElement).closest(".slick-header-column") &&
                    this.handleHeaderMouseEnter(e));
                hs.addEventListener("mouseleave", e => (e.target as HTMLElement).closest(".slick-header-column") &&
                    this.handleHeaderMouseLeave(e));
            }
        });

        this._layout.getHeaderRowCols().forEach(el => {
            el && onEvent(el.parentElement, 'scroll', this.handleHeaderFooterRowScroll.bind(this));
        });

        this._layout.getFooterRowCols().forEach(el => {
            el && onEvent(el.parentElement, 'scroll', this.handleHeaderFooterRowScroll.bind(this));
        });

        [this._focusSink1, this._focusSink2].forEach(fs => onEvent(fs, "keydown", this.handleKeyDown.bind(this)));

        var canvases = Array.from<HTMLElement>(this.getCanvases());
        canvases.forEach(canvas => {
            onEvent(canvas, "keydown", this.handleKeyDown.bind(this))
            onEvent(canvas, "click", this.handleClick.bind(this))
            onEvent(canvas, "dblclick", this.handleDblClick.bind(this))
            onEvent(canvas, "contextmenu", this.handleContextMenu.bind(this));
        });

        if (this._jQuery && (this._jQuery.fn as any).drag) {
            this._jQuery(canvases)
                .on("draginit", this.handleDragInit.bind(this))
                .on("dragstart", { distance: 3 }, this.handleDragStart.bind(this))
                .on("drag", this.handleDrag.bind(this))
                .on("dragend", this.handleDragEnd.bind(this))
        }
        else {
            this._draggableInstance = Draggable({
                containerElement: this._container,
                //allowDragFrom: 'div.slick-cell',
                // the slick cell parent must always contain `.dnd` and/or `.cell-reorder` class to be identified as draggable
                //allowDragFromClosest: 'div.slick-cell.dnd, div.slick-cell.cell-reorder',
                preventDragFromKeys: ['ctrlKey', 'metaKey'],
                onDragInit: this.handleDragInit.bind(this),
                onDragStart: this.handleDragStart.bind(this),
                onDrag: this.handleDrag.bind(this),
                onDragEnd: this.handleDragEnd.bind(this)
            });
        }

        canvases.forEach(canvas => {
            if (this._jQuery) {
                this._jQuery(canvas)
                    .on('mouseenter', '.slick-cell', this.handleMouseEnter.bind(this))
                    .on('mouseleave', '.slick-cell', this.handleMouseLeave.bind(this));
            }
            else {
                canvas.addEventListener("mouseenter", e => (e.target as HTMLElement)?.classList?.contains("slick-cell") && this.handleMouseEnter(e), { capture: true });
                canvas.addEventListener("mouseleave", e => (e.target as HTMLElement)?.classList?.contains("slick-cell") && this.handleMouseLeave(e), { capture: true });
            }
        });

        // Work around http://crbug.com/312427.
        if (navigator.userAgent.toLowerCase().match(/webkit/) &&
            navigator.userAgent.toLowerCase().match(/macintosh/)) {
            const handleMouseWheel = this.handleMouseWheel.bind(this);
            canvases.forEach(c => {
                onEvent(c, "wheel", handleMouseWheel);
                onEvent(c, "mousewheel" as any, handleMouseWheel);
            });
        }
    }

    registerPlugin(plugin: IPlugin): void {
        this._plugins.unshift(plugin);
        plugin.init(this);
    }

    unregisterPlugin(plugin: IPlugin): void {
        for (var i = this._plugins.length; i >= 0; i--) {
            if (this._plugins[i] === plugin) {
                if (this._plugins[i].destroy) {
                    this._plugins[i].destroy();
                }
                this._plugins.splice(i, 1);
                break;
            }
        }
    }

    getPluginByName(name: string): IPlugin {
        for (var i = this._plugins.length - 1; i >= 0; i--) {
            if (this._plugins[i].pluginName === name)
                return this._plugins[i];
        }
    }

    setSelectionModel(model: SelectionModel): void {
        this.unregisterSelectionModel();

        this._selectionModel = model;
        if (this._selectionModel) {
            this._selectionModel.init(this);
            this._selectionModel.onSelectedRangesChanged.subscribe(this.handleSelectedRangesChanged);
        }
    }

    private unregisterSelectionModel(): void {
        if (!this._selectionModel)
            return;

        this._selectionModel.onSelectedRangesChanged.unsubscribe(this.handleSelectedRangesChanged);
        this._selectionModel.destroy?.();
    }

    getScrollBarDimensions(): { width: number; height: number; } {
        return this._scrollDims;
    }

    getDisplayedScrollbarDimensions(): { width: number; height: number; } {
        return {
            width: this._viewportInfo.hasVScroll ? this._scrollDims.width : 0,
            height: this._viewportInfo.hasHScroll ? this._scrollDims.height : 0
        };
    }

    getAbsoluteColumnMinWidth() {
        return this._absoluteColMinWidth;
    }

    getSelectionModel(): SelectionModel {
        return this._selectionModel;
    }

    private colIdOrIdxToCell(columnIdOrIdx: string | number): number {
        if (columnIdOrIdx == null)
            return null;

        if (typeof columnIdOrIdx !== "number")
            return this.getColumnIndex(columnIdOrIdx);

        return columnIdOrIdx;
    }

    getCanvasNode(columnIdOrIdx?: string | number, row?: number): HTMLElement {
        return this._layout.getCanvasNodeFor(this.colIdOrIdxToCell(columnIdOrIdx || 0), row || 0);
    }

    getCanvases(): any | HTMLElement[] {
        var canvases = this._layout.getCanvasNodes();
        return this._jQuery ? this._jQuery(canvases) : canvases;
    }

    getActiveCanvasNode(e?: IEventData): HTMLElement {
        if (e) { // compatibility with celldecorator plugin
            this._activeCanvasNode = (e.target as HTMLElement).closest('.grid-canvas');
        }
        return this._activeCanvasNode;
    }

    getViewportNode(columnIdOrIdx?: string | number, row?: number): HTMLElement {
        return this._layout.getViewportNodeFor(this.colIdOrIdxToCell(columnIdOrIdx || 0), row || 0);
    }

    private getViewports(): HTMLElement[] {
        return this._layout.getViewportNodes();
    }

    getActiveViewportNode(e?: IEventData): HTMLElement {
        if (e) { // compatibility with celldecorator plugin
            this._activeViewportNode = (e.target as HTMLElement).closest('.slick-viewport');
        }

        return this._activeViewportNode;
    }

    private getAvailableWidth() {
        return this._viewportInfo.hasVScroll ? this._viewportInfo.width - this._scrollDims.width : this._viewportInfo.width;
    }

    private updateCanvasWidth(forceColumnWidthsUpdate?: boolean): void {
        const widthChanged = this._layout.updateCanvasWidth();

        if (widthChanged || forceColumnWidthsUpdate) {
            this._layout.applyColumnWidths();
        }
    }

    private unbindAncestorScrollEvents(): void {
        if (this._boundAncestorScroll) {
            for (var x of this._boundAncestorScroll)
                x.removeEventListener('scroll', this.handleActiveCellPositionChange);
        }
        this._boundAncestorScroll = [];
    }

    updateColumnHeader(columnId: string, title?: string | ColumnFormat<any>, toolTip?: string): void {
        if (!this._initialized) {
            return;
        }

        var idx = this.getColumnIndex(columnId);
        if (idx == null) {
            return;
        }

        var columnDef = this._cols[idx];
        var header = this._layout.getHeaderColumn(idx);
        if (!header)
            return;

        if (title !== undefined) {
            if (typeof title === "function")
                columnDef.nameFormat = title;
            else
                columnDef.name = title;
        }
        if (toolTip !== undefined) {
            columnDef.toolTip = toolTip;
        }

        this.trigger(this.onBeforeHeaderCellDestroy, {
            node: header,
            column: columnDef
        });

        if (toolTip !== undefined)
            header.title = toolTip || "";

        const nameSpan = (header.querySelector(":scope > .slick-column-name") ?? header.firstElementChild) as HTMLElement;
        if (nameSpan) {
            this._emptyNode(nameSpan);
            if (typeof columnDef.nameFormat === "function") {
                const ctx = this.getFormatterContext(-1, idx);
                (ctx as any).enableHtmlRendering = false;
                ctx.value = columnDef.name;
                const fmtResult = columnDef.nameFormat(ctx);
                applyFormatterResultToCellNode(ctx, fmtResult, nameSpan, contentOnly);
            }
            else
                nameSpan.textContent = columnDef.name ?? '';
        }

        this.trigger(this.onHeaderCellRendered, {
            node: header,
            column: columnDef
        });
    }

    getHeader(): HTMLElement {
        return this._layout.getHeaderCols()[0];
    }

    getHeaderColumn(columnIdOrIdx: string | number): HTMLElement {
        var cell = this.colIdOrIdxToCell(columnIdOrIdx);
        if (cell == null)
            return null;

        return this._layout.getHeaderColumn(cell);
    }

    getGroupingPanel(): HTMLElement {
        return this._groupingPanel;
    }

    getPreHeaderPanel(): HTMLElement {
        return this._groupingPanel?.querySelector('.slick-preheader-panel');
    }

    getHeaderRow(): HTMLElement {
        return this._layout.getHeaderRowCols()[0];
    }

    getHeaderRowColumn(columnIdOrIdx: string | number): HTMLElement {
        var cell = this.colIdOrIdxToCell(columnIdOrIdx);
        if (cell == null)
            return;

        return this._layout.getHeaderRowColumn(cell);
    }

    getFooterRow(): HTMLElement {
        return this._layout.getFooterRowCols()[0];
    }

    getFooterRowColumn(columnIdOrIdx: string | number): HTMLElement {
        var cell = this.colIdOrIdxToCell(columnIdOrIdx);
        if (cell == null)
            return null;

        return this._layout.getFooterRowColumn(cell);
    }

    private createColumnFooters(): void {
        var footerRowCols = this._layout.getFooterRowCols();
        footerRowCols.forEach(frc => {
            frc.querySelectorAll(".slick-footerrow-column")
                .forEach((el) => {
                    var columnDef = this.getColumnFromNode(el);
                    if (columnDef) {
                        this.trigger(this.onBeforeFooterRowCellDestroy, {
                            node: el as HTMLElement,
                            column: columnDef
                        });
                    }
                })

            if (this._jQuery) {
                this._jQuery(frc).empty();
            }
            else
                frc.innerHTML = '';
        });

        var cols = this._cols;
        for (var i = 0; i < cols.length; i++) {
            var m = cols[i];

            const footerRowCell = <div class={"slick-footerrow-column l" + i + " r" + i} /> as HTMLElement;
            footerRowCell.dataset.c = i.toString();
            this._jQuery && this._jQuery(footerRowCell).data("column", m);

            if (m.footerCssClass)
                addClass(footerRowCell, m.footerCssClass);
            else if (m.cssClass)
                addClass(footerRowCell, m.cssClass);

            this._layout.getFooterRowColsFor(i).appendChild(footerRowCell);

            this.trigger(this.onFooterRowCellRendered, {
                node: footerRowCell,
                column: m
            });
        }
    }

    private createColumnHeaders(): void {
        const headerCols = this._layout.getHeaderCols();
        headerCols.forEach(hc => {
            hc.querySelectorAll(".slick-header-column")
                .forEach((el) => {
                    var columnDef = this.getColumnFromNode(el);
                    if (columnDef) {
                        this.trigger(this.onBeforeHeaderCellDestroy, {
                            node: el as HTMLElement,
                            column: columnDef
                        });
                    }
                });

            this._emptyNode(hc);
        });

        this._layout.updateHeadersWidth();

        const headerRowCols = this._layout.getHeaderRowCols();
        headerRowCols.forEach(hrc => {
            hrc.querySelectorAll(".slick-headerrow-column")
                .forEach((el) => {
                    var columnDef = this.getColumnFromNode(el);
                    if (columnDef) {
                        this.trigger(this.onBeforeHeaderRowCellDestroy, {
                            node: el as HTMLElement,
                            column: columnDef,
                            grid: this
                        });
                    }
                });
            if (this._jQuery) {
                this._jQuery(hrc).empty();
            } else {
                hrc.innerHTML = "";
            }
        });

        const cols = this._cols, pinnedStartLast = this._layout.getPinnedStartLastCol(), pinnedEndFirst = this._layout.getPinnedEndFirstCol();
        for (let i = 0; i < cols.length; i++) {
            const m = cols[i];

            const nameSpan = <span class="slick-column-name" /> as HTMLElement;

            if (typeof m.nameFormat === "function") {
                const ctx = this.getFormatterContext(-1, i);
                (ctx as any).enableHtmlRendering = false;
                ctx.value = m.name;
                const fmtResult = m.nameFormat(ctx);
                applyFormatterResultToCellNode(ctx, fmtResult, nameSpan, contentOnly);
            }
            else
                nameSpan.textContent = m.name ?? '';

            const header = this._layout.getHeaderColsFor(i).appendChild(
                <div class={["slick-header-column", m.headerCssClass,
                    i <= pinnedStartLast && "frozen pinned-start",
                    i >= pinnedEndFirst && "frozen pinned-end",
                    m.sortable && "slick-header-sortable"]}
                    data-id={m.id} data-c={i} id={`${this._uid}${m.id}`} title={m.toolTip || ""}
                    style={{ width: `${m.width - this._headerColumnWidthDiff}px` }}>
                    {nameSpan}
                    {m.sortable && <span class="slick-sort-indicator" />}
                </div> as HTMLElement);

            this._jQuery?.(header).data("column", m);
            this.trigger(this.onHeaderCellRendered, { node: header, column: m });

            if (this._options.showHeaderRow) {
                const headerRowCell = this._layout.getHeaderRowColsFor(i).appendChild(
                    <div class={"slick-headerrow-column l" + i + " r" + i} data-c={i} /> as HTMLElement);
                this._jQuery?.(headerRowCell).data("column", m);
                this.trigger(this.onHeaderRowCellRendered, { node: headerRowCell, column: m });
            }
        }

        this.setSortColumns(this._sortColumns);
        this.setupColumnResize();
        if (this._options.enableColumnReorder) {
            this.setupColumnReorder();
            // sortable js removes draggable attribute after disposing / recreating
            this._layout.getHeaderCols().forEach(el => el.querySelectorAll<HTMLDivElement>(".slick-resizable-handle").forEach(x => x.draggable = true));
        }
    }

    private setupColumnSort(): void {
        const handler = (e: MouseEvent) => {
            var tgt = e.target as Element;
            if (tgt.classList.contains("slick-resizable-handle")) {
                return;
            }

            var colNode = tgt.closest(".slick-header-column");
            if (!colNode) {
                return;
            }

            var column = this.getColumnFromNode(colNode);
            if (column.sortable) {
                if (!this.getEditorLock().commitCurrentEdit()) {
                    return;
                }

                var sortOpts = null;
                var i = 0;
                for (; i < this._sortColumns.length; i++) {
                    if (this._sortColumns[i].columnId == column.id) {
                        sortOpts = this._sortColumns[i];
                        sortOpts.sortAsc = !sortOpts.sortAsc;
                        break;
                    }
                }

                if (e.metaKey && this._options.multiColumnSort) {
                    if (sortOpts) {
                        this._sortColumns.splice(i, 1);
                    }
                }
                else {
                    if ((!e.shiftKey && !e.metaKey) || !this._options.multiColumnSort) {
                        this._sortColumns = [];
                    }

                    if (!sortOpts) {
                        sortOpts = { columnId: column.id, sortAsc: column.defaultSortAsc };
                        this._sortColumns.push(sortOpts);
                    } else if (this._sortColumns.length == 0) {
                        this._sortColumns.push(sortOpts);
                    }
                }

                this.setSortColumns(this._sortColumns);

                if (!this._options.multiColumnSort) {
                    this.trigger(this.onSort, {
                        multiColumnSort: false,
                        sortCol: column,
                        sortAsc: sortOpts.sortAsc
                    }, e);
                } else {
                    var cols = this._initCols;
                    this.trigger(this.onSort, {
                        multiColumnSort: true,
                        sortCols: this._sortColumns.map(col => ({ sortCol: cols[this.getInitialColumnIndex(col.columnId)], sortAsc: col.sortAsc }))
                    }, e);
                }
            }
        };

        this._layout.getHeaderCols().forEach(el => {
            if (this._jQuery)
                this._jQuery(el).on('click', handler as any);
            else
                el.addEventListener("click", handler);
        });
    }

    private static offset(el: HTMLElement | null) {
        if (!el || !el.getBoundingClientRect)
            return;
        const box = el.getBoundingClientRect();
        const docElem = document.documentElement;
        return {
            top: box.top + window.scrollY - docElem.clientTop,
            left: box.left + window.scrollX - docElem.clientLeft
        };
    }

    declare private sortableColInstances: any[];

    private hasPinnedCols(): boolean {
        return this._layout.getPinnedStartLastCol() >= 0 || this._layout.getPinnedEndFirstCol() != Infinity;
    }

    private setupColumnReorder(): void {

        // @ts-ignore
        if (typeof Sortable === "undefined")
            return;

        this.sortableColInstances?.forEach(x => x.destroy());
        let columnScrollTimer: number = null;

        const scrollColumnsLeft = () => this._layout.getScrollContainerX().scrollLeft = this._layout.getScrollContainerX().scrollLeft + 10;
        const scrollColumnsRight = () => this._layout.getScrollContainerX().scrollLeft = this._layout.getScrollContainerX().scrollLeft - 10;

        let canDragScroll;
        const hasPinnedCols = this.hasPinnedCols();
        const sortableOptions: any = {
            animation: 50,
            direction: 'horizontal',
            chosenClass: 'slick-header-column-active',
            ghostClass: 'slick-sortable-placeholder',
            draggable: '.slick-header-column',
            filter: ".slick-resizable-handle",
            preventOnFilter: false,
            dragoverBubble: false,
            revertClone: true,
            scroll: !hasPinnedCols,
            onStart: (e: { item: any; originalEvent: MouseEvent; }) => {
                canDragScroll = !hasPinnedCols ||
                    Grid.offset(e.item)!.left > Grid.offset(this._layout.getScrollContainerX())!.left;

                if (canDragScroll && e.originalEvent && e.originalEvent.pageX > this._container.clientWidth) {
                    if (!(columnScrollTimer)) {
                        columnScrollTimer = setInterval(scrollColumnsRight, 100);
                    }
                } else if (canDragScroll && e.originalEvent && e.originalEvent.pageX < Grid.offset(this._layout.getScrollContainerX())!.left) {
                    if (!(columnScrollTimer)) {
                        columnScrollTimer = setInterval(scrollColumnsLeft, 100);
                    }
                } else {
                    clearInterval(columnScrollTimer);
                    columnScrollTimer = null;
                }
            },
            onEnd: (e: MouseEvent & { item: any; originalEvent: MouseEvent; }) => {
                const cancel = false;
                clearInterval(columnScrollTimer);
                columnScrollTimer = null;
                if (cancel || !this.getEditorLock()?.commitCurrentEdit()) {
                    return;
                }

                var reorderedCols;
                this._layout.getHeaderCols().forEach((el, i) => reorderedCols = sortToDesiredOrderAndKeepRest(
                    this._initCols,
                    (this.sortableColInstances[i]?.toArray?.() ?? [])
                ));

                this.setColumns(reorderedCols);
                this.trigger(this.onColumnsReordered, {});
                e.stopPropagation();
                this.setupColumnResize();
                if (this._activeCellNode) {
                    this.setFocus(); // refocus on active cell
                }
            }
        }

        // @ts-ignore
        this.sortableColInstances = this._layout.getHeaderCols().map(x => Sortable.create(x, sortableOptions));
    }

    private setupColumnResize(): void {

        var minPageX: number, pageX: number, maxPageX: number, cols = this._cols;
        var columnElements: Element[] = [];
        this._layout.getHeaderCols().forEach(el => {
            columnElements = columnElements.concat(Array.from(el.children));
        });

        var j: number, c: Column<TItem>, pageX: number, minPageX: number, maxPageX: number, firstResizable: number, lastResizable: number, cols = this._cols;
        var firstResizable: number, lastResizable: number;
        columnElements.forEach((el, i) => {
            var handle = el.querySelector(".slick-resizable-handle");
            handle && this._removeNode(handle);
            if (cols[i].resizable) {
                if (firstResizable === undefined) {
                    firstResizable = i;
                }
                lastResizable = i;
            }
        });

        if (firstResizable === undefined) {
            return;
        }

        const noJQueryDrag = !this._jQuery || !this._jQuery.fn || !(this._jQuery.fn as any).drag;
        columnElements.forEach((el, colIdx) => {

            if (colIdx < firstResizable || (this._options.forceFitColumns && colIdx >= lastResizable)) {
                return;
            }

            const handle = el.appendChild(document.createElement('div'));
            handle.classList.add('slick-resizable-handle');
            handle.draggable = true;

            var docDragOver: any = null;
            var lastDragOverPos: any = null;

            const dragStart = (e: DragEvent) => {
                if (!this.getEditorLock().commitCurrentEdit()) {
                    !noJQueryDrag && e.preventDefault();
                    return;
                }

                if (noJQueryDrag) {
                    docDragOver = (z: DragEvent) => {
                        lastDragOverPos = { pageX: z.pageX, pageY: z.pageY };
                        z.preventDefault();
                    }
                    document.addEventListener('dragover', docDragOver);
                }

                pageX = e.pageX;
                (e.target as HTMLElement).parentElement?.classList.add("slick-header-column-active");

                // lock each column's width option to current width
                columnElements.forEach((e, z) => {
                    cols[z].previousWidth = (e as HTMLElement).offsetWidth;
                });

                const minMax = calcMinMaxPageXOnDragStart(cols, colIdx, pageX, this._options.forceFitColumns, this._absoluteColMinWidth);
                maxPageX = minMax.maxPageX;
                minPageX = minMax.minPageX;

                noJQueryDrag && (e.dataTransfer.effectAllowed = 'move');
            };

            const drag = (e: DragEvent) => {
                var dist;
                if (noJQueryDrag) {
                    var thisPageX = (!e.pageX && !e.pageY) ? lastDragOverPos?.pageX : e.pageX;
                    var thisPageY = (!e.pageX && !e.pageY) ? lastDragOverPos?.pageY : e.pageY;
                    if (!thisPageX && !e.clientX && !thisPageY && !e.clientY)
                        return;
                    dist = Math.min(maxPageX, Math.max(minPageX, thisPageX)) - pageX;
                    e.dataTransfer.effectAllowed = 'none';
                    e.preventDefault();
                }
                else {
                    dist = Math.min(maxPageX, Math.max(minPageX, e.pageX)) - pageX;
                }
                if (isNaN(dist)) {
                    return;
                }
                shrinkOrStretchColumn(cols, colIdx, dist, this._options.forceFitColumns, this._absoluteColMinWidth);

                this._layout.afterHeaderColumnDrag();

                this.applyColumnHeaderWidths();
                if (this._options.syncColumnCellResize) {
                    this._layout.applyColumnWidths();
                }
            }

            const dragEnd = (e: any) => {
                if (docDragOver) {
                    document.removeEventListener('dragover', docDragOver);
                    docDragOver = null;
                }
                (e.target.parentElement as HTMLElement)?.classList.remove("slick-header-column-active");
                for (j = 0; j < columnElements.length; j++) {
                    c = cols[j];
                    var newWidth = (columnElements[j] as HTMLElement).offsetWidth;

                    if (c.previousWidth !== newWidth && c.rerenderOnResize) {
                        this.invalidateAllRows();
                    }
                }
                this.columnsResized(false);
            }

            if (noJQueryDrag) {
                handle.addEventListener("dragstart", dragStart);
                handle.addEventListener("drag", drag);
                handle.addEventListener("dragend", dragEnd);
                handle.addEventListener("dragover", (e: any) => { e.preventDefault(); e.dataTransfer.effectAllowed = "move"; });
            }
            else {
                (this._jQuery(handle) as any)
                    .on("dragstart", dragStart)
                    .on("drag", drag)
                    .on("dragend", dragEnd);
            }
        });
    }

    public columnsResized(invalidate = true) {
        this.applyColumnHeaderWidths();
        this._layout.applyColumnWidths();
        invalidate && this.invalidateAllRows();
        this.updateCanvasWidth(true);
        this.render();
        this.trigger(this.onColumnsResized);
    }

    private setOverflow(): void {
        this._layout.setOverflow();
        if (this._options.viewportClass)
            this.getViewports().forEach(vp => addClass(vp, this._options.viewportClass));
    }

    private measureCellPaddingAndBorder(): void {
        const h = ["border-left-width", "border-right-width", "padding-left", "padding-right"];
        const v = ["border-top-width", "border-bottom-width", "padding-top", "padding-bottom"];

        let el = this._layout.getHeaderColsFor(0).appendChild(<div class="slick-header-column" style="visibility: hidden" /> as HTMLElement);
        this._headerColumnWidthDiff = 0;
        let cs = getComputedStyle(el);
        if (cs.boxSizing != "border-box")
            h.forEach(val => this._headerColumnWidthDiff += parsePx(cs.getPropertyValue(val)) || 0);
        el.remove();

        const r = this._layout.getCanvasNodeFor(0, 0).appendChild(<div class="slick-row">
            {el = <div class="slick-cell" id="" style="visibility: hidden" /> as HTMLElement};
        </div>);
        el.innerHTML = "-";
        this._cellWidthDiff = this._cellHeightDiff = 0;
        cs = getComputedStyle(el);
        if (cs.boxSizing != "border-box") {
            h.forEach(val => this._cellWidthDiff += parsePx(cs.getPropertyValue(val)) || 0);
            v.forEach(val => this._cellHeightDiff += parsePx(cs.getPropertyValue(val)) || 0);
        }
        r.remove();

        this._absoluteColMinWidth = Math.max(this._headerColumnWidthDiff, this._cellWidthDiff);
    }

    private createCssRules() {
        var cellHeight = (this._options.rowHeight - this._cellHeightDiff);

        if (this._options.useCssVars && this.getColumns().length > 50)
            this._options.useCssVars = false;

        this._container.classList.toggle('sleek-vars', !!this._options.useCssVars);

        if (this._options.useCssVars) {
            var style = this._container.style;
            style.setProperty("--sleek-row-height", this._options.rowHeight + "px");
            style.setProperty("--sleek-cell-height", cellHeight + "px");
            style.setProperty("--sleek-scrollbar-width", this._scrollDims.width + "px");
            style.setProperty("--sleek-scrollbar-height", this._scrollDims.height + "px");
            style.setProperty("--sleek-top-panel-height", this._options.topPanelHeight + "px");
            style.setProperty("--sleek-grouping-panel-height", this._options.groupingPanelHeight + "px");
            style.setProperty("--sleek-headerrow-height", this._options.headerRowHeight + "px");
            style.setProperty("--sleek-footerrow-height", this._options.footerRowHeight + "px");
            return;
        }

        var el = this._styleNode = document.createElement('style');
        el.dataset.uid = this._uid;
        var rules = [
            "." + this._uid + " { --sleek-cell-height: " + this._options.rowHeight + "px; }",
            "." + this._uid + " { --sleek-scrollbar-width: " + this._scrollDims.width + "px; }",
            "." + this._uid + " { --sleek-scrollbar-height: " + this._scrollDims.height + "px; }",
            "." + this._uid + " .slick-top-panel { height:" + this._options.topPanelHeight + "px; }",
            "." + this._uid + " .slick-grouping-panel { height:" + this._options.groupingPanelHeight + "px; }",
            "." + this._uid + " .slick-headerrow-columns { height:" + this._options.headerRowHeight + "px; }",
            "." + this._uid + " .slick-cell { height:" + cellHeight + "px; }",
            "." + this._uid + " .slick-row { height:" + this._options.rowHeight + "px; }",
            "." + this._uid + " .slick-footerrow-columns { height:" + this._options.footerRowHeight + "px; }",
            "." + this._uid + " .slick-spacer-h { margin-right:" + this._scrollDims.width + "px; }",
        ];

        var cols = this._cols;
        for (var i = 0; i < cols.length; i++) {
            rules.push("." + this._uid + " .l" + i + " { }");
            rules.push("." + this._uid + " .r" + i + " { }");
        }

        el.appendChild(document.createTextNode(rules.join(" ")));
        document.head.appendChild(el);
    }

    private getColumnCssRules(idx: number): { right: any; left: any; } {
        if (this._options.useCssVars)
            return null;

        if (!this._stylesheet) {
            var stylesheetFromUid = document.querySelector("style[data-uid='" + this._uid + "']") as any
            if (stylesheetFromUid && stylesheetFromUid.sheet) {
                this._stylesheet = stylesheetFromUid.sheet;
            } else {
                var sheets = document.styleSheets;
                for (var i = 0; i < sheets.length; i++) {
                    if ((sheets[i].ownerNode || (sheets[i] as any).owningElement) == this._styleNode) {
                        this._stylesheet = sheets[i];
                        break;
                    }
                }
            }

            if (!this._stylesheet) {
                throw new Error("Cannot find stylesheet.");
            }

            // find and cache column CSS rules
            this._columnCssRulesL = [];
            this._columnCssRulesR = [];
            var cssRules = (this._stylesheet.cssRules || this._stylesheet.rules);
            var matches, columnIdx;
            for (var i = 0; i < cssRules.length; i++) {
                var selector = cssRules[i].selectorText;
                if (matches = /\.l\d+/.exec(selector)) {
                    columnIdx = parseInt(matches[0].substring(2, matches[0].length), 10);
                    this._columnCssRulesL[columnIdx] = cssRules[i];
                } else if (matches = /\.r\d+/.exec(selector)) {
                    columnIdx = parseInt(matches[0].substring(2, matches[0].length), 10);
                    this._columnCssRulesR[columnIdx] = cssRules[i];
                }
            }
        }

        return this._options.rtl ? {
            "right": this._columnCssRulesL[idx],
            "left": this._columnCssRulesR[idx]
        } : {
            "left": this._columnCssRulesL[idx],
            "right": this._columnCssRulesR[idx]
        }
    }

    private removeCssRules() {
        this._styleNode?.remove();
        this._styleNode = null;
        this._stylesheet = null;
    }

    destroy() {
        this.getEditorLock().cancelCurrentEdit();

        this.trigger(this.onBeforeDestroy);

        var i = this._plugins.length;
        while (i--) {
            this.unregisterPlugin(this._plugins[i]);
        }

        if (this._draggableInstance) {
            this._draggableInstance.destroy();
            this._draggableInstance = null;
        }

        if (this._options.enableColumnReorder && this._jQuery && (this._jQuery.fn as any).sortable) {
            (this._jQuery(this._layout.getHeaderCols()).filter(":ui-sortable") as any).sortable("destroy");
        }

        this.unbindAncestorScrollEvents();
        this.unbindFromData();
        this.unregisterSelectionModel();
        this._jQuery?.(this._container).off(".slickgrid");
        this.removeCssRules();

        var canvasNodes = this._layout.getCanvasNodes();
        if (this._jQuery)
            this._jQuery(canvasNodes).off("draginit dragstart dragend drag");
        else
            canvasNodes.forEach(el => this._removeNode(el));

        for (var k in this) {
            if (!Object.prototype.hasOwnProperty.call(this, k))
                continue;
            if (k.startsWith('on')) {
                var ev: any = this[k];
                if ((ev as EventEmitter)?.clear && (ev as EventEmitter)?.subscribe)
                    (ev as EventEmitter)?.clear();
            }
            delete this[k];
        }
    }


    //////////////////////////////////////////////////////////////////////////////////////////////
    // General

    private trigger<TArgs extends ArgsGrid, TEventData extends IEventData = IEventData>(
        evt: EventEmitter<TArgs, TEventData>, args?: TArgs, e?: TEventData) {
        e = e || new EventData() as any;
        args = args || {} as any;
        args.grid = this;
        return evt.notify(args, e, this);
    }

    getEditorLock(): EditorLock {
        return this._options.editorLock;
    }

    getEditController(): EditController {
        return this._editController;
    }

    /** Gets a column by its ID. May also return non visible columns */
    getColumnById(id: string): Column<TItem> {
        return id ? this._cols[this._colById[id]] : null;
    }

    /** Returns a column's index in the visible columns list by its column ID */
    getColumnIndex(id: string): number {
        return id ? this._colById[id] : null;
    }

    /** Gets index of a column in the initial column list passed to the grid, or setColumns method. May include invisible cols and index does not have to match visible column order. */
    getInitialColumnIndex(id: string): number {
        return id ? this._initColById[id] : null;
    }

    /** Gets a view (e.g. visible) column by its column ID */
    getVisibleColumnById(id: string): Column<TItem> {
        return id ? this._cols[this._colById[id]] : null;
    }

    autosizeColumns(): void {
        var vpi = this._viewportInfo,
            availWidth = vpi.hasVScroll ? vpi.width - this._scrollDims.width : vpi.width;

        var reRender = autosizeColumns(this._cols, availWidth, this._absoluteColMinWidth);

        this.applyColumnHeaderWidths();
        this.updateCanvasWidth(true);
        if (reRender) {
            this.invalidateAllRows();
            this.render();
        }
    }

    private applyColumnHeaderWidths(): void {
        if (!this._initialized) { return; }

        var h: HTMLElement;
        for (var i = 0, cols = this._cols, colCount = cols.length, diff = this._headerColumnWidthDiff; i < colCount; i++) {
            h = this._layout.getHeaderColumn(i);
            if (h) {
                var target = cols[i].width - diff;
                if (h.offsetWidth !== target) {
                    h.style.width = target + 'px'
                }
            }
        }

        this.updateViewColLeftRight();
    }

    setSortColumn(columnId: string, ascending: boolean) {
        this.setSortColumns([{ columnId: columnId, sortAsc: ascending }]);
    }

    setSortColumns(cols: ColumnSort[]) {
        this._sortColumns = cols || [];

        var headerColumnEls: Element[] = [];
        this._layout.getHeaderCols().forEach(el => headerColumnEls = headerColumnEls.concat(Array.from(el.children)));
        headerColumnEls.forEach(hel => {
            hel.classList.remove("slick-header-column-sorted");
            var si = hel.querySelector(".slick-sort-indicator");
            si && si.classList.remove("slick-sort-indicator-asc", "slick-sort-indicator-desc");
        });

        this._sortColumns.forEach(col => {
            if (col.sortAsc == null) {
                col.sortAsc = true;
            }
            var columnIndex = this.getColumnIndex(col.columnId);
            if (columnIndex != null) {
                var header = headerColumnEls[columnIndex];
                if (header) {
                    header.classList.add("slick-header-column-sorted");
                    var si = header.querySelector(".slick-sort-indicator");
                    si && si.classList.add(col.sortAsc ? "slick-sort-indicator-asc" : "slick-sort-indicator-desc");
                }
            }
        });
    }

    getSortColumns(): ColumnSort[] {
        return this._sortColumns;
    }

    private handleSelectedRangesChanged = (e: IEventData, ranges: CellRange[]): void => {
        var previousSelectedRows = this._selectedRows.slice(0); // shallow copy previously selected rows for later comparison
        this._selectedRows = [];
        var hash: any = {}, cols = this._cols;
        for (var i = 0; i < ranges.length; i++) {
            for (var j = ranges[i].fromRow; j <= ranges[i].toRow; j++) {
                if (!hash[j]) {  // prevent duplicates
                    this._selectedRows.push(j);
                    hash[j] = Object.create(null);
                }
                for (var k = ranges[i].fromCell; k <= ranges[i].toCell; k++) {
                    if (this.canCellBeSelected(j, k)) {
                        const cid = cols[k].id;
                        if (!isPollutingKey(cid)) {
                            hash[j][cid] = this._options.selectedCellCssClass;
                        }
                    }
                }
            }
        }

        this.setCellCssStyles(this._options.selectedCellCssClass, hash);

        if (!simpleArrayEquals(previousSelectedRows, this._selectedRows)) {
            var caller = e && (e as any).detail && (e as any).detail.caller || 'click';
            var newSelectedAdditions = this._selectedRows.filter(i => previousSelectedRows.indexOf(i) < 0);
            var newSelectedDeletions = previousSelectedRows.filter(i => this._selectedRows.indexOf(i) < 0);

            this.trigger(this.onSelectedRowsChanged, {
                rows: this.getSelectedRows(),
                previousSelectedRows: previousSelectedRows,
                caller: caller,
                changedSelectedRows: newSelectedAdditions,
                changedUnselectedRows: newSelectedDeletions
            }, e);
        }

        this._selectedRows = [];
        hash = {}, cols = this._cols;
        for (var i = 0; i < ranges.length; i++) {
            for (var j = ranges[i].fromRow; j <= ranges[i].toRow; j++) {
                if (!hash[j]) {  // prevent duplicates
                    this._selectedRows.push(j);
                    hash[j] = Object.create(null);
                }
                for (var k = ranges[i].fromCell; k <= ranges[i].toCell; k++) {
                    if (this.canCellBeSelected(j, k)) {
                        const cid = cols[k].id;
                        if (!isPollutingKey(cid)) {
                            hash[j][cid] = this._options.selectedCellCssClass;
                        }
                    }
                }
            }
        }
    }

    /** Returns only the visible columns in order */
    getColumns(): Column<TItem>[] {
        return this._cols;
    }

    /** Returns list of columns passed to the grid constructor, or setColumns method. May include invisible columns and order does not match visible column order. */
    getInitialColumns(): Column<TItem>[] {
        return this._initCols;
    }

    private updateViewColLeftRight(): void {
        this._colLeft = [];
        this._colRight = [];
        var x = 0, r: number, cols = this._cols, i: number, l: number = cols.length,
            pinnedStartLast = this._layout.getPinnedStartLastCol(), pinnedEndFirst = this._layout.getPinnedEndFirstCol();
        for (var i = 0; i < l; i++) {
            if (pinnedStartLast === i || pinnedEndFirst === i)
                x = 0;
            r = x + cols[i].width;
            this._colLeft[i] = x;
            this._colRight[i] = r;
            x = r;
        }
    }

    private setInitialCols(initCols: Column[]) {

        initializeColumns(initCols, this._colDefaults);

        var initColById: any = {};
        var viewCols: Column[] = [];
        var viewColById: { [key: string]: number } = {};
        var i: number, m: Column;
        for (i = 0; i < initCols.length; i++) {
            m = initCols[i];
            initColById[m.id] = i;
            if (m.visible !== false)
                viewCols.push(m);
        }

        viewCols = this._layout.reorderViewColumns(viewCols, this._options) ?? viewCols;

        this._postRenderActive = this._options.enableAsyncPostRender ?? false;
        this._postCleanupActive = this._options.enableAsyncPostRenderCleanup ?? false;
        for (i = 0; i < viewCols.length; i++) {
            m = viewCols[i];
            viewColById[m.id] = i;
            if (m.asyncPostRenderCleanup != null)
                this._postCleanupActive = true;
            if (m.asyncPostRender != null)
                this._postRenderActive = true;
        }

        this._initCols = initCols;
        this._initColById = initColById;
        this._cols = viewCols;
        this._colById = viewColById;
    }

    setColumns(columns: Column<TItem>[]): void {

        if (columns &&
            this._initCols &&
            this._cols &&
            columns.length === this._cols.length &&
            this._initCols.length > this._cols.length &&
            !columns.some(x => this._cols.indexOf(x) < 0) &&
            !this._cols.some(x => columns.indexOf(x) < 0)) {
            // probably called with grid.setColumns(grid.getColumns()) potentially changing orders / widths etc
            // try to preserve initial columns
            sortToDesiredOrderAndKeepRest(
                this._initCols,
                columns.map(x => x.id));
            columns = this._initCols;
        }

        this.setInitialCols(columns);
        this.updateViewColLeftRight();

        if (this._initialized) {
            this._layout.setPaneVisibility();
            this.setOverflow();

            this.invalidateAllRows();
            this.createColumnHeaders();
            this.createColumnFooters();
            this.updateGrandTotals();
            this.removeCssRules();
            this.createCssRules();
            this.resizeCanvas();
            this.updateCanvasWidth();
            this._layout.applyColumnWidths();
            this.handleScroll();
            this.getSelectionModel()?.refreshSelections?.();
        }
    }

    getOptions(): GridOptions<TItem> {
        return this._options;
    }

    setOptions(args: GridOptions<TItem>, suppressRender?: boolean, suppressColumnSet?: boolean, suppressSetOverflow?: boolean): void {
        if (!this.getEditorLock().commitCurrentEdit()) {
            return;
        }

        this.makeActiveCellNormal();

        if (args.groupingPanel && !this._options.groupingPanel)
            this.createGroupingPanel();
        else if (args.groupingPanel != void 0 && !args.groupingPanel && this._groupingPanel)
            this._removeNode(this._groupingPanel);

        if (args.showColumnHeader !== undefined) {
            this.setColumnHeaderVisibility(args.showColumnHeader);
        }

        if (this._options.enableAddRow !== args.enableAddRow) {
            this.invalidateRow(this.getDataLength());
        }

        this._options = Object.assign(this._options, args);
        this.validateAndEnforceOptions();
        this.setOptionSignals();
        this._layout.afterSetOptions(args);

        if (args.columns && !suppressColumnSet) {
            this.setColumns(args.columns ?? this._initCols);
        }

        if (!suppressSetOverflow) {
            this.setOverflow();
        }

        this._layout.setScroller();
        if (!suppressRender)
            this.render();
    }

    private validateAndEnforceOptions(): void {
        if (this._options.autoHeight) {
            this._options.leaveSpaceForNewRows = false;
        }
    }

    private setOptionSignals() {
        const sig = this._optionSignals;
        const opt = this._options;
        for (let k in sig) {
            (sig as any)[k].value = (opt as any)[k];
        }
    }

    private viewOnRowCountChanged = () => {
        this.updateRowCount();
        this.render();
    }

    private viewOnRowsChanged = (_: any, args: { rows: number[] }): void => {
        this.invalidateRows(args.rows);
        this.render();
        this.updateGrandTotals();
    }

    private viewOnDataChanged = (): void => {
        this.invalidate();
        this.render();
    }

    private bindToData(): void {
        const view = this._data as IDataView;
        if (view) {
            view.onRowCountChanged && view.onRowCountChanged.subscribe(this.viewOnRowCountChanged);
            view.onRowsChanged && view.onRowsChanged.subscribe(this.viewOnRowsChanged);
            view.onDataChanged && view.onDataChanged.subscribe(this.viewOnDataChanged);
        }
    }

    private unbindFromData(): void {
        const view = this._data as IDataView;
        if (view) {
            view.onRowCountChanged && view.onRowCountChanged.unsubscribe(this.viewOnRowCountChanged);
            view.onRowsChanged && view.onRowsChanged.unsubscribe(this.viewOnRowsChanged);
            view.onDataChanged && view.onDataChanged.unsubscribe(this.viewOnDataChanged);
        }
    }

    setData(newData: any, scrollToTop?: boolean) {
        this.unbindFromData();
        this._data = newData;
        this.bindToData();
        this.invalidateAllRows();
        this.updateRowCount();
        if (scrollToTop) {
            this.scrollTo(0);
        }
    }

    getData(): any {
        return this._data;
    }

    getDataLength(): number {
        if ((this._data as IDataView).getLength) {
            return (this._data as IDataView).getLength();
        } else {
            return (this._data as TItem[]).length;
        }
    }

    private getDataLengthIncludingAddNew(): number {
        return this.getDataLength() + (!this._options.enableAddRow ? 0 :
            (!this._pagingActive || this._pagingIsLastPage ? 1 : 0));
    }

    getDataItem(row: number): TItem {
        if ((this._data as IDataView).getItem) {
            return (this._data as IDataView).getItem(row);
        } else {
            return (this._data as TItem[])[row];
        }
    }

    getTopPanel(): HTMLElement {
        return this._layout.getTopPanelFor(0);
    }

    setTopPanelVisibility(visible: boolean): void {
        if (this._options.showTopPanel != visible) {
            this._options.showTopPanel = this._optionSignals.showTopPanel.value = !!visible;
            this.resizeCanvas();
        }
    }

    setColumnHeaderVisibility(visible: boolean) {
        if (this._options.showColumnHeader != visible) {
            this._options.showColumnHeader = visible;
            this._layout.getHeaderCols().forEach(n => n.parentElement?.classList.toggle("slick-hidden", !visible));
            this.resizeCanvas();
        }
    }

    setFooterRowVisibility(visible: boolean): void {
        if (this._options.showFooterRow != visible) {
            this._options.showFooterRow = this._optionSignals.showFooterRow.value = !!visible;
            this.resizeCanvas();
        }
    }

    setGroupingPanelVisibility(visible: boolean): void {
        if (this._options.showGroupingPanel != visible) {
            this._options.showGroupingPanel = visible;
            this._groupingPanel?.classList.toggle("slick-hidden", !visible);
            this.resizeCanvas();
        }
    }

    setPreHeaderPanelVisibility(visible: boolean): void {
        this.setGroupingPanelVisibility(visible);
    }

    setHeaderRowVisibility(visible: boolean): void {
        if (this._options.showHeaderRow != visible) {
            this._options.showHeaderRow = this._optionSignals.showHeaderRow.value = !!visible;
            this.resizeCanvas();
        }
    }

    getContainerNode(): HTMLElement {
        return this._container;
    }

    getUID(): string {
        return this._uid;
    }

    //////////////////////////////////////////////////////////////////////////////////////////////
    // Rendering / Scrolling

    private getRowTop(row: number): number {
        return this._options.rowHeight * row - this._pageOffset;
    }

    private getRowFromPosition(y: number): number {
        return Math.floor((y + this._pageOffset) / this._options.rowHeight);
    }

    private scrollTo(y: number): void {
        const vpi = this._viewportInfo;
        y = Math.max(y, 0);
        y = Math.min(y, vpi.virtualHeight - Math.round(this._layout.getScrollContainerY().clientHeight) + ((vpi.hasHScroll || this.hasPinnedCols()) ? this._scrollDims.height : 0));

        var oldOffset = this._pageOffset;

        this._page = Math.min(this._numberOfPages - 1, Math.floor(y / this._pageHeight));
        this._pageOffset = Math.round(this._page * this._jumpinessCoefficient);
        var newScrollTop = y - this._pageOffset;

        if (this._pageOffset != oldOffset) {
            var range = this.getVisibleRange(newScrollTop);
            this.cleanupRows(range);
            this.updateRowPositions();
        }

        if (this._scrollTopPrev != newScrollTop) {
            this._vScrollDir = (this._scrollTopPrev + oldOffset < newScrollTop + this._pageOffset) ? 1 : -1;

            this._scrollTopRendered = (this._scrollTop = this._scrollTopPrev = newScrollTop);

            this._layout.handleScrollV();
            this._layout.getScrollContainerY().scrollTop = newScrollTop;

            this.trigger(this.onViewportChanged);
        }
    }

    getFormatter(row: number, column: Column<TItem>): ColumnFormat<TItem> {
        const view = this._data as IDataView;
        if (view.getItemMetadata) {
            const itemMetadata = view.getItemMetadata(row) as ItemMetadata;
            if (itemMetadata) {
                const colsMetadata = itemMetadata.columns;
                if (colsMetadata) {
                    var columnMetadata: ColumnMetadata = colsMetadata[column.id] || colsMetadata[this.getColumnIndex(column.id)];
                    if (columnMetadata) {
                        if (columnMetadata.format)
                            return columnMetadata.format;
                        if ((columnMetadata as any).formatter)
                            return convertCompatFormatter((columnMetadata as any).formatter);
                    }
                }
                if (itemMetadata.format)
                    return itemMetadata.format;
                if ((itemMetadata as any).formatter)
                    return convertCompatFormatter((itemMetadata as any).formatter);
            }
        }

        if (column.format)
            return column.format;

        if ((column as any).formatter)
            return convertCompatFormatter((column as any).formatter);

        var opt = this._options;

        var factory = opt.formatterFactory;
        if (factory) {
            if (factory.getFormat) {
                var format = factory.getFormat(column);
                if (format)
                    return format;
            }
            else if (factory.getFormatter) {
                var compat = factory.getFormatter(column);
                if (compat)
                    return convertCompatFormatter(compat);
            }
        }

        if (opt.defaultFormat)
            return opt.defaultFormat;

        if (opt.defaultFormatter)
            return convertCompatFormatter(opt.defaultFormatter);

        return defaultColumnFormat;
    }

    getFormatterContext(row: number, cell: number): FormatterContext {
        const column = this._cols[cell];
        const item = this.getDataItem(row);
        return formatterContext<TItem>({
            cell,
            column,
            grid: this,
            item,
            row,
            value: item ? this.getDataItemValueForColumn(item, column) : void 0
        });
    }

    getTotalsFormatter(column: Column<TItem>): ColumnFormat<TItem> {
        if (column.groupTotalsFormat)
            return column.groupTotalsFormat;

        if ((column as any).groupTotalsFormatter)
            return convertCompatFormatter((column as any).groupTotalsFormatter);

        const opt = this._options;
        if (opt.groupTotalsFormat)
            return opt.groupTotalsFormat;

        if ((opt as any).groupTotalsFormatter)
            return convertCompatFormatter((opt as any).groupTotalsFormatter);

        return null;
    }

    private getEditor(row: number, cell: number): EditorClass {
        var column = this._cols[cell];
        var itemMetadata = (this._data as IDataView).getItemMetadata?.(row) as ItemMetadata;
        var colsMetadata = itemMetadata && itemMetadata.columns;

        if (colsMetadata && colsMetadata[column.id] && colsMetadata[column.id].editor !== undefined) {
            return colsMetadata[column.id].editor;
        }
        if (colsMetadata && colsMetadata[cell] && colsMetadata[cell].editor !== undefined) {
            return colsMetadata[cell].editor;
        }

        return column.editor || (this._options.editorFactory && this._options.editorFactory.getEditor(column, row));
    }

    getDataItemValueForColumn(item: TItem, columnDef: Column<TItem>): any {
        if (this._options.dataItemColumnValueExtractor)
            return this._options.dataItemColumnValueExtractor(item, columnDef);
        return (item as any)[columnDef.field];
    }

    private appendRowHtml(sbStart: string[], sbCenter: string[], sbEnd: string[], row: number, range: ViewRange, dataLength: number): void {
        var d = this.getDataItem(row);
        var dataLoading = row < dataLength && !d;
        var rowCss = "slick-row" +
            (this._layout.isFrozenRow(row) ? ' frozen' : '') +
            (dataLoading ? " loading" : "") +
            (row === this._activeRow ? " active" : "") +
            (row % 2 == 1 ? " odd" : " even");

        if (!d) {
            rowCss += " " + this._options.addNewRowCssClass;
        }

        var itemMetadata = (this._data as IDataView).getItemMetadata?.(row);

        if (itemMetadata && itemMetadata.cssClasses) {
            rowCss += " " + itemMetadata.cssClasses;
        }

        var rowOffset = this._layout.getFrozenRowOffset(row);

        var rowHtml = "<div class='" + rowCss + "' style='top:"
            + (this.getRowTop(row) - rowOffset)
            + "px'>";

        sbCenter.push(rowHtml);

        const pinnedStartLast = this._layout.getPinnedStartLastCol();
        const pinnedEndFirst = this._layout.getPinnedEndFirstCol();

        if (pinnedStartLast >= 0) {
            sbStart.push(rowHtml);
        }

        if (pinnedEndFirst != Infinity) {
            sbEnd.push(rowHtml);
        }

        var colspan, m, cols = this._cols;
        for (var i = 0, ii = cols.length; i < ii; i++) {
            var columnData: ColumnMetadata = null;
            m = cols[i];
            colspan = 1;
            if (itemMetadata && itemMetadata.columns) {
                columnData = itemMetadata.columns[m.id] || itemMetadata.columns[i];
                colspan = (columnData && columnData.colspan) || 1;
                if (colspan === "*") {
                    colspan = ii - i;
                }
            }

            const pinnedStart = pinnedStartLast >= 0 && i <= pinnedStartLast;
            const pinnedEnd = pinnedEndFirst != Infinity && i >= pinnedEndFirst;
            // Do not render cells outside of the viewport.
            if (pinnedStart || pinnedEnd || this._colRight[Math.min(ii - 1, i + colspan - 1)] > range.leftPx) {
                if (!(pinnedStart || pinnedEnd) && this._colLeft[i] > range.rightPx) {
                    // All columns to the right are outside the range.
                    if (pinnedEndFirst != Infinity)
                        break;
                    i = pinnedEndFirst - 1;
                    continue;
                }

                this.appendCellHtml(pinnedStart ? sbStart : pinnedEnd ? sbEnd : sbCenter, row, i, colspan, d, columnData);
            }

            if (colspan > 1) {
                i += (colspan - 1);
            }
        }

        sbCenter.push("</div>");

        if (pinnedStartLast >= 0) {
            sbStart.push("</div>");
        }

        if (pinnedEndFirst != Infinity) {
            sbEnd.push("</div>");
        }
    }

    private appendCellHtml(sb: string[], row: number, cell: number, colspan: number, item: TItem, metadata: ColumnMetadata): void {
        var cols = this._cols, pinnedStartLast = this._layout.getPinnedStartLastCol(), pinnedEndFirst = this._layout.getPinnedEndFirstCol(), column = cols[cell];
        var klass = "slick-cell l" + cell + " r" + Math.min(cols.length - 1, cell + colspan - 1) +
            (column.cssClass ? " " + column.cssClass : "");

        if (cell <= pinnedStartLast)
            klass += ' frozen pinned-start';
        else if (cell >= pinnedEndFirst)
            klass += ' frozen pinned-end';

        if (row === this._activeRow && cell === this._activeCell)
            klass += " active";

        if (metadata && metadata.cssClasses) {
            klass += " " + metadata.cssClasses;
        }

        for (var key in this._cellCssClasses) {
            if (this._cellCssClasses[key][row] && (this._cellCssClasses[key][row] as any)[column.id]) {
                klass += (" " + this._cellCssClasses[key][row][column.id]);
            }
        }

        // if there is a corresponding row (if not, this is the Add New row or this data hasn't been loaded yet)
        var fmtResult: FormatterResult;
        const ctx = formatterContext<TItem>({
            cell,
            column,
            grid: this,
            item,
            row
        });

        if (item) {
            ctx.value = this.getDataItemValueForColumn(item, column);
            fmtResult = this.getFormatter(row, column)(ctx);
            if (typeof fmtResult === "string" && fmtResult.length) {
                if (ctx.enableHtmlRendering)
                    fmtResult = (ctx.sanitizer ?? escapeHtml)(fmtResult);
                else
                    fmtResult = escapeHtml(fmtResult);
            }
        }

        klass = escapeHtml(klass);

        if (ctx.addClass?.length || ctx.addAttrs?.length || ctx.tooltip?.length) {
            if (ctx.addClass?.length)
                klass += (" " + escapeHtml(ctx.addClass));

            sb.push('<div class="' + klass + '"');

            if (ctx.addClass?.length)
                sb.push(' data-fmtcls="' + escapeHtml(ctx.addClass) + '"');

            var attrs = ctx.addAttrs;
            if (attrs != null) {
                var ks = [];
                for (var k in attrs) {
                    sb.push(k + '="' + escapeHtml(attrs[k]) + '"');
                    ks.push(k);
                }
                sb.push(' data-fmtatt="' + escapeHtml(ks.join(',')) + '"');
            }

            var toolTip = ctx.tooltip;
            if (toolTip != null && toolTip.length)
                sb.push('tooltip="' + escapeHtml(toolTip) + '"');

            if (fmtResult != null && !(fmtResult instanceof Node))
                sb.push('>' + fmtResult + '</div>');
            else
                sb.push('></div>');
        }
        else if (fmtResult != null && !(fmtResult instanceof Node))
            sb.push('<div class="' + klass + '">' + fmtResult + '</div>');
        else
            sb.push('<div class="' + klass + '"></div>');

        var cache = this._rowsCache[row];
        cache.cellRenderQueue.push(cell);
        cache.cellRenderContent.push(fmtResult instanceof Node ? fmtResult : void 0);
        this._rowsCache[row].cellColSpans[cell] = colspan;
    }

    private cleanupRows(rangeToKeep: ViewRange): void {
        var i: number;
        for (var x in this._rowsCache) {
            i = parseInt(x, 10);
            if (i !== this._activeRow && (i < rangeToKeep.top || i > rangeToKeep.bottom)
                && !this._layout.isFrozenRow(i))
                this.removeRowFromCache(i);
        }

        this.startPostProcessingCleanup();
    }

    invalidate(): void {
        this.updateRowCount();
        this.invalidateAllRows();
        this.render();
        this.updateGrandTotals();
    }

    invalidateAllRows(): void {
        if (this._currentEditor) {
            this.makeActiveCellNormal();
        }
        for (var row in this._rowsCache) {
            this.removeRowFromCache(parseInt(row, 10));
        }

        this.startPostProcessingCleanup();
    }

    private queuePostProcessedRowForCleanup(cacheEntry: CachedRow, row: number): void {

        var postProcessedRow = this._postProcessedRows[row];
        if (!postProcessedRow)
            return;

        this._postProcessGroupId++;

        // store and detach node for later async cleanup
        for (var x in postProcessedRow) {
            var columnIdx = parseInt(x, 10);
            this._postProcessCleanupQueue.push({
                groupId: this._postProcessGroupId,
                cellNode: cacheEntry.cellNodesByColumnIdx[columnIdx | 0],
                columnIdx: columnIdx | 0,
                rowIdx: row
            });
        }

        this._postProcessCleanupQueue.push({
            groupId: this._postProcessGroupId,
            rowNodeS: cacheEntry.rowNodeS,
            rowNodeC: cacheEntry.rowNodeC,
            rowNodeE: cacheEntry.rowNodeE,
        });

        cacheEntry.rowNodeC?.remove();
        cacheEntry.rowNodeS?.remove();
        cacheEntry.rowNodeE?.remove();
    }

    private queuePostProcessedCellForCleanup(cellnode: HTMLElement, columnIdx: number, rowIdx: number): void {
        this._postProcessCleanupQueue.push({
            groupId: this._postProcessGroupId,
            cellNode: cellnode,
            columnIdx: columnIdx,
            rowIdx: rowIdx
        });
        cellnode.remove(); // cleanup should be handled by postprocess
    }

    private removeRowFromCache(row: number): void {
        var cacheEntry = this._rowsCache[row];
        if (!cacheEntry) {
            return;
        }

        if (this._postCleanupActive && this._postProcessedRows[row]) {
            this.queuePostProcessedRowForCleanup(cacheEntry, row);
        }
        else {
            cacheEntry.rowNodeS?.remove();
            cacheEntry.rowNodeC?.remove();
            cacheEntry.rowNodeE?.remove();
        }

        delete this._rowsCache[row];
        delete this._postProcessedRows[row];
    }

    invalidateRows(rows: number[]): void {
        var i, rl;
        if (!rows || !rows.length) {
            return;
        }
        this._vScrollDir = 0;
        for (i = 0, rl = rows.length; i < rl; i++) {
            if (this._currentEditor && this._activeRow === rows[i]) {
                this.makeActiveCellNormal();
            }
            if (this._rowsCache[rows[i]]) {
                this.removeRowFromCache(rows[i]);
            }
        }

        this.startPostProcessingCleanup();
    }

    invalidateRow(row: number): void {
        this.invalidateRows([row]);
    }

    updateCell(row: number, cell: number): void {
        var cellNode = this.getCellNode(row, cell);
        if (!cellNode)
            return;

        if (this._currentEditor && this._activeRow === row && this._activeCell === cell) {
            this._currentEditor.loadValue(this.getDataItem(row));
        } else {
            this.updateCellWithFormatter(cellNode, row, cell);
            this.invalidatePostProcessingResults(row);
        }
    }

    private updateCellWithFormatter(cellNode: HTMLElement, row: number, cell: number): void {
        let fmtResult: FormatterResult;
        const ctx = this.getFormatterContext(row, cell);
        if (ctx.item) {
            fmtResult = this.getFormatter(row, ctx.column)(ctx);
            if (typeof fmtResult === "string" && fmtResult.length) {
                fmtResult = (ctx.sanitizer ?? escapeHtml)(fmtResult);
            }
        }
        this._emptyNode(cellNode);
        applyFormatterResultToCellNode(ctx, fmtResult, cellNode);
    }

    updateRow(row: number): void {
        var cacheEntry = this._rowsCache[row];
        if (!cacheEntry) {
            return;
        }

        this.ensureCellNodesInRowsCache(row);

        var d = this.getDataItem(row);

        for (var x in cacheEntry.cellNodesByColumnIdx) {
            var cell = parseInt(x, 10);
            if (row === this._activeRow && cell === this._activeCell && this._currentEditor) {
                this._currentEditor.loadValue(d);
            }
            else {
                this.updateCellWithFormatter(cacheEntry.cellNodesByColumnIdx[cell], row, cell);
            }
        }

        this.invalidatePostProcessingResults(row);
    }

    private calcViewportSize(): void {
        const layout = this._layout;
        const vs = this._viewportInfo;
        vs.width = getInnerWidth(this._container);
        vs.groupingPanelHeight = (this._options.groupingPanel && this._options.showGroupingPanel) ? (this._options.groupingPanelHeight + getVBoxDelta(this._groupingPanel)) : 0
        vs.topPanelHeight = this._options.showTopPanel ? (this._options.topPanelHeight + getVBoxDelta(layout.getTopPanelFor(0).parentElement)) : 0;
        vs.headerRowHeight = this._options.showHeaderRow ? (this._options.headerRowHeight + getVBoxDelta(layout.getHeaderRowColsFor(0).parentElement)) : 0;
        vs.footerRowHeight = this._options.showFooterRow ? (this._options.footerRowHeight + getVBoxDelta(layout.getFooterRowColsFor(0).parentElement)) : 0;
        vs.headerHeight = (this._options.showColumnHeader) ? (parsePx(getComputedStyle(layout.getHeaderColsFor(0).parentElement).height) + getVBoxDelta(layout.getHeaderColsFor(0).parentElement)) : 0;

        if (this._options.autoHeight) {
            vs.height = this._options.rowHeight * this.getDataLengthIncludingAddNew();
            if (this._layout.calcCanvasWidth() > vs.width)
                vs.height += this._scrollDims.height;
        } else {

            var style = getComputedStyle(this._container);
            vs.height = parsePx(style.height)
                - parsePx(style.paddingTop)
                - parsePx(style.paddingBottom)
                - vs.headerHeight
                - vs.topPanelHeight
                - vs.headerRowHeight
                - vs.footerRowHeight
                - vs.groupingPanelHeight;
        }

        vs.numVisibleRows = Math.ceil(vs.height / this._options.rowHeight);
    }

    resizeCanvas = (): void => {
        if (!this._initialized) {
            return;
        }

        this.calcViewportSize();
        this._layout.resizeCanvas();

        if (!this._scrollDims || !this._scrollDims.width) {
            this._scrollDims = getScrollBarDimensions(true);
        }

        if (this._options.forceFitColumns) {
            this.autosizeColumns();
        }

        this.updateRowCount();
        this.handleScroll();
        // Since the width has changed, force the render() to reevaluate virtually rendered cells.
        this._scrollLeftRendered = -1;
        this.render();
    }

    updatePagingStatusFromView(pagingInfo: { pageSize: number, pageNum: number, totalPages: number }) {
        this._pagingActive = (pagingInfo.pageSize !== 0);
        this._pagingIsLastPage = (pagingInfo.pageNum == pagingInfo.totalPages - 1);
    }

    updateRowCount(): void {
        if (!this._initialized) {
            return;
        }

        var dataLengthIncludingAddNew = this.getDataLengthIncludingAddNew();
        var scrollCanvas = this._layout.getScrollCanvasY();
        var oldH = Math.round(parsePx(getComputedStyle(scrollCanvas).height));

        var numberOfRows;
        const frozenTopLast = this._layout.getFrozenTopLastRow();
        const frozenBottomFirst = this._layout.getFrozenBottomFirstRow();
        const dataLength = this.getDataLength();
        if (frozenTopLast >= 0 || frozenBottomFirst != Infinity) {
            numberOfRows = dataLength - (frozenTopLast + 1) - (dataLength - frozenBottomFirst - 1);
        } else {
            numberOfRows = dataLengthIncludingAddNew + (this._options.leaveSpaceForNewRows ? this._viewportInfo.numVisibleRows - 1 : 0);
        }

        var tempViewportH = Math.round(parsePx(getComputedStyle(this._layout.getScrollContainerY()).height));
        const vpi = this._viewportInfo;
        var oldViewportHasVScroll = vpi.hasVScroll;
        // with autoHeight, we do not need to accommodate the vertical scroll bar
        vpi.hasVScroll = !this._options.autoHeight && (numberOfRows * this._options.rowHeight > tempViewportH);

        this.makeActiveCellNormal();

        // remove the rows that are now outside of the data range
        // this helps avoid redundant calls to .removeRow() when the size of the data decreased by thousands of rows
        var l = dataLengthIncludingAddNew - 1;
        for (var x in this._rowsCache) {
            var i = parseInt(x, 10);
            if (i >= l) {
                this.removeRowFromCache(i);
            }
        }

        this.startPostProcessingCleanup();

        vpi.virtualHeight = Math.max(this._options.rowHeight * numberOfRows, tempViewportH - this._scrollDims.height);

        if (this._activeCellNode && this._activeRow > l) {
            this.resetActiveCell();
        }

        if (vpi.virtualHeight < getMaxSupportedCssHeight()) {
            // just one page
            vpi.realScrollHeight = this._pageHeight = vpi.virtualHeight;
            this._numberOfPages = 1;
            this._jumpinessCoefficient = 0;
        } else {
            // break into pages
            vpi.realScrollHeight = getMaxSupportedCssHeight();
            this._pageHeight = vpi.realScrollHeight / 100;
            this._numberOfPages = Math.floor(vpi.virtualHeight / this._pageHeight);
            this._jumpinessCoefficient = (vpi.virtualHeight - vpi.realScrollHeight) / (this._numberOfPages - 1);
        }

        if (vpi.realScrollHeight !== oldH) {
            this._layout.realScrollHeightChange();
            this._scrollTop = this._layout.getScrollContainerY().scrollTop;
        }

        var oldScrollTopInRange = (this._scrollTop + this._pageOffset <= vpi.virtualHeight - tempViewportH);

        if (vpi.virtualHeight == 0 || this._scrollTop == 0) {
            this._page = this._pageOffset = 0;
        } else if (oldScrollTopInRange) {
            // maintain virtual position
            this.scrollTo(this._scrollTop + this._pageOffset);
        } else {
            // scroll to bottom
            this.scrollTo(vpi.virtualHeight - tempViewportH);
        }

        if (vpi.realScrollHeight != oldH && this._options.autoHeight) {
            this.resizeCanvas();
        }

        if (this._options.forceFitColumns && oldViewportHasVScroll != vpi.hasVScroll) {
            this.autosizeColumns();
        }
        this.updateCanvasWidth(false);
    }

    /**
     * @param viewportTop optional viewport top
     * @param viewportLeft optional viewport left
     * @returns viewport range
     */
    getViewport(viewportTop?: number, viewportLeft?: number): ViewRange {
        return this.getVisibleRange(viewportTop, viewportLeft);
    }

    getVisibleRange(viewportTop?: number, viewportLeft?: number): ViewRange {
        if (viewportTop == null) {
            viewportTop = this._scrollTop;
        }
        if (viewportLeft == null) {
            viewportLeft = this._scrollLeft;
        }
        if (this._options.rtl) {
            viewportLeft = Math.abs(viewportLeft);
        }
        return {
            top: this.getRowFromPosition(viewportTop),
            bottom: this.getRowFromPosition(viewportTop + this._viewportInfo.height) + 1,
            leftPx: viewportLeft,
            rightPx: viewportLeft + this._viewportInfo.width
        };
    }

    getRenderedRange(viewportTop?: number, viewportLeft?: number): ViewRange {
        var range = this.getVisibleRange(viewportTop, viewportLeft);
        if (this._options.renderAllRows) {
            range.top = 0;
            range.bottom = this.getDataLengthIncludingAddNew() - 1;
        }
        else {
            var buffer = Math.round(this._viewportInfo.height / this._options.rowHeight);
            var minBuffer = this._options.minBuffer || 3;
            if (this._vScrollDir == -1) {
                range.top -= buffer;
                range.bottom += minBuffer;
            } else if (this._vScrollDir == 1) {
                range.top -= minBuffer;
                range.bottom += buffer;
            } else {
                range.top -= minBuffer;
                range.bottom += minBuffer;
            }

            range.top = Math.max(0, range.top);
            range.bottom = Math.min(this.getDataLengthIncludingAddNew() - 1, range.bottom);
        }

        if (this._options.renderAllCells) {
            range.leftPx = 0;
            range.rightPx = this._layout.getCanvasWidth();
        }
        else {
            range.leftPx -= this._viewportInfo.width;
            range.rightPx += this._viewportInfo.width;

            range.leftPx = Math.max(0, range.leftPx);
            range.rightPx = Math.min(this._layout.getCanvasWidth(), range.rightPx);
        }

        return range;
    }

    private ensureCellNodesInRowsCache(row: number): void {
        var cacheEntry = this._rowsCache[row];
        if (cacheEntry && cacheEntry.cellRenderQueue.length) {
            for (const rowNode of [cacheEntry.rowNodeE, cacheEntry.rowNodeC, cacheEntry.rowNodeE]) {
                var lastChild = rowNode?.lastElementChild;
                while (lastChild && cacheEntry.cellRenderQueue.length) {
                    var columnIdx = cacheEntry.cellRenderQueue.pop();
                    var element = cacheEntry.cellRenderContent.pop();

                    cacheEntry.cellNodesByColumnIdx[columnIdx] = lastChild as HTMLElement;
                    if (element instanceof Node)
                        lastChild.appendChild(element);
                    lastChild = lastChild.previousElementSibling;
                }
            }
        }
    }

    private cleanUpCells(rangeToKeep: ViewRange, row: number): void {
        // Ignore frozen rows
        if (this._layout.isFrozenRow(row))
            return;

        var cacheEntry = this._rowsCache[row];

        // Remove cells outside the range.
        var cellsToRemove = [], pinnedStartLast = this._layout.getPinnedStartLastCol(), pinnedEndFirst = this._layout.getPinnedEndFirstCol();
        for (var x in cacheEntry.cellNodesByColumnIdx) {

            var i = parseInt(x, 10);

            // Ignore frozen columns
            if (i < pinnedStartLast || i > pinnedEndFirst) {
                continue;
            }

            var colspan = cacheEntry.cellColSpans[i], cols = this._cols;
            if (this._colLeft[i] > rangeToKeep.rightPx || this._colRight[Math.min(cols.length - 1, i + colspan - 1)] < rangeToKeep.leftPx) {
                if (!(row == this._activeRow && i === this._activeCell)) {
                    cellsToRemove.push(i);
                }
            }
        }

        var cellToRemove, node;
        this._postProcessGroupId++;
        while ((cellToRemove = cellsToRemove.pop()) != null) {
            node = cacheEntry.cellNodesByColumnIdx[cellToRemove];

            if (this._postCleanupActive && this._postProcessedRows[row] && this._postProcessedRows[row][cellToRemove]) {
                this.queuePostProcessedCellForCleanup(node, cellToRemove, row);
            } else {
                this._removeNode(node);
            }

            delete cacheEntry.cellColSpans[cellToRemove];
            delete cacheEntry.cellNodesByColumnIdx[cellToRemove];
            if (this._postProcessedRows[row]) {
                delete this._postProcessedRows[row][cellToRemove];
            }
        }
    }

    private cleanUpAndRenderCells(range: ViewRange) {
        var cacheEntry;
        var stringArray: string[] = [];
        var processedRows = [];
        var cellsAdded;
        var colspan;
        var cols = this._cols;
        var cellContents: Element[] = [];

        for (var row = range.top, btm = range.bottom; row <= btm; row++) {
            cacheEntry = this._rowsCache[row];
            if (!cacheEntry) {
                continue;
            }

            // cellRenderQueue populated in renderRows() needs to be cleared first
            this.ensureCellNodesInRowsCache(row);

            this.cleanUpCells(range, row);

            // Render missing cells.
            cellsAdded = 0;

            var itemMetadata = (this._data as IDataView).getItemMetadata?.(row);
            var colsMetadata = itemMetadata && itemMetadata.columns;

            var d = this.getDataItem(row);

            // TODO:  shorten this loop (index? heuristics? binary search?)
            for (var i = 0, ii = cols.length; i < ii; i++) {
                // Cells to the right are outside the range.
                if (this._colLeft[i] > range.rightPx) {
                    break;
                }

                // Already rendered.
                if ((colspan = cacheEntry.cellColSpans[i]) != null) {
                    i += (colspan > 1 ? colspan - 1 : 0);
                    continue;
                }

                var columnData: ColumnMetadata = null;
                colspan = 1;
                if (colsMetadata) {
                    columnData = colsMetadata[cols[i].id] || colsMetadata[i];
                    colspan = (columnData && columnData.colspan) || 1;
                    if (colspan === "*") {
                        colspan = ii - i;
                    }
                }

                if (this._colRight[Math.min(ii - 1, i + colspan - 1)] > range.leftPx) {
                    this.appendCellHtml(stringArray, row, i, colspan, d, columnData);
                    cellsAdded++;
                }

                i += (colspan > 1 ? colspan - 1 : 0);
            }

            if (cellsAdded) {
                processedRows.push(row);
            }
        }

        if (!stringArray.length) {
            return;
        }

        var x = document.createElement("div");
        x.innerHTML = stringArray.join("");

        var processedRow;
        var node: HTMLElement, pinnedStartLast = this._layout.getPinnedStartLastCol(), pinnedEndFirst = this._layout.getPinnedEndFirstCol();
        while ((processedRow = processedRows.pop()) != null) {
            cacheEntry = this._rowsCache[processedRow];
            var columnIdx;
            while ((columnIdx = cacheEntry.cellRenderQueue.pop()) != null) {
                var element = cacheEntry.cellRenderContent.pop();
                node = x.lastElementChild as HTMLElement;
                if (element instanceof Node)
                    node.appendChild(element);

                if (pinnedStartLast >= 0 && columnIdx <= pinnedStartLast) {
                    cacheEntry.rowNodeS.appendChild(node);
                } else if (pinnedEndFirst != Infinity && columnIdx >= pinnedEndFirst) {
                    cacheEntry.rowNodeE.appendChild(node);
                } else {
                    cacheEntry.rowNodeC.appendChild(node);
                }

                cacheEntry.cellNodesByColumnIdx[columnIdx] = node;
            }
        }
    }

    private renderRows(range: ViewRange): void {
        var sbStart: string[] = [],
            sbCenter: string[] = [],
            sbEnd: string[] = [],
            rows = [],
            needToReselectCell = false,
            dataLength = this.getDataLength();

        for (var i = range.top, ii = range.bottom; i <= ii; i++) {

            if (this._rowsCache[i] || (this._layout.getFrozenBottomFirstRow() != Infinity && i == dataLength)) {
                continue;
            }

            rows.push(i);

            // Create an entry right away so that appendRowHtml() can
            // start populatating it.
            this._rowsCache[i] = {
                rowNodeS: null,
                rowNodeC: null,
                rowNodeE: null,

                // ColSpans of rendered cells (by column idx).
                // Can also be used for checking whether a cell has been rendered.
                cellColSpans: [],

                // Cell nodes (by column idx).  Lazy-populated by ensureCellNodesInRowsCache().
                cellNodesByColumnIdx: [],

                // Column indices of cell nodes that have been rendered, but not yet indexed in
                // cellNodesByColumnIdx.  These are in the same order as cell nodes added at the
                // end of the row.
                cellRenderQueue: [],

                cellRenderContent: []
            };

            this.appendRowHtml(sbStart, sbCenter, sbEnd, i, range, dataLength);
            if (this._activeCellNode && this._activeRow === i) {
                needToReselectCell = true;
            }
        }

        if (!rows.length) {
            return;
        }

        var s = document.createElement("div"),
            c = document.createElement("div"),
            e = document.createElement("div");

        s.innerHTML = sbStart.join("");
        c.innerHTML = sbCenter.join("");
        e.innerHTML = sbEnd.join("");

        const layout = this._layout;
        for (var i = 0, ii = rows.length; i < ii; i++) {
            var row = rows[i];
            var cache = this._rowsCache[row];
            cache.rowNodeS = s.firstElementChild as HTMLElement;
            cache.rowNodeC = c.firstElementChild as HTMLElement;
            cache.rowNodeE = e.firstElementChild as HTMLElement;
            layout.appendCachedRow(row, cache.rowNodeS, cache.rowNodeC, cache.rowNodeE);
            if (cache.cellRenderContent.some(x => x instanceof Node))
                this.ensureCellNodesInRowsCache(row);
        }

        if (needToReselectCell) {
            this._activeCellNode = this.getCellNode(this._activeRow, this._activeCell);
        }
    }

    private startPostProcessing(): void {
        if (!this._postRenderActive) {
            return;
        }

        clearTimeout(this._hPostRender);

        if (this._options.asyncPostRenderDelay < 0) {
            this.asyncPostProcessRows();
        } else {
            this._hPostRender = setTimeout(this.asyncPostProcessRows, this._options.asyncPostRenderDelay);
        }
    }

    private startPostProcessingCleanup(): void {
        if (!this._postCleanupActive) {
            return;
        }

        clearTimeout(this._hPostRenderCleanup);

        if (this._options.asyncPostCleanupDelay < 0) {
            this.asyncPostProcessCleanupRows();
        }
        else {
            this._hPostRenderCleanup = setTimeout(this.asyncPostProcessCleanupRows.bind(this), this._options.asyncPostCleanupDelay);
        }
    }

    private invalidatePostProcessingResults(row: number): void {
        if (this._options.enableAsyncPostRenderCleanup) {
            var postProcessed = this._postProcessedRows[row];
            if (postProcessed) {
                // change status of columns to be re-rendered
                for (var columnIdx in postProcessed) {
                    if (isPollutingKey(columnIdx)) continue;
                    postProcessed[columnIdx] = 'C';
                }
            }
        }
        else {
            delete this._postProcessedRows[row];
        }

        this._postProcessFromRow = Math.min(this._postProcessFromRow, row);
        this._postProcessToRow = Math.max(this._postProcessToRow, row);
        this.startPostProcessing();
    }

    private updateRowPositions(): void {
        for (var row in this._rowsCache) {
            var c = this._rowsCache[row];
            var p = this.getRowTop(parseInt(row, 10)) + "px";
            c.rowNodeS && (c.rowNodeS.style.top = p);
            c.rowNodeC && (c.rowNodeC.style.top = p);
            c.rowNodeE && (c.rowNodeE.style.top = p);
        }
    }

    private updateGrandTotals(): void {
        if (!this._options.showFooterRow || !this._initialized)
            return;

        var totals: IGroupTotals;
        if (this._data && (this._data as IDataView).getGrandTotals)
            totals = (this._data as IDataView).getGrandTotals();
        totals = totals ?? {};

        var cols = this._cols;
        for (var m of cols) {
            if (m.id != void 0) {
                const formatter = this.getTotalsFormatter(m);
                if (!formatter)
                    continue;
                const ctx = this.getFormatterContext(-1, -1);
                ctx.item = totals;
                ctx.column = m;
                ctx.purpose = "grand-totals";
                const fmtResult = formatter(ctx);
                const footerNode = this.getFooterRowColumn(m.id);
                this._emptyNode(footerNode);
                applyFormatterResultToCellNode(ctx, fmtResult, footerNode, contentOnly);
            }
        }
    }

    public render = (): void => {
        if (!this._initialized) { return; }
        if (this._hRender) {
            clearTimeout(this._hRender);
        }
        var visible = this.getVisibleRange();
        var rendered = this.getRenderedRange();

        // remove rows no longer in the viewport
        this.cleanupRows(rendered);

        // add new rows & missing cells in existing rows
        if (this._scrollLeftRendered != this._scrollLeft) {
            this._layout.beforeCleanupAndRenderCells(rendered);
            this.cleanUpAndRenderCells(rendered);
        }

        // render missing rows
        this.renderRows(rendered);
        this._layout.afterRenderRows(rendered);

        this._postProcessFromRow = visible.top;
        this._postProcessToRow = Math.min(this.getDataLengthIncludingAddNew() - 1, visible.bottom);
        this.startPostProcessing();

        this._scrollTopRendered = this._scrollTop;
        this._scrollLeftRendered = this._scrollLeft;
        this._lastRenderTime = new Date().getTime();
        this._hRender = null;
    }

    private handleHeaderFooterRowScroll = (e: IEventData): void => {
        if (this._ignoreScrollUntil >= new Date().getTime())
            return;

        var scrollLeft = (e.target as HTMLElement).scrollLeft;
        if (scrollLeft != this._layout.getScrollContainerX().scrollLeft) {
            this._layout.getScrollContainerX().scrollLeft = scrollLeft;
        }
    }

    private handleMouseWheel(e: MouseEvent & { axis: number; wheelDelta: number; wheelDeltaX: number; wheelDeltaY: number; HORIZONTAL_AXIS: number; }): void {
        e = (e as any).originalEvent || e;
        let delta = 0;
        let deltaX = 0;
        let deltaY = 0;

        if (e.wheelDelta) {
            delta = e.wheelDelta / 120;
        }
        if (e.detail) {
            delta = -e.detail / 3;
        }

        deltaY = delta;

        if (e.axis !== undefined && e.axis === e.HORIZONTAL_AXIS) {
            deltaY = 0;
            deltaX = -1 * delta;
        }

        if (e.wheelDeltaY !== undefined) {
            deltaY = e.wheelDeltaY / 120;
        }
        if (e.wheelDeltaX !== undefined) {
            deltaX = -1 * e.wheelDeltaX / 120;
        }

        this._scrollTop = Math.max(0, this._layout.getScrollContainerY().scrollTop - (deltaY * this._options.rowHeight));
        this._scrollLeft = this._layout.getScrollContainerX().scrollLeft + (deltaX * 10);
        if (this._handleScroll(true)) {
            e.preventDefault();
        }
    }

    private handleScroll() {
        this._scrollTop = this._layout.getScrollContainerY().scrollTop;
        this._scrollLeft = this._layout.getScrollContainerX().scrollLeft;
        this._handleScroll();
    }

    private _handleScroll(isMouseWheel?: boolean) {

        var vScrollDist = Math.abs(this._scrollTop - this._scrollTopPrev);
        var hScrollDist = Math.abs(this._scrollLeft - this._scrollLeftPrev);

        if (hScrollDist || vScrollDist)
            this._ignoreScrollUntil = new Date().getTime() + 100;

        if (hScrollDist) {
            this._scrollLeftPrev = this._scrollLeft;

            this._layout.getScrollContainerX().scrollLeft = this._scrollLeft;
            this._layout.handleScrollH();
        }

        const vpi = this._viewportInfo;

        if (vScrollDist) {
            this._vScrollDir = this._scrollTopPrev < this._scrollTop ? 1 : -1;
            this._scrollTopPrev = this._scrollTop;

            if (isMouseWheel === true) {
                this._layout.getScrollContainerY().scrollTop = this._scrollTop;
            }

            this._layout.handleScrollV();

            // switch virtual pages if needed
            if (vScrollDist < this._viewportInfo.height) {
                this.scrollTo(this._scrollTop + this._pageOffset);
            } else {
                var oldOffset = this._pageOffset;
                if (vpi.realScrollHeight == vpi.height) {
                    this._page = 0;
                } else {
                    this._page = Math.min(this._numberOfPages - 1, Math.floor(this._scrollTop * ((vpi.virtualHeight - this._viewportInfo.height) / (vpi.realScrollHeight - this._viewportInfo.height)) * (1 / this._pageHeight)));
                }
                this._pageOffset = Math.round(this._page * this._jumpinessCoefficient);
                if (oldOffset != this._pageOffset) {
                    this.invalidateAllRows();
                }
            }
        }

        if (hScrollDist || vScrollDist) {
            const wasScheduled = !!this._hRender;
            if (this._hRender) {
                clearTimeout(this._hRender);
            }

            if (Math.abs(this._scrollTopRendered - this._scrollTop) > 20 ||
                Math.abs(this._scrollLeftRendered - this._scrollLeft) > 20 ||
                wasScheduled) {
                if (this._options.forceSyncScrolling ||
                    (this._options.forceSyncScrollInterval &&
                        (this._lastRenderTime < new Date().getTime() - this._options.forceSyncScrollInterval))) {
                    this.render();
                } else {
                    this._hRender = setTimeout(this.render, 50);
                }

                this.trigger(this.onViewportChanged);
            }
        }

        this.trigger(this.onScroll, { scrollLeft: this._scrollLeft, scrollTop: this._scrollTop });

        return !!(hScrollDist || vScrollDist);
    }

    private asyncPostProcessRows = () => {
        var dataLength = this.getDataLength();
        var cols = this._cols;
        while (this._postProcessFromRow <= this._postProcessToRow) {
            var row = (this._vScrollDir >= 0) ? this._postProcessFromRow++ : this._postProcessToRow--;
            var cacheEntry = this._rowsCache[row];
            if (!cacheEntry || row >= dataLength) {
                continue;
            }

            if (!this._postProcessedRows[row]) {
                this._postProcessedRows[row] = {};
            }

            this.ensureCellNodesInRowsCache(row);
            for (var x in cacheEntry.cellNodesByColumnIdx) {
                var columnIdx = parseInt(x, 10);

                var m = cols[columnIdx];
                var processedStatus = this._postProcessedRows[row][columnIdx]; // C=cleanup and re-render, R=render
                if (processedStatus !== 'R') {
                    if (m.asyncPostRender || m.asyncPostRenderCleanup) {
                        var node = cacheEntry.cellNodesByColumnIdx[columnIdx];
                        if (node) {
                            m.asyncPostRender && m.asyncPostRender(node, row, this.getDataItem(row), m, processedStatus === 'C');
                        }
                    }
                    this._postProcessedRows[row][columnIdx] = 'R';
                }
            }

            if (this._options.asyncPostRenderDelay >= 0) {
                this._hPostRender = setTimeout(this.asyncPostProcessRows, this._options.asyncPostRenderDelay);
                return;
            }
        }
    }

    private asyncPostProcessCleanupRows(): void {
        var cols = this._cols;
        while (this._postProcessCleanupQueue?.length > 0) {
            var groupId = this._postProcessCleanupQueue[0].groupId;

            // loop through all queue members with this groupID
            while (this._postProcessCleanupQueue.length > 0 && this._postProcessCleanupQueue[0].groupId == groupId) {
                var entry = this._postProcessCleanupQueue.shift();
                entry.rowNodeS?.remove();
                entry.rowNodeC?.remove();
                entry.rowNodeE?.remove();
                if (entry.cellNode != null) {
                    var column = cols[entry.columnIdx];
                    if (column && column.asyncPostRenderCleanup) {
                        column.asyncPostRenderCleanup(entry.cellNode, entry.rowIdx, column);
                        this._removeNode(entry.cellNode);
                    }
                }
            }

            // call this function again after the specified delay
            if (this._options.asyncPostRenderDelay >= 0) {
                this._hPostRenderCleanup = setTimeout(this.asyncPostProcessCleanupRows.bind(this), this._options.asyncPostCleanupDelay);
                return;
            }
        }
    }

    private updateCellCssStylesOnRenderedRows(addedHash: CellStylesHash, removedHash: CellStylesHash) {
        var node, columnId, addedRowHash, removedRowHash;
        for (var row in this._rowsCache) {
            removedRowHash = removedHash && removedHash[row];
            addedRowHash = addedHash && addedHash[row];

            if (removedRowHash) {
                for (columnId in removedRowHash) {
                    if (!addedRowHash || removedRowHash[columnId] != addedRowHash[columnId]) {
                        node = this.getCellNode(parseInt(row, 10), this.getColumnIndex(columnId));
                        if (node) {
                            const r = removedRowHash[columnId];
                            removeClass(node, r);
                        }
                    }
                }
            }

            if (addedRowHash) {
                for (columnId in addedRowHash) {
                    if (!removedRowHash || removedRowHash[columnId] != addedRowHash[columnId]) {
                        node = this.getCellNode(parseInt(row, 10), this.getColumnIndex(columnId));
                        if (node) {
                            const a = addedRowHash[columnId];
                            addClass(node, a);
                        }
                    }
                }
            }
        }
    }

    addCellCssStyles(key: string, hash: CellStylesHash): void {
        if (this._cellCssClasses[key]) {
            throw "addCellCssStyles: cell CSS hash with key '" + key + "' already exists.";
        }

        this._cellCssClasses[key] = hash;
        this.updateCellCssStylesOnRenderedRows(hash, null);

        this.trigger(this.onCellCssStylesChanged, { key: key, hash: hash });
    }

    removeCellCssStyles(key: string): void {
        if (!this._cellCssClasses[key]) {
            return;
        }

        this.updateCellCssStylesOnRenderedRows(null, this._cellCssClasses[key]);
        delete this._cellCssClasses[key];

        this.trigger(this.onCellCssStylesChanged, { key: key, hash: null });
    }

    setCellCssStyles(key: string, hash: CellStylesHash): void {
        var prevHash = this._cellCssClasses[key];

        this._cellCssClasses[key] = hash;
        this.updateCellCssStylesOnRenderedRows(hash, prevHash);

        this.trigger(this.onCellCssStylesChanged, { key: key, hash: hash });
    }

    getCellCssStyles(key: string): CellStylesHash {
        return this._cellCssClasses[key];
    }

    flashCell(row: number, cell: number, speed?: number): void {
        speed = speed || 100;
        if (this._rowsCache[row]) {
            var cellEl = this._jQuery(this.getCellNode(row, cell));
            toggleCellClass(4);
        }

        var klass = this._options.cellFlashingCssClass;

        function toggleCellClass(times: number) {
            if (!times) {
                return;
            }
            setTimeout(function () {
                cellEl.queue(function () {
                    cellEl.toggleClass(klass).dequeue();
                    toggleCellClass(times - 1);
                });
            }, speed);
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////////////
    // Interactivity

    private handleDragInit(e: UIEvent, dd: any): boolean {
        var cell = this.getCellFromEvent(e);
        if (!cell || !this.cellExists(cell.row, cell.cell)) {
            return false;
        }

        var retval = this.trigger(this.onDragInit, dd, e);
        if ((e as IEventData).isImmediatePropagationStopped && (e as IEventData).isImmediatePropagationStopped()) {
            e.preventDefault();
            return retval;
        }

        // if nobody claims to be handling drag'n'drop by stopping immediate propagation,
        // cancel out of it
        return false;
    }

    private handleDragStart(e: UIEvent, dd: any): boolean {
        var cell = this.getCellFromEvent(e);
        if (!cell || !this.cellExists(cell.row, cell.cell)) {
            return false;
        }

        var retval = this.trigger(this.onDragStart, dd, e);
        if ((e as IEventData).isImmediatePropagationStopped && (e as IEventData).isImmediatePropagationStopped()) {
            return retval;
        }

        return false;
    }

    private handleDrag(e: UIEvent, dd: any): any {
        return this.trigger(this.onDrag, dd, e);
    }

    private handleDragEnd(e: UIEvent, dd: any): void {
        this.trigger(this.onDragEnd, dd, e);
    }

    private handleKeyDown(e: KeyboardEvent): void {
        this.trigger(this.onKeyDown, { row: this._activeRow, cell: this._activeCell }, e);
        var handled = (e as IEventData).isImmediatePropagationStopped && (e as IEventData).isImmediatePropagationStopped();

        if (!handled) {
            if (!e.shiftKey && !e.altKey) {
                if (this._options.editable && this._currentEditor && this._currentEditor.keyCaptureList) {
                    if (this._currentEditor.keyCaptureList.indexOf((e as any).which) >= 0) {
                        return;
                    }
                }

                if (e.key === "Home") {
                    if (e.ctrlKey) {
                        this.navigateTop();
                        handled = true;
                    }
                    else
                        handled = this.navigateRowStart();
                }
                else if (e.key === "End") {
                    if (e.ctrlKey) {
                        this.navigateBottom();
                        handled = true;
                    }
                    else
                        handled = this.navigateRowEnd();
                }
            }
        }

        if (!handled) {
            if (!e.shiftKey && !e.altKey && !e.ctrlKey) {

                if (this._options.editable && this._currentEditor && this._currentEditor.keyCaptureList) {
                    if (this._currentEditor.keyCaptureList.indexOf((e as any).which) >= 0) {
                        return;
                    }
                }

                if (e.key === "Esc" || e.key === "Escape") {
                    if (!this.getEditorLock().isActive()) {
                        return; // no editing mode to cancel, allow bubbling and default processing (exit without cancelling the event)
                    }
                    this.cancelEditAndSetFocus();
                } else if (e.key === "PageDown") {
                    this.navigatePageDown();
                    handled = true;
                } else if (e.key === "PageUp") {
                    this.navigatePageUp();
                    handled = true;
                } else if (e.key === "Left" || e.key === "ArrowLeft") {
                    handled = this.navigateLeft();
                } else if (e.key === "Right" || e.key === "ArrowRight") {
                    handled = this.navigateRight();
                } else if (e.key === "Up" || e.key === "ArrowUp") {
                    handled = this.navigateUp();
                } else if (e.key === "Down" || e.key === "ArrowDown") {
                    handled = this.navigateDown();
                } else if (e.key === "Tab") {
                    if (this._options.enableTabKeyNavigation)
                        handled = this.navigateNext();
                } else if (e.key === "Enter") {
                    if (this._options.editable) {
                        if (this._currentEditor) {
                            // adding new row
                            if (this._activeRow === this.getDataLength()) {
                                this.navigateDown();
                            } else {
                                this.commitEditAndSetFocus();
                            }
                        } else {
                            if (this.getEditorLock().commitCurrentEdit()) {
                                this.makeActiveCellEditable();
                            }
                        }
                    }
                    handled = true;
                }
            } else if (e.key === "Tab" && e.shiftKey && !e.ctrlKey && !e.altKey) {
                handled = this.navigatePrev();
            }
        }

        if (handled) {
            // the event has been handled so don't let parent element (bubbling/propagation) or browser (default) handle it
            e.stopPropagation();
            e.preventDefault();
            try {
                ((e as IEventData).originalEvent as any).keyCode = 0; // prevent default behaviour for special keys in IE browsers (F3, F5, etc.)
            }
            // ignore exceptions - setting the original event's keycode throws access denied exception for "Ctrl"
            // (hitting control key only, nothing else), "Shift" (maybe others)
            catch (error) {
            }
        }
    }

    private getTextSelection() {
        var selection = null;

        if (window.getSelection && window.getSelection().rangeCount > 0) {
            selection = window.getSelection().getRangeAt(0);
        }

        return selection;
    }

    private setTextSelection(selection: Range) {
        if (window.getSelection && selection) {
            var target = window.getSelection();
            target.removeAllRanges();
            target.addRange(selection);
        }
    }

    private handleClick(e: MouseEvent): void {
        if (!this._currentEditor) {
            // if this click resulted in some cell child node getting focus,
            // don't steal it back - keyboard events will still bubble up
            // IE9+ seems to default DIVs to tabIndex=0 instead of -1, so check for cell clicks directly.
            if (e.target != document.activeElement || (e.target as HTMLElement)?.classList?.contains?.("slick-cell")) {
                var selection = this.getTextSelection();
                this.setFocus();
                if (selection && this._options.enableTextSelectionOnCells) {
                    this.setTextSelection(selection);
                }
            }
        }

        var cell = this.getCellFromEvent(e as any);
        if (!cell || (this._currentEditor != null && this._activeRow == cell.row && this._activeCell == cell.cell)) {
            return;
        }

        this.trigger(this.onClick, { row: cell.row, cell: cell.cell }, e);
        if ((e as IEventData).isImmediatePropagationStopped && (e as IEventData).isImmediatePropagationStopped()) {
            return;
        }

        if (this.canCellBeActive(cell.row, cell.cell)) {
            if (!this.getEditorLock().isActive() || this.getEditorLock().commitCurrentEdit()) {

                var preClickModeOn = (e.target && (e.target as HTMLElement).classList.contains(preClickClassName));
                var column = this._cols[cell.cell];
                var suppressActiveCellChangedEvent = !!(this._options.editable && column && column.editor && this._options.suppressActiveCellChangeOnEdit);
                this.setActiveCellInternal(this.getCellNode(cell.row, cell.cell), null, preClickModeOn, suppressActiveCellChangedEvent, e);
            }
        }
    }

    private handleContextMenu(e: MouseEvent): void {
        var cellEl = (e.target as HTMLElement).closest(".slick-cell");
        if (!cellEl) {
            return;
        }

        // are we editing this cell?
        if (this._activeCellNode === cellEl && this._currentEditor != null) {
            return;
        }

        this.trigger(this.onContextMenu, {}, e);
    }

    private handleDblClick(e: MouseEvent): void {
        var cell = this.getCellFromEvent(e as any);
        if (!cell || (this._currentEditor != null && this._activeRow == cell.row && this._activeCell == cell.cell)) {
            return;
        }

        this.trigger(this.onDblClick, { row: cell.row, cell: cell.cell }, e);
        if ((e as IEventData).isImmediatePropagationStopped && (e as IEventData).isImmediatePropagationStopped()) {
            return;
        }

        if (this._options.editable) {
            this.gotoCell(cell.row, cell.cell, true);
        }
    }

    private handleHeaderMouseEnter(e: MouseEvent): void {
        const column = this.getColumnFromNode(e.target as HTMLElement)
        column && this.trigger(this.onHeaderMouseEnter, { column }, e);
    }

    private handleHeaderMouseLeave(e: MouseEvent): void {
        const column = this.getColumnFromNode(e.target as HTMLElement)
        column && this.trigger(this.onHeaderMouseLeave, { column }, e);
    }

    private handleHeaderContextMenu(e: any): void {
        var header = e.target.closest(".slick-header-column");
        var column = this.getColumnFromNode(header);
        column && this.trigger(this.onHeaderContextMenu, { column }, e);
    }

    private handleHeaderClick(e: any): void {
        var header = e.target.closest(".slick-header-column");
        var column = this.getColumnFromNode(header);
        column && this.trigger(this.onHeaderClick, { column: column }, e);
    }

    private handleMouseEnter(e: MouseEvent): void {
        this.trigger(this.onMouseEnter, {}, e);
    }

    private handleMouseLeave(e: MouseEvent): void {
        this.trigger(this.onMouseLeave, {}, e);
    }

    private cellExists(row: number, cell: number): boolean {
        return !(row < 0 || row >= this.getDataLength() || cell < 0 || cell >= this._cols.length);
    }

    getCellFromPoint(x: number, y: number): { row: number; cell: number; } {
        var row = this.getRowFromPosition(y);
        var cell = 0;
        var cols = this._cols;
        var w = 0;
        for (var i = 0; i < cols.length && w < x; i++) {
            w += cols[i].width;
            cell++;
        }

        if (cell < 0) {
            cell = 0;
        }

        return { row: row, cell: cell - 1 };
    }

    getCellFromNode(cellNode: Element): number {
        if (cellNode == null)
            return null;

        var c = (cellNode as HTMLElement).dataset.c;
        if (c != null)
            return parseInt(c, 10);

        // read column number from .l<columnNumber> CSS class
        var cls = /\sl(\d+)\s/.exec(' ' + cellNode.className + ' ');
        if (!cls) {
            return null;
        }
        return parseInt(cls[1], 10);
    }

    getColumnFromNode(cellNode: Element): Column<TItem> {
        if (cellNode == null)
            return null;

        var cell = this.getCellFromNode(cellNode);
        if (cell === null && this._jQuery)
            return this._jQuery(cell).data("column") as Column<TItem>;

        return this._cols[cell];
    }

    getRowFromNode(rowNode: Element): number {
        if (rowNode != null) {
            for (var row in this._rowsCache) {
                var c = this._rowsCache[row];
                if (c.rowNodeS === rowNode || c.rowNodeC === rowNode || c.rowNodeE === rowNode)
                    return parseInt(row, 10);
            }
        }
        return null;
    }

    getCellFromEvent(e: any): { row: number; cell: number; } {
        var row, cell;
        var cellEl = (e.target as HTMLElement).closest(".slick-cell") as HTMLElement;
        if (!cellEl) {
            return null;
        }

        row = this._layout.getRowFromCellNode(cellEl, e.clientX, e.clientY);
        cell = this.getCellFromNode(cellEl);

        if (row == null || cell == null) {
            return null;
        } else {
            return {
                "row": row,
                "cell": cell
            };
        }
    }

    getCellNodeBox(row: number, cell: number): { top: number; right: number; bottom: number; left: number; } {
        if (!this.cellExists(row, cell)) {
            return null;
        }

        var rowOffset = this._layout.getFrozenRowOffset(row);
        var cols = this._cols, pinnedStartLast = this._layout.getPinnedStartLastCol(), pinnedEndFirst = this._layout.getPinnedEndFirstCol();
        var y1 = this.getRowTop(row) - rowOffset;
        var y2 = y1 + this._options.rowHeight - 1;
        var x1 = 0;
        for (var i = 0; i < cell; i++) {
            x1 += cols[i].width;
            if (i === pinnedStartLast || i === pinnedEndFirst - 1) {
                x1 = 0;
            }
        }
        var x2 = x1 + cols[cell].width;

        return this._options.rtl ? {
            top: y1,
            right: x1,
            bottom: y2,
            left: x2
        } : {
            top: y1,
            left: x1,
            bottom: y2,
            right: x2
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////////////
    // Cell switching

    resetActiveCell(): void {
        this.setActiveCellInternal(null, false);
    }

    focus(): void {
        this.setFocus();
    }

    private setFocus(): void {
        if (this._tabbingDirection == -1) {
            this._focusSink1.focus();
        } else {
            this._focusSink2.focus();
        }
    }

    scrollCellIntoView(row: number, cell: number, doPaging?: boolean): void {
        this.scrollRowIntoView(row, doPaging);

        if (cell <= this._layout.getPinnedStartLastCol() ||
            cell >= this._layout.getPinnedEndFirstCol())
            return;

        var colspan = this.getColspan(row, cell);
        this.internalScrollColumnIntoView(this._colLeft[cell], this._colRight[cell + (colspan > 1 ? colspan - 1 : 0)]);
    }

    scrollColumnIntoView(cell: number): void {
        this.internalScrollColumnIntoView(this._colLeft[cell], this._colRight[cell]);
    }

    private internalScrollColumnIntoView(left: number, right: number): void {

        var scrollRight = this._scrollLeft + parsePx(getComputedStyle(this._layout.getScrollContainerX()).width) -
            (this._viewportInfo.hasVScroll ? this._scrollDims.width : 0);

        var target;
        if (left < this._scrollLeft)
            target = left;
        else if (right > scrollRight)
            target = Math.min(left, right - this._layout.getScrollContainerX().clientWidth);
        else
            return;

        this._layout.getScrollContainerX().scrollLeft = target;
        this.handleScroll();
        this.render();
    }

    private setActiveCellInternal(newCell: HTMLElement, opt_editMode?: boolean, preClickModeOn?: boolean, suppressActiveCellChangedEvent?: boolean, e?: any): void {
        if (this._activeCellNode) {
            this.makeActiveCellNormal();
            this._activeCellNode.classList.remove("active");
            var c = this._rowsCache[this._activeRow];
            if (c) {
                c.rowNodeS && c.rowNodeS.classList.remove("active");
                c.rowNodeC && c.rowNodeC.classList.remove("active");
                c.rowNodeE && c.rowNodeE.classList.remove("active");
            }
        }

        this._activeCellNode = newCell;

        if (this._activeCellNode) {
            var bcl = this._activeCellNode.getBoundingClientRect();

            var rowOffset = Math.floor(this._activeCellNode.closest('.grid-canvas')?.getBoundingClientRect().top ?? 0 + document.body.scrollTop);
            var isBottom = this._activeCellNode.closest('.grid-canvas-bottom') != null;
            const frozenBottomFirst = this._layout.getFrozenBottomFirstRow();
            if (frozenBottomFirst != Infinity && isBottom) {
                rowOffset -= (this._options.frozenBottom)
                    ? Math.round(parsePx(getComputedStyle(this._layout.getCanvasNodeFor(0, 0)).height))
                    : (this.getDataLength() - frozenBottomFirst) * this._options.rowHeight;
            }

            var cell = this.getCellFromPoint(bcl[this._options.rtl ? 'right' : 'left'] + document.body.scrollLeft, Math.ceil(bcl.top + document.body.scrollTop) - rowOffset);

            this._activeRow = cell.row;
            this._activeCell = this._activePosX = this.getCellFromNode(this._activeCellNode);

            if (this._options.showCellSelection) {
                this._activeCellNode.classList.add("active");
                var c = this._rowsCache[this._activeRow];
                if (c) {
                    c.rowNodeS?.classList.add("active");
                    c.rowNodeC?.classList.add("active");
                    c.rowNodeE?.classList.add("active");
                }
            }

            if (opt_editMode == null) {
                opt_editMode = (this._activeRow == this.getDataLength()) || this._options.autoEdit;
            }

            if (this._options.editable && opt_editMode && this.isCellPotentiallyEditable(this._activeRow, this._activeCell)) {
                clearTimeout(this._hEditorLoader);

                if (this._options.asyncEditorLoading) {
                    this._hEditorLoader = setTimeout(() => {
                        this.makeActiveCellEditable(undefined, preClickModeOn, e);
                    }, this._options.asyncEditorLoadDelay);
                } else {
                    this.makeActiveCellEditable(undefined, preClickModeOn, e);
                }
            }
        } else {
            this._activeRow = this._activeCell = null;
        }

        if (!suppressActiveCellChangedEvent) {
            this.trigger(this.onActiveCellChanged, this.getActiveCell() as ArgsCell);
        }
    }

    clearTextSelection(): void {
        if ((document as any).selection && (document as any).selection.empty) {
            try {
                //IE fails here if selected element is not in dom
                (document as any).selection.empty();
            } catch (e) { }
        } else if (window.getSelection) {
            var sel = window.getSelection();
            if (sel && sel.removeAllRanges) {
                sel.removeAllRanges();
            }
        }
    }

    private isCellPotentiallyEditable(row: number, cell: number): boolean {
        var dataLength = this.getDataLength();
        // is the data for this row loaded?
        if (row < dataLength && !this.getDataItem(row)) {
            return false;
        }

        // are we in the Add New row?  can we create new from this cell?
        if (this._cols[cell].cannotTriggerInsert && row >= dataLength) {
            return false;
        }

        // does this cell have an editor?
        if (!this.getEditor(row, cell)) {
            return false;
        }

        return true;
    }

    private makeActiveCellNormal(): void {
        if (!this._currentEditor) {
            return;
        }
        this.trigger(this.onBeforeCellEditorDestroy, { editor: this._currentEditor });
        this._currentEditor.destroy();
        this._currentEditor = null;

        if (this._activeCellNode) {
            this._activeCellNode.classList.remove("editable", "invalid");
            this.updateCellWithFormatter(this._activeCellNode, this._activeRow, this._activeCell);
            this.invalidatePostProcessingResults(this._activeRow);
        }

        // if there previously was text selected on a page (such as selected text in the edit cell just removed),
        // IE can't set focus to anything else correctly
        if (navigator.userAgent.toLowerCase().match(/msie/)) {
            this.clearTextSelection();
        }

        this.getEditorLock().deactivate(this._editController);
    }

    editActiveCell(editor?: EditorClass): void {
        this.makeActiveCellEditable(editor);
    }

    private makeActiveCellEditable(editor?: EditorClass, preClickModeOn?: boolean, e?: any): void {
        if (!this._activeCellNode) {
            return;
        }
        if (!this._options.editable) {
            throw "Grid : makeActiveCellEditable : should never get called when options.editable is false";
        }

        // cancel pending async call if there is one
        clearTimeout(this._hEditorLoader);

        if (!this.isCellPotentiallyEditable(this._activeRow, this._activeCell)) {
            return;
        }

        var columnDef = this._cols[this._activeCell];
        var item = this.getDataItem(this._activeRow);

        if (this.trigger(this.onBeforeEditCell, { row: this._activeRow, cell: this._activeCell, item: item, column: columnDef }) === false) {
            this.setFocus();
            return;
        }

        this.getEditorLock().activate(this._editController);
        this._activeCellNode.classList.add("editable");

        var useEditor = editor || this.getEditor(this._activeRow, this._activeCell);

        // don't clear the cell if a custom editor is passed through
        if (!editor && !useEditor.suppressClearOnEdit) {
            this._activeCellNode.innerHTML = "";
        }

        var itemMetadata = (this._data as IDataView).getItemMetadata?.(this._activeRow) as ItemMetadata;
        var colsMetadata = itemMetadata && itemMetadata.columns;
        var columnMetadata = colsMetadata && (colsMetadata[columnDef.id] || colsMetadata[this._activeCell]);

        this._currentEditor = new useEditor({
            grid: this,
            gridPosition: absBox(this._container),
            position: absBox(this._activeCellNode),
            container: this._activeCellNode,
            column: columnDef,
            columnMetaData: columnMetadata,
            item: item || {},
            event: e,
            editorCellNavOnLRKeys: this._options.editorCellNavOnLRKeys,
            commitChanges: this.commitEditAndSetFocus.bind(this),
            cancelChanges: this.cancelEditAndSetFocus.bind(this)
        });

        if (item) {
            this._currentEditor.loadValue(item);
            if (preClickModeOn && this._currentEditor.preClick) {
                this._currentEditor.preClick();
            }
        }

        this._serializedEditorValue = this._currentEditor.serializeValue();

        if (this._currentEditor.position) {
            this.handleActiveCellPositionChange();
        }
    }

    private commitEditAndSetFocus(): void {
        // if the commit fails, it would do so due to a validation error
        // if so, do not steal the focus from the editor
        if (this.getEditorLock().commitCurrentEdit()) {
            this.setFocus();
            if (this._options.autoEdit) {
                this.navigateDown();
            }
        }
    }

    private cancelEditAndSetFocus(): void {
        if (this.getEditorLock().cancelCurrentEdit()) {
            this.setFocus();
        }
    }

    private getActiveCellPosition(): Position {
        return absBox(this._activeCellNode);
    }

    getGridPosition(): Position {
        return absBox(this._container);
    }

    private handleActiveCellPositionChange = (): void => {
        if (!this._activeCellNode) {
            return;
        }

        this.trigger(this.onActiveCellPositionChanged, {});

        if (this._currentEditor) {
            var cellBox = this.getActiveCellPosition();
            if (this._currentEditor.show && this._currentEditor.hide) {
                if (!cellBox.visible) {
                    this._currentEditor.hide();
                } else {
                    this._currentEditor.show();
                }
            }

            if (this._currentEditor.position) {
                this._currentEditor.position(cellBox);
            }
        }
    }

    getCellEditor(): Editor {
        return this._currentEditor;
    }

    getActiveCell(): RowCell {
        if (!this._activeCellNode) {
            return null;
        } else {
            return { row: this._activeRow, cell: this._activeCell };
        }
    }

    getActiveCellNode(): HTMLElement {
        return this._activeCellNode;
    }

    scrollActiveCellIntoView(): void {
        if (this._activeRow != null && this._activeCell != null) {
            this.scrollCellIntoView(this._activeRow, this._activeCell);
        }
    }

    scrollRowIntoView(row: number, doPaging?: boolean): void {

        if (!this._layout.isFrozenRow(row)) {

            var viewportScrollH = Math.round(parsePx(getComputedStyle(this._layout.getScrollContainerY()).height));

            var rowNumber = this._layout.getFrozenTopLastRow() >= 0 ? (row - this._layout.getFrozenTopLastRow() + 1) : row;

            // if frozen row on top subtract number of frozen row
            var rowAtTop = rowNumber * this._options.rowHeight;
            var rowAtBottom = (rowNumber + 1) * this._options.rowHeight
                - viewportScrollH
                + (this._viewportInfo.hasHScroll ? this._scrollDims.height : 0);

            // need to page down?
            if ((rowNumber + 1) * this._options.rowHeight > this._scrollTop + viewportScrollH + this._pageOffset) {
                this.scrollTo(doPaging ? rowAtTop : rowAtBottom);
                this.render();
            }
            // or page up?
            else if (rowNumber * this._options.rowHeight < this._scrollTop + this._pageOffset) {
                this.scrollTo(doPaging ? rowAtBottom : rowAtTop);
                this.render();
            }
        }
    }

    scrollRowToTop(row: number): void {
        this.scrollTo(row * this._options.rowHeight);
        this.render();
    }

    private scrollPage(dir: number): void {
        var deltaRows = dir * this._viewportInfo.numVisibleRows;
        this.scrollTo((this.getRowFromPosition(this._scrollTop) + deltaRows) * this._options.rowHeight);
        this.render();

        if (this._options.enableCellNavigation && this._activeRow != null) {
            var row = this._activeRow + deltaRows;
            var dataLengthIncludingAddNew = this.getDataLengthIncludingAddNew();
            if (row >= dataLengthIncludingAddNew) {
                row = dataLengthIncludingAddNew - 1;
            }
            if (row < 0) {
                row = 0;
            }

            var cell = 0, prevCell = null;
            var prevActivePosX = this._activePosX;
            while (cell <= this._activePosX) {
                if (this.canCellBeActive(row, cell)) {
                    prevCell = cell;
                }
                cell += this.getColspan(row, cell);
            }

            if (prevCell != null) {
                this.setActiveCellInternal(this.getCellNode(row, prevCell));
                this._activePosX = prevActivePosX;
            } else {
                this.resetActiveCell();
            }
        }
    }

    navigatePageDown(): void {
        this.scrollPage(1);
    }

    navigatePageUp(): void {
        this.scrollPage(-1);
    }

    navigateTop(): void {
        this.navigateToRow(0);
    }

    navigateBottom(): void {
        this.navigateToRow(this.getDataLength() - 1);
    }

    navigateToRow(row: number) {
        var dataLength = this.getDataLength();
        if (!dataLength) {
            return true;
        }

        if (row < 0)
            row = 0;
        else if (row >= dataLength)
            row = dataLength - 1;

        this.scrollCellIntoView(row, 0, true);

        if (this._options.enableCellNavigation && this._activeRow != null) {
            var cell = 0, prevCell = null;
            var prevActivePosX = this._activePosX;
            while (cell <= this._activePosX) {
                if (this.canCellBeActive(row, cell))
                    prevCell = cell;
                cell += this.getColspan(row, cell);
            }

            if (prevCell != null) {
                this.setActiveCellInternal(this.getCellNode(row, prevCell));
                this._activePosX = prevActivePosX;
            }
            else
                this.resetActiveCell();
        }

        return true;
    }

    getColspan(row: number, cell: number): number {
        var itemMetadata = (this._data as IDataView).getItemMetadata?.(row) as ItemMetadata;
        if (!itemMetadata || !itemMetadata.columns) {
            return 1;
        }

        var cols = this._cols;
        var columnData = cols[cell] && (itemMetadata.columns[cols[cell].id] || itemMetadata.columns[cell]);
        var colspan = (columnData && columnData.colspan);
        if (colspan === "*") {
            colspan = cols.length - cell;
        } else {
            colspan = colspan || 1;
        }

        return colspan;
    }

    navigateRight(): boolean {
        return this.navigate("right");
    }

    navigateLeft(): boolean {
        return this.navigate("left");
    }

    navigateDown(): boolean {
        return this.navigate("down");
    }

    navigateUp(): boolean {
        return this.navigate("up");
    }

    navigateNext(): boolean {
        return this.navigate("next");
    }

    navigatePrev(): boolean {
        return this.navigate("prev");
    }

    navigateRowStart(): boolean {
        return this.navigate("home");
    }

    navigateRowEnd(): boolean {
        return this.navigate("end");
    }

    /**
     * @param {string} dir Navigation direction.
     * @return {boolean} Whether navigation resulted in a change of active cell.
     */
    navigate(dir: string): boolean {
        if (!this._options.enableCellNavigation) {
            return false;
        }

        if (!this._activeCellNode && dir != "prev" && dir != "next") {
            return false;
        }

        if (!this.getEditorLock().commitCurrentEdit()) {
            return true;
        }

        this.setFocus();

        if (!this._cellNavigator) {
            this._cellNavigator = new CellNavigator({
                getColumnCount: () => this._cols.length,
                getRowCount: () => this.getDataLengthIncludingAddNew(),
                getColspan: this.getColspan.bind(this),
                canCellBeActive: this.canCellBeActive.bind(this),
                setTabbingDirection: dir => this._tabbingDirection = dir,
                isRTL: () => this._options.rtl
            });
        }

        var pos = this._cellNavigator.navigate(dir, this._activeRow, this._activeCell, this._activePosX);
        if (pos) {
            if (this._layout.getFrozenBottomFirstRow() != Infinity && pos.row == this.getDataLength()) {
                return;
            }

            var isAddNewRow = (pos.row == this.getDataLength());

            if (!this._layout.isFrozenRow(pos.row)) {
                this.scrollCellIntoView(pos.row, pos.cell, !isAddNewRow);
            }

            this.setActiveCellInternal(this.getCellNode(pos.row, pos.cell))
            this._activePosX = pos.posX;
            return true;
        } else {
            this.setActiveCellInternal(this.getCellNode(this._activeRow, this._activeCell));
            return false;
        }
    }

    getCellNode(row: number, cell: number): HTMLElement {
        if (this._rowsCache[row]) {
            this.ensureCellNodesInRowsCache(row);
            return this._rowsCache[row].cellNodesByColumnIdx[cell];
        }
        return null;
    }

    setActiveCell(row: number, cell: number) {
        if (!this._initialized) { return; }
        var cols = this._cols;
        if (row > this.getDataLength() || row < 0 || cell >= cols.length || cell < 0) {
            return;
        }

        if (!this._options.enableCellNavigation) {
            return;
        }

        this.scrollCellIntoView(row, cell, false);
        this.setActiveCellInternal(this.getCellNode(row, cell), false);
    }

    setActiveRow(row: number, cell: number, suppressScrollIntoView?: boolean) {
        if (!this._initialized)
            return;

        if (row > this.getDataLength() || row < 0 || cell >= this._cols.length || cell < 0)
            return;

        this._activeRow = row;
        if (!suppressScrollIntoView)
            this.scrollCellIntoView(row, cell || 0, false);
    }

    canCellBeActive(row: number, cell: number): boolean {
        var cols = this._cols;
        if (!this._options.enableCellNavigation || row >= this.getDataLengthIncludingAddNew() ||
            row < 0 || cell >= cols.length || cell < 0) {
            return false;
        }

        var rowMetadata = (this._data as IDataView).getItemMetadata?.(row);
        if (rowMetadata && typeof rowMetadata.focusable === "boolean") {
            return rowMetadata.focusable;
        }

        var colsMetadata = rowMetadata && rowMetadata.columns;
        if (colsMetadata && cols[cell] && colsMetadata[cols[cell].id] && typeof colsMetadata[cols[cell].id].focusable === "boolean") {
            return colsMetadata[cols[cell].id].focusable;
        }
        if (colsMetadata && colsMetadata[cell] && typeof colsMetadata[cell].focusable === "boolean") {
            return colsMetadata[cell].focusable;
        }

        return cols[cell].focusable;
    }

    canCellBeSelected(row: number, cell: number) {
        var cols = this._cols;
        if (row >= this.getDataLength() || row < 0 || cell >= cols.length || cell < 0) {
            return false;
        }

        var itemMetadata = (this._data as IDataView).getItemMetadata?.(row);
        if (itemMetadata && typeof itemMetadata.selectable === "boolean") {
            return itemMetadata.selectable;
        }

        var columnMetadata = itemMetadata && itemMetadata.columns && (itemMetadata.columns[cols[cell].id] || itemMetadata.columns[cell]);
        if (columnMetadata && typeof columnMetadata.selectable === "boolean") {
            return columnMetadata.selectable;
        }

        return cols[cell].selectable;
    }

    gotoCell(row: number, cell: number, forceEdit?: boolean) {
        if (!this._initialized) { return; }
        if (!this.canCellBeActive(row, cell)) {
            return;
        }

        if (!this.getEditorLock().commitCurrentEdit()) {
            return;
        }

        this.scrollCellIntoView(row, cell, false);

        var newCell = this.getCellNode(row, cell);

        // if selecting the 'add new' row, start editing right away
        this.setActiveCellInternal(newCell, forceEdit || (row === this.getDataLength()) || this._options.autoEdit);

        // if no editor was created, set the focus back on the grid
        if (!this._currentEditor) {
            this.setFocus();
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////////////
    // IEditor implementation for the editor lock

    commitCurrentEdit(): boolean {
        var item = this.getDataItem(this._activeRow);
        var column = this._cols[this._activeCell];
        var self = this;

        if (this._currentEditor) {
            if (this._currentEditor.isValueChanged()) {
                var validationResults = this._currentEditor.validate();

                if (validationResults.valid) {
                    if (this._activeRow < this.getDataLength()) {
                        var editCommand: EditCommand = {
                            row: this._activeRow,
                            cell: self._activeCell,
                            editor: this._currentEditor,
                            serializedValue: this._currentEditor.serializeValue(),
                            prevSerializedValue: this._serializedEditorValue,
                            execute: function () {
                                this.editor.applyValue(item, this.serializedValue);
                                self.updateRow(this.row);
                                self.trigger(self.onCellChange, {
                                    row: this.activeRow,
                                    cell: self._activeCell,
                                    item: item
                                });
                            },
                            undo: function () {
                                this.editor.applyValue(item, this.prevSerializedValue);
                                self.updateRow(this.row);
                                self.trigger(self.onCellChange, {
                                    row: this.activeRow,
                                    cell: self._activeCell,
                                    item: item
                                });
                            }
                        };

                        if (this._options.editCommandHandler) {
                            this.makeActiveCellNormal();
                            this._options.editCommandHandler(item, column, editCommand);
                        } else {
                            editCommand.execute();
                            this.makeActiveCellNormal();
                        }

                    } else {
                        var newItem = {} as TItem;
                        this._currentEditor.applyValue(newItem, this._currentEditor.serializeValue());
                        this.makeActiveCellNormal();
                        this.trigger(this.onAddNewRow, { item: newItem, column: column });
                    }

                    // check whether the lock has been re-acquired by event handlers
                    return !this.getEditorLock().isActive();
                } else {
                    // Re-add the CSS class to trigger transitions, if any.
                    this._activeCellNode.classList.remove("invalid");
                    this._activeCellNode.offsetWidth;  // force layout
                    this._activeCellNode.classList.add("invalid");

                    this.trigger(this.onValidationError, {
                        editor: this._currentEditor,
                        cellNode: this._activeCellNode,
                        validationResults: validationResults,
                        row: this._activeRow,
                        cell: this._activeCell,
                        column: column
                    });

                    this._currentEditor.focus();
                    return false;
                }
            }

            this.makeActiveCellNormal();
        }
        return true;
    }

    private cancelCurrentEdit() {
        this.makeActiveCellNormal();
        return true;
    }

    private rowsToRanges(rows: number[]): CellRange[] {
        var ranges = [];
        var lastCell = this._cols.length - 1;
        for (var i = 0; i < rows.length; i++) {
            ranges.push(new CellRange(rows[i], 0, rows[i], lastCell));
        }
        return ranges;
    }

    getSelectedRows(): number[] {
        if (!this._selectionModel) {
            throw "Selection model is not set";
        }
        return this._selectedRows;
    }

    setSelectedRows(rows: number[]) {
        if (!this._selectionModel) {
            throw "Selection model is not set";
        }
        this._selectionModel.setSelectedRanges(this.rowsToRanges(rows));
    }
}

function isPollutingKey(key: string | null | undefined): boolean {
    return key === '__proto__' || key === 'constructor' || key === 'prototype';
}

const contentOnly = {
    contentOnly: true
}
