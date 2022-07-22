/**
 * @license
 * (c) 2017-2022 Serenity.is 
 * Ported to TypeScript
 * 
 * (c) 2009-2013 Michael Leibman
 * michael{dot}leibman{at}gmail{dot}com
 * http://github.com/mleibman/slickgrid
 *
 * Distributed under MIT license.
 * All rights reserved.
 *
 * SlickGrid v2.2
 *
 * NOTES:
 *     Cell/row DOM manipulations are done directly bypassing jQuery's DOM manipulation methods.
 *     This increases the speed dramatically, but can only be done safely because there are no event handlers
 *     or data associated with any cell/row DOM nodes.  Cell editors must make sure they implement .destroy()
 *     and do proper cleanup.
 */

// make sure required JavaScript modules are loaded
if (typeof jQuery === "undefined") {
    throw "SlickGrid requires jquery module to be loaded";
}
if (!(jQuery.fn as any).drag) {
    throw "SlickGrid requires jquery.event.drag module to be loaded";
}

//@ts-ignore
if (typeof Slick === "undefined") {
    throw "slick.core.js not loaded";
}

namespace Slick {

    export type ColumnFormatter<TItem = any> = (row: number, cell: number, value: any, column: Column<TItem>, item: TItem, grid?: Grid<TItem>, colMeta?: ColumnMetadata) => string;
    export type ColumnFormat<TItem = any> = (ctx: Slick.FormatterContext<TItem>) => string;
    export type AsyncPostRender<TItem = any> = (cellNode: HTMLElement, row: number, item: TItem, column: Column<TItem>) => void;
    export type AsyncPostCleanup<TItem = any> = (cellNode: HTMLElement, row?: number, column?: Column<TItem>) => void;

    export interface FormatterContext<TItem = any> {
        row?: number;
        cell?: number;
        value?: any;
        column?: Column<TItem>;
        item?: TItem;
    }

    export interface Plugin {
        init(grid: Grid): void;
        destroy?: () => void;
    }

    export interface SelectionModel {
        init(grid: Grid): void;
        destroy?: () => void;
        setSelectedRanges(ranges: Range[]): void;
        onSelectedRangesChanged: Event<Range[]>;
    }

    export interface ColumnSort {
        columnId: string;
        sortAsc?: boolean;
    }
    
    export interface RowCell {
        row: number;
        cell: number;
    }
        
    export interface Position {
        bottom?: number;
        height?: number;
        left?: number;
        right?: number;
        top?: number;
        visible?: boolean;
        width?: number;
    }
           
    export interface ViewRange {
        top?: number;
        bottom?: number;
        leftPx?: number;
        rightPx?: number;
    }

    export interface EditorOptions {
        grid: Grid;
        gridPosition?: Position;
        position?: Position;
        container?: HTMLElement;
        column?: Column;
        item?: any;
        commitChanges?: () => void,
        cancelChanges?: () => void
    }

    export interface ValidationResult {
        valid: boolean;
        msg?: string;
    }

    export interface Editor {
        new (options: EditorOptions) : Editor;
        destroy(): void;
        applyValue(item: any, value: any): void;
        focus(): void;
        isValueChanged(): boolean;
        loadValue(value: any): void;
        serializeValue(): any;
        position?(pos: Position): void;
        hide?(): void;
        show?(): void;
        validate?(): ValidationResult;
    }
    
    export interface EditorFactory {
        getEditor(column: Column): Editor;
    }

    export interface EditCommand {
        row: number;
        cell: number;
        editor: Editor;
        serializedValue: any;
        prevSerializedValue: any;
        execute: () => void;
        undo: () => void;
    }

    export interface FormatterFactory<TItem = any> {
        getFormatter(column: Column<TItem>): ColumnFormatter<TItem>;
    }

    export interface ColumnMetadata<TItem = any> {
        colspan: number | '*';
        formatter?: ColumnFormatter<TItem>;
    }

    export interface ItemMetadata<TItem = any> {
        columns?: { [key: string]: ColumnMetadata<TItem> };
        formatter?: ColumnFormatter<TItem>;
    }

    export interface ArgsGrid {
        grid?: Grid;
    }

    export interface ArgsColumn extends ArgsGrid {
        column: Column;
    }

    export interface ArgsColumnNode extends ArgsColumn {
        node: HTMLElement;
    }

    export type ArgsSortCol = {
        sortCol: Column;
        sortAsc: boolean;
    }

    export interface ArgsSort extends ArgsGrid {
        multiColumnSort: boolean;
        sortAsc?: boolean;
        sortCol?: Column;
        sortCols?: ArgsSortCol[];
    }

    export interface ArgsRowNumbers extends ArgsGrid {
        rows: number[];
    }

    export interface ArgsScroll extends ArgsGrid {
        scrollLeft: number;
        scrollTop: number;
    }

    export interface ArgsCssStyle extends ArgsGrid {
        key: string;
        hash: CellStylesHash;
    }

    export interface ArgsCell extends ArgsGrid {
        row: number;
        cell: number;
    }

    export interface ArgsCellChange extends ArgsCell {
        item: any;
    }

    export interface ArgsCellEdit extends ArgsCellChange {
        column: Column;
    }

    export interface ArgsAddNewRow extends ArgsColumn {
        item: any;
    }

    export interface ArgsEditorDestroy extends ArgsGrid {
        editor: Editor;
    }

    export interface ArgsValidationError extends ArgsCell {
        editor: Editor,
        column: Column;
        cellNode: HTMLElement;
        validationResults: ValidationResult;
    }

    export type CellStylesHash = { [row: number]: { [cell: number]: string } }

    export interface Column<TItem = any> {
        asyncPostRender?: AsyncPostRender<TItem>;
        asyncPostCleanup?: AsyncPostCleanup<TItem>;
        behavior?: any;
        cannotTriggerInsert?: boolean;
        cssClass?: string;
        defaultSortAsc?: boolean;
        editor?: Editor;
        field: string;
        focusable?: boolean;
        footerCssClass?: string;
        format?: ColumnFormat<TItem>;
        formatter?: ColumnFormatter<TItem>;
        fixedTo?: "start";
        groupTotalsFormatter?: (p1?: GroupTotals<TItem>, p2?: Column<TItem>, grid?: Grid<TItem>) => string;
        headerCssClass?: string;
        id?: string;
        maxWidth?: any;
        minWidth?: number;
        name?: string;
        previousWidth?: number;
        referencedFields?: string[];
        rerenderOnResize?: boolean;
        resizable?: boolean;
        selectable?: boolean;
        sortable?: boolean;
        sortOrder?: number;
        toolTip?: string;
        validator?: (value: any) => ValidationResult;
        visible?: boolean;
        width?: number;
    }

    export interface GridOptions<TItem = any> {
        addNewRowCssClass?: string;
        asyncEditorLoading?: boolean;
        asyncEditorLoadDelay?: number;
        asyncPostRenderDelay?: number;
        asyncPostCleanupDelay?: number;
        autoEdit?: boolean;
        autoHeight?: boolean;
        cellFlashingCssClass?: string;
        cellHighlightCssClass?: string;
        columns?: Column<TItem>[];
        dataItemColumnValueExtractor?: (item: TItem, column: Column<TItem>) => void;
        groupingPanel?: boolean,
        groupingPanelHeight?: number;
        setGroupingPanelVisibility?: (value: boolean) => void;
        defaultColumnWidth?: number;
        defaultFormatter?: ColumnFormatter<TItem>;
        editable?: boolean;
        editCommandHandler?: (item: TItem, column: Column<TItem>, command: EditCommand) => void;
        editorFactory?: EditorFactory;
        editorLock?: EditorLock;
        enableAddRow?: boolean;
        enableAsyncPostCleanup?: boolean;
        enableAsyncPostRender?: boolean;
        enableCellRangeSelection?: boolean;
        enableCellNavigation?: boolean;
        enableColumnReorder?: boolean;
        enableRowReordering?: boolean;
        enableTextSelectionOnCells?: boolean;
        explicitInitialization?: boolean;
        footerRowHeight?: number;
        forceFitColumns?: boolean;
        forceSyncScrolling?: boolean;
        formatterFactory?: FormatterFactory;
        fullWidthRows?: boolean;
        frozenColumn?: number;
        frozenRow?: number;
        frozenBottom?: boolean;
        headerRowHeight?: number;
        leaveSpaceForNewRows?: boolean;
        minBuffer?: number;
        multiColumnSort?: boolean;
        multiSelect?: boolean;
        renderAllCells?: boolean;
        rowHeight?: number;
        selectedCellCssClass?: string;
        showGroupingPanel?: boolean;
        showHeaderRow?: boolean;
        showFooterRow?: boolean;
        showTopPanel?: boolean;
        syncColumnCellResize?: boolean;
        topPanelHeight?: number;
    }

    //////////////////////////////////////////////////////////////////////////////////////////////
    // SlickGrid class implementation (available as Slick.Grid)

    /**
     * Creates a new instance of the grid.
     * @class Grid
     * @constructor
     * @param {Node}              container   Container node to create the grid in.
     * @param {Array,Object}      data        An array of objects for databinding.
     * @param {Array}             columns     An array of column definitions.
     * @param {Object}            options     Grid options.
     **/
    export class Grid<TItem = any> {

        private _columns: Column<TItem>[];
        private _columnById: { [key: string]: number };
        private _colDefaults: Partial<Column>;
        private _data: any;
        private _viewCols: Column<TItem>[];
        private _viewColById: { [key: string]: number };
        private _viewColLeft: number[] = [];
        private _viewColRight: number[] = [];
        private _fixedStartCols: number;
        private _options: GridOptions<TItem>;

        // scroller
        private th: number;   // virtual height
        private h: number;    // real scrollable height
        private ph: number;   // page height
        private n: number;    // number of pages
        private cj: number;   // "jumpiness" coefficient

        private page: number = 0;       // current page
        private offset: number = 0;     // current page offset
        private vScrollDir: number = 1;

        // private
        private initialized = false;
        private $container: JQuery;
        private uid: string = "slickgrid_" + Math.round(1000000 * Math.random());
        private $focusSink: JQuery;
        private $focusSink2: JQuery;
        private $headerScroller: JQuery;
        private $headers: JQuery;
        private $headerRow: JQuery;
        private $headerRowScroller: JQuery;
        private $headerRowSpacerL: JQuery;
        private $headerRowSpacerR: JQuery;
        private $footerRow: JQuery;
        private $footerRowScroller: JQuery;
        private $footerRowSpacerL: JQuery;
        private $footerRowSpacerR: JQuery;
        private $groupingPanel: JQuery;
        private $topPanelScroller: JQuery;
        private $topPanel: JQuery;
        private $viewport: JQuery;
        private $canvas: JQuery;
        private $style: JQuery;
        private $boundAncestors: JQuery;
        private stylesheet: any;
        private columnCssRulesL: any;
        private columnCssRulesR: any;
        private viewportH: number;
        private viewportW: number;
        private canvasWidth: number;
        private canvasWidthL: number;
        private canvasWidthR: number;
        private headersWidthL: number;
        private headersWidthR: number;
        private viewportHasHScroll: boolean;
        private viewportHasVScroll: boolean;
        private headerColumnWidthDiff: number = 0;
        private cellWidthDiff: number = 0;
        private cellHeightDiff: number = 0;
        private jQueryNewWidthBehaviour: boolean = false;
        private absoluteColumnMinWidth: number;
        private hasFrozenRows = false;
        private frozenRowsHeight: number = 0;
        private actualFrozenRow: number = -1;
        private paneTopH: number = 0;
        private paneBottomH: number = 0;
        private viewportTopH: number = 0;
        private topPanelH: number = 0;
        private groupPanelH: number = 0;
        private headerH: number = 0;
        private headerRowH: number = 0;
        private footerRowH: number = 0;

        private tabbingDirection: number = 1;
        private activePosX: number;
        private activeRow: number;
        private activeCell: number;
        private activeCellNode: HTMLElement = null;
        private currentEditor: Editor = null;
        private serializedEditorValue: any;
        private editController: EditController;

        private rowsCache: { [key: number]: CachedRow } = {};
        private numVisibleRows: number = 0;
        private prevScrollTop: number = 0;
        private scrollTop: number = 0;
        private lastRenderedScrollTop: number = 0;
        private lastRenderedScrollLeft: number = 0;
        private prevScrollLeft: number = 0;
        private scrollLeft: number = 0;

        private selectionModel: SelectionModel;
        private selectedRows: number[] = [];

        private plugins: Plugin[] = [];
        private cellCssClasses: CellStylesHash = {};

        private sortColumns: ColumnSort[] = [];

        private rtl = false;
        private xLeft = 'left';
        private xRight = 'right';


        // async call handles
        private h_editorLoader: any = null;
        private h_render: any = null;
        private h_postrender: any = null;
        private h_postrenderCleanup: any = null;
        private postProcessedRows: any = {};
        private postProcessToRow: any = null;
        private postProcessFromRow: any = null;
        private postProcessedCleanupQueue: PostProcessCleanupEntry[] = [];
        private postProcessgroupId: number = 0;

        // store css attributes if display:none is active in container or parent
        private $paneHeaderL: JQuery;
        private $paneHeaderR: JQuery;
        private $paneTopL: JQuery;
        private $paneTopR: JQuery;
        private $paneBottomL: JQuery;
        private $paneBottomR: JQuery;

        private $headerScrollerL: JQuery;
        private $headerScrollerR: JQuery;

        private $headerL: JQuery;
        private $headerR: JQuery;

        private $headerRowScrollerL: JQuery;
        private $headerRowScrollerR: JQuery;

        private $footerRowScrollerL: JQuery;
        private $footerRowScrollerR: JQuery;

        private $headerRowL: JQuery;
        private $headerRowR: JQuery;

        private $footerRowL: JQuery;
        private $footerRowR: JQuery;

        private $topPanelScrollerL: JQuery;
        private $topPanelScrollerR: JQuery;

        private $topPanelL: JQuery;
        private $topPanelR: JQuery;

        private $viewportTopL: JQuery;
        private $viewportTopR: JQuery;
        private $viewportBottomL: JQuery;
        private $viewportBottomR: JQuery;

        private $canvasTopL: JQuery;
        private $canvasTopR: JQuery;
        private $canvasBottomL: JQuery;
        private $canvasBottomR: JQuery;

        private $viewportScrollContainerX: JQuery;
        private $viewportScrollContainerY: JQuery;
        private $headerScrollContainer: JQuery;
        private $headerRowScrollContainer: JQuery;
        private $footerRowScrollContainer: JQuery;

        readonly onScroll = new Event<ArgsScroll>();
        readonly onSort = new Event<ArgsSort>();
        readonly onHeaderMouseEnter = new Event<ArgsColumn, JQueryMouseEventObject>();
        readonly onHeaderMouseLeave = new Event<ArgsColumn>();
        readonly onHeaderContextMenu = new Event<ArgsColumn>();
        readonly onHeaderClick = new Event<ArgsColumn>();
        readonly onHeaderCellRendered = new Event<ArgsColumnNode>();
        readonly onBeforeHeaderCellDestroy = new Event<ArgsColumnNode>();
        readonly onHeaderRowCellRendered = new Event<ArgsColumnNode>();
        readonly onFooterRowCellRendered = new Event<ArgsColumnNode>();
        readonly onBeforeHeaderRowCellDestroy = new Event<ArgsColumnNode>();
        readonly onBeforeFooterRowCellDestroy = new Event<ArgsColumnNode>();
        readonly onMouseEnter = new Event<ArgsGrid, JQueryMouseEventObject>();
        readonly onMouseLeave = new Event<ArgsGrid, JQueryMouseEventObject>();
        readonly onClick = new Event<ArgsCell, JQueryMouseEventObject>();
        readonly onDblClick = new Event<ArgsCell, JQueryMouseEventObject>();
        readonly onContextMenu = new Event<ArgsGrid, JQueryEventObject>();
        readonly onKeyDown = new Event<ArgsCell, JQueryKeyEventObject>();
        readonly onAddNewRow = new Event<ArgsAddNewRow>();
        readonly onValidationError = new Event<ArgsValidationError>();
        readonly onViewportChanged = new Event<ArgsGrid>();
        readonly onColumnsReordered = new Event<ArgsGrid>();
        readonly onColumnsResized = new Event<ArgsGrid>();
        readonly onCellChange = new Event<ArgsCellChange>();
        readonly onBeforeEditCell = new Event<ArgsCellEdit>();
        readonly onBeforeCellEditorDestroy = new Event<ArgsEditorDestroy>();
        readonly onBeforeDestroy = new Event<ArgsGrid>();
        readonly onActiveCellChanged = new Event<ArgsGrid>();
        readonly onActiveCellPositionChanged = new Event<ArgsGrid>();
        readonly onDragInit = new Event<ArgsGrid, JQueryEventObject>();
        readonly onDragStart = new Event<ArgsGrid, JQueryEventObject>();
        readonly onDrag = new Event<ArgsGrid, JQueryEventObject>();
        readonly onDragEnd = new Event<ArgsGrid, JQueryEventObject>();
        readonly onSelectedRowsChanged = new Event<ArgsRowNumbers>();
        readonly onCellCssStylesChanged = new Event<ArgsCssStyle>();

