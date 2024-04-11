

declare namespace Slick {
    /***
     * A base class that all special / non-data rows (like Group and GroupTotals) derive from.
     */
    class NonDataRow {
    	__nonDataRow: boolean;
    }
    const preClickClassName = "slick-edit-preclick";
    interface IEventData {
    	readonly type?: string;
    	currentTarget?: EventTarget | null;
    	target?: EventTarget | null;
    	originalEvent?: any;
    	defaultPrevented?: boolean;
    	preventDefault?(): void;
    	stopPropagation?(): void;
    	stopImmediatePropagation?(): void;
    	isDefaultPrevented?(): boolean;
    	isImmediatePropagationStopped?(): boolean;
    	isPropagationStopped?(): boolean;
    }
    /***
     * An event object for passing data to event handlers and letting them control propagation.
     * <p>This is pretty much identical to how W3C and jQuery implement events.</p>
     */
    class EventData implements IEventData {
    	private _isPropagationStopped;
    	private _isImmediatePropagationStopped;
    	/***
    	 * Stops event from propagating up the DOM tree.
    	 * @method stopPropagation
    	 */
    	stopPropagation(): void;
    	/***
    	 * Returns whether stopPropagation was called on this event object.
    	 */
    	isPropagationStopped(): boolean;
    	/***
    	 * Prevents the rest of the handlers from being executed.
    	 */
    	stopImmediatePropagation(): void;
    	/***
    	 * Returns whether stopImmediatePropagation was called on this event object.\
    	 */
    	isImmediatePropagationStopped(): boolean;
    }
    /***
     * A simple publisher-subscriber implementation.
     */
    class EventEmitter<TArgs = any, TEventData extends IEventData = IEventData> {
    	private _handlers;
    	/***
    	 * Adds an event handler to be called when the event is fired.
    	 * <p>Event handler will receive two arguments - an <code>EventData</code> and the <code>data</code>
    	 * object the event was fired with.<p>
    	 * @method subscribe
    	 * @param fn {Function} Event handler.
    	 */
    	subscribe(fn: ((e: TEventData, args: TArgs) => void)): void;
    	/***
    	 * Removes an event handler added with <code>subscribe(fn)</code>.
    	 * @method unsubscribe
    	 * @param fn {Function} Event handler to be removed.
    	 */
    	unsubscribe(fn: ((e: TEventData, args: TArgs) => void)): void;
    	/***
    	 * Fires an event notifying all subscribers.
    	 * @param args {Object} Additional data object to be passed to all handlers.
    	 * @param e {EventData}
    	 *      Optional.
    	 *      An <code>EventData</code> object to be passed to all handlers.
    	 *      For DOM events, an existing W3C/jQuery event object can be passed in.
    	 * @param scope {Object}
    	 *      Optional.
    	 *      The scope ("this") within which the handler will be executed.
    	 *      If not specified, the scope will be set to the <code>Event</code> instance.
    	 */
    	notify(args?: any, e?: TEventData, scope?: object): any;
    	clear(): void;
    }
    class EventSubscriber<TArgs = any, TEventData extends IEventData = IEventData> {
    	private _handlers;
    	subscribe(event: EventEmitter<TArgs, TEventData>, handler: ((e: TEventData, args: TArgs) => void)): this;
    	unsubscribe(event: EventEmitter<TArgs, TEventData>, handler: ((e: TEventData, args: TArgs) => void)): this;
    	unsubscribeAll(): EventSubscriber<TArgs, TEventData>;
    }
    /** @deprecated */
    const keyCode: {
    	BACKSPACE: number;
    	DELETE: number;
    	DOWN: number;
    	END: number;
    	ENTER: number;
    	ESCAPE: number;
    	HOME: number;
    	INSERT: number;
    	LEFT: number;
    	PAGEDOWN: number;
    	PAGEUP: number;
    	RIGHT: number;
    	TAB: number;
    	UP: number;
    };
    function patchEvent(e: IEventData): IEventData;
    interface Position {
    	bottom?: number;
    	height?: number;
    	left?: number;
    	right?: number;
    	top?: number;
    	visible?: boolean;
    	width?: number;
    }
    interface ValidationResult {
    	valid: boolean;
    	msg?: string;
    }
    interface RowCell {
    	row: number;
    	cell: number;
    }
    interface EditorHost {
    	getActiveCell(): RowCell;
    	navigateNext(): boolean;
    	navigatePrev(): boolean;
    	onCompositeEditorChange: EventEmitter<any>;
    }
    interface CompositeEditorOptions {
    	formValues: any;
    }
    interface EditorOptions {
    	grid: EditorHost;
    	gridPosition?: Position;
    	position?: Position;
    	editorCellNavOnLRKeys?: boolean;
    	column?: Column;
    	columnMetaData?: ColumnMetadata<any>;
    	compositeEditorOptions?: CompositeEditorOptions;
    	container?: HTMLElement;
    	item?: any;
    	event?: IEventData;
    	commitChanges?: () => void;
    	cancelChanges?: () => void;
    }
    interface EditorFactory {
    	getEditor(column: Column, row?: number): EditorClass;
    }
    interface EditCommand {
    	row: number;
    	cell: number;
    	editor: Editor;
    	serializedValue: any;
    	prevSerializedValue: any;
    	execute: () => void;
    	undo: () => void;
    }
    interface EditorClass {
    	new (options: EditorOptions): Editor;
    	suppressClearOnEdit?: boolean;
    }
    interface Editor {
    	destroy(): void;
    	applyValue(item: any, value: any): void;
    	focus(): void;
    	isValueChanged(): boolean;
    	keyCaptureList?: number[];
    	loadValue(value: any): void;
    	serializeValue(): any;
    	position?(pos: Position): void;
    	preClick?(): void;
    	hide?(): void;
    	show?(): void;
    	validate?(): ValidationResult;
    }
    interface EditController {
    	commitCurrentEdit(): boolean;
    	cancelCurrentEdit(): boolean;
    }
    /***
     * A locking helper to track the active edit controller and ensure that only a single controller
     * can be active at a time.  This prevents a whole class of state and validation synchronization
     * issues.  An edit controller (such as SleekGrid) can query if an active edit is in progress
     * and attempt a commit or cancel before proceeding.
     * @class EditorLock
     * @constructor
     */
    class EditorLock {
    	private activeEditController;
    	/***
    	 * Returns true if a specified edit controller is active (has the edit lock).
    	 * If the parameter is not specified, returns true if any edit controller is active.
    	 * @method isActive
    	 * @param editController {EditController}
    	 * @return {Boolean}
    	 */
    	isActive(editController?: EditController): boolean;
    	/***
    	 * Sets the specified edit controller as the active edit controller (acquire edit lock).
    	 * If another edit controller is already active, and exception will be thrown.
    	 * @method activate
    	 * @param editController {EditController} edit controller acquiring the lock
    	 */
    	activate(editController: EditController): void;
    	/***
    	 * Unsets the specified edit controller as the active edit controller (release edit lock).
    	 * If the specified edit controller is not the active one, an exception will be thrown.
    	 * @method deactivate
    	 * @param editController {EditController} edit controller releasing the lock
    	 */
    	deactivate(editController: EditController): void;
    	/***
    	 * Attempts to commit the current edit by calling "commitCurrentEdit" method on the active edit
    	 * controller and returns whether the commit attempt was successful (commit may fail due to validation
    	 * errors, etc.).  Edit controller's "commitCurrentEdit" must return true if the commit has succeeded
    	 * and false otherwise.  If no edit controller is active, returns true.
    	 * @method commitCurrentEdit
    	 * @return {Boolean}
    	 */
    	commitCurrentEdit(): boolean;
    	/***
    	 * Attempts to cancel the current edit by calling "cancelCurrentEdit" method on the active edit
    	 * controller and returns whether the edit was successfully cancelled.  If no edit controller is
    	 * active, returns true.
    	 * @method cancelCurrentEdit
    	 * @return {Boolean}
    	 */
    	cancelCurrentEdit(): boolean;
    }
    /***
     * A global singleton editor lock.
     * @class GlobalEditorLock
     * @static
     * @constructor
     */
    const GlobalEditorLock: EditorLock;
    interface FormatterContext<TItem = any> {
    	addAttrs?: {
    		[key: string]: string;
    	};
    	addClass?: string;
    	cell?: number;
    	column?: Column<TItem>;
    	/** returns html escaped ctx.value if called without arguments. prefer this over ctx.value to avoid html injection attacks! */
    	readonly escape: ((value?: any) => string);
    	grid?: any;
    	item?: TItem;
    	row?: number;
    	tooltip?: string;
    	/** when returning a formatter result, prefer ctx.escape() to avoid script injection attacks! */
    	value?: any;
    }
    export type FormatterResult = (string | Element | DocumentFragment);
    export type ColumnFormat<TItem = any> = (ctx: FormatterContext<TItem>) => FormatterResult;
    interface CompatFormatterResult {
    	addClasses?: string;
    	text?: FormatterResult;
    	toolTip?: string;
    }
    export type CompatFormatter<TItem = any> = (row: number, cell: number, value: any, column: Column<TItem>, item: TItem, grid?: any) => string | CompatFormatterResult;
    interface FormatterFactory<TItem = any> {
    	getFormat?(column: Column<TItem>): ColumnFormat<TItem>;
    	getFormatter?(column: Column<TItem>): CompatFormatter<TItem>;
    }
    export type AsyncPostRender<TItem = any> = (cellNode: HTMLElement, row: number, item: TItem, column: Column<TItem>, reRender: boolean) => void;
    export type AsyncPostCleanup<TItem = any> = (cellNode: HTMLElement, row?: number, column?: Column<TItem>) => void;
    export type CellStylesHash = {
    	[row: number]: {
    		[columnId: string]: string;
    	};
    };
    function defaultColumnFormat(ctx: FormatterContext): any;
    function convertCompatFormatter(compatFormatter: CompatFormatter): ColumnFormat;
    function applyFormatterResultToCellNode(ctx: FormatterContext, html: FormatterResult, node: HTMLElement): void;
    /***
     * Information about a group of rows.
     */
    class Group<TEntity = any> extends NonDataRow {
    	readonly __group = true;
    	/**
    	 * Grouping level, starting with 0.
    	 * @property level
    	 * @type {Number}
    	 */
    	level: number;
    	/***
    	 * Number of rows in the group.
    	 * @property count
    	 * @type {Number}
    	 */
    	count: number;
    	/***
    	 * Grouping value.
    	 * @property value
    	 * @type {Object}
    	 */
    	value: any;
    	/***
    	 * Formatted display value of the group.
    	 * @property title
    	 * @type {String}
    	 */
    	title: string;
    	/***
    	 * Whether a group is collapsed.
    	 * @property collapsed
    	 * @type {Boolean}
    	 */
    	collapsed: boolean;
    	/***
    	 * GroupTotals, if any.
    	 * @property totals
    	 * @type {GroupTotals}
    	 */
    	totals: GroupTotals<TEntity>;
    	/**
    	 * Rows that are part of the group.
    	 * @property rows
    	 * @type {Array}
    	 */
    	rows: TEntity[];
    	/**
    	 * Sub-groups that are part of the group.
    	 * @property groups
    	 * @type {Array}
    	 */
    	groups: Group<TEntity>[];
    	/**
    	 * A unique key used to identify the group.  This key can be used in calls to DataView
    	 * collapseGroup() or expandGroup().
    	 * @property groupingKey
    	 * @type {Object}
    	 */
    	groupingKey: string;
    	/***
    	 * Compares two Group instances.
    	 * @method equals
    	 * @return {Boolean}
    	 * @param group {Group} Group instance to compare to.
    	 */
    	equals(group: Group): boolean;
    }
    /***
     * Information about group totals.
     * An instance of GroupTotals will be created for each totals row and passed to the aggregators
     * so that they can store arbitrary data in it.  That data can later be accessed by group totals
     * formatters during the display.
     * @class GroupTotals
     * @extends NonDataRow
     * @constructor
     */
    class GroupTotals<TEntity = any> extends NonDataRow {
    	readonly __groupTotals = true;
    	/***
    	 * Parent Group.
    	 * @param group
    	 * @type {Group}
    	 */
    	group: Group<TEntity>;
    	/***
    	 * Whether the totals have been fully initialized / calculated.
    	 * Will be set to false for lazy-calculated group totals.
    	 * @param initialized
    	 * @type {Boolean}
    	 */
    	initialized: boolean;
    	/**
    	 * Contains sum
    	 */
    	sum?: number;
    	/**
    	 * Contains avg
    	 */
    	avg?: number;
    	/**
    	 * Contains min
    	 */
    	min?: any;
    	/**
    	 * Contains max
    	 */
    	max?: any;
    }
    interface Column<TItem = any> {
    	asyncPostRender?: AsyncPostRender<TItem>;
    	asyncPostRenderCleanup?: AsyncPostCleanup<TItem>;
    	behavior?: any;
    	cannotTriggerInsert?: boolean;
    	cssClass?: string;
    	defaultSortAsc?: boolean;
    	editor?: EditorClass;
    	editorFixedDecimalPlaces?: number;
    	field?: string;
    	frozen?: boolean;
    	focusable?: boolean;
    	footerCssClass?: string;
    	format?: ColumnFormat<TItem>;
    	/** @deprecated */
    	formatter?: CompatFormatter<TItem>;
    	groupTotalsFormatter?: (p1?: GroupTotals<TItem>, p2?: Column<TItem>, grid?: unknown) => string;
    	headerCssClass?: string;
    	id?: string;
    	maxWidth?: any;
    	minWidth?: number;
    	name?: string;
    	nameIsHtml?: boolean;
    	previousWidth?: number;
    	referencedFields?: string[];
    	rerenderOnResize?: boolean;
    	resizable?: boolean;
    	selectable?: boolean;
    	sortable?: boolean;
    	sortOrder?: number;
    	toolTip?: string;
    	validator?: (value: any, editorArgs?: any) => ValidationResult;
    	visible?: boolean;
    	width?: number;
    }
    const columnDefaults: Partial<Column>;
    interface ColumnMetadata<TItem = any> {
    	colspan: number | "*";
    	cssClasses?: string;
    	editor?: EditorClass;
    	format?: ColumnFormat<TItem>;
    	/** @deprecated */
    	formatter?: CompatFormatter<TItem>;
    }
    interface ColumnSort {
    	columnId: string;
    	sortAsc?: boolean;
    }
    interface ItemMetadata<TItem = any> {
    	cssClasses?: string;
    	columns?: {
    		[key: string]: ColumnMetadata<TItem>;
    	};
    	focusable?: boolean;
    	format?: ColumnFormat<TItem>;
    	/** @deprecated */
    	formatter?: CompatFormatter<TItem>;
    	selectable?: boolean;
    }
    function initializeColumns(columns: Column[], defaults: Partial<Column<any>>): void;
    function titleize(str: string): string;
    class CellRange {
    	fromRow: number;
    	fromCell: number;
    	toRow: number;
    	toCell: number;
    	constructor(fromRow: number, fromCell: number, toRow?: number, toCell?: number);
    	/***
    	 * Returns whether a range represents a single row.
    	 */
    	isSingleRow(): boolean;
    	/***
    	 * Returns whether a range represents a single cell.
    	 */
    	isSingleCell(): boolean;
    	/***
    	 * Returns whether a range contains a given cell.
    	 */
    	contains(row: number, cell: number): boolean;
    	/***
    	 * Returns a readable representation of a range.
    	 */
    	toString(): string;
    }
    function addClass(el: Element, cls: string): void;
    function escapeHtml(s: any): any;
    function disableSelection(target: HTMLElement): void;
    function removeClass(el: Element, cls: string): void;
    function H<K extends keyof HTMLElementTagNameMap>(tag: K, attr?: {
    	ref?: (el?: HTMLElementTagNameMap[K]) => void;
    	[key: string]: string | number | boolean | ((el?: HTMLElementTagNameMap[K]) => void) | null | undefined;
    }, ...children: (string | Node)[]): HTMLElementTagNameMap[K];
    function spacerDiv(width: string): HTMLDivElement;
    function parsePx(str: string): number;
    interface IPlugin {
    	init(grid: Grid): void;
    	pluginName?: string;
    	destroy?: () => void;
    }
    interface ViewportInfo {
    	height: number;
    	width: number;
    	hasVScroll: boolean;
    	hasHScroll: boolean;
    	headerHeight: number;
    	groupingPanelHeight: number;
    	virtualHeight: number;
    	realScrollHeight: number;
    	topPanelHeight: number;
    	headerRowHeight: number;
    	footerRowHeight: number;
    	numVisibleRows: number;
    }
    interface SelectionModel extends IPlugin {
    	setSelectedRanges(ranges: CellRange[]): void;
    	onSelectedRangesChanged: EventEmitter<CellRange[]>;
    	refreshSelections?(): void;
    }
    interface ViewRange {
    	top?: number;
    	bottom?: number;
    	leftPx?: number;
    	rightPx?: number;
    }
    interface LayoutHost {
    	bindAncestorScroll(el: HTMLElement): void;
    	cleanUpAndRenderCells(range: ViewRange): void;
    	getAvailableWidth(): number;
    	getCellFromPoint(x: number, y: number): RowCell;
    	getColumnCssRules(idx: number): {
    		right: any;
    		left: any;
    	};
    	getColumns(): Column[];
    	getContainerNode(): HTMLElement;
    	getDataLength(): number;
    	getOptions(): GridOptions;
    	getRowFromNode(rowNode: HTMLElement): number;
    	getScrollDims(): {
    		width: number;
    		height: number;
    	};
    	getScrollLeft(): number;
    	getScrollTop(): number;
    	getViewportInfo(): ViewportInfo;
    	renderRows(range: ViewRange): void;
    }
    interface LayoutEngine {
    	appendCachedRow(row: number, rowNodeL: HTMLElement, rowNodeR: HTMLElement): void;
    	afterHeaderColumnDrag(): void;
    	afterSetOptions(args: GridOptions): void;
    	applyColumnWidths(): void;
    	beforeCleanupAndRenderCells(rendered: ViewRange): void;
    	afterRenderRows(rendered: ViewRange): void;
    	bindAncestorScrollEvents(): void;
    	calcCanvasWidth(): number;
    	updateHeadersWidth(): void;
    	isFrozenRow(row: number): boolean;
    	destroy(): void;
    	getCanvasNodeFor(cell: number, row: number): HTMLElement;
    	getCanvasNodes(): HTMLElement[];
    	getCanvasWidth(): number;
    	getRowFromCellNode(cellNode: HTMLElement, clientX: number, clientY: number): number;
    	getFooterRowCols(): HTMLElement[];
    	getFooterRowColsFor(cell: number): HTMLElement;
    	getFooterRowColumn(cell: number): HTMLElement;
    	getFrozenCols(): number;
    	getFrozenRowOffset(row: number): number;
    	getFrozenRows(): number;
    	getHeaderCols(): HTMLElement[];
    	getHeaderColsFor(cell: number): HTMLElement;
    	getHeaderColumn(cell: number): HTMLElement;
    	getHeaderRowCols(): HTMLElement[];
    	getHeaderRowColsFor(cell: number): HTMLElement;
    	getHeaderRowColumn(cell: number): HTMLElement;
    	getScrollCanvasY(): HTMLElement;
    	getScrollContainerX(): HTMLElement;
    	getScrollContainerY(): HTMLElement;
    	getTopPanelFor(arg0: number): HTMLElement;
    	getTopPanelNodes(): HTMLElement[];
    	getViewportNodeFor(cell: number, row: number): HTMLElement;
    	getViewportNodes(): HTMLElement[];
    	handleScrollH(): void;
    	handleScrollV(): void;
    	init(host: LayoutHost): void;
    	layoutName: string;
    	realScrollHeightChange(): void;
    	/** this might be called before init, chicken egg situation */
    	reorderViewColumns(viewCols: Column[], options?: GridOptions): Column[];
    	resizeCanvas(): void;
    	setPaneVisibility(): void;
    	setScroller(): void;
    	setOverflow(): void;
    	updateCanvasWidth(): boolean;
    }
    interface GridOptions<TItem = any> {
    	addNewRowCssClass?: string;
    	alwaysAllowHorizontalScroll?: boolean;
    	alwaysShowVerticalScroll?: boolean;
    	asyncEditorLoadDelay?: number;
    	asyncEditorLoading?: boolean;
    	asyncPostCleanupDelay?: number;
    	asyncPostRenderDelay?: number;
    	autoEdit?: boolean;
    	autoHeight?: boolean;
    	cellFlashingCssClass?: string;
    	cellHighlightCssClass?: string;
    	emptyNode?: (node: Element) => void;
    	columns?: Column<TItem>[];
    	createPreHeaderPanel?: boolean;
    	dataItemColumnValueExtractor?: (item: TItem, column: Column<TItem>) => void;
    	defaultColumnWidth?: number;
    	defaultFormat?: ColumnFormat<TItem>;
    	defaultFormatter?: CompatFormatter<TItem>;
    	editable?: boolean;
    	editCommandHandler?: (item: TItem, column: Column<TItem>, command: EditCommand) => void;
    	editorCellNavOnLRKeys?: boolean;
    	editorFactory?: EditorFactory;
    	editorLock?: EditorLock;
    	enableAddRow?: boolean;
    	enableAsyncPostRender?: boolean;
    	enableAsyncPostRenderCleanup?: boolean;
    	enableCellNavigation?: boolean;
    	enableCellRangeSelection?: boolean;
    	enableColumnReorder?: boolean;
    	enableRowReordering?: boolean;
    	enableTabKeyNavigation?: boolean;
    	enableTextSelectionOnCells?: boolean;
    	explicitInitialization?: boolean;
    	footerRowHeight?: number;
    	forceFitColumns?: boolean;
    	forceSyncScrolling?: boolean;
    	forceSyncScrollInterval?: number;
    	formatterFactory?: FormatterFactory;
    	frozenBottom?: boolean;
    	frozenColumns?: number;
    	frozenRows?: number;
    	fullWidthRows?: boolean;
    	groupingPanel?: boolean;
    	groupingPanelHeight?: number;
    	groupTotalsFormatter?: (p1?: GroupTotals<TItem>, p2?: Column<TItem>, grid?: any) => string;
    	headerRowHeight?: number;
    	jQuery?: {
    		ready: any;
    		fn: any;
    	};
    	leaveSpaceForNewRows?: boolean;
    	layoutEngine?: LayoutEngine;
    	minBuffer?: number;
    	multiColumnSort?: boolean;
    	multiSelect?: boolean;
    	preHeaderPanelHeight?: number;
    	renderAllCells?: boolean;
    	removeNode?: (node: Element) => void;
    	rowHeight?: number;
    	rtl?: boolean;
    	selectedCellCssClass?: string;
    	showCellSelection?: boolean;
    	showColumnHeader?: boolean;
    	showFooterRow?: boolean;
    	showGroupingPanel?: boolean;
    	showHeaderRow?: boolean;
    	showPreHeaderPanel?: boolean;
    	showTopPanel?: boolean;
    	slickCompat?: boolean;
    	suppressActiveCellChangeOnEdit?: boolean;
    	syncColumnCellResize?: boolean;
    	topPanelHeight?: number;
    	useLegacyUI?: boolean;
    	useCssVars?: boolean;
    	viewportClass?: string;
    }
    const gridDefaults: GridOptions;
    class Grid<TItem = any> implements EditorHost {
    	private _absoluteColMinWidth;
    	private _activeCanvasNode;
    	private _activeCell;
    	private _activeCellNode;
    	private _activePosX;
    	private _activeRow;
    	private _activeViewportNode;
    	private _cellCssClasses;
    	private _cellHeightDiff;
    	private _cellWidthDiff;
    	private _cellNavigator;
    	private _colById;
    	private _colDefaults;
    	private _colLeft;
    	private _colRight;
    	private _cols;
    	private _columnCssRulesL;
    	private _columnCssRulesR;
    	private _currentEditor;
    	private _data;
    	private _editController;
    	private _emptyNode;
    	private _headerColumnWidthDiff;
    	private _hEditorLoader;
    	private _hPostRender;
    	private _hPostRenderCleanup;
    	private _hRender;
    	private _ignoreScrollUntil;
    	private _initColById;
    	private _initCols;
    	private _initialized;
    	private _jQuery;
    	private _jumpinessCoefficient;
    	private _lastRenderTime;
    	private _layout;
    	private _numberOfPages;
    	private _options;
    	private _page;
    	private _pageHeight;
    	private _pageOffset;
    	private _pagingActive;
    	private _pagingIsLastPage;
    	private _plugins;
    	private _postCleanupActive;
    	private _postProcessCleanupQueue;
    	private _postProcessedRows;
    	private _postProcessFromRow;
    	private _postProcessGroupId;
    	private _postProcessToRow;
    	private _postRenderActive;
    	private _removeNode;
    	private _rowsCache;
    	private _scrollDims;
    	private _scrollLeft;
    	private _scrollLeftPrev;
    	private _scrollLeftRendered;
    	private _scrollTop;
    	private _scrollTopPrev;
    	private _scrollTopRendered;
    	private _selectedRows;
    	private _selectionModel;
    	private _serializedEditorValue;
    	private _sortColumns;
    	private _styleNode;
    	private _stylesheet;
    	private _tabbingDirection;
    	private _uid;
    	private _viewportInfo;
    	private _vScrollDir;
    	private _boundAncestorScroll;
    	private _container;
    	private _focusSink1;
    	private _focusSink2;
    	private _groupingPanel;
    	readonly onActiveCellChanged: EventEmitter<ArgsCell, IEventData>;
    	readonly onActiveCellPositionChanged: EventEmitter<ArgsGrid, IEventData>;
    	readonly onAddNewRow: EventEmitter<ArgsAddNewRow, IEventData>;
    	readonly onBeforeCellEditorDestroy: EventEmitter<ArgsEditorDestroy, IEventData>;
    	readonly onBeforeDestroy: EventEmitter<ArgsGrid, IEventData>;
    	readonly onBeforeEditCell: EventEmitter<ArgsCellEdit, IEventData>;
    	readonly onBeforeFooterRowCellDestroy: EventEmitter<ArgsColumnNode, IEventData>;
    	readonly onBeforeHeaderCellDestroy: EventEmitter<ArgsColumnNode, IEventData>;
    	readonly onBeforeHeaderRowCellDestroy: EventEmitter<ArgsColumnNode, IEventData>;
    	readonly onCellChange: EventEmitter<ArgsCellChange, IEventData>;
    	readonly onCellCssStylesChanged: EventEmitter<ArgsCssStyle, IEventData>;
    	readonly onClick: EventEmitter<ArgsCell, MouseEvent>;
    	readonly onColumnsReordered: EventEmitter<ArgsGrid, IEventData>;
    	readonly onColumnsResized: EventEmitter<ArgsGrid, IEventData>;
    	readonly onCompositeEditorChange: EventEmitter<ArgsGrid, IEventData>;
    	readonly onContextMenu: EventEmitter<ArgsGrid, UIEvent>;
    	readonly onDblClick: EventEmitter<ArgsCell, MouseEvent>;
    	readonly onDrag: EventEmitter<ArgsGrid, UIEvent>;
    	readonly onDragEnd: EventEmitter<ArgsGrid, UIEvent>;
    	readonly onDragInit: EventEmitter<ArgsGrid, UIEvent>;
    	readonly onDragStart: EventEmitter<ArgsGrid, UIEvent>;
    	readonly onFooterRowCellRendered: EventEmitter<ArgsColumnNode, IEventData>;
    	readonly onHeaderCellRendered: EventEmitter<ArgsColumnNode, IEventData>;
    	readonly onHeaderClick: EventEmitter<ArgsColumn, IEventData>;
    	readonly onHeaderContextMenu: EventEmitter<ArgsColumn, IEventData>;
    	readonly onHeaderMouseEnter: EventEmitter<ArgsColumn, MouseEvent>;
    	readonly onHeaderMouseLeave: EventEmitter<ArgsColumn, MouseEvent>;
    	readonly onHeaderRowCellRendered: EventEmitter<ArgsColumnNode, IEventData>;
    	readonly onKeyDown: EventEmitter<ArgsCell, KeyboardEvent>;
    	readonly onMouseEnter: EventEmitter<ArgsGrid, MouseEvent>;
    	readonly onMouseLeave: EventEmitter<ArgsGrid, MouseEvent>;
    	readonly onScroll: EventEmitter<ArgsScroll, IEventData>;
    	readonly onSelectedRowsChanged: EventEmitter<ArgsSelectedRowsChange, IEventData>;
    	readonly onSort: EventEmitter<ArgsSort, IEventData>;
    	readonly onValidationError: EventEmitter<ArgsValidationError, IEventData>;
    	readonly onViewportChanged: EventEmitter<ArgsGrid, IEventData>;
    	constructor(container: HTMLElement | ArrayLike<HTMLElement>, data: any, columns: Column<TItem>[], options: GridOptions<TItem>);
    	private createGroupingPanel;
    	private bindAncestorScroll;
    	init(): void;
    	private hasFrozenColumns;
    	private hasFrozenRows;
    	registerPlugin(plugin: IPlugin): void;
    	unregisterPlugin(plugin: IPlugin): void;
    	getPluginByName(name: string): IPlugin;
    	setSelectionModel(model: SelectionModel): void;
    	private unregisterSelectionModel;
    	getScrollBarDimensions(): {
    		width: number;
    		height: number;
    	};
    	getDisplayedScrollbarDimensions(): {
    		width: number;
    		height: number;
    	};
    	getAbsoluteColumnMinWidth(): number;
    	getSelectionModel(): SelectionModel;
    	private colIdOrIdxToCell;
    	getCanvasNode(columnIdOrIdx?: string | number, row?: number): HTMLElement;
    	getCanvases(): any | HTMLElement[];
    	getActiveCanvasNode(e?: IEventData): HTMLElement;
    	getViewportNode(columnIdOrIdx?: string | number, row?: number): HTMLElement;
    	private getViewports;
    	getActiveViewportNode(e?: IEventData): HTMLElement;
    	private getAvailableWidth;
    	private updateCanvasWidth;
    	private unbindAncestorScrollEvents;
    	updateColumnHeader(columnId: string, title?: string, toolTip?: string): void;
    	getHeader(): HTMLElement;
    	getHeaderColumn(columnIdOrIdx: string | number): HTMLElement;
    	getGroupingPanel(): HTMLElement;
    	getPreHeaderPanel(): HTMLElement;
    	getHeaderRow(): HTMLElement;
    	getHeaderRowColumn(columnIdOrIdx: string | number): HTMLElement;
    	getFooterRow(): HTMLElement;
    	getFooterRowColumn(columnIdOrIdx: string | number): HTMLElement;
    	private createColumnFooters;
    	private createColumnHeaders;
    	private setupColumnSort;
    	private static offset;
    	private sortableColInstances;
    	private setupColumnReorder;
    	private setupColumnResize;
    	columnsResized(invalidate?: boolean): void;
    	private setOverflow;
    	private measureCellPaddingAndBorder;
    	private createCssRules;
    	private getColumnCssRules;
    	private removeCssRules;
    	destroy(): void;
    	private trigger;
    	getEditorLock(): EditorLock;
    	getEditController(): EditController;
    	/** Gets a column by its ID. May also return non visible columns */
    	getColumnById(id: string): Column<TItem>;
    	/** Returns a column's index in the visible columns list by its column ID */
    	getColumnIndex(id: string): number;
    	/** Gets index of a column in the initial column list passed to the grid, or setColumns method. May include invisible cols and index does not have to match visible column order. */
    	getInitialColumnIndex(id: string): number;
    	/** Gets a view (e.g. visible) column by its column ID */
    	getVisibleColumnById(id: string): Column<TItem>;
    	autosizeColumns(): void;
    	private applyColumnHeaderWidths;
    	setSortColumn(columnId: string, ascending: boolean): void;
    	setSortColumns(cols: ColumnSort[]): void;
    	getSortColumns(): ColumnSort[];
    	private handleSelectedRangesChanged;
    	/** Returns only the visible columns in order */
    	getColumns(): Column<TItem>[];
    	/** Returns list of columns passed to the grid constructor, or setColumns method. May include invisible columns and order does not match visible column order. */
    	getInitialColumns(): Column<TItem>[];
    	private updateViewColLeftRight;
    	private setInitialCols;
    	setColumns(columns: Column<TItem>[]): void;
    	getOptions(): GridOptions<TItem>;
    	setOptions(args: GridOptions<TItem>, suppressRender?: boolean, suppressColumnSet?: boolean, suppressSetOverflow?: boolean): void;
    	private validateAndEnforceOptions;
    	private viewOnRowCountChanged;
    	private viewOnRowsChanged;
    	private viewOnDataChanged;
    	private bindToData;
    	private unbindFromData;
    	setData(newData: any, scrollToTop?: boolean): void;
    	getData(): any;
    	getDataLength(): number;
    	private getDataLengthIncludingAddNew;
    	getDataItem(i: number): TItem;
    	getTopPanel(): HTMLElement;
    	setTopPanelVisibility(visible: boolean): void;
    	setColumnHeaderVisibility(visible: boolean, animate?: boolean): void;
    	setFooterRowVisibility(visible: boolean): void;
    	setGroupingPanelVisibility(visible: boolean): void;
    	setPreHeaderPanelVisibility(visible: boolean): void;
    	setHeaderRowVisibility(visible: boolean): void;
    	getContainerNode(): HTMLElement;
    	getUID(): string;
    	private getRowTop;
    	private getRowFromPosition;
    	private scrollTo;
    	getFormatter(row: number, column: Column<TItem>): ColumnFormat<TItem>;
    	getFormatterContext(row: number, cell: number): FormatterContext;
    	private getEditor;
    	getDataItemValueForColumn(item: TItem, columnDef: Column<TItem>): any;
    	private appendRowHtml;
    	private appendCellHtml;
    	private cleanupRows;
    	invalidate(): void;
    	invalidateAllRows(): void;
    	private queuePostProcessedRowForCleanup;
    	private queuePostProcessedCellForCleanup;
    	private removeRowFromCache;
    	invalidateRows(rows: number[]): void;
    	invalidateRow(row: number): void;
    	updateCell(row: number, cell: number): void;
    	private updateCellWithFormatter;
    	updateRow(row: number): void;
    	private calcViewportSize;
    	resizeCanvas: () => void;
    	updatePagingStatusFromView(pagingInfo: {
    		pageSize: number;
    		pageNum: number;
    		totalPages: number;
    	}): void;
    	private updateRowCount;
    	/**
    	 * @param viewportTop optional viewport top
    	 * @param viewportLeft optional viewport left
    	 * @returns viewport range
    	 */
    	getViewport(viewportTop?: number, viewportLeft?: number): ViewRange;
    	getVisibleRange(viewportTop?: number, viewportLeft?: number): ViewRange;
    	getRenderedRange(viewportTop?: number, viewportLeft?: number): ViewRange;
    	private ensureCellNodesInRowsCache;
    	private cleanUpCells;
    	private cleanUpAndRenderCells;
    	private renderRows;
    	private startPostProcessing;
    	private startPostProcessingCleanup;
    	private invalidatePostProcessingResults;
    	private updateRowPositions;
    	private updateGrandTotals;
    	groupTotalsFormatter(p1?: GroupTotals<TItem>, p2?: Column<TItem>, grid?: any): string;
    	render: () => void;
    	private handleHeaderRowScroll;
    	private handleFooterRowScroll;
    	private handleMouseWheel;
    	private handleScroll;
    	private asyncPostProcessRows;
    	private asyncPostProcessCleanupRows;
    	private updateCellCssStylesOnRenderedRows;
    	addCellCssStyles(key: string, hash: CellStylesHash): void;
    	removeCellCssStyles(key: string): void;
    	setCellCssStyles(key: string, hash: CellStylesHash): void;
    	getCellCssStyles(key: string): CellStylesHash;
    	flashCell(row: number, cell: number, speed?: number): void;
    	private handleDragInit;
    	private handleDragStart;
    	private handleDrag;
    	private handleDragEnd;
    	private handleKeyDown;
    	private handleClick;
    	private handleContextMenu;
    	private handleDblClick;
    	private handleHeaderMouseEnter;
    	private handleHeaderMouseLeave;
    	private handleHeaderContextMenu;
    	private handleHeaderClick;
    	private handleMouseEnter;
    	private handleMouseLeave;
    	private cellExists;
    	getCellFromPoint(x: number, y: number): {
    		row: number;
    		cell: number;
    	};
    	getCellFromNode(cellNode: Element): number;
    	getColumnFromNode(cellNode: Element): Column<TItem>;
    	getRowFromNode(rowNode: Element): number;
    	getCellFromEvent(e: any): {
    		row: number;
    		cell: number;
    	};
    	getCellNodeBox(row: number, cell: number): {
    		top: number;
    		right: number;
    		bottom: number;
    		left: number;
    	};
    	resetActiveCell(): void;
    	focus(): void;
    	private setFocus;
    	scrollCellIntoView(row: number, cell: number, doPaging?: boolean): void;
    	scrollColumnIntoView(cell: number): void;
    	private internalScrollColumnIntoView;
    	private setActiveCellInternal;
    	clearTextSelection(): void;
    	private isCellPotentiallyEditable;
    	private makeActiveCellNormal;
    	editActiveCell(editor?: EditorClass): void;
    	private makeActiveCellEditable;
    	private commitEditAndSetFocus;
    	private cancelEditAndSetFocus;
    	private getActiveCellPosition;
    	getGridPosition(): Position;
    	private handleActiveCellPositionChange;
    	getCellEditor(): Editor;
    	getActiveCell(): RowCell;
    	getActiveCellNode(): HTMLElement;
    	scrollActiveCellIntoView(): void;
    	scrollRowIntoView(row: number, doPaging?: boolean): void;
    	scrollRowToTop(row: number): void;
    	private scrollPage;
    	navigatePageDown(): void;
    	navigatePageUp(): void;
    	navigateTop(): void;
    	navigateBottom(): void;
    	navigateToRow(row: number): boolean;
    	getColspan(row: number, cell: number): number;
    	navigateRight(): boolean;
    	navigateLeft(): boolean;
    	navigateDown(): boolean;
    	navigateUp(): boolean;
    	navigateNext(): boolean;
    	navigatePrev(): boolean;
    	navigateRowStart(): boolean;
    	navigateRowEnd(): boolean;
    	/**
    	 * @param {string} dir Navigation direction.
    	 * @return {boolean} Whether navigation resulted in a change of active cell.
    	 */
    	navigate(dir: string): boolean;
    	getCellNode(row: number, cell: number): HTMLElement;
    	setActiveCell(row: number, cell: number): void;
    	setActiveRow(row: number, cell: number, suppressScrollIntoView?: boolean): void;
    	canCellBeActive(row: number, cell: number): boolean;
    	canCellBeSelected(row: number, cell: number): any;
    	gotoCell(row: number, cell: number, forceEdit?: boolean): void;
    	commitCurrentEdit(): boolean;
    	private cancelCurrentEdit;
    	private rowsToRanges;
    	getSelectedRows(): number[];
    	setSelectedRows(rows: number[]): void;
    }
    interface ArgsGrid {
    	grid?: Grid;
    }
    interface ArgsColumn extends ArgsGrid {
    	column: Column;
    }
    interface ArgsColumnNode extends ArgsColumn {
    	node: HTMLElement;
    }
    export type ArgsSortCol = {
    	sortCol: Column;
    	sortAsc: boolean;
    };
    interface ArgsSort extends ArgsGrid {
    	multiColumnSort: boolean;
    	sortAsc?: boolean;
    	sortCol?: Column;
    	sortCols?: ArgsSortCol[];
    }
    interface ArgsSelectedRowsChange extends ArgsGrid {
    	rows: number[];
    	changedSelectedRows?: number[];
    	changedUnselectedRows?: number[];
    	previousSelectedRows?: number[];
    	caller: any;
    }
    interface ArgsScroll extends ArgsGrid {
    	scrollLeft: number;
    	scrollTop: number;
    }
    interface ArgsCssStyle extends ArgsGrid {
    	key: string;
    	hash: CellStylesHash;
    }
    interface ArgsCell extends ArgsGrid {
    	row: number;
    	cell: number;
    }
    interface ArgsCellChange extends ArgsCell {
    	item: any;
    }
    interface ArgsCellEdit extends ArgsCellChange {
    	column: Column;
    }
    interface ArgsAddNewRow extends ArgsColumn {
    	item: any;
    }
    interface ArgsEditorDestroy extends ArgsGrid {
    	editor: Editor;
    }
    interface ArgsValidationError extends ArgsCell {
    	editor: Editor;
    	column: Column;
    	cellNode: HTMLElement;
    	validationResults: ValidationResult;
    }
    const BasicLayout: {
    	new (): LayoutEngine;
    };
    const FrozenLayout: {
    	new (): LayoutEngine;
    };
    function PercentCompleteFormatter(ctx: FormatterContext): string;
    function PercentCompleteBarFormatter(ctx: FormatterContext): string;
    function YesNoFormatter(ctx: FormatterContext): "Yes" | "No";
    function CheckboxFormatter(ctx: FormatterContext): string;
    function CheckmarkFormatter(ctx: FormatterContext): "" | "<i class=\"slick-checkmark\"></i>";
    namespace Formatters {
    	function PercentComplete(_row: number, _cell: number, value: any): string;
    	function PercentCompleteBar(_row: number, _cell: number, value: any): string;
    	function YesNo(_row: number, _cell: number, value: any): "Yes" | "No";
    	function Checkbox(_row: number, _cell: number, value: any): string;
    	function Checkmark(_row: number, _cell: number, value: any): "" | "<i class=\"slick-checkmark\"></i>";
    }
    abstract class BaseEditor {
    	protected _input: HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement;
    	protected _defaultValue: any;
    	protected _args: EditorOptions;
    	constructor(args: EditorOptions);
    	abstract init(): void;
    	destroy(): void;
    	focus(): void;
    	getValue(): string;
    	setValue(val: string): void;
    	loadValue(item: any): void;
    	serializeValue(): any;
    	applyValue(item: any, state: any): void;
    	isValueChanged(): boolean;
    	validate(): ValidationResult;
    }
    class TextEditor extends BaseEditor {
    	_input: HTMLInputElement;
    	init(): void;
    }
    class IntegerEditor extends TextEditor {
    	serializeValue(): number;
    	validate(): ValidationResult;
    }
    class FloatEditor extends TextEditor {
    	static AllowEmptyValue: boolean;
    	static DefaultDecimalPlaces: number;
    	getDecimalPlaces(): number;
    	loadValue(item: any): void;
    	serializeValue(): any;
    	validate(): ValidationResult;
    }
    class DateEditor extends TextEditor {
    	private _calendarOpen;
    	init(): void;
    	destroy(): void;
    	show(): void;
    	hide(): void;
    	position(position: Position): void;
    }
    class YesNoSelectEditor extends BaseEditor {
    	_input: HTMLSelectElement;
    	init(): void;
    	loadValue(item: any): void;
    	serializeValue(): boolean;
    	isValueChanged(): boolean;
    	validate(): {
    		valid: boolean;
    		msg: string;
    	};
    }
    class CheckboxEditor extends BaseEditor {
    	_input: HTMLInputElement;
    	init(): void;
    	loadValue(item: any): void;
    	preClick(): void;
    	serializeValue(): boolean;
    	applyValue(item: any, state: any): void;
    	isValueChanged(): boolean;
    	validate(): {
    		valid: boolean;
    		msg: string;
    	};
    }
    class PercentCompleteEditor extends IntegerEditor {
    	protected _picker: HTMLDivElement;
    	init(): void;
    	destroy(): void;
    }
    class LongTextEditor extends BaseEditor {
    	_input: HTMLTextAreaElement;
    	protected _container: HTMLElement;
    	protected _wrapper: HTMLDivElement;
    	init(): void;
    	handleKeyDown(e: KeyboardEvent): void;
    	save(): void;
    	cancel(): void;
    	hide(): void;
    	show(): void;
    	position(position: Position): void;
    	destroy(): void;
    }
    namespace Editors {
    	const Text: typeof TextEditor;
    	const Integer: typeof IntegerEditor;
    	const Float: typeof FloatEditor;
    	const Date: typeof DateEditor;
    	const YesNoSelect: typeof YesNoSelectEditor;
    	const Checkbox: typeof CheckboxEditor;
    	const PercentComplete: typeof PercentCompleteEditor;
    	const LongText: typeof LongTextEditor;
    }
    interface GroupItemMetadataProviderOptions {
    	enableExpandCollapse?: boolean;
    	groupCellCssClass?: string;
    	groupCssClass?: string;
    	groupIndentation?: number;
    	groupFocusable?: boolean;
    	groupFormat?: ColumnFormat<Group>;
    	groupFormatter?: CompatFormatter<Group>;
    	groupLevelPrefix?: string;
    	groupRowTotals?: boolean;
    	groupTitleCssClass?: string;
    	hasSummaryType?: (column: Column) => boolean;
    	toggleCssClass?: string;
    	toggleExpandedCssClass?: string;
    	toggleCollapsedCssClass?: string;
    	totalsCssClass?: string;
    	totalsFocusable?: boolean;
    	totalsFormat?: ColumnFormat<GroupTotals>;
    	totalsFormatter?: CompatFormatter<GroupTotals>;
    }
    class GroupItemMetadataProvider {
    	protected grid: Grid;
    	private options;
    	constructor(opt?: GroupItemMetadataProviderOptions);
    	static readonly defaults: GroupItemMetadataProviderOptions;
    	static defaultGroupFormat(ctx: FormatterContext, opt?: GroupItemMetadataProviderOptions): string;
    	static defaultTotalsFormat(ctx: FormatterContext, grid?: Grid): string;
    	init(grid: Grid): void;
    	readonly pluginName = "GroupItemMetadataProvider";
    	destroy(): void;
    	getOptions(): GroupItemMetadataProviderOptions;
    	setOptions(value: GroupItemMetadataProviderOptions): void;
    	handleGridClick: (e: MouseEvent, args: ArgsCell) => void;
    	handleGridKeyDown: (e: KeyboardEvent, args: ArgsCell) => void;
    	groupCellPosition: () => {
    		cell: number;
    		colspan: number | "*";
    	};
    	getGroupRowMetadata: ((item: Group) => ItemMetadata);
    	getTotalsRowMetadata: ((item: GroupTotals) => ItemMetadata);
    }
    interface AutoTooltipsOptions {
    	enableForCells?: boolean;
    	enableForHeaderCells?: boolean;
    	maxToolTipLength?: number;
    	replaceExisting?: boolean;
    }
    class AutoTooltips implements IPlugin {
    	private grid;
    	private options;
    	constructor(options?: AutoTooltipsOptions);
    	static readonly defaults: AutoTooltipsOptions;
    	init(grid: Grid): void;
    	destroy(): void;
    	private handleMouseEnter;
    	private handleHeaderMouseEnter;
    	pluginName: string;
    }
    interface RowMoveManagerOptions {
    	cancelEditOnDrag?: boolean;
    }
    interface ArgsMoveRows {
    	rows: number[];
    	insertBefore: number;
    }
    class RowMoveManager implements IPlugin {
    	private grid;
    	private options;
    	private dragging;
    	private handler;
    	onBeforeMoveRows: EventEmitter<ArgsMoveRows, IEventData>;
    	onMoveRows: EventEmitter<ArgsMoveRows, IEventData>;
    	constructor(options?: RowMoveManagerOptions);
    	static readonly defaults: RowMoveManagerOptions;
    	init(grid: Grid): void;
    	destroy(): void;
    	private handleDragInit;
    	private handleDragStart;
    	private handleDrag;
    	private handleDragEnd;
    }
    interface RowSelectionModelOptions {
    	selectActiveRow?: boolean;
    }
    class RowSelectionModel implements IPlugin, SelectionModel {
    	private grid;
    	private handler;
    	private inHandler;
    	private options;
    	private ranges;
    	onSelectedRangesChanged: EventEmitter<CellRange[], IEventData>;
    	constructor(options?: RowSelectionModelOptions);
    	static readonly defaults: RowSelectionModelOptions;
    	init(grid: Grid): void;
    	destroy(): void;
    	private wrapHandler;
    	private rowsToRanges;
    	getSelectedRows(): number[];
    	setSelectedRows(rows: number[]): void;
    	setSelectedRanges(ranges: CellRange[]): void;
    	getSelectedRanges(): CellRange[];
    	private handleActiveCellChange;
    	private handleKeyDown;
    	private handleClick;
    }
}


