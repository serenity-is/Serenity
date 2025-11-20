import { bindThis, currentLifecycleRoot, observeSignal } from "@serenity-is/domwise";
import { preClickClassName } from "../core/base";
import { CellRange } from "../core/cellrange";
import { columnDefaults, initColumnProps, type Column, type ColumnMetadata, type ColumnSort, type ItemMetadata } from "../core/column";
import { Draggable, type DragPosition } from "../core/draggable";
import type { EditCommand, EditController, Editor, EditorClass, EditorFactory, EditorLock, Position, RowCell } from "../core/editing";
import { EventEmitter, type EventData } from "../core/event";
import type { ArgsAddNewRow, ArgsCell, ArgsCellChange, ArgsCellEdit, ArgsColumn, ArgsColumnNode, ArgsCssStyle, ArgsDrag, ArgsEditorDestroy, ArgsGrid, ArgsScroll, ArgsSelectedRowsChange, ArgsSort, ArgsValidationError } from "../core/eventargs";
import { applyFormatterResultToCellNode, convertCompatFormatter, defaultColumnFormat, formatterContext, type CellStylesHash, type ColumnFormat, type FormatterContext, type FormatterResult } from "../core/formatting";
import type { GridPlugin } from "../core/grid-plugin";
import type { GridSignals } from "../core/grid-signals";
import { gridDefaults, type GridOptions } from "../core/gridoptions";
import type { IGroupTotals } from "../core/group";
import { type IDataView } from "../core/idataview";
import type { GridLayoutInfo, ISleekGrid } from "../core/isleekgrid";
import type { SelectionModel } from "../core/selection-model";
import { addClass, escapeHtml, parsePx, removeClass } from "../core/util";
import type { ViewportInfo } from "../core/viewportinfo";
import type { ViewRange } from "../core/viewrange";
import { BasicLayout } from "../layouts/basic-layout";
import type { LayoutEngine } from "../layouts/layout-engine";
import { createGridSignalsAndRefs, disposeBandRefs, forEachBand, getAllCanvasNodes, getAllHScrollContainers, getAllViewportNodes, getAllVScrollContainers, mapBands, type GridBandRefs, type GridLayoutRefs } from "../layouts/layout-refs";
import { CellNavigator } from "./cellnavigator";
import { autosizeColumns, setupColumnResize } from "./column-resizing";
import { columnSortHandler, sortToDesiredOrderAndKeepRest } from "./column-sorting";
import { addListener, removeListener, triggerGridEvent } from "./event-utils";
import { defaultEmptyNode, defaultJQueryEmptyNode, defaultJQueryRemoveNode, defaultRemoveNode, PostProcessCleanupEntry, simpleArrayEquals, type CachedRow } from "./internal";
import type { RowCellRenderArgs } from "./render-args";
import { renderCell } from "./render-cell";
import { renderRow } from "./render-row";
import { absBox, applyColumnWidths, applyLegacyHeightOptions, createCssRules, findStylesheetByUID, getInnerWidth, getMaxSupportedCssHeight, getScrollBarDimensions, setStyleProp } from "./style-utils";

export class SleekGrid<TItem = any> implements ISleekGrid<TItem> {
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
    declare private _cssColRulesL: Record<number, CSSStyleRule>;
    declare private _cssColRulesR: Record<number, CSSStyleRule>;
    declare private _cssVarRules: CSSStyleRule;
    declare private _columnSortHandler: (e: MouseEvent) => void;
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
    declare private _allCols: Column<TItem>[];
    declare private _allColsById: { [key: string]: number };
    declare private _initialized: boolean;
    declare private _jQuery: any;
    declare private _jumpinessCoefficient: number;
    declare private _lastRenderTime: number;
    declare private _layout: LayoutEngine;
    private _ignorePinChangeUntil: number = 0;
    declare private _numberOfPages: number;
    declare private _on: OmitThisParameter<typeof addListener>;
    declare private _off: OmitThisParameter<typeof removeListener>;
    declare private _options: GridOptions<TItem>;
    declare private _signals: GridSignals;
    private _signalsDisposers: (() => void)[] = [];
    private _page: number = 0;
    declare private _pageHeight: number;
    private _pageOffset: number = 0;
    private _pagingActive: boolean = false;
    private _pagingIsLastPage: boolean = false;
    private _plugins: GridPlugin[] = [];
    declare private _postCleanupActive: boolean;
    private _postProcessCleanupQueue: PostProcessCleanupEntry[] = [];
    private _postProcessedRows: { [row: number]: { [cell: number]: string } } = Object.create(null);
    declare private _postProcessFromRow: number;
    private _postProcessGroupId: number = 0;
    declare private _postProcessToRow: number;
    declare private _postRenderActive: boolean;
    private _refs: GridLayoutRefs;
    private _mapBands: <T>(fn: (band: GridBandRefs) => T) => T[];
    private _forEachBand: (fn: (band: GridBandRefs) => void) => void;
    declare private _removeNode: (node: Element) => void;
    private _rowsCache: { [key: number]: CachedRow } = Object.create(null);
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
    declare private _stylesheet: CSSStyleSheet;
    private _tabbingDirection: number = 1;
    declare private _trigger: typeof triggerGridEvent;
    declare private static _nextUid: number;
    private _uid: string = "_sleekgrid_" + (SleekGrid._nextUid = (SleekGrid._nextUid || 0) + 1) + "_";
    private _viewportInfo: ViewportInfo = {} as any;
    private _vScrollDir: number = 1;

    private _boundAncestorScroll: HTMLElement[] = [];
    declare private _colResizeDisposer: AbortController;
    declare private _container: HTMLElement;
    declare private _focusSink1: HTMLElement;
    declare private _focusSink2: HTMLElement;
    declare private _groupingPanel: HTMLElement;
    declare private _eventDisposer: AbortController;

    readonly onActiveCellChanged = new EventEmitter<ArgsCell>();
    readonly onActiveCellPositionChanged = new EventEmitter<ArgsGrid>();
    readonly onAddNewRow = new EventEmitter<ArgsAddNewRow>();
    static readonly onAfterInit = new EventEmitter<ArgsGrid>();
    readonly onAfterInit = new EventEmitter<ArgsGrid>();
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
    readonly onDrag = new EventEmitter<ArgsDrag, UIEvent>();
    readonly onDragEnd = new EventEmitter<ArgsDrag, UIEvent>();
    readonly onDragInit = new EventEmitter<ArgsDrag, UIEvent>();
    readonly onDragStart = new EventEmitter<ArgsDrag, UIEvent>();
    readonly onFooterRowCellRendered = new EventEmitter<ArgsColumnNode>();
    readonly onHeaderCellRendered = new EventEmitter<ArgsColumnNode>();
    readonly onHeaderClick = new EventEmitter<ArgsColumn, MouseEvent>();
    readonly onHeaderContextMenu = new EventEmitter<ArgsColumn, MouseEvent>();
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

        this._emptyNode = options.emptyNode ?? (this._jQuery ? defaultJQueryEmptyNode.bind(this._jQuery) : defaultEmptyNode);
        this._removeNode = options.removeNode ?? (this._jQuery ? defaultJQueryRemoveNode.bind(this._jQuery) : defaultRemoveNode);

        const boundThis = bindThis(this);
        this._eventDisposer = new AbortController();
        this._on = addListener.bind({ eventDisposer: this._eventDisposer, jQuery: this._jQuery, uid: this._uid });
        this._off = removeListener.bind({ jQuery: this._jQuery, uid: this._uid });
        this._trigger = triggerGridEvent.bind(this);

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

        const { signals, refs } = createGridSignalsAndRefs();
        this._refs = refs;
        this._mapBands = mapBands.bind(null, refs);
        this._forEachBand = forEachBand.bind(null, refs);
        this._signals = signals;

        const frozenTopBottomChanged = () => {
            if (this._initialized) {
                this.invalidateAllRows?.();
                this.render?.();
            }
        };
        const pinnedStartEndChanged = () => {
            if (this._initialized && this._ignorePinChangeUntil <= new Date().getTime()) {
                this.invalidateColumns();
            }
        };
        this._signalsDisposers.push(observeSignal(signals.frozenTopRows, frozenTopBottomChanged));
        this._signalsDisposers.push(observeSignal(signals.frozenBottomRows, frozenTopBottomChanged));
        this._signalsDisposers.push(observeSignal(signals.pinnedStartCols, pinnedStartEndChanged));
        this._signalsDisposers.push(observeSignal(signals.pinnedEndCols, pinnedStartEndChanged));

        this.validateAndEnforceOptions();
        this.setOptionDependentSignals();

        this._colDefaults.width = options.defaultColumnWidth;
        this._editController = {
            "commitCurrentEdit": boundThis.commitCurrentEdit,
            "cancelCurrentEdit": boundThis.cancelCurrentEdit
        };

        this._emptyNode(this._container);

        this._container.style.overflow = "hidden";
        this._container.style.outline = "0";
        this._container.classList.add(this._uid);

        // set up a positioning container if needed
        if (!/relative|absolute|fixed/.test(getComputedStyle(this._container).position)) {
            this._container.style.position = "relative";
        }

        this._container.appendChild(<div class="slick-focus-sink" tabindex={0} ref={el => this._focusSink1 = el} />);

        this._layout = typeof options.layoutEngine === "function" ? options.layoutEngine() : (options.layoutEngine ?? new BasicLayout());

        this.setAllCols(columns, { initProps: true });
        this._scrollDims = getScrollBarDimensions();