        constructor(container: JQuery, data: any, columns: Column<TItem>[], options: GridOptions<TItem>) {

            this._data = data;

            // settings
            var defaults: Slick.GridOptions<TItem> = {
                explicitInitialization: false,
                rowHeight: 25,
                defaultColumnWidth: 80,
                enableAddRow: false,
                leaveSpaceForNewRows: false,
                editable: false,
                autoEdit: true,
                enableCellNavigation: true,
                enableColumnReorder: true,
                asyncEditorLoading: false,
                asyncEditorLoadDelay: 100,
                forceFitColumns: false,
                enableAsyncPostRender: false,
                enableAsyncPostCleanup: false,
                asyncPostRenderDelay: 50,
                asyncPostCleanupDelay: 40,
                autoHeight: false,
                editorLock: Slick.GlobalEditorLock,
                showHeaderRow: false,
                headerRowHeight: 25,
                showFooterRow: false,
                footerRowHeight: 25,
                showTopPanel: false,
                topPanelHeight: 25,
                groupingPanel: false,
                showGroupingPanel: true,
                groupingPanelHeight: 34,
                formatterFactory: null,
                editorFactory: null,
                cellFlashingCssClass: "flashing",
                selectedCellCssClass: "selected",
                multiSelect: true,
                enableTextSelectionOnCells: false,
                dataItemColumnValueExtractor: null,
                frozenBottom: false,
                frozenRow: -1,
                fullWidthRows: false,
                multiColumnSort: false,
                defaultFormatter: defaultFormatter,
                forceSyncScrolling: false,
                addNewRowCssClass: "new-row",
                minBuffer: 3,
                renderAllCells: false
            };

            this._colDefaults = {
                name: "",
                resizable: true,
                sortable: false,
                minWidth: 30,
                rerenderOnResize: false,
                headerCssClass: null,
                footerCssClass: null,
                defaultSortAsc: true,
                focusable: true,
                selectable: true
            };

            //////////////////////////////////////////////////////////////////////////////////////////////
            // Initialization

            this.$container = $(container);
            if (this.$container.length < 1) {
                throw new Error("SlickGrid requires a valid container, " + container + " does not exist in the DOM.");
            }

            this.rtl = $(document.body).hasClass('rtl') || this.$container.css('direction') == 'rtl';
            if (this.rtl) {
                this.xLeft = 'right';
                this.xRight = 'left';
            }

            // calculate these only once and share between grid instances
            maxSupportedCssHeight = maxSupportedCssHeight || getMaxSupportedCssHeight();
            scrollbarDimensions = scrollbarDimensions || this.measureScrollbar();

            options = $.extend({}, defaults, options);
            this._options = options;
            this.validateAndEnforceOptions();
            this._colDefaults.width = options.defaultColumnWidth;
            
            adjustFrozenColumnCompat(columns, this._options);
            this.setColumnsInternal(columns);

            // validate loaded JavaScript modules against requested options
            if (options.enableColumnReorder && !($.fn as any).sortable) {
                throw new Error("SlickGrid's 'enableColumnReorder = true' option requires jquery-ui.sortable module to be loaded");
            }

            this.editController = {
                "commitCurrentEdit": this.commitCurrentEdit,
                "cancelCurrentEdit": this.cancelCurrentEdit
            };

            this.$container
                .empty()
                .css("overflow", "hidden")
                .css("outline", 0)
                .addClass(this.uid)
                .addClass("ui-widget");

            // set up a positioning container if needed
            if (!/relative|absolute|fixed/.test(this.$container.css("position"))) {
                this.$container.css("position", "relative");
            }

            this.$focusSink = $("<div tabIndex='0' hideFocus style='position:fixed;width:0;height:0;top:0;" + this.xLeft + ":0;outline:0;'></div>").appendTo(this.$container);

            if (options.groupingPanel) {
                this.$groupingPanel = $("<div class='slick-grouping-panel' style='overflow:hidden; position:relative;' />")
                    .appendTo(this.$container);

                if (!options.showGroupingPanel) {
                    this.$groupingPanel.hide();
                }
            }

            // Containers used for scrolling frozen columns and rows
            this.$paneHeaderL = $("<div class='slick-pane slick-pane-header slick-pane-" + this.xLeft + "' tabIndex='0' />").appendTo(this.$container);
            this.$paneHeaderR = $("<div class='slick-pane slick-pane-header slick-pane-" + this.xRight + "' tabIndex='0' />").appendTo(this.$container);
            this.$paneTopL = $("<div class='slick-pane slick-pane-top slick-pane-" + this.xLeft + "' tabIndex='0' />").appendTo(this.$container);
            this.$paneTopR = $("<div class='slick-pane slick-pane-top slick-pane-" + this.xRight + "' tabIndex='0' />").appendTo(this.$container);
            this.$paneBottomL = $("<div class='slick-pane slick-pane-bottom slick-pane-" + this.xLeft + "' tabIndex='0' />").appendTo(this.$container);
            this.$paneBottomR = $("<div class='slick-pane slick-pane-bottom slick-pane-" + this.xRight + "' tabIndex='0' />").appendTo(this.$container);

            // Append the header scroller containers
            this.$headerScrollerL = $("<div class='ui-state-default slick-header slick-header-" + this.xLeft + "' />").appendTo(this.$paneHeaderL);
            this.$headerScrollerR = $("<div class='ui-state-default slick-header slick-header-" + this.xRight + "' />").appendTo(this.$paneHeaderR);

            // Cache the header scroller containers
            this.$headerScroller = $().add(this.$headerScrollerL).add(this.$headerScrollerR);

            // Append the columnn containers to the headers
            this.$headerL = $("<div class='slick-header-columns slick-header-columns-" + this.xLeft + "' style='" + this.xLeft + ":-1000px' />").appendTo(this.$headerScrollerL);
            this.$headerR = $("<div class='slick-header-columns slick-header-columns-" + this.xRight + "' style='" + this.xLeft + ":-1000px' />").appendTo(this.$headerScrollerR);

            // Cache the header columns
            this.$headers = $().add(this.$headerL).add(this.$headerR);

            this.$headerRowScrollerL = $("<div class='ui-state-default slick-headerrow' />").appendTo(this.$paneTopL);
            this.$headerRowScrollerR = $("<div class='ui-state-default slick-headerrow' />").appendTo(this.$paneTopR);

            this.$headerRowScroller = $().add(this.$headerRowScrollerL).add(this.$headerRowScrollerR);

            this.$headerRowSpacerL = $("<div style='display:block;height:1px;position:absolute;top:0;" + this.xLeft + ":0;'></div>")
                .css("width", this.getCanvasWidth() + scrollbarDimensions.width + "px")
                .appendTo(this.$headerRowScrollerL);
            this.$headerRowSpacerR = $("<div style='display:block;height:1px;position:absolute;top:0;" + this.xLeft + ":0;'></div>")
                .css("width", this.getCanvasWidth() + scrollbarDimensions.width + "px")
                .appendTo(this.$headerRowScrollerR);


            this.$headerRowL = $("<div class='slick-headerrow-columns slick-headerrow-columns-" + this.xLeft + "' />").appendTo(this.$headerRowScrollerL);
            this.$headerRowR = $("<div class='slick-headerrow-columns slick-headerrow-columns-" + this.xRight + "' />").appendTo(this.$headerRowScrollerR);

            this.$headerRow = $().add(this.$headerRowL).add(this.$headerRowR);

            // Append the top panel scroller
            this.$topPanelScrollerL = $("<div class='ui-state-default slick-top-panel-scroller' />").appendTo(this.$paneTopL);
            this.$topPanelScrollerR = $("<div class='ui-state-default slick-top-panel-scroller' />").appendTo(this.$paneTopR);

            this.$topPanelScroller = $().add(this.$topPanelScrollerL).add(this.$topPanelScrollerR);

            // Append the top panel
            this.$topPanelL = $("<div class='slick-top-panel' style='width:10000px' />").appendTo(this.$topPanelScrollerL);
            this.$topPanelR = $("<div class='slick-top-panel' style='width:10000px' />").appendTo(this.$topPanelScrollerR);

            this.$topPanel = $().add(this.$topPanelL).add(this.$topPanelR);

            if (!options.showTopPanel) {
                this.$topPanelScroller.hide();
            }

            if (!options.showHeaderRow) {
                this.$headerRowScroller.hide();
            }

            // Append the viewport containers
            this.$viewportTopL = $("<div class='slick-viewport slick-viewport-top slick-viewport-" + this.xLeft + "' tabIndex='0' hideFocus />").appendTo(this.$paneTopL);
            this.$viewportTopR = $("<div class='slick-viewport slick-viewport-top slick-viewport-" + this.xRight + "' tabIndex='0' hideFocus />").appendTo(this.$paneTopR);
            this.$viewportBottomL = $("<div class='slick-viewport slick-viewport-bottom slick-viewport-" + this.xLeft + "' tabIndex='0' hideFocus />").appendTo(this.$paneBottomL);
            this.$viewportBottomR = $("<div class='slick-viewport slick-viewport-bottom slick-viewport-" + this.xRight + "' tabIndex='0' hideFocus />").appendTo(this.$paneBottomR);

            // Cache the viewports
            this.$viewport = $().add(this.$viewportTopL).add(this.$viewportTopR).add(this.$viewportBottomL).add(this.$viewportBottomR);

            // Append the canvas containers
            this.$canvasTopL = $("<div class='grid-canvas grid-canvas-top grid-canvas-" + this.xLeft + "' tabIndex='0' hideFocus />").appendTo(this.$viewportTopL);
            this.$canvasTopR = $("<div class='grid-canvas grid-canvas-top grid-canvas-" + this.xRight + "' tabIndex='0' hideFocus />").appendTo(this.$viewportTopR);
            this.$canvasBottomL = $("<div class='grid-canvas grid-canvas-bottom grid-canvas-" + this.xLeft + "' tabIndex='0' hideFocus />").appendTo(this.$viewportBottomL);
            this.$canvasBottomR = $("<div class='grid-canvas grid-canvas-bottom grid-canvas-" + this.xRight + "' tabIndex='0' hideFocus />").appendTo(this.$viewportBottomR);

            // Cache the canvases
            this.$canvas = $().add(this.$canvasTopL).add(this.$canvasTopR).add(this.$canvasBottomL).add(this.$canvasBottomR);

            // footer Row
            this.$footerRowScrollerR = $("<div class='ui-state-default slick-footerrow' />").appendTo(this.$paneTopR);
            this.$footerRowScrollerL = $("<div class='ui-state-default slick-footerrow' />").appendTo(this.$paneTopL);

            this.$footerRowScroller = $().add(this.$footerRowScrollerL).add(this.$footerRowScrollerR);

            this.$footerRowSpacerL = $("<div style='display:block;height:1px;position:absolute;top:0;" + this.xLeft + ":0;'></div>")
                .css("width", this.getCanvasWidth() + scrollbarDimensions.width + "px")
                .appendTo(this.$footerRowScrollerL);
            this.$footerRowSpacerR = $("<div style='display:block;height:1px;position:absolute;top:0;" + this.xLeft + ":0;'></div>")
                .css("width", this.getCanvasWidth() + scrollbarDimensions.width + "px")
                .appendTo(this.$footerRowScrollerR);


            this.$footerRowL = $("<div class='slick-footerrow-columns slick-footerrow-columns-" + this.xLeft + "' />").appendTo(this.$footerRowScrollerL);
            this.$footerRowR = $("<div class='slick-footerrow-columns slick-footerrow-columns-" + this.xRight + "' />").appendTo(this.$footerRowScrollerR);

            this.$footerRow = $().add(this.$footerRowL).add(this.$footerRowR);

            if (!options.showFooterRow) {
                this.$footerRowScroller.hide();
            }

            this.$focusSink2 = this.$focusSink.clone().appendTo(this.$container);

            if (!options.explicitInitialization) {
                this.init();
            }

            this.bindToData();
        }

        init(): void {
            if (this.initialized)
                return;

            this.initialized = true;

            this.calcViewportWidth();
            this.calcViewportHeight();

            // header columns and cells may have different padding/border skewing width calculations (box-sizing, hello?)
            // calculate the diff so we can set consistent sizes
            this.measureCellPaddingAndBorder();

            // for usability reasons, all text selection in SlickGrid is disabled
            // with the exception of input and textarea elements (selection must
            // be enabled there so that editors work as expected); note that
            // selection in grid cells (grid body) is already unavailable in
            // all browsers except IE
            disableSelection(this.$headers); // disable all text selection in header (including input and textarea)

            if (!this._options.enableTextSelectionOnCells) {
                // disable text selection in grid cells except in input and textarea elements
                // (this is IE-specific, because selectstart event will only fire in IE)
                this.$viewport.on("selectstart.ui", function () {
                    return $(this).is("input,textarea");
                });
            }

            this.adjustFrozenRowOption();
            this.setPaneVisibility();
            this.setScroller();
            this.setOverflow();

            this.updateViewColLeftRight();
            this.createColumnHeaders();
            this.createColumnFooter();
            this.setupColumnSort();
            this.createCssRules();
            this.resizeCanvas();
            this.bindAncestorScrollEvents();

            this.$container
                .on("resize.slickgrid", this.resizeCanvas.bind(this));
            this.$viewport
                .on("scroll", this.handleScroll.bind(this));

            if ((jQuery.fn as any).mousewheel && (this.hasFixedColumns() || this.hasFrozenRows)) {
                this.$viewport
                    .on("mousewheel", this.handleMouseWheel.bind(this));
            }

            this.$headerScroller
                .on("contextmenu", this.handleHeaderContextMenu.bind(this))
                .on("click", this.handleHeaderClick.bind(this))
                .delegate(".slick-header-column", "mouseenter", this.handleHeaderMouseEnter.bind(this))
                .delegate(".slick-header-column", "mouseleave", this.handleHeaderMouseLeave.bind(this));

            this.$headerRowScroller
                .on("scroll", this.handleHeaderRowScroll.bind(this));

            this.$footerRowScroller
                .on("scroll", this.handleFooterRowScroll.bind(this));

            this.$focusSink.add(this.$focusSink2)
                .on("keydown", this.handleKeyDown.bind(this));
            this.$canvas
                .on("keydown", this.handleKeyDown.bind(this))
                .on("click", this.handleClick.bind(this))
                .on("dblclick", this.handleDblClick.bind(this))
                .on("contextmenu", this.handleContextMenu.bind(this))
                .on("draginit", this.handleDragInit.bind(this))
                .on("dragstart", { distance: 3 }, this.handleDragStart.bind(this))
                .on("drag", this.handleDrag.bind(this))
                .on("dragend", this.handleDragEnd.bind(this))
                .delegate(".slick-cell", "mouseenter", this.handleMouseEnter.bind(this))
                .delegate(".slick-cell", "mouseleave", this.handleMouseLeave.bind(this));

            // Work around http://crbug.com/312427.
            if (navigator.userAgent.toLowerCase().match(/webkit/) &&
                navigator.userAgent.toLowerCase().match(/macintosh/)) {
                this.$canvas.on("mousewheel", this.handleMouseWheel.bind(this));
            }
        }

        private hasFixedColumns(): boolean {
            return this._fixedStartCols > 0;
        }

        registerPlugin(plugin: Plugin): void {
            this.plugins.unshift(plugin);
            plugin.init(this);
        }

        private unregisterPlugin(plugin: Plugin): void {
            for (var i = this.plugins.length; i >= 0; i--) {
                if (this.plugins[i] === plugin) {
                    if (this.plugins[i].destroy) {
                        this.plugins[i].destroy();
                    }
                    this.plugins.splice(i, 1);
                    break;
                }
            }
        }

        setSelectionModel(model: SelectionModel): void {
            if (this.selectionModel) {
                this.selectionModel.onSelectedRangesChanged.unsubscribe(this.handleSelectedRangesChanged);
                if (this.selectionModel.destroy) {
                    this.selectionModel.destroy();
                }
            }

            this.selectionModel = model;
            if (this.selectionModel) {
                this.selectionModel.init(this);
                this.selectionModel.onSelectedRangesChanged.subscribe(this.handleSelectedRangesChanged);
            }
        }

        getSelectionModel(): SelectionModel {
            return this.selectionModel;
        }

        getCanvasNode(): HTMLDivElement {
            return this.$canvas[0] as HTMLDivElement;
        }

        getCanvasNodes(): JQuery {
            return this.$canvas;
        }

        getViewportNode(): HTMLDivElement {
            return this.$viewport[0] as HTMLDivElement;
        }

        getViewportNodes(): JQuery {
            return this.$viewport;
        }

        private measureScrollbar(): { width: number, height: number } {
            var $c = $("<div style='position:absolute; top:-10000px; " + this.xLeft + ":-10000px; width:100px; height:100px; overflow:scroll;'></div>").appendTo("body");
            var dim = {
                width: Math.round($c.width() - $c[0].clientWidth),
                height: Math.round($c.height() - $c[0].clientHeight)
            };
            $c.remove();
            return dim;
        }

        private calcHeaderWidths(): void {
            this.headersWidthL = this.headersWidthR = 0;

            var cols = this._columns, fixStart = this._fixedStartCols;
            for (var i = 0, ii = cols.length; i < ii; i++) {
                var width = cols[i].width;

                if (fixStart > 0 && i >= fixStart) {
                    this.headersWidthR += width;
                } else {
                    this.headersWidthL += width;
                }
            }

            if (fixStart > 0) {
                this.headersWidthL = this.headersWidthL + 1000;

                this.headersWidthR = Math.max(this.headersWidthR, this.viewportW) + this.headersWidthL;
                this.headersWidthR += scrollbarDimensions.width;
            } else {
                this.headersWidthL += scrollbarDimensions.width;
                this.headersWidthL = Math.max(this.headersWidthL, this.viewportW) + 1000;
            }
        }

        private getCanvasWidth(): number {
            var availableWidth = this.viewportHasVScroll ? this.viewportW - scrollbarDimensions.width : this.viewportW;

            var cols = this._columns, i = cols.length, fixStart = this._fixedStartCols;

            this.canvasWidthL = this.canvasWidthR = 0;

            while (i--) {
                if (fixStart > 0 && i >= fixStart) {
                    this.canvasWidthR += cols[i].width;
                } else {
                    this.canvasWidthL += cols[i].width;
                }
            }

            var totalRowWidth = this.canvasWidthL + this.canvasWidthR;

            return this._options.fullWidthRows ? Math.max(totalRowWidth, availableWidth) : totalRowWidth;
        }

        private updateCanvasWidth(forceColumnWidthsUpdate?: boolean): void {
            var oldCanvasWidth = this.canvasWidth;
            var oldCanvasWidthL = this.canvasWidthL;
            var oldCanvasWidthR = this.canvasWidthR;
            var widthChanged;
            this.canvasWidth = this.getCanvasWidth();

            widthChanged = this.canvasWidth !== oldCanvasWidth || this.canvasWidthL !== oldCanvasWidthL || this.canvasWidthR !== oldCanvasWidthR;

            if (widthChanged || this.hasFixedColumns() || this.hasFrozenRows) {
                this.$canvasTopL.width(this.canvasWidthL);

                this.calcHeaderWidths();

                this.$headerL.width(this.headersWidthL);
                this.$headerR.width(this.headersWidthR);

                if (this.hasFixedColumns()) {
                    this.$canvasTopR.width(this.canvasWidthR);

                    this.$paneHeaderL.width(this.canvasWidthL);
                    this.$paneHeaderR.css(this.xLeft, this.canvasWidthL);
                    this.$paneHeaderR.css('width', this.viewportW - this.canvasWidthL);

                    this.$paneTopL.width(this.canvasWidthL);
                    this.$paneTopR.css(this.xLeft, this.canvasWidthL);
                    this.$paneTopR.css('width', this.viewportW - this.canvasWidthL);

                    this.$headerRowScrollerL.width(this.canvasWidthL);
                    this.$headerRowScrollerR.width(this.viewportW - this.canvasWidthL);

                    this.$headerRowL.width(this.canvasWidthL);
                    this.$headerRowR.width(this.canvasWidthR);

                    this.$footerRowScrollerL.width(this.canvasWidthL);
                    this.$footerRowScrollerR.width(this.viewportW - this.canvasWidthL);

                    this.$footerRowL.width(this.canvasWidthL);
                    this.$footerRowR.width(this.canvasWidthR);

                    this.$viewportTopL.width(this.canvasWidthL);
                    this.$viewportTopR.width(this.viewportW - this.canvasWidthL);

                    if (this.hasFrozenRows) {
                        this.$paneBottomL.width(this.canvasWidthL);
                        this.$paneBottomR.css(this.xLeft, this.canvasWidthL);

                        this.$viewportBottomL.width(this.canvasWidthL);
                        this.$viewportBottomR.width(this.viewportW - this.canvasWidthL);

                        this.$canvasBottomL.width(this.canvasWidthL);
                        this.$canvasBottomR.width(this.canvasWidthR);
                    }
                } else {
                    this.$paneHeaderL.css('width', '100%');

                    this.$paneTopL.css('width', '100%');

                    this.$headerRowScrollerL.css('width', '100%');

                    this.$headerRowL.width(this.canvasWidth);

                    this.$footerRowScrollerL.css('width', '100%');

                    this.$footerRowL.width(this.canvasWidth);

                    this.$viewportTopL.css('width', '100%');

                    if (this.hasFrozenRows) {
                        this.$viewportBottomL.css('width', '100%');
                        this.$canvasBottomL.width(this.canvasWidthL);
                    }
                }

                this.viewportHasHScroll = (this.canvasWidth > this.viewportW - scrollbarDimensions.width);
            }

            var w = this.canvasWidth + (this.viewportHasVScroll ? scrollbarDimensions.width : 0);

            this.$headerRowSpacerL.width(w);
            this.$headerRowSpacerR.width(w);

            this.$footerRowSpacerL.width(w);
            this.$footerRowSpacerR.width(w);

            if (widthChanged || forceColumnWidthsUpdate) {
                this.applyColumnWidths();
            }
        }

        private bindAncestorScrollEvents(): void {
            var elem: HTMLElement = (this.hasFrozenRows && !this._options.frozenBottom) ? this.$canvasBottomL[0] : this.$canvasTopL[0];
            while ((elem = elem.parentNode as HTMLElement) != document.body && elem != null) {
                // bind to scroll containers only
                if (elem == this.$viewportTopL[0] || elem.scrollWidth != elem.clientWidth || elem.scrollHeight != elem.clientHeight) {
                    var $elem = $(elem);
                    if (!this.$boundAncestors) {
                        this.$boundAncestors = $elem;
                    } else {
                        this.$boundAncestors = this.$boundAncestors.add($elem);
                    }
                    $elem.on("scroll." + this.uid, this.handleActiveCellPositionChange.bind(this));
                }
            }
        }

        private unbindAncestorScrollEvents(): void {
            if (!this.$boundAncestors) {
                return;
            }
            this.$boundAncestors.off("scroll." + this.uid);
            this.$boundAncestors = null;
        }

        updateColumnHeader(columnId: string, title?: string, toolTip?: string): void {
            if (!this.initialized) { return; }
            var idx = this.getColumnIndex(columnId);
            if (idx == null) {
                return;
            }

            var columnDef = this._columns[idx];
            var $header = this.$headers.children().eq(idx);
            if (!$header)
                return;

            if (title !== undefined) {
                columnDef.name = title;
            }
            if (toolTip !== undefined) {
                columnDef.toolTip = toolTip;
            }

            this.trigger(this.onBeforeHeaderCellDestroy, {
                node: $header[0],
                column: columnDef
            });

            $header
                .attr("title", toolTip || "")
                .children().eq(0).html(title);

            this.trigger(this.onHeaderCellRendered, {
                node: $header[0],
                column: columnDef
            });
        }

        getHeader(): HTMLDivElement {
            return this.$headers[0] as HTMLDivElement;
        }