declare namespace Slick {
    interface Column<TItem = any> {
        referencedFields?: string[];
        sourceItem?: Serenity.PropertyItem;
    }
}

declare namespace Serenity {
    /**
     * Tries to block the page
     */
    function blockUI(options?: {
        zIndex?: number;
        useTimeout?: boolean;
    }): void;
    /**
     * Unblocks the page.
     */
    function blockUndo(): void;

    var Config: {
        /**
         * This is the root path of your application. If your application resides under http://localhost/mysite/,
         * your root path is "/mysite/". This variable is automatically initialized by reading from a <link> element
         * with ID "ApplicationPath" from current page, which is usually located in your _LayoutHead.cshtml file
         */
        applicationPath: string;
        /**
         * Email validation by default only allows ASCII characters. Set this to true if you want to allow unicode.
         */
        emailAllowOnlyAscii: boolean;
        /**
         * This is the list of root namespaces that may be searched for types. For example, if you specify an editor type
         * of "MyEditor", first a class with name "MyEditor" will be searched, if not found, search will be followed by
         * "Serenity.MyEditor" and "MyApp.MyEditor" if you added "MyApp" to the list of root namespaces.
         *
         * You should usually add your application root namespace to this list in ScriptInit(ialization).ts file.
         */
        rootNamespaces: string[];
        /**
         * This is an optional method for handling when user is not logged in. If a users session is expired
         * and when a NotAuthorized response is received from a service call, Serenity will call this handler, so
         * you may intercept it and notify user about this situation and ask if she wants to login again...
         */
        notLoggedInHandler: Function;
    };

    /**
     * CriteriaBuilder is a class that allows to build unary or binary criteria with completion support.
     */
    class CriteriaBuilder extends Array {
        /**
         * Creates a between criteria.
         * @param fromInclusive from value
         * @param toInclusive to value
         */
        bw(fromInclusive: any, toInclusive: any): Array<any>;
        /**
         * Creates a contains criteria
         * @param value contains value
         */
        contains(value: string): Array<any>;
        /**
         * Creates a endsWith criteria
         * @param value endsWith value
         */
        endsWith(value: string): Array<any>;
        /**
         * Creates an equal (=) criteria
         * @param value equal value
         */
        eq(value: any): Array<any>;
        /**
         * Creates a greater than criteria
         * @param value greater than value
         */
        gt(value: any): Array<any>;
        /**
         * Creates a greater than or equal criteria
         * @param value greater than or equal value
         */
        ge(value: any): Array<any>;
        /**
         * Creates a in criteria
         * @param values in values
         */
        in(values: any[]): Array<any>;
        /**
         * Creates a IS NULL criteria
         */
        isNull(): Array<any>;
        /**
         * Creates a IS NOT NULL criteria
         */
        isNotNull(): Array<any>;
        /**
         * Creates a less than or equal to criteria
         * @param value less than or equal to value
         */
        le(value: any): Array<any>;
        /**
         * Creates a less than criteria
         * @param value less than value
         */
        lt(value: any): Array<any>;
        /**
         * Creates a not equal criteria
         * @param value not equal value
         */
        ne(value: any): Array<any>;
        /**
         * Creates a LIKE criteria
         * @param value like value
         */
        like(value: any): Array<any>;
        /**
         * Creates a STARTS WITH criteria
         * @param value startsWith value
         */
        startsWith(value: string): Array<any>;
        /**
         * Creates a NOT IN criteria
         * @param values array of NOT IN values
         */
        notIn(values: any[]): Array<any>;
        /**
         * Creates a NOT LIKE criteria
         * @param value not like value
         */
        notLike(value: any): Array<any>;
    }
    /**
     * Parses a criteria expression to Serenity Criteria array format.
     * The string may optionally contain parameters like `A >= @p1 and B < @p2`.
     * @param expression The criteria expression.
     * @param params The dictionary containing parameter values like { p1: 10, p2: 20 }.
     * @example
     * parseCriteria('A >= @p1 and B < @p2', { p1: 5, p2: 4 }) // [[[a], '>=' 5], 'and', [[b], '<', 4]]
     */
    function parseCriteria(expression: string, params?: any): any[];
    /**
     * Parses a criteria expression to Serenity Criteria array format.
     * The expression may contain parameter placeholders like `A >= ${p1}`
     * where p1 is a variable in the scope.
     * @param strings The string fragments.
     * @param values The tagged template arguments.
     * @example
     * var a = 5, b = 4;
     * parseCriteria`A >= ${a} and B < ${b}` // [[[a], '>=' 5], 'and', [[b], '<', 4]]
     */
    function parseCriteria(strings: TemplateStringsArray, ...values: any[]): any[];
    /**
     * Enumeration of Criteria operator keys.
     */
    enum CriteriaOperator {
        paren = "()",
        not = "not",
        isNull = "is null",
        isNotNull = "is not null",
        exists = "exists",
        and = "and",
        or = "or",
        xor = "xor",
        eq = "=",
        ne = "!=",
        gt = ">",
        ge = ">=",
        lt = "<",
        le = "<=",
        in = "in",
        notIn = "not in",
        like = "like",
        notLike = "not like"
    }
    /**
     * Creates a new criteria builder containg the passed field name.
     * @param field The field name.
     */
    function Criteria(field: string): CriteriaBuilder;
    namespace Criteria {
        var and: (c1: any[], c2: any[], ...rest: any[][]) => any[];
        var Operator: typeof CriteriaOperator;
        var isEmpty: (c: any[]) => boolean;
        var join: (c1: any[], op: string, c2: any[]) => any[];
        var not: (c: any[]) => (string | any[])[];
        var or: (c1: any[], c2: any[], ...rest: any[][]) => any[];
        var paren: (c: any[]) => any[];
        var parse: typeof parseCriteria;
    }

    interface DebouncedFunction<T extends (...args: any[]) => any> {
        /**
         * Call the original function, but applying the debounce rules.
         *
         * If the debounced function can be run immediately, this calls it and returns its return
         * value.
         *
         * Otherwise, it returns the return value of the last invocation, or undefined if the debounced
         * function was not invoked yet.
         */
        (...args: Parameters<T>): ReturnType<T> | undefined;
        /**
         * Throw away any pending invocation of the debounced function.
         */
        clear(): void;
        /**
         * If there is a pending invocation of the debounced function, invoke it immediately and return
         * its return value.
         *
         * Otherwise, return the value from the last invocation, or undefined if the debounced function
         * was never invoked.
         */
        flush(): ReturnType<T> | undefined;
    }
    /**
     * Returns a function, that, as long as it continues to be invoked, will not
     * be triggered. The function also has a property 'clear' that can be used
     * to clear the timer to prevent previously scheduled executions, and flush method
     * to invoke scheduled executions now if any.
     * @param wait The function will be called after it stops being called for
     * N milliseconds.
     * @param immediate If passed, trigger the function on the leading edge, instead of the trailing.
     *
     * @source underscore.js
     */
    function debounce<T extends (...args: any) => any>(func: T, wait?: number, immediate?: boolean): DebouncedFunction<T>;