        if (options.groupingPanel) {
            this.createGroupingPanel();
        }

        const prevLifecycleRoot = currentLifecycleRoot(this._container);
        try {
            this._layout.init({
                onAfterInit: this.onAfterInit,
                getAllColumns: boundThis.getAllColumns,
                getColumns: boundThis.getColumns,
                getContainerNode: boundThis.getContainerNode,
                getDataLength: boundThis.getDataLength,
                getOptions: boundThis.getOptions,
                getSignals: boundThis.getSignals,
                getViewportInfo: boundThis.getViewportInfo,
                refs: this._refs,
                removeNode: this._removeNode,
                registerPlugin: boundThis.registerPlugin,
                unregisterPlugin: boundThis.unregisterPlugin,
                getPluginByName: boundThis.getPluginByName
            });

            this.applyLegacyHeightOptions();
        }
        finally {
            currentLifecycleRoot(prevLifecycleRoot);
        }

        this._container.append(this._focusSink2 = this._focusSink1.cloneNode() as HTMLElement);

        if (options.viewportClass)
            this.getViewports().forEach(vp => addClass(vp, options.viewportClass));

        if (!options.explicitInitialization) {
            this.init();
        }

        this.bindToData();
    }

    private applyLegacyHeightOptions() {
        applyLegacyHeightOptions({ groupingPanel: this._groupingPanel, opt: this._options, refs: this._refs });
    }

    private createGroupingPanel() {
        if (this._groupingPanel)
            return;

        this._groupingPanel = <div hidden={!this._options.showGroupingPanel} class="slick-grouping-panel">
            {this._options.createPreHeaderPanel && <div class="slick-preheader-panel" />}
        </div> as HTMLElement;

        this._focusSink1?.insertAdjacentElement("afterend", this._groupingPanel);
    }

    private getSignals(): GridSignals {
        return this._signals;
    }

    init(): void {
        if (this._initialized)
            return;

        try {
            this.calcViewportSize();
            this._layout.adjustFrozenRowsOption?.();

            // header columns and cells may have different padding/border skewing width calculations (box-sizing, hello?)
            // calculate the diff so we can set consistent sizes
            this.measureCellPaddingAndBorder();
            this.setOverflow();
            this.updateViewColLeftRight();
            this.adjustPinnedColsLimit();
            this.createCssRules();
            this.createColumnHeaders();
            this.createColumnFooters();
            this.applyColumnWidths()
            this.setupColumnSort();
        } finally {
            this._initialized = true;
        }
        this.resizeCanvas();
        this.bindAncestorScrollEvents();

        const boundThis = bindThis(this);
        this._on(this._container, "resize", boundThis.resizeCanvas);

        this.getViewports().forEach(vp => {
            var scrollTicking = false;
            this._on(vp, "scroll", (e) => {
                if (!scrollTicking) {
                    scrollTicking = true;

                    window.requestAnimationFrame(() => {
                        this.handleScroll();
                        scrollTicking = false;
                    });
                }
            });
            this._on(vp, "wheel", boundThis.handleMouseWheel as any);
            this._on(vp, "mousewheel" as any, boundThis.handleMouseWheel);
        });

        this._forEachBand(band => {
            const hs = band.headerCols;
            if (hs) {
                hs.onselectstart = () => false;
                this._on(hs, "contextmenu", boundThis.handleHeaderContextMenu);
                this._on(hs, "click", boundThis.handleHeaderClick);
                if (this._jQuery) {
                    this._jQuery(hs)
                        .on('mouseenter.' + this._uid, '.slick-header-column', boundThis.handleHeaderMouseEnter)
                        .on('mouseleave.' + this._uid, '.slick-header-column', boundThis.handleHeaderMouseLeave);
                }
                else {
                    // need to reimplement this similar to jquery events
                    this._on(hs, "mouseenter", e => (e.target as HTMLElement).closest(".slick-header-column") &&
                        boundThis.handleHeaderMouseEnter(e));
                    this._on(hs, "mouseleave", e => (e.target as HTMLElement).closest(".slick-header-column") &&
                        boundThis.handleHeaderMouseLeave(e));
                }
            }

            band.headerRowCols && this._on(band.headerRowCols.parentElement, 'scroll', boundThis.handleHeaderFooterRowScroll);
            band.footerRowCols && this._on(band.footerRowCols.parentElement, 'scroll', boundThis.handleHeaderFooterRowScroll);
        });

        [this._focusSink1, this._focusSink2].forEach(fs => this._on(fs, "keydown", boundThis.handleKeyDown));

        var canvases = Array.from<HTMLElement>(this.getCanvases());
        canvases.forEach(canvas => {
            this._on(canvas, "keydown", boundThis.handleKeyDown)
            this._on(canvas, "click", boundThis.handleClick)
            this._on(canvas, "dblclick", boundThis.handleDblClick)
            this._on(canvas, "contextmenu", boundThis.handleContextMenu);
        });

        this._draggableInstance = Draggable({
            containerElement: this._container,
            //allowDragFrom: 'div.slick-cell',
            // the slick cell parent must always contain `.dnd` and/or `.cell-reorder` class to be identified as draggable
            //allowDragFromClosest: 'div.slick-cell.dnd, div.slick-cell.cell-reorder',
            preventDragFromKeys: ['ctrlKey', 'metaKey'],
            onDragInit: boundThis.handleDragInit,
            onDragStart: boundThis.handleDragStart,
            onDrag: boundThis.handleDrag,
            onDragEnd: boundThis.handleDragEnd
        });

        canvases.forEach(canvas => {
            if (this._jQuery) {
                this._jQuery(canvas)
                    .on('mouseenter' + this._uid, '.slick-cell', boundThis.handleMouseEnter)
                    .on('mouseleave' + this._uid, '.slick-cell', boundThis.handleMouseLeave);
            }
            else {
                this._on(canvas, "mouseenter", e => (e.target as HTMLElement)?.classList?.contains("slick-cell") && boundThis.handleMouseEnter(e), { capture: true });
                this._on(canvas, "mouseleave", e => (e.target as HTMLElement)?.classList?.contains("slick-cell") && boundThis.handleMouseLeave(e), { capture: true });
            }
        });

        // Work around http://crbug.com/312427.
        if (navigator.userAgent.toLowerCase().match(/webkit/) &&
            navigator.userAgent.toLowerCase().match(/macintosh/)) {
            canvases.forEach(c => {
                this._on(c, "wheel" as any, boundThis.handleMouseWheel as any);
                this._on(c, "mousewheel" as any, boundThis.handleMouseWheel as any);
            });
        }

        SleekGrid.onAfterInit.notify({ grid: this });
        this.onAfterInit.notify({ grid: this });
    }

    registerPlugin(plugin: GridPlugin): void {
        this._plugins.unshift(plugin);
        plugin.init(this);
    }

    unregisterPlugin(plugin: GridPlugin): void {
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

    getPluginByName(name: string): GridPlugin {
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
            this._selectionModel.onSelectedRangesChanged.subscribe(bindThis(this).handleSelectedRangesChanged);
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

    private getBandRefsForCell(cell: number): GridBandRefs {
        const refs = this._refs;
        if (cell != null) {
            if (refs.pinnedStartLast >= 0 && cell <= refs.pinnedStartLast)
                return refs.start;
            if (refs.pinnedEndFirst != Infinity && cell >= refs.pinnedEndFirst)
                return refs.end;
        }
        return refs.main;
    }

    public getLayoutInfo(): GridLayoutInfo {
        const { frozenTopRows, frozenBottomRows, pinnedStartCols, pinnedEndCols } = this._refs;
        const { supportFrozenRows, supportFrozenBottom, supportPinnedCols, supportPinnedEnd } = this._layout;
        return {
            frozenTopRows,
            frozenBottomRows,
            pinnedStartCols,
            pinnedEndCols,
            supportFrozenRows,
            supportFrozenBottom,
            supportPinnedCols,
            supportPinnedEnd
        };
    }

    getCanvasNode(row?: number, cell?: number): HTMLElement {
        const refs = this._refs;
        let band = this.getBandRefsForCell(cell);
        if (row != null) {
            const { frozenBottomFirst, frozenTopLast } = this._refs;
            if (frozenBottomFirst >= 0 && row >= frozenBottomFirst)
                return band.canvas.top;
            if (frozenTopLast != Infinity && row <= frozenTopLast)
                return band.canvas.bottom;
        }
        return band.canvas.body;
    }

    getCanvases(): any | HTMLElement[] {
        const canvases = getAllCanvasNodes(this._refs);
        return this._jQuery ? this._jQuery(canvases) : canvases;
    }

    getActiveCanvasNode(e?: { target: EventTarget }): HTMLElement {
        if (e) { // compatibility with celldecorator plugin
            this._activeCanvasNode = (e.target as HTMLElement)?.closest?.('.grid-canvas');
        }
        return this._activeCanvasNode;
    }

    getViewportNode(row?: number, cell?: number): HTMLElement {
        return this.getCanvasNode(row, cell)?.parentElement;
    }

    private getViewportInfo() {
        return this._viewportInfo;
    }

    private getViewports(): HTMLElement[] {
        return getAllViewportNodes(this._refs);
    }

    getActiveViewportNode(e?: { target: EventTarget }): HTMLElement {
        if (e) { // compatibility with celldecorator plugin
            this._activeViewportNode = (e.target as HTMLElement)?.closest?.('.slick-viewport');
        }

        return this._activeViewportNode;
    }

    private getAvailableWidth() {
        return this._viewportInfo.hasVScroll ? this._viewportInfo.width - this._scrollDims.width : this._viewportInfo.width;
    }

    private applyColumnWidths(): void {

        applyColumnWidths({
            cols: this.getColumns(),
            cssColRulesL: this._cssColRulesL,
            cssColRulesR: this._cssColRulesR,
            container: this._container,
            opts: this.getOptions(),
            refs: this._refs
        });
    }

    private adjustPinnedColsLimit() {
        let colCount = this._cols.length;
        let pinnedAvail = colCount;
        const config = this._refs.config;
        let pinnedStartCols = config.pinnedStartCols > 0 ? Math.min(config.pinnedStartCols, pinnedAvail) : 0;
        pinnedAvail -= pinnedStartCols;
        let pinnedEndCols = config.pinnedEndCols > 0 ? Math.min(config.pinnedEndCols, pinnedAvail) : 0;
        do {
            const calc = this.calcCanvasBandWidths({ pinnedStartLast: pinnedStartCols - 1, pinnedEndFirst: pinnedEndCols > 0 ? colCount - pinnedEndCols : Infinity });
            if (calc.mainBand >= 100)
                break;

            if (pinnedEndCols > 0)
                pinnedEndCols--;
            else if (pinnedStartCols > 0)
                pinnedStartCols--;
            else
                break;
        } while (true);

        this._refs.config.pinnedLimit = pinnedStartCols + pinnedEndCols;
    }

    private calcCanvasBandWidths(opt?: { pinnedStartLast: number, pinnedEndFirst: number }) {
        const result = { start: 0, main: 0, end: 0, startBand: 0, endBand: 0, mainBand: 0, hasHScroll: false };
        const cols = this._cols;
        const pinnedStartLast = opt?.pinnedStartLast ?? this._refs.pinnedStartLast;
        const pinnedEndFirst = opt?.pinnedEndFirst ?? this._refs.pinnedEndFirst;
        const start = this._refs.start;
        const end = this._refs.end;
        let c = cols.length;
        while (c--) {
            if (c <= pinnedStartLast) {
                result.start += cols[c].width;
            }
            else if (c >= pinnedEndFirst) {
                result.end += cols[c].width;
            }
            else {
                result.main += cols[c].width;
            }
        }
        result.hasHScroll = this.getAvailableWidth() < result.main + result.start + result.end;

        const startViewport = start.canvas.body?.parentElement;
        const endViewport = end.canvas.body?.parentElement;
        result.startBand = result.start + ((startViewport?.offsetWidth || 0) - (startViewport?.clientWidth || 0)); // include pinned start border
        result.endBand = result.end + ((endViewport?.offsetWidth || 0) - (endViewport?.clientWidth || 0)); // include pinned end border
        result.mainBand = this._viewportInfo.width - result.startBand - result.endBand;
        if (result.mainBand < 0) {
            result.mainBand = 0;
        }
        return result;
    }

    private updateBandCanvasWidths(): boolean {
        const oldCanvasWidths = this._mapBands(x => x.canvasWidth);;
        const newCanvasWidths = this.calcCanvasBandWidths();
        const { start, main, end } = this._refs;
        start.canvasWidth = newCanvasWidths.start;
        main.canvasWidth = newCanvasWidths.main;
        end.canvasWidth = newCanvasWidths.end;
        const widthChanged = start.canvasWidth !== oldCanvasWidths[0] ||
            main.canvasWidth !== oldCanvasWidths[1] ||
            end.canvasWidth !== oldCanvasWidths[2];

        this._viewportInfo.hasHScroll = newCanvasWidths.hasHScroll;

        const style = this._cssVarRules?.style ?? this._container.style;
        setStyleProp(style, "--sg-start-width", newCanvasWidths.startBand + "px");
        setStyleProp(style, "--sg-end-width", newCanvasWidths.endBand + "px");
        setStyleProp(style, "--sg-main-width", newCanvasWidths.mainBand + "px");
        setStyleProp(style, "--sg-virtual-width", newCanvasWidths.main + "px");
        return widthChanged;
    }

    private updateCanvasWidth(forceColumnWidthsUpdate?: boolean): void {
        const widthChanged = this.updateBandCanvasWidths();
        if (widthChanged || forceColumnWidthsUpdate) {
            this.applyColumnWidths();
        }
    }

    private bindAncestorScrollEvents() {
        const refs = this._refs;
        const canvas = refs.main.canvas.body;
        let elem = canvas as HTMLElement;
        while ((elem = elem?.parentNode as HTMLElement) != document.body && elem != null) {
            // bind to scroll containers only
            if (elem == canvas.parentElement || elem.scrollWidth != elem.clientWidth || elem.scrollHeight != elem.clientHeight) {
                this._on(elem, 'scroll', bindThis(this).handleActiveCellPositionChange);
                this._boundAncestorScroll.push(elem);
            }
        }
    }

    private unbindAncestorScrollEvents(): void {
        if (this._boundAncestorScroll) {
            for (var x of this._boundAncestorScroll)
                this._off(x, 'scroll', bindThis(this).handleActiveCellPositionChange);
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
        var header = this.getHeaderColumn(idx);
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

        this._trigger(this.onBeforeHeaderCellDestroy, {
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

        this._trigger(this.onHeaderCellRendered, {
            node: header,
            column: columnDef
        });
    }

    getHeader(): HTMLElement {
        return this._refs.main.headerCols;
    }

    getHeaderColumn(cell: number | string): HTMLElement {
        if (typeof cell === "string")
            cell = this.getColumnIndex(cell);

        if (cell == null)
            return null;

        const band = this.getBandRefsForCell(cell);
        return band.headerCols?.children.item(cell - band.cellOffset) as HTMLDivElement;
    }

    getGroupingPanel(): HTMLElement {
        return this._groupingPanel;
    }

    getPreHeaderPanel(): HTMLElement {
        return this._groupingPanel?.querySelector('.slick-preheader-panel');
    }

    getHeaderRow(): HTMLElement {
        return this._refs.main.headerRowCols;
    }

    getHeaderRowColumn(cell: string | number): HTMLElement {
        if (typeof cell === "string")
            cell = this.getColumnIndex(cell);

        if (cell == null)
            return;

        const band = this.getBandRefsForCell(cell);
        return band.headerRowCols?.children.item(cell - band.cellOffset) as HTMLDivElement;
    }

    getFooterRow(): HTMLElement {
        return this._refs.main.footerRowCols;
    }

    getFooterRowColumn(cell: string | number): HTMLElement {
        if (typeof cell === "string")
            cell = this.getColumnIndex(cell);

        if (cell == null)
            return null;

        const band = this.getBandRefsForCell(cell);
        return band.footerRowCols?.children.item(cell - band.cellOffset) as HTMLDivElement;
    }

    private createColumnFooters(): void {
        this._mapBands(band => band.footerRowCols).forEach(frc => {
            frc.querySelectorAll(".slick-footerrow-column")
                .forEach((el) => {
                    var columnDef = this.getColumnFromNode(el);
                    if (columnDef) {
                        this._trigger(this.onBeforeFooterRowCellDestroy, {
                            node: el as HTMLElement,
                            column: columnDef
                        });
                    }
                })

            this._emptyNode(frc);
        });

        var cols = this._cols;
        for (var i = 0; i < cols.length; i++) {
            const footerRowColsNode = this.getBandRefsForCell(i).footerRowCols;
            if (!footerRowColsNode)
                continue;
            var m = cols[i];

            const footerRowCell = <div class={"slick-footerrow-column l" + i + " r" + i} /> as HTMLElement;
            footerRowCell.dataset.c = i.toString();
            this._jQuery && this._jQuery(footerRowCell).data("column", m);

            if (m.footerCssClass)
                addClass(footerRowCell, m.footerCssClass);
            else if (m.cssClass)
                addClass(footerRowCell, m.cssClass);

            footerRowColsNode.appendChild(footerRowCell);

            this._trigger(this.onFooterRowCellRendered, {
                node: footerRowCell,
                column: m
            });
        }
    }

    private createColumnHeaders(): void {
        this._mapBands(band => band.headerCols).forEach(hc => {
            hc.querySelectorAll(".slick-header-column")
                .forEach((el) => {
                    var columnDef = this.getColumnFromNode(el);
                    if (columnDef) {
                        this._trigger(this.onBeforeHeaderCellDestroy, {
                            node: el as HTMLElement,
                            column: columnDef
                        });
                    }
                });

            this._emptyNode(hc);
        });

        this.updateBandCanvasWidths();

        this._mapBands(band => band.headerRowCols).forEach(hrc => {
            hrc.querySelectorAll(".slick-headerrow-column")
                .forEach((el) => {
                    var columnDef = this.getColumnFromNode(el);
                    if (columnDef) {
                        this._trigger(this.onBeforeHeaderRowCellDestroy, {
                            node: el as HTMLElement,
                            column: columnDef
                        });
                    }
                });
            this._emptyNode(hrc);
        });

        const cols = this._cols, refs = this._refs;
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

            const { pinnedStartLast, pinnedEndFirst } = refs;

            const header = this.getBandRefsForCell(i).headerCols.appendChild(
                <div class={["slick-header-column", m.headerCssClass,
                    i <= pinnedStartLast && "frozen pinned-start",
                    i >= pinnedEndFirst && "frozen pinned-end",
                    m.sortable && "slick-header-sortable"]}
                    data-id={m.id} data-c={i} id={`${this._uid}${m.id}`} title={m.toolTip || ""}
                    style={{ width: `${m.width - this._headerColumnWidthDiff}px` }}>
                    {nameSpan}
                    {m.sortable && <span class="slick-sort-indicator"><sub></sub></span>}
                </div> as HTMLElement);

            this._jQuery?.(header).data("column", m);
            this._trigger(this.onHeaderCellRendered, { node: header, column: m });

            if (this._options.showHeaderRow) {
                const headerRowColsNode = this.getBandRefsForCell(i).headerRowCols;
                if (!headerRowColsNode)
                    continue;
                const headerRowCell = headerRowColsNode.appendChild(
                    <div class={"slick-headerrow-column l" + i + " r" + i} data-c={i} /> as HTMLElement);
                this._jQuery?.(headerRowCell).data("column", m);
                this._trigger(this.onHeaderRowCellRendered, { node: headerRowCell, column: m });
            }
        }

        this.setSortColumns(this._sortColumns);
        this.setupColumnResize();
        if (this._options.enableColumnReorder) {
            this.setupColumnReorder();
        }
    }

    private setupColumnSort(): void {
        this._columnSortHandler ??= columnSortHandler.bind(this);
        this._mapBands(band => band.headerCols).forEach(el => this._on(el, 'click', this._columnSortHandler));
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
        const { pinnedStartLast, pinnedEndFirst } = this._refs;
        return pinnedStartLast >= 0 || pinnedEndFirst != Infinity;
    }

    private scrollColumnsLeft(): void {
        this.getScrollContainerX().scrollLeft = this.getScrollContainerX().scrollLeft - 10;
    }

    private scrollColumnsRight(): void {
        this.getScrollContainerX().scrollLeft = this.getScrollContainerX().scrollLeft + 10;
    }

    private setupColumnReorder(): void {
        // @ts-ignore
        if (typeof Sortable === "undefined")
            return;

        this.sortableColInstances?.forEach(x => x.destroy());
        let columnScrollTimer: number = null;

        let canDragScroll;
        const hasPinnedCols = this.hasPinnedCols();
        const sortableOptions: any = {
            animation: 50,
            direction: 'horizontal',
            chosenClass: 'slick-header-column-dragging',
            ghostClass: 'slick-sortable-placeholder',
            draggable: '.slick-header-column',
            filter: ".slick-resizable-handle, .no-drag",
            preventOnFilter: false,
            dragoverBubble: false,
            revertClone: true,
            scroll: !hasPinnedCols,
            onStart: (e: { item: any; originalEvent: MouseEvent; }) => {
                canDragScroll = !hasPinnedCols ||
                    SleekGrid.offset(e.item)!.left > SleekGrid.offset(this.getScrollContainerX())!.left;

                if (canDragScroll && e.originalEvent && e.originalEvent.pageX > this._container.clientWidth) {
                    if (!(columnScrollTimer)) {
                        columnScrollTimer = setInterval(bindThis(this).scrollColumnsLeft, 100);
                    }
                } else if (canDragScroll && e.originalEvent && e.originalEvent.pageX < SleekGrid.offset(this.getScrollContainerX())!.left) {
                    if (!(columnScrollTimer)) {
                        columnScrollTimer = setInterval(bindThis(this).scrollColumnsRight, 100);
                    }
                } else {
                    clearInterval(columnScrollTimer);
                    columnScrollTimer = null;
                }
                this._container.classList.add('slick-column-reordering');
            },
            onEnd: (e: MouseEvent & { item: any; originalEvent: MouseEvent; }) => {
                const cancel = false;
                this._container.classList.remove('slick-column-reordering');
                clearInterval(columnScrollTimer);
                columnScrollTimer = null;
                if (cancel || !this.getEditorLock()?.commitCurrentEdit()) {
                    return;
                }

                let reorderedCols: Column<TItem>[] = [];
                this._mapBands(band => band.headerCols).forEach((headerCols, i) => {
                    if (!headerCols.contains(e.item))
                        return;
                    this.reorderColumns((this.sortableColInstances[i]?.toArray?.() ?? []));
                });

                e.stopPropagation();
                this.setupColumnResize();
                if (this._activeCellNode) {
                    this.setFocus(); // refocus on active cell
                }
            }
        }

        this.sortableColInstances = this._mapBands(band => band.headerCols).map(x =>
            // @ts-ignore
            Sortable.create(x, sortableOptions));
    }

    private colResizing() {
        this.updateCanvasWidth(false);
        this.applyColumnHeaderWidths();
        this._options.syncColumnCellResize && this.applyColumnWidths();
    }

    private setupColumnResize(): void {
        this._colResizeDisposer && this._colResizeDisposer.abort();
        this._colResizeDisposer = new AbortController();
        const boundThis = bindThis(this);
        setupColumnResize({
            absoluteColMinWidth: this._absoluteColMinWidth,
            disposer: this._colResizeDisposer,
            colResized: boundThis.columnsResized,
            colResizing: boundThis.colResizing,
            cols: this._cols,
            container: this._container,
            headerColsElements: this._mapBands(band => band.headerCols),
            getEditorLock: boundThis.getEditorLock,
            removeNode: this._removeNode,
            options: this._options
        });
    }

    public columnsResized(invalidate = true) {
        this.adjustPinnedColsLimit();
        this.applyColumnHeaderWidths();
        this.applyColumnWidths();
        invalidate && this.invalidateAllRows();
        this.updateCanvasWidth(true);
        this.render();
        this._trigger(this.onColumnsResized);
    }

    private setOverflow(): void {
        const alwaysHS = this._options.alwaysAllowHorizontalScroll;
        const alwaysVS = this._options.alwaysShowVerticalScroll;
        const autoHeight = this._options.autoHeight;

        this._refs.main.canvas.body.parentElement.style.overflowX = alwaysHS ? "scroll" : "auto";
        this._refs.main.canvas.body.parentElement.style.overflowY = alwaysVS ? "scroll" : autoHeight ? "hidden" : "auto";

        if (this._options.viewportClass)
            this.getViewports().forEach(vp => addClass(vp, this._options.viewportClass));
    }

    private measureCellPaddingAndBorder(): void {
        const h = ["border-left-width", "border-right-width", "padding-left", "padding-right"];
        const v = ["border-top-width", "border-bottom-width", "padding-top", "padding-bottom"];

        let el = this._refs.main.headerCols.appendChild(<div class="slick-header-column" style={{ visibility: "hidden" }} /> as HTMLElement);
        this._headerColumnWidthDiff = 0;
        let cs = getComputedStyle(el);
        if (cs.boxSizing != "border-box")
            h.forEach(val => this._headerColumnWidthDiff += parsePx(cs.getPropertyValue(val)) || 0);
        el.remove();

        const r = this.getCanvasNode().appendChild(<div class="slick-row">
            {el = <div class="slick-cell" id="" style={{ visibility: "hidden" }} /> as HTMLElement};
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


    private removeCssRules() {
        this._styleNode?.remove();
        this._styleNode = null;
        this._stylesheet = null;
    }

    private createCssRules() {
        this._styleNode = createCssRules({
            opt: this._options,
            uid: this._uid,
            container: this._container,
            scrollDims: this._scrollDims,
            cellHeightDiff: this._cellHeightDiff,
            colCount: this._cols.length
        })?.styleNode;

        if (this._styleNode != null && this._stylesheet == null) {
            const res = findStylesheetByUID(this._uid, this._styleNode);
            this._stylesheet = res.stylesheet;
            this._cssColRulesL = res.colCssRulesL;
            this._cssColRulesR = res.colCssRulesR;
            this._cssVarRules = res.varRule;
        }
    }

    destroy() {
        this.getEditorLock().cancelCurrentEdit();

        this._trigger(this.onBeforeDestroy);

        var i = this._plugins.length;
        while (i--) {
            this.unregisterPlugin(this._plugins[i]);
        }

        if (this._draggableInstance) {
            this._draggableInstance.destroy();
            this._draggableInstance = null;
        }

        if (this._options.enableColumnReorder && this._jQuery && (this._jQuery.fn as any).sortable) {
            (this._jQuery(this._mapBands(band => band.headerCols)).filter(":ui-sortable") as any).sortable("destroy");
        }

        this.unbindAncestorScrollEvents();
        this.unbindFromData();
        this.unregisterSelectionModel();
        if (this._jQuery) {
            this._jQuery(this._container).off("." + this._uid);
            this._jQuery(document).off("." + this._uid);
            this._jQuery(this._container).find(".slick-header-column, .slick-headerrow-column, .slick-footerrow-column, .slick-focus-sink").off("." + this._uid);
            this._jQuery(this._mapBands(band => band.headerCols)).off("." + this._uid);
            this._jQuery(this._mapBands(band => band.headerRowCols)).off("." + this._uid);
            this._jQuery(this._mapBands(band => band.footerRowCols)).off("." + this._uid);
        }
        this.removeCssRules();
        this.sortableColInstances?.forEach(instance => { try { instance.destroy() } catch (e) { console.warn(e); } });

        const refs = this._refs;
        this._signalsDisposers?.forEach(d => d?.());
        this._signalsDisposers = [];
        const canvasNodes = getAllCanvasNodes(refs);
        if (this._jQuery)
            this._jQuery(canvasNodes).off("draginit dragstart dragend drag");
        else
            canvasNodes.forEach(el => el && this._removeNode(el));

        this._layout?.destroy();
        forEachBand(refs, band => disposeBandRefs(band, this._removeNode));
        this._removeNode(refs.topPanel);
        refs.topPanel = null;

        this._emptyNode(this._container);
        this._eventDisposer?.abort();
        this._colResizeDisposer?.abort();

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

    getEditorFactory(): EditorFactory {
        return this._options.editorFactory;
    }

    getEditorLock(): EditorLock {
        return this._options.editorLock;
    }

    getEditController(): EditController {
        return this._editController;
    }

    getColumnById(id: string): Column<TItem> {
        return id ? this._allCols[this._allColsById[id]] : null;
    }

    getColumnIndex(id: string, opt?: { inAll?: boolean }): number {
        return id ? (opt?.inAll ? this._allColsById[id] : this._colById[id]) : null;
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
            h = this.getHeaderColumn(i);
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
        this._mapBands(band => band.headerCols).forEach(el => headerColumnEls = headerColumnEls.concat(Array.from(el.children)));
        headerColumnEls.forEach(hel => {
            hel.classList.remove("slick-header-column-sorted");
            const si = hel.querySelector(".slick-sort-indicator");
            si && si.classList.remove("slick-sort-indicator-asc", "slick-sort-indicator-desc");
        });

        this._sortColumns.forEach((col, i) => {
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
                    const sub = si.querySelector("sub");
                    sub && (sub.textContent = this._sortColumns.length > 1 ? (i + 1).toString() : "");
                }
            }
        });
    }

    getSortColumns(): ColumnSort[] {
        return this._sortColumns;
    }

    private handleSelectedRangesChanged = (e: EventData, ranges: CellRange[]): void => {
        var previousSelectedRows = this._selectedRows.slice(0); // shallow copy previously selected rows for later comparison
        this._selectedRows = [];
        var hash: any = Object.create(null), cols = this._cols;
        for (var i = 0; i < ranges.length; i++) {
            for (var j = ranges[i].fromRow; j <= ranges[i].toRow; j++) {
                if (!hash[j]) {  // prevent duplicates
                    this._selectedRows.push(j);
                    hash[j] = Object.create(null);
                }
                for (var k = ranges[i].fromCell; k <= ranges[i].toCell; k++) {
                    if (this.canCellBeSelected(j, k)) {
                        const cid = cols[k].id;
                        if (!isPollutingKey(j as any) && !isPollutingKey(cid)) {
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

            this._trigger(this.onSelectedRowsChanged, {
                rows: this.getSelectedRows(),
                previousSelectedRows: previousSelectedRows,
                caller: caller,
                changedSelectedRows: newSelectedAdditions,
                changedUnselectedRows: newSelectedDeletions
            }, e);
        }

        this._selectedRows = [];
        hash = Object.create(null), cols = this._cols;
        for (var i = 0; i < ranges.length; i++) {
            for (var j = ranges[i].fromRow; j <= ranges[i].toRow; j++) {
                if (!hash[j]) {  // prevent duplicates
                    this._selectedRows.push(j);
                    hash[j] = Object.create(null);
                }
                for (var k = ranges[i].fromCell; k <= ranges[i].toCell; k++) {
                    if (this.canCellBeSelected(j, k)) {
                        const cid = cols[k].id;
                        if (!isPollutingKey(j as any) && !isPollutingKey(cid)) {
                            hash[j][cid] = this._options.selectedCellCssClass;
                        }
                    }
                }
            }
        }
    }

    getAllColumns(): Column<TItem>[] {
        return this._allCols;
    }

    getColumns(): Column<TItem>[] {
        return this._cols;
    }

    private updateViewColLeftRight(): void {
        this._colLeft = [];
        this._colRight = [];
        var x = 0, r: number, cols = this._cols, c: number, l: number = cols.length;
        const { pinnedStartLast, pinnedEndFirst } = this._refs;
        for (var c = 0; c < l; c++) {
            if (pinnedStartLast + 1 === c || pinnedEndFirst === c)
                x = 0;
            r = x + cols[c].width;
            this._colLeft[c] = x;
            this._colRight[c] = r;
            x = r;
        }
    }

    private updateViewCols() {
        const allCols = this._allCols;
        let col: Column;
        let cols: Column[] = [];
        var colById: { [key: string]: number } = {};
        for (col of allCols) {
            if (col.visible !== false)
                cols.push(col);
        }

        this.handleFrozenColsOption(cols, this._options);
        cols = this._layout.reorderViewColumns?.(cols, this._refs) ?? cols;

        this._postRenderActive = this._options.enableAsyncPostRender ?? false;
        this._postCleanupActive = this._options.enableAsyncPostRenderCleanup ?? false;
        for (let i = 0; i < cols.length; i++) {
            col = cols[i];
            colById[col.id] = i;
            if (col.asyncPostRenderCleanup != null)
                this._postCleanupActive = true;
            if (col.asyncPostRender != null)
                this._postRenderActive = true;
        }

        this._cols = cols;
        this._colById = colById;
        this._refs.config.colCount = cols.length;
    }

    /** Set the initial columns, also calls initColumnProps unless opt.initProps is false */
    private setAllCols(initCols: Column[], opt?: { initProps?: boolean }): void {
        initCols ||= [];
        (opt?.initProps ?? true) && initColumnProps(initCols, this._colDefaults);
        const allColsById: any = {};
        for (let i = 0; i < initCols.length; i++) {
            const col = initCols[i];
            allColsById[col.id] = i;
        }
        this._allCols = initCols;
        this._allColsById = allColsById;
        this.updateViewCols();
    }

    private handleFrozenColsOption(viewCols: Column[], options?: GridOptions) {
        if (options?.frozenColumns == null) {
            delete options?.frozenColumns;
        }
        else {
            let toFreezeStart = options.frozenColumns;
            delete options.frozenColumns;
            let i = 0;
            while (i < viewCols.length) {
                const col = viewCols[i++];
                if (toFreezeStart > 0 && col.visible !== false) {
                    col.frozen = true;
                    toFreezeStart--;
                }
                else if (col.frozen !== undefined && col.frozen !== "end")
                    delete col.frozen;
            }
        }
    }

    setColumns(columns: Column<TItem>[]): void {

        let initProps = true;
        if (columns &&
            this._allCols &&
            this._cols &&
            columns.length === this._cols.length &&
            this._allCols.length > this._cols.length &&
            !columns.some(x => this._cols.indexOf(x) < 0) &&
            !this._cols.some(x => columns.indexOf(x) < 0)) {
            // probably called with grid.setColumns(grid.getColumns()) potentially
            // changing orders / widths etc, try to preserve initial columns
            columns = sortToDesiredOrderAndKeepRest(this._allCols, columns.map(x => x.id));
            initProps = false;
        }

        this.setAllCols(columns, { initProps });
        this.invalidateColumns();
    }

    private internalSetVisibleColumns(columnIds: string[]): boolean {
        const idSet = new Set(columnIds);
        let anyChange = false;
        for (const col of this._allCols) {
            const visible = idSet.has(col.id);
            if ((col.visible !== false) != visible) {
                col.visible = visible;
                anyChange = true;
            }
        }
        return anyChange;
    }

    public reorderColumns(columnIds: string[], opt?: { notify?: boolean, setVisible?: string[] }): void {
        opt?.setVisible && this.internalSetVisibleColumns(opt.setVisible);
        const newColumns = sortToDesiredOrderAndKeepRest(this._allCols, columnIds);
        this.setAllCols(newColumns, { initProps: false });
        this.invalidateColumns();
        if (opt?.notify ?? true) {
            this._trigger(this.onColumnsReordered, {});
        }
    }

    public setVisibleColumns(columnIds: string[], opt?: { reorder?: boolean, notify?: boolean }): void {
        const visibilityChange = this.internalSetVisibleColumns(columnIds);
        if (opt?.reorder ?? true) {
            this.reorderColumns(columnIds, { notify: opt?.notify });
        }
        else if (visibilityChange) {
            this.setAllCols(this._allCols, { initProps: false });
            this.invalidateColumns();
            if (opt?.notify ?? true)
                this._trigger(this.onColumnsReordered, {});
        }
    }

    public invalidateColumns() {
        this.updateViewCols();
        this.updateViewColLeftRight();
        if (this._initialized) {
            this._ignorePinChangeUntil = new Date().getTime() + 100;
            this.adjustPinnedColsLimit();
            this.updateViewColLeftRight();
            this.setOverflow();
            this.invalidateAllRows();
            this.createColumnHeaders();
            this.createColumnFooters();
            this.updateGrandTotals();
            this.removeCssRules();
            this.createCssRules();
            this.resizeCanvas();
            this.updateCanvasWidth(true);
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
        this.setOptionDependentSignals();
        this.handleFrozenColsOption(this._cols, args);
        this._layout.afterSetOptions(args);

        if (args.columns && !suppressColumnSet) {
            this.setColumns(args.columns ?? this._allCols);
        }

        if (!suppressSetOverflow) {
            this.setOverflow();
        }

        if (!suppressRender)
            this.render();
    }

    private validateAndEnforceOptions(): void {
        if (this._options.autoHeight) {
            this._options.leaveSpaceForNewRows = false;
        }
    }

    private setOptionDependentSignals() {
        const sig = this._signals;
        const opt = this._options;
        sig.showColumnHeader.value = opt.showColumnHeader;
        sig.showTopPanel.value = opt.showTopPanel;
        sig.showHeaderRow.value = opt.showHeaderRow;
        sig.showFooterRow.value = opt.showFooterRow;
    }

    private viewOnRowCountChanged() {
        this.updateRowCount();
        this.render();
    }

    private viewOnRowsChanged({ args }: { args: { rows: number[] } }) {
        this.invalidateRows(args.rows);
        this.render();
        this.updateGrandTotals();
    }

    private viewOnDataChanged() {
        this.invalidate();
        this.render();
    }

    private bindToData(): void {
        const view = this._data as IDataView;
        if (view) {
            const boundThis = bindThis(this);
            view.onRowCountChanged && view.onRowCountChanged.subscribe(boundThis.viewOnRowCountChanged);
            view.onRowsChanged && view.onRowsChanged.subscribe(boundThis.viewOnRowsChanged);
            view.onDataChanged && view.onDataChanged.subscribe(boundThis.viewOnDataChanged);
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
        return this._refs.topPanel;
    }

    setTopPanelVisibility(visible: boolean): void {
        if (!this._options.showTopPanel != !visible) {
            this._signals.showTopPanel.value = this._options.showTopPanel = !!visible;
            this.resizeCanvas();
        }
    }

    setColumnHeaderVisibility(visible: boolean) {
        if (!this._options.showColumnHeader != !visible) {
            this._signals.showColumnHeader.value = this._options.showColumnHeader = !!visible;
            this.resizeCanvas();
        }
    }

    setFooterRowVisibility(visible: boolean): void {
        if (!this._options.showFooterRow != !visible) {
            this._signals.showFooterRow.value = this._options.showFooterRow = !!visible;
            this.resizeCanvas();
        }
    }

    setGroupingPanelVisibility(visible: boolean): void {
        if (this._options.showGroupingPanel != visible) {
            this._options.showGroupingPanel = visible;
            this._groupingPanel && (this._groupingPanel.hidden = !visible);
            this.resizeCanvas();
        }
    }

    setPreHeaderPanelVisibility(visible: boolean): void {
        this.setGroupingPanelVisibility(visible);
    }

    setHeaderRowVisibility(visible: boolean): void {
        if (!this._options.showHeaderRow != !visible) {
            this._signals.showHeaderRow.value = this._options.showHeaderRow = !!visible;
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
        const { frozenTopLast, frozenBottomFirst } = this._refs;
        const rowHeight = this._options.rowHeight;
        if (row <= frozenTopLast)
            return rowHeight * row;

        if (row >= frozenBottomFirst)
            return rowHeight * (row - frozenBottomFirst);

        if (frozenTopLast >= 0)
            row -= (frozenTopLast + 1);

        return rowHeight * row - this._pageOffset;
    }

    private getRowFromPosition(y: number): number {
        return Math.floor((y + this._pageOffset) / this._options.rowHeight);
    }

    private scrollTo(y: number): void {
        const vpi = this._viewportInfo;
        y = Math.max(y, 0);
        y = Math.min(y, vpi.virtualHeight - Math.round(this.getScrollContainerY().clientHeight) + ((vpi.hasHScroll || this.hasPinnedCols()) ? this._scrollDims.height : 0));

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
            getAllVScrollContainers(this._refs).forEach(sc => sc.scrollTop = newScrollTop);
            this._trigger(this.onViewportChanged);
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

    private cleanupRows(rangeToKeep: ViewRange): void {
        var i: number;
        for (var x in this._rowsCache) {
            i = parseInt(x, 10);
            if (i !== this._activeRow && (i < rangeToKeep.top || i > rangeToKeep.bottom)
                && !this.isFrozenRow(i))
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
        const refs = this._refs;
        const vs = this._viewportInfo;
        this.adjustPinnedColsLimit();
        this.updateBandCanvasWidths();
        vs.width = getInnerWidth(this._container);
        vs.groupingPanelHeight = (this._options.groupingPanel && this._options.showGroupingPanel) ? this._groupingPanel?.offsetHeight || 0 : 0;
        vs.topPanelHeight = this._options.showTopPanel ? (refs.topPanel?.parentElement?.offsetHeight || 0) : 0;
        vs.headerRowHeight = this._options.showHeaderRow ? refs.main.headerRowCols?.parentElement?.offsetHeight || 0 : 0;
        vs.footerRowHeight = this._options.showFooterRow ? refs.main.footerRowCols?.parentElement?.offsetHeight || 0 : 0;
        vs.headerHeight = (this._options.showColumnHeader) ? refs.main.headerCols?.parentElement?.offsetHeight || 0 : 0;

        if (this._options.autoHeight) {
            vs.height = this._options.rowHeight * this.getDataLengthIncludingAddNew();
            if (this._refs.main.canvasWidth > vs.width) {
                vs.height += this._scrollDims.height;
            }
        } else {

            var style = getComputedStyle(this._container);
            vs.height =
                parsePx(style.height)
                - parsePx(style.paddingTop)
                - parsePx(style.paddingBottom)
                - vs.headerHeight
                - vs.topPanelHeight
                - vs.headerRowHeight
                - vs.footerRowHeight
                - vs.groupingPanelHeight;
        }

        vs.numVisibleRows = Math.ceil(vs.height / this._options.rowHeight);
        this._refs.config.frozenLimit = this._options.autoHeight ? 0 : Math.floor(vs.height / this._options.rowHeight) - 1;
    }

    resizeCanvas = (): void => {
        if (!this._initialized) {
            return;
        }

        this.calcViewportSize();
        this.setPaneHeights();

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

    public getScrollContainerX() {
        return this._refs.main.canvas.body.parentElement;
    }

    public getScrollContainerY() {
        return this._refs.main.canvas.body.parentElement;
    }

    updateRowCount(): void {
        if (!this._initialized) {
            return;
        }

        var dataLengthIncludingAddNew = this.getDataLengthIncludingAddNew();
        var scrollCanvas = this._refs.main.canvas.body;
        var oldH = Math.round(parsePx(getComputedStyle(scrollCanvas).height));

        let numberOfRows = dataLengthIncludingAddNew + (this._options.leaveSpaceForNewRows ? this._viewportInfo.numVisibleRows - 1 : 0);
        this._refs.config.dataLength = this.getDataLength();
        const { frozenTopRows, frozenBottomRows } = this._refs;
        numberOfRows -= frozenTopRows + frozenBottomRows;

        var tempViewportH = Math.floor(parsePx(getComputedStyle(this.getScrollContainerY()).height));
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

        vpi.virtualHeight = Math.max(this._options.rowHeight * numberOfRows, Math.floor(tempViewportH - this._scrollDims.height));

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
            this.setVirtualHeight();
            this._scrollTop = this.getScrollContainerY().scrollTop;
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

    private setPaneHeights() {
        const style = this._cssVarRules?.style ?? this._container.style;
        const { frozenTopRows, frozenBottomRows } = this._refs;
        const topHeight = frozenTopRows * this._options.rowHeight;
        const bottomHeight = frozenBottomRows * this._options.rowHeight;
        setStyleProp(style, '--sg-top-height', topHeight + "px");
        setStyleProp(style, '--sg-bottom-height', bottomHeight + "px");
        setStyleProp(style, '--sg-body-height', (this._viewportInfo.height - topHeight - bottomHeight) + "px");
        this.setVirtualHeight();
    }

    private setVirtualHeight() {
        const style = this._cssVarRules?.style ?? this._container.style;
        setStyleProp(style, '--sg-virtual-height', this._viewportInfo.realScrollHeight + "px");
    }

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
            range.rightPx = this._refs.main.canvasWidth;
        }
        else {
            range.leftPx -= this._viewportInfo.width;
            range.rightPx += this._viewportInfo.width;

            range.leftPx = Math.max(0, range.leftPx);
            range.rightPx = Math.min(this._refs.main.canvasWidth, range.rightPx);
        }

        return range;
    }

    private ensureCellNodesInRowsCache(row: number): void {
        var cacheEntry = this._rowsCache[row];
        if (cacheEntry && cacheEntry.cellRenderQueue.length) {
            for (const rowNode of [cacheEntry.rowNodeE, cacheEntry.rowNodeC, cacheEntry.rowNodeS]) {
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

    private isFrozenRow(row: number): boolean {
        const { frozenTopLast, frozenBottomFirst } = this._refs;
        return row <= frozenTopLast || row >= frozenBottomFirst;
    }

    private cleanUpCells(rangeToKeep: ViewRange, row: number): void {
        if (this.isFrozenRow(row))
            return;

        var cacheEntry = this._rowsCache[row];

        // Remove cells outside the range.
        const cellsToRemove = [], { pinnedStartLast, pinnedEndFirst } = this._refs;
        for (var x in cacheEntry.cellNodesByColumnIdx) {

            var i = parseInt(x, 10);

            // Ignore frozen columns
            if (i <= pinnedStartLast || i >= pinnedEndFirst) {
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
                node && this._removeNode(node);
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
        var processedRows = [];
        var cellsAdded;
        var colspan;
        var cols = this._cols;

        const args = this.createRowCellRenderArgs(null, -1); // cell != null indicates cell rendering mode

        for (var row = range.top, btm = range.bottom; row <= btm; row++) {
            args.cachedRow = cacheEntry = this._rowsCache[row];
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

            // TODO:  shorten this loop (index? heuristics? binary search?)
            for (var cell = 0, colCount = cols.length; cell < colCount; cell++) {
                // Cells to the right are outside the range.
                if (this._colLeft[cell] > range.rightPx) {
                    break;
                }

                // Already rendered.
                if ((colspan = cacheEntry.cellColSpans[cell]) != null) {
                    cell += (colspan > 1 ? colspan - 1 : 0);
                    continue;
                }

                var colMetadata: ColumnMetadata = null;
                colspan = 1;
                if (colsMetadata) {
                    colMetadata = colsMetadata[cols[cell].id] || colsMetadata[cell];
                    colspan = (colMetadata && colMetadata.colspan) || 1;
                    if (colspan === "*") {
                        colspan = colCount - cell;
                    }
                }

                if (args.colRight[Math.min(colCount - 1, cell + colspan - 1)] > range.leftPx) {
                    args.item = this.getDataItem(row);
                    args.row = row;
                    args.cell = cell;
                    args.colspan = colspan;
                    args.colMetadata = colMetadata;
                    renderCell(args);
                    cellsAdded++;
                }

                cell += (colspan > 1 ? colspan - 1 : 0);
            }

            if (cellsAdded) {
                processedRows.push(row);
            }
        }

        if (!args.sb.length) {
            return;
        }

        var x = document.createElement("div");
        x.innerHTML = args.sb.join("");

        var processedRow;
        var node: HTMLElement;
        const { pinnedStartLast, pinnedEndFirst } = args.frozenPinned;
        while ((processedRow = processedRows.pop()) != null) {
            cacheEntry = this._rowsCache[processedRow];
            var columnIdx;
            while ((columnIdx = cacheEntry.cellRenderQueue.pop()) != null) {
                var element = cacheEntry.cellRenderContent.pop();
                node = x.lastElementChild as HTMLElement;
                if (element instanceof Node)
                    node.appendChild(element);

                if (columnIdx <= pinnedStartLast) {
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

    private createRowCellRenderArgs(row: number, cell: number): RowCellRenderArgs<TItem> {
        const rowRendering = cell == null;
        return {
            activeCell: this._activeCell,
            activeRow: this._activeRow,
            cellCssClasses: this._cellCssClasses,
            frozenPinned: this._refs,
            getRowTop: bindThis(this).getRowTop,
            grid: this,
            item: row != null ? this.getDataItem(row) : null,
            row: row,
            colLeft: this._colLeft,
            colRight: this._colRight,
            rtl: this._options.rtl,
            // row only args
            range: null,
            sbCenter: rowRendering ? [] : null,
            sbEnd: rowRendering ? [] : null,
            sbStart: rowRendering ? [] : null,
            // cell only args
            cachedRow: row != null ? this._rowsCache[row] : null,
            cell: cell,
            colMetadata: null,
            colspan: null,
            sb: rowRendering ? null : [],
        };
    }

    private renderRows(range: ViewRange): void {
        const args = this.createRowCellRenderArgs(null, null);
        args.range = range;
        const rows: number[] = [];
        let needToReselectCell = false;
        const { start, main, end, pinnedStartLast, pinnedEndFirst, frozenBottomFirst, frozenTopLast } = this._refs;
        const pinnedStart = pinnedStartLast >= 0;
        const pinnedEnd = pinnedEndFirst != Infinity;

        for (let row = range.top, rangeBottom = range.bottom; row <= rangeBottom; row++) {

            if (this._rowsCache[row] || (frozenBottomFirst != Infinity && row == this.getDataLength())) {
                continue;
            }

            rows.push(row);
            // Create an entry right away so that appendRowHtml() can
            // start populatating it.
            args.cachedRow = this._rowsCache[row] = {
                rowNodeS: null,
                rowNodeC: null,
                rowNodeE: null,
                cellColSpans: [],
                cellNodesByColumnIdx: [],
                cellRenderQueue: [],
                cellRenderContent: []
            };
            args.row = row;
            args.item = this.getDataItem(row);
            renderRow(args);
            if (this._activeCellNode && args.activeRow === row) {
                needToReselectCell = true;
            }
        }

        if (!rows.length) {
            return;
        }

        const s = document.createElement("div"), c = document.createElement("div"), e = document.createElement("div");
        s.innerHTML = args.sbStart.join(""); c.innerHTML = args.sbCenter.join(""); e.innerHTML = args.sbEnd.join("");

        for (let row of rows) {
            const cache = this._rowsCache[row];
            function append(canvas: GridBandRefs["canvas"], rowNode: HTMLElement) {
                canvas[row <= frozenTopLast ? "top" : row >= frozenBottomFirst ? "bottom" : "body"]?.appendChild(rowNode);
            }

            (cache.rowNodeS = s.firstElementChild as HTMLElement) && pinnedStart && append(start.canvas, cache.rowNodeS);
            (cache.rowNodeC = c.firstElementChild as HTMLElement) && append(main.canvas, cache.rowNodeC);
            (cache.rowNodeE = e.firstElementChild as HTMLElement) && pinnedEnd && append(end.canvas, cache.rowNodeE);
            const rowTop = args.getRowTop(row) + "px";
            cache.rowNodeS && (cache.rowNodeS.style.top = rowTop);
            cache.rowNodeC && (cache.rowNodeC.style.top = rowTop);
            cache.rowNodeE && (cache.rowNodeE.style.top = rowTop);
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
            this._hPostRender = setTimeout(bindThis(this).asyncPostProcessRows, this._options.asyncPostRenderDelay);
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
            this._hPostRenderCleanup = setTimeout(bindThis(this).asyncPostProcessCleanupRows, this._options.asyncPostCleanupDelay);
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

    public render(): void {
        if (!this._initialized) { return; }
        if (this._hRender) {
            clearTimeout(this._hRender);
        }
        var visible = this.getVisibleRange();
        var rendered = this.getRenderedRange();

        // remove rows no longer in the viewport
        this.cleanupRows(rendered);

        const { frozenTopLast, frozenBottomFirst } = this._refs;

        // add new rows & missing cells in existing rows
        if (this._scrollLeftRendered != this._scrollLeft) {
            if (frozenTopLast >= 0) {
                const renderedFrozenTop = Object.assign({}, rendered);
                renderedFrozenTop.top = 0;
                renderedFrozenTop.bottom = frozenTopLast;
                this.cleanUpAndRenderCells(renderedFrozenTop);
            }
            const dataLength = this.getDataLength();
            if (frozenBottomFirst != Infinity) {
                const renderedFrozenBottom = Object.assign({}, rendered);
                renderedFrozenBottom.top = frozenBottomFirst;
                renderedFrozenBottom.bottom = dataLength - 1;
                this.cleanUpAndRenderCells(renderedFrozenBottom);
            }

            this.cleanUpAndRenderCells(rendered);
        }

        // render missing rows
        this.renderRows(rendered);

        // render frozen rows, need to check if still required
        frozenTopLast >= 0 && this.renderRows({ top: 0, bottom: frozenTopLast, leftPx: rendered.leftPx, rightPx: rendered.rightPx });
        frozenBottomFirst != Infinity && this.renderRows({ top: frozenBottomFirst, bottom: this.getDataLength() - 1, leftPx: rendered.leftPx, rightPx: rendered.rightPx });

        this._postProcessFromRow = visible.top;
        this._postProcessToRow = Math.min(this.getDataLengthIncludingAddNew() - 1, visible.bottom);
        this.startPostProcessing();

        this._scrollTopRendered = this._scrollTop;
        this._scrollLeftRendered = this._scrollLeft;
        this._lastRenderTime = new Date().getTime();
        this._hRender = null;
    }

    private handleHeaderFooterRowScroll = (e: Event): void => {
        if (this._ignoreScrollUntil >= new Date().getTime())
            return;

        var scrollLeft = (e.target as HTMLElement).scrollLeft;
        if (scrollLeft != this.getScrollContainerX().scrollLeft) {
            this.getScrollContainerX().scrollLeft = scrollLeft;
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

        this._scrollTop = Math.max(0, this.getScrollContainerY().scrollTop - (deltaY * this._options.rowHeight));
        this._scrollLeft = this.getScrollContainerX().scrollLeft + (deltaX * 10);
        if (this._handleScroll(true)) {
            e.preventDefault();
        }
    }

    private handleScroll() {
        this._scrollTop = this.getScrollContainerY().scrollTop;
        this._scrollLeft = this.getScrollContainerX().scrollLeft;
        this._handleScroll();
    }

    private _handleScroll(isMouseWheel?: boolean) {

        var vScrollDist = Math.abs(this._scrollTop - this._scrollTopPrev);
        var hScrollDist = Math.abs(this._scrollLeft - this._scrollLeftPrev);

        if (hScrollDist || vScrollDist)
            this._ignoreScrollUntil = new Date().getTime() + 100;

        if (hScrollDist) {
            const scrollLeft = this._scrollLeftPrev = this._scrollLeft;
            getAllHScrollContainers(this._refs).forEach(sc => sc.scrollLeft = scrollLeft);
        }

        const vpi = this._viewportInfo;

        if (vScrollDist) {
            this._vScrollDir = this._scrollTopPrev < this._scrollTop ? 1 : -1;
            const scrollTop = this._scrollTopPrev = this._scrollTop;

            const scrollContainerY = this.getScrollContainerY();
            if (isMouseWheel === true) {
                scrollContainerY.scrollTop = scrollTop;
            }

            getAllVScrollContainers(this._refs).forEach(sc => sc !== scrollContainerY && (sc.scrollTop = scrollTop));

            // switch virtual pages if needed
            if (vScrollDist < this._viewportInfo.height) {
                this.scrollTo(scrollTop + this._pageOffset);
            } else {
                var oldOffset = this._pageOffset;
                if (vpi.realScrollHeight == vpi.height) {
                    this._page = 0;
                } else {
                    this._page = Math.min(this._numberOfPages - 1, Math.floor(scrollTop * ((vpi.virtualHeight - this._viewportInfo.height) / (vpi.realScrollHeight - this._viewportInfo.height)) * (1 / this._pageHeight)));
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
                    this._hRender = setTimeout(bindThis(this).render, 50);
                }

                this._trigger(this.onViewportChanged);
            }
        }

        this._trigger(this.onScroll, { scrollLeft: this._scrollLeft, scrollTop: this._scrollTop });

        return !!(hScrollDist || vScrollDist);
    }

    private asyncPostProcessRows() {
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
                this._hPostRender = setTimeout(bindThis(this).asyncPostProcessRows, this._options.asyncPostRenderDelay);
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
                this._hPostRenderCleanup = setTimeout(bindThis(this).asyncPostProcessCleanupRows, this._options.asyncPostCleanupDelay);
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

        this._trigger(this.onCellCssStylesChanged, { key: key, hash: hash });
    }

    removeCellCssStyles(key: string): void {
        if (!this._cellCssClasses[key]) {
            return;
        }

        this.updateCellCssStylesOnRenderedRows(null, this._cellCssClasses[key]);
        delete this._cellCssClasses[key];

        this._trigger(this.onCellCssStylesChanged, { key: key, hash: null });
    }

    setCellCssStyles(key: string, hash: CellStylesHash): void {
        var prevHash = this._cellCssClasses[key];

        this._cellCssClasses[key] = hash;
        this.updateCellCssStylesOnRenderedRows(hash, prevHash);

        this._trigger(this.onCellCssStylesChanged, { key: key, hash: hash });
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

    private handleDragInit(e: UIEvent, dd: DragPosition): boolean {
        var cell = this.getCellFromEvent(e);
        if (!cell || !this.cellExists(cell.row, cell.cell)) {
            return false;
        }

        const sge = this._trigger(this.onDragInit as any, dd, e);
        if (sge.isImmediatePropagationStopped()) {
            e.preventDefault();
            return sge.getReturnValue();
        }

        // if nobody claims to be handling drag'n'drop by stopping immediate propagation,
        // cancel out of it
        return false;
    }

    private handleDragStart(e: DragEvent, dd: DragPosition): boolean {
        var cell = this.getCellFromEvent(e);
        if (!cell || !this.cellExists(cell.row, cell.cell)) {
            return false;
        }

        const sge = this._trigger(this.onDragStart as any, dd, e);
        if (sge.isImmediatePropagationStopped()) {
            return true
        }

        return false;
    }

    private handleDrag(e: DragEvent, dd: DragPosition): any {
        const sge = this._trigger(this.onDrag as any, dd, e);
        return sge.getReturnValue();
    }

    private handleDragEnd(e: DragEvent, dd: DragPosition): void {
        this._trigger(this.onDragEnd as any, dd, e);
    }

    private handleKeyDown(e: KeyboardEvent): void {
        const sge = this._trigger(this.onKeyDown, { row: this._activeRow, cell: this._activeCell }, e);
        var handled = sge.isImmediatePropagationStopped();

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
                (e as any).originalEvent && ((e as any).originalEvent.keyCode = 0); // prevent default behaviour for special keys in IE browsers (F3, F5, etc.)
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

        const sge = this._trigger(this.onClick, { row: cell.row, cell: cell.cell }, e);
        if (sge.isImmediatePropagationStopped()) {
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

        this._trigger(this.onContextMenu, {}, e);
    }

    private handleDblClick(e: MouseEvent): void {
        var cell = this.getCellFromEvent(e as any);
        if (!cell || (this._currentEditor != null && this._activeRow == cell.row && this._activeCell == cell.cell)) {
            return;
        }

        const sge = this._trigger(this.onDblClick, { row: cell.row, cell: cell.cell }, e);
        if (sge.isImmediatePropagationStopped()) {
            return;
        }

        if (this._options.editable) {
            this.gotoCell(cell.row, cell.cell, true);
        }
    }

    private handleHeaderMouseEnter(e: Event): void {
        const column = this.getColumnFromNode(e.target as HTMLElement)
        column && this._trigger(this.onHeaderMouseEnter, { column }, e);
    }

    private handleHeaderMouseLeave(e: Event): void {
        const column = this.getColumnFromNode(e.target as HTMLElement)
        column && this._trigger(this.onHeaderMouseLeave, { column }, e);
    }

    private handleHeaderContextMenu(e: MouseEvent): void {
        var header = (e.target as Element)?.closest?.(".slick-header-column");
        var column = this.getColumnFromNode(header);
        column && this._trigger(this.onHeaderContextMenu, { column }, e);
    }

    private handleHeaderClick(e: MouseEvent): void {
        var header = (e.target as Element)?.closest?.(".slick-header-column");
        var column = this.getColumnFromNode(header);
        column && this._trigger(this.onHeaderClick, { column: column }, e);
    }

    private handleMouseEnter(e: Event): void {
        this._trigger(this.onMouseEnter, {}, e);
    }

    private handleMouseLeave(e: Event): void {
        this._trigger(this.onMouseLeave, {}, e);
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
            const srow = (rowNode as HTMLElement).dataset?.row;
            if (srow != null)
                return parseInt(srow, 10);
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

        row = this.getRowFromNode(cellEl.parentElement);
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

        const cols = this._cols, { pinnedStartLast, pinnedEndFirst } = this._refs;
        var y1 = this.getRowTop(row);
        var y2 = y1 + this._options.rowHeight - 1;
        var x1 = 0;
        for (var i = 0; i < cell; i++) {
            x1 += cols[i].width;
            if (i === pinnedStartLast + 1 || i === pinnedEndFirst) {
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
        const { pinnedStartLast, pinnedEndFirst } = this._refs;

        if (cell <= pinnedStartLast || cell >= pinnedEndFirst)
            return;

        var colspan = this.getColspan(row, cell);
        this.internalScrollColumnIntoView(this._colLeft[cell], this._colRight[cell + (colspan > 1 ? colspan - 1 : 0)]);
    }

    scrollColumnIntoView(cell: number): void {
        this.internalScrollColumnIntoView(this._colLeft[cell], this._colRight[cell]);
    }

    private internalScrollColumnIntoView(left: number, right: number): void {

        var scrollRight = this._scrollLeft + parsePx(getComputedStyle(this.getScrollContainerX()).width) -
            (this._viewportInfo.hasVScroll ? this._scrollDims.width : 0);

        var target;
        if (left < this._scrollLeft)
            target = left;
        else if (right > scrollRight)
            target = Math.min(left, right - this.getScrollContainerX().clientWidth);
        else
            return;

        this.getScrollContainerX().scrollLeft = target;
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
            var isBottom = this._activeCellNode.closest('.grid-canvas.sg-bottom') != null;
            const { frozenBottomFirst } = this._refs;
            if (frozenBottomFirst != Infinity && isBottom) {
                rowOffset -= (this._options.frozenBottom)
                    ? Math.round(parsePx(getComputedStyle(this.getCanvasNode(0, 0)).height))
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
            this._trigger(this.onActiveCellChanged, this.getActiveCell() as ArgsCell);
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
        this._trigger(this.onBeforeCellEditorDestroy, { editor: this._currentEditor });
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

        const sge = this._trigger(this.onBeforeEditCell, { row: this._activeRow, cell: this._activeCell, item: item, column: columnDef });
        if (sge.isDefaultPrevented() ||
            sge.getReturnValue() === false) {
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

        const boundThis = bindThis(this);
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
            commitChanges: boundThis.commitEditAndSetFocus,
            cancelChanges: boundThis.cancelEditAndSetFocus
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

        this._trigger(this.onActiveCellPositionChanged, {});

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

        const { frozenTopLast } = this._refs;
        if (!this.isFrozenRow(row)) {

            var viewportScrollH = Math.round(parsePx(getComputedStyle(this.getScrollContainerY()).height));

            const rowNumber = frozenTopLast >= 0 ? (row - frozenTopLast + 1) : row;

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

    private getColumnCount(): number {
        return this._cols.length;
    }

    private isRTL(): boolean {
        return this._options.rtl;
    }

    private setTabbingDirection(dir: number): void {
        this._tabbingDirection = dir;
    }

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
            const boundThis = bindThis(this);
            this._cellNavigator = new CellNavigator({
                getColumnCount: boundThis.getColumnCount,
                getRowCount: boundThis.getDataLengthIncludingAddNew,
                getColspan: boundThis.getColspan,
                canCellBeActive: boundThis.canCellBeActive,
                setTabbingDirection: boundThis.setTabbingDirection,
                isRTL: boundThis.isRTL
            });
        }

        var pos = this._cellNavigator.navigate(dir, this._activeRow, this._activeCell, this._activePosX);
        if (pos) {
            const { frozenBottomFirst } = this._refs;
            if (frozenBottomFirst != Infinity && pos.row == this.getDataLength()) {
                return;
            }

            var isAddNewRow = (pos.row == this.getDataLength());

            if (!this.isFrozenRow(pos.row)) {
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
                                self._trigger(self.onCellChange, {
                                    row: this.activeRow,
                                    cell: self._activeCell,
                                    item: item
                                });
                            },
                            undo: function () {
                                this.editor.applyValue(item, this.prevSerializedValue);
                                self.updateRow(this.row);
                                self._trigger(self.onCellChange, {
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
                        this._trigger(this.onAddNewRow, { item: newItem, column: column });
                    }

                    // check whether the lock has been re-acquired by event handlers
                    return !this.getEditorLock().isActive();
                } else {
                    // Re-add the CSS class to trigger transitions, if any.
                    this._activeCellNode.classList.remove("invalid");
                    this._activeCellNode.offsetWidth;  // force layout
                    this._activeCellNode.classList.add("invalid");

                    this._trigger(this.onValidationError, {
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

export { SleekGrid as Grid };