        getHeaderColumn(columnIdOrIdx: string | number): HTMLDivElement {
            var idx = (typeof columnIdOrIdx === "number" ? columnIdOrIdx : this.getColumnIndex(columnIdOrIdx));
            var $rtn = this.$headers.children().eq(idx);
            return $rtn && $rtn[0] as HTMLDivElement;
        }

        getGroupingPanel(): HTMLDivElement {
            return this.$groupingPanel == null ? null : this.$groupingPanel[0] as HTMLDivElement;
        }

        getHeaderRow(): HTMLDivElement {
            return this.$headerRow[0] as HTMLDivElement;
        }

        getHeaderRowColumn(columnId: string): any {
            var idx = this.getColumnIndex(columnId);

            var $headerRowTarget, fixStart = this._fixedStartCols;

            if (fixStart > 0) {
                if (idx < fixStart) {
                    $headerRowTarget = this.$headerRowL;
                } else {
                    $headerRowTarget = this.$headerRowR;

                    idx -= fixStart;
                }
            } else {
                $headerRowTarget = this.$headerRowL;
            }

            var $header = $headerRowTarget.children().eq(idx);
            return $header && $header[0];
        }

        getFooterRow(): HTMLDivElement {
            return this.$footerRow[0] as HTMLDivElement;
        }

        getFooterRowColumn(columnId: string): HTMLElement {
            var idx = this.getColumnIndex(columnId);

            var $footerRowTarget, fixStart = this._fixedStartCols;

            if (fixStart > 0) {
                if (idx < fixStart) {
                    $footerRowTarget = this.$footerRowL;
                } else {
                    $footerRowTarget = this.$footerRowR;

                    idx -= fixStart;
                }
            } else {
                $footerRowTarget = this.$footerRowL;
            }

            var $footer = $footerRowTarget.children().eq(idx);
            return $footer && $footer[0];
        }

        private createColumnFooter(): void {
            var self = this;
            this.$footerRow.find(".slick-footerrow-column")
                .each(function () {
                    var columnDef = $(self).data("column");
                    if (columnDef) {
                        self.trigger(self.onBeforeFooterRowCellDestroy, {
                            node: this,
                            column: columnDef
                        });
                    }
                });

            this.$footerRowL.empty();
            this.$footerRowR.empty();

            var cols = this._columns, fixStart = this._fixedStartCols;
            for (var i = 0; i < cols.length; i++) {
                var m = cols[i];

                var footerRowCell = $("<div class='ui-state-default slick-footerrow-column l" + i + " r" + i + "'></div>")
                    .data("column", m)
                    .addClass(m.footerCssClass || m.cssClass || '')
                    .addClass(i < fixStart ? 'frozen' : '')
                    .appendTo(fixStart > 0 && i >= fixStart ? this.$footerRowR : this.$footerRowL);

                this.trigger(this.onFooterRowCellRendered, {
                    node: footerRowCell[0],
                    column: m
                });
            }
        }

        private formatGroupTotal(total: GroupTotals, columnDef: Column<TItem>): any {
            if (columnDef.formatter != null) {
                var item = new Slick.NonDataRow();
                item[columnDef.field] = total;
                try {
                    return columnDef.formatter(-1, -1, total, columnDef, item as any);
                }
                catch (e) {
                }
            }

            //@ts-ignore
            if (typeof total == "number" && Q && Q.formatNumber) {
                if ((columnDef as any).sourceItem && (columnDef as any).sourceItem.displayFormat) {
                    //@ts-ignore
                    return Q.formatNumber(total, columnDef.sourceItem.displayFormat);
                }
                else
                    //@ts-ignore
                    return Q.formatNumber(total, "#,##0.##");
            }
            //@ts-ignore
            else if (Q.htmlEncode)
                //@ts-ignore
                return Q.htmlEncode(total);
            else
                return total;
        }

        private groupTotalText(totals: GroupTotals, columnDef: Column<TItem>, key: string): string {
            var ltKey = (key.substring(0, 1).toUpperCase() + key.substring(1));
            //@ts-ignore
            var text = (Q && Q.tryGetText && Q.tryGetText(ltKey)) || ltKey;

            var total = totals[key][columnDef.field];
            total = this.formatGroupTotal(total, columnDef);

            return "<span class='aggregate agg-" + key + "'  title='" + text + "'>" +
                total +
                "</span>";
        }

        private groupTotalsFormatter(totals: GroupTotals, columnDef: Column<TItem>): string {
            if (!totals || !columnDef)
                return "";

            var text: string = null;
            var self = this;

            ["sum", "avg", "min", "max", "cnt"].forEach(function (key) {
                if (text == null && totals[key] && totals[key][columnDef.field] != null) {
                    text = self.groupTotalText(totals, columnDef, key);
                    return false;
                }
            });

            return text || "";
        }

        private createColumnHeaders(): void {
            function onMouseEnter() {
                $(this).addClass("ui-state-hover");
            }

            function onMouseLeave() {
                $(this).removeClass("ui-state-hover");
            }

            var self = this;
            this.$headers.find(".slick-header-column")
                .each(function () {
                    var columnDef = $(self).data("column");
                    if (columnDef) {
                        self.trigger(self.onBeforeHeaderCellDestroy, {
                            node: this,
                            column: columnDef
                        });
                    }
                });

            this.$headerL.empty();
            this.$headerR.empty();

            this.calcHeaderWidths();

            this.$headerL.width(this.headersWidthL);
            this.$headerR.width(this.headersWidthR);

            this.$headerRow.find(".slick-headerrow-column")
                .each(function () {
                    var columnDef = $(this).data("column");
                    if (columnDef) {
                        self.trigger(self.onBeforeHeaderRowCellDestroy, {
                            node: this,
                            column: columnDef
                        });
                    }
                });

            this.$headerRowL.empty();
            this.$headerRowR.empty();

            var cols = this._columns, fixStart = this._fixedStartCols;
            for (var i = 0; i < cols.length; i++) {
                var m = cols[i];

                var $headerTarget = fixStart > 0 && i >= fixStart ? this.$headerR : this.$headerL;
                var $headerRowTarget = fixStart > 0 && i >= fixStart ? this.$headerRowR : this.$headerRowL;

                var header = $("<div class='ui-state-default slick-header-column' />")
                    .html("<span class='slick-column-name'>" + m.name + "</span>")
                    .width(m.width - this.headerColumnWidthDiff)
                    .attr("id", "" + this.uid + m.id)
                    .attr("title", m.toolTip || "")
                    .data("column", m)
                    .addClass(m.headerCssClass || "")
                    .addClass(i < fixStart ? 'frozen' : '')
                    .appendTo($headerTarget);

                if (this._options.enableColumnReorder || m.sortable) {
                    header
                        .on('mouseenter', onMouseEnter)
                        .on('mouseleave', onMouseLeave);
                }

                if (m.sortable) {
                    header.addClass("slick-header-sortable");
                    header.append("<span class='slick-sort-indicator' />");
                }

                this.trigger(this.onHeaderCellRendered, {
                    node: header[0],
                    column: m
                });

                if (this._options.showHeaderRow) {
                    var headerRowCell = $("<div class='ui-state-default slick-headerrow-column l" + i + " r" + i + "'></div>")
                        .data("column", m)
                        .appendTo($headerRowTarget);

                    this.trigger(this.onHeaderRowCellRendered, {
                        node: headerRowCell[0],
                        column: m
                    });
                }
            }

            this.setSortColumns(this.sortColumns);
            this.setupColumnResize();
            if (this._options.enableColumnReorder)
                this.setupColumnReorder();
        }

        private setupColumnSort(): void {
            this.$headers.click((e) => {
                // temporary workaround for a bug in jQuery 1.7.1 (http://bugs.jquery.com/ticket/11328)
                e.metaKey = e.metaKey || e.ctrlKey;

                if ($(e.target).hasClass("slick-resizable-handle")) {
                    return;
                }

                var $col = $(e.target).closest(".slick-header-column");
                if (!$col.length) {
                    return;
                }

                var column = $col.data("column") as Column<TItem>;
                if (column.sortable) {
                    if (!this.getEditorLock().commitCurrentEdit()) {
                        return;
                    }

                    var sortOpts = null;
                    var i = 0;
                    for (; i < this.sortColumns.length; i++) {
                        if (this.sortColumns[i].columnId == column.id) {
                            sortOpts = this.sortColumns[i];
                            sortOpts.sortAsc = !sortOpts.sortAsc;
                            break;
                        }
                    }

                    if (e.metaKey && this._options.multiColumnSort) {
                        if (sortOpts) {
                            this.sortColumns.splice(i, 1);
                        }
                    }
                    else {
                        if ((!e.shiftKey && !e.metaKey) || !this._options.multiColumnSort) {
                            this.sortColumns = [];
                        }

                        if (!sortOpts) {
                            sortOpts = { columnId: column.id, sortAsc: column.defaultSortAsc };
                            this.sortColumns.push(sortOpts);
                        } else if (this.sortColumns.length == 0) {
                            this.sortColumns.push(sortOpts);
                        }
                    }

                    this.setSortColumns(this.sortColumns);

                    if (!this._options.multiColumnSort) {
                        this.trigger(this.onSort, {
                            multiColumnSort: false,
                            sortCol: column,
                            sortAsc: sortOpts.sortAsc
                        }, e);
                    } else {
                        var cols = this._columns;
                        this.trigger(this.onSort, {
                            multiColumnSort: true,
                            sortCols: this.sortColumns.map(col => ({ sortCol: cols[this.getColumnIndex(col.columnId)], sortAsc: col.sortAsc }))
                        }, e);
                    }
                }
            });
        }

        private setupColumnReorder(): void {
            (this.$headers.filter(":ui-sortable") as any).sortable("destroy");
            var columnScrollTimer: number = null;

            var scrollColumnsRight = () => {
                this.$viewportScrollContainerX[0].scrollLeft = this.$viewportScrollContainerX[0].scrollLeft + 10;
            }

            var scrollColumnsLeft = () => {
                this.$viewportScrollContainerX[0].scrollLeft = this.$viewportScrollContainerX[0].scrollLeft - 10;
            }

            var canDragScroll: boolean;

            var hasGrouping = this._options.groupingPanel;
            var columns = this._columns;
            (this.$headers as any).sortable({
                containment: hasGrouping ? undefined : "parent",
                distance: 3,
                axis: hasGrouping ? undefined : "x",
                cursor: "default",
                tolerance: "intersection",
                helper: "clone",
                placeholder: "slick-sortable-placeholder ui-state-default slick-header-column",
                forcePlaceholderSize: hasGrouping ? true : undefined,
                appendTo: hasGrouping ? "body" : undefined,
                start: (_: HtmlEvent, ui: any) => {
                    ui.placeholder.width(ui.helper.outerWidth() - this.headerColumnWidthDiff);
                    canDragScroll = !this.hasFixedColumns() ||
                        (ui.placeholder.offset()[this.xLeft] + Math.round(ui.placeholder.width())) > this.$viewportScrollContainerX.offset()[this.xLeft];
                    $(ui.helper).addClass("slick-header-column-active");
                },
                beforeStop: (_: HtmlEvent, ui: any) => {
                    $(ui.helper).removeClass("slick-header-column-active");
                    if (hasGrouping) {
                        var $headerDraggableGroupBy = $(this.getGroupingPanel());
                        var hasDroppedColumn = $headerDraggableGroupBy
                            .find(".slick-dropped-grouping").length;
                        if (hasDroppedColumn > 0) {
                            $headerDraggableGroupBy.find(".slick-dropped-placeholder").hide();
                            $headerDraggableGroupBy.find(".slick-dropped-grouping").show();
                        }
                    }
                },
                sort: (e: JQueryEventObject) => {
                    if (canDragScroll && (e.originalEvent as any).pageX > this.$container[0].clientWidth) {
                        if (!(columnScrollTimer)) {
                            columnScrollTimer = setInterval(
                                scrollColumnsRight, 100);
                        }
                    } else if (canDragScroll && (e.originalEvent as any).pageX < this.$viewportScrollContainerX.offset().left) {
                        if (!(columnScrollTimer)) {
                            columnScrollTimer = setInterval(
                                scrollColumnsLeft, 100);
                        }
                    } else {
                        clearInterval(columnScrollTimer);
                        columnScrollTimer = null;
                    }
                },
                stop: (e: HtmlEvent) => {
                    var cancel = false;
                    clearInterval(columnScrollTimer);
                    columnScrollTimer = null;

                    if (cancel || !this.getEditorLock().commitCurrentEdit()) {
                        ($(e.target) as any).sortable("cancel");
                        return;
                    }

                    var reorderedIds = (this.$headerL as any).sortable("toArray");
                    reorderedIds = reorderedIds.concat((this.$headerR as any).sortable("toArray"));

                    var reorderedColumns = [];
                    for (var i = 0; i < reorderedIds.length; i++) {
                        reorderedColumns.push(columns[this.getColumnIndex(reorderedIds[i].replace(this.uid, ""))]);
                    }
                    this.setColumns(reorderedColumns);

                    this.trigger(this.onColumnsReordered, { });
                    e.stopPropagation();
                    this.setupColumnResize();
                }
            });
        }