    type UtilityColor = "primary" | "secondary" | "success" | "danger" | "warning" | "info" | "light" | "dark" | "muted" | "white";
    type TextColor = UtilityColor | "aqua" | "blue" | "fuschia" | "gray" | "green" | "light-blue" | "lime" | "maroon" | "navy" | "olive" | "orange" | "purple" | "red" | "teal" | "yellow";
    function bgColor(color: UtilityColor): string;
    function textColor(color: TextColor): string;
    function faIcon(key: faIconKey, color?: TextColor): string;
    function fabIcon(key: fabIconKey, color?: TextColor): string;
    type KnownIconClass = `fa fa-${faIconKey}` | `fab fa-${fabIconKey}`;
    type AnyIconClass = KnownIconClass | (string & {});
    type IconClassName = AnyIconClass | (AnyIconClass[]);
    function iconClassName(icon: IconClassName): string;
    type faIconKey = "ad" | "address-book" | "address-card" | "adjust" | "air-freshener" | "align-center" | "align-justify" | "align-left" | "align-right" | "allergies" | "ambulance" | "american-sign-language-interpreting" | "anchor" | "angle-double-down" | "angle-double-left" | "angle-double-right" | "angle-double-up" | "angle-down" | "angle-left" | "angle-right" | "angle-up" | "angry" | "ankh" | "apple-alt" | "archive" | "archway" | "arrow-alt-circle-down" | "arrow-alt-circle-left" | "arrow-alt-circle-right" | "arrow-alt-circle-up" | "arrow-circle-down" | "arrow-circle-left" | "arrow-circle-right" | "arrow-circle-up" | "arrow-down" | "arrow-left" | "arrow-right" | "arrow-up" | "arrows-alt" | "arrows-alt-h" | "arrows-alt-v" | "assistive-listening-systems" | "asterisk" | "at" | "atlas" | "atom" | "audio-description" | "award" | "baby" | "baby-carriage" | "backspace" | "backward" | "bacon" | "balance-scale" | "balance-scale-left" | "balance-scale-right" | "ban" | "band-aid" | "barcode" | "bars" | "baseball-ball" | "basketball-ball" | "bath" | "battery-empty" | "battery-full" | "battery-half" | "battery-quarter" | "battery-three-quarters" | "bed" | "beer" | "bell" | "bell-o" | "bell-slash" | "bezier-curve" | "bible" | "bicycle" | "biking" | "binoculars" | "biohazard" | "birthday-cake" | "blender" | "blender-phone" | "blind" | "blog" | "bold" | "bolt" | "bomb" | "bone" | "bong" | "book" | "book-dead" | "book-medical" | "book-open" | "book-reader" | "bookmark" | "border-all" | "border-none" | "border-style" | "bowling-ball" | "box" | "box-open" | "boxes" | "braille" | "brain" | "bread-slice" | "briefcase" | "briefcase-medical" | "broadcast-tower" | "broom" | "brush" | "bug" | "building" | "bullhorn" | "bullseye" | "burn" | "bus" | "bus-alt" | "business-time" | "calculator" | "calendar" | "calendar-alt" | "calendar-check" | "calendar-day" | "calendar-minus" | "calendar-plus" | "calendar-times" | "calendar-week" | "camera" | "camera-retro" | "campground" | "candy-cane" | "cannabis" | "capsules" | "car" | "car-alt" | "car-battery" | "car-crash" | "car-side" | "caret-down" | "caret-left" | "caret-right" | "caret-square-down" | "caret-square-left" | "caret-square-right" | "caret-square-up" | "caret-up" | "carrot" | "cart-arrow-down" | "cart-plus" | "cash-register" | "cat" | "certificate" | "chair" | "chalkboard" | "chalkboard-teacher" | "charging-station" | "chart-area" | "chart-bar" | "chart-line" | "chart-pie" | "check" | "check-circle" | "check-double" | "check-square" | "cheese" | "chess" | "chess-bishop" | "chess-board" | "chess-king" | "chess-knight" | "chess-pawn" | "chess-queen" | "chess-rook" | "chevron-circle-down" | "chevron-circle-left" | "chevron-circle-right" | "chevron-circle-up" | "chevron-down" | "chevron-left" | "chevron-right" | "chevron-up" | "child" | "church" | "circle" | "circle-notch" | "city" | "clinic-medical" | "clipboard" | "clipboard-check" | "clipboard-list" | "clock" | "clock-o" | "clone" | "closed-captioning" | "cloud" | "cloud-download-alt" | "cloud-meatball" | "cloud-moon" | "cloud-moon-rain" | "cloud-rain" | "cloud-showers-heavy" | "cloud-sun" | "cloud-sun-rain" | "cloud-upload-alt" | "cocktail" | "code" | "code-branch" | "coffee" | "cog" | "cogs" | "coins" | "columns" | "comment" | "comment-alt" | "comment-dollar" | "comment-dots" | "comment-medical" | "comment-slash" | "comments" | "comments-dollar" | "compact-disc" | "compass" | "compress" | "compress-arrows-alt" | "concierge-bell" | "cookie" | "cookie-bite" | "copy" | "copyright" | "couch" | "credit-card" | "crop" | "crop-alt" | "cross" | "crosshairs" | "crow" | "crown" | "crutch" | "cube" | "cubes" | "cut" | "database" | "deaf" | "democrat" | "desktop" | "dharmachakra" | "diagnoses" | "dice" | "dice-d20" | "dice-d6" | "dice-five" | "dice-four" | "dice-one" | "dice-six" | "dice-three" | "dice-two" | "digital-tachograph" | "directions" | "divide" | "dizzy" | "dna" | "dog" | "dollar-sign" | "dolly" | "dolly-flatbed" | "donate" | "door-closed" | "door-open" | "dot-circle" | "dove" | "download" | "drafting-compass" | "dragon" | "draw-polygon" | "drum" | "drum-steelpan" | "drumstick-bite" | "dumbbell" | "dumpster" | "dumpster-fire" | "dungeon" | "edit" | "egg" | "eject" | "ellipsis-h" | "ellipsis-v" | "envelope" | "envelope-o" | "envelope-open" | "envelope-open-text" | "envelope-square" | "equals" | "eraser" | "ethernet" | "euro-sign" | "exchange-alt" | "exclamation" | "exclamation-circle" | "exclamation-triangle" | "expand" | "expand-arrows-alt" | "external-link-alt" | "external-link-square-alt" | "eye" | "eye-dropper" | "eye-slash" | "fan" | "fast-backward" | "fast-forward" | "fax" | "feather" | "feather-alt" | "female" | "fighter-jet" | "file" | "file-alt" | "file-archive" | "file-audio" | "file-code" | "file-contract" | "file-csv" | "file-download" | "file-excel" | "file-excel-o" | "file-export" | "file-image" | "file-import" | "file-invoice" | "file-invoice-dollar" | "file-medical" | "file-medical-alt" | "file-pdf" | "file-pdf-o" | "file-powerpoint" | "file-prescription" | "file-signature" | "file-upload" | "file-text" | "file-text-o" | "file-video" | "file-word" | "fill" | "fill-drip" | "film" | "filter" | "fingerprint" | "fire" | "floppy-o" | "fire-alt" | "fire-extinguisher" | "first-aid" | "fish" | "fist-raised" | "flag" | "flag-checkered" | "flag-usa" | "flask" | "flushed" | "folder" | "folder-minus" | "folder-open" | "folder-open-o" | "folder-plus" | "font" | "football-ball" | "forward" | "frog" | "frown" | "frown-open" | "funnel-dollar" | "futbol" | "gamepad" | "gas-pump" | "gavel" | "gem" | "genderless" | "ghost" | "gift" | "gifts" | "glass-cheers" | "glass-martini" | "glass-martini-alt" | "glass-whiskey" | "glasses" | "globe" | "globe-africa" | "globe-americas" | "globe-asia" | "globe-europe" | "golf-ball" | "gopuram" | "graduation-cap" | "greater-than" | "greater-than-equal" | "grimace" | "grin" | "grin-alt" | "grin-beam" | "grin-beam-sweat" | "grin-hearts" | "grin-squint" | "grin-squint-tears" | "grin-stars" | "grin-tears" | "grin-tongue" | "grin-tongue-squint" | "grin-tongue-wink" | "grin-wink" | "grip-horizontal" | "grip-lines" | "grip-lines-vertical" | "grip-vertical" | "guitar" | "h-square" | "hamburger" | "hammer" | "hamsa" | "hand-holding" | "hand-holding-heart" | "hand-holding-usd" | "hand-lizard" | "hand-middle-finger" | "hand-paper" | "hand-peace" | "hand-point-down" | "hand-point-left" | "hand-point-right" | "hand-point-up" | "hand-pointer" | "hand-rock" | "hand-scissors" | "hand-spock" | "hands" | "hands-helping" | "handshake" | "hanukiah" | "hard-hat" | "hashtag" | "hat-cowboy" | "hat-cowboy-side" | "hat-wizard" | "haykal" | "hdd" | "heading" | "headphones" | "headphones-alt" | "headset" | "heart" | "heart-broken" | "heartbeat" | "helicopter" | "highlighter" | "hiking" | "hippo" | "history" | "hockey-puck" | "holly-berry" | "home" | "horse" | "horse-head" | "hospital" | "hospital-alt" | "hospital-symbol" | "hot-tub" | "hotdog" | "hotel" | "hourglass" | "hourglass-end" | "hourglass-half" | "hourglass-start" | "house-damage" | "hryvnia" | "i-cursor" | "ice-cream" | "icicles" | "icons" | "id-badge" | "id-card" | "id-card-alt" | "igloo" | "image" | "images" | "inbox" | "indent" | "industry" | "infinity" | "info" | "info-circle" | "italic" | "jedi" | "joint" | "journal-whills" | "kaaba" | "key" | "keyboard" | "khanda" | "kiss" | "kiss-beam" | "kiss-wink-heart" | "kiwi-bird" | "landmark" | "language" | "laptop" | "laptop-code" | "laptop-medical" | "laugh" | "laugh-beam" | "laugh-squint" | "laugh-wink" | "layer-group" | "leaf" | "lemon" | "less-than" | "less-than-equal" | "level-down-alt" | "level-up-alt" | "life-ring" | "lightbulb" | "link" | "lira-sign" | "list" | "list-alt" | "list-ol" | "list-ul" | "location-arrow" | "lock" | "lock-open" | "long-arrow-alt-down" | "long-arrow-alt-left" | "long-arrow-alt-right" | "long-arrow-alt-up" | "low-vision" | "luggage-cart" | "magic" | "magnet" | "mail-bulk" | "mail-forward" | "mail-reply" | "male" | "map" | "map-marked" | "map-marked-alt" | "map-marker" | "map-marker-alt" | "map-pin" | "map-signs" | "marker" | "mars" | "mars-double" | "mars-stroke" | "mars-stroke-h" | "mars-stroke-v" | "mask" | "medal" | "medkit" | "meh" | "meh-blank" | "meh-rolling-eyes" | "memory" | "menorah" | "mercury" | "meteor" | "microchip" | "microphone" | "microphone-alt" | "microphone-alt-slash" | "microphone-slash" | "microscope" | "minus" | "minus-circle" | "minus-square" | "mitten" | "mobile" | "mobile-alt" | "money-bill" | "money-bill-alt" | "money-bill-wave" | "money-bill-wave-alt" | "money-check" | "money-check-alt" | "monument" | "moon" | "mortar-pestle" | "mosque" | "motorcycle" | "mountain" | "mouse" | "mouse-pointer" | "mug-hot" | "music" | "network-wired" | "neuter" | "newspaper" | "not-equal" | "notes-medical" | "object-group" | "object-ungroup" | "oil-can" | "om" | "otter" | "outdent" | "pager" | "paint-brush" | "paint-roller" | "palette" | "pallet" | "paper-plane" | "paperclip" | "parachute-box" | "paragraph" | "parking" | "passport" | "pastafarianism" | "paste" | "pause" | "pause-circle" | "paw" | "peace" | "pen" | "pen-alt" | "pen-fancy" | "pen-nib" | "pen-square" | "pencil-alt" | "pencil-ruler" | "pencil-square-o" | "people-carry" | "pepper-hot" | "percent" | "percentage" | "person-booth" | "phone" | "phone-alt" | "phone-slash" | "phone-square" | "phone-square-alt" | "phone-volume" | "photo-video" | "piggy-bank" | "pills" | "pizza-slice" | "place-of-worship" | "plane" | "plane-arrival" | "plane-departure" | "play" | "play-circle" | "plug" | "plus" | "plus-circle" | "plus-square" | "podcast" | "poll" | "poll-h" | "poo" | "poo-storm" | "poop" | "portrait" | "pound-sign" | "power-off" | "pray" | "praying-hands" | "prescription" | "prescription-bottle" | "prescription-bottle-alt" | "print" | "procedures" | "project-diagram" | "puzzle-piece" | "qrcode" | "question" | "question-circle" | "quidditch" | "quote-left" | "quote-right" | "quran" | "radiation" | "radiation-alt" | "rainbow" | "random" | "receipt" | "record-vinyl" | "recycle" | "redo" | "refresh" | "redo-alt" | "registered" | "remove-format" | "reply" | "reply-all" | "republican" | "restroom" | "retweet" | "ribbon" | "ring" | "road" | "robot" | "rocket" | "route" | "rss" | "rss-square" | "ruble-sign" | "ruler" | "ruler-combined" | "ruler-horizontal" | "ruler-vertical" | "running" | "rupee-sign" | "sad-cry" | "sad-tear" | "satellite" | "satellite-dish" | "save" | "school" | "screwdriver" | "scroll" | "sd-card" | "search" | "search-dollar" | "search-location" | "search-minus" | "search-plus" | "seedling" | "server" | "shapes" | "share" | "share-alt" | "share-alt-square" | "share-square" | "shekel-sign" | "shield-alt" | "ship" | "shipping-fast" | "shoe-prints" | "shopping-bag" | "shopping-basket" | "shopping-cart" | "shower" | "shuttle-van" | "sign" | "sign-in-alt" | "sign-language" | "sign-out" | "sign-out-alt" | "signal" | "signature" | "sim-card" | "sitemap" | "skating" | "skiing" | "skiing-nordic" | "skull" | "skull-crossbones" | "slash" | "sleigh" | "sliders-h" | "smile" | "smile-beam" | "smile-wink" | "smog" | "smoking" | "smoking-ban" | "sms" | "snowboarding" | "snowflake" | "snowman" | "snowplow" | "socks" | "solar-panel" | "sort" | "sort-alpha-down" | "sort-alpha-down-alt" | "sort-alpha-up" | "sort-alpha-up-alt" | "sort-amount-down" | "sort-amount-down-alt" | "sort-amount-up" | "sort-amount-up-alt" | "sort-down" | "sort-numeric-down" | "sort-numeric-down-alt" | "sort-numeric-up" | "sort-numeric-up-alt" | "sort-up" | "spa" | "space-shuttle" | "spell-check" | "spider" | "spinner" | "splotch" | "spray-can" | "square" | "square-full" | "square-root-alt" | "stamp" | "star" | "star-and-crescent" | "star-half" | "star-half-alt" | "star-o" | "star-of-david" | "star-of-life" | "step-backward" | "step-forward" | "stethoscope" | "sticky-note" | "stop" | "stop-circle" | "stopwatch" | "store" | "store-alt" | "stream" | "street-view" | "strikethrough" | "stroopwafel" | "subscript" | "subway" | "suitcase" | "suitcase-rolling" | "sun" | "superscript" | "surprise" | "swatchbook" | "swimmer" | "swimming-pool" | "synagogue" | "sync" | "sync-alt" | "syringe" | "table" | "table-tennis" | "tablet" | "tablet-alt" | "tablets" | "tachometer-alt" | "tag" | "tags" | "tape" | "tasks" | "taxi" | "teeth" | "teeth-open" | "temperature-high" | "temperature-low" | "tenge" | "terminal" | "text-height" | "text-width" | "th" | "th-large" | "th-list" | "theater-masks" | "thermometer" | "thermometer-empty" | "thermometer-full" | "thermometer-half" | "thermometer-quarter" | "thermometer-three-quarters" | "thumbs-down" | "thumbs-up" | "thumbtack" | "ticket-alt" | "times" | "times-circle" | "tint" | "tint-slash" | "tired" | "toggle-off" | "toggle-on" | "toilet" | "toilet-paper" | "toolbox" | "tools" | "tooth" | "torah" | "torii-gate" | "tractor" | "trademark" | "traffic-light" | "train" | "tram" | "transgender" | "transgender-alt" | "trash" | "trash-alt" | "trash-o" | "trash-restore" | "trash-restore-alt" | "tree" | "trophy" | "truck" | "truck-loading" | "truck-monster" | "truck-moving" | "truck-pickup" | "tshirt" | "tty" | "tv" | "umbrella" | "umbrella-beach" | "underline" | "undo" | "undo-alt" | "universal-access" | "university" | "unlink" | "unlock" | "unlock-alt" | "upload" | "user" | "user-alt" | "user-alt-slash" | "user-astronaut" | "user-check" | "user-circle" | "user-clock" | "user-cog" | "user-edit" | "user-friends" | "user-graduate" | "user-injured" | "user-lock" | "user-md" | "user-minus" | "user-ninja" | "user-nurse" | "user-plus" | "user-secret" | "user-shield" | "user-slash" | "user-tag" | "user-tie" | "user-times" | "users" | "users-cog" | "utensil-spoon" | "utensils" | "vector-square" | "venus" | "venus-double" | "venus-mars" | "vial" | "vials" | "video" | "video-slash" | "vihara" | "voicemail" | "volleyball-ball" | "volume-down" | "volume-mute" | "volume-off" | "volume-up" | "vote-yea" | "vr-cardboard" | "walking" | "wallet" | "warehouse" | "water" | "wave-square" | "weight" | "weight-hanging" | "wheelchair" | "wifi" | "wind" | "window-close" | "window-maximize" | "window-minimize" | "window-restore" | "wine-bottle" | "wine-glass" | "wine-glass-alt" | "won-sign" | "wrench" | "x-ray" | "yen-sign" | "yin-yang";
    type fabIconKey = "500px" | "accessible-icon" | "accusoft" | "acquisitions-incorporated" | "adn" | "adobe" | "adversal" | "affiliatetheme" | "airbnb" | "algolia" | "alipay" | "amazon" | "amazon-pay" | "amilia" | "android" | "angellist" | "angrycreative" | "angular" | "app-store" | "app-store-ios" | "apper" | "apple" | "apple-pay" | "artstation" | "asymmetrik" | "atlassian" | "audible" | "autoprefixer" | "avianex" | "aviato" | "aws" | "bandcamp" | "battle-net" | "behance" | "behance-square" | "bimobject" | "bitbucket" | "bitcoin" | "bity" | "black-tie" | "blackberry" | "blogger" | "blogger-b" | "bluetooth" | "bluetooth-b" | "bootstrap" | "btc" | "buffer" | "buromobelexperte" | "buy-n-large" | "buysellads" | "canadian-maple-leaf" | "cc-amazon-pay" | "cc-amex" | "cc-apple-pay" | "cc-diners-club" | "cc-discover" | "cc-jcb" | "cc-mastercard" | "cc-paypal" | "cc-stripe" | "cc-visa" | "centercode" | "centos" | "chrome" | "chromecast" | "cloudscale" | "cloudsmith" | "cloudversify" | "codepen" | "codiepie" | "confluence" | "connectdevelop" | "contao" | "cotton-bureau" | "cpanel" | "creative-commons" | "creative-commons-by" | "creative-commons-nc" | "creative-commons-nc-eu" | "creative-commons-nc-jp" | "creative-commons-nd" | "creative-commons-pd" | "creative-commons-pd-alt" | "creative-commons-remix" | "creative-commons-sa" | "creative-commons-sampling" | "creative-commons-sampling-plus" | "creative-commons-share" | "creative-commons-zero" | "critical-role" | "css3" | "css3-alt" | "cuttlefish" | "d-and-d" | "d-and-d-beyond" | "dashcube" | "delicious" | "deploydog" | "deskpro" | "dev" | "deviantart" | "dhl" | "diaspora" | "digg" | "digital-ocean" | "discord" | "discourse" | "dochub" | "docker" | "draft2digital" | "dribbble" | "dribbble-square" | "dropbox" | "drupal" | "dyalog" | "earlybirds" | "ebay" | "edge" | "elementor" | "ello" | "ember" | "empire" | "envira" | "erlang" | "ethereum" | "etsy" | "evernote" | "expeditedssl" | "facebook" | "facebook-f" | "facebook-messenger" | "facebook-square" | "fantasy-flight-games" | "fedex" | "fedora" | "figma" | "firefox" | "first-order" | "first-order-alt" | "firstdraft" | "flickr" | "flipboard" | "fly" | "font-awesome" | "font-awesome-alt" | "font-awesome-flag" | "fonticons" | "fonticons-fi" | "fort-awesome" | "fort-awesome-alt" | "forumbee" | "foursquare" | "free-code-camp" | "freebsd" | "fulcrum" | "galactic-republic" | "galactic-senate" | "get-pocket" | "gg" | "gg-circle" | "git" | "git-alt" | "git-square" | "github" | "github-alt" | "github-square" | "gitkraken" | "gitlab" | "gitter" | "glide" | "glide-g" | "gofore" | "goodreads" | "goodreads-g" | "google" | "google-drive" | "google-play" | "google-plus" | "google-plus-g" | "google-plus-square" | "google-wallet" | "gratipay" | "grav" | "gripfire" | "grunt" | "gulp" | "hacker-news" | "hacker-news-square" | "hackerrank" | "hips" | "hire-a-helper" | "hooli" | "hornbill" | "hotjar" | "houzz" | "html5" | "hubspot" | "imdb" | "instagram" | "intercom" | "internet-explorer" | "invision" | "ioxhost" | "itch-io" | "itunes" | "itunes-note" | "java" | "jedi-order" | "jenkins" | "jira" | "joget" | "joomla" | "js" | "js-square" | "jsfiddle" | "kaggle" | "keybase" | "keycdn" | "kickstarter" | "kickstarter-k" | "korvue" | "laravel" | "lastfm" | "lastfm-square" | "leanpub" | "less" | "line" | "linkedin" | "linkedin-in" | "linode" | "linux" | "lyft" | "magento" | "mailchimp" | "mandalorian" | "markdown" | "mastodon" | "maxcdn" | "mdb" | "medapps" | "medium" | "medium-m" | "medrt" | "meetup" | "megaport" | "mendeley" | "microsoft" | "mix" | "mixcloud" | "mizuni" | "modx" | "monero" | "napster" | "neos" | "nimblr" | "node" | "node-js" | "npm" | "ns8" | "nutritionix" | "odnoklassniki" | "odnoklassniki-square" | "old-republic" | "opencart" | "openid" | "opera" | "optin-monster" | "orcid" | "osi" | "page4" | "pagelines" | "palfed" | "patreon" | "paypal" | "penny-arcade" | "periscope" | "phabricator" | "phoenix-framework" | "phoenix-squadron" | "php" | "pied-piper" | "pied-piper-alt" | "pied-piper-hat" | "pied-piper-pp" | "pinterest" | "pinterest-p" | "pinterest-square" | "playstation" | "product-hunt" | "pushed" | "python" | "qq" | "quinscape" | "quora" | "r-project" | "raspberry-pi" | "ravelry" | "react" | "reacteurope" | "readme" | "rebel" | "red-river" | "reddit" | "reddit-alien" | "reddit-square" | "redhat" | "renren" | "replyd" | "researchgate" | "resolving" | "rev" | "rocketchat" | "rockrms" | "safari" | "salesforce" | "sass" | "schlix" | "scribd" | "searchengin" | "sellcast" | "sellsy" | "servicestack" | "shirtsinbulk" | "shopware" | "simplybuilt" | "sistrix" | "sith" | "sketch" | "skyatlas" | "skype" | "slack" | "slack-hash" | "slideshare" | "snapchat" | "snapchat-ghost" | "snapchat-square" | "soundcloud" | "sourcetree" | "speakap" | "speaker-deck" | "spotify" | "squarespace" | "stack-exchange" | "stack-overflow" | "stackpath" | "staylinked" | "steam" | "steam-square" | "steam-symbol" | "sticker-mule" | "strava" | "stripe" | "stripe-s" | "studiovinari" | "stumbleupon" | "stumbleupon-circle" | "superpowers" | "supple" | "suse" | "swift" | "symfony" | "teamspeak" | "telegram" | "telegram-plane" | "tencent-weibo" | "the-red-yeti" | "themeco" | "themeisle" | "think-peaks" | "trade-federation" | "trello" | "tripadvisor" | "tumblr" | "tumblr-square" | "twitch" | "twitter" | "twitter-square" | "typo3" | "uber" | "ubuntu" | "uikit" | "umbraco" | "uniregistry" | "untappd" | "ups" | "usb" | "usps" | "ussunnah" | "vaadin" | "viacoin" | "viadeo" | "viadeo-square" | "viber" | "vimeo" | "vimeo-square" | "vimeo-v" | "vine" | "vk" | "vnv" | "vuejs" | "waze" | "weebly" | "weibo" | "weixin" | "whatsapp" | "whatsapp-square" | "whmcs" | "wikipedia-w" | "windows" | "wix" | "wizards-of-the-coast" | "wolf-pack-battalion" | "wordpress" | "wordpress-simple" | "wpbeginner" | "wpexplorer" | "wpforms" | "wpressr" | "xbox" | "xing" | "xing-square" | "y-combinator" | "yahoo" | "yammer" | "yandex" | "yandex-international" | "yarn" | "yelp" | "yoast" | "youtube" | "youtube-square" | "zhihu";

    /**
     * Options for a message dialog button
     */
    interface DialogButton {
        /** Button text */
        text?: string;
        /** Button hint */
        hint?: string;
        /** Button icon */
        icon?: IconClassName;
        /** Click handler */
        click?: (e: MouseEvent) => void | false | Promise<void | false>;
        /** CSS class for button */
        cssClass?: string;
        /** The code that is returned from message dialog function when this button is clicked.
         *  If this is set, and click event will not be defaultPrevented dialog will close.
         */
        result?: string;
    }
    type DialogType = "bsmodal" | "uidialog" | "panel";
    /**
     * Options that apply to all dialog types
     */
    interface DialogOptions {
        /** Auto dispose dialog on close, default is true */
        autoDispose?: boolean;
        /** True to auto open dialog */
        autoOpen?: boolean;
        /** Backdrop type, static to make it modal, e.g. can't be closed by clicking outside */
        backdrop?: boolean | "static";
        /** List of buttons to show on the dialog */
        buttons?: DialogButton[];
        /** Vertically center modal */
        centered?: boolean;
        /** Show close button, default is true */
        closeButton?: boolean;
        /** Close dialog on escape key. Default is true for message dialogs. */
        closeOnEscape?: boolean;
        /** CSS class to use for all dialog types. Is added to the top ui-dialog, panel or modal element */
        dialogClass?: string;
        /** Dialog content/body element, or callback that will populate the content element */
        element?: HTMLElement | ArrayLike<HTMLElement> | ((element: HTMLElement) => void);
        /** Enable / disable animation. Default is false for message dialogs, true for other dialogs */
        fade?: boolean;
        /** Sets one of modal-fullscreen{-...-down} classes. Only used for bootstrap modals */
        fullScreen?: boolean | "sm-down" | "md-down" | "lg-down" | "xl-down" | "xxl-down";
        /** Modal option for jQuery UI dialog compatibility only. Not to be confused with Bootstrap modal. */
        modal?: boolean;
        /** Event handler that is called when dialog is opened */
        onOpen?: (e?: Event) => void;
        /** Event handler that is called when dialog is closed */
        onClose?: (result: string, e?: Event) => void;
        /** Prefer Bootstrap modals to jQuery UI dialogs when both are available */
        preferBSModal?: boolean;
        /** Prefer Panel even when Modal / jQuery UI is available */
        preferPanel?: boolean;
        /** Callback to get options specific to the dialog provider type */
        providerOptions?: (type: DialogType, opt: DialogOptions) => any;
        /** Scrollable, sets content of the modal to scrollable, only for Bootstrap */
        scrollable?: boolean;
        /** Size. Default is null for (500px) message dialogs, lg for normal dialogs */
        size?: "sm" | "md" | "lg" | "xl";
        /** Dialog title */
        title?: string;
        /** Only used for jQuery UI dialogs for backwards compatibility */
        width?: number;
    }
    /**
     * Wrapper for different types of dialogs, including jQuery UI, Bootstrap modals, and Panels.
     */
    class Dialog {
        private el;
        private dialogResult;
        /**
         * Creates a new dialog. The type of the dialog will be determined based on
         * the availability of jQuery UI, Bootstrap, and the options provided.
         * @param opt Optional configuration for the dialog
         */
        constructor(opt?: DialogOptions);
        /** Default set of dialog options */
        static defaults: DialogOptions;
        /** Default set of message dialog options */
        static messageDefaults: MessageDialogOptions;
        /**
         * Gets the dialog instance for the specified element.
         * @param el The dialog body element (.s-Panel, .ui-dialog-content, or .modal-body) or the root element (.modal, .ui-dialog, .s-Panel)
         * @returns The dialog instance, or null if the element is not a dialog.
         */
        static getInstance(el: HTMLElement | ArrayLike<HTMLElement>): Dialog;
        /** The result code of the button that is clicked. Also attached to the dialog element as data-dialog-result */
        get result(): string;
        /** Closes dialog setting the result to null */
        close(): this;
        /** Closes dialog with the result set to value */
        close(result: string): this;
        /**
         * Adds an event handler that is called when the dialog is closed. If the second parameter is true, the handler is called before the dialog is closed and
         * the closing can be cancelled by calling preventDefault on the event object.
         * @param handler The event handler function
         * @param before Indicates whether the handler should be called before the dialog is closed
         * @returns The dialog instance
         */
        onClose(handler: (result?: string, e?: Event) => void, before?: boolean): void;
        /**
         * Adds an event handler that is called when the dialog is opened. If the second parameter is true, the handler is called before the dialog is opened and
         * the opening can be cancelled by calling preventDefault on the event object.
         * @param handler The event handler function
         * @param before Indicates whether the handler should be called before the dialog is opened
         * @returns The dialog instance
         */
        onOpen(handler: (e?: Event) => void, before?: boolean): this;
        /** Opens the dialog */
        open(): this;
        /** Gets the title text of the dialog */
        title(): string;
        /** Sets the title text of the dialog. */
        title(value: string): this;
        /** Returns the type of the dialog, or null if no dialog on the current element or if the element is null, e.g. dialog was disposed  */
        get type(): DialogType;
        /** Gets the body/content element of the dialog */
        getContentNode(): HTMLElement;
        /** Gets the dialog element of the dialog */
        getDialogNode(): HTMLElement;
        /** Gets the node that receives events for the dialog. It's .ui-dialog-content, .modal, or .panel-body */
        getEventsNode(): HTMLElement;
        /** Gets the footer element of the dialog */
        getFooterNode(): HTMLElement;
        /** Gets the header element of the dialog */
        getHeaderNode(): HTMLElement;
        private onButtonClick;
        private createBSButtons;
        private createBSModal;
        private createPanel;
        private createUIDialog;
        /**
         * Disposes the dialog, removing it from the DOM and unbinding all event handlers.
         */
        dispose(): void;
    }
    /** Returns true if Bootstrap modal is available */
    function hasBSModal(): boolean;
    /** Returns true if jQuery UI dialog is available */
    function hasUIDialog(): boolean;
    /** Calls Bootstrap button.noConflict method if both jQuery UI and Bootstrap buttons are available in the page */
    function uiAndBSButtonNoConflict(): void;
    /**
     * Creates a dialog button which, by default, has "Yes" as caption (localized) and "ok" as the result.
     * @param opt - Optional configuration for the dialog button.
     * @returns The dialog button with the specified configuration.
     */
    function okDialogButton(opt?: DialogButton): DialogButton;
    /**
     * Creates a dialog button which, by default, has "Yes" as the caption (localized) and "yes" as the result.
     * @param opt - Optional configuration for the dialog button.
     * @returns The dialog button with the specified configuration.
     */
    function yesDialogButton(opt?: DialogButton): DialogButton;
    /**
     * Creates a dialog button which, by default, has "No" as the caption (localized) and "no" as the result.
     * @param opt - Optional configuration for the dialog button.
     * @returns The dialog button with the specified configuration.
     */
    function noDialogButton(opt?: DialogButton): DialogButton;
    /**
     * Creates a dialog button which, by default, has "Cancel" as the caption (localized) and "cancel" as the result.
     * @param opt - Optional configuration for the dialog button.
     * @returns The dialog button with the specified configuration.
     */
    function cancelDialogButton(opt?: DialogButton): DialogButton;
    /**
     * Namespace containing localizable text constants for dialogs.
     */
    namespace DialogTexts {
        /**
         * Title for alert dialogs.
         */
        const AlertTitle: string;
        /**
         * Text for the cancel button in dialogs.
         */
        const CancelButton: string;
        /**
         * Text for the close button in dialogs.
         */
        const CloseButton: string;
        /**
         * Title for confirmation dialogs.
         */
        const ConfirmationTitle: string;
        /**
         * Title for information dialogs.
         */
        const InformationTitle: string;
        /**
         * Hint for maximizing dialogs.
         */
        const MaximizeHint: string;
        /**
         * Text for the "No" button in dialogs.
         */
        const NoButton: string;
        /**
         * Text for the "OK" button in dialogs.
         */
        const OkButton: string;
        /**
         * Hint for restoring dialogs.
         */
        const RestoreHint: string;
        /**
         * Title for success dialogs.
         */
        const SuccessTitle: string;
        /**
         * Title for warning dialogs.
         */
        const WarningTitle: string;
        /**
         * Text for the "Yes" button in dialogs.
         */
        const YesButton: string;
    }
    /**
     * Options that apply to all message dialog types
     */
    interface MessageDialogOptions extends DialogOptions {
        /** HTML encode the message, default is true */
        htmlEncode?: boolean;
        /** Wrap the message in a `<pre>` element, so that line endings are preserved, default is true */
        preWrap?: boolean;
    }
    /**
     * Displays an alert dialog
     * @param message The message to display
     * @param options Additional options.
     * @see AlertOptions
     * @example
     * alertDialog("An error occured!"); }
     */
    function alertDialog(message: string, options?: MessageDialogOptions): Partial<Dialog>;
    /** Additional options for confirm dialog */
    interface ConfirmDialogOptions extends MessageDialogOptions {
        /** True to also add a cancel button */
        cancelButton?: boolean;
        /** Event handler for cancel button click */
        onCancel?: () => void;
        /** Event handler for no button click */
        onNo?: () => void;
    }
    /**
     * Display a confirmation dialog
     * @param message The message to display
     * @param onYes Callback for Yes button click
     * @param options Additional options.
     * @see ConfirmOptions
     * @example
     * confirmDialog("Are you sure you want to delete?", () => {
     *     // do something when yes is clicked
     * }
     */
    function confirmDialog(message: string, onYes: () => void, options?: ConfirmDialogOptions): Partial<Dialog>;
    /**
     * Display an information dialog
     * @param message The message to display
     * @param onOk Callback for OK button click
     * @param options Additional options.
     * @see ConfirmOptions
     * @example
     * informationDialog("Operation complete", () => {
     *     // do something when OK is clicked
     * }
     */
    function informationDialog(message: string, onOk?: () => void, options?: MessageDialogOptions): Partial<Dialog>;
    /**
     * Display a success dialog
     * @param message The message to display
     * @param onOk Callback for OK button click
     * @param options Additional options.
     * @see MessageDialogOptions
     * @example
     * successDialog("Operation complete", () => {
     *     // do something when OK is clicked
     * }
     */
    function successDialog(message: string, onOk?: () => void, options?: MessageDialogOptions): Partial<Dialog>;
    /**
     * Display a warning dialog
     * @param message The message to display
     * @param options Additional options.
     * @see MessageDialogOptions
     * @example
     * warningDialog("Something is odd!");
     */
    function warningDialog(message: string, options?: MessageDialogOptions): Partial<Dialog>;
    /** Options for `iframeDialog` **/
    interface IFrameDialogOptions {
        html?: string;
    }
    /**
     * Display a dialog that shows an HTML block in an IFRAME, which is usually returned from server callbacks
     * @param options The options
     */
    function iframeDialog(options: IFrameDialogOptions): Partial<Dialog>;

    function getjQuery(): any;
    /** Returns true if Bootstrap 3 is loaded */
    function isBS3(): boolean;
    /** Returns true if Bootstrap 5+ is loaded */
    function isBS5Plus(): boolean;

    interface ServiceError {
        Code?: string;
        Arguments?: string;
        Message?: string;
        Details?: string;
        ErrorId?: string;
    }
    interface ServiceResponse {
        Error?: ServiceError;
    }
    interface ServiceRequest {
    }
    interface SaveRequest<TEntity> extends ServiceRequest {
        EntityId?: any;
        Entity?: TEntity;
        Localizations?: any;
    }
    interface SaveRequestWithAttachment<TEntity> extends SaveRequest<TEntity> {
        Attachments?: any[];
    }
    interface SaveResponse extends ServiceResponse {
        EntityId?: any;
    }
    interface SaveWithLocalizationRequest<TEntity> extends SaveRequest<TEntity> {
        Localizations?: {
            [key: string]: TEntity;
        };
    }
    interface DeleteRequest extends ServiceRequest {
        EntityId?: any;
    }
    interface DeleteResponse extends ServiceResponse {
    }
    interface UndeleteRequest extends ServiceRequest {
        EntityId?: any;
    }
    interface UndeleteResponse extends ServiceResponse {
    }
    enum ColumnSelection {
        List = 0,
        KeyOnly = 1,
        Details = 2,
        None = 3,
        IdOnly = 4,
        Lookup = 5
    }
    enum RetrieveColumnSelection {
        details = 0,
        keyOnly = 1,
        list = 2,
        none = 3,
        idOnly = 4,
        lookup = 5
    }
    interface ListRequest extends ServiceRequest {
        Skip?: number;
        Take?: number;
        Sort?: string[];
        ContainsText?: string;
        ContainsField?: string;
        Criteria?: any[];
        EqualityFilter?: any;
        IncludeDeleted?: boolean;
        ExcludeTotalCount?: boolean;
        ColumnSelection?: ColumnSelection;
        IncludeColumns?: string[];
        ExcludeColumns?: string[];
        ExportColumns?: string[];
        DistinctFields?: string[];
    }
    interface ListResponse<TEntity> extends ServiceResponse {
        Entities?: TEntity[];
        Values?: any[];
        TotalCount?: number;
        Skip?: number;
        Take?: number;
    }
    interface RetrieveRequest extends ServiceRequest {
        EntityId?: any;
        ColumnSelection?: RetrieveColumnSelection;
        IncludeColumns?: string[];
        ExcludeColumns?: string[];
    }
    interface RetrieveResponse<TEntity> extends ServiceResponse {
        Entity?: TEntity;
    }
    interface RetrieveLocalizationRequest extends RetrieveRequest {
    }
    interface RetrieveLocalizationResponse<TEntity> extends ServiceResponse {
        Entities?: {
            [key: string]: TEntity;
        };
    }
    interface RequestErrorInfo {
        status?: number;
        statusText?: string;
        responseText?: string;
    }
    interface ServiceOptions<TResponse extends ServiceResponse> extends RequestInit {
        allowRedirect?: boolean;
        async?: boolean;
        blockUI?: boolean;
        headers?: Record<string, string>;
        request?: any;
        service?: string;
        url?: string;
        onCleanup?(): void;
        /** Should return true if the error is handled (e.g. notification shown). Otherwise the error may be shown twice. */
        onError?(response: TResponse, info?: RequestErrorInfo): void | boolean;
        onSuccess?(response: TResponse): void;
    }

    namespace ErrorHandling {
        /**
         * Shows a service error as an alert dialog. If the error
         * is null, has no message or code, it shows "??ERROR??".
         */
        function showServiceError(error: ServiceError, errorInfo?: RequestErrorInfo): void;
        /**
         * Runtime error handler that shows a runtime error as a notification
         * by default only in development mode (@see isDevelopmentMode)
         * This function is assigned as window.onerror handler in
         * ScriptInit.ts for Serenity applications so that developers
         * can notice an error without having to check the browser console.
         */
        function runtimeErrorHandler(message: string, filename?: string, lineno?: number, colno?: number, error?: Error): void;
        /**
         * Determines if the current environment is development mode.
         * The runtimeErrorHandler (window.onerror) shows error notifications only
         * when this function returns true. The default implementation considers
         * the environment as development mode if the host is localhost, 127.0.0.1, ::1,
         * or a domain name that ends with .local/.localhost.
         * @returns true if the current environment is development mode, false otherwise.
         */
        function isDevelopmentMode(): boolean;
        /**
         * Unhandled promise rejection error handler. It's purpose is to
         * ignore logging serviceCall / serviceFetch errors as they have built-in
         * error handling but browser logs it in the console, while Node crashes.
         * Include below code in script-init/errorhandling.ts to enable:
         * window.addEventListener("unhandledrejection", ErrorHandling.unhandledRejectionHandler);
         */
        function unhandledRejectionHandler(err: PromiseRejectionEvent): void;
    }

    /**
     * Represents a Fluent object, which is similar to jQuery but works for only one element.
     * It implements the `ArrayLike` interface and can have 0 (null) or 1 element.
     *
     * @typeparam TElement The type of the underlying HTML element.
     */
    interface Fluent<TElement extends HTMLElement = HTMLElement> extends ArrayLike<TElement> {
        /**
         * Adds one or more classes to the element. Any falsy value is ignored.
         *
         * @param value The class or classes to add. It can be a string, boolean, or an array of strings or booleans.
         * @returns The Fluent object itself.
         */
        addClass(value: string | boolean | (string | boolean)[]): this;
        /**
         * Appends content to the element.
         *
         * @param child The content to append. It can be a string, a Node object, or another Fluent object.
         * @returns The Fluent object itself.
         */
        append(child: string | Node | Fluent<any>): this;
        /**
         * Inserts content after the element.
         *
         * @param content The content to insert. It can be a string, a Node object, or another Fluent object.
         * @returns The Fluent object itself.
         */
        after(content: string | Node | Fluent<any>): this;
        /**
         * Appends the element to the specified parent element.
         *
         * @param parent The parent element to append to. It can be an Element object or another Fluent object.
         * @returns The Fluent object itself.
         */
        appendTo(parent: Element | Fluent<any>): this;
        /**
         * Gets the value of the specified attribute.
         *
         * @param name The name of the attribute.
         * @returns The value of the attribute.
         */
        attr(name: string): string;
        /**
         * Sets the value of the specified attribute.
         *
         * @param name The name of the attribute.
         * @param value The value of the attribute. If the value is falsy the attribute is removed.
         * @returns The Fluent object itself if a value is provided.
         */
        attr(name: string, value: string | number | boolean | null | undefined): this;
        /**
         * Inserts content before the element.
         *
         * @param content The content to insert. It can be a string, a Node object, or another Fluent object.
         * @returns The Fluent object itself.
         */
        before(content: string | Node | Fluent<any>): this;
        /**
         * Gets the children of the element as an array (not Fluent)
         *
         * @param selector Optional. A CSS selector to filter the children.
         * @returns An array of HTMLElement objects representing the children.
         */
        children(selector?: string): HTMLElement[];
        /**
         * Sets (overrides) the class attribute of the element. Any falsy value is ignored.
         *
         * @param value The class or classes to add. It can be a string, boolean, or an array of strings or booleans.
         * @returns The Fluent object itself.
         */
        class(value: string | boolean | (string | boolean)[]): this;
        /**
         * Triggers a click event on the element.
         *
         * @returns The Fluent object itself.
         */
        click(): this;
        /**
         * Adds a click event listener on the element.
         *
         * @param listener A callback function to execute when the click event is triggered.
         * @returns The Fluent object itself.
         */
        click(listener: (e: MouseEvent) => void): this;
        /**
         * Gets the closest ancestor of the element that matches the specified selector.
         *
         * @param selector A CSS selector to match against.
         * @returns A Fluent object representing the closest ancestor element.
         */
        closest(selector: string): Fluent<HTMLElement>;
        /**
         * Gets or sets the value of the specified data attribute.
         *
         * @param name The name of the data attribute.
         * @returns The value of the data attribute if no value is provided, or the Fluent object itself if a value is provided.
         */
        data(name: string): string;
        data(name: string, value: string): this;
        /**
         * Executes a callback function for the element in the Fluent object if it is not null.
         *
         * @param callback The callback function to execute for each element.
         * @returns The Fluent object itself.
         */
        each(callback: (el: TElement) => void): this;
        /**
         * Gets the underlying HTML element.
         *
         * @returns The underlying HTML element.
         */
        getNode(): TElement;
        /**
         * Removes all child nodes from the element. It also clears event handlers attached via Fluent, and disposes any attached widgets.
         *
         * @returns The Fluent object itself.
         */
        empty(): this;
        /**
         * Finds all elements that match the specified selector within the element.
         *
         * @typeparam TElement The type of the found elements.
         * @param selector A CSS selector to match against.
         * @returns An array of elements that match the selector.
         */
        findAll<TElement extends HTMLElement = HTMLElement>(selector: string): TElement[];
        /**
         * Finds each element that matches the specified selector within the element and executes a callback function for each found element as a Fluent object.
         *
         * @typeparam TElement The type of the found elements.
         * @param selector A CSS selector to match against.
         * @param callback The callback function to execute for each found element. It receives a Fluent object for each element.
         * @returns The Fluent object itself.
         */
        findEach<TElement extends HTMLElement = HTMLElement>(selector: string, callback: (el: Fluent<TElement>) => void): Fluent;
        /**
         * Finds the first element that matches the specified selector within the element.
         *
         * @typeparam TElement The type of the found element.
         * @param selector A CSS selector to match against.
         * @returns A Fluent object representing the first element that matches the selector.
         */
        findFirst<TElement extends HTMLElement = HTMLElement>(selector: string): Fluent<TElement>;
        /**
         * Sets focus on the element.
         *
         * @returns The Fluent object itself.
         */
        focus(): this;
        /**
         * Checks if the element has the specified class.
         *
         * @param klass The class to check for.
         * @returns `true` if the element has the class, `false` otherwise.
         */
        hasClass(klass: string): boolean;
        /**
         * Hides the element by setting its display property to "none".
         *
         * @returns The Fluent object itself.
         */
        hide(): this;
        /**
         * Gets the widget associated with the element.
         *
         * @typeparam TWidget The type of the widget.
         * @param type Optional. The constructor function of the widget.
         * @returns The widget associated with the element.
         */
        getWidget<TWidget>(type?: {
            new (...args: any[]): TWidget;
        }): TWidget;
        /**
         * Inserts the element after the specified reference element.
         *
         * @param referenceNode The reference element to insert after. It can be an HTMLElement object or another Fluent object.
         * @returns The Fluent object itself.
         */
        insertAfter(referenceNode: HTMLElement | Fluent<HTMLElement>): this;
        /**
         * Inserts the element before the specified reference element.
         *
         * @param referenceNode The reference element to insert before. It can be an HTMLElement object or another Fluent object.
         * @returns The Fluent object itself.
         */
        insertBefore(referenceNode: HTMLElement | Fluent<HTMLElement>): this;
        /**
         * Gets an iterator for the elements in the Fluent object.
         *
         * @returns An iterator for the elements in the Fluent object.
         */
        [Symbol.iterator]: TElement[];
        /**
         * Gets the element at the specified index.
         *
         * @param n The index of the element.
         * @returns The element at the specified index.
         */
        readonly [n: number]: TElement;
        /**
         * Gets the number of elements in the Fluent object. Can only be 1 or 0.
         */
        readonly length: number;
        /**
         * Removes an event listener from the element.
         *
         * @typeparam K The type of the event.
         * @param type The type of the event. It can include a ".namespace" similar to jQuery.
         * @param listener The event listener to remove.
         * @returns The Fluent object itself.
         */
        off<K extends keyof HTMLElementEventMap>(type: K, listener: (this: HTMLElement, ev: HTMLElementEventMap[K]) => any): this;
        off(type: string): this;
        off(type: string, listener: EventListener): this;
        off(type: string, selector: string, delegationHandler: Function): this;
        /**
         * Adds an event listener to the element. It is possible to use delegated events like jQuery.
         *
         * @typeparam K The type of the event.
         * @param type The type of the event. It can include a ".namespace" similar to jQuery.
         * @param listener The event listener to add.
         * @returns The Fluent object itself.
         */
        on<K extends keyof HTMLElementEventMap>(type: K, listener: (this: HTMLElement, ev: HTMLElementEventMap[K]) => any): this;
        on(type: string, listener: EventListener): this;
        on(type: string, selector: string, delegationHandler: Function): this;
        /**
         * Adds a one-time event listener to the element. It is possible to use delegated events like jQuery.
         *
         * @typeparam K The type of the event.
         * @param type The type of the event. It can include a ".namespace" similar to jQuery.
         * @param listener The event listener to add.
         * @returns The Fluent object itself.
         */
        one<K extends keyof HTMLElementEventMap>(type: K, listener: (this: HTMLElement, ev: HTMLElementEventMap[K]) => any): this;
        one(type: string, listener: EventListener): this;
        one(type: string, selector: string, delegationHandler: Function): this;
        /**
         * Checks if the element matches the specified selector.
         *
         * @param selector A CSS selector to match against.
         * @returns `true` if the element matches the selector, `false` otherwise.
         */
        matches(selector: string): boolean;
        /**
         * Gets the next sibling element that matches the specified selector, or the first sibling if no selector is provided..
         *
         * @param selector Optional. A CSS selector to filter the next sibling.
         * @returns A Fluent object representing the next sibling element.
         */
        nextSibling(selector?: string): Fluent<any>;
        /**
         * Gets the parent element of the element.
         *
         * @returns A Fluent object representing the parent element.
         */
        parent(): Fluent<HTMLElement>;
        /**
         * Prepends content to the element.
         *
         * @param child The content to prepend. It can be a string, a Node object, or another Fluent object.
         * @returns The Fluent object itself.
         */
        prepend(child: string | Node | Fluent<any>): this;
        /**
         * Prepends the element to the specified parent element.
         *
         * @param parent The parent element to prepend to. It can be an Element object or another Fluent object.
         * @returns The Fluent object itself.
         */
        prependTo(parent: Element | Fluent<any>): this;
        /**
         * Gets the previous sibling element that matches the specified selector, or the first sibling if no selector is provided.
         *
         * @param selector Optional. A CSS selector to filter the previous sibling.
         * @returns A Fluent object representing the previous sibling element.
         */
        prevSibling(selector?: string): Fluent<any>;
        /**
         * Removes the element from the DOM. It also removes event handlers and disposes widgets by calling "remove" event handlers.
         *
         * @returns The Fluent object itself.
         */
        remove(): this;
        /**
         * Removes the specified attribute from the element.
         *
         * @param name The name of the attribute to remove.
         * @returns The Fluent object itself.
         */
        removeAttr(name: string): this;
        /**
         * Removes one or more classes from the element. Any falsy value is ignored.
         *
         * @param value The class or classes to remove. It can be a string, boolean, or an array of strings or booleans.
         * @returns The Fluent object itself.
         */
        removeClass(value: string | boolean | (string | boolean)[]): this;
        /**
         * Shows the element by setting its display property to empty string.
         *
         * @returns The Fluent object itself.
         */
        show(): this;
        /**
         * Executes a callback function to modify the inline style of the element.
         *
         * @param callback The callback function to modify the inline style.
         * @returns The Fluent object itself.
         */
        style(callback: (css: CSSStyleDeclaration) => void): this;
        /**
         * Gets or sets the text content of the element.
         *
         * @returns The text content of the element if no value is provided, or the Fluent object itself if a value is provided.
         */
        text(): string;
        text(value: string): this;
        /**
         * Toggles the visibility of the element.
         *
         * @param flag Optional. A flag indicating whether to show or hide the element. If not provided, the visibility will be toggled.
         * @returns The Fluent object itself.
         */
        toggle(flag?: boolean): this;
        /**
         * Toggles one or more classes on the element. If the class exists, it is removed; otherwise, it is added.
         *
         * @param value The class or classes to toggle. It can be a string, boolean, or an array of strings or booleans.
         * @returns The Fluent object itself.
         */
        toggleClass(value: (string | boolean | (string | boolean)[]), add?: boolean): this;
        /**
         * Triggers a specified event on the element.
         *
         * @param type The type of the event to trigger.
         * @param args Optional. An object that specifies event-specific initialization properties.
         * @returns The Fluent object itself.
         */
        trigger(type: string, args?: any): this;
        /**
         * Tries to get the widget associated with the element.
         *
         * @typeparam TWidget The type of the widget.
         * @param type Optional. The constructor function of the widget.
         * @returns The widget associated with the element, or `null` if no widget is found.
         */
        tryGetWidget<TWidget>(type?: {
            new (...args: any[]): TWidget;
        }): TWidget;
        /**
         * Gets or sets the value of the element.
         *
         * @param value The value to set. If no value is provided, returns the current value of the element.
         * @returns The value of the element if no value is provided, or the Fluent object itself if a value is provided.
         */
        val(value: string): this;
        val(): string;
    }
    function Fluent<K extends keyof HTMLElementTagNameMap>(tag: K): Fluent<HTMLElementTagNameMap[K]>;
    function Fluent<TElement extends HTMLElement>(element: TElement): Fluent<TElement>;
    function Fluent(element: EventTarget): Fluent<HTMLElement>;
    namespace Fluent {
        var ready: (callback: () => void) => void;
        var byId: <TElement extends HTMLElement>(id: string) => Fluent<TElement>;
        var findAll: <TElement extends HTMLElement>(selector: string) => TElement[];
        var findEach: <TElement extends HTMLElement>(selector: string, callback: (el: Fluent<TElement>) => void) => void;
        var findFirst: <TElement extends HTMLElement>(selector: string) => Fluent<TElement>;
    }
    namespace Fluent {
        /**
         * Adds an event listener to the element. It is possible to use delegated events like jQuery.
         *
         * @typeparam K The type of the event.
         * @param element The target element
         * @param type The type of the event. It can include a ".namespace" similar to jQuery.
         * @param listener The event listener to add.
         */
        function on<K extends keyof HTMLElementEventMap>(element: EventTarget, type: K, listener: (this: HTMLElement, ev: HTMLElementEventMap[K]) => any): void;
        function on(element: EventTarget, type: string, listener: EventListener): void;
        function on(element: EventTarget, type: string, selector: string, delegationHandler: Function): void;
        /**
         * Adds a one-time event listener to the element. It is possible to use delegated events like jQuery.
         *
         * @typeparam K The type of the event.
         * @param element The target element
         * @param type The type of the event. It can include a ".namespace" similar to jQuery.
         * @param listener The event listener to add.
         */
        function one<K extends keyof HTMLElementEventMap>(element: EventTarget, type: K, listener: (this: HTMLElement, ev: HTMLElementEventMap[K]) => any): void;
        function one(element: EventTarget, type: string, listener: EventListener): void;
        function one(element: EventTarget, type: string, selector: string, delegationHandler: Function): void;
        /**
         * Removes an event listener from the element.
         *
         * @param element The target element
         * @typeparam K The type of the event.
         * @param type The type of the event. It can include a ".namespace" similar to jQuery.
         * @param listener The event listener to remove.
         */
        function off<K extends keyof HTMLElementEventMap>(element: EventTarget, type: K, listener?: (this: HTMLElement, ev: HTMLElementEventMap[K]) => any): void;
        function off(element: EventTarget, type: string, listener?: EventListener): void;
        function off(element: EventTarget, type: string, selector?: string, delegationHandler?: Function): void;
        /**
         * Triggers a specified event on the element.
         *
         * @param element The target element
         * @param type The type of the event to trigger.
         * @param args Optional. An object that specifies event-specific initialization properties.
         * @returns The event object. Use Fluent.isDefaultPrevented the check if preventDefault is called.
         */
        function trigger(element: EventTarget, type: string, args?: any): Event & {
            isDefaultPrevented?(): boolean;
        };
        /**
         * Adds one or more classes to the element. Any falsy value is ignored.
         *
         * @param element The target element
         * @param value The class or classes to add. It can be a string, boolean, or an array of strings or booleans.
         */
        function addClass(element: Element, value: string | boolean | (string | boolean)[]): void;
        function empty(element: Element): void;
        /**
         * Returns true if the element is visible like. This is for compatibility with jQuery's :visible selector.
         * @param element The target element
         * @returns true if the element has offsetWidth or offsetHeight or any getClientRects().length > 0
         */
        function isVisibleLike(element: Element): boolean;
        /**
         * Removes the element from the DOM. It also removes event handlers and disposes widgets by calling "remove" event handlers.
         *
         * @param element The element to remove
         */
        function remove(element: Element): void;
        /**
         * Removes one or more classes from the element. Any falsy value is ignored.
         *
         * @param element The target element
         * @param value The class or classes to remove. It can be a string, boolean, or an array of strings or booleans.
         */
        function removeClass(element: Element, value: string | boolean | (string | boolean)[]): void;
        /**
         * Toggles the visibility of the element.
         *
         * @param element The target element
         * @param flag Optional. A flag indicating whether to show or hide the element. If not provided, the visibility will be toggled.
         * @returns The Fluent object itself.
         */
        function toggle(element: Element, flag?: boolean): void;
        /**
         * Toggles one or more classes on the element. If the class exists, it is removed; otherwise, it is added. Falsy values are ignored.
         *
         * @param element The target element
         * @param value The class or classes to toggle. It can be a string, boolean, or an array of strings or booleans.
         */
        function toggleClass(element: Element, value: string | boolean | (string | boolean)[], add?: boolean): void;
        /**
         * Converts the given class value or an array of class values to a CSS class name. Any falsy value is ignored.
         * @param value The class or classes. It can be a string, boolean, or an array of strings or booleans.
         * @returns Class name string
         */
        function toClassName(value: string | boolean | (string | boolean)[]): string;
        /**
         * Returns true if the element is input like. E.g. one of input, textarea, select, button. This is for compatibility with jQuery's :input selector.
         * @param element The target element
         * @returns true if element is an input like node
         */
        function isInputLike(element: Element): element is (HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement | HTMLButtonElement);
        /** A CSS selector for input like tags */
        const inputLikeSelector = "input,select,textarea,button";
        /**
         * Returns true if the tag is one of input, textarea, select, button.
         * @param tag The tag
         * @returns true if the element has offsetWidth or offsetHeight or any getClientRects().length > 0
         */
        function isInputTag(tag: string): boolean;
        /**
         * Checks if the event's preventDefault method is called. This is for compatibility with jQuery which
         * has a non-standard isDefaultPrevented method.
         * @param event The event object
         * @returns True if preventDefault is called.
         */
        function isDefaultPrevented(event: {
            defaultPrevented?: boolean;
            isDefaultPrevented?: () => boolean;
        }): boolean;
        /**
         * Tries to read a property from the event, or event.originalEvent, or event.detail. It is designed
         * for compatibility with the way jQuery wraps original event under originalEvent property, that
         * causes custom properties to be not available in the event object.
         *
         * @param event The event object
         * @param prop The property name
         * @returns The property value
         */
        function eventProp(event: any, prop: string): any;
    }

    /**
     * Interface for number formatting, similar to .NET's NumberFormatInfo
     */
    interface NumberFormat {
        /** Decimal separator */
        decimalSeparator: string;
        /** Group separator */
        groupSeparator?: string;
        /** Number of digits after decimal separator */
        decimalDigits?: number;
        /** Positive sign */
        positiveSign?: string;
        /** Negative sign */
        negativeSign?: string;
        /** Zero symbol */
        nanSymbol?: string;
        /** Percentage symbol */
        percentSymbol?: string;
        /** Currency symbol */
        currencySymbol?: string;
    }
    /** Interface for date formatting, similar to .NET's DateFormatInfo */
    interface DateFormat {
        /** Date separator */
        dateSeparator?: string;
        /** Default date format string */
        dateFormat?: string;
        /** Date order, like dmy, or ymd */
        dateOrder?: string;
        /** Default date time format string */
        dateTimeFormat?: string;
        /** AM designator */
        amDesignator?: string;
        /** PM designator */
        pmDesignator?: string;
        /** Time separator */
        timeSeparator?: string;
        /** First day of week, 0 = Sunday, 1 = Monday */
        firstDayOfWeek?: number;
        /** Array of day names */
        dayNames?: string[];
        /** Array of short day names */
        shortDayNames?: string[];
        /** Array of two letter day names */
        minimizedDayNames?: string[];
        /** Array of month names */
        monthNames?: string[];
        /** Array of short month names */
        shortMonthNames?: string[];
    }
    /** Interface for a locale, similar to .NET's CultureInfo */
    interface Locale extends NumberFormat, DateFormat {
        /** Locale string comparison function, similar to .NET's StringComparer */
        stringCompare?: (a: string, b: string) => number;
        /** Locale string to upper case function */
        toUpper?: (a: string) => string;
    }
    /** Invariant locale (e.g. CultureInfo.InvariantCulture) */
    let Invariant: Locale;
    /**
     * Factory for a function that compares two strings, based on a character order
     * passed in the `order` argument.
     */
    function compareStringFactory(order: string): ((a: string, b: string) => number);
    /**
     * Current culture, e.g. CultureInfo.CurrentCulture. This is overridden by
     * settings passed from a `<script>` element in the page with id `ScriptCulture`
     * containing a JSON object if available. This element is generally created in
     * the _LayoutHead.cshtml file for Serenity applications, so that the culture
     * settings determined server, can be passed to the client.
     */
    let Culture: Locale;
    /**
     * Formats a string with parameters similar to .NET's String.Format function
     * using current `Culture` locale settings.
     */
    function stringFormat(format: string, ...prm: any[]): string;
    /**
     * Formats a string with parameters similar to .NET's String.Format function
     * using the locale passed as the first argument.
     */
    function stringFormatLocale(l: Locale, format: string, ...prm: any[]): string;
    /**
     * Rounds a number to specified digits or an integer number if digits are not specified.
     * @param n the number to round
     * @param d the number of digits to round to. default is zero.
     * @param rounding whether to use banker's rounding
     * @returns the rounded number
     */
    let round: (n: number, d?: number, rounding?: boolean) => number;
    /**
     * Truncates a number to an integer number.
     */
    let trunc: (n: number) => number;
    /**
     * Formats a number using the current `Culture` locale (or the passed locale) settings.
     * It supports format specifiers similar to .NET numeric formatting strings.
     * @param num the number to format
     * @param format the format specifier. default is 'g'.
     * See .NET numeric formatting strings documentation for more information.
     */
    function formatNumber(num: number, format?: string, decOrLoc?: string | NumberFormat, grp?: string): string;
    /**
     * Converts a string to an integer. The difference between parseInt and parseInteger
     * is that parseInteger will return null if the string is empty or null, whereas
     * parseInt will return NaN and parseInteger will use the current culture's group
     * and decimal separators.
     * @param s the string to parse
     */
    function parseInteger(s: string): number;
    /**
     * Converts a string to a decimal. The difference between parseFloat and parseDecimal
     * is that parseDecimal will return null if the string is empty or null, whereas
     * parseFloat will return NaN and parseDecimal will use the current culture's group
     * and decimal separators.
     * @param s the string to parse
     */
    function parseDecimal(s: string): number;
    /**
     * Converts a string to an ID. If the string is a number, it is returned as-is.
     * If the string is empty, null or whitespace, null is returned.
     * Otherwise, it is converted to a number if possible. If the string is not a
     * valid number or longer than 14 digits, the trimmed string is returned as-is.
     * @param id the string to convert to an ID
     */
    function toId(id: any): any;
    /**
     * Formats a date using the specified format string and optional culture.
     * Supports .NET style format strings including custom formats.
     * See .NET documentation for supported formats.
     * @param d the date to format. If null, it returns empty string.
     * @param format the format string to use. If null, it uses the current culture's default format.
     * 'G' uses the culture's datetime format.
     * 'g' uses the culture's datetime format with secs removed.
     * 'd' uses the culture's date format.
     * 't' uses the culture's time format.
     * 'u' uses the sortable ISO format with UTC time.
     * 'U' uses the culture's date format with UTC time.
     * @param locale the locale to use
     * @returns the formatted date
     * @example
     * // returns "2019-01-01"
     * formatDate(new Date(2019, 0, 1), "yyyy-MM-dd");
     * @example
     * // returns "2019-01-01 12:00:00"
     * formatDate(new Date(2019, 0, 1, 12), "yyyy-MM-dd HH:mm:ss");
     * @example
     * // returns "2019-01-01 12:00:00.000"
     * formatDate(new Date(2019, 0, 1, 12), "yyyy-MM-dd HH:mm:ss.fff");
     * @example
     * // returns "2019-01-01 12:00:00.000 AM"
     * formatDate(new Date(2019, 0, 1, 12), "yyyy-MM-dd HH:mm:ss.fff tt");
     */
    function formatDate(d: Date | string, format?: string, locale?: Locale): string;
    /**
     * Formats a date as the ISO 8601 UTC date/time format.
     * @param d The date.
     */
    function formatISODateTimeUTC(d: Date): string;
    /**
     * Parses a string in the ISO 8601 UTC date/time format.
     * @param s The string to parse.
     */
    function parseISODateTime(s: string): Date;
    /**
     * Parses a string to a date. If the string is empty or whitespace, returns null.
     * Returns a NaN Date if the string is not a valid date.
     * @param s The string to parse.
     * @param dateOrder The order of the date parts in the string. Defaults to culture's default date order.
      */
    function parseDate(s: string, dateOrder?: string): Date;
    /**
     * Splits a date string into an array of strings, each containing a single date part.
     * It can handle separators "/", ".", "-" and "\".
     * @param s The string to split.
     */
    function splitDateString(s: string): string[];

    /**
     * Html encodes a string (encodes single and double quotes, & (ampersand), > and < characters)
     * @param s String (or number etc.) to be HTML encoded
     */
    function htmlEncode(s: any): string;
    /**
     * Toggles the class on the element handling spaces like addClass does.
     * @param el the element
     * @param cls the class to toggle
     * @param add if true, the class will be added, if false the class will be removed, otherwise it will be toggled.
     */
    function toggleClass(el: Element, cls: string, add?: boolean): void;
    function addClass(el: Element, cls: string): void;
    function removeClass(el: Element, cls: string): void;

    function addLocalText(obj: string | Record<string, string | Record<string, any>> | string, pre?: string): void;
    function localText(key: string, defaultText?: string): string;
    function tryGetText(key: string): string;
    function proxyTexts(o: Record<string, any>, p: string, t: Record<string, any>): Object;

    interface LookupOptions<TItem> {
        idField?: string;
        parentIdField?: string;
        textField?: string;
    }
    interface Lookup<TItem> {
        items: TItem[];
        itemById: {
            [key: string]: TItem;
        };
        idField: string;
        parentIdField: string;
        textField: string;
    }
    class Lookup<TItem> {
        items: TItem[];
        itemById: {
            [key: string]: TItem;
        };
        idField: string;
        parentIdField: string;
        textField: string;
        constructor(options: LookupOptions<TItem>, items?: TItem[]);
        update?(value: TItem[]): void;
    }

    type ToastContainerOptions = {
        containerId?: string;
        positionClass?: string;
        target?: string;
    };
    type ToastrOptions = ToastContainerOptions & {
        tapToDismiss?: boolean;
        toastClass?: string;
        showDuration?: number;
        onShown?: () => void;
        hideDuration?: number;
        onHidden?: () => void;
        closeMethod?: boolean;
        closeDuration?: number | false;
        closeEasing?: boolean;
        closeOnHover?: boolean;
        extendedTimeOut?: number;
        iconClass?: string;
        positionClass?: string;
        timeOut?: number;
        titleClass?: string;
        messageClass?: string;
        escapeHtml?: boolean;
        target?: string;
        closeHtml?: string;
        closeClass?: string;
        newestOnTop?: boolean;
        preventDuplicates?: boolean;
        onclick?: (event: MouseEvent) => void;
        onCloseClick?: (event: Event) => void;
        closeButton?: boolean;
        rtl?: boolean;
    };
    type NotifyMap = {
        type: string;
        iconClass: string;
        title?: string;
        message?: string;
    };
    class Toastr {
        private listener;
        private toastId;
        private previousToast;
        options: ToastrOptions;
        constructor(options?: ToastrOptions);
        getContainer(options?: ToastContainerOptions, create?: boolean): HTMLElement;
        error(message?: string, title?: string, opt?: ToastrOptions): HTMLElement | null;
        warning(message?: string, title?: string, opt?: ToastrOptions): HTMLElement | null;
        success(message?: string, title?: string, opt?: ToastrOptions): HTMLElement | null;
        info(message?: string, title?: string, opt?: ToastrOptions): HTMLElement | null;
        subscribe(callback: (response: Toastr) => void): void;
        publish(args: Toastr): void;
        private removeContainerIfEmpty;
        removeToast(toastElement: HTMLElement, options?: ToastContainerOptions): void;
        clear(options?: ToastContainerOptions): void;
        private notify;
    }

    let defaultNotifyOptions: ToastrOptions;
    function positionToastContainer(options?: ToastrOptions, create?: boolean): void;
    function notifyError(message: string, title?: string, options?: ToastrOptions): void;
    function notifyInfo(message: string, title?: string, options?: ToastrOptions): void;
    function notifySuccess(message: string, title?: string, options?: ToastrOptions): void;
    function notifyWarning(message: string, title?: string, options?: ToastrOptions): void;

    enum SummaryType {
        Disabled = -1,
        None = 0,
        Sum = 1,
        Avg = 2,
        Min = 3,
        Max = 4
    }
    interface PropertyItem {
        name?: string;
        title?: string;
        hint?: string;
        placeholder?: string;
        editorType?: string;
        editorParams?: any;
        category?: string;
        collapsible?: boolean;
        collapsed?: boolean;
        tab?: string;
        cssClass?: string;
        headerCssClass?: string;
        formCssClass?: string;
        maxLength?: number;
        required?: boolean;
        insertable?: boolean;
        insertPermission?: string;
        hideOnInsert?: boolean;
        updatable?: boolean;
        updatePermission?: string;
        hideOnUpdate?: boolean;
        readOnly?: boolean;
        readPermission?: string;
        oneWay?: boolean;
        defaultValue?: any;
        localizable?: boolean;
        visible?: boolean;
        allowHide?: boolean;
        formatterType?: string;
        formatterParams?: any;
        displayFormat?: string;
        alignment?: string;
        width?: number;
        widthSet?: boolean;
        minWidth?: number;
        maxWidth?: number;
        labelWidth?: string;
        resizable?: boolean;
        sortable?: boolean;
        sortOrder?: number;
        groupOrder?: number;
        summaryType?: SummaryType;
        editLink?: boolean;
        editLinkItemType?: string;
        editLinkIdField?: string;
        editLinkCssClass?: string;
        filteringType?: string;
        filteringParams?: any;
        filteringIdField?: string;
        notFilterable?: boolean;
        filterOnly?: boolean;
        quickFilter?: boolean;
        quickFilterParams?: any;
        quickFilterSeparator?: boolean;
        quickFilterCssClass?: string;
    }
    interface PropertyItemsData {
        items: PropertyItem[];
        additionalItems: PropertyItem[];
    }

    /**
     * Gets the known hash value for a given dynamic script name. They are usually
     * registered server-side via dynamic script manager and their latest known
     * hashes are passed to the client-side via a script element named RegisteredScripts.
     * @param name The dynamic script name
     * @param reload True to force resetting the script hash client side, e.g. for loading
     * lookups etc.
     * @returns The hash or null if no such known registration
     */
    function getScriptDataHash(name: string, reload?: boolean): string;
    /**
     * Fetches a script data with given name via ~/DynamicData endpoint
     * @param name Dynamic script name
     * @returns A promise that will return data if successfull
     */
    function fetchScriptData<TData>(name: string): Promise<TData>;
    /**
     * Returns the script data from cache if available, or via a fetch
     * request to ~/DynamicData endpoint
     * @param name
     * @param reload Clear cache and force reload
     * @returns
     */
    function getScriptData<TData = any>(name: string, reload?: boolean): Promise<TData>;
    /**
     * Gets or loads a [ColumnsScript] data
     * @param key Form key
     * @returns A property items data object containing items and additionalItems properties
     */
    function getColumnsScript(key: string): Promise<PropertyItemsData>;
    /**
     * Gets or loads a [FormScript] data
     * @param key Form key
     * @returns A property items data object containing items and additionalItems properties
     */
    function getFormScript(key: string): Promise<PropertyItemsData>;
    /**
     * Gets or loads a Lookup
     * @param key Lookup key
     */
    function getLookupAsync<TItem>(key: string): Promise<Lookup<TItem>>;
    /**
     * Gets or loads a [RemoteData]
     * @param key Remote data key
     */
    function getRemoteDataAsync<TData = any>(key: string): Promise<TData>;
    /**
     * Shows a suitable error message for errors occured during loading of
     * a dynamic script data.
     * @param name Name of the dynamic script
     * @param status HTTP status returned if available
     * @param statusText HTTP status text returned if available
     */
    function handleScriptDataError(name: string, status?: number, statusText?: string, shouldThrow?: boolean): string;
    function peekScriptData(name: string): any;
    /**
     * Forces reload of a lookup from the server. Note that only the
     * client side cache is cleared. This does not force reloading in the server-side.
     * @param key Lookup key
     * @returns Lookup
     */
    function reloadLookupAsync<TItem = any>(key: string): Promise<Lookup<TItem>>;
    function setRegisteredScripts(scripts: any[]): void;
    function setScriptData(name: string, value: any): void;

    function resolveUrl(url: string): string;
    function resolveServiceUrl(url: string): string;
    function getCookie(name: string): any;
    function isSameOrigin(url: string): boolean;
    function requestStarting(): void;
    function requestFinished(): void;
    function getActiveRequests(): number;
    function serviceCall<TResponse extends ServiceResponse>(options: ServiceOptions<TResponse>): PromiseLike<TResponse>;
    function serviceRequest<TResponse extends ServiceResponse>(service: string, request?: any, onSuccess?: (response: TResponse) => void, options?: ServiceOptions<TResponse>): PromiseLike<TResponse>;

    const typeInfoProperty = "typeInfo";
    type StringLiteral<T> = T extends string ? string extends T ? never : T : never;
    type TypeInfo<T> = {
        typeKind: "class" | "enum" | "interface" | "editor" | "formatter";
        typeName: StringLiteral<T> | (string & {});
        interfaces?: any[];
        customAttributes?: any[];
        enumFlags?: boolean;
        registered?: boolean;
    };
    function getTypeRegistry(): any;
    function getTypeNameProp(type: any): string;
    function setTypeNameProp(type: any, value: string): void;

    function getGlobalObject(): any;
    function omitUndefined(x: {
        [key: string]: any;
    }): any;
    type Type = Function | Object;
    function getNested(from: any, name: string): any;
    function getType(name: string, target?: any): Type;
    function getTypeFullName(type: Type): string;
    function getTypeShortName(type: Type): string;
    function getInstanceType(instance: any): any;
    function isAssignableFrom(target: any, type: Type): any;
    function isInstanceOfType(instance: any, type: Type): any;
    function getBaseType(type: any): any;
    function registerClass(type: any, name: string, intf?: any[]): void;
    function registerEnum(type: any, name: string, enumKey?: string): void;
    function registerInterface(type: any, name: string, intf?: any[]): void;
    namespace Enum {
        let toString: (enumType: any, value: number) => string;
        let getValues: (enumType: any) => any[];
    }
    const isEnum: (type: any) => boolean;
    function initFormType(typ: Function, nameWidgetPairs: any[]): void;
    function fieldsProxy<TRow>(): Readonly<Record<keyof TRow, string>>;
    function isArrayLike(obj: any): obj is ArrayLike<any>;
    function isPromiseLike(obj: any): obj is PromiseLike<any>;
    type NoInfer<T> = [T][T extends any ? 0 : never];
    class EditorAttribute {
    }
    class ISlickFormatter {
    }
    function registerFormatter(type: any, name: string, intf?: any[]): void;
    function registerEditor(type: any, name: string, intf?: any[]): void;
    function addCustomAttribute(type: any, attr: any): void;
    function getCustomAttribute<TAttr>(type: any, attrType: {
        new (...args: any[]): TAttr;
    }, inherit?: boolean): TAttr;
    function hasCustomAttribute<TAttr>(type: any, attrType: {
        new (...args: any[]): TAttr;
    }, inherit?: boolean): boolean;
    function getCustomAttributes<TAttr>(type: any, attrType: {
        new (...args: any[]): TAttr;
    }, inherit?: boolean): TAttr[];

    type ClassTypeInfo<T> = TypeInfo<T>;
    type EditorTypeInfo<T> = TypeInfo<T>;
    type FormatterTypeInfo<T> = TypeInfo<T>;
    type InterfaceTypeInfo<T> = TypeInfo<T>;
    function classTypeInfo<T>(typeName: StringLiteral<T>, interfaces?: any[]): ClassTypeInfo<T>;
    function editorTypeInfo<T>(typeName: StringLiteral<T>, interfaces?: any[]): EditorTypeInfo<T>;
    function formatterTypeInfo<T>(typeName: StringLiteral<T>, interfaces?: any[]): FormatterTypeInfo<T>;
    function interfaceTypeInfo<T>(typeName: StringLiteral<T>, interfaces?: any[]): InterfaceTypeInfo<T>;
    function registerType(type: {
        [typeInfoProperty]: TypeInfo<any>;
        name: string;
    }): void;

    interface TooltipOptions {
        title?: string;
        trigger?: string;
    }
    class Tooltip {
        private el;
        constructor(el: ArrayLike<HTMLElement> | HTMLElement, opt?: TooltipOptions);
        static defaults: TooltipOptions;
        dispose(): void;
        delayedDispose(delay?: number): void;
        delayedHide(delay?: number): void;
        private static existingInstance;
        static getInstance(el: ArrayLike<HTMLElement> | HTMLElement): Tooltip;
        static get isAvailable(): boolean;
        setTitle(value: string): Tooltip;
        toggle(show: boolean): Tooltip;
        hide(): Tooltip;
        show(): Tooltip;
    }

    /** Inspired from https://github.com/silverwind/uppie and https://github.com/GoogleChromeLabs/file-drop/blob/master/lib/filedrop.ts */
    interface UploaderOptions {
        /** Accept. If not specified, read from the passed input  */
        accept?: string;
        /** Auto clear input value after selection, so when same file selected it works. Default is true */
        autoClear?: boolean;
        /** Only used for multiple, default is 1 to upload multiple files in batches of size 1 */
        batchSize?: number;
        /** An optional list of dropzones. */
        dropZone?: HTMLElement | ArrayLike<HTMLElement>;
        /** Progress event that is called when a batch is about to be uploaded */
        batchStart?: (data: {
            batch: UploaderBatch;
        }) => void;
        /** Progress event that is called when a batch is ended uploading or failed */
        batchStop?: (data: {
            batch: UploaderBatch;
        }) => void;
        /** Called after batch is uploaded successfully */
        batchSuccess?: (data: UploaderSuccessData) => void;
        /** Progress event that is called during upload */
        batchProgress?: (data: {
            batch: UploaderBatch;
            loaded: number;
            total: number;
        }) => void;
        /** Callback to handle a batch. If not specified, a default handler is used. */
        batchHandler?: (batch: UploaderBatch, uploader: Uploader) => void | Promise<void>;
        /** Only called when a change/drop event occurs, but files can't be determined */
        changeCallback?: (e: Event) => void;
        /** Error handler, if not specified Uploader.errorHandler is used */
        errorHandler?: (data: UploaderErrorData) => void;
        /** Ignore file types, e.g. don't check accept property of input or this options */
        ignoreType?: boolean;
        /** Target input. If null, dropZone should be specified. */
        input?: HTMLInputElement;
        /** Allow multiple files. If not specified is read from the input */
        multiple?: boolean;
        /** The field name to use in FormData object. Default is files[] */
        name?: string;
    }
    interface UploaderRequest {
        /** A function that will return headers to be sent with request, or static set of headers */
        headers?: Record<string, string>;
        /** Response type expected from the server. Default is json */
        responseType?: "json" | "text";
        /** URL to send the request to. Default is ~/File/TemporaryUpload */
        url?: string;
    }
    interface UploaderBatch {
        event?: Event;
        filePaths?: string[];
        formData: FormData;
    }
    interface UploaderSuccessData {
        batch: UploaderBatch;
        request: UploaderRequest;
        event: ProgressEvent;
        xhr: XMLHttpRequest;
        response: any;
    }
    interface UploaderErrorData {
        batch?: UploaderBatch;
        event?: ProgressEvent;
        exception?: any;
        request?: UploaderRequest;
        response?: any;
        xhr?: XMLHttpRequest;
    }
    class Uploader {
        private opt;
        private batch;
        constructor(opt: UploaderOptions);
        private newBatch;
        private addToBatch;
        private endBatch;
        static defaults: Partial<UploaderOptions>;
        static requestDefaults: Partial<UploaderRequest>;
        private isMultiple;
        private getTypePredicate;
        private getMatchingItems;
        private watchInput;
        private watchDropZone;
        private arrayApi;
        private entriesApi;
        uploadBatch(batch: UploaderBatch, request?: UploaderRequest): Promise<void>;
        static errorHandler: (data: UploaderErrorData) => void;
    }