        private setupColumnResize(): void {
            var $col: JQuery, j: number, k: number, c: Column<TItem>, pageX: number, columnElements: JQuery, minPageX: number, maxPageX: number, firstResizable: number, lastResizable: number, cols = this._columns;
            columnElements = this.$headers.children();
            columnElements.find(".slick-resizable-handle").remove();
            columnElements.each((i) => {
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
            columnElements.each((i, e) => {
                if (i < firstResizable || (this._options.forceFitColumns && i >= lastResizable)) {
                    return;
                }
                $col = $(e);
                $("<div class='slick-resizable-handle' />")
                    .appendTo(e)
                    .on("dragstart", (e, dd) => {
                        if (!this.getEditorLock().commitCurrentEdit()) {
                            return false;
                        }
                        pageX = e.pageX;
                        $(e.target).parent().addClass("slick-header-column-active");
                        var shrinkLeewayOnRight = null, stretchLeewayOnRight = null;
                        // lock each column's width option to current width
                        columnElements.each((i, e) => {
                            cols[i].previousWidth = $(e).outerWidth();
                        });
                        if (this._options.forceFitColumns) {
                            shrinkLeewayOnRight = 0;
                            stretchLeewayOnRight = 0;
                            // colums on right affect maxPageX/minPageX
                            for (j = i + 1; j < columnElements.length; j++) {
                                c = cols[j];
                                if (c.resizable) {
                                    if (stretchLeewayOnRight !== null) {
                                        if (c.maxWidth) {
                                            stretchLeewayOnRight += c.maxWidth - c.previousWidth;
                                        } else {
                                            stretchLeewayOnRight = null;
                                        }
                                    }
                                    shrinkLeewayOnRight += c.previousWidth - Math.max(c.minWidth || 0, this.absoluteColumnMinWidth);
                                }
                            }
                        }
                        var shrinkLeewayOnLeft = 0, stretchLeewayOnLeft = 0;
                        for (j = 0; j <= i; j++) {
                            // columns on left only affect minPageX
                            c = cols[j];
                            if (c.resizable) {
                                if (stretchLeewayOnLeft !== null) {
                                    if (c.maxWidth) {
                                        stretchLeewayOnLeft += c.maxWidth - c.previousWidth;
                                    } else {
                                        stretchLeewayOnLeft = null;
                                    }
                                }
                                shrinkLeewayOnLeft += c.previousWidth - Math.max(c.minWidth || 0, this.absoluteColumnMinWidth);
                            }
                        }
                        if (shrinkLeewayOnRight === null) {
                            shrinkLeewayOnRight = 100000;
                        }
                        if (shrinkLeewayOnLeft === null) {
                            shrinkLeewayOnLeft = 100000;
                        }
                        if (stretchLeewayOnRight === null) {
                            stretchLeewayOnRight = 100000;
                        }
                        if (stretchLeewayOnLeft === null) {
                            stretchLeewayOnLeft = 100000;
                        }
                        maxPageX = pageX + Math.min(shrinkLeewayOnRight, stretchLeewayOnLeft);
                        minPageX = pageX - Math.min(shrinkLeewayOnLeft, stretchLeewayOnRight);
                    })
                    .on("drag", (e, dd) => {
                        var actualMinWidth, d = Math.min(maxPageX, Math.max(minPageX, e.pageX)) - pageX, x;
                        if (d < 0) { // shrink column
                            x = d;

                            var newCanvasWidthL = 0, newCanvasWidthR = 0;

                            for (j = i; j >= 0; j--) {
                                c = cols[j];
                                if (c.resizable) {
                                    actualMinWidth = Math.max(c.minWidth || 0, this.absoluteColumnMinWidth);
                                    if (x && c.previousWidth + x < actualMinWidth) {
                                        x += c.previousWidth - actualMinWidth;
                                        c.width = actualMinWidth;
                                    } else {
                                        c.width = c.previousWidth + x;
                                        x = 0;
                                    }
                                }
                            }

                            var fixStart = this._fixedStartCols;

                            for (k = 0; k <= i; k++) {
                                c = cols[k];

                                if (fixStart > 0 && k >= fixStart) {
                                    newCanvasWidthR += c.width;
                                } else {
                                    newCanvasWidthL += c.width;
                                }
                            }

                            if (this._options.forceFitColumns) {
                                x = -d;
                                for (j = i + 1; j < columnElements.length; j++) {
                                    c = cols[j];
                                    if (c.resizable) {
                                        if (x && c.maxWidth && (c.maxWidth - c.previousWidth < x)) {
                                            x -= c.maxWidth - c.previousWidth;
                                            c.width = c.maxWidth;
                                        } else {
                                            c.width = c.previousWidth + x;
                                            x = 0;
                                        }

                                        if (fixStart > 0 && j >= fixStart) {
                                            newCanvasWidthR += c.width;
                                        } else {
                                            newCanvasWidthL += c.width;
                                        }
                                    }
                                }
                            } else {
                                for (j = i + 1; j < columnElements.length; j++) {
                                    c = cols[j];

                                    if (fixStart >= 0 && j >= fixStart) {
                                        newCanvasWidthR += c.width;
                                    } else {
                                        newCanvasWidthL += c.width;
                                    }
                                }
                            }
                        } else { // stretch column
                            x = d;

                            var newCanvasWidthL = 0, newCanvasWidthR = 0;

                            for (j = i; j >= 0; j--) {
                                c = cols[j];
                                if (c.resizable) {
                                    if (x && c.maxWidth && (c.maxWidth - c.previousWidth < x)) {
                                        x -= c.maxWidth - c.previousWidth;
                                        c.width = c.maxWidth;
                                    } else {
                                        c.width = c.previousWidth + x;
                                        x = 0;
                                    }
                                }
                            }

                            for (k = 0; k <= i; k++) {
                                c = cols[k];

                                if (fixStart > 0 && k >= fixStart) {
                                    newCanvasWidthR += c.width;
                                } else {
                                    newCanvasWidthL += c.width;
                                }
                            }

                            if (this._options.forceFitColumns) {
                                x = -d;
                                for (j = i + 1; j < columnElements.length; j++) {
                                    c = cols[j];
                                    if (c.resizable) {
                                        actualMinWidth = Math.max(c.minWidth || 0, this.absoluteColumnMinWidth);
                                        if (x && c.previousWidth + x < actualMinWidth) {
                                            x += c.previousWidth - actualMinWidth;
                                            c.width = actualMinWidth;
                                        } else {
                                            c.width = c.previousWidth + x;
                                            x = 0;
                                        }

                                        if (fixStart && j >= fixStart) {
                                            newCanvasWidthR += c.width;
                                        } else {
                                            newCanvasWidthL += c.width;
                                        }
                                    }
                                }
                            } else {
                                for (j = i + 1; j < columnElements.length; j++) {
                                    c = cols[j];

                                    if (fixStart > 0 && j >= fixStart) {
                                        newCanvasWidthR += c.width;
                                    } else {
                                        newCanvasWidthL += c.width;
                                    }
                                }
                            }
                        }

                        if (this.hasFixedColumns() && newCanvasWidthL != this.canvasWidthL) {
                            this.$headerL.width(newCanvasWidthL + 1000);
                            this.$paneHeaderR.css(this.xLeft, newCanvasWidthL);
                        }

                        this.applyColumnHeaderWidths();
                        if (this._options.syncColumnCellResize) {
                            //              updateCanvasWidth()
                            this.applyColumnWidths();
                        }
                    })
                    .on("dragend", (e: HtmlEvent) => {
                        var newWidth;
                        $(e.target).parent().removeClass("slick-header-column-active");
                        for (j = 0; j < columnElements.length; j++) {
                            c = cols[j];
                            newWidth = $(columnElements[j]).outerWidth();

                            if (c.previousWidth !== newWidth && c.rerenderOnResize) {
                                this.invalidateAllRows();
                            }
                        }
                        this.updateCanvasWidth(true);
                        this.render();
                        this.trigger(this.onColumnsResized);
                    });
            });
        }

        private getVBoxDelta($el: JQuery): number {
            if ($el.css('box-sizing') == 'border-box')
                return 0;

            var p = ["borderTopWidth", "borderBottomWidth", "paddingTop", "paddingBottom"];
            var delta = 0;
            $.each(p, function (n, val) {
                delta += parseFloat($el.css(val)) || 0;
            });
            return delta;
        }

        private adjustFrozenRowOption(): void {
            this._options.frozenRow = (this._options.frozenRow >= 0
                && this._options.frozenRow < this.numVisibleRows
            )
                ? this._options.frozenRow
                : -1;

            if (this._options.frozenRow > -1) {
                this.hasFrozenRows = true;
                this.frozenRowsHeight = (this._options.frozenRow) * this._options.rowHeight;

                var dataLength = this.getDataLength() || this._data.length;

                this.actualFrozenRow = (this._options.frozenBottom)
                    ? (dataLength - this._options.frozenRow)
                    : this._options.frozenRow;
            } else {
                this.hasFrozenRows = false;
            }
        }

        private setPaneVisibility(): void {
            if (this.hasFixedColumns()) {
                this.$paneHeaderR.show();
                this.$paneTopR.show();

                if (this.hasFrozenRows) {
                    this.$paneBottomL.show();
                    this.$paneBottomR.show();
                } else {
                    this.$paneBottomR.hide();
                    this.$paneBottomL.hide();
                }
            } else {
                this.$paneHeaderR.hide();
                this.$paneTopR.hide();
                this.$paneBottomR.hide();

                if (this.hasFrozenRows) {
                    this.$paneBottomL.show();
                } else {
                    this.$paneBottomR.hide();
                    this.$paneBottomL.hide();
                }
            }
        }

        private setOverflow(): void {
            this.$viewportTopL.css({
                'overflow-x': (this.hasFixedColumns()) ? (this.hasFrozenRows ? 'hidden' : 'scroll') : (this.hasFrozenRows ? 'hidden' : 'auto'),
                'overflow-y': (this.hasFixedColumns()) ? (this.hasFrozenRows ? 'hidden' : 'hidden') : (this.hasFrozenRows ? 'scroll' : 'auto')
            });

            this.$viewportTopR.css({
                'overflow-x': (this.hasFixedColumns()) ? (this.hasFrozenRows ? 'hidden' : 'scroll') : (this.hasFrozenRows ? 'hidden' : 'auto'),
                'overflow-y': (this.hasFixedColumns()) ? (this.hasFrozenRows ? 'scroll' : 'auto') : (this.hasFrozenRows ? 'scroll' : 'auto')
            });

            this.$viewportBottomL.css({
                'overflow-x': (this.hasFixedColumns()) ? (this.hasFrozenRows ? 'scroll' : 'auto') : (this.hasFrozenRows ? 'auto' : 'auto'),
                'overflow-y': (this.hasFixedColumns()) ? (this.hasFrozenRows ? 'hidden' : 'hidden') : (this.hasFrozenRows ? 'scroll' : 'auto')
            });

            this.$viewportBottomR.css({
                'overflow-x': (this.hasFixedColumns()) ? (this.hasFrozenRows ? 'scroll' : 'auto') : (this.hasFrozenRows ? 'auto' : 'auto'),
                'overflow-y': (this.hasFixedColumns()) ? (this.hasFrozenRows ? 'auto' : 'auto') : (this.hasFrozenRows ? 'auto' : 'auto')
            });
        }

        private setScroller(): void {
            if (this.hasFixedColumns()) {
                this.$headerScrollContainer = this.$headerScrollerR;
                this.$headerRowScrollContainer = this.$headerRowScrollerR;
                this.$footerRowScrollContainer = this.$footerRowScrollerR

                if (this.hasFrozenRows) {
                    if (this._options.frozenBottom) {
                        this.$viewportScrollContainerX = this.$viewportBottomR;
                        this.$viewportScrollContainerY = this.$viewportTopR;
                    } else {
                        this.$viewportScrollContainerX = this.$viewportScrollContainerY = this.$viewportBottomR;
                    }
                } else {
                    this.$viewportScrollContainerX = this.$viewportScrollContainerY = this.$viewportTopR;
                }
            } else {
                this.$headerScrollContainer = this.$headerScrollerL;
                this.$headerRowScrollContainer = this.$headerRowScrollerL;
                this.$footerRowScrollContainer = this.$footerRowScrollerL;

                if (this.hasFrozenRows) {
                    if (this._options.frozenBottom) {
                        this.$viewportScrollContainerX = this.$viewportBottomL;
                        this.$viewportScrollContainerY = this.$viewportTopL;
                    } else {
                        this.$viewportScrollContainerX = this.$viewportScrollContainerY = this.$viewportBottomL;
                    }
                } else {
                    this.$viewportScrollContainerX = this.$viewportScrollContainerY = this.$viewportTopL;
                }
            }
        }

        private measureCellPaddingAndBorder(): void {
            var el: JQuery;
            var h = ["borderLeftWidth", "borderRightWidth", "paddingLeft", "paddingRight"];
            var v = ["borderTopWidth", "borderBottomWidth", "paddingTop", "paddingBottom"];

            // jquery prior to version 1.8 handles .width setter/getter as a direct css write/read
            // jquery 1.8 changed .width to read the true inner element width if box-sizing is set to border-box, and introduced a setter for .outerWidth
            // so for equivalent functionality, prior to 1.8 use .width, and after use .outerWidth
            var verArray = $.fn.jquery.split('.');
            this.jQueryNewWidthBehaviour = (verArray[0] as any == "1" && verArray[1] as any >= 8) || verArray[0] as any >= 2;

            el = $("<div class='ui-state-default slick-header-column' style='visibility:hidden'>-</div>").appendTo(this.$headers);
            this.headerColumnWidthDiff = 0;
            if (el.css("box-sizing") != "border-box" && el.css("-moz-box-sizing") != "border-box" && el.css("-webkit-box-sizing") != "border-box") {
                $.each(h, (_, val) => {
                    this.headerColumnWidthDiff += parseFloat(el.css(val)) || 0;
                });
            }
            el.remove();

            var r = $("<div class='slick-row' />").appendTo(this.$canvas);
            el = $("<div class='slick-cell' id='' style='visibility:hidden'>-</div>").appendTo(r);
            this.cellWidthDiff = this.cellHeightDiff = 0;
            if (el.css("box-sizing") != "border-box" && el.css("-moz-box-sizing") != "border-box" && el.css("-webkit-box-sizing") != "border-box") {
                $.each(h, (_, val) => {
                    this.cellWidthDiff += parseFloat(el.css(val)) || 0;
                });
                $.each(v, (_, val) => {
                    this.cellHeightDiff += parseFloat(el.css(val)) || 0;
                });
            }
            r.remove();

            this.absoluteColumnMinWidth = Math.max(this.headerColumnWidthDiff, this.cellWidthDiff);
        }

        private createCssRules() {
            this.$style = $("<style type='text/css' rel='stylesheet' data-uid='" + this.uid + "' />").appendTo($("head"));
            var rowHeight = (this._options.rowHeight - this.cellHeightDiff);
            var rules = [
                "." + this.uid + " .slick-group-header-column { " + this.xLeft + ": 1000px; }",
                "." + this.uid + " .slick-header-column { " + this.xLeft + ": 1000px; }",
                "." + this.uid + " .slick-top-panel { height:" + this._options.topPanelHeight + "px; }",
                "." + this.uid + " .slick-grouping-panel { height:" + this._options.groupingPanelHeight + "px; }",
                "." + this.uid + " .slick-headerrow-columns { height:" + this._options.headerRowHeight + "px; }",
                "." + this.uid + " .slick-cell { height:" + rowHeight + "px; }",
                "." + this.uid + " .slick-row { height:" + this._options.rowHeight + "px; }",
                "." + this.uid + " .slick-footerrow-columns { height:" + this._options.footerRowHeight + "px; }"
            ];

            var cols = this._columns;
            for (var i = 0; i < cols.length; i++) {
                rules.push("." + this.uid + " .l" + i + " { }");
                rules.push("." + this.uid + " .r" + i + " { }");
            }

            if ((this.$style[0] as any).styleSheet) { // IE
                (this.$style[0] as any).styleSheet.cssText = rules.join(" ");
            } else {
                this.$style[0].appendChild(document.createTextNode(rules.join(" ")));
            }
        }

        private getColumnCssRules(idx: number): { right: any; left: any; } {
            if (!this.stylesheet) {
                var stylesheetFromUid = document.querySelector("style[data-uid='" + this.uid + "']") as any
                if (stylesheetFromUid && stylesheetFromUid.sheet) {
                    this.stylesheet = stylesheetFromUid.sheet;
                } else {
                    var sheets = document.styleSheets;
                    for (var i = 0; i < sheets.length; i++) {
                        if ((sheets[i].ownerNode || (sheets[i] as any).owningElement) == this.$style[0]) {
                            this.stylesheet = sheets[i];
                            break;
                        }
                    }
                }

                if (!this.stylesheet) {
                    throw new Error("Cannot find stylesheet.");
                }

                // find and cache column CSS rules
                this.columnCssRulesL = [];
                this.columnCssRulesR = [];
                var cssRules = (this.stylesheet.cssRules || this.stylesheet.rules);
                var matches, columnIdx;
                for (var i = 0; i < cssRules.length; i++) {
                    var selector = cssRules[i].selectorText;
                    if (matches = /\.l\d+/.exec(selector)) {
                        columnIdx = parseInt(matches[0].substring(2, matches[0].length), 10);
                        this.columnCssRulesL[columnIdx] = cssRules[i];
                    } else if (matches = /\.r\d+/.exec(selector)) {
                        columnIdx = parseInt(matches[0].substring(2, matches[0].length), 10);
                        this.columnCssRulesR[columnIdx] = cssRules[i];
                    }
                }
            }

            return this.rtl ? {
                "right": this.columnCssRulesL[idx],
                "left": this.columnCssRulesR[idx]
            } : {
                "left": this.columnCssRulesL[idx],
                "right": this.columnCssRulesR[idx]
            }
        }

        private removeCssRules() {
            this.$style.remove();
            this.stylesheet = null;
        }

        destroy() {
            this.getEditorLock().cancelCurrentEdit();

            this.trigger(this.onBeforeDestroy);

            var i = this.plugins.length;
            while (i--) {
                this.unregisterPlugin(this.plugins[i]);
            }

            if (this._options.enableColumnReorder) {
                (this.$headers.filter(":ui-sortable") as any).sortable("destroy");
            }

            this.unbindAncestorScrollEvents();
            this.$container.off(".slickgrid");
            this.removeCssRules();

            this.$canvas.off("draginit dragstart dragend drag");
            this.$container.empty().removeClass(this.uid);
        }


        //////////////////////////////////////////////////////////////////////////////////////////////
        // General

        private trigger<TArgs extends ArgsGrid, TEventData extends IEventData = IEventData>(
            evt: Event<TArgs, TEventData>, args?: TArgs, e?: TEventData) {
            e = e || new Slick.EventData() as any;
            args = args || {} as any;
            args.grid = this;
            return evt.notify(args, e, this);
        }

        getEditorLock(): EditorLock {
            return this._options.editorLock;
        }

        getEditController(): EditController {
            return this.editController;
        }

        getColumnIndex(id: string): number {
            return this._columnById[id];
        }

        getViewColumnIndex(id: string): number {
            return this._viewColById[id];
        }

        autosizeColumns(): void {
            var i, c,
                widths = [],
                shrinkLeeway = 0,
                total = 0,
                prevTotal,
                availWidth = this.viewportHasVScroll ? this.viewportW - scrollbarDimensions.width : this.viewportW,
                cols = this._columns;
                
            for (i = 0; i < cols.length; i++) {
                c = cols[i];
                widths.push(c.width);
                total += c.width;
                if (c.resizable) {
                    shrinkLeeway += c.width - Math.max(c.minWidth, this.absoluteColumnMinWidth);
                }
            }

            // shrink
            prevTotal = total;
            while (total > availWidth && shrinkLeeway) {
                var shrinkProportion = (total - availWidth) / shrinkLeeway;
                for (i = 0; i < cols.length && total > availWidth; i++) {
                    c = cols[i];
                    var width = widths[i];
                    if (!c.resizable || width <= c.minWidth || width <= this.absoluteColumnMinWidth) {
                        continue;
                    }
                    var absMinWidth = Math.max(c.minWidth, this.absoluteColumnMinWidth);
                    var shrinkSize = Math.floor(shrinkProportion * (width - absMinWidth)) || 1;
                    shrinkSize = Math.min(shrinkSize, width - absMinWidth);
                    total -= shrinkSize;
                    shrinkLeeway -= shrinkSize;
                    widths[i] -= shrinkSize;
                }
                if (prevTotal <= total) {  // avoid infinite loop
                    break;
                }
                prevTotal = total;
            }

            // grow
            prevTotal = total;
            while (total < availWidth) {
                var growProportion = availWidth / total;
                for (i = 0; i < cols.length && total < availWidth; i++) {
                    c = cols[i];
                    var currentWidth = widths[i];
                    var growSize;

                    if (!c.resizable || c.maxWidth <= currentWidth) {
                        growSize = 0;
                    } else {
                        growSize = Math.min(Math.floor(growProportion * currentWidth) - currentWidth, (c.maxWidth - currentWidth) || 1000000) || 1;
                    }
                    total += growSize;
                    widths[i] += (total <= availWidth ? growSize : 0);
                }
                if (prevTotal >= total) {  // avoid infinite loop
                    break;
                }
                prevTotal = total;
            }

            var reRender = false;
            for (i = 0; i < cols.length; i++) {
                if (cols[i].rerenderOnResize && cols[i].width != widths[i]) {
                    reRender = true;
                }
                cols[i].width = widths[i];
            }

            this.applyColumnHeaderWidths();
            this.updateCanvasWidth(true);
            if (reRender) {
                this.invalidateAllRows();
                this.render();
            }
        }

        private applyColumnHeaderWidths(): void {
            if (!this.initialized) { return; }
            var h, cols = this._columns;
            for (var i = 0, headers = this.$headers.children(), ii = headers.length; i < ii; i++) {
                h = $(headers[i]);
                if (this.jQueryNewWidthBehaviour) {
                    if (h.outerWidth() !== cols[i].width) {
                        h.outerWidth(cols[i].width);
                    }
                } else {
                    if (Math.round(h.width()) !== cols[i].width - this.headerColumnWidthDiff) {
                        h.width(cols[i].width - this.headerColumnWidthDiff);
                    }
                }
            }

            this.updateViewColLeftRight();
        }

        private applyColumnWidths(): void {
            var x = 0, w, rule, cols = this._columns, fixStart = this._fixedStartCols;
            for (var i = 0; i < cols.length; i++) {
                if (fixStart == i)
                    x = 0;
                w = cols[i].width;
                rule = this.getColumnCssRules(i);
                rule[this.xLeft].style[this.xLeft] = x + "px";
                rule[this.xRight].style[this.xRight] = (((fixStart > 0 && i >= fixStart) ? this.canvasWidthR : this.canvasWidthL) - x - w) + "px";
                x += w;
            }
        }

        setSortColumn(columnId: string, ascending: boolean) {
            this.setSortColumns([{ columnId: columnId, sortAsc: ascending }]);
        }

        setSortColumns(cols: ColumnSort[]) {
            this.sortColumns = cols || [];

            var headerColumnEls = this.$headers.children();
            headerColumnEls
                .removeClass("slick-header-column-sorted")
                .find(".slick-sort-indicator")
                .removeClass("slick-sort-indicator-asc slick-sort-indicator-desc");

            $.each(this.sortColumns, (_, col) => {
                if (col.sortAsc == null) {
                    col.sortAsc = true;
                }
                var columnIndex = this.getColumnIndex(col.columnId);
                if (columnIndex != null) {
                    headerColumnEls.eq(columnIndex)
                        .addClass("slick-header-column-sorted")
                        .find(".slick-sort-indicator")
                        .addClass(col.sortAsc ? "slick-sort-indicator-asc" : "slick-sort-indicator-desc");
                }
            });
        }

        getSortColumns(): ColumnSort[] {
            return this.sortColumns;
        }

        private handleSelectedRangesChanged = (e: IEventData, ranges: Range[]): void => {
            this.selectedRows = [];
            var hash = {}, cols = this._columns;
            for (var i = 0; i < ranges.length; i++) {
                for (var j = ranges[i].fromRow; j <= ranges[i].toRow; j++) {
                    if (!hash[j]) {  // prevent duplicates
                        this.selectedRows.push(j);
                        hash[j] = {};
                    }
                    for (var k = ranges[i].fromCell; k <= ranges[i].toCell; k++) {
                        if (this.canCellBeSelected(j, k)) {
                            hash[j][cols[k].id] = this._options.selectedCellCssClass;
                        }
                    }
                }
            }

            this.setCellCssStyles(this._options.selectedCellCssClass, hash);

            this.trigger(this.onSelectedRowsChanged, { rows: this.getSelectedRows() }, e);
        }

        getColumns(): Column<TItem>[] {
            return this._columns;
        }

        getViewColumns(): Column<TItem>[] {
            return this._viewCols;
        }

        private updateViewColLeftRight(): void {
            this._viewColLeft = [];
            this._viewColRight = [];
            var x = 0, r: number, cols = this._viewCols, i: number, l: number = cols.length, fixStart = this._fixedStartCols;
            for (var i = 0; i < l; i++) {
                if (fixStart === i)
                    x = 0;
                r = x + cols[i].width;
                this._viewColLeft[i] = x;
                this._viewColRight[i] = r;
                x = r;
            }
        }

        private setColumnsInternal(columns: Column[]) {

            var defs = this._colDefaults;
            var columnsById = {};
            var fixedStartCols: Column[] = [];
            var viewCols: Column[] = [];
            var viewColById: { [key: string]: number } = {};
            var i: number, m: Column, k: string;
            for (i = 0; i < columns.length; i++) {
                m = columns[i];
                
                for (k in defs) {
                    if (m[k] === undefined)
                        m[k] = this._colDefaults[k];
                }

                if (m.minWidth && m.width < m.minWidth) {
                    m.width = m.minWidth;
                }

                if (m.maxWidth && m.width > m.maxWidth) {
                    m.width = m.maxWidth;
                }

                columnsById[m.id] = i;

                if (m.visible !== false) {
                    (m.fixedTo === "start" ? fixedStartCols : viewCols).push(m);
                }
            }

            this._fixedStartCols = fixedStartCols.length;
            if (fixedStartCols.length > 0)
                viewCols = fixedStartCols.concat(viewCols);

            for (i = 0; i < viewCols.length; i++) {
                m = viewCols[i];
                viewColById[m.id] = i;
            }

            this._columns = columns;
            this._columnById = columnsById;            
            this._viewCols = viewCols;
            this._viewColById = viewColById;
        }

        setColumns(columns: Column<TItem>[]): void {
            this.setColumnsInternal(columns);
            this.updateViewColLeftRight();

            if (this.initialized) {
                this.setPaneVisibility();
                this.setOverflow();

                this.invalidateAllRows();
                this.createColumnHeaders();
                this.createColumnFooter();
                this.updateFooterTotals();
                this.removeCssRules();
                this.createCssRules();
                this.resizeCanvas();
                this.updateCanvasWidth();
                this.applyColumnWidths();
                this.handleScroll();
            }
        }

        getOptions(): GridOptions<TItem> {
            return this._options;
        }

        setOptions(args: GridOptions<TItem>): void {
            if (!this.getEditorLock().commitCurrentEdit()) {
                return;
            }

            this.makeActiveCellNormal();

            if (this._options.enableAddRow !== args.enableAddRow) {
                this.invalidateRow(this.getDataLength());
            }

            this._options = $.extend(this._options, args);
            this.validateAndEnforceOptions();

            this.adjustFrozenRowOption();
            this.setScroller();

            if (args.columns) {
                adjustFrozenColumnCompat(args.columns, this._options);
                this.setColumns(args.columns);
            }

            this.render();
        }

        private validateAndEnforceOptions(): void {
            if (this._options.autoHeight) {
                this._options.leaveSpaceForNewRows = false;
            }
        }

        private viewOnRowCountChanged = () => {
            this.updateRowCount();
            this.render();
        }

        private viewOnRowsChanged = (_: any, args: { rows: number[] }): void => {
            this.invalidateRows(args.rows);
            this.render();
            this.updateFooterTotals();
        }

        private viewOnDataChanged = (): void => {
            this.invalidate();
            this.render();
        }

        private bindToData(): void {
            if (this._data) {
                this._data.onRowCountChanged && this._data.onRowCountChanged.subscribe(this.viewOnRowCountChanged);
                this._data.onRowsChanged && this._data.onRowsChanged.subscribe(this.viewOnRowsChanged);
                this._data.onDataChanged && this._data.onDataChanged.subscribe(this.viewOnDataChanged);
            }
        }

        private unbindFromData(): void {
            if (this._data) {
                this._data.onRowCountChanged && this._data.onRowCountChanged.unsubscribe(this.viewOnRowCountChanged);
                this._data.onRowsChanged && this._data.onRowsChanged.unsubscribe(this.viewOnRowsChanged);
                this._data.onDataChanged && this._data.onDataChanged.unsubscribe(this.viewOnDataChanged);
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
            if (this._data.getLength) {
                return this._data.getLength();
            } else {
                return this._data.length;
            }
        }

        getDataLengthIncludingAddNew(): number {
            return this.getDataLength() + (this._options.enableAddRow ? 1 : 0);
        }

        getDataItem(i: number): TItem {
            if (this._data.getItem) {
                return this._data.getItem(i);
            } else {
                return this._data[i];
            }
        }

        getTopPanel(): HTMLDivElement {
            return this.$topPanel[0] as HTMLDivElement;
        }

        setTopPanelVisibility(visible: boolean): void {
            if (this._options.showTopPanel != visible) {
                this._options.showTopPanel = visible;
                if (visible) {
                    this.$topPanelScroller.slideDown("fast", this.resizeCanvas);
                } else {
                    this.$topPanelScroller.slideUp("fast", this.resizeCanvas);
                }
            }
        }

        setHeaderRowVisibility(visible: boolean): void {
            if (this._options.showHeaderRow != visible) {
                this._options.showHeaderRow = visible;
                if (visible) {
                    this.$headerRowScroller.slideDown("fast", this.resizeCanvas);
                } else {
                    this.$headerRowScroller.slideUp("fast", this.resizeCanvas);
                }
            }
        }

        setFooterRowVisibility(visible: boolean): void {
            if (this._options.showFooterRow != visible) {
                this._options.showFooterRow = visible;
                if (visible) {
                    this.$footerRowScroller.fadeIn("fast", this.resizeCanvas);
                } else {
                    this.$footerRowScroller.slideDown("fast", this.resizeCanvas);
                }
            }
        }

        setGroupingPanelVisibility(visible: boolean): void {
            if (this._options.showGroupingPanel != visible) {
                this._options.showGroupingPanel = visible;
                if (visible) {
                    this.$groupingPanel.slideDown("fast", this.resizeCanvas);
                } else {
                    this.$groupingPanel.slideUp("fast", this.resizeCanvas);
                }
            }
        }

        getContainerNode(): HTMLDivElement {
            return this.$container[0] as HTMLDivElement
        }

        getUID(): string {
            return this.uid;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        // Rendering / Scrolling

        private getRowTop(row: number): number {
            return this._options.rowHeight * row - this.offset;
        }

        private getRowFromPosition(y: number): number {
            return Math.floor((y + this.offset) / this._options.rowHeight);
        }

        private scrollTo(y: number): void {
            y = Math.max(y, 0);
            y = Math.min(y, this.th - Math.round(this.$viewportScrollContainerY.height()) + ((this.viewportHasHScroll || this.hasFixedColumns()) ? scrollbarDimensions.height : 0));

            var oldOffset = this.offset;

            this.page = Math.min(this.n - 1, Math.floor(y / this.ph));
            this.offset = Math.round(this.page * this.cj);
            var newScrollTop = y - this.offset;

            if (this.offset != oldOffset) {
                var range = this.getVisibleRange(newScrollTop);
                this.cleanupRows(range);
                this.updateRowPositions();
            }

            if (this.prevScrollTop != newScrollTop) {
                this.vScrollDir = (this.prevScrollTop + oldOffset < newScrollTop + this.offset) ? 1 : -1;

                this.lastRenderedScrollTop = (this.scrollTop = this.prevScrollTop = newScrollTop);

                if (this.hasFixedColumns()) {
                    this.$viewportTopL[0].scrollTop = newScrollTop;
                }

                if (this.hasFrozenRows) {
                    this.$viewportBottomL[0].scrollTop = this.$viewportBottomR[0].scrollTop = newScrollTop;
                }

                this.$viewportScrollContainerY[0].scrollTop = newScrollTop;

                this.trigger(this.onViewportChanged);
            }
        }

        private getFormatter(row: number, column: Column<TItem>): ColumnFormatter<TItem> {
            var itemMetadata = this._data.getItemMetadata && this._data.getItemMetadata(row) as ItemMetadata;
            var colsMetadata = itemMetadata && itemMetadata.columns;

            // look up by id, then index
            var colMetadata = colsMetadata && (colsMetadata[column.id] || colsMetadata[this.getColumnIndex(column.id)]);

            return (colMetadata && colMetadata.formatter) ||
                (itemMetadata && itemMetadata.formatter) ||
                column.formatter ||
                (this._options.formatterFactory && this._options.formatterFactory.getFormatter(column)) ||
                this._options.defaultFormatter;
        }

        private callFormatter(row: number, cell: number, value: any, m: Column<TItem>, item: TItem): string {

            var result: string;

            // pass metadata to formatter
            var itemMetadata = this._data.getItemMetadata && this._data.getItemMetadata(row) as ItemMetadata;
            var colsMetadata = itemMetadata && itemMetadata.columns;

            if (colsMetadata) {
                var columnData = colsMetadata[m.id] || colsMetadata[cell];
                result = this.getFormatter(row, m)(row, cell, value, m, item, this, columnData);
            }
            else {
                result = this.getFormatter(row, m)(row, cell, value, m, item, this);
            }

            return result;
        }

        private getEditor(row: number, cell: number): Editor {
            var column = this._columns[cell];
            var rowMetadata = this._data.getItemMetadata && this._data.getItemMetadata(row);
            var columnMetadata = rowMetadata && rowMetadata.columns;

            if (columnMetadata && columnMetadata[column.id] && columnMetadata[column.id].editor !== undefined) {
                return columnMetadata[column.id].editor;
            }
            if (columnMetadata && columnMetadata[cell] && columnMetadata[cell].editor !== undefined) {
                return columnMetadata[cell].editor;
            }

            return column.editor || (this._options.editorFactory && this._options.editorFactory.getEditor(column));
        }

        private getDataItemValueForColumn(item: TItem, columnDef: Column<TItem>): any {
            if (this._options.dataItemColumnValueExtractor) {
                return this._options.dataItemColumnValueExtractor(item, columnDef);
            }
            return item[columnDef.field];
        }

        private appendRowHtml(stringArrayL: string[], stringArrayR: string[], row: number, range: ViewRange, dataLength: number): void {
            var d = this.getDataItem(row);
            var dataLoading = row < dataLength && !d;
            var rowCss = "slick-row" +
                (this.hasFrozenRows && row <= this._options.frozenRow ? ' frozen' : '') +
                (dataLoading ? " loading" : "") +
                (row === this.activeRow ? " active" : "") +
                (row % 2 == 1 ? " odd" : " even");

            if (!d) {
                rowCss += " " + this._options.addNewRowCssClass;
            }

            var metadata = this._data.getItemMetadata && this._data.getItemMetadata(row);

            if (metadata && metadata.cssClasses) {
                rowCss += " " + metadata.cssClasses;
            }

            var frozenRowOffset = this.getFrozenRowOffset(row);

            var rowHtml = "<div class='ui-widget-content " + rowCss + "' style='top:"
                + (this.getRowTop(row) - frozenRowOffset)
                + "px'>";

            stringArrayL.push(rowHtml);

            if (this.hasFixedColumns()) {
                stringArrayR.push(rowHtml);
            }

            var colspan, m, cols = this._columns, fixStart = this._fixedStartCols;
            for (var i = 0, ii = cols.length; i < ii; i++) {
                var columnData = null;
                m = cols[i];
                colspan = 1;
                if (metadata && metadata.columns) {
                    columnData = metadata.columns[m.id] || metadata.columns[i];
                    colspan = (columnData && columnData.colspan) || 1;
                    if (colspan === "*") {
                        colspan = ii - i;
                    }
                }

                // Do not render cells outside of the viewport.
                if (this._viewColRight[Math.min(ii - 1, i + colspan - 1)] > range.leftPx) {
                    if (this._viewColLeft[i] > range.rightPx) {
                        // All columns to the right are outside the range.
                        break;
                    }

                    this.appendCellHtml(fixStart > 0 && i >= fixStart ? stringArrayR : stringArrayL, row, i, colspan, d, columnData);
                }

                if (colspan > 1) {
                    i += (colspan - 1);
                }
            }

            stringArrayL.push("</div>");

            if (this.hasFixedColumns()) {
                stringArrayR.push("</div>");
            }
        }

        private appendCellHtml(stringArray: string[], row: number, cell: number, colspan: number, item: TItem, metadata: any): void {
            var cols = this._columns, fixStart = this._fixedStartCols, m = cols[cell];
            var cellCss = "slick-cell l" + cell + " r" + Math.min(cols.length - 1, cell + colspan - 1) +
                (m.cssClass ? " " + m.cssClass : "");

            if (cell < fixStart)
                cellCss += ' frozen';

            if (row === this.activeRow && cell === this.activeCell)
                cellCss += " active";

            if (metadata && metadata.cssClasses) {
                cellCss += " " + metadata.cssClasses;
            }

            // TODO:  merge them together in the setter
            for (var key in this.cellCssClasses) {
                if (this.cellCssClasses[key][row] && this.cellCssClasses[key][row][m.id]) {
                    cellCss += (" " + this.cellCssClasses[key][row][m.id]);
                }
            }

            stringArray.push("<div class='" + cellCss + "'>");

            // if there is a corresponding row (if not, this is the Add New row or this data hasn't been loaded yet)
            if (item) {
                var value = this.getDataItemValueForColumn(item, m);
                stringArray.push(this.callFormatter(row, cell, value, m, item));
            }

            stringArray.push("</div>");

            this.rowsCache[row].cellRenderQueue.push(cell);
            this.rowsCache[row].cellColSpans[cell] = colspan;
        }

        private cleanupRows(rangeToKeep: ViewRange): void {
            var i: number;
            for (var x in this.rowsCache) {
                var removeFrozenRow = true;
                i = parseInt(x, 10);
                if (this.hasFrozenRows
                    && ((this._options.frozenBottom && i >= this.actualFrozenRow) // Frozen bottom rows
                        || (!this._options.frozenBottom && i <= this.actualFrozenRow) // Frozen top rows
                    )
                ) {
                    removeFrozenRow = false;
                }

                if (i !== this.activeRow
                    && (i < rangeToKeep.top || i > rangeToKeep.bottom)
                    && (removeFrozenRow)) {
                    this.removeRowFromCache(i);
                }
            }

            this._options.enableAsyncPostCleanup && this.startPostProcessingCleanup();
        }

        invalidate(): void {
            this.updateRowCount();
            this.invalidateAllRows();
            this.render();
            this.updateFooterTotals();
        }

        invalidateAllRows(): void {
            if (this.currentEditor) {
                this.makeActiveCellNormal();
            }
            for (var row in this.rowsCache) {
                this.removeRowFromCache(parseInt(row, 10));
            }

            this._options.enableAsyncPostCleanup && this.startPostProcessingCleanup();
        }

        private queuePostProcessedRowForCleanup(cacheEntry: CachedRow, postProcessedRow: { [ key: number ]: any }, rowIdx: number): void {
            this.postProcessgroupId++;

            // store and detach node for later async cleanup
            for (var x in postProcessedRow) {
                if (postProcessedRow.hasOwnProperty(x)) {
                    var columnIdx = parseInt(x, 10);
                    this.postProcessedCleanupQueue.push({
                        groupId: this.postProcessgroupId,
                        cellNode: cacheEntry.cellNodesByColumnIdx[columnIdx | 0],
                        columnIdx: columnIdx | 0,
                        rowIdx: rowIdx
                    });
                }
            }
            this.postProcessedCleanupQueue.push({
                groupId: this.postProcessgroupId,
                rowNode: cacheEntry.rowNode
            });
            $(cacheEntry.rowNode).detach();
        }

        private queuePostProcessedCellForCleanup(cellnode: HTMLElement, columnIdx: number, rowIdx: number): void {
            this.postProcessedCleanupQueue.push({
                groupId: this.postProcessgroupId,
                cellNode: cellnode,
                columnIdx: columnIdx,
                rowIdx: rowIdx
            });
            $(cellnode).detach();
        }

        private removeRowFromCache(row: number): void {
            var cacheEntry = this.rowsCache[row];
            if (!cacheEntry) {
                return;
            }

            if (this._options.enableAsyncPostCleanup && this.postProcessedRows[row]) {
                this.queuePostProcessedRowForCleanup(cacheEntry, this.postProcessedRows[row], row);
            }
            else {
                cacheEntry.rowNode.each(function () {
                    if (this.parentElement)
                        this.parentElement.removeChild(this);
                });
            }

            delete this.rowsCache[row];
            delete this.postProcessedRows[row];
        }

        invalidateRows(rows: number[]): void {
            var i, rl;
            if (!rows || !rows.length) {
                return;
            }
            this.vScrollDir = 0;
            for (i = 0, rl = rows.length; i < rl; i++) {
                if (this.currentEditor && this.activeRow === rows[i]) {
                    this.makeActiveCellNormal();
                }
                if (this.rowsCache[rows[i]]) {
                    this.removeRowFromCache(rows[i]);
                }
            }

            this._options.enableAsyncPostCleanup && this.startPostProcessingCleanup();
        }

        invalidateRow(row: number): void {
            this.invalidateRows([row]);
        }

        updateCell(row: number, cell: number): void {
            var cellNode = this.getCellNode(row, cell);
            if (!cellNode) {
                return;
            }

            var m = this._columns[cell], d = this.getDataItem(row);
            if (this.currentEditor && this.activeRow === row && this.activeCell === cell) {
                this.currentEditor.loadValue(d);
            } else {
                cellNode.innerHTML = d ? this.callFormatter(row, cell, this.getDataItemValueForColumn(d, m), m, d) : "";
                this.invalidatePostProcessingResults(row);
            }
        }

        updateRow(row: number): void {
            var cacheEntry = this.rowsCache[row];
            if (!cacheEntry) {
                return;
            }

            this.ensureCellNodesInRowsCache(row);

            var d = this.getDataItem(row);

            for (var x in cacheEntry.cellNodesByColumnIdx) {
                if (!cacheEntry.cellNodesByColumnIdx.hasOwnProperty(x)) {
                    continue;
                }

                var columnIdx = parseInt(x, 10);
                var m = this._columns[columnIdx],
                    node = cacheEntry.cellNodesByColumnIdx[columnIdx];

                if (row === this.activeRow && columnIdx === this.activeCell && this.currentEditor) {
                    this.currentEditor.loadValue(d);
                } else if (d) {
                    node.innerHTML = this.callFormatter(row, columnIdx, this.getDataItemValueForColumn(d, m), m, d);
                } else {
                    node.innerHTML = "";
                }
            }

            this.invalidatePostProcessingResults(row);
        }

        private calcViewportHeight(): void {
            if (this._options.autoHeight) {
                this.viewportH = this._options.rowHeight
                    * this.getDataLengthIncludingAddNew()
                    + (this.hasFixedColumns() ? 0 : this.$headers.outerHeight());
            } else {
                this.topPanelH = (this._options.showTopPanel)
                    ? this._options.topPanelHeight + this.getVBoxDelta(this.$topPanelScroller)
                    : 0;

                this.groupPanelH = (this._options.groupingPanel && this._options.showGroupingPanel)
                    ? this._options.groupingPanelHeight + this.getVBoxDelta(this.$groupingPanel) : 0;

                this.headerRowH = (this._options.showHeaderRow)
                    ? this._options.headerRowHeight + this.getVBoxDelta(this.$headerRowScroller)
                    : 0;

                this.footerRowH = (this._options.showFooterRow)
                    ? this._options.footerRowHeight + this.getVBoxDelta(this.$footerRowScroller)
                    : 0;

                this.headerH = parseFloat(($ as any).css(this.$headerScroller[0], "height")) + this.getVBoxDelta(this.$headerScroller);

                this.viewportH = parseFloat(($ as any).css(this.$container[0], "height", true))
                    - parseFloat(($ as any).css(this.$container[0], "paddingTop", true))
                    - parseFloat(($ as any).css(this.$container[0], "paddingBottom", true))
                    - this.headerH
                    - this.topPanelH
                    - this.groupPanelH
                    - this.headerRowH
                    - this.footerRowH;
            }

            this.numVisibleRows = Math.ceil(this.viewportH / this._options.rowHeight);
        }

        private calcViewportWidth(): void {
            this.viewportW = parseFloat(($ as any).css(this.$container[0], "width", true));
        }

        private resizeCanvas(): void {
            if (!this.initialized) {
                return;
            }

            this.paneTopH = 0
            this.paneBottomH = 0
            this.viewportTopH = 0

            this.calcViewportWidth();
            this.calcViewportHeight();

            // Account for Frozen Rows
            if (this.hasFrozenRows) {
                if (this._options.frozenBottom) {
                    this.paneTopH = this.viewportH - this.frozenRowsHeight - scrollbarDimensions.height;

                    this.paneBottomH = this.frozenRowsHeight + scrollbarDimensions.height;
                } else {
                    this.paneTopH = this.frozenRowsHeight;
                    this.paneBottomH = this.viewportH - this.frozenRowsHeight;
                }
            } else {
                this.paneTopH = this.viewportH;
            }

            // The top pane includes the top panel and the header row
            this.paneTopH += this.topPanelH + this.headerRowH + this.footerRowH;

            if (this.hasFixedColumns() && this._options.autoHeight) {
                this.paneTopH += scrollbarDimensions.height;
            }

            // The top viewport does not contain the top panel or header row
            this.viewportTopH = this.paneTopH - this.topPanelH - this.headerRowH - this.footerRowH;

            if (this._options.autoHeight) {
                if (this.hasFixedColumns()) {
                    this.$container.height(
                        this.paneTopH
                        + parseFloat(($ as any).css(this.$headerScrollerL[0], "height"))
                    );
                }

                this.$paneTopL.css({
                    'position': 'relative',
                    'top': this.headerH,
                    'height': this.paneTopH
                });
            }
            else {
                this.$paneTopL.css({
                    'position': '',
                    'top': this.headerH + this.groupPanelH,
                    'height': this.paneTopH
                });
            }

            var paneBottomTop = this.$paneTopL.position().top
                + this.paneTopH;

            this.$viewportTopL.height(this.viewportTopH);

            if (this.hasFixedColumns()) {
                this.$paneTopR.css({
                    'top': this.$paneTopL.css('top'), 'height': this.paneTopH
                });

                this.$viewportTopR.height(this.viewportTopH);

                if (this.hasFrozenRows) {
                    this.$paneBottomL.css({
                        'top': paneBottomTop, 'height': this.paneBottomH
                    });

                    this.$paneBottomR.css({
                        'top': paneBottomTop, 'height': this.paneBottomH
                    });

                    this.$viewportBottomR.height(this.paneBottomH);
                }
            } else {
                if (this.hasFrozenRows) {
                    this.$paneBottomL.css({
                        'width': '100%', 'height': this.paneBottomH
                    });

                    this.$paneBottomL.css('top', paneBottomTop);
                }
            }

            if (this.hasFrozenRows) {
                this.$viewportBottomL.height(this.paneBottomH);

                if (this._options.frozenBottom) {
                    this.$canvasBottomL.height(this.frozenRowsHeight);

                    if (this.hasFixedColumns()) {
                        this.$canvasBottomR.height(this.frozenRowsHeight);
                    }
                } else {
                    this.$canvasTopL.height(this.frozenRowsHeight);

                    if (this.hasFixedColumns()) {
                        this.$canvasTopR.height(this.frozenRowsHeight);
                    }
                }
            } else {
                this.$viewportTopR.height(this.viewportTopH);
            }

            if (this._options.forceFitColumns) {
                this.autosizeColumns();
            }

            this.updateRowCount();
            this.handleScroll();
            // Since the width has changed, force the render() to reevaluate virtually rendered cells.
            this.lastRenderedScrollLeft = -1;
            this.render();
        }

        private updateRowCount(): void {
            if (!this.initialized) {
                 return; 
            }

            var dataLengthIncludingAddNew = this.getDataLengthIncludingAddNew();
            var numberOfRows = 0;
            var oldH = (this.hasFrozenRows && !this._options.frozenBottom) ? Math.round(this.$canvasBottomL.height()) : Math.round(this.$canvasTopL.height());

            if (this.hasFrozenRows) {
                var numberOfRows = this.getDataLength() - this._options.frozenRow;
            } else {
                var numberOfRows = dataLengthIncludingAddNew + (this._options.leaveSpaceForNewRows ? this.numVisibleRows - 1 : 0);
            }

            var tempViewportH = Math.round(this.$viewportScrollContainerY.height());
            var oldViewportHasVScroll = this.viewportHasVScroll;
            // with autoHeight, we do not need to accommodate the vertical scroll bar
            this.viewportHasVScroll = !this._options.autoHeight && (numberOfRows * this._options.rowHeight > tempViewportH);

            this.makeActiveCellNormal();

            // remove the rows that are now outside of the data range
            // this helps avoid redundant calls to .removeRow() when the size of the data decreased by thousands of rows
            var l = dataLengthIncludingAddNew - 1;
            for (var x in this.rowsCache) {
                var i = parseInt(x, 10);
                if (i >= l) {
                    this.removeRowFromCache(i);
                }
            }

            this._options.enableAsyncPostCleanup && this.startPostProcessingCleanup();

            this.th = Math.max(this._options.rowHeight * numberOfRows, tempViewportH - scrollbarDimensions.height);

            if (this.activeCellNode && this.activeRow > l) {
                this.resetActiveCell();
            }

            if (this.th < maxSupportedCssHeight) {
                // just one page
                this.h = this.ph = this.th;
                this.n = 1;
                this.cj = 0;
            } else {
                // break into pages
                this.h = maxSupportedCssHeight;
                this.ph = this.h / 100;
                this.n = Math.floor(this.th / this.ph);
                this.cj = (this.th - this.h) / (this.n - 1);
            }

            if (this.h !== oldH) {
                if (this.hasFrozenRows && !this._options.frozenBottom) {
                    this.$canvasBottomL.css("height", this.h);

                    if (this.hasFixedColumns()) {
                        this.$canvasBottomR.css("height", this.h);
                    }
                } else {
                    this.$canvasTopL.css("height", this.h);
                    this.$canvasTopR.css("height", this.h);
                }

                this.scrollTop = this.$viewportScrollContainerY[0].scrollTop;
            }

            var oldScrollTopInRange = (this.scrollTop + this.offset <= this.th - tempViewportH);

            if (this.th == 0 || this.scrollTop == 0) {
                this.page = this.offset = 0;
            } else if (oldScrollTopInRange) {
                // maintain virtual position
                this.scrollTo(this.scrollTop + this.offset);
            } else {
                // scroll to bottom
                this.scrollTo(this.th - tempViewportH);
            }

            if (this.h != oldH && this._options.autoHeight) {
                this.resizeCanvas();
            }

            if (this._options.forceFitColumns && oldViewportHasVScroll != this.viewportHasVScroll) {
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
                viewportTop = this.scrollTop;
            }
            if (viewportLeft == null) {
                viewportLeft = this.scrollLeft;
            }

            return {
                top: this.getRowFromPosition(viewportTop),
                bottom: this.getRowFromPosition(viewportTop + this.viewportH) + 1,
                leftPx: viewportLeft,
                rightPx: viewportLeft + this.viewportW
            };
        }

        getRenderedRange(viewportTop?: number, viewportLeft?: number): ViewRange {
            var range = this.getVisibleRange(viewportTop, viewportLeft);
            var buffer = Math.round(this.viewportH / this._options.rowHeight);
            var minBuffer = this._options.minBuffer || 3;

            if (this.vScrollDir == -1) {
                range.top -= buffer;
                range.bottom += minBuffer;
            } else if (this.vScrollDir == 1) {
                range.top -= minBuffer;
                range.bottom += buffer;
            } else {
                range.top -= minBuffer;
                range.bottom += minBuffer;
            }

            range.top = Math.max(0, range.top);
            range.bottom = Math.min(this.getDataLengthIncludingAddNew() - 1, range.bottom);

            if (this._options.renderAllCells) {
                range.leftPx = 0;
                range.rightPx = this.canvasWidth;
            }
            else {
                range.leftPx -= this.viewportW;
                range.rightPx += this.viewportW;

                range.leftPx = Math.max(0, range.leftPx);
                range.rightPx = Math.min(this.canvasWidth, range.rightPx);
            }

            return range;
        }

        private ensureCellNodesInRowsCache(row: number): void {
            var cacheEntry = this.rowsCache[row];
            if (cacheEntry) {
                if (cacheEntry.cellRenderQueue.length) {
                    var lastChild = cacheEntry.rowNode.children().last()[0];
                    while (cacheEntry.cellRenderQueue.length) {
                        var columnIdx = cacheEntry.cellRenderQueue.pop();

                        cacheEntry.cellNodesByColumnIdx[columnIdx] = lastChild;
                        lastChild = lastChild.previousSibling as HTMLElement;

                        // Hack to retrieve the frozen columns because
                        if (lastChild == null)
                            lastChild = cacheEntry.rowNode.children().last()[0];
                    }
                }
            }
        }

        private cleanUpCells(range: ViewRange, row: number): void {
            // Ignore frozen rows
            if (this.hasFrozenRows
                && ((this._options.frozenBottom && row > this.actualFrozenRow) // Frozen bottom rows
                    || (row <= this.actualFrozenRow)  // Frozen top rows
                )
            ) {
                return;
            }

            var totalCellsRemoved = 0;
            var cacheEntry = this.rowsCache[row];

            // Remove cells outside the range.
            var cellsToRemove = [], fixStart = this._fixedStartCols;
            for (var x in cacheEntry.cellNodesByColumnIdx) {
                // I really hate it when people mess with Array.prototype.
                if (!cacheEntry.cellNodesByColumnIdx.hasOwnProperty(x)) {
                    continue;
                }

                var i = parseInt(x, 10);

                // Ignore frozen columns
                if (i < fixStart) {
                    continue;
                }

                var colspan = cacheEntry.cellColSpans[i], cols = this._columns;
                if (this._viewColLeft[i] > range.rightPx || this._viewColRight[Math.min(cols.length - 1, i + colspan - 1)] < range.leftPx) {
                    if (!(row == this.activeRow && i === this.activeCell)) {
                        cellsToRemove.push(i);
                    }
                }
            }

            var cellToRemove, node;
            this.postProcessgroupId++;
            while ((cellToRemove = cellsToRemove.pop()) != null) {
                node = cacheEntry.cellNodesByColumnIdx[cellToRemove];

                if (this._options.enableAsyncPostCleanup && this.postProcessedRows[row] && this.postProcessedRows[row][cellToRemove]) {
                    this.queuePostProcessedCellForCleanup(node, cellToRemove, row);
                } else {
                    node.parentElement.removeChild(node);
                }

                delete cacheEntry.cellColSpans[cellToRemove];
                delete cacheEntry.cellNodesByColumnIdx[cellToRemove];
                if (this.postProcessedRows[row]) {
                    delete this.postProcessedRows[row][cellToRemove];
                }
                totalCellsRemoved++;
            }
        }

        private cleanUpAndRenderCells(range: ViewRange) {
            var cacheEntry;
            var stringArray: string[] = [];
            var processedRows = [];
            var cellsAdded;
            var totalCellsAdded = 0;
            var colspan;
            var cols = this._columns;

            for (var row = range.top, btm = range.bottom; row <= btm; row++) {
                cacheEntry = this.rowsCache[row];
                if (!cacheEntry) {
                    continue;
                }

                // cellRenderQueue populated in renderRows() needs to be cleared first
                this.ensureCellNodesInRowsCache(row);

                this.cleanUpCells(range, row);

                // Render missing cells.
                cellsAdded = 0;

                var metadata = this._data.getItemMetadata && this._data.getItemMetadata(row);
                metadata = metadata && metadata.columns;

                var d = this.getDataItem(row);

                // TODO:  shorten this loop (index? heuristics? binary search?)
                for (var i = 0, ii = cols.length; i < ii; i++) {
                    // Cells to the right are outside the range.
                    if (this._viewColLeft[i] > range.rightPx) {
                        break;
                    }

                    // Already rendered.
                    if ((colspan = cacheEntry.cellColSpans[i]) != null) {
                        i += (colspan > 1 ? colspan - 1 : 0);
                        continue;
                    }

                    var columnData = null;
                    colspan = 1;
                    if (metadata) {
                        columnData = metadata[cols[i].id] || metadata[i];
                        colspan = (columnData && columnData.colspan) || 1;
                        if (colspan === "*") {
                            colspan = ii - i;
                        }
                    }

                    if (this._viewColRight[Math.min(ii - 1, i + colspan - 1)] > range.leftPx) {
                        this.appendCellHtml(stringArray, row, i, colspan, d, columnData);
                        cellsAdded++;
                    }

                    i += (colspan > 1 ? colspan - 1 : 0);
                }

                if (cellsAdded) {
                    totalCellsAdded += cellsAdded;
                    processedRows.push(row);
                }
            }

            if (!stringArray.length) {
                return;
            }

            var x = document.createElement("div");
            x.innerHTML = stringArray.join("");

            var processedRow;
            var node: HTMLElement, fixStart = this._fixedStartCols;
            while ((processedRow = processedRows.pop()) != null) {
                cacheEntry = this.rowsCache[processedRow];
                var columnIdx;
                while ((columnIdx = cacheEntry.cellRenderQueue.pop()) != null) {
                    node = x.lastChild as HTMLElement;

                    if (fixStart > 0 && columnIdx >= fixStart) {
                        cacheEntry.rowNode[1].appendChild(node);
                    } else {
                        cacheEntry.rowNode[0].appendChild(node);
                    }

                    cacheEntry.cellNodesByColumnIdx[columnIdx] = node;
                }
            }
        }

        private renderRows(range: ViewRange): void {
            var stringArrayL: string[] = [],
                stringArrayR: string[] = [],
                rows = [],
                needToReselectCell = false,
                dataLength = this.getDataLength();

            for (var i = range.top, ii = range.bottom; i <= ii; i++) {
                if (this.rowsCache[i] || (this.hasFrozenRows && this._options.frozenBottom && i == dataLength)) {
                    continue;
                }
                rows.push(i);

                // Create an entry right away so that appendRowHtml() can
                // start populatating it.
                this.rowsCache[i] = {
                    rowNode: null,

                    // ColSpans of rendered cells (by column idx).
                    // Can also be used for checking whether a cell has been rendered.
                    cellColSpans: [],

                    // Cell nodes (by column idx).  Lazy-populated by ensureCellNodesInRowsCache().
                    cellNodesByColumnIdx: [],

                    // Column indices of cell nodes that have been rendered, but not yet indexed in
                    // cellNodesByColumnIdx.  These are in the same order as cell nodes added at the
                    // end of the row.
                    cellRenderQueue: []
                };

                this.appendRowHtml(stringArrayL, stringArrayR, i, range, dataLength);
                if (this.activeCellNode && this.activeRow === i) {
                    needToReselectCell = true;
                }
            }

            if (!rows.length) {
                return;
            }

            var x = document.createElement("div"),
                xRight = document.createElement("div");

            x.innerHTML = stringArrayL.join("");
            xRight.innerHTML = stringArrayR.join("");

            for (var i = 0, ii = rows.length; i < ii; i++) {
                if ((this.hasFrozenRows) && (rows[i] >= this.actualFrozenRow)) {
                    if (this.hasFixedColumns()) {
                        this.rowsCache[rows[i]].rowNode = $()
                            .add($(x.firstChild).appendTo(this.$canvasBottomL))
                            .add($(xRight.firstChild).appendTo(this.$canvasBottomR));
                    } else {
                        this.rowsCache[rows[i]].rowNode = $()
                            .add($(x.firstChild).appendTo(this.$canvasBottomL));
                    }
                } else if (this.hasFixedColumns()) {
                    this.rowsCache[rows[i]].rowNode = $()
                        .add($(x.firstChild).appendTo(this.$canvasTopL))
                        .add($(xRight.firstChild).appendTo(this.$canvasTopR));
                } else {
                    this.rowsCache[rows[i]].rowNode = $()
                        .add($(x.firstChild).appendTo(this.$canvasTopL));
                }
            }

            if (needToReselectCell) {
                this.activeCellNode = this.getCellNode(this.activeRow, this.activeCell);
            }
        }

        private startPostProcessing(): void {
            if (!this._options.enableAsyncPostRender) {
                return;
            }

            clearTimeout(this.h_postrender);

            if (this._options.asyncPostRenderDelay < 0) {
                this.asyncPostProcessRows();
            } else {
                this.h_postrender = setTimeout(this.asyncPostProcessRows.bind(this), this._options.asyncPostRenderDelay);
            }
        }

        private startPostProcessingCleanup(): void {
            if (!this._options.enableAsyncPostCleanup) {
                return;
            }

            clearTimeout(this.h_postrenderCleanup);

            if (this._options.asyncPostCleanupDelay < 0) {
                this.asyncPostProcessCleanupRows();
            }
            else {
                this.h_postrenderCleanup = setTimeout(this.asyncPostProcessCleanupRows.bind(this), this._options.asyncPostCleanupDelay);
            }
        }

        private invalidatePostProcessingResults(row: number): void {
            if (this._options.enableAsyncPostCleanup) {
                // change status of columns to be re-rendered
                for (var columnIdx in this.postProcessedRows[row]) {
                    if (this.postProcessedRows[row].hasOwnProperty(columnIdx)) {
                        this.postProcessedRows[row][columnIdx] = 'C';
                    }
                }
            }
            else {
                delete this.postProcessedRows[row];
            }

            this.postProcessFromRow = Math.min(this.postProcessFromRow, row);
            this.postProcessToRow = Math.max(this.postProcessToRow, row);
            this.startPostProcessing();
        }

        private updateRowPositions(): void {
            for (var row in this.rowsCache) {
                this.rowsCache[row].rowNode.css('top', this.getRowTop(parseInt(row, 10)) + "px");
            }
        }

        private updateFooterTotals(): void {
            if (!this._options.showFooterRow || !this.initialized)
                return;

            var totals = null;
            if (this._data.getGrandTotals) {
                totals = this._data.getGrandTotals();
            }

            var cols = this._columns;
            for (var i = 0; i < cols.length; i++) {
                var m = cols[i];

                var content;
                if (m.field && totals) {
                    content = (m.groupTotalsFormatter && m.groupTotalsFormatter(totals, m, this)) ||
                        (this.groupTotalsFormatter && this.groupTotalsFormatter(totals, m)) || "";
                }

                $(this.getFooterRowColumn(m.id)).html(content);
            }
        }

        private render(): void {
            if (!this.initialized) { return; }
            var visible = this.getVisibleRange();
            var rendered = this.getRenderedRange();

            // remove rows no longer in the viewport
            this.cleanupRows(rendered);

            // add new rows & missing cells in existing rows
            if (this.lastRenderedScrollLeft != this.scrollLeft) {

                if (this.hasFrozenRows) {

                    var renderedFrozenRows = jQuery.extend(true, {}, rendered);

                    if (this._options.frozenBottom) {

                        renderedFrozenRows.top = this.actualFrozenRow;
                        renderedFrozenRows.bottom = this.getDataLength();
                    }
                    else {

                        renderedFrozenRows.top = 0;
                        renderedFrozenRows.bottom = this._options.frozenRow;
                    }

                    this.cleanUpAndRenderCells(renderedFrozenRows);
                }

                this.cleanUpAndRenderCells(rendered);
            }

            // render missing rows
            this.renderRows(rendered);

            // Render frozen rows
            if (this.hasFrozenRows) {
                if (this._options.frozenBottom) {
                    this.renderRows({
                        top: this.actualFrozenRow, bottom: this.getDataLength() - 1, leftPx: rendered.leftPx, rightPx: rendered.rightPx
                    });
                }
                else {
                    this.renderRows({
                        top: 0, bottom: this._options.frozenRow - 1, leftPx: rendered.leftPx, rightPx: rendered.rightPx
                    });
                }
            }

            this.postProcessFromRow = visible.top;
            this.postProcessToRow = Math.min(this.getDataLengthIncludingAddNew() - 1, visible.bottom);
            this.startPostProcessing();

            this.lastRenderedScrollTop = this.scrollTop;
            this.lastRenderedScrollLeft = this.scrollLeft;
            this.h_render = null;
        }

        private handleHeaderRowScroll(): void {
            var scrollLeft = this.$headerRowScrollContainer[0].scrollLeft;
            if (scrollLeft != this.$viewportScrollContainerX[0].scrollLeft) {
                this.$viewportScrollContainerX[0].scrollLeft = scrollLeft;
            }
        }

        private handleFooterRowScroll(): void {
            var scrollLeft = this.$footerRowScrollContainer[0].scrollLeft;
            if (scrollLeft != this.$viewportScrollContainerX[0].scrollLeft) {
                this.$viewportScrollContainerX[0].scrollLeft = scrollLeft;
            }
        }

        private handleMouseWheel(e: JQueryEventObject, delta: number, deltaX: number, deltaY: number): void {
            deltaX = (typeof deltaX == "undefined" ? (e as any).originalEvent.deltaX : deltaX) || 0;
            deltaY = (typeof deltaY == "undefined" ? (e as any).originalEvent.deltaY : deltaY) || 0;
            this.scrollTop = Math.max(0, this.$viewportScrollContainerY[0].scrollTop - (deltaY * this._options.rowHeight));
            this.scrollLeft = this.$viewportScrollContainerX[0].scrollLeft + (deltaX * 10);
            var handled = this._handleScroll(true);
            if (handled)
                e.preventDefault();
        }

        private handleScroll(): boolean {
            this.scrollTop = this.$viewportScrollContainerY[0].scrollTop;
            this.scrollLeft = this.$viewportScrollContainerX[0].scrollLeft;
            return this._handleScroll(false);
        }

        private _handleScroll(isMouseWheel?: boolean): boolean {
            var maxScrollDistanceY = this.$viewportScrollContainerY[0].scrollHeight - this.$viewportScrollContainerY[0].clientHeight;
            var maxScrollDistanceX = this.$viewportScrollContainerY[0].scrollWidth - this.$viewportScrollContainerY[0].clientWidth;

            // Protect against erroneous clientHeight/Width greater than scrollHeight/Width.
            // Sometimes seen in Chrome.
            maxScrollDistanceY = Math.max(0, maxScrollDistanceY);
            maxScrollDistanceX = Math.max(0, maxScrollDistanceX);

            // Ceiling the max scroll values
            if (this.scrollTop > maxScrollDistanceY) {
                this.scrollTop = maxScrollDistanceY;
            }
            if (this.scrollLeft > maxScrollDistanceX) {
                this.scrollLeft = maxScrollDistanceX;
            }

            var vScrollDist = Math.abs(this.scrollTop - this.prevScrollTop);
            var hScrollDist = Math.abs(this.scrollLeft - this.prevScrollLeft);

            if (hScrollDist) {
                this.prevScrollLeft = this.scrollLeft;

                this.$viewportScrollContainerX[0].scrollLeft = this.scrollLeft;
                this.$headerScrollContainer[0].scrollLeft = this.scrollLeft;
                this.$topPanelScroller[0].scrollLeft = this.scrollLeft;
                this.$headerRowScrollContainer[0].scrollLeft = this.scrollLeft;
                this.$footerRowScrollContainer[0].scrollLeft = this.scrollLeft;

                if (this.hasFixedColumns()) {
                    if (this.hasFrozenRows) {
                        this.$viewportTopR[0].scrollLeft = this.scrollLeft;
                    }
                } else {
                    if (this.hasFrozenRows) {
                        this.$viewportTopL[0].scrollLeft = this.scrollLeft;
                    }
                }
            }

            if (vScrollDist) {
                this.vScrollDir = this.prevScrollTop < this.scrollTop ? 1 : -1;
                this.prevScrollTop = this.scrollTop;

                if (isMouseWheel) {
                    this.$viewportScrollContainerY[0].scrollTop = this.scrollTop;
                }

                if (this.hasFixedColumns()) {
                    if (this.hasFrozenRows && !this._options.frozenBottom) {
                        this.$viewportBottomL[0].scrollTop = this.scrollTop;
                    } else {
                        this.$viewportTopL[0].scrollTop = this.scrollTop;
                    }
                }

                // switch virtual pages if needed
                if (vScrollDist < this.viewportH) {
                    this.scrollTo(this.scrollTop + this.offset);
                } else {
                    var oldOffset = this.offset;
                    if (this.h == this.viewportH) {
                        this.page = 0;
                    } else {
                        this.page = Math.min(this.n - 1, Math.floor(this.scrollTop * ((this.th - this.viewportH) / (this.h - this.viewportH)) * (1 / this.ph)));
                    }
                    this.offset = Math.round(this.page * this.cj);
                    if (oldOffset != this.offset) {
                        this.invalidateAllRows();
                    }
                }
            }

            if (hScrollDist || vScrollDist) {
                if (this.h_render) {
                    clearTimeout(this.h_render);
                }

                if (Math.abs(this.lastRenderedScrollTop - this.scrollTop) > 20 ||
                    Math.abs(this.lastRenderedScrollLeft - this.scrollLeft) > 20) {
                    if (this._options.forceSyncScrolling || (
                        Math.abs(this.lastRenderedScrollTop - this.scrollTop) < this.viewportH &&
                        Math.abs(this.lastRenderedScrollLeft - this.scrollLeft) < this.viewportW)) {
                        this.render();
                    } else {
                        this.h_render = setTimeout(this.render.bind(this), 50);
                    }

                    this.trigger(this.onViewportChanged);
                }
            }

            this.trigger(this.onScroll, { scrollLeft: this.scrollLeft, scrollTop: this.scrollTop });

            return !!(hScrollDist || vScrollDist);
        }

        private asyncPostProcessRows(): void {
            var dataLength = this.getDataLength();
            var cols = this._columns;
            while (this.postProcessFromRow <= this.postProcessToRow) {
                var row = (this.vScrollDir >= 0) ? this.postProcessFromRow++ : this.postProcessToRow--;
                var cacheEntry = this.rowsCache[row];
                if (!cacheEntry || row >= dataLength) {
                    continue;
                }

                if (!this.postProcessedRows[row]) {
                    this.postProcessedRows[row] = {};
                }

                this.ensureCellNodesInRowsCache(row);
                for (var x in cacheEntry.cellNodesByColumnIdx) {
                    if (!cacheEntry.cellNodesByColumnIdx.hasOwnProperty(x)) {
                        continue;
                    }

                    var columnIdx = parseInt(x, 10);

                    var m = cols[columnIdx];
                    var processedStatus = this.postProcessedRows[row][columnIdx]; // C=cleanup and re-render, R=render
                    if (processedStatus !== 'R') {
                        if (m.asyncPostRender || m.asyncPostCleanup) {
                            var node = cacheEntry.cellNodesByColumnIdx[columnIdx];
                            if (node) {
                                if (m.asyncPostCleanup && processedStatus === 'C')
                                    m.asyncPostCleanup(node, row, m);
                                m.asyncPostRender && m.asyncPostRender(node, row, this.getDataItem(row), m);
                            }
                        }
                        this.postProcessedRows[row][columnIdx] = 'R';
                    }
                }

                if (this._options.asyncPostRenderDelay >= 0) {
                    this.h_postrender = setTimeout(this.asyncPostProcessRows.bind(this), this._options.asyncPostRenderDelay);
                    return;
                }
            }
        }

        private asyncPostProcessCleanupRows(): void {
            var cols = this._columns;
            while (this.postProcessedCleanupQueue.length > 0) {
                var groupId = this.postProcessedCleanupQueue[0].groupId;

                // loop through all queue members with this groupID
                while (this.postProcessedCleanupQueue.length > 0 && this.postProcessedCleanupQueue[0].groupId == groupId) {
                    var entry = this.postProcessedCleanupQueue.shift();
                    if (entry.rowNode != null) {
                        $(entry.rowNode).remove();
                    }
                    if (entry.cellNode != null) {
                        var column = cols[entry.columnIdx];
                        if (column && column.asyncPostCleanup) {
                            column.asyncPostCleanup(entry.cellNode, entry.rowIdx, column);
                            entry.cellNode.remove();
                        }
                    }
                }

                // call this function again after the specified delay
                if (this._options.asyncPostRenderDelay >= 0) {
                    this.h_postrenderCleanup = setTimeout(this.asyncPostProcessCleanupRows.bind(this), this._options.asyncPostCleanupDelay);
                    return;
                }
            }
        }

        private updateCellCssStylesOnRenderedRows(addedHash: CellStylesHash, removedHash: CellStylesHash) {
            var node, columnId, addedRowHash, removedRowHash;
            for (var row in this.rowsCache) {
                removedRowHash = removedHash && removedHash[row];
                addedRowHash = addedHash && addedHash[row];

                if (removedRowHash) {
                    for (columnId in removedRowHash) {
                        if (!addedRowHash || removedRowHash[columnId] != addedRowHash[columnId]) {
                            node = this.getCellNode(parseInt(row, 10), this.getColumnIndex(columnId));
                            if (node) {
                                $(node).removeClass(removedRowHash[columnId]);
                            }
                        }
                    }
                }

                if (addedRowHash) {
                    for (columnId in addedRowHash) {
                        if (!removedRowHash || removedRowHash[columnId] != addedRowHash[columnId]) {
                            node = this.getCellNode(parseInt(row, 10), this.getColumnIndex(columnId));
                            if (node) {
                                $(node).addClass(addedRowHash[columnId]);
                            }
                        }
                    }
                }
            }
        }

        addCellCssStyles(key: string, hash: CellStylesHash): void {
            if (this.cellCssClasses[key]) {
                throw "addCellCssStyles: cell CSS hash with key '" + key + "' already exists.";
            }

            this.cellCssClasses[key] = hash;
            this.updateCellCssStylesOnRenderedRows(hash, null);

            this.trigger(this.onCellCssStylesChanged, { key: key, hash: hash });
        }

        removeCellCssStyles(key: string): void {
            if (!this.cellCssClasses[key]) {
                return;
            }

            this.updateCellCssStylesOnRenderedRows(null, this.cellCssClasses[key]);
            delete this.cellCssClasses[key];

            this.trigger(this.onCellCssStylesChanged, { key: key, hash: null });
        }

        setCellCssStyles(key: string, hash: CellStylesHash): void {
            var prevHash = this.cellCssClasses[key];

            this.cellCssClasses[key] = hash;
            this.updateCellCssStylesOnRenderedRows(hash, prevHash);

            this.trigger(this.onCellCssStylesChanged, { key: key, hash: hash });
        }

        getCellCssStyles(key: string): CellStylesHash {
            return this.cellCssClasses[key];
        }

        flashCell(row: number, cell: number, speed?: number): void {
            speed = speed || 100;
            if (this.rowsCache[row]) {
                var $cell = $(this.getCellNode(row, cell));
                toggleCellClass(4);
            }

            function toggleCellClass(times: number) {
                if (!times) {
                    return;
                }
                setTimeout(function () {
                    $cell.queue(function () {
                        $cell.toggleClass(this.$options.cellFlashingCssClass).dequeue();
                        toggleCellClass(times - 1);
                    });
                }, speed);
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        // Interactivity

        private handleDragInit(e: JQueryEventObject, dd: any): boolean {
            var cell = this.getCellFromEvent(e);
            if (!cell || !this.cellExists(cell.row, cell.cell)) {
                return false;
            }

            var retval = this.trigger(this.onDragInit, dd, e);
            if (e.isImmediatePropagationStopped()) {
                return retval;
            }

            // if nobody claims to be handling drag'n'drop by stopping immediate propagation,
            // cancel out of it
            return false;
        }

        private handleDragStart(e: JQueryEventObject, dd: any): boolean {
            var cell = this.getCellFromEvent(e);
            if (!cell || !this.cellExists(cell.row, cell.cell)) {
                return false;
            }

            var retval = this.trigger(this.onDragStart, dd, e);
            if (e.isImmediatePropagationStopped()) {
                return retval;
            }

            return false;
        }

        private handleDrag(e: JQueryEventObject, dd: any): any {
            return this.trigger(this.onDrag, dd, e);
        }

        private handleDragEnd(e: JQueryEventObject, dd: any): void {
            this.trigger(this.onDragEnd, dd, e);
        }

        private handleKeyDown(e: JQueryKeyEventObject): void {
            this.trigger(this.onKeyDown, { row: this.activeRow, cell: this.activeCell }, e);
            var handled = e.isImmediatePropagationStopped();
            var keyCode = Slick.keyCode;

            if (!handled) {
                if (!e.shiftKey && !e.altKey && !e.ctrlKey) {
                    if (e.which == keyCode.ESCAPE) {
                        if (!this.getEditorLock().isActive()) {
                            return; // no editing mode to cancel, allow bubbling and default processing (exit without cancelling the event)
                        }
                        this.cancelEditAndSetFocus();
                    } else if (e.which == keyCode.PAGEDOWN) {
                        this.navigatePageDown();
                        handled = true;
                    } else if (e.which == keyCode.PAGEUP) {
                        this.navigatePageUp();
                        handled = true;
                    } else if (e.which == keyCode.LEFT) {
                        handled = this.navigateLeft();
                    } else if (e.which == keyCode.RIGHT) {
                        handled = this.navigateRight();
                    } else if (e.which == keyCode.UP) {
                        handled = this.navigateUp();
                    } else if (e.which == keyCode.DOWN) {
                        handled = this.navigateDown();
                    } else if (e.which == keyCode.TAB) {
                        handled = this.navigateNext();
                    } else if (e.which == keyCode.ENTER) {
                        if (this._options.editable) {
                            if (this.currentEditor) {
                                // adding new row
                                if (this.activeRow === this.getDataLength()) {
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
                } else if (e.which == keyCode.TAB && e.shiftKey && !e.ctrlKey && !e.altKey) {
                    handled = this.navigatePrev();
                }
            }

            if (handled) {
                // the event has been handled so don't let parent element (bubbling/propagation) or browser (default) handle it
                e.stopPropagation();
                e.preventDefault();
                try {
                    (e.originalEvent as JQueryKeyEventObject).keyCode = 0; // prevent default behaviour for special keys in IE browsers (F3, F5, etc.)
                }
                // ignore exceptions - setting the original event's keycode throws access denied exception for "Ctrl"
                // (hitting control key only, nothing else), "Shift" (maybe others)
                catch (error) {
                }
            }
        }

        private handleClick(e: JQueryMouseEventObject): void {
            if (!this.currentEditor) {
                // if this click resulted in some cell child node getting focus,
                // don't steal it back - keyboard events will still bubble up
                // IE9+ seems to default DIVs to tabIndex=0 instead of -1, so check for cell clicks directly.
                if (e.target != document.activeElement || $(e.target).hasClass("slick-cell")) {
                    this.setFocus();
                }
            }

            var cell = this.getCellFromEvent(e as any);
            if (!cell || (this.currentEditor !== null && this.activeRow == cell.row && this.activeCell == cell.cell)) {
                return;
            }

            this.trigger(this.onClick, { row: cell.row, cell: cell.cell }, e);
            if (e.isImmediatePropagationStopped()) {
                return;
            }

            if ((this.activeCell != cell.cell || this.activeRow != cell.row) && this.canCellBeActive(cell.row, cell.cell)) {
                if (!this.getEditorLock().isActive() || this.getEditorLock().commitCurrentEdit()) {

                    this.scrollRowIntoView(cell.row, false);
                    this.setActiveCellInternal(this.getCellNode(cell.row, cell.cell));
                }
            }
        }

        private handleContextMenu(e: JQueryMouseEventObject): void {
            var $cell = $(e.target).closest(".slick-cell", this.$canvas as any);
            if ($cell.length === 0) {
                return;
            }

            // are we editing this cell?
            if (this.activeCellNode === $cell[0] && this.currentEditor !== null) {
                return;
            }

            this.trigger(this.onContextMenu, {}, e);
        }

        private handleDblClick(e: JQueryMouseEventObject): void {
            var cell = this.getCellFromEvent(e as any);
            if (!cell || (this.currentEditor !== null && this.activeRow == cell.row && this.activeCell == cell.cell)) {
                return;
            }

            this.trigger(this.onDblClick, { row: cell.row, cell: cell.cell }, e);
            if (e.isImmediatePropagationStopped()) {
                return;
            }

            if (this._options.editable) {
                this.gotoCell(cell.row, cell.cell, true);
            }
        }

        private handleHeaderMouseEnter(e: JQueryMouseEventObject): void {
            this.trigger(this.onHeaderMouseEnter, {
                column: $(e.target).data("column")
            }, e);
        }

        private handleHeaderMouseLeave(e: JQueryMouseEventObject): void {
            this.trigger(this.onHeaderMouseLeave, {
                column: $(e.target).data("column")
            }, e);
        }

        private handleHeaderContextMenu(e: JQueryMouseEventObject): void {
            var $header = $(e.target).closest(".slick-header-column", ".slick-header-columns" as any);
            var column = $header && $header.data("column");
            this.trigger(this.onHeaderContextMenu, { column: column }, e);
        }

        private handleHeaderClick(e: JQueryMouseEventObject): void {
            var $header = $(e.target).closest(".slick-header-column", ".slick-header-columns" as any);
            var column = $header && $header.data("column");
            if (column) {
                this.trigger(this.onHeaderClick, { column: column }, e);
            }
        }

        private handleMouseEnter(e: JQueryMouseEventObject): void {
            this.trigger(this.onMouseEnter, {}, e);
        }

        private handleMouseLeave(e: JQueryMouseEventObject): void {
            this.trigger(this.onMouseLeave, {}, e);
        }

        private cellExists(row: number, cell: number): boolean {
            return !(row < 0 || row >= this.getDataLength() || cell < 0 || cell >= this._columns.length);
        }

        getCellFromPoint(x: number, y: number): { row: number; cell: number; } {
            var row = this.getRowFromPosition(y);
            var cell = 0;
            var cols = this._columns;
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

        getCellFromNode(cellNode: HTMLElement): number {
            // read column number from .l<columnNumber> CSS class
            var cls = /l\d+/.exec(cellNode.className);
            if (!cls) {
                throw "getCellFromNode: cannot get cell - " + cellNode.className;
            }
            return parseInt(cls[0].substr(1, cls[0].length - 1), 10);
        }

        getRowFromNode(rowNode: HTMLElement): number {
            for (var row in this.rowsCache)
                for (var i in this.rowsCache[row].rowNode)
                    if (this.rowsCache[row].rowNode[i] === rowNode)
                        return parseInt(row, 10);

            return null;
        }

        private getFrozenRowOffset(row: number): any {
            var offset =
                (this.hasFrozenRows)
                    ? (this._options.frozenBottom)
                        ? (row >= this.actualFrozenRow)
                            ? (this.h < this.viewportTopH)
                                ? (this.actualFrozenRow * this._options.rowHeight)
                                : this.h
                            : 0
                        : (row >= this.actualFrozenRow)
                            ? this.frozenRowsHeight
                            : 0
                    : 0;

            return offset;
        }

        getCellFromEvent(e: any): { row: number; cell: number; } {
            var row, cell;
            var $cell = $(e.target).closest(".slick-cell", this.$canvas as any);
            if (!$cell.length) {
                return null;
            }

            row = this.getRowFromNode($cell[0].parentNode as HTMLElement);

            if (this.hasFrozenRows) {

                var c = $cell.parents('.grid-canvas').offset();

                var rowOffset = 0;
                var isBottom = $cell.parents('.grid-canvas-bottom').length;

                if (isBottom) {
                    rowOffset = (this._options.frozenBottom) ? Math.round(this.$canvasTopL.height()) : this.frozenRowsHeight;
                }

                row = this.getCellFromPoint(e.clientX - c[this.xLeft], e.clientY - c.top + rowOffset + $(document).scrollTop()).row;
            }

            cell = this.getCellFromNode($cell[0]);

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

            var frozenRowOffset = this.getFrozenRowOffset(row);
            var cols = this._columns, fixStart = this._fixedStartCols;
            var y1 = this.getRowTop(row) - frozenRowOffset;
            var y2 = y1 + this._options.rowHeight - 1;
            var x1 = 0;
            for (var i = 0; i < cell; i++) {
                if (i == fixStart) {
                    x1 = 0;
                }
                x1 += cols[i].width;
            }
            var x2 = x1 + cols[cell].width;

            return this.rtl ? {
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
            if (this.tabbingDirection == -1) {
                this.$focusSink[0].focus();
            } else {
                this.$focusSink2[0].focus();
            }
        }

        scrollCellIntoView(row: number, cell: number, doPaging?: boolean): void {
            // Don't scroll to frozen cells
            if (cell < this._fixedStartCols) {
                return;
            }

            if (row < this.actualFrozenRow) {
                this.scrollRowIntoView(row, doPaging);
            }

            var colspan = this.getColspan(row, cell);
            var left = this._viewColLeft[cell],
                right = this._viewColRight[cell + (colspan > 1 ? colspan - 1 : 0)],
                scrollRight = this.scrollLeft + Math.round(this.$viewportScrollContainerX.width());

            if (left < this.scrollLeft) {
                this.$viewportScrollContainerX.scrollLeft(left);
                this.handleScroll();
                this.render();
            } else if (right > scrollRight) {
                this.$viewportScrollContainerX.scrollLeft(Math.min(left, right - this.$viewportScrollContainerX[0].clientWidth));
                this.handleScroll();
                this.render();
            }
        }

        private setActiveCellInternal(newCell: HTMLElement, opt_editMode?: boolean): void {
            if (this.activeCellNode !== null) {
                this.makeActiveCellNormal();
                $(this.activeCellNode).removeClass("active");
                if (this.rowsCache[this.activeRow]) {
                    $(this.rowsCache[this.activeRow].rowNode).removeClass("active");
                }
            }

            var activeCellChanged = (this.activeCellNode !== newCell);
            this.activeCellNode = newCell;

            if (this.activeCellNode != null) {
                var $activeCellNode = $(this.activeCellNode);
                var $activeCellOffset = $activeCellNode.offset();

                var rowOffset = Math.floor($activeCellNode.parents('.grid-canvas').offset().top);
                var isBottom = $activeCellNode.parents('.grid-canvas-bottom').length;

                if (this.hasFrozenRows && isBottom) {
                    rowOffset -= (this._options.frozenBottom)
                        ? Math.round(this.$canvasTopL.height())
                        : this.frozenRowsHeight;
                }

                var cell = this.getCellFromPoint($activeCellOffset[this.xLeft], Math.ceil($activeCellOffset.top) - rowOffset);

                this.activeRow = cell.row;
                var activeCell: number = this.activePosX = activeCell = this.activePosX = this.getCellFromNode(this.activeCellNode);

                $activeCellNode.addClass("active");
                if (this.rowsCache[this.activeRow]) {
                    $(this.rowsCache[this.activeRow].rowNode).addClass('active');
                }


                if (opt_editMode == null) {
                    opt_editMode = (this.activeRow == this.getDataLength()) || this._options.autoEdit;
                }

                if (this._options.editable && opt_editMode && this.isCellPotentiallyEditable(this.activeRow, activeCell)) {
                    clearTimeout(this.h_editorLoader);

                    if (this._options.asyncEditorLoading) {
                        this.h_editorLoader = setTimeout(() => {
                            this.makeActiveCellEditable();
                        }, this._options.asyncEditorLoadDelay);
                    } else {
                        this.makeActiveCellEditable();
                    }
                }
            } else {
                this.activeRow = activeCell = null;
            }

            if (activeCellChanged) {
                setTimeout(this.scrollActiveCellIntoView.bind(this), 50);
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
            if (this._columns[cell].cannotTriggerInsert && row >= dataLength) {
                return false;
            }

            // does this cell have an editor?
            if (!this.getEditor(row, cell)) {
                return false;
            }

            return true;
        }

        private makeActiveCellNormal(): void {
            if (!this.currentEditor) {
                return;
            }
            this.trigger(this.onBeforeCellEditorDestroy, { editor: this.currentEditor });
            this.currentEditor.destroy();
            this.currentEditor = null;

            if (this.activeCellNode) {
                var d = this.getDataItem(this.activeRow);
                $(this.activeCellNode).removeClass("editable invalid");
                if (d) {
                    var column = this._columns[this.activeCell];
                    this.activeCellNode.innerHTML = this.callFormatter(this.activeRow, this.activeCell, this.getDataItemValueForColumn(d, column), column, d);
                    this.invalidatePostProcessingResults(this.activeRow);
                }
            }

            // if there previously was text selected on a page (such as selected text in the edit cell just removed),
            // IE can't set focus to anything else correctly
            if (navigator.userAgent.toLowerCase().match(/msie/)) {
                this.clearTextSelection();
            }

            this.getEditorLock().deactivate(this.editController);
        }

        editActiveCell(editor?: Editor): void {
            this.makeActiveCellEditable(editor);
        }

        private makeActiveCellEditable(editor?: Editor): void {
            if (!this.activeCellNode) {
                return;
            }
            if (!this._options.editable) {
                throw "Grid : makeActiveCellEditable : should never get called when options.editable is false";
            }

            // cancel pending async call if there is one
            clearTimeout(this.h_editorLoader);

            if (!this.isCellPotentiallyEditable(this.activeRow, this.activeCell)) {
                return;
            }

            var columnDef = this._columns[this.activeCell];
            var item = this.getDataItem(this.activeRow);

            if (this.trigger(this.onBeforeEditCell, { row: this.activeRow, cell: this.activeCell, item: item, column: columnDef }) === false) {
                this.setFocus();
                return;
            }

            this.getEditorLock().activate(this.editController);
            $(this.activeCellNode).addClass("editable");

            // don't clear the cell if a custom editor is passed through
            if (!editor) {
                this.activeCellNode.innerHTML = "";
            }

            this.currentEditor = new (editor || this.getEditor(this.activeRow, this.activeCell))({
                grid: this,
                gridPosition: this.absBox(this.$container[0]),
                position: this.absBox(this.activeCellNode),
                container: this.activeCellNode,
                column: columnDef,
                item: item || {},
                commitChanges: this.commitEditAndSetFocus,
                cancelChanges: this.cancelEditAndSetFocus
            });

            if (item) {
                this.currentEditor.loadValue(item);
            }

            this.serializedEditorValue = this.currentEditor.serializeValue();

            if (this.currentEditor.position) {
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

        private absBox(elem: HTMLElement): Position {
            var box: Position = {
                top: elem.offsetTop,
                bottom: 0,
                width: $(elem).outerWidth(),
                height: $(elem).outerHeight(),
                visible: true
            };

            box[this.xLeft] = elem.offsetLeft;
            box[this.xRight] = 0;

            box.bottom = box.top + box.height;
            box[this.xRight] = box[this.xLeft] + box.width;

            // walk up the tree
            var offsetParent = elem.offsetParent;
            while ((elem = elem.parentNode as HTMLElement) != document.body) {
                if (box.visible && elem.scrollHeight != elem.offsetHeight && $(elem).css("overflowY") != "visible") {
                    box.visible = box.bottom > elem.scrollTop && box.top < elem.scrollTop + elem.clientHeight;
                }

                if (box.visible && elem.scrollWidth != elem.offsetWidth && $(elem).css("overflowX") != "visible") {
                    box.visible = box[this.xRight] > elem.scrollLeft && box[this.xLeft] < elem.scrollLeft + elem.clientWidth;
                }

                box[this.xLeft] -= elem.scrollLeft;
                box.top -= elem.scrollTop;

                if (elem === offsetParent) {
                    box.right += elem.offsetLeft;
                    box.top += elem.offsetTop;
                    offsetParent = elem.offsetParent;
                }

                box.bottom = box.top + box.height;
                box[this.xRight] = box[this.xLeft] + box.width;
            }

            return box;
        }

        private getActiveCellPosition(): Position {
            return this.absBox(this.activeCellNode);
        }

        getGridPosition(): Position {
            return this.absBox(this.$container[0]);
        }

        private handleActiveCellPositionChange(): void {
            if (!this.activeCellNode) {
                return;
            }

            this.trigger(this.onActiveCellPositionChanged, {});

            if (this.currentEditor) {
                var cellBox = this.getActiveCellPosition();
                if (this.currentEditor.show && this.currentEditor.hide) {
                    if (!cellBox.visible) {
                        this.currentEditor.hide();
                    } else {
                        this.currentEditor.show();
                    }
                }

                if (this.currentEditor.position) {
                    this.currentEditor.position(cellBox);
                }
            }
        }

        getCellEditor(): Editor {
            return this.currentEditor;
        }

        getActiveCell(): RowCell {
            if (!this.activeCellNode) {
                return null;
            } else {
                return { row: this.activeRow, cell: this.activeCell };
            }
        }

        getActiveCellNode(): HTMLElement {
            return this.activeCellNode;
        }

        scrollActiveCellIntoView(): void {
            if (this.activeRow != null && this.activeCell != null) {
                this.scrollCellIntoView(this.activeRow, this.activeCell);
            }
        }

        scrollRowIntoView(row: number, doPaging?: boolean): void {

            if (!this.hasFrozenRows ||
                (!this._options.frozenBottom && row > this.actualFrozenRow - 1) ||
                (this._options.frozenBottom && row < this.actualFrozenRow - 1)) {

                var viewportScrollH = Math.round(this.$viewportScrollContainerY.height());

                var rowAtTop = row * this._options.rowHeight;
                var rowAtBottom = (row + 1) * this._options.rowHeight
                    - viewportScrollH
                    + (this.viewportHasHScroll ? scrollbarDimensions.height : 0);

                // need to page down?
                if ((row + 1) * this._options.rowHeight > this.scrollTop + viewportScrollH + this.offset) {
                    this.scrollTo(doPaging ? rowAtTop : rowAtBottom);
                    this.render();
                }
                // or page up?
                else if (row * this._options.rowHeight < this.scrollTop + this.offset) {
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
            var deltaRows = dir * this.numVisibleRows;
            this.scrollTo((this.getRowFromPosition(this.scrollTop) + deltaRows) * this._options.rowHeight);
            this.render();

            if (this._options.enableCellNavigation && this.activeRow != null) {
                var row = this.activeRow + deltaRows;
                var dataLengthIncludingAddNew = this.getDataLengthIncludingAddNew();
                if (row >= dataLengthIncludingAddNew) {
                    row = dataLengthIncludingAddNew - 1;
                }
                if (row < 0) {
                    row = 0;
                }

                var cell = 0, prevCell = null;
                var prevActivePosX = this.activePosX;
                while (cell <= this.activePosX) {
                    if (this.canCellBeActive(row, cell)) {
                        prevCell = cell;
                    }
                    cell += this.getColspan(row, cell);
                }

                if (prevCell !== null) {
                    this.setActiveCellInternal(this.getCellNode(row, prevCell));
                    this.activePosX = prevActivePosX;
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

        getColspan(row: number, cell: number): number {
            var itemMetadata = this._data.getItemMetadata && this._data.getItemMetadata(row) as ItemMetadata;
            if (!itemMetadata || !itemMetadata.columns) {
                return 1;
            }

            var cols = this._columns;
            var columnData = cols[cell] && (itemMetadata.columns[cols[cell].id] || itemMetadata.columns[cell]);
            var colspan = (columnData && columnData.colspan);
            if (colspan === "*") {
                colspan = cols.length - cell;
            } else {
                colspan = colspan || 1;
            }

            return colspan;
        }

        findFirstFocusableCell(row: number): number {
            var cell = 0;
            var cols = this._columns;
            while (cell < cols.length) {
                if (this.canCellBeActive(row, cell)) {
                    return cell;
                }
                cell += this.getColspan(row, cell);
            }
            return null;
        }

        findLastFocusableCell(row: number): number {
            var cell = 0;
            var lastFocusableCell = null;
            var cols = this._columns;
            while (cell < cols.length) {
                if (this.canCellBeActive(row, cell)) {
                    lastFocusableCell = cell;
                }
                cell += this.getColspan(row, cell);
            }
            return lastFocusableCell;
        }

        gotoRight(row?: number, cell?: number, posX?: number): { row: any; cell: any; posX: any; } {
            var cols = this._columns;
            if (cell >= cols.length) {
                return null;
            }

            do {
                cell += this.getColspan(row, cell);
            }
            while (cell < cols.length && !this.canCellBeActive(row, cell));

            if (cell < cols.length) {
                return {
                    row: row,
                    cell: cell,
                    posX: posX
                };
            }
            return null;
        }

        gotoLeft(row?: number, cell?: number, posX?: number): { row: number; cell: number; posX: number; } {
            if (cell <= 0) {
                return null;
            }

            var firstFocusableCell = this.findFirstFocusableCell(row);
            if (firstFocusableCell === null || firstFocusableCell >= cell) {
                return null;
            }

            var prev = {
                row: row,
                cell: firstFocusableCell,
                posX: firstFocusableCell
            };
            var pos;
            while (true) {
                pos = this.gotoRight(prev.row, prev.cell, prev.posX);
                if (!pos) {
                    return null;
                }
                if (pos.cell >= cell) {
                    return prev;
                }
                prev = pos;
            }
        }

        gotoDown(row?: number, cell?: number, posX?: number): { row: number; cell: number; posX: number; } {
            var prevCell;
            var dataLengthIncludingAddNew = this.getDataLengthIncludingAddNew();
            while (true) {
                if (++row >= dataLengthIncludingAddNew) {
                    return null;
                }

                prevCell = cell = 0;
                while (cell <= posX) {
                    prevCell = cell;
                    cell += this.getColspan(row, cell);
                }

                if (this.canCellBeActive(row, prevCell)) {
                    return {
                        row: row,
                        cell: prevCell,
                        posX: posX
                    };
                }
            }
        }

        gotoUp(row?: number, cell?: number, posX?: number): { row: number; cell: number; posX: number; } {
            var prevCell;
            while (true) {
                if (--row < 0) {
                    return null;
                }

                prevCell = cell = 0;
                while (cell <= posX) {
                    prevCell = cell;
                    cell += this.getColspan(row, cell);
                }

                if (this.canCellBeActive(row, prevCell)) {
                    return {
                        "row": row,
                        "cell": prevCell,
                        "posX": posX
                    };
                }
            }
        }

        gotoNext(row?: number, cell?: number, posX?: number) {
            if (row == null && cell == null) {
                row = cell = posX = 0;
                if (this.canCellBeActive(row, cell)) {
                    return {
                        "row": row,
                        "cell": cell,
                        "posX": cell
                    };
                }
            }

            var pos = this.gotoRight(row, cell, posX);
            if (pos) {
                return pos;
            }

            var firstFocusableCell = null;
            var dataLengthIncludingAddNew = this.getDataLengthIncludingAddNew();
            while (++row < dataLengthIncludingAddNew) {
                firstFocusableCell = this.findFirstFocusableCell(row);
                if (firstFocusableCell !== null) {
                    return {
                        "row": row,
                        "cell": firstFocusableCell,
                        "posX": firstFocusableCell
                    };
                }
            }
            return null;
        }

        gotoPrev(row?: number, cell?: number, posX?: number) {
            var cols = this._columnById;
            if (row == null && cell == null) {
                row = this.getDataLengthIncludingAddNew() - 1;
                cell = posX = cols.length - 1;
                if (this.canCellBeActive(row, cell)) {
                    return {
                        "row": row,
                        "cell": cell,
                        "posX": cell
                    };
                }
            }

            var pos;
            var lastSelectableCell;
            while (!pos) {
                pos = this.gotoLeft(row, cell, posX);
                if (pos) {
                    break;
                }
                if (--row < 0) {
                    return null;
                }

                cell = 0;
                lastSelectableCell = this.findLastFocusableCell(row);
                if (lastSelectableCell !== null) {
                    pos = {
                        "row": row,
                        "cell": lastSelectableCell,
                        "posX": lastSelectableCell
                    };
                }
            }
            return pos;
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

        /**
         * @param {string} dir Navigation direction.
         * @return {boolean} Whether navigation resulted in a change of active cell.
         */
        navigate(dir: string): boolean {
            if (!this._options.enableCellNavigation) {
                return false;
            }

            if (!this.activeCellNode && dir != "prev" && dir != "next") {
                return false;
            }

            if (!this.getEditorLock().commitCurrentEdit()) {
                return true;
            }
            this.setFocus();

            var tabbingDirections = {
                "up": -1,
                "down": 1,
                "prev": -1,
                "next": 1
            };

            tabbingDirections[this.xLeft] = -1;
            tabbingDirections[this.xRight] = 1;

            this.tabbingDirection = tabbingDirections[dir];

            var stepFunctions = {
                "up": this.gotoUp,
                "down": this.gotoDown,
                "prev": this.gotoPrev,
                "next": this.gotoNext
            };

            stepFunctions[this.xLeft] = this.gotoLeft;
            stepFunctions[this.xRight] = this.gotoRight;

            var stepFn = stepFunctions[dir];
            var pos = stepFn(this.activeRow, this.activeCell, this.activePosX);
            if (pos) {
                if (this.hasFrozenRows && this._options.frozenBottom && pos.row == this.getDataLength()) {
                    return;
                }

                var isAddNewRow = (pos.row == this.getDataLength());

                if ((!this._options.frozenBottom && pos.row >= this.actualFrozenRow)
                    || (this._options.frozenBottom && pos.row < this.actualFrozenRow)
                ) {
                    this.scrollCellIntoView(pos.row, pos.cell, !isAddNewRow);
                }

                this.setActiveCellInternal(this.getCellNode(pos.row, pos.cell))
                this.activePosX = pos.posX;
                return true;
            } else {
                this.setActiveCellInternal(this.getCellNode(this.activeRow, this.activeCell));
                return false;
            }
        }

        getCellNode(row: number, cell: number): HTMLElement {
            if (this.rowsCache[row]) {
                this.ensureCellNodesInRowsCache(row);
                return this.rowsCache[row].cellNodesByColumnIdx[cell];
            }
            return null;
        }

        setActiveCell(row: number, cell: number) {
            if (!this.initialized) { return; }
            var cols = this._columns;
            if (row > this.getDataLength() || row < 0 || cell >= cols.length || cell < 0) {
                return;
            }

            if (!this._options.enableCellNavigation) {
                return;
            }

            this.scrollCellIntoView(row, cell, false);
            this.setActiveCellInternal(this.getCellNode(row, cell), false);
        }

        private canCellBeActive(row: number, cell: number): boolean {
            var cols = this._columns;
            if (!this._options.enableCellNavigation || row >= this.getDataLengthIncludingAddNew() ||
                row < 0 || cell >= cols.length || cell < 0) {
                return false;
            }

            var rowMetadata = this._data.getItemMetadata && this._data.getItemMetadata(row);
            if (rowMetadata && typeof rowMetadata.focusable === "boolean") {
                return rowMetadata.focusable;
            }

            var columnMetadata = rowMetadata && rowMetadata.columns;
            if (columnMetadata && cols[cell] && columnMetadata[cols[cell].id] && typeof columnMetadata[cols[cell].id].focusable === "boolean") {
                return columnMetadata[cols[cell].id].focusable;
            }
            if (columnMetadata && columnMetadata[cell] && typeof columnMetadata[cell].focusable === "boolean") {
                return columnMetadata[cell].focusable;
            }

            return cols[cell].focusable;
        }

        canCellBeSelected(row: number, cell: number) {
            var cols = this._columns;
            if (row >= this.getDataLength() || row < 0 || cell >= cols.length || cell < 0) {
                return false;
            }

            var rowMetadata = this._data.getItemMetadata && this._data.getItemMetadata(row);
            if (rowMetadata && typeof rowMetadata.selectable === "boolean") {
                return rowMetadata.selectable;
            }

            var columnMetadata = rowMetadata && rowMetadata.columns && (rowMetadata.columns[cols[cell].id] || rowMetadata.columns[cell]);
            if (columnMetadata && typeof columnMetadata.selectable === "boolean") {
                return columnMetadata.selectable;
            }

            return cols[cell].selectable;
        }

        gotoCell(row: number, cell: number, forceEdit?: boolean) {
            if (!this.initialized) { return; }
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
            if (!this.currentEditor) {
                this.setFocus();
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        // IEditor implementation for the editor lock

        commitCurrentEdit(): boolean {
            var item = this.getDataItem(this.activeRow);
            var column = this._columns[this.activeCell];
            var self = this;

            if (this.currentEditor) {
                if (this.currentEditor.isValueChanged()) {
                    var validationResults = this.currentEditor.validate();

                    if (validationResults.valid) {
                        if (this.activeRow < this.getDataLength()) {
                            var editCommand: EditCommand = {
                                row: this.activeRow,
                                cell: self.activeCell,
                                editor: this.currentEditor,
                                serializedValue: this.currentEditor.serializeValue(),
                                prevSerializedValue: this.serializedEditorValue,
                                execute: function () {
                                    this.editor.applyValue(item, this.serializedValue);
                                    self.updateRow(this.row);
                                    self.trigger(self.onCellChange, {
                                        row: this.activeRow,
                                        cell: self.activeCell,
                                        item: item
                                    });
                                },
                                undo: function () {
                                    this.editor.applyValue(item, this.prevSerializedValue);
                                    self.updateRow(this.row);
                                    self.trigger(self.onCellChange, {
                                        row: this.activeRow,
                                        cell: self.activeCell,
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
                            this.currentEditor.applyValue(newItem, this.currentEditor.serializeValue());
                            this.makeActiveCellNormal();
                            this.trigger(this.onAddNewRow, { item: newItem, column: column });
                        }

                        // check whether the lock has been re-acquired by event handlers
                        return !this.getEditorLock().isActive();
                    } else {
                        // Re-add the CSS class to trigger transitions, if any.
                        $(this.activeCellNode).removeClass("invalid");
                        $(this.activeCellNode).width();  // force layout
                        $(this.activeCellNode).addClass("invalid");

                        this.trigger(this.onValidationError, {
                            editor: this.currentEditor,
                            cellNode: this.activeCellNode,
                            validationResults: validationResults,
                            row: this.activeRow,
                            cell: this.activeCell,
                            column: column
                        });

                        this.currentEditor.focus();
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

        private rowsToRanges(rows: number[]): Range[] {
            var ranges = [];
            var lastCell = this._columns.length - 1;
            for (var i = 0; i < rows.length; i++) {
                ranges.push(new Range(rows[i], 0, rows[i], lastCell));
            }
            return ranges;
        }

        private getSelectedRows(): number[] {
            if (!this.selectionModel) {
                throw "Selection model is not set";
            }
            return this.selectedRows;
        }

        private setSelectedRows(rows: number[]) {
            if (!this.selectionModel) {
                throw "Selection model is not set";
            }
            this.selectionModel.setSelectedRanges(this.rowsToRanges(rows));
        }
    }

    // shared across all grids on the page
    var scrollbarDimensions: { width: number, height: number };
    var maxSupportedCssHeight: number;  // browser's breaking point

    interface CachedRow {
        rowNode: JQuery,
        // ColSpans of rendered cells (by column idx).
        // Can also be used for checking whether a cell has been rendered.
        cellColSpans: number[],

        // Cell nodes (by column idx).  Lazy-populated by ensureCellNodesInRowsCache().
        cellNodesByColumnIdx: { [key: number]: HTMLElement },

        // Column indices of cell nodes that have been rendered, but not yet indexed in
        // cellNodesByColumnIdx.  These are in the same order as cell nodes added at the
        // end of the row.
        cellRenderQueue: number[]
    }

    interface PostProcessCleanupEntry {
        groupId: number,
        cellNode?: HTMLElement,
        columnIdx?: number,
        rowNode?: JQuery;
        rowIdx?: number;
    }

    function defaultFormatter(row: number, cell: number, value: any) {
        if (value == null) {
            return "";
        } else {
            return (value + "").replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
        }
    }

    function disableSelection($target: JQuery) {
        if ($target && $target.jquery) {
            $target
                .attr("unselectable", "on")
                .css("MozUserSelect", "none")
                .on("selectstart.ui", function () {
                    return false;
                }); // from jquery:ui.core.js 1.7.2
        }
    }

    function getMaxSupportedCssHeight(): number {
        var supportedHeight = 1000000;
        // FF reports the height back but still renders blank after ~6M px
        var testUpTo = navigator.userAgent.toLowerCase().match(/firefox/) ? 6000000 : 1000000000;
        var div = $("<div style='display:none' />").appendTo(document.body);

        while (true) {
            var test = supportedHeight * 2;
            div.css("height", test);
            if (test > testUpTo || Math.round(div.height()) !== test) {
                break;
            } else {
                supportedHeight = test;
            }
        }

        div.remove();
        return supportedHeight;
    }    

    function adjustFrozenColumnCompat(columns: Column[], options: GridOptions) {
        if (options?.frozenColumn == null || options.frozenColumn < 0)
            return;

        var toFreeze = options.frozenColumn + 1;
        var i = 0;
        while (i < columns.length) {
            var col = columns[i++];
            if (toFreeze > 0 && col.visible !== false) {
                col.fixedTo = "start";
                toFreeze--;
            }
            else if (col.fixedTo !== undefined)
                delete col.fixedTo;
        }

        delete options.frozenColumn;
    }


}