    /**
     * An `HTMLElement` that can be validated (`input`, `select`, `textarea`, or [contenteditable).
     */
    interface ValidatableElement extends HTMLElement {
        form?: HTMLFormElement;
        name?: string;
        type?: string;
        value?: string;
    }
    type ValidationValue = string | string[] | number | boolean;
    /**
     * Validation plugin signature with multitype return.
     * Boolean return signifies the validation result, which uses the default validation error message read from the element attribute.
     * String return signifies failed validation, which then will be used as the validation error message.
     * Promise return signifies asynchronous plugin behavior, with same behavior as Boolean or String.
     */
    type ValidationProvider = (value: ValidationValue, element: ValidatableElement, params?: any) => boolean | string | Promise<boolean | string>;
    interface ValidationErrorMap {
        [name: string]: (string | boolean);
    }
    interface ValidationErrorItem {
        message: string;
        element: ValidatableElement;
        method?: string;
    }
    type ValidationErrorList = ValidationErrorItem[];
    type ValidationRules = Record<string, any>;
    interface ValidationRulesMap {
        [name: string]: ValidationRules;
    }
    type ValidateEventDelegate = (element: ValidatableElement, event: Event, validator: Validator) => void;
    interface ValidatorOptions {
        /** True for logging debug info */
        debug?: boolean;
        /**
         * Use this class to create error labels, to look for existing error labels and to add it to invalid elements.
         *
         * default: "error"
         */
        errorClass?: string | undefined;
        /**
         * Use this element type to create error messages and to look for existing error messages. The default, "label",
         * has the advantage of creating a meaningful link between error message and invalid field using the for attribute (which is always used, regardless of element type).
         *
         * default: "label"
         */
        errorElement?: string | undefined;
        /**
         * Customize placement of created error labels. First argument: The created error label. Second argument: The invalid element.
         *
         * default: Places the error label after the invalid element
         */
        errorPlacement?(error: HTMLElement, element: ValidatableElement, validator: Validator): void;
        /**
         * Focus the last active or first invalid element on submit via validator.focusInvalid(). The last active element is the one
         * that had focus when the form was submitted, avoiding stealing its focus. If there was no element focused, the first one
         * in the form gets it, unless this option is turned off.
         *
         * default: true
         */
        focusInvalid?: boolean | undefined;
        /**
         * How to highlight invalid fields. Override to decide which fields and how to highlight.
         *
         * default: Adds errorClass (see the option) to the element
         */
        highlight?(element: ValidatableElement, errorClass: string, validClass: string): void;
        /**
         * Elements to ignore when validating, simply filtering them out. CSS not-method is used, therefore everything that is
         * accepted by not() can be passed as this option. Inputs of type submit and reset are always ignored, so are disabled elements.
         */
        ignore?: string | undefined;
        /**
         * Callback for custom code when an invalid form is submitted. Called with an event object as the first argument, and the validator
         * as in the second.
         */
        invalidHandler?(event: Event, validator: Validator): void;
        /**
         * Key/value pairs defining custom messages. Key is the name of an element, value the message to display for that element. Instead
         * of a plain message, another map with specific messages for each rule can be used. Overrides the title attribute of an element or
         * the default message for the method (in that order). Each message can be a String or a Callback. The callback is called in the scope
         * of the validator, with the rule's parameters as the first argument and the element as the second, and must return a String to display
         * as the message.
         *
         * default: the default message for the method used
         */
        messages?: Record<string, string> | undefined;
        normalizer?: (val: ValidationValue, element: ValidatableElement) => string;
        /**
         * Boolean or Function. Validate checkboxes and radio buttons on click. Set to false to disable.
         *
         * Set to a Function to decide for yourself when to run validation.
         * A boolean true is not a valid value.
         */
        onclick?: ValidateEventDelegate | boolean | undefined;
        /**
         * Function. Validate elements when user focuses in. If omitted hides all other fields marked as invalid.
         *
         * Set to a custom Function to decide for yourself when to run validation.
         */
        onfocusin?: ValidateEventDelegate | undefined;
        /**
         * Boolean or Function. Validate elements (except checkboxes/radio buttons) on blur. If nothing is entered, all rules are skipped, except when the field was already marked as invalid.
         *
         * Set to a Function to decide for yourself when to run validation.
         * A boolean true is not a valid value.
         */
        onfocusout?: ValidateEventDelegate | undefined;
        /**
         * Boolean or Function. Validate elements on keyup. As long as the field is not marked as invalid, nothing happens.
         * Otherwise, all rules are checked on each key up event. Set to false to disable.
         *
         * Set to a Function to decide for yourself when to run validation.
         * A boolean true is not a valid value.
         */
        onkeyup?: ValidateEventDelegate | undefined;
        /**
         * Validate the form on submit. Set to false to use only other events for validation.
         * Set to a Function to decide for yourself when to run validation.
         * A boolean true is not a valid value.
         *
         * default: true
         */
        onsubmit?: boolean | undefined;
        /**
         * Pending class
         * default: "pending"
         */
        pendingClass?: string | undefined;
        /**
         * A custom message display handler. Gets the map of errors as the first argument and an array of errors as the second,
         * called in the context of the validator object. The arguments contain only those elements currently validated,
         * which can be a single element when doing validation onblur/keyup. You can trigger (in addition to your own messages)
         * the default behaviour by calling this.defaultShowErrors().
         */
        rules?: ValidationRulesMap | undefined;
        /**
         * A custom message display handler. Gets the map of errors as the first argument and an array of errors as the second,
         * called in the context of the validator object. The arguments contain only those elements currently validated, which can
         * be a single element when doing validation onblur/keyup. You can trigger (in addition to your own messages) the default
         * behaviour by calling this.defaultShowErrors().
         */
        showErrors?(errorMap: ValidationErrorMap, errorList: ValidationErrorList, validator: Validator): void;
        abortHandler?(validator: Validator): void;
        /**
         * Callback for handling the actual submit when the form is valid. Gets the form and the event object. Replaces the default submit.
         * The right place to submit a form via Ajax after it is validated.
         */
        submitHandler?(form: HTMLFormElement, event: Event, validator: Validator): void | boolean;
        /**
         * String or Function. If specified, the error label is displayed to show a valid element. If a String is given, it is added as
         * a class to the label. If a Function is given, it is called with the label and the validated input (as a DOM element).
         * The label can be used to add a text like "ok!".
         */
        success?: string | ((label: HTMLElement, validatedInput: ValidatableElement) => void) | undefined;
        /**
         * Called to revert changes made by option highlight, same arguments as highlight.
         *
         * default: Removes the errorClass
         */
        unhighlight?(element: ValidatableElement, errorClass: string, validClass: string, validator: Validator): void;
        /**
         * This class is added to an element after it was validated and considered valid.
         *
         * default: "valid"
         */
        validClass?: string | undefined;
    }
    class Validator {
        static optional(element: ValidatableElement): "" | "dependency-mismatch";
        static autoCreateRanges: boolean;
        static defaults: ValidatorOptions;
        static readonly messages: Record<string, string | Function>;
        static readonly methods: Record<string, ValidationProvider>;
        readonly settings: ValidatorOptions;
        lastActive: ValidatableElement;
        private cancelSubmit;
        private currentElements;
        private currentForm;
        private errorMap;
        private errorList;
        private formSubmitted;
        private submitted;
        private submitButton;
        private pendingRequest;
        private invalid;
        private pending;
        private successList;
        private toHide;
        private toShow;
        constructor(form: HTMLFormElement, options: ValidatorOptions);
        static getInstance(element: HTMLFormElement | Node | ArrayLike<HTMLElement>): Validator;
        private init;
        /**
         * Checks if `element` is validatable (`input`, `select`, `textarea`).
         * @param element The element to check.
         * @returns `true` if validatable, otherwise `false`.
         */
        static isValidatableElement(element: EventTarget): element is ValidatableElement;
        static isCheckOrRadio(element: Node): element is HTMLInputElement;
        static getLength(value: ValidationValue, element: HTMLElement): number;
        static isContentEditable(element: HTMLElement): boolean;
        static elementValue(element: HTMLElement): string | number | string[];
        static valid(element: HTMLFormElement | ValidatableElement | ArrayLike<ValidatableElement>): boolean;
        static rules(element: ValidatableElement, command?: "add" | "remove", argument?: any): Record<string, any>;
        form(): boolean;
        checkForm(): boolean;
        element(element: ValidatableElement): boolean;
        showErrors(errors?: ValidationErrorMap): void;
        resetForm(): void;
        resetElements(elements: ValidatableElement[]): void;
        numberOfInvalids(): number;
        private static objectLength;
        hideErrors(): void;
        hideThese(errors: HTMLElement[]): void;
        valid(): boolean;
        size(): number;
        focusInvalid(): void;
        findLastActive(): ValidatableElement;
        elements(): ValidatableElement[];
        errors(): HTMLElement[];
        resetInternals(): void;
        reset(): void;
        resetAll(): void;
        prepareForm(): void;
        prepareElement(element: ValidatableElement): void;
        check(element: ValidatableElement): boolean;
        customDataMessage(element: ValidatableElement, method: string): string;
        customMessage(name: string, method: string): any;
        findDefined(...args: any[]): any;
        defaultMessage(element: ValidatableElement, rule: {
            method: string;
            parameters?: any;
        }): any;
        formatAndAdd(element: ValidatableElement, rule: {
            method: string;
            parameters: any;
        }): void;
        defaultShowErrors(): void;
        validElements(): ValidatableElement[];
        invalidElements(): ValidatableElement[];
        showLabel(element: ValidatableElement, message?: string): void;
        errorsFor(element: ValidatableElement): HTMLElement[];
        idOrName(element: ValidatableElement): string;
        validationTargetFor(element: ValidatableElement): ValidatableElement;
        findByName(name: string): ValidatableElement[];
        dependTypes: {
            boolean: (param: any) => any;
            string: (param: any, element: ValidatableElement) => boolean;
            function: (param: any, element: ValidatableElement) => any;
        };
        depend(param: any, element: ValidatableElement): any;
        startRequest(element: ValidatableElement): void;
        stopRequest(element: ValidatableElement, valid: boolean): void;
        abortRequest(element: ValidatableElement): void;
        previousValue(element: ValidatableElement, method: string): any;
        destroy(): void;
        static classRuleSettings: Record<string, ValidationRules>;
        static addClassRules(className: (string | any), rules: ValidationRules): void;
        static classRules(element: ValidatableElement): ValidationRules;
        static normalizeAttributeRule(rules: ValidationRules, type: string, method: string, value: ValidationValue): void;
        static attributeRules(element: ValidatableElement): ValidationRules;
        static dataRules(element: ValidatableElement): {};
        static staticRules(element: ValidatableElement): ValidationRules;
        static normalizeRules(rules: ValidationRules, element: ValidatableElement): ValidationRules;
        static addMethod(name: string, method: ValidationProvider, message?: string): void;
        static getHighlightTarget(el: HTMLElement): HTMLElement;
    }
    function addValidationRule(element: HTMLElement | ArrayLike<HTMLElement>, rule: (input: ValidatableElement) => string, uniqueName?: string): void;
    function removeValidationRule(element: HTMLElement | ArrayLike<HTMLElement>, uniqueName: string): void;

    /**
     * Tests if any of array elements matches given predicate. Prefer Array.some() over this function (e.g. `[1, 2, 3].some(predicate)`).
     * @param array Array to test.
     * @param predicate Predicate to test elements.
     * @returns True if any element matches.
     */
    function any<TItem>(array: TItem[], predicate: (x: TItem) => boolean): boolean;
    /**
     * Counts number of array elements that matches a given predicate.
     * @param array Array to test.
     * @param predicate Predicate to test elements.
     */
    function count<TItem>(array: TItem[], predicate: (x: TItem) => boolean): number;
    /**
     * Gets first element in an array that matches given predicate similar to LINQ's First.
     * Throws an error if no match is found.
     * @param array Array to test.
     * @param predicate Predicate to test elements.
     * @returns First element that matches.
     */
    function first<TItem>(array: TItem[], predicate: (x: TItem) => boolean): TItem;
    /**
     * A group item returned by `groupBy()`.
     */
    type GroupByElement<TItem> = {
        /** index of the item in `inOrder` array */
        order: number;
        /** key of the group */
        key: string;
        /** the items in the group */
        items: TItem[];
        /** index of the first item of this group in the original array */
        start: number;
    };
    /**
     * Return type of the `groupBy` function.
     */
    type GroupByResult<TItem> = {
        byKey: {
            [key: string]: GroupByElement<TItem>;
        };
        inOrder: GroupByElement<TItem>[];
    };
    /**
     * Groups an array with keys determined by specified getKey() callback.
     * Resulting object contains group objects in order and a dictionary to access by key.
     * This is similar to LINQ's ToLookup function with some additional details like start index.
     * @param items Array to group.
     * @param getKey Function that returns key for each item.
     * @returns GroupByResult object.
     */
    function groupBy<TItem>(items: TItem[], getKey: (x: TItem) => any): GroupByResult<TItem>;
    /**
     * Gets index of first element in an array that matches given predicate.
     * @param array Array to test.
     * @param predicate Predicate to test elements.
     */
    function indexOf<TItem>(array: TItem[], predicate: (x: TItem) => boolean): number;
    /**
     * Inserts an item to the array at specified index. Prefer Array.splice unless
     * you need to support IE.
     * @param obj Array or array like object to insert to.
     * @param index Index to insert at.
     * @param item Item to insert.
     * @throws Error if object does not support insert.
     * @example
     * insert([1, 2, 3], 1, 4); // [1, 4, 2, 3]
     * insert({ insert: (index, item) => { this.splice(index, 0, item); } }
     */
    function insert(obj: any, index: number, item: any): void;
    /**
     * Determines if the object is an array. Prefer Array.isArray over this function (e.g. `Array.isArray(obj)`).
     * @param obj Object to test.
     * @returns True if the object is an array.
     * @example
     * isArray([1, 2, 3]); // true
     * isArray({}); // false
     */
    const isArray: (arg: any) => arg is any[];
    /**
    * Gets first element in an array that matches given predicate.
    * Throws an error if no matches is found, or there are multiple matches.
    * @param array Array to test.
    * @param predicate Predicate to test elements.
    * @returns First element that matches.
    * @example
    * first([1, 2, 3], x => x == 2); // 2
    * first([1, 2, 3], x => x == 4); // throws error.
    */
    function single<TItem>(array: TItem[], predicate: (x: TItem) => boolean): TItem;
    type Grouping<TItem> = {
        [key: string]: TItem[];
    };
    /**
     * Maps an array into a dictionary with keys determined by specified getKey() callback,
     * and values that are arrays containing elements for a particular key.
     * @param items Array to map.
     * @param getKey Function that returns key for each item.
     * @returns Grouping object.
     * @example
     * toGrouping([1, 2, 3], x => x % 2 == 0 ? "even" : "odd"); // { odd: [1, 3], even: [2] }
     */
    function toGrouping<TItem>(items: TItem[], getKey: (x: TItem) => any): Grouping<TItem>;
    /**
     * Gets first element in an array that matches given predicate (similar to LINQ's FirstOrDefault).
     * Returns null if no match is found.
     * @param array Array to test.
     * @param predicate Predicate to test elements.
     * @returns First element that matches.
     * @example
     * tryFirst([1, 2, 3], x => x == 2); // 2
     * tryFirst([1, 2, 3], x => x == 4); // null
     */
    function tryFirst<TItem>(array: TItem[], predicate: (x: TItem) => boolean): TItem;

    interface UserDefinition {
        /**
         * Username of the logged user
         */
        Username?: string;
        /**
         * Display name of the logged user
         */
        DisplayName?: string;
        /**
         * This indicates that the user is a super "admin", e.g. assumed to have all the permissions available.
         * It does not mean a member of Administrators, who might not have some of the permissions */
        IsAdmin?: boolean;
        /**
         * A hashset of permission keys that the current user have, explicitly assigned or via its
         * roles. Note that client side permission checks should only be used for UI enable/disable etc.
         * You should not rely on client side permission checks and always re-check permissions server side.
         */
        Permissions?: {
            [key: string]: boolean;
        };
    }

    /**
     * Contains permission related functions.
     *
     * ## Note
     * We use a namespace here both for compatibility and for allowing users to override
     * these functions easily in ES modules environment, which is normally hard to do.
     */
    namespace Authorization {
        /**
         * Checks if the current user has the permission specified.
         * This should only be used for UI purposes and it is strongly recommended to check permissions server side.
         *
         * > Please prefer the `hasPermissionAsync` variant as this may block the UI thread if the `UserData` script is not already loaded.
         * @param permission Permission key. It may contain logical operators like A&B|C.
         * @returns `false` for "null or undefined", true for "*", `IsLoggedIn` for "?". For other permissions,
         * if the user has the permission or if the user has the `IsAdmin` flag (super admin) `true`, otherwise `false`.
         */
        function hasPermission(permission: string): boolean;
        /**
         * Checks if the current user has the permission specified.
         * This should only be used for UI purposes and it is strongly recommended to check permissions server side.
         *
         * @param permission Permission key. It may contain logical operators like A&B|C.
         * @returns `false` for "null or undefined", true for "*", `IsLoggedIn` for "?". For other permissions,
         * if the user has the permission or if the user has the `IsAdmin` flag (super admin) `true`, otherwise `false`.
         */
        function hasPermissionAsync(permission: string): Promise<boolean>;
        /**
         * Checks if the hashset contains the specified permission, also handling logical "|" and "&" operators
         * @param permissionSet Set of permissions
         * @param permission Permission key or a permission expression containing & | operators
         * @returns true if set contains permission
         */
        function isPermissionInSet(permissionSet: {
            [key: string]: boolean;
        }, permission: string): boolean;
        /**
         * Throws an error if the current user does not have the specified permission.
         * Prefer `await validatePermissionAsync()` as this one might block the UI if the `UserData`
         * is not already loaded.
         * @param permission Permission key. It may contain logical operators like A&B|C.
         */
        function validatePermission(permission: string): void;
        /**
        * Throws an error if the current user does not have the specified permission.
        * @param permission Permission key. It may contain logical operators like A&B|C.
        * @example
        * await Authorization.validatePermissionAsync("A&B|C");
        */
        function validatePermissionAsync(permission: string): Promise<void>;
    }
    namespace Authorization {
        /**
         * Checks if the current user is logged in. Prefer `isLoggedInAsync` as this one might block the UI if the `UserData`
         * is not already loaded.
         * @returns `true` if the user is logged in, `false` otherwise.
         * @example
         * if (Authorization.isLoggedIn) {
         *     // do something
         * }
         */
        let isLoggedIn: boolean;
        /**
         * Checks if the current user is logged in.
         * @returns `true` if the user is logged in, `false` otherwise.
         * @example
         * if (await Authorization.isLoggedInAsync) {
         *     // do something
         * }
         */
        let isLoggedInAsync: Promise<boolean>;
        /** Returns the username for currently logged user. Prefer `usernameAsync` as this one might block the UI if the `UserData`
         * is not already loaded.
         * @returns Username for currently logged user.
         * @example
         * if (Authorization.username) {
         *     // do something
         * }
         */
        let username: string;
        /** Returns the username for currently logged user.
         * @returns Username for currently logged user.
         * @example
         * if (await Authorization.usernameAsync) {
         *     // do something
         * }
         */
        let usernameAsync: Promise<string>;
        /** Returns the user data for currently logged user. Prefer `userDefinitionAsync` as this one might block the UI if the `UserData`
         * is not already loaded.
         * @returns User data for currently logged user.
         * @example
         * if (Authorization.userDefinition.IsAdmin) {
         *     // do something
         * }
         */
        let userDefinition: UserDefinition;
        /** Returns the user data for currently logged user.
         * @returns User data for currently logged user.
         * @example
         * if ((await Authorization.userDefinitionAsync).IsAdmin) {
         *     // do something
         * }
         */
        let userDefinitionAsync: Promise<UserDefinition>;
    }

    /** @deprecated use alertDialog */
    const alert: typeof alertDialog;
    /** @deprecated use confirmDialog */
    const confirm: typeof confirmDialog;
    /** @deprecated use informationDialog */
    const information: typeof informationDialog;
    /** @deprecated use successDialog */
    const success: typeof successDialog;
    /** @deprecated use warningDialog */
    const warning: typeof warningDialog;

    /**
     * A string to lowercase function that handles special Turkish
     * characters like 'ı'. Left in for compatibility reasons.
     */
    function turkishLocaleToLower(a: string): string;
    /**
     * A string to uppercase function that handles special Turkish
     * characters like 'ı'. Left in for compatibility reasons.
     */
    function turkishLocaleToUpper(a: string): string;
    /**
     * This is an alias for Culture.stringCompare, left in for compatibility reasons.
     */
    let turkishLocaleCompare: (a: string, b: string) => number;
    /** @deprecated Use stringFormat */
    let format: typeof stringFormat;
    /** @deprecated Use stringFormatLocale */
    let localeFormat: typeof stringFormatLocale;
    /**
     * Formats a number containing number of minutes into a string in the format "d.hh:mm".
     * @param n The number of minutes.
     */
    function formatDayHourAndMin(n: number): string;
    /**
     * Parses a time string in the format "hh:mm" into a number containing number of minutes.
     * Returns NaN if the hours not in range 0-23 or minutes not in range 0-59.
     * @param value The string to parse.
     */
    function parseHourAndMin(value: string): number;
    /**
     * Parses a string in the format "d.hh:mm" into a number containing number of minutes.
     * Returns NaN if the hours not in range 0-23 or minutes not in range 0-59.
     * Returns NULL if the string is empty or whitespace.
     */
    function parseDayHourAndMin(s: string): number;

    /**
     * Adds an empty option to the select.
     * @param select the select element
     */
    function addEmptyOption(select: ArrayLike<HTMLElement> | HTMLSelectElement): void;
    /**
     * Adds an option to the select.
     */
    function addOption(select: ArrayLike<HTMLElement> | HTMLSelectElement, key: string, text: string): void;
    /** @deprecated use htmlEncode as it also encodes quotes */
    const attrEncode: typeof htmlEncode;
    /** Clears the options in the select element */
    function clearOptions(select: HTMLElement | ArrayLike<HTMLElement>): void;
    /**
     * Finds the first element with the given relative id to the source element.
     * It can handle underscores in the source element id.
     * @param element the source element
     * @param relativeId the relative id to the source element
     * @param context the context element (optional)
     * @returns the element with the given relative id to the source element.
     */
    function findElementWithRelativeId(element: HTMLElement | ArrayLike<HTMLElement>, relativeId: string, context?: HTMLElement): HTMLElement;
    /**
     * Creates a new DIV and appends it to the body.
     * @returns the new DIV element.
     */
    function newBodyDiv(): HTMLDivElement;
    /**
     * Returns the outer HTML of the element.
     */
    function outerHtml(element: Element | ArrayLike<HTMLElement>): string;
    /**
     * Appends child at first argument to given node at second argument.
     * From https://github.com/alex-kinokon/jsx-dom.
     * @param child Child element or elements
     * @param node Target parent element
     */
    function appendChild(child: any, node: HTMLElement): void;

    function getWidgetName(type: Function): string;
    function associateWidget(widget: {
        domNode: HTMLElement;
    }): void;
    function deassociateWidget(widget: {
        domNode: HTMLElement;
    }): void;
    function tryGetWidget<TWidget>(element: Element | ArrayLike<HTMLElement> | string, type?: {
        new (...args: any[]): TWidget;
    }): TWidget;
    function getWidgetFrom<TWidget>(element: ArrayLike<HTMLElement> | Element | string, type?: {
        new (...args: any[]): TWidget;
    }): TWidget;
    type IdPrefixType = {
        [key: string]: string;
        Form: string;
        Tabs: string;
        Toolbar: string;
        PropertyGrid: string;
    };
    function useIdPrefix(prefix: string): IdPrefixType;
    type WidgetProps<P> = {
        id?: string;
        class?: string;
        element?: ((el: HTMLElement) => void) | HTMLElement | ArrayLike<HTMLElement> | string;
    } & NoInfer<P>;
    type EditorProps<T> = WidgetProps<T> & {
        initialValue?: any;
        maxLength?: number;
        name?: string;
        placeholder?: string;
        required?: boolean;
        readOnly?: boolean;
    };

    class Widget<P = {}> {
        static typeInfo: ClassTypeInfo<"Serenity.Widget">;
        private static nextWidgetNumber;
        protected readonly options: WidgetProps<P>;
        protected readonly uniqueName: string;
        readonly idPrefix: string;
        readonly domNode: HTMLElement;
        constructor(props: WidgetProps<P>);
        destroy(): void;
        static createDefaultElement(): HTMLElement;
        /**
         * Returns a Fluent(this.domNode) object
         */
        get element(): Fluent;
        protected addCssClass(): void;
        protected getCssClass(): string;
        static getWidgetName(type: Function): string;
        addValidationRule(rule: (input: HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement) => string, uniqueName?: string): void;
        addValidationRule(uniqueName: string, rule: (input: HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement) => string): void;
        getFieldElement(): HTMLElement;
        getGridField(): Fluent;
        change(handler: (e: Event) => void): void;
        changeSelect2(handler: (e: Event) => void): void;
        static create<TWidget extends Widget<P>, P>(params: CreateWidgetParams<TWidget, P>): TWidget;
        protected getCustomAttribute<TAttr>(attrType: {
            new (...args: any[]): TAttr;
        }, inherit?: boolean): TAttr;
        protected internalInit(): void;
        init(): this;
        /**
         * Returns the main element for this widget or the document fragment.
         * As widgets may get their elements from props unlike regular JSX widgets,
         * this method should not be overridden. Override renderContents() instead.
         */
        render(): any;
        protected internalRenderContents(): void;
        protected renderContents(): any | void;
        get props(): WidgetProps<P>;
        protected syncOrAsyncThen<T>(syncMethod: (() => T), asyncMethod: (() => PromiseLike<T>), then: (v: T) => void): void;
        protected useIdPrefix(): IdPrefixType;
    }
    class EditorWidget<P> extends Widget<EditorProps<P>> {
        static typeInfo: ClassTypeInfo<"Serenity.EditorWidget">;
        constructor(props: EditorProps<P>);
    }
    interface CreateWidgetParams<TWidget extends Widget<P>, P> {
        type?: {
            new (options?: P): TWidget;
            prototype: TWidget;
        };
        options?: P & WidgetProps<{}>;
        container?: HTMLElement | ArrayLike<HTMLElement>;
        element?: (e: Fluent) => void;
        init?: (w: TWidget) => void;
    }

    function GridPageInit<TGrid extends Widget<P>, P>({ type, props }: {
        type: CreateWidgetParams<TGrid, P>["type"];
        props?: WidgetProps<P>;
    }): HTMLElement;
    function PanelPageInit<TPanel extends Widget<P>, P>({ type, props }: {
        type: CreateWidgetParams<TPanel, P>["type"];
        props?: WidgetProps<P>;
    }): HTMLElement;
    function gridPageInit<TGrid extends Widget<P>, P>(grid: TGrid & {
        domNode: HTMLElement;
    }): TGrid;
    function gridPageInit<TGrid extends Widget<P>, P>(type: CreateWidgetParams<TGrid, P>["type"], props?: WidgetProps<P>): TGrid;
    function panelPageInit<TGrid extends Widget<P>, P>(panel: TGrid & {
        domNode: HTMLElement;
    }): TGrid;
    function panelPageInit<TGrid extends Widget<P>, P>(type: CreateWidgetParams<TGrid, P>["type"], props?: WidgetProps<P>): TGrid;
    function initFullHeightGridPage(gridDiv: HTMLElement | ArrayLike<HTMLElement> | {
        domNode: HTMLElement;
    }, opt?: {
        noRoute?: boolean;
        setHeight?: boolean;
    }): void;
    function layoutFillHeightValue(element: HTMLElement | ArrayLike<HTMLElement>): number;
    function layoutFillHeight(element: HTMLElement | ArrayLike<HTMLElement>): void;
    function isMobileView(): boolean;
    function triggerLayoutOnShow(element: HTMLElement | ArrayLike<HTMLElement>): void;
    function centerDialog(el: HTMLElement | ArrayLike<HTMLElement>): void;

    namespace LayoutTimer {
        function store(key: number): void;
        function trigger(key: number): void;
        function onSizeChange(element: () => HTMLElement, handler: () => void, width?: boolean, height?: boolean): number;
        function onWidthChange(element: () => HTMLElement, handler: () => void): number;
        function onHeightChange(element: () => HTMLElement, handler: () => void): number;
        function onShown(element: () => HTMLElement, handler: () => void): number;
        function off(key: number): number;
    }
    function executeOnceWhenVisible(el: HTMLElement | ArrayLike<HTMLElement>, callback: Function): void;
    function executeEverytimeWhenVisible(el: HTMLElement | ArrayLike<HTMLElement>, callback: Function, callNowIfVisible: boolean): void;

    /** @deprecated prefer localText for better discoverability */
    const text: typeof localText;
    function dbText(prefix: string): ((key: string) => string);
    function prefixedText(prefix: string): (text: string, key: string | ((p?: string) => string)) => string;
    function dbTryText(prefix: string): ((key: string) => string);
    namespace LT {
        /** @deprecated Use addLocalText */
        const add: typeof addLocalText;
        /** @deprecated Use localText */
        const getDefault: typeof localText;
    }

    interface HandleRouteEvent extends Event {
        route: string;
        parts: string[];
        index: number;
        isInitial: boolean;
    }
    namespace Router {
        let enabled: boolean;
        function navigate(newHash: string, tryBack?: boolean, silent?: boolean): void;
        function replace(newHash: string, tryBack?: boolean): void;
        function replaceLast(newHash: string, tryBack?: boolean): void;
        function dialog(owner: HTMLElement | ArrayLike<HTMLElement>, element: HTMLElement | ArrayLike<HTMLElement>, dialogHash: () => string): void;
        function resolve(newHash?: string): void;
    }

    namespace ScriptData {
        function bindToChange(name: string, onChange: () => void): void | (() => void);
        const canLoad: typeof canLoadScriptData;
        function ensure<TData = any>(name: string, dynJS?: boolean): TData;
        function reload<TData = any>(name: string, dynJS?: boolean): TData;
        function reloadAsync<TData = any>(name: string): Promise<TData>;
        const set: typeof setScriptData;
    }
    /**
     * Check if a dynamic script with provided name is available in the cache
     * or it is a registered script name
     * @param name Dynamic script name
     * @returns True if already available or registered
     */
    function canLoadScriptData(name: string): boolean;
    function getRemoteData<TData = any>(key: string): TData;
    function getLookup<TItem>(key: string): Lookup<TItem>;
    function reloadLookup<TItem = any>(key: string): Lookup<TItem>;
    function getColumns(key: string): PropertyItem[];
    function getColumnsAsync(key: string): Promise<PropertyItem[]>;
    function getColumnsData(key: string): PropertyItemsData;
    const getColumnsDataAsync: typeof getColumnsScript;
    function getForm(key: string): PropertyItem[];
    function getFormAsync(key: string): Promise<PropertyItem[]>;
    function getFormData(key: string): PropertyItemsData;
    const getFormDataAsync: typeof getFormScript;
    function getTemplate(key: string): string;

    function setEquality(request: ListRequest, field: string, value: any): void;
    interface PostToServiceOptions {
        url?: string;
        service?: string;
        target?: string;
        request: any;
    }
    interface PostToUrlOptions {
        url?: string;
        target?: string;
        params: any;
    }
    function parseQueryString(s?: string): {};
    function postToService(options: PostToServiceOptions): void;
    function postToUrl(options: PostToUrlOptions): void;

    /**
     * Checks if the string ends with the specified substring.
     * @deprecated Use .endsWith method of String directly
     * @param s String to check.
     * @param suffix Suffix to check.
     * @returns True if the string ends with the specified substring.
      */
    function endsWith(s: string, suffix: string): boolean;
    /**
     * Checks if the string is empty or null. Prefer (!s) instead.
     * @param s String to check.
     * @returns True if the string is empty or null.
     */
    function isEmptyOrNull(s: string): boolean;
    /**
     * Checks if the string is empty or null or whitespace. Prefer !s?.Trim() instead.
     * @param s String to check.
     * @returns True if the string is empty or null or whitespace.
     */
    function isTrimmedEmpty(s: string): boolean;
    /**
     * Pads the string to the left with the specified character.
     * @param s String to pad.
     * @param len Target length of the string.
     * @param ch Character to pad with.
     * @returns Padded string.
     */
    function padLeft(s: string | number, len: number, ch?: string): any;
    /**
     * Checks if the string starts with the prefix
     * @deprecated Use .startsWith method of String directly
     * @param s String to check.
     * @param prefix Prefix to check.
     * @returns True if the string starts with the prefix.
     */
    function startsWith(s: string, prefix: string): boolean;
    /**
     * Converts the string to single line by removing line end characters
     * @param str String to convert.
     */
    function toSingleLine(str: string): string;
    /**
     * Trims the whitespace characters from the end of the string
     */
    var trimEnd: (s: string) => any;
    /**
     * Trims the whitespace characters from the start of the string
     */
    var trimStart: (s: string) => any;
    /**
     * Trims the whitespace characters from the start and end of the string
     * This returns empty string even when the string is null or undefined.
     */
    function trim(s: string): string;
    /**
     * Trims the whitespace characters from the start and end of the string
     * Returns empty string if the string is null or undefined.
     */
    function trimToEmpty(s: string): string;
    /**
     * Trims the whitespace characters from the start and end of the string
     * Returns null if the string is null, undefined or whitespace.
     */
    function trimToNull(s: string): string;
    /**
     * Replaces all occurrences of the search string with the replacement string.
     * @param str String to replace.
     * @param find String to find.
     * @param replace String to replace with.
     * @returns Replaced string.
     */
    function replaceAll(str: string, find: string, replace: string): string;
    /**
     * Pads the start of string to make it the specified length.
     * @param n The number to pad.
     * @param len Target length of the string.
     */
    function zeroPad(n: number, len: number): string;

    type Dictionary<TItem> = {
        [key: string]: TItem;
    };
    /** @deprecated Use ?? operator */
    function coalesce(a: any, b: any): any;
    /** @deprecated Use a != null */
    function isValue(a: any): boolean;
    let today: () => Date;
    function extend<T = any>(a: T, b: T): T;
    function deepClone<T = any>(a: T, a2?: any, a3?: any): T;
    interface TypeMember {
        name: string;
        type: MemberType;
        attr?: any[];
        getter?: string;
        setter?: string;
    }
    enum MemberType {
        field = 4,
        property = 16
    }
    function getMembers(type: any, memberTypes: MemberType): TypeMember[];
    function addTypeMember(type: any, member: TypeMember): TypeMember;
    function getTypes(from?: any): any[];
    function clearKeys(d: any): void;
    function keyOf<T>(prop: keyof T): keyof T;
    function cast(instance: any, type: Type): any;
    function safeCast(instance: any, type: Type): any;
    function initializeTypes(root: any, pre: string, limit: number): void;
    class Exception extends Error {
        constructor(message: string);
    }
    class ArgumentNullException extends Exception {
        constructor(paramName: string, message?: string);
    }
    class InvalidCastException extends Exception {
        constructor(message: string);
    }

    function validatorAbortHandler(validator: Validator): void;
    function validateOptions(options?: ValidatorOptions): ValidatorOptions;
    namespace ValidationHelper {
        function asyncSubmit(form: ArrayLike<HTMLElement> | HTMLElement, validateBeforeSave: () => boolean, submitHandler: () => void): boolean;
        function submit(form: ArrayLike<HTMLElement> | HTMLElement, validateBeforeSave: () => boolean, submitHandler: () => void): boolean;
        function getValidator(elem: ArrayLike<HTMLElement> | HTMLElement): Validator;
        function validateElement(elem: ArrayLike<HTMLElement> | HTMLElement): void;
    }

    namespace Aggregators {
        function Avg(field: string): void;
        function WeightedAvg(field: string, weightedField: string): void;
        function Min(field: string): void;
        function Max(field: string): void;
        function Sum(field: string): void;
    }
    namespace AggregateFormatting {
        function formatMarkup<TItem = any>(totals: Slick.GroupTotals, column: Slick.Column<TItem>, aggType: string): string;
        function formatValue(column: Slick.Column, value: number): string;
        function groupTotalsFormatter<TItem = any>(totals: Slick.GroupTotals, column: Slick.Column<TItem>): string;
    }

    type Format<TItem = any> = (ctx: Slick.FormatterContext<TItem>) => Slick.FormatterResult;

    interface Formatter {
        format(ctx: Slick.FormatterContext): Slick.FormatterResult;
    }
    interface GroupInfo<TItem> {
        getter?: any;
        formatter?: (p1: Slick.Group<TItem>) => string;
        comparer?: (a: Slick.Group<TItem>, b: Slick.Group<TItem>) => number;
        aggregators?: any[];
        aggregateCollapsed?: boolean;
        lazyTotalsCalculation?: boolean;
    }
    interface PagerOptions {
        view?: any;
        showRowsPerPage?: boolean;
        rowsPerPage?: number;
        rowsPerPageOptions?: number[];
        onChangePage?: (newPage: number) => void;
        onRowsPerPageChange?: (n: number) => void;
    }
    interface SummaryOptions {
        aggregators: any[];
    }
    interface PagingOptions {
        rowsPerPage?: number;
        page?: number;
    }

    interface RemoteViewOptions {
        autoLoad?: boolean;
        idField?: string;
        contentType?: string;
        dataType?: string;
        filter?: any;
        params?: any;
        onSubmit?: CancellableViewCallback<any>;
        url?: string;
        localSort?: boolean;
        sortBy?: any;
        rowsPerPage?: number;
        seekToPage?: number;
        onProcessData?: RemoteViewProcessCallback<any>;
        method?: string;
        inlineFilters?: boolean;
        groupItemMetadataProvider?: Slick.GroupItemMetadataProvider;
        onAjaxCall?: RemoteViewAjaxCallback<any>;
        getItemMetadata?: (p1?: any, p2?: number) => any;
        errorMsg?: string;
    }
    interface PagingInfo {
        rowsPerPage: number;
        page: number;
        totalCount: number;
        loading: boolean;
        error: string;
        dataView: RemoteView<any>;
    }
    type CancellableViewCallback<TEntity> = (view: RemoteView<TEntity>) => boolean | void;
    type RemoteViewAjaxCallback<TEntity> = (view: RemoteView<TEntity>, options: ServiceOptions<ListResponse<TEntity>>) => boolean | void;
    type RemoteViewFilter<TEntity> = (item: TEntity, view: RemoteView<TEntity>) => boolean;
    type RemoteViewProcessCallback<TEntity> = (data: ListResponse<TEntity>, view: RemoteView<TEntity>) => ListResponse<TEntity>;
    interface RemoteView<TEntity> {
        onSubmit: CancellableViewCallback<TEntity>;
        onDataChanged: Slick.EventEmitter;
        onDataLoading: Slick.EventEmitter;
        onDataLoaded: Slick.EventEmitter;
        onPagingInfoChanged: Slick.EventEmitter;
        onRowCountChanged: Slick.EventEmitter;
        onRowsChanged: Slick.EventEmitter;
        onRowsOrCountChanged: Slick.EventEmitter;
        getPagingInfo(): PagingInfo;
        onGroupExpanded: Slick.EventEmitter;
        onGroupCollapsed: Slick.EventEmitter;
        onAjaxCall: RemoteViewAjaxCallback<TEntity>;
        onProcessData: RemoteViewProcessCallback<TEntity>;
        addData(data: ListResponse<TEntity>): void;
        beginUpdate(): void;
        endUpdate(): void;
        deleteItem(id: any): void;
        getItems(): TEntity[];
        setFilter(filter: RemoteViewFilter<TEntity>): void;
        getFilter(): RemoteViewFilter<TEntity>;
        getFilteredItems(): any;
        getGroupItemMetadataProvider(): Slick.GroupItemMetadataProvider;
        setGroupItemMetadataProvider(value: Slick.GroupItemMetadataProvider): void;
        fastSort: any;
        setItems(items: any[], newIdProperty?: boolean | string): void;
        getIdPropertyName(): string;
        getItemById(id: any): TEntity;
        getGrandTotals(): any;
        getGrouping(): GroupInfo<TEntity>[];
        getGroups(): any[];
        getRowById(id: any): number;
        getRowByItem(item: any): number;
        getRows(): any[];
        mapItemsToRows(itemArray: any[]): any[];
        mapRowsToIds(rowArray: number[]): any[];
        mapIdsToRows(idAray: any[]): number[];
        setFilterArgs(args: any): void;
        setRefreshHints(hints: any[]): void;
        insertItem(insertBefore: number, item: any): void;
        sortedAddItem(item: any): void;
        sortedUpdateItem(id: any, item: any): void;
        syncGridSelection(grid: any, preserveHidden?: boolean, preserveHiddenOnSelectionChange?: boolean): void;
        syncGridCellCssStyles(grid: any, key: string): void;
        getItemMetadata(i: number): any;
        updateItem(id: any, item: TEntity): void;
        addItem(item: TEntity): void;
        getIdxById(id: any): any;
        getItemByIdx(index: number): any;
        setGrouping(groupInfo: GroupInfo<TEntity>[]): void;
        collapseAllGroups(level: number): void;
        expandAllGroups(level: number): void;
        expandGroup(keys: any[]): void;
        collapseGroup(keys: any[]): void;
        setSummaryOptions(options: SummaryOptions): void;
        setPagingOptions(options: PagingOptions): void;
        refresh(): void;
        populate(): void;
        populateLock(): void;
        populateUnlock(): void;
        getItem(row: number): any;
        getLength(): number;
        rowsPerPage: number;
        errormsg: string;
        params: any;
        getLocalSort(): boolean;
        setLocalSort(value: boolean): void;
        sort(comparer?: (a: any, b: any) => number, ascending?: boolean): void;
        reSort(): void;
        sortBy: string[];
        url: string;
        method: string;
        idField: string;
        seekToPage?: number;
    }
    class RemoteView<TEntity> {
        constructor(options: RemoteViewOptions);
    }

    class IBooleanValue {
    }
    interface IBooleanValue {
        get_value(): boolean;
        set_value(value: boolean): void;
    }

    class IDoubleValue {
    }
    interface IDoubleValue {
        get_value(): any;
        set_value(value: any): void;
    }

    class IDialog {
    }
    interface IDialog {
        dialogOpen(asPanel?: boolean): void;
    }

    class IEditDialog {
    }
    interface IEditDialog {
        load(entityOrId: any, done: () => void, fail?: (p1: any) => void): void;
    }

    class IGetEditValue {
    }
    interface IGetEditValue {
        getEditValue(property: PropertyItem, target: any): void;
    }

    interface IReadOnly {
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
    }
    class IReadOnly {
    }

    class ISetEditValue {
    }
    interface ISetEditValue {
        setEditValue(source: any, property: PropertyItem): void;
    }

    class IStringValue {
    }
    interface IStringValue {
        get_value(): string;
        set_value(value: string): void;
    }

    interface IValidateRequired {
        get_required(): boolean;
        set_required(value: boolean): void;
    }
    class IValidateRequired {
    }

    class EnumKeyAttribute {
        value: string;
        constructor(value: string);
    }
    class DisplayNameAttribute {
        displayName: string;
        constructor(displayName: string);
    }
    class CategoryAttribute {
        category: string;
        constructor(category: string);
    }
    class ColumnsKeyAttribute {
        value: string;
        constructor(value: string);
    }
    class CloseButtonAttribute {
        value: boolean;
        constructor(value?: boolean);
    }
    class CssClassAttribute {
        cssClass: string;
        constructor(cssClass: string);
    }
    class DefaultValueAttribute {
        value: any;
        constructor(value: any);
    }
    class DialogTypeAttribute {
        value: any;
        constructor(value: any);
    }
    class EditorOptionAttribute {
        key: string;
        value: any;
        constructor(key: string, value: any);
    }
    class EditorTypeAttributeBase {
        editorType: string;
        constructor(editorType: string);
        setParams(editorParams: any): void;
    }
    class EditorTypeAttribute extends EditorTypeAttributeBase {
        constructor(editorType: string);
    }
    class ElementAttribute {
        value: string;
        constructor(value: string);
    }
    class EntityTypeAttribute {
        value: string;
        constructor(value: string);
    }
    class FilterableAttribute {
        value: boolean;
        constructor(value?: boolean);
    }
    class FlexifyAttribute {
        value: boolean;
        constructor(value?: boolean);
    }
    class FormKeyAttribute {
        value: string;
        constructor(value: string);
    }
    class GeneratedCodeAttribute {
        origin?: string;
        constructor(origin?: string);
    }
    class HiddenAttribute {
        constructor();
    }
    class HintAttribute {
        hint: string;
        constructor(hint: string);
    }
    class IdPropertyAttribute {
        value: string;
        constructor(value: string);
    }
    class InsertableAttribute {
        value: boolean;
        constructor(value?: boolean);
    }
    class IsActivePropertyAttribute {
        value: string;
        constructor(value: string);
    }
    class ItemNameAttribute {
        value: string;
        constructor(value: string);
    }
    class LocalTextPrefixAttribute {
        value: string;
        constructor(value: string);
    }
    class MaximizableAttribute {
        value: boolean;
        constructor(value?: boolean);
    }
    class MaxLengthAttribute {
        maxLength: number;
        constructor(maxLength: number);
    }
    class NamePropertyAttribute {
        value: string;
        constructor(value: string);
    }
    class OneWayAttribute {
    }
    class OptionAttribute {
    }
    class OptionsTypeAttribute {
        value: Function;
        constructor(value: Function);
    }
    class PanelAttribute {
        value: boolean;
        constructor(value?: boolean);
    }
    class PlaceholderAttribute {
        value: string;
        constructor(value: string);
    }
    class ReadOnlyAttribute {
        value: boolean;
        constructor(value?: boolean);
    }
    class RequiredAttribute {
        isRequired: boolean;
        constructor(isRequired?: boolean);
    }
    class ResizableAttribute {
        value: boolean;
        constructor(value?: boolean);
    }
    class ResponsiveAttribute {
        value: boolean;
        constructor(value?: boolean);
    }
    class ServiceAttribute {
        value: string;
        constructor(value: string);
    }
    class StaticPanelAttribute {
        value: boolean;
        constructor(value?: boolean);
    }
    class UpdatableAttribute {
        value: boolean;
        constructor(value?: boolean);
    }

    enum CaptureOperationType {
        Before = 0,
        Delete = 1,
        Insert = 2,
        Update = 3
    }

    interface DataChangeInfo extends Event {
        operationType: string;
        entityId: any;
        entity: any;
    }

    namespace Decorators {
        const classType: typeof classTypeInfo;
        const editorType: typeof editorTypeInfo;
        const interfaceType: typeof interfaceTypeInfo;
        const formatterType: typeof formatterTypeInfo;
        function registerType(): (target: Function & {
            [typeInfoProperty]: any;
        }) => void;
        function registerClass(nameOrIntf?: string | any[], intf2?: any[]): (target: Function) => void;
        function registerInterface(nameOrIntf?: string | any[], intf2?: any[]): (target: Function) => void;
        function registerEditor(nameOrIntf?: string | any[], intf2?: any[]): (target: Function) => void;
        function registerEnum(target: any, enumKey?: string, name?: string): void;
        function registerEnumType(target: any, name?: string, enumKey?: string): void;
        function registerFormatter(nameOrIntf?: string | any[], intf2?: any[]): (target: Function) => void;
        function enumKey(value: string): (target: Function) => void;
        function option(): (target: Object, propertyKey: string) => void;
        function closeButton(value?: boolean): (target: Function) => void;
        function dialogType(value: any): (target: Function) => void;
        function editor(): (target: Function) => void;
        function element(value: string): (target: Function) => void;
        function filterable(value?: boolean): (target: Function) => void;
        function itemName(value: string): (target: Function) => void;
        function maximizable(value?: boolean): (target: Function) => void;
        function optionsType(value: Function): (target: Function) => void;
        function panel(value?: boolean): (target: Function) => void;
        function resizable(value?: boolean): (target: Function) => void;
        function responsive(value?: boolean): (target: Function) => void;
        function service(value: string): (target: Function) => void;
        function staticPanel(value?: boolean): (target: Function) => void;
    }

    namespace DialogTypeRegistry {
        function get(key: string): any;
        function reset(): void;
        function tryGet(key: string): any;
    }

    namespace EditorTypeRegistry {
        function get(key: string): any;
        function reset(): void;
        function tryGet(key: string): any;
    }

    namespace EnumTypeRegistry {
        function get(key: string): Function;
        function reset(): void;
        function tryGet(key: string): any;
    }

    namespace ReflectionUtils {
        function getPropertyValue(o: any, property: string): any;
        function setPropertyValue(o: any, property: string, value: any): void;
        function makeCamelCase(s: string): string;
    }

    interface IRowDefinition {
        readonly deletePermission?: string;
        readonly idProperty?: string;
        readonly insertPermission?: string;
        readonly isActiveProperty?: string;
        readonly isDeletedProperty?: string;
        readonly localTextPrefix?: string;
        readonly nameProperty?: string;
        readonly readPermission?: string;
        readonly updatePermission?: string;
    }

    namespace LazyLoadHelper {
        const executeOnceWhenShown: typeof Q.executeOnceWhenVisible;
        const executeEverytimeWhenShown: typeof Q.executeEverytimeWhenVisible;
    }

    class PrefixedContext {
        readonly idPrefix: string;
        constructor(idPrefix: string);
        byId(id: string): Fluent;
        w<TWidget>(id: string, type: {
            new (...args: any[]): TWidget;
        }): TWidget;
    }

    interface ToolButtonProps {
        action?: string;
        title?: string;
        hint?: string;
        cssClass?: string;
        icon?: IconClassName;
        onClick?: any;
        hotkey?: string;
        hotkeyAllowDefault?: boolean;
        hotkeyContext?: any;
        separator?: (false | true | 'left' | 'right' | 'both');
        visible?: boolean | (() => boolean);
        disabled?: boolean | (() => boolean);
    }
    interface ToolButton extends ToolButtonProps {
        hotkey?: string;
        hotkeyAllowDefault?: boolean;
        hotkeyContext?: any;
        separator?: (false | true | 'left' | 'right' | 'both');
    }
    function ToolbarButton(tb: ToolButtonProps): HTMLElement;
    interface ToolbarOptions {
        buttons?: ToolButton[];
        hotkeyContext?: any;
    }
    class Toolbar<P extends ToolbarOptions = ToolbarOptions> extends Widget<P> {
        protected renderContents(): HTMLDivElement;
        destroy(): void;
        protected mouseTrap: any;
        createButton(container: Fluent, tb: ToolButton): void;
        findButton(className: string): Fluent<HTMLElement>;
        updateInterface(): void;
    }

    class TemplatedWidget<P> extends Widget<P> {
        protected byId<TElement extends HTMLElement = HTMLElement>(id: string): Fluent<TElement>;
        protected findById<TElement extends HTMLElement = HTMLElement>(id: string): TElement;
        protected getTemplate(): string;
        protected renderContents(): void;
    }

    class TemplatedDialog<P> extends TemplatedWidget<P> {
        static createDefaultElement(): HTMLDivElement;
        protected tabs: Fluent<HTMLElement>;
        protected toolbar: Toolbar;
        protected validator: any;
        protected dialog: Dialog;
        constructor(props?: WidgetProps<P>);
        destroy(): void;
        protected addCssClass(): void;
        protected getInitialDialogTitle(): string;
        protected isStaticPanel(): boolean;
        protected getDialogOptions(): DialogOptions;
        protected initDialog(): void;
        protected initUIDialog(): void;
        dialogOpen(asPanel?: boolean): void;
        protected onDialogOpen(): void;
        protected getToolbarButtons(): ToolButton[];
        protected initToolbar(): void;
        protected getValidatorOptions(): any;
        protected initValidator(): void;
        protected resetValidation(): void;
        protected validateForm(): boolean;
        arrange(): void;
        protected onDialogClose(result?: string): void;
        protected getDialogButtons(): DialogButton[];
        dialogClose(result?: string): void;
        get dialogTitle(): string;
        set dialogTitle(value: string);
        protected initTabs(): void;
        protected handleResponsive(): void;
    }

    class TemplatedPanel<P = {}> extends TemplatedWidget<P> {
        constructor(props: WidgetProps<P>);
        destroy(): void;
        protected tabs: Fluent;
        protected toolbar: Toolbar;
        protected validator: Validator;
        protected isPanel: boolean;
        protected responsive: boolean;
        arrange(): void;
        protected getToolbarButtons(): ToolButton[];
        protected getValidatorOptions(): any;
        protected initTabs(): void;
        protected initToolbar(): void;
        protected initValidator(): void;
        protected resetValidation(): void;
        protected validateForm(): boolean;
    }

    class CascadedWidgetLink<TParent extends Widget<any>> {
        private parentType;
        private widget;
        private parentChange;
        constructor(parentType: {
            new (...args: any[]): TParent;
        }, widget: Widget<any>, parentChange: (p1: TParent) => void);
        private _parentID;
        bind(): TParent;
        unbind(): TParent;
        get_parentID(): string;
        set_parentID(value: string): void;
    }

    namespace TabsExtensions {
        function setDisabled(tabs: ArrayLike<HTMLElement> | HTMLElement, tabKey: string, isDisabled: boolean): void;
        function toggle(tabs: ArrayLike<HTMLElement> | HTMLElement, tabKey: string, visible: boolean): void;
        function activeTabKey(tabs: ArrayLike<HTMLElement> | HTMLElement): string;
        function indexByKey(tabs: ArrayLike<HTMLElement> | HTMLElement): Record<string, number>;
        function selectTab(tabs: HTMLElement | ArrayLike<HTMLElement>, tabKey: string | number): void;
        function initialize(tabs: HTMLElement | ArrayLike<HTMLElement>, activeChange: () => void): Fluent<HTMLElement>;
        function destroy(tabs: HTMLElement | ArrayLike<HTMLElement>): void;
    }

    namespace ReflectionOptionsSetter {
        function set(target: any, options: any): void;
    }

    class PropertyGrid<P extends PropertyGridOptions = PropertyGridOptions> extends Widget<P> {
        private editors;
        private items;
        protected renderContents(): void;
        destroy(): void;
        private createItems;
        private createCategoryDiv;
        private categoryLinkClick;
        private determineText;
        private createField;
        private getCategoryOrder;
        private createCategoryLinks;
        get_editors(): Widget<any>[];
        get_items(): PropertyItem[];
        get_idPrefix(): string;
        get_mode(): PropertyGridMode;
        set_mode(value: PropertyGridMode): void;
        private static setMaxLength;
        load(source: any): void;
        save(target?: any): any;
        get value(): any;
        set value(val: any);
        private canModifyItem;
        updateInterface(): void;
        enumerateItems(callback: (p1: PropertyItem, p2: Widget<any>) => void): void;
    }
    enum PropertyGridMode {
        insert = 1,
        update = 2
    }
    interface PropertyGridOptions {
        idPrefix?: string;
        items: PropertyItem[];
        useCategories?: boolean;
        categoryOrder?: string;
        defaultCategory?: string;
        localTextPrefix?: string;
        mode?: PropertyGridMode;
    }

    class PropertyPanel<TItem, P> extends TemplatedPanel<P> {
        private _entity;
        private _entityId;
        constructor(props: WidgetProps<P>);
        destroy(): void;
        protected initPropertyGrid(): void;
        protected loadInitialEntity(): void;
        protected getFormKey(): string;
        protected getPropertyGridOptions(): PropertyGridOptions;
        protected getPropertyItems(): PropertyItem[];
        protected getSaveEntity(): TItem;
        protected get_entity(): TItem;
        protected get_entityId(): any;
        protected set_entity(value: TItem): void;
        protected set_entityId(value: any): void;
        protected validateBeforeSave(): boolean;
        protected propertyGrid: PropertyGrid;
    }

    namespace SubDialogHelper {
        function bindToDataChange(dialog: any, owner: Widget<any>, dataChange: (ev: DataChangeInfo) => void, useTimeout?: boolean): any;
        function triggerDataChange(dialog: Widget<any>): any;
        function triggerDataChanged(element: HTMLElement | ArrayLike<HTMLElement>): void;
        function bubbleDataChange(dialog: any, owner: Widget<any>, useTimeout?: boolean): any;
        function cascade(cascadedDialog: {
            domNode: HTMLElement;
        }, ofElement: HTMLElement | ArrayLike<HTMLElement>): any;
        function cascadedDialogOffset(element: HTMLElement | ArrayLike<HTMLElement>): any;
    }

    namespace DialogExtensions {
        function dialogResizable(dialog: HTMLElement | ArrayLike<HTMLElement>, w?: any, h?: any, mw?: any, mh?: any): void;
        function dialogMaximizable(dialog: HTMLElement | ArrayLike<HTMLElement>): void;
    }

    class PropertyDialog<TItem, P> extends TemplatedDialog<P> {
        protected entity: TItem;
        protected entityId: any;
        protected propertyItemsData: PropertyItemsData;
        protected isClosable(): boolean;
        protected isStatic(): boolean;
        constructor(props?: WidgetProps<P>);
        protected propertyItemsReady(itemsData: PropertyItemsData): void;
        protected afterInit(): void;
        protected useAsync(): boolean;
        destroy(): void;
        protected getDialogOptions(): DialogOptions;
        protected getDialogButtons(): DialogButton[];
        protected okClick(): void;
        protected okClickValidated(): void;
        protected cancelClick(): void;
        protected initPropertyGrid(): void;
        protected getFormKey(): string;
        protected getPropertyGridOptions(): PropertyGridOptions;
        protected getPropertyItems(): PropertyItem[];
        protected getPropertyItemsData(): PropertyItemsData;
        protected getPropertyItemsDataAsync(): Promise<PropertyItemsData>;
        protected getSaveEntity(): TItem;
        protected loadInitialEntity(): void;
        protected get_entity(): TItem;
        protected set_entity(value: TItem): void;
        protected get_entityId(): any;
        protected set_entityId(value: any): void;
        protected validateBeforeSave(): boolean;
        protected updateTitle(): void;
        protected propertyGrid: PropertyGrid;
        protected getTemplate(): string;
    }

    namespace EditorUtils {
        function getDisplayText(editor: Widget<any>): string;
        function getValue(editor: Widget<any>): any;
        function saveValue(editor: Widget<any>, item: PropertyItem, target: any): void;
        function setValue(editor: Widget<any>, value: any): void;
        function loadValue(editor: Widget<any>, item: PropertyItem, source: any): void;
        function setReadonly(elements: Element | ArrayLike<Element>, isReadOnly: boolean): void;
        function setReadOnly(widget: Widget<any>, isReadOnly: boolean): void;
        function setRequired(widget: Widget<any>, isRequired: boolean): void;
        function setContainerReadOnly(container: ArrayLike<HTMLElement> | HTMLElement, readOnly: boolean): void;
    }

    class StringEditor<P = {}> extends EditorWidget<P> {
        static typeInfo: EditorTypeInfo<"Serenity.StringEditor">;
        readonly domNode: HTMLInputElement;
        static createDefaultElement(): HTMLInputElement;
        get value(): string;
        protected get_value(): string;
        set value(value: string);
        protected set_value(value: string): void;
    }

    class PasswordEditor<TOptions = {}> extends StringEditor<TOptions> {
        static typeInfo: EditorTypeInfo<"Serenity.PasswordEditor">;
        static createDefaultElement(): HTMLInputElement;
    }

    interface TextAreaEditorOptions {
        cols?: number;
        rows?: number;
    }
    class TextAreaEditor<P extends TextAreaEditorOptions = TextAreaEditorOptions> extends EditorWidget<P> {
        static createDefaultElement(): HTMLTextAreaElement;
        constructor(props: EditorProps<P>);
        get value(): string;
        protected get_value(): string;
        set value(value: string);
        protected set_value(value: string): void;
    }

    class BooleanEditor<P = {}> extends EditorWidget<P> {
        static createDefaultElement(): HTMLInputElement;
        readonly domNode: HTMLInputElement;
        get value(): boolean;
        protected get_value(): boolean;
        set value(value: boolean);
        protected set_value(value: boolean): void;
    }

    interface AutoNumericOptions {
        aDec?: string;
        allowedAutoStrip?: RegExp;
        allowLeading?: boolean;
        altDec?: string;
        aForm?: boolean;
        aNum?: string;
        aNeg?: string;
        aSep?: string;
        aSign?: string;
        aNegRegAutoStrip?: string;
        aPad?: boolean;
        dGroup?: string;
        /** internal */
        holder?: any;
        lZero?: string;
        mDec?: number;
        mInt?: number;
        mRound?: string;
        nBracket?: string;
        numRegAutoStrip?: RegExp;
        oEvent?: any;
        pSign?: string;
        /** internal */
        runOnce?: boolean;
        skipFirstAutoStrip?: RegExp;
        skipLastAutoStrip?: RegExp;
        tagList?: string[];
        vMax?: any;
        vMin?: any;
        wEmpty?: string;
    }
    class AutoNumeric {
        static init(input: HTMLInputElement, options: AutoNumericOptions): void;
        /** method to remove settings and stop autoNumeric() */
        static destroy(input: HTMLInputElement): void;
        /** method to update settings - can call as many times */
        static updateOptions(input: HTMLInputElement, options: AutoNumericOptions): void;
        /** returns a formatted strings for "input:text" fields Uses jQuery's .val() method*/
        static setValue(input: HTMLInputElement, valueIn: number | string): string;
        /** method to get the unformatted value from a specific input field, returns a numeric value */
        static getValue(input: HTMLInputElement): string;
        /** returns the settings object for those who need to look under the hood */
        static getSettings(input: HTMLInputElement): AutoNumericOptions;
        static hasInstance(input: HTMLInputElement): boolean;
    }

    interface DecimalEditorOptions {
        minValue?: string;
        maxValue?: string;
        decimals?: any;
        padDecimals?: any;
        allowNegatives?: boolean;
    }
    class DecimalEditor<P extends DecimalEditorOptions = DecimalEditorOptions> extends EditorWidget<P> implements IDoubleValue {
        static createDefaultElement(): HTMLInputElement;
        readonly domNode: HTMLInputElement;
        constructor(props: EditorProps<P>);
        destroy(): void;
        protected initAutoNumeric(): void;
        protected getAutoNumericOptions(): any;
        get_value(): number;
        get value(): number;
        set_value(value: number): void;
        set value(v: number);
        get_isValid(): boolean;
        static defaultAutoNumericOptions(): AutoNumericOptions;
    }

    interface IntegerEditorOptions {
        minValue?: number;
        maxValue?: number;
        allowNegatives?: boolean;
    }
    class IntegerEditor<P extends IntegerEditorOptions = IntegerEditorOptions> extends EditorWidget<P> implements IDoubleValue {
        static createDefaultElement(): HTMLInputElement;
        readonly domNode: HTMLInputElement;
        constructor(props: EditorProps<P>);
        destroy(): void;
        protected initAutoNumeric(): void;
        protected getAutoNumericOptions(): any;
        get_value(): number;
        get value(): number;
        set_value(value: number): void;
        set value(v: number);
        get_isValid(): boolean;
    }

    interface DateEditorOptions {
        yearRange?: string;
        minValue?: string;
        maxValue?: string;
        sqlMinMax?: boolean;
    }
    class DateEditor<P extends DateEditorOptions = DateEditorOptions> extends EditorWidget<P> implements IStringValue, IReadOnly {
        static createDefaultElement(): HTMLInputElement;
        readonly domNode: HTMLInputElement;
        constructor(props: EditorProps<P>);
        destroy(): void;
        get_value(): string;
        get value(): string;
        set_value(value: string): void;
        set value(v: string);
        private get_valueAsDate;
        get valueAsDate(): Date;
        private set_valueAsDate;
        set valueAsDate(v: Date);
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
        get_minValue(): string;
        set_minValue(value: string): void;
        get_maxValue(): string;
        set_maxValue(value: string): void;
        get_minDate(): Date;
        set_minDate(value: Date): void;
        get_maxDate(): Date;
        set_maxDate(value: Date): void;
        get_sqlMinMax(): boolean;
        set_sqlMinMax(value: boolean): void;
        static dateInputChange: (e: Event) => void;
        static dateInputKeyup(e: KeyboardEvent): void;
        static useFlatpickr: boolean;
        getFlatpickrOptions(input: HTMLElement): any;
        createFlatPickrTrigger(): HTMLElement;
        static uiPickerZIndexWorkaround(el: HTMLElement | ArrayLike<HTMLElement>): void;
    }

    class DateTimeEditor<P extends DateTimeEditorOptions = DateTimeEditorOptions> extends EditorWidget<P> implements IStringValue, IReadOnly {
        static createDefaultElement(): HTMLInputElement;
        readonly domNode: HTMLInputElement;
        private time;
        private lastSetValue;
        private lastSetValueGet;
        constructor(props: EditorProps<P>);
        destroy(): void;
        getFlatpickrOptions(): any;
        createFlatPickrTrigger(): HTMLElement;
        get_value(): string;
        get value(): string;
        set_value(value: string): void;
        private getInplaceNowText;
        private getDisplayFormat;
        set value(v: string);
        private get_valueAsDate;
        get valueAsDate(): Date;
        private set_valueAsDate;
        set valueAsDate(value: Date);
        get_minValue(): string;
        set_minValue(value: string): void;
        get_maxValue(): string;
        set_maxValue(value: string): void;
        get_minDate(): Date;
        set_minDate(value: Date): void;
        get_maxDate(): Date;
        set_maxDate(value: Date): void;
        get_sqlMinMax(): boolean;
        set_sqlMinMax(value: boolean): void;
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
        static roundToMinutes(date: Date, minutesStep: number): Date;
        static getTimeOptions: (fromHour: number, fromMin: number, toHour: number, toMin: number, stepMins: number) => string[];
    }
    interface DateTimeEditorOptions {
        startHour?: any;
        endHour?: any;
        intervalMinutes?: any;
        minValue?: string;
        maxValue?: string;
        yearRange?: string;
        useUtc?: boolean;
        seconds?: boolean;
        inputOnly?: boolean;
    }

    interface TimeEditorOptions {
        noEmptyOption?: boolean;
        startHour?: any;
        endHour?: any;
        intervalMinutes?: any;
    }
    class TimeEditor<P extends TimeEditorOptions = TimeEditorOptions> extends EditorWidget<P> {
        static createDefaultElement(): HTMLElement;
        readonly domNode: HTMLSelectElement;
        private minutes;
        constructor(props: EditorProps<P>);
        get value(): number;
        protected get_value(): number;
        set value(value: number);
        protected set_value(value: number): void;
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
    }

    interface EmailEditorOptions {
        domain?: string;
        readOnlyDomain?: boolean;
    }
    class EmailEditor<P extends EmailEditorOptions = EmailEditorOptions> extends EditorWidget<P> {
        static createDefaultElement(): HTMLInputElement;
        readonly domNode: HTMLInputElement;
        private readonly domain;
        constructor(props: EditorProps<P>);
        static registerValidationMethods(): void;
        get_value(): string;
        get value(): string;
        set_value(value: string): void;
        set value(v: string);
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
    }

    class EmailAddressEditor<P = {}> extends StringEditor<P> {
        static createDefaultElement(): HTMLInputElement;
        constructor(props: EditorProps<P>);
    }

    class URLEditor<P = {}> extends StringEditor<P> {
        constructor(props: EditorProps<P>);
    }

    interface RadioButtonEditorOptions {
        enumKey?: string;
        enumType?: any;
        lookupKey?: string;
    }
    class RadioButtonEditor<P extends RadioButtonEditorOptions = RadioButtonEditorOptions> extends EditorWidget<P> implements IReadOnly {
        constructor(props: EditorProps<P>);
        protected addRadio(value: string, text: string): void;
        get_value(): string;
        get value(): string;
        set_value(value: string): void;
        set value(v: string);
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
    }

    type ComboboxType = "select2";
    type ComboboxFormatResult = string | Element | DocumentFragment;
    interface ComboboxItem<TSource = any> {
        id?: string;
        text?: string;
        source?: TSource;
        disabled?: boolean;
    }
    interface ComboboxSearchQuery {
        searchTerm?: string;
        idList?: string[];
        skip?: number;
        take?: number;
        checkMore?: boolean;
        initSelection?: boolean;
        signal?: AbortSignal;
    }
    interface ComboboxSearchResult<TItem> {
        items: TItem[];
        more: boolean;
    }
    interface ComboboxOptions<TSource = any> {
        allowClear?: boolean;
        createSearchChoice?: (s: string) => ComboboxItem<TSource>;
        element?: HTMLInputElement | HTMLSelectElement | Element[];
        /** Allow arbitrary values for items */
        arbitraryValues?: boolean;
        formatSelection?: (p1: ComboboxItem<TSource>) => ComboboxFormatResult;
        formatResult?: (p1: ComboboxItem<TSource>) => ComboboxFormatResult;
        minimumResultsForSearch?: number;
        multiple?: boolean;
        /** Page size to use while loading or displaying results */
        pageSize?: number;
        placeholder?: string;
        /** Callback to get options specific to the combobox provider type */
        providerOptions?: (type: ComboboxType, opt: ComboboxOptions) => any;
        search?: (query: ComboboxSearchQuery) => (PromiseLike<ComboboxSearchResult<ComboboxItem<TSource>>> | ComboboxSearchResult<ComboboxItem<TSource>>);
        /** Type delay for searching, default is 200 */
        typeDelay?: number;
    }
    class Combobox<TItem = any> {
        private el;
        static defaults: ComboboxOptions;
        constructor(opt: ComboboxOptions);
        private createSelect2;
        abortPendingQuery(): void;
        abortInitSelection(): void;
        dispose(): void;
        get container(): HTMLElement;
        get type(): ComboboxType;
        get isMultiple(): boolean;
        getSelectedItem(): ComboboxItem;
        getSelectedItems(): ComboboxItem[];
        getValue(): string;
        getValues(): string[];
        setValue(value: string, triggerChange?: boolean): void;
        setValues(value: string[], triggerChange?: boolean): void;
        closeDropdown(): void;
        openDropdown(): void;
        static getInstance(el: Element | ArrayLike<Element>): Combobox;
    }
    function stripDiacritics(str: string): string;

    interface ComboboxCommonOptions {
        allowClear?: boolean;
        delimited?: boolean;
        minimumResultsForSearch?: any;
        multiple?: boolean;
    }
    interface ComboboxFilterOptions {
        cascadeFrom?: string;
        cascadeField?: string;
        cascadeValue?: any;
        filterField?: string;
        filterValue?: any;
    }
    interface ComboboxInplaceAddOptions {
        inplaceAdd?: boolean;
        inplaceAddPermission?: string;
        dialogType?: string;
        autoComplete?: boolean;
    }
    interface ComboboxEditorOptions extends ComboboxFilterOptions, ComboboxInplaceAddOptions, ComboboxCommonOptions {
    }
    class ComboboxEditor<P, TItem> extends Widget<P> implements ISetEditValue, IGetEditValue, IStringValue, IReadOnly {
        static createDefaultElement(): HTMLInputElement;
        readonly domNode: HTMLInputElement;
        private combobox;
        private _items;
        private _itemById;
        protected lastCreateTerm: string;
        constructor(props: EditorProps<P>);
        destroy(): void;
        protected hasAsyncSource(): boolean;
        protected asyncSearch(query: ComboboxSearchQuery): PromiseLike<ComboboxSearchResult<TItem>>;
        protected getTypeDelay(): any;
        protected emptyItemText(): string;
        protected getPageSize(): number;
        protected getIdField(): any;
        protected itemId(item: TItem): string;
        protected getTextField(): any;
        protected itemText(item: TItem): string;
        protected itemDisabled(item: TItem): boolean;
        protected mapItem(item: TItem): ComboboxItem;
        protected mapItems(items: TItem[]): ComboboxItem[];
        protected allowClear(): boolean;
        protected isMultiple(): boolean;
        protected abortPendingQuery(): void;
        protected getComboboxOptions(): ComboboxOptions;
        get_delimited(): boolean;
        get items(): ComboboxItem<TItem>[];
        set items(value: ComboboxItem<TItem>[]);
        protected get itemById(): {
            [key: string]: ComboboxItem<TItem>;
        };
        protected set itemById(value: {
            [key: string]: ComboboxItem<TItem>;
        });
        clearItems(): void;
        addItem(item: ComboboxItem<TItem>): void;
        addOption(key: string, text: string, source?: any, disabled?: boolean): void;
        protected addInplaceCreate(addTitle: string, editTitle: string): void;
        protected useInplaceAdd(): boolean;
        protected isAutoComplete(): boolean;
        getCreateSearchChoice(getName: (z: any) => string): (s: string) => {
            id: string;
            text: string;
        };
        setEditValue(source: any, property: PropertyItem): void;
        getEditValue(property: PropertyItem, target: any): void;
        protected getComboboxContainer(): HTMLElement;
        protected get_items(): ComboboxItem<TItem>[];
        protected get_itemByKey(): {
            [key: string]: ComboboxItem<TItem>;
        };
        static filterByText<TItem>(items: TItem[], getText: (item: TItem) => string, term: string): TItem[];
        get_value(): string;
        get value(): string;
        set_value(value: string): void;
        set value(v: string);
        get selectedItem(): TItem;
        get selectedItems(): TItem[];
        protected get_values(): string[];
        get values(): string[];
        protected set_values(value: string[]): void;
        set values(value: string[]);
        protected get_text(): string;
        get text(): string;
        get_readOnly(): boolean;
        get readOnly(): boolean;
        private updateInplaceReadOnly;
        set_readOnly(value: boolean): void;
        set readOnly(value: boolean);
        protected getCascadeFromValue(parent: Widget<any>): any;
        protected cascadeLink: CascadedWidgetLink<Widget<any>>;
        protected setCascadeFrom(value: string): void;
        protected get_cascadeFrom(): string;
        get cascadeFrom(): string;
        protected set_cascadeFrom(value: string): void;
        set cascadeFrom(value: string);
        protected get_cascadeField(): string;
        get cascadeField(): string;
        protected set_cascadeField(value: string): void;
        set cascadeField(value: string);
        protected get_cascadeValue(): any;
        get cascadeValue(): any;
        protected set_cascadeValue(value: any): void;
        set cascadeValue(value: any);
        protected get_filterField(): string;
        get filterField(): string;
        protected set_filterField(value: string): void;
        set filterField(value: string);
        protected get_filterValue(): any;
        get filterValue(): any;
        protected set_filterValue(value: any): void;
        set filterValue(value: any);
        protected cascadeItems(items: TItem[]): TItem[];
        protected filterItems(items: TItem[]): TItem[];
        protected updateItems(): void;
        protected getDialogTypeKey(): string;
        protected createEditDialog(callback: (dlg: IEditDialog) => void): void;
        onInitNewEntity: (entity: TItem) => void;
        protected initNewEntity(entity: TItem): void;
        protected setEditDialogReadOnly(dialog: any): void;
        protected editDialogDataChange(): void;
        protected setTermOnNewEntity(entity: TItem, term: string): void;
        protected inplaceCreateClick(e: Event): void;
        openDropdown(): void;
        openDialogAsPanel: boolean;
    }

    class SelectEditor<P extends SelectEditorOptions = SelectEditorOptions> extends ComboboxEditor<P, ComboboxItem> {
        constructor(props: EditorProps<P>);
        getItems(): any[];
        protected emptyItemText(): string;
        updateItems(): void;
    }
    interface SelectEditorOptions extends ComboboxCommonOptions {
        items?: any[];
        emptyOptionText?: string;
    }

    /**
     * Adapted from 3.5.x version of Select2 (https://github.com/select2/select2), removing jQuery dependency
     */
    type Select2Element = HTMLInputElement | HTMLSelectElement;
    type Select2FormatResult = string | Element | DocumentFragment;
    interface Select2QueryOptions {
        element?: Select2Element;
        term?: string;
        page?: number;
        context?: any;
        callback?: (p1: Select2Result) => void;
        matcher?: (p1: any, p2: any, p3?: any) => boolean;
    }
    interface Select2Item {
        id?: string;
        text?: string;
        source?: any;
        children?: Select2Item[];
        disabled?: boolean;
        locked?: boolean;
    }
    interface Select2Result {
        hasError?: boolean;
        errorInfo?: any;
        results: Select2Item[];
        more?: boolean;
        context?: any;
    }
    interface Select2AjaxOptions extends RequestInit {
        headers?: Record<string, string>;
        url?: string | ((term: string, page: number, context: any) => string);
        quietMillis?: number;
        data?: (p1: string, p2: number, p3: any) => any;
        results?: (p1: any, p2: number, p3: any) => any;
        params?: (() => any) | any;
        onError?(response: any, info?: any): void | boolean;
        onSuccess?(response: any): void;
    }
    interface Select2Options {
        element?: Select2Element;
        width?: any;
        minimumInputLength?: number;
        maximumInputLength?: number;
        minimumResultsForSearch?: number;
        maximumSelectionSize?: any;
        placeholder?: string;
        placeholderOption?: any;
        separator?: string;
        allowClear?: boolean;
        multiple?: boolean;
        closeOnSelect?: boolean;
        openOnEnter?: boolean;
        id?: (p1: any) => string;
        matcher?: (p1: string, p2: string, p3: HTMLElement) => boolean;
        sortResults?: (p1: any, p2: HTMLElement, p3: any) => any;
        formatAjaxError?: (p1: any, p2: any) => Select2FormatResult;
        formatMatches?: (matches: number) => Select2FormatResult;
        formatSelection?: (p1: any, p2: HTMLElement, p3: (p1: string) => string) => Select2FormatResult;
        formatResult?: (p1: any, p2: HTMLElement, p3: any, p4: (p1: string) => string) => Select2FormatResult;
        formatResultCssClass?: (p1: any) => string;
        formatSelectionCssClass?: (item: Select2Item, container: HTMLElement) => string;
        formatNoMatches?: (input: string) => Select2FormatResult;
        formatLoadMore?: (pageNumber: number) => Select2FormatResult;
        formatSearching?: () => Select2FormatResult;
        formatInputTooLong?: (input: string, max: number) => Select2FormatResult;
        formatInputTooShort?: (input: string, min: number) => Select2FormatResult;
        formatSelectionTooBig?: (p1: number) => Select2FormatResult;
        createSearchChoice?: (p1: string) => Select2Item;
        createSearchChoicePosition?: string | ((list: Select2Item[], item: Select2Item) => void);
        initSelection?: (p1: HTMLElement, p2: (p1: any) => void) => void;
        tokenizer?: (p1: string, p2: any, p3: (p1: any) => any, p4: any) => string;
        tokenSeparators?: any;
        query?: (p1: Select2QueryOptions) => void;
        ajax?: Select2AjaxOptions;
        data?: any;
        tags?: ((string | Select2Item)[]) | (() => (string | Select2Item)[]);
        containerCss?: any;
        containerCssClass?: any;
        dropdownCss?: any;
        dropdownCssClass?: any;
        dropdownAutoWidth?: boolean;
        dropdownParent?: (input: HTMLElement) => HTMLElement;
        adaptContainerCssClass?: (p1: string) => string;
        adaptDropdownCssClass?: (p1: string) => string;
        escapeMarkup?: (p1: string) => string;
        searchInputPlaceholder?: string;
        selectOnBlur?: boolean;
        blurOnChange?: boolean;
        loadMorePadding?: number;
        nextSearchTerm?: (p1: any, p2: string) => string;
        populateResults?: (container: HTMLElement, results: Select2Item[], query: Select2QueryOptions) => void;
        shouldFocusInput?: (p1: any) => boolean;
    }
    class Select2 {
        private el;
        constructor(opts?: Select2Options);
        private get instance();
        close(): void;
        get container(): HTMLElement;
        get dropdown(): HTMLElement;
        destroy(): void;
        get data(): (Select2Item | Select2Item[]);
        set data(value: Select2Item | Select2Item[]);
        disable(): void;
        enable(enabled?: boolean): void;
        focus(): void;
        get isFocused(): boolean;
        get isMultiple(): boolean;
        get opened(): boolean;
        open(): boolean;
        positionDropdown(): void;
        readonly(value?: boolean): void;
        get search(): HTMLInputElement;
        get val(): (string | string[]);
        set val(value: string[]);
        static getInstance(el: Select2Element): Select2;
        static readonly ajaxDefaults: Select2AjaxOptions;
        static readonly defaults: Select2Options;
        static stripDiacritics(str: string): string;
    }

    class DateYearEditor<P extends DateYearEditorOptions = DateYearEditorOptions> extends SelectEditor<P> {
        constructor(props: EditorProps<P>);
        getItems(): any[];
    }
    interface DateYearEditorOptions extends SelectEditorOptions {
        minYear?: string;
        maxYear?: string;
        descending?: boolean;
    }

    interface EnumEditorOptions extends ComboboxCommonOptions {
        enumKey?: string;
        enumType?: any;
    }
    class EnumEditor<P extends EnumEditorOptions = EnumEditorOptions> extends ComboboxEditor<P, ComboboxItem> {
        constructor(props: EditorProps<P>);
        protected updateItems(): void;
        protected allowClear(): boolean;
    }

    interface LookupEditorOptions extends ComboboxEditorOptions {
        lookupKey?: string;
        async?: boolean;
    }
    abstract class LookupEditorBase<P extends LookupEditorOptions, TItem> extends ComboboxEditor<P, TItem> {
        private lookupChangeUnbind;
        constructor(props: EditorProps<P>);
        hasAsyncSource(): boolean;
        destroy(): void;
        protected getLookupKey(): string;
        protected lookup: Lookup<TItem>;
        protected getLookupAsync(): PromiseLike<Lookup<TItem>>;
        protected getLookup(): Lookup<TItem>;
        protected getItems(lookup: Lookup<TItem>): TItem[];
        protected getIdField(): any;
        protected getItemText(item: TItem, lookup: Lookup<TItem>): any;
        protected mapItem(item: TItem): ComboboxItem<TItem>;
        protected getItemDisabled(item: TItem, lookup: Lookup<TItem>): boolean;
        updateItems(): void;
        protected asyncSearch(query: ComboboxSearchQuery): Promise<ComboboxSearchResult<TItem>>;
        protected getDialogTypeKey(): string;
        protected setCreateTermOnNewEntity(entity: TItem, term: string): void;
        protected editDialogDataChange(): void;
    }
    class LookupEditor<P extends LookupEditorOptions = LookupEditorOptions> extends LookupEditorBase<P, {}> {
        constructor(props: EditorProps<P>);
    }

    interface ServiceLookupEditorOptions extends ComboboxEditorOptions {
        service?: string;
        idField?: string;
        textField?: string;
        pageSize?: number;
        minimumResultsForSearch?: any;
        sort?: string[];
        columnSelection?: ColumnSelection;
        includeColumns?: string[];
        excludeColumns?: string[];
        includeDeleted?: boolean;
        containsField?: string;
        equalityFilter?: any;
        criteria?: any[];
    }
    abstract class ServiceLookupEditorBase<P extends ServiceLookupEditorOptions, TItem> extends ComboboxEditor<P, TItem> {
        protected getDialogTypeKey(): string;
        protected getService(): string;
        protected getServiceUrl(): string;
        protected getIncludeColumns(): string[];
        protected getSort(): any[];
        protected getCascadeCriteria(): any[];
        protected getFilterCriteria(): any[];
        protected getIdListCriteria(idList: any[]): any[];
        protected getCriteria(query: ComboboxSearchQuery): any[];
        protected getListRequest(query: ComboboxSearchQuery): ListRequest;
        protected getServiceCallOptions(query: ComboboxSearchQuery): ServiceOptions<ListResponse<TItem>>;
        protected hasAsyncSource(): boolean;
        protected canSearch(byId: boolean): boolean;
        protected asyncSearch(query: ComboboxSearchQuery): Promise<ComboboxSearchResult<TItem>>;
    }
    class ServiceLookupEditor<P extends ServiceLookupEditorOptions = ServiceLookupEditorOptions, TItem = any> extends ServiceLookupEditorBase<ServiceLookupEditorOptions, TItem> {
        constructor(props: EditorProps<P>);
    }

    interface HtmlContentEditorOptions {
        cols?: number;
        rows?: number;
    }
    interface CKEditorConfig {
    }
    class HtmlContentEditor<P extends HtmlContentEditorOptions = HtmlContentEditorOptions> extends EditorWidget<P> implements IStringValue, IReadOnly {
        private _instanceReady;
        readonly domNode: HTMLTextAreaElement;
        static createDefaultElement(): HTMLTextAreaElement;
        constructor(props: EditorProps<P>);
        protected instanceReady(x: any): void;
        protected getLanguage(): string;
        protected getConfig(): CKEditorConfig;
        protected getEditorInstance(): any;
        destroy(): void;
        get_value(): string;
        get value(): string;
        set_value(value: string): void;
        set value(v: string);
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
        static CKEditorVer: string;
        static CKEditorBasePath: string;
        static getCKEditorBasePath(): string;
        static includeCKEditor(then: () => void): void;
    }
    class HtmlNoteContentEditor<P extends HtmlContentEditorOptions = HtmlContentEditorOptions> extends HtmlContentEditor<P> {
        protected getConfig(): CKEditorConfig;
    }
    class HtmlReportContentEditor<P extends HtmlContentEditorOptions = HtmlContentEditorOptions> extends HtmlContentEditor<P> {
        protected getConfig(): CKEditorConfig;
    }

    class MaskedEditor<P extends MaskedEditorOptions = MaskedEditorOptions> extends EditorWidget<P> {
        static createDefaultElement(): HTMLInputElement;
        readonly domNode: HTMLInputElement;
        constructor(props: EditorProps<P>);
        get value(): string;
        protected get_value(): string;
        set value(value: string);
        protected set_value(value: string): void;
    }
    interface MaskedEditorOptions {
        mask?: string;
        placeholder?: string;
    }

    interface RecaptchaOptions {
        siteKey?: string;
        language?: string;
    }
    class Recaptcha<P extends RecaptchaOptions = RecaptchaOptions> extends EditorWidget<P> implements IStringValue {
        constructor(props: EditorProps<P>);
        get_value(): string;
        set_value(value: string): void;
    }

    namespace UploadHelper {
        function addUploadInput(options: UploadInputOptions): Fluent;
        function checkImageConstraints(file: UploadResponse, opt: FileUploadConstraints): boolean;
        function fileNameSizeDisplay(name: string, bytes: number): string;
        function fileSizeDisplay(bytes: number): string;
        function hasImageExtension(filename: string): boolean;
        function thumbFileName(filename: string): string;
        function dbFileUrl(filename: string): string;
        function colorBox(link: HTMLElement | ArrayLike<HTMLElement>, options: any): void;
        function populateFileSymbols(c: HTMLElement | ArrayLike<HTMLElement>, items: UploadedFile[], displayOriginalName?: boolean, urlPrefix?: string): void;
    }
    interface UploadedFile {
        Filename?: string;
        OriginalName?: string;
    }
    interface UploadInputOptions {
        container?: HTMLElement | ArrayLike<HTMLElement>;
        zone?: HTMLElement | ArrayLike<HTMLElement>;
        progress?: HTMLElement | ArrayLike<HTMLElement>;
        inputName?: string;
        allowMultiple?: boolean;
        uploadIntent?: string;
        uploadUrl?: string;
        fileDone?: (p1: UploadResponse, p2: string, p3: any) => void;
    }
    interface UploadResponse extends ServiceResponse {
        TemporaryFile: string;
        Size: number;
        IsImage: boolean;
        Width: number;
        Height: number;
    }
    interface FileUploadConstraints {
        minWidth?: number;
        maxWidth?: number;
        minHeight?: number;
        maxHeight?: number;
        minSize?: number;
        maxSize?: number;
        allowNonImage?: boolean;
        originalNameProperty?: string;
    }

    interface FileUploadEditorOptions extends FileUploadConstraints {
        displayFileName?: boolean;
        uploadIntent?: string;
        uploadUrl?: string;
        urlPrefix?: string;
    }
    interface ImageUploadEditorOptions extends FileUploadEditorOptions {
    }
    class FileUploadEditor<P extends FileUploadEditorOptions = FileUploadEditorOptions> extends EditorWidget<P> implements IReadOnly, IGetEditValue, ISetEditValue, IValidateRequired {
        constructor(props: EditorProps<P>);
        protected getUploadInputOptions(): UploadInputOptions;
        protected addFileButtonText(): string;
        protected getToolButtons(): ToolButton[];
        protected populate(): void;
        protected updateInterface(): void;
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
        get_required(): boolean;
        set_required(value: boolean): void;
        get_value(): UploadedFile;
        get value(): UploadedFile;
        set_value(value: UploadedFile): void;
        set value(v: UploadedFile);
        getEditValue(property: PropertyItem, target: any): void;
        setEditValue(source: any, property: PropertyItem): void;
        protected entity: UploadedFile;
        protected toolbar: Toolbar;
        protected progress: Fluent;
        protected fileSymbols: Fluent;
        protected uploadInput: Fluent;
        protected hiddenInput: Fluent;
    }
    class ImageUploadEditor<P extends ImageUploadEditorOptions = ImageUploadEditorOptions> extends FileUploadEditor<P> {
        constructor(props: EditorProps<P>);
    }
    interface MultipleFileUploadEditorOptions extends FileUploadEditorOptions {
        jsonEncodeValue?: boolean;
    }
    class MultipleFileUploadEditor<P extends MultipleFileUploadEditorOptions = MultipleFileUploadEditorOptions> extends EditorWidget<P> implements IReadOnly, IGetEditValue, ISetEditValue, IValidateRequired {
        private entities;
        private toolbar;
        private fileSymbols;
        private uploadInput;
        protected progress: Fluent;
        protected hiddenInput: Fluent;
        constructor(props: EditorProps<P>);
        protected getUploadInputOptions(): UploadInputOptions;
        protected addFileButtonText(): string;
        protected getToolButtons(): ToolButton[];
        protected populate(): void;
        protected updateInterface(): void;
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
        get_required(): boolean;
        set_required(value: boolean): void;
        get_value(): UploadedFile[];
        get value(): UploadedFile[];
        set_value(value: UploadedFile[]): void;
        set value(v: UploadedFile[]);
        getEditValue(property: PropertyItem, target: any): void;
        setEditValue(source: any, property: PropertyItem): void;
        get jsonEncodeValue(): boolean;
        set jsonEncodeValue(value: boolean);
    }
    class MultipleImageUploadEditor<P extends ImageUploadEditorOptions = ImageUploadEditorOptions> extends MultipleFileUploadEditor<P> {
        constructor(props: EditorProps<P>);
    }

    interface QuickFilterArgs<TWidget> {
        field?: string;
        widget?: TWidget;
        request?: ListRequest;
        equalityFilter?: any;
        value?: any;
        active?: boolean;
        handled?: boolean;
    }
    interface QuickFilter<TWidget extends Widget<P>, P> {
        field?: string;
        type?: {
            new (options?: P): TWidget;
            prototype: TWidget;
        };
        handler?: (h: QuickFilterArgs<TWidget>) => void;
        title?: string;
        options?: P & WidgetProps<{}>;
        element?: (e: Fluent) => void;
        init?: (w: TWidget) => void;
        separator?: boolean;
        cssClass?: string;
        loadState?: (w: TWidget, state: any) => void;
        saveState?: (w: TWidget) => any;
        displayText?: (w: TWidget, label: string) => string;
    }

    interface QuickFilterBarOptions {
        filters: QuickFilter<Widget<any>, any>[];
        getTitle?: (filter: QuickFilter<Widget<any>, any>) => string;
        idPrefix?: string;
    }
    class QuickFilterBar<P extends QuickFilterBarOptions = QuickFilterBarOptions> extends Widget<P> {
        constructor(props: WidgetProps<P>);
        addSeparator(): void;
        add<TWidget extends Widget<any>, TOptions>(opt: QuickFilter<TWidget, TOptions>): TWidget;
        addDateRange(field: string, title?: string): DateEditor;
        static dateRange(field: string, title?: string): QuickFilter<DateEditor, DateTimeEditorOptions>;
        addDateTimeRange(field: string, title?: string): DateTimeEditor<DateTimeEditorOptions>;
        static dateTimeRange(field: string, title?: string, useUtc?: boolean): QuickFilter<DateTimeEditor, DateTimeEditorOptions>;
        addBoolean(field: string, title?: string, yes?: string, no?: string): SelectEditor;
        static boolean(field: string, title?: string, yes?: string, no?: string): QuickFilter<SelectEditor, SelectEditorOptions>;
        onChange: (e: Event) => void;
        private submitHandlers;
        destroy(): void;
        onSubmit(request: ListRequest): void;
        protected add_submitHandlers(action: (request: ListRequest) => void): void;
        protected remove_submitHandlers(action: (request: ListRequest) => void): void;
        protected clear_submitHandlers(): void;
        find<TWidget>(type: {
            new (...args: any[]): TWidget;
        }, field: string): TWidget;
        tryFind<TWidget>(type: {
            new (...args: any[]): TWidget;
        }, field: string): TWidget;
    }

    interface QuickSearchField {
        name: string;
        title: string;
    }
    interface QuickSearchInputOptions {
        typeDelay?: number;
        loadingParentClass?: string;
        filteredParentClass?: string;
        onSearch?: (p1: string, p2: string, p3: (p1: boolean) => void) => void;
        fields?: QuickSearchField[];
    }
    class QuickSearchInput<P extends QuickSearchInputOptions = QuickSearchInputOptions> extends Widget<P> {
        static createDefaultElement(): HTMLInputElement;
        readonly domNode: HTMLInputElement;
        private lastValue;
        private field;
        private fieldLink;
        private fieldChanged;
        private timer;
        constructor(props: WidgetProps<P>);
        protected checkIfValueChanged(): void;
        get_value(): string;
        get_field(): QuickSearchField;
        set_field(value: QuickSearchField): void;
        protected updateInputPlaceHolder(): void;
        restoreState(value: string, field: QuickSearchField): void;
        protected searchNow(value: string): void;
    }

    interface FilterOperator {
        key?: string;
        title?: string;
        format?: string;
    }
    namespace FilterOperators {
        const isTrue = "true";
        const isFalse = "false";
        const contains = "contains";
        const startsWith = "startswith";
        const EQ = "eq";
        const NE = "ne";
        const GT = "gt";
        const GE = "ge";
        const LT = "lt";
        const LE = "le";
        const BW = "bw";
        const IN = "in";
        const isNull = "isnull";
        const isNotNull = "isnotnull";
        const toCriteriaOperator: {
            [key: string]: string;
        };
    }

    interface FilterLine {
        field?: string;
        operator?: string;
        isOr?: boolean;
        leftParen?: boolean;
        rightParen?: boolean;
        validationError?: string;
        criteria?: any[];
        displayText?: string;
        state?: any;
    }

    class FilterStore {
        constructor(fields: PropertyItem[]);
        static getCriteriaFor(items: FilterLine[]): any[];
        static getDisplayTextFor(items: FilterLine[]): string;
        private changed;
        private displayText;
        private fields;
        private fieldByName;
        private items;
        get_fields(): PropertyItem[];
        get_fieldByName(): {
            [key: string]: PropertyItem;
        };
        get_items(): FilterLine[];
        raiseChanged(): void;
        add_changed(value: (e: Event, a: any) => void): void;
        remove_changed(value: (e: Event, a: any) => void): void;
        get_activeCriteria(): any[];
        get_displayText(): string;
    }
    function delegateCombine(delegate1: any, delegate2: any): any;
    function delegateRemove(delegate1: any, delegate2: any): any;
    function delegateContains(targets: any[], object: any, method: any): boolean;

    interface IFiltering {
        createEditor(): void;
        getCriteria(): CriteriaWithText;
        getOperators(): FilterOperator[];
        loadState(state: any): void;
        saveState(): any;
        get_field(): PropertyItem;
        set_field(value: PropertyItem): void;
        get_container(): HTMLElement;
        set_container(value: HTMLElement): void;
        get_operator(): FilterOperator;
        set_operator(value: FilterOperator): void;
    }
    class IFiltering {
    }
    interface CriteriaWithText {
        criteria?: any[];
        displayText?: string;
    }
    interface IQuickFiltering {
        initQuickFilter(filter: QuickFilter<Widget<any>, any>): void;
    }
    class IQuickFiltering {
    }
    abstract class BaseFiltering implements IFiltering, IQuickFiltering {
        private field;
        get_field(): PropertyItem;
        set_field(value: PropertyItem): void;
        private container;
        get_container(): HTMLElement;
        set_container(value: HTMLElement): void;
        private operator;
        get_operator(): FilterOperator;
        set_operator(value: FilterOperator): void;
        abstract getOperators(): FilterOperator[];
        protected appendNullableOperators(list: FilterOperator[]): FilterOperator[];
        protected appendComparisonOperators(list: FilterOperator[]): FilterOperator[];
        protected isNullable(): boolean;
        createEditor(): void;
        protected operatorFormat(op: FilterOperator): string;
        protected getTitle(field: PropertyItem): string;
        protected displayText(op: FilterOperator, values?: any[]): string;
        protected getCriteriaField(): string;
        getCriteria(): CriteriaWithText;
        loadState(state: any): void;
        saveState(): string;
        protected argumentNull(): Error;
        validateEditorValue(value: string): string;
        getEditorValue(): string;
        getEditorText(): string;
        initQuickFilter(filter: QuickFilter<Widget<any>, any>): void;
    }
    abstract class BaseEditorFiltering<TEditor extends Widget<any>> extends BaseFiltering {
        editorTypeRef: any;
        constructor(editorTypeRef: any);
        protected useEditor(): boolean;
        protected editor: TEditor;
        createEditor(): void;
        protected useIdField(): boolean;
        getCriteriaField(): string;
        getEditorOptions(): any;
        loadState(state: any): void;
        saveState(): any;
        getEditorValue(): any;
        initQuickFilter(filter: QuickFilter<Widget<any>, any>): void;
    }
    class DateFiltering extends BaseEditorFiltering<DateEditor> {
        constructor();
        getOperators(): FilterOperator[];
    }
    class BooleanFiltering extends BaseFiltering {
        getOperators(): FilterOperator[];
    }
    class DateTimeFiltering extends BaseEditorFiltering<DateEditor> {
        constructor();
        getOperators(): FilterOperator[];
        getCriteria(): CriteriaWithText;
    }
    class DecimalFiltering extends BaseEditorFiltering<DecimalEditor> {
        constructor();
        getOperators(): FilterOperator[];
    }
    class EditorFiltering extends BaseEditorFiltering<Widget<any>> {
        readonly props: {
            editorType?: string;
            useRelative?: boolean;
            useLike?: boolean;
        };
        constructor(props?: {
            editorType?: string;
            useRelative?: boolean;
            useLike?: boolean;
        });
        get editorType(): string;
        set editorType(value: string);
        get useRelative(): boolean;
        set useRelative(value: boolean);
        get useLike(): boolean;
        set useLike(value: boolean);
        getOperators(): FilterOperator[];
        protected useEditor(): boolean;
        getEditorOptions(): any;
        createEditor(): void;
        protected useIdField(): boolean;
        initQuickFilter(filter: QuickFilter<Widget<any>, any>): void;
    }
    class EnumFiltering extends BaseEditorFiltering<EnumEditor> {
        constructor();
        getOperators(): FilterOperator[];
    }
    class IntegerFiltering extends BaseEditorFiltering<IntegerEditor> {
        constructor();
        getOperators(): FilterOperator[];
    }
    class LookupFiltering extends BaseEditorFiltering<LookupEditor> {
        constructor();
        getOperators(): FilterOperator[];
        protected useEditor(): boolean;
        protected useIdField(): boolean;
        getEditorText(): string;
    }
    class ServiceLookupFiltering extends BaseEditorFiltering<ServiceLookupEditor> {
        constructor();
        getOperators(): FilterOperator[];
        protected useEditor(): boolean;
        protected useIdField(): boolean;
        getEditorText(): string;
    }
    class StringFiltering extends BaseFiltering {
        getOperators(): FilterOperator[];
        validateEditorValue(value: string): string;
    }
    namespace FilteringTypeRegistry {
        function get(key: string): Function;
    }

    class FilterWidgetBase<P = {}> extends TemplatedWidget<P> {
        private store;
        private onFilterStoreChanged;
        constructor(props: WidgetProps<P>);
        destroy(): void;
        protected filterStoreChanged(): void;
        get_store(): FilterStore;
        set_store(value: FilterStore): void;
    }

    interface FilterFieldSelectOptions {
        fields: PropertyItem[];
    }
    class FilterPanel<P = {}> extends FilterWidgetBase<P> {
        private rowsDiv;
        constructor(props: WidgetProps<P>);
        private showInitialLine;
        get_showInitialLine(): boolean;
        set_showInitialLine(value: boolean): void;
        protected filterStoreChanged(): void;
        updateRowsFromStore(): void;
        private showSearchButton;
        get_showSearchButton(): boolean;
        set_showSearchButton(value: boolean): void;
        private updateStoreOnReset;
        get_updateStoreOnReset(): boolean;
        set_updateStoreOnReset(value: boolean): void;
        protected getTemplate(): string;
        protected initButtons(): void;
        protected searchButtonClick(e: Event): void;
        get_hasErrors(): boolean;
        search(): void;
        protected addButtonClick(e: Event): void;
        protected resetButtonClick(e: Event): void;
        protected findEmptyRow(): HTMLElement;
        protected addEmptyRow(popupField: boolean): HTMLElement;
        protected onRowFieldChange(e: Event): void;
        protected rowFieldChange(row: HTMLElement): void;
        protected removeFiltering(row: HTMLElement): void;
        protected populateOperatorList(row: HTMLElement): void;
        protected getFieldFor(row: HTMLElement): PropertyItem;
        protected getFilteringFor(row: HTMLElement): IFiltering;
        protected onRowOperatorChange(e: Event): void;
        protected rowOperatorChange(row: HTMLElement): void;
        protected deleteRowClick(e: Event): void;
        protected updateButtons(): void;
        protected andOrClick(e: Event): void;
        protected leftRightParenClick(e: Event): void;
        protected updateParens(): void;
    }

    class FilterDialog<P = {}> extends TemplatedDialog<P> {
        private filterPanel;
        constructor(props: WidgetProps<P>);
        get_filterPanel(): FilterPanel;
        protected getTemplate(): string;
        protected getDialogOptions(): DialogOptions;
        protected getDialogButtons(): DialogButton[];
    }

    class FilterDisplayBar<P = {}> extends FilterWidgetBase<P> {
        protected renderContents(): HTMLDivElement;
        protected filterStoreChanged(): void;
    }

    class SlickPager<P extends PagerOptions = PagerOptions> extends Widget<P> {
        private currentPage;
        private totalPages;
        private pageSize;
        private stat;
        constructor(props: WidgetProps<P>);
        _changePage(ctype: string): boolean;
        _updatePager(): void;
    }

    interface IDataGrid {
        getElement(): HTMLElement;
        getGrid(): Slick.Grid;
        getView(): RemoteView<any>;
        getFilterStore(): FilterStore;
    }

    interface GridRowSelectionMixinOptions {
        selectable?: (item: any) => boolean;
    }
    class GridRowSelectionMixin {
        private idField;
        private include;
        private grid;
        private options;
        constructor(grid: IDataGrid, options?: GridRowSelectionMixinOptions);
        updateSelectAll(): void;
        clear(): void;
        resetCheckedAndRefresh(): void;
        selectKeys(keys: string[]): void;
        getSelectedKeys(): string[];
        getSelectedAsInt32(): number[];
        getSelectedAsInt64(): number[];
        setSelectedKeys(keys: string[]): void;
        private isSelectable;
        static createSelectColumn(getMixin: () => GridRowSelectionMixin): Slick.Column;
    }
    interface GridRadioSelectionMixinOptions {
        selectable?: (item: any) => boolean;
    }
    class GridRadioSelectionMixin {
        private idField;
        private include;
        private grid;
        private options;
        constructor(grid: IDataGrid, options?: GridRadioSelectionMixinOptions);
        private isSelectable;
        clear(): void;
        resetCheckedAndRefresh(): void;
        getSelectedKey(): string;
        getSelectedAsInt32(): number;
        getSelectedAsInt64(): number;
        setSelectedKey(key: string): void;
        static createSelectColumn(getMixin: () => GridRadioSelectionMixin): Slick.Column;
    }
    namespace GridSelectAllButtonHelper {
        function update(grid: IDataGrid, getSelected: (p1: any) => boolean): void;
        function define(getGrid: () => IDataGrid, getId: (p1: any) => any, getSelected: (p1: any) => boolean, setSelected: (p1: any, p2: boolean) => void, text?: string, onClick?: () => void): ToolButton;
    }
    namespace GridUtils {
        function addToggleButton(toolDiv: HTMLElement | ArrayLike<HTMLElement>, cssClass: string, callback: (p1: boolean) => void, hint: string, initial?: boolean): void;
        function addIncludeDeletedToggle(toolDiv: HTMLElement | ArrayLike<HTMLElement>, view: RemoteView<any>, hint?: string, initial?: boolean): void;
        function addQuickSearchInput(toolDiv: HTMLElement | ArrayLike<HTMLElement>, view: RemoteView<any>, fields?: QuickSearchField[], onChange?: () => void): QuickSearchInput;
        function addQuickSearchInputCustom(container: HTMLElement | ArrayLike<HTMLElement>, onSearch: (p1: string, p2: string, done: (p3: boolean) => void) => void, fields?: QuickSearchField[]): QuickSearchInput;
        function makeOrderable(grid: Slick.Grid, handleMove: (rows: number[], insertBefore: number) => void): void;
        function makeOrderableWithUpdateRequest<TItem = any, TId = any>(grid: IDataGrid, getId: (item: TItem) => TId, getDisplayOrder: (item: TItem) => any, service: string, getUpdateRequest: (id: TId, order: number) => SaveRequest<TItem>): void;
    }
    namespace PropertyItemSlickConverter {
        function toSlickColumns(items: PropertyItem[]): Slick.Column[];
        function toSlickColumn(item: PropertyItem): Slick.Column;
    }
    namespace SlickFormatting {
        function getEnumText(enumKey: string, name: string): string;
        function treeToggle(getView: () => RemoteView<any>, getId: (x: any) => any, formatter: Format): Format;
        function date(format?: string): Format;
        function dateTime(format?: string): Format;
        function checkBox(): Format;
        function number(format: string): Format;
        function getItemType(link: HTMLElement | ArrayLike<HTMLElement>): string;
        function getItemId(link: HTMLElement | ArrayLike<HTMLElement>): string;
        function itemLinkText(itemType: string, id: any, text: Slick.FormatterResult, extraClass: string, encode: boolean): Slick.FormatterResult;
        function itemLink<TItem = any>(itemType: string, idField: string, getText: Format<TItem>, cssClass?: (ctx: Slick.FormatterContext<TItem>) => string, encode?: boolean): Format<TItem>;
    }
    namespace SlickHelper {
        function setDefaults(columns: Slick.Column[], localTextPrefix?: string): any;
    }
    namespace SlickTreeHelper {
        function filterCustom<TItem>(item: TItem, getParent: (x: TItem) => any): boolean;
        function filterById<TItem>(item: TItem, view: RemoteView<TItem>, getParentId: (x: TItem) => any): boolean;
        function setCollapsed<TItem>(items: TItem[], collapsed: boolean): void;
        function setCollapsedFlag<TItem>(item: TItem, collapsed: boolean): void;
        function setIndents<TItem>(items: TItem[], getId: (x: TItem) => any, getParentId: (x: TItem) => any, setCollapsed?: boolean): void;
        function toggleClick<TItem>(e: Event, row: number, cell: number, view: RemoteView<TItem>, getId: (x: TItem) => any): void;
    }
    class ColumnsBase<TRow = any> {
        constructor(items: Slick.Column<TRow>[]);
        valueOf(): Slick.Column<TRow>[];
    }

    interface IInitializeColumn {
        initializeColumn(column: Slick.Column): void;
    }
    class IInitializeColumn {
    }
    class BooleanFormatter implements Formatter {
        readonly props: {
            falseText?: string;
            trueText?: string;
        };
        constructor(props?: {
            falseText?: string;
            trueText?: string;
        });
        format(ctx: Slick.FormatterContext): string;
        get falseText(): string;
        set falseText(value: string);
        get trueText(): string;
        set trueText(value: string);
    }
    class CheckboxFormatter implements Formatter {
        static typeInfo: FormatterTypeInfo<"Serenity.CheckboxFormatter">;
        format(ctx: Slick.FormatterContext): string;
    }
    class DateFormatter implements Formatter {
        readonly props: {
            displayFormat?: string;
        };
        constructor(props?: {
            displayFormat?: string;
        });
        static format(value: any, format?: string): any;
        get displayFormat(): string;
        set displayFormat(value: string);
        format(ctx: Slick.FormatterContext): string;
    }
    class DateTimeFormatter extends DateFormatter {
        constructor();
    }
    class EnumFormatter implements Formatter {
        readonly props: {
            enumKey?: string;
        };
        constructor(props?: {
            enumKey?: string;
        });
        format(ctx: Slick.FormatterContext): string;
        get enumKey(): string;
        set enumKey(value: string);
        static format(enumType: any, value: any): string;
        static getText(enumKey: string, name: string): string;
        static getName(enumType: any, value: any): string;
    }
    class FileDownloadFormatter implements Formatter, IInitializeColumn {
        readonly props: {
            displayFormat?: string;
            originalNameProperty?: string;
            iconClass?: string;
        };
        constructor(props?: {
            displayFormat?: string;
            originalNameProperty?: string;
            iconClass?: string;
        });
        format(ctx: Slick.FormatterContext): string;
        static dbFileUrl(filename: string): string;
        initializeColumn(column: Slick.Column): void;
        get displayFormat(): string;
        set displayFormat(value: string);
        get originalNameProperty(): string;
        set originalNameProperty(value: string);
        get iconClass(): string;
        set iconClass(value: string);
    }
    class MinuteFormatter implements Formatter {
        format(ctx: Slick.FormatterContext): string;
        static format(value: number): string;
    }
    class NumberFormatter {
        readonly props: {
            displayFormat?: string;
        };
        constructor(props?: {
            displayFormat?: string;
        });
        format(ctx: Slick.FormatterContext): string;
        static format(value: any, format?: string): string;
        get displayFormat(): string;
        set displayFormat(value: string);
    }
    class UrlFormatter implements Formatter, IInitializeColumn {
        readonly props: {
            displayProperty?: string;
            displayFormat?: string;
            urlProperty?: string;
            urlFormat?: string;
            target?: string;
        };
        constructor(props?: {
            displayProperty?: string;
            displayFormat?: string;
            urlProperty?: string;
            urlFormat?: string;
            target?: string;
        });
        format(ctx: Slick.FormatterContext): string;
        initializeColumn(column: Slick.Column): void;
        get displayProperty(): string;
        set displayProperty(value: string);
        get displayFormat(): string;
        set displayFormat(value: string);
        get urlProperty(): string;
        set urlProperty(value: string);
        get urlFormat(): string;
        set urlFormat(value: string);
        get target(): string;
        set target(value: string);
    }

    namespace FormatterTypeRegistry {
        function get(key: string): any;
        function reset(): void;
        function tryGet(key: string): any;
    }

    interface SettingStorage {
        getItem(key: string): string | Promise<string>;
        setItem(key: string, value: string): void | Promise<void>;
    }
    interface PersistedGridColumn {
        id: string;
        width?: number;
        sort?: number;
        visible?: boolean;
    }
    interface PersistedGridSettings {
        columns?: PersistedGridColumn[];
        filterItems?: FilterLine[];
        quickFilters?: {
            [key: string]: any;
        };
        quickFilterText?: string;
        quickSearchField?: QuickSearchField;
        quickSearchText?: string;
        includeDeleted?: boolean;
    }
    interface GridPersistanceFlags {
        columnWidths?: boolean;
        columnVisibility?: boolean;
        sortColumns?: boolean;
        filterItems?: boolean;
        quickFilters?: boolean;
        quickFilterText?: boolean;
        quickSearch?: boolean;
        includeDeleted?: boolean;
    }
    class DataGrid<TItem, P = {}> extends Widget<P> implements IDataGrid, IReadOnly {
        private _isDisabled;
        private _layoutTimer;
        private _slickGridOnSort;
        private _slickGridOnClick;
        protected titleDiv: Fluent;
        protected toolbar: Toolbar;
        protected filterBar: FilterDisplayBar;
        protected quickFiltersDiv: Fluent;
        protected quickFiltersBar: QuickFilterBar;
        protected slickContainer: Fluent;
        protected allColumns: Slick.Column[];
        protected propertyItemsData: PropertyItemsData;
        protected initialSettings: PersistedGridSettings;
        protected restoringSettings: number;
        view: RemoteView<TItem>;
        slickGrid: Slick.Grid;
        openDialogsAsPanel: boolean;
        static defaultRowHeight: number;
        static defaultHeaderHeight: number;
        static defaultPersistanceStorage: SettingStorage;
        static defaultColumnWidthScale: number;
        static defaultColumnWidthDelta: number;
        constructor(props: WidgetProps<P>);
        protected propertyItemsReady(itemsData: PropertyItemsData): void;
        protected afterInit(): void;
        protected useAsync(): boolean;
        protected useLayoutTimer(): boolean;
        protected layout(): void;
        protected getInitialTitle(): string;
        protected createToolbarExtensions(): void;
        protected ensureQuickFilterBar(): QuickFilterBar;
        protected createQuickFilters(filters?: QuickFilter<Widget<any>, any>[]): void;
        protected getQuickFilters(): QuickFilter<Widget<any>, any>[];
        static propertyItemToQuickFilter(item: PropertyItem): any;
        protected findQuickFilter<TWidget>(type: {
            new (...args: any[]): TWidget;
        }, field: string): TWidget;
        protected tryFindQuickFilter<TWidget>(type: {
            new (...args: any[]): TWidget;
        }, field: string): TWidget;
        protected createIncludeDeletedButton(): void;
        protected getQuickSearchFields(): QuickSearchField[];
        protected createQuickSearchInput(): void;
        destroy(): void;
        protected getItemCssClass(item: TItem, index: number): string;
        protected getItemMetadata(item: TItem, index: number): any;
        protected postProcessColumns(columns: Slick.Column[]): Slick.Column[];
        protected getColumnWidthDelta(): number;
        protected getColumnWidthScale(): number;
        protected initialPopulate(): void;
        protected canFilterColumn(column: Slick.Column): boolean;
        protected initializeFilterBar(): void;
        protected createSlickGrid(): Slick.Grid;
        protected setInitialSortOrder(): void;
        itemAt(row: number): TItem;
        rowCount(): number;
        getItems(): TItem[];
        setItems(value: TItem[]): void;
        protected bindToSlickEvents(): void;
        protected getAddButtonCaption(): string;
        protected getButtons(): ToolButton[];
        protected editItem(entityOrId: any): void;
        protected editItemOfType(itemType: string, entityOrId: any): void;
        protected onClick(e: Event, row: number, cell: number): void;
        protected viewDataChanged(e: any, rows: TItem[]): void;
        protected bindToViewEvents(): void;
        protected onViewProcessData(response: ListResponse<TItem>): ListResponse<TItem>;
        protected onViewFilter(item: TItem): boolean;
        protected getIncludeColumns(include: {
            [key: string]: boolean;
        }): void;
        protected setCriteriaParameter(): void;
        protected setEquality(field: string, value: any): void;
        protected setIncludeColumnsParameter(): void;
        protected onViewSubmit(): boolean;
        protected markupReady(): void;
        protected createSlickContainer(): Fluent;
        protected createView(): RemoteView<TItem>;
        protected getDefaultSortBy(): any[];
        protected usePager(): boolean;
        protected enableFiltering(): boolean;
        protected populateWhenVisible(): boolean;
        protected createFilterBar(): void;
        protected getPagerOptions(): PagerOptions;
        protected createPager(): void;
        protected getViewOptions(): RemoteViewOptions;
        protected createToolbar(buttons: ToolButton[]): void;
        getTitle(): string;
        setTitle(value: string): void;
        protected getItemType(): string;
        protected itemLink(itemType?: string, idField?: string, text?: Format<TItem>, cssClass?: (ctx: Slick.FormatterContext) => string, encode?: boolean): Format<TItem>;
        protected getColumnsKey(): string;
        protected getPropertyItems(): PropertyItem[];
        protected getPropertyItemsData(): PropertyItemsData;
        protected getPropertyItemsDataAsync(): Promise<PropertyItemsData>;
        protected getColumns(): Slick.Column<TItem>[];
        protected wrapFormatterWithEditLink(column: Slick.Column, item: PropertyItem): void;
        protected propertyItemsToSlickColumns(propertyItems: PropertyItem[]): Slick.Column[];
        protected getSlickOptions(): Slick.GridOptions;
        protected populateLock(): void;
        protected populateUnlock(): void;
        protected getGridCanLoad(): boolean;
        refresh(): void;
        protected refreshIfNeeded(): void;
        protected internalRefresh(): void;
        setIsDisabled(value: boolean): void;
        private _readonly;
        get readOnly(): boolean;
        set readOnly(value: boolean);
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
        updateInterface(): void;
        protected getRowDefinition(): IRowDefinition;
        private _localTextDbPrefix;
        protected getLocalTextDbPrefix(): string;
        protected getLocalTextPrefix(): string;
        private _idProperty;
        protected getIdProperty(): string;
        protected getIsDeletedProperty(): string;
        private _isActiveProperty;
        protected getIsActiveProperty(): string;
        protected updateDisabledState(): void;
        protected resizeCanvas(): void;
        protected subDialogDataChange(): void;
        protected addFilterSeparator(): void;
        protected determineText(getKey: (prefix: string) => string): string;
        protected addQuickFilter<TWidget extends Widget<any>, P>(opt: QuickFilter<TWidget, P>): TWidget;
        protected addDateRangeFilter(field: string, title?: string): DateEditor;
        protected dateRangeQuickFilter(field: string, title?: string): QuickFilter<DateEditor<DateEditorOptions>, DateTimeEditorOptions>;
        protected addDateTimeRangeFilter(field: string, title?: string): DateTimeEditor<DateTimeEditorOptions>;
        protected dateTimeRangeQuickFilter(field: string, title?: string): QuickFilter<DateTimeEditor<DateTimeEditorOptions>, DateTimeEditorOptions>;
        protected addBooleanFilter(field: string, title?: string, yes?: string, no?: string): SelectEditor;
        protected booleanQuickFilter(field: string, title?: string, yes?: string, no?: string): QuickFilter<SelectEditor<SelectEditorOptions>, SelectEditorOptions>;
        protected invokeSubmitHandlers(): void;
        protected quickFilterChange(e: Event): void;
        protected getPersistanceStorage(): SettingStorage;
        protected getPersistanceKey(): string;
        protected gridPersistanceFlags(): GridPersistanceFlags;
        protected canShowColumn(column: Slick.Column): boolean;
        protected getPersistedSettings(): PersistedGridSettings | Promise<PersistedGridSettings>;
        protected restoreSettings(settings?: PersistedGridSettings, flags?: GridPersistanceFlags): void | Promise<void>;
        protected restoreSettingsFrom(settings: PersistedGridSettings, flags?: GridPersistanceFlags): void;
        protected persistSettings(flags?: GridPersistanceFlags): void | Promise<void>;
        protected getCurrentSettings(flags?: GridPersistanceFlags): PersistedGridSettings;
        getElement(): HTMLElement;
        getGrid(): Slick.Grid;
        getView(): RemoteView<TItem>;
        getFilterStore(): FilterStore;
    }

    class ColumnPickerDialog<P = {}> extends TemplatedDialog<P> {
        private ulVisible;
        private ulHidden;
        private colById;
        allColumns: Slick.Column[];
        visibleColumns: string[];
        defaultColumns: string[];
        done: () => void;
        protected renderContents(): Fluent<HTMLDivElement>;
        static createToolButton(grid: IDataGrid): ToolButton;
        protected getDialogButtons(): DialogButton[];
        protected getDialogOptions(): DialogOptions;
        private getTitle;
        private allowHide;
        private createLI;
        private updateListStates;
        protected setupColumns(): void;
        protected onDialogOpen(): void;
    }

    /**
     * A mixin that can be applied to a DataGrid for tree functionality
     */
    class TreeGridMixin<TItem> {
        private options;
        private dataGrid;
        constructor(options: TreeGridMixinOptions<TItem>);
        /**
         * Expands / collapses all rows in a grid automatically
         */
        toggleAll(): void;
        collapseAll(): void;
        expandAll(): void;
        /**
         * Reorders a set of items so that parents comes before their children.
         * This method is required for proper tree ordering, as it is not so easy to perform with SQL.
         * @param items list of items to be ordered
         * @param getId a delegate to get ID of a record (must return same ID with grid identity field)
         * @param getParentId a delegate to get parent ID of a record
         */
        static applyTreeOrdering<TItem>(items: TItem[], getId: (item: TItem) => any, getParentId: (item: TItem) => any): TItem[];
    }
    interface TreeGridMixinOptions<TItem> {
        grid: DataGrid<TItem, any>;
        getParentId: (item: TItem) => any;
        toggleField: string;
        initialCollapse?: () => boolean;
    }

    interface CheckTreeItem<TSource> {
        isSelected?: boolean;
        hideCheckBox?: boolean;
        isAllDescendantsSelected?: boolean;
        id?: string;
        text?: string;
        parentId?: string;
        children?: CheckTreeItem<TSource>[];
        source?: TSource;
    }
    class CheckTreeEditor<TItem extends CheckTreeItem<TItem>, P = {}> extends DataGrid<TItem, P> implements IGetEditValue, ISetEditValue, IReadOnly {
        static createDefaultElement(): HTMLDivElement;
        private byId;
        constructor(props: EditorProps<P>);
        protected getIdProperty(): string;
        protected getTreeItems(): TItem[];
        protected updateItems(): void;
        getEditValue(property: PropertyItem, target: any): void;
        setEditValue(source: any, property: PropertyItem): void;
        protected getButtons(): ToolButton[];
        protected itemSelectedChanged(item: TItem): void;
        protected getSelectAllText(): string;
        protected isThreeStateHierarchy(): boolean;
        protected createSlickGrid(): Slick.Grid;
        protected onViewFilter(item: TItem): boolean;
        protected getInitialCollapse(): boolean;
        protected onViewProcessData(response: ListResponse<TItem>): ListResponse<TItem>;
        protected onClick(e: Event, row: number, cell: number): void;
        protected updateSelectAll(): void;
        protected updateFlags(): void;
        protected getDescendantsSelected(item: TItem): boolean;
        protected setAllSubTreeSelected(item: TItem, selected: boolean): boolean;
        protected allItemsSelected(): boolean;
        protected allDescendantsSelected(item: TItem): boolean;
        protected getDelimited(): boolean;
        protected anyDescendantsSelected(item: TItem): boolean;
        protected getColumns(): Slick.Column[];
        protected getItemText(ctx: Slick.FormatterContext): string;
        protected getSlickOptions(): Slick.GridOptions;
        protected sortItems(): void;
        protected moveSelectedUp(): boolean;
        private _readOnly;
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
        private get_value;
        get value(): string[];
        private set_value;
        set value(v: string[]);
    }
    interface CheckLookupEditorOptions {
        lookupKey?: string;
        checkedOnTop?: boolean;
        showSelectAll?: boolean;
        hideSearch?: boolean;
        delimited?: boolean;
        cascadeFrom?: string;
        cascadeField?: string;
        cascadeValue?: any;
        filterField?: string;
        filterValue?: any;
    }
    class CheckLookupEditor<TItem extends CheckTreeItem<TItem> = any, P extends CheckLookupEditorOptions = CheckLookupEditorOptions> extends CheckTreeEditor<CheckTreeItem<TItem>, P> {
        private searchText;
        private enableUpdateItems;
        private lookupChangeUnbind;
        constructor(props: EditorProps<P>);
        destroy(): void;
        protected updateItems(): void;
        protected getLookupKey(): string;
        protected getButtons(): ToolButton[];
        protected createToolbarExtensions(): void;
        protected getSelectAllText(): string;
        protected cascadeItems(items: TItem[]): TItem[];
        protected filterItems(items: TItem[]): TItem[];
        protected getLookupItems(lookup: Lookup<TItem>): TItem[];
        protected getTreeItems(): CheckTreeItem<TItem>[];
        protected onViewFilter(item: CheckTreeItem<TItem>): boolean;
        protected moveSelectedUp(): boolean;
        protected get_cascadeFrom(): string;
        get cascadeFrom(): string;
        protected getCascadeFromValue(parent: Widget<any>): any;
        protected cascadeLink: CascadedWidgetLink<Widget<any>>;
        protected setCascadeFrom(value: string): void;
        protected set_cascadeFrom(value: string): void;
        set cascadeFrom(value: string);
        protected get_cascadeField(): string;
        get cascadeField(): string;
        protected set_cascadeField(value: string): void;
        set cascadeField(value: string);
        protected get_cascadeValue(): any;
        get cascadeValue(): any;
        protected set_cascadeValue(value: any): void;
        set cascadeValue(value: any);
        protected get_filterField(): string;
        get filterField(): string;
        protected set_filterField(value: string): void;
        set filterField(value: string);
        protected get_filterValue(): any;
        get filterValue(): any;
        protected set_filterValue(value: any): void;
        set filterValue(value: any);
    }

    class EntityGrid<TItem, P = {}> extends DataGrid<TItem, P> {
        constructor(props: WidgetProps<P>);
        destroy(): void;
        protected handleRoute(e: HandleRouteEvent): void;
        protected usePager(): boolean;
        protected createToolbarExtensions(): void;
        protected getInitialTitle(): string;
        protected getLocalTextPrefix(): string;
        private _entityType;
        protected getEntityType(): string;
        private _displayName;
        protected getDisplayName(): string;
        private _itemName;
        protected getItemName(): string;
        protected getAddButtonCaption(): string;
        protected getButtons(): ToolButton[];
        protected newRefreshButton(noText?: boolean): ToolButton;
        protected addButtonClick(): void;
        protected editItem(entityOrId: any): void;
        protected editItemOfType(itemType: string, entityOrId: any): void;
        private _service;
        protected getService(): string;
        protected getViewOptions(): RemoteViewOptions;
        protected getItemType(): string;
        protected routeDialog(itemType: string, dialog: Widget<any>): void;
        protected getInsertPermission(): string;
        protected hasInsertPermission(): boolean;
        protected transferDialogReadOnly(dialog: Widget<any>): void;
        protected initDialog(dialog: Widget<any>): void;
        protected initEntityDialog(itemType: string, dialog: Widget<any>): void;
        protected createEntityDialog(itemType: string, callback?: (dlg: Widget<any>) => void): Widget<any>;
        protected getDialogOptions(): any;
        protected getDialogOptionsFor(itemType: string): any;
        protected getDialogTypeFor(itemType: string): {
            new (...args: any[]): Widget<any>;
        };
        private _dialogType;
        protected getDialogType(): {
            new (...args: any[]): Widget<any>;
        };
    }

    class EntityDialog<TItem, P = {}> extends TemplatedDialog<P> implements IEditDialog, IReadOnly {
        protected entity: TItem;
        protected entityId: any;
        protected propertyItemsData: PropertyItemsData;
        protected propertyGrid: PropertyGrid;
        protected toolbar: Toolbar;
        protected saveAndCloseButton: Fluent;
        protected applyChangesButton: Fluent;
        protected deleteButton: Fluent;
        protected undeleteButton: Fluent;
        protected cloneButton: Fluent;
        protected editButton: Fluent;
        protected localizationGrid: PropertyGrid;
        protected localizationButton: Fluent;
        protected localizationPendingValue: any;
        protected localizationLastValue: any;
        static defaultLanguageList: () => string[][];
        constructor(props?: WidgetProps<P>);
        protected propertyItemsReady(itemsData: PropertyItemsData): void;
        protected afterInit(): void;
        protected useAsync(): boolean;
        destroy(): void;
        protected get_entity(): TItem;
        protected set_entity(entity: any): void;
        protected get_entityId(): any;
        protected set_entityId(value: any): void;
        protected getEntityNameFieldValue(): any;
        protected getEntityTitle(): string;
        protected updateTitle(): void;
        protected isCloneMode(): boolean;
        protected isEditMode(): boolean;
        protected isDeleted(): boolean;
        protected isNew(): boolean;
        protected isNewOrDeleted(): boolean;
        protected getDeleteOptions(callback: (response: DeleteResponse) => void): ServiceOptions<DeleteResponse>;
        protected deleteHandler(options: ServiceOptions<DeleteResponse>, callback: (response: DeleteResponse) => void): void;
        protected doDelete(callback: (response: DeleteResponse) => void): void;
        protected onDeleteSuccess(response: DeleteResponse): void;
        protected getRowDefinition(): IRowDefinition;
        private _entityType;
        protected getEntityType(): string;
        private _formKey;
        protected getFormKey(): string;
        private _localTextDbPrefix;
        protected getLocalTextDbPrefix(): string;
        protected getLocalTextPrefix(): string;
        private _entitySingular;
        protected getEntitySingular(): string;
        private _nameProperty;
        protected getNameProperty(): string;
        private _idProperty;
        protected getIdProperty(): string;
        private _isActiveProperty;
        protected getIsActiveProperty(): string;
        protected getIsDeletedProperty(): string;
        private _service;
        protected getService(): string;
        load(entityOrId: any, done: () => void, fail?: (ex: Exception) => void): void;
        loadNewAndOpenDialog(asPanel?: boolean): void;
        loadEntityAndOpenDialog(entity: TItem, asPanel?: boolean): void;
        protected loadResponse(data: any): void;
        protected loadEntity(entity: TItem): void;
        protected beforeLoadEntity(entity: TItem): void;
        protected afterLoadEntity(): void;
        loadByIdAndOpenDialog(entityId: any, asPanel?: boolean): void;
        protected onLoadingData(data: RetrieveResponse<TItem>): void;
        protected getLoadByIdOptions(id: any, callback: (response: RetrieveResponse<TItem>) => void): ServiceOptions<RetrieveResponse<TItem>>;
        protected getLoadByIdRequest(id: any): RetrieveRequest;
        protected reloadById(): void;
        loadById(id: any, callback?: (response: RetrieveResponse<TItem>) => void, fail?: () => void): void;
        protected loadByIdHandler(options: ServiceOptions<RetrieveResponse<TItem>>, callback: (response: RetrieveResponse<TItem>) => void, fail: () => void): void;
        protected initLocalizationGrid(): void;
        protected initLocalizationGridCommon(pgOptions: PropertyGridOptions): void;
        protected isLocalizationMode(): boolean;
        protected isLocalizationModeAndChanged(): boolean;
        protected localizationButtonClick(): void;
        protected getLanguages(): any[];
        private getLangs;
        protected loadLocalization(): void;
        protected setLocalizationGridCurrentValues(): void;
        protected getLocalizationGridValue(): any;
        protected getPendingLocalizations(): any;
        protected initPropertyGrid(): void;
        protected getPropertyItems(): PropertyItem[];
        protected getPropertyItemsData(): PropertyItemsData;
        protected getPropertyItemsDataAsync(): Promise<PropertyItemsData>;
        protected getPropertyGridOptions(): PropertyGridOptions;
        protected validateBeforeSave(): boolean;
        protected getSaveOptions(callback: (response: SaveResponse) => void): ServiceOptions<SaveResponse>;
        protected getSaveEntity(): TItem;
        protected getSaveRequest(): SaveRequest<TItem>;
        protected onSaveSuccess(response: SaveResponse): void;
        protected save_submitHandler(callback: (response: SaveResponse) => void): void;
        protected save(callback?: (response: SaveResponse) => void): void | boolean;
        protected saveHandler(options: ServiceOptions<SaveResponse>, callback: (response: SaveResponse) => void): void;
        protected initToolbar(): void;
        protected showSaveSuccessMessage(response: SaveResponse): void;
        protected getToolbarButtons(): ToolButton[];
        protected getCloningEntity(): TItem;
        protected updateInterface(): void;
        protected getUndeleteOptions(callback?: (response: UndeleteResponse) => void): ServiceOptions<UndeleteResponse>;
        protected undeleteHandler(options: ServiceOptions<UndeleteResponse>, callback: (response: UndeleteResponse) => void): void;
        protected undelete(callback?: (response: UndeleteResponse) => void): void;
        private _readonly;
        get readOnly(): boolean;
        set readOnly(value: boolean);
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
        protected getInsertPermission(): string;
        protected getUpdatePermission(): string;
        protected getDeletePermission(): string;
        protected hasDeletePermission(): boolean;
        protected hasInsertPermission(): boolean;
        protected hasUpdatePermission(): boolean;
        protected hasSavePermission(): boolean;
        protected editClicked: boolean;
        protected isViewMode(): boolean;
        protected useViewMode(): boolean;
        protected getTemplate(): string;
    }

    /**
     * ## Serenity Core Library
     *
     * This is the package containing core TypeScript classes and functions used in Serenity applications.
     *
     * It should be installed by default in your projects created from `Serene` or `StartSharp` template:
     *
     * ```json
     * {
     *   "dependencies": {
     *     // ...
     *     "@serenity-is/corelib": "latest"
     *   }
     * }
     * ```
     *
     * The version number for this package should be equal or as close as possible to Serenity NuGet package versions in your project file.
     *
     * > When using classic namespaces instead of the ESM modules, the types and functions in this module are directly available from the global `Serenity` and `Q` namespaces.
     * > e.g. `Serenity.EntityGrid`
     * @packageDocumentation
     */

    type Constructor<T> = new (...args: any[]) => T;
}


import Q = Serenity;

declare namespace Slick {
    namespace Data {
        /** @obsolete use the type exported from @serenity-is/sleekgrid */
        export import GroupItemMetadataProvider = Slick.GroupItemMetadataProvider;
    }
    export import AggregateFormatting = Serenity.AggregateFormatting;
    export import Aggregators = Serenity.Aggregators;
    export import CancellableViewCallback = Serenity.CancellableViewCallback;
    export import Formatter = Serenity.Formatter;
    export import GroupInfo = Serenity.GroupInfo;   
    export import PagerOptions = Serenity.PagerOptions;
    export import PagingInfo = Serenity.PagingInfo;
    export import PagingOptions = Serenity.PagingOptions;
    export import RemoteView = Serenity.RemoteView;
    export import RemoteViewAjaxCallback = Serenity.RemoteViewAjaxCallback;
    export import RemoteViewFilter = Serenity.RemoteViewFilter;
    export import RemoteViewOptions = Serenity.RemoteViewOptions;
    export import RemoteViewProcessCallback = Serenity.RemoteViewProcessCallback;
    export import SummaryOptions = Serenity.SummaryOptions;
}
