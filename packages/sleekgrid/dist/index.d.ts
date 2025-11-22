import { Computed, Signal } from '@serenity-is/domwise';

/***
 * A base class that all special / non-data rows (like Group and GroupTotals) derive from.
 */
export declare class NonDataRow {
	__nonDataRow: boolean;
}
export declare const preClickClassName = "slick-edit-preclick";
export type CellNavigationDirection = "up" | "down" | "left" | "right" | "next" | "prev" | "home" | "end";
export interface CellNavigation {
	navigateBottom(): void;
	navigateDown(): boolean;
	navigateLeft(): boolean;
	navigateNext(): boolean;
	navigatePageDown(): void;
	navigatePageUp(): void;
	navigatePrev(): boolean;
	navigateRight(): boolean;
	navigateRowEnd(): boolean;
	navigateRowStart(): boolean;
	navigateTop(): void;
	navigateToRow(row: number): boolean;
	navigateUp(): boolean;
	/**
	 * Navigate the active cell in the specified direction.
	 * @param dir Navigation direction.
	 * @return Whether navigation resulted in a change of active cell.
	 */
	navigate(dir: CellNavigationDirection): boolean;
}
export declare class CellRange {
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
export interface IEventData<TArgs = {}, TEvent = {}> {
	args: TArgs;
	defaultPrevented: boolean;
	preventDefault(): void;
	stopPropagation(): void;
	stopImmediatePropagation(): void;
	isDefaultPrevented(): boolean;
	isImmediatePropagationStopped(): boolean;
	isPropagationStopped(): boolean;
	getReturnValue(): any;
	getReturnValues(): any[];
	nativeEvent: TEvent | null | undefined;
}
export type MergeArgKeys = "grid" | "column" | "node" | "row" | "cell" | "item";
export type EventData<TArgs = {}, TEvent = {}> = IEventData<TArgs, TEvent> & TEvent & {
	[key in keyof TArgs & (MergeArgKeys)]: TArgs[key];
};
export type EventCallback<TArgs = {}, TEvent = {}> = (e: EventData<TArgs, TEvent>, args?: TArgs) => void;
/***
 * An event object for passing data to event handlers and letting them control propagation.
 * <p>This is pretty much identical to how W3C and jQuery implement events.</p>
 */
export declare class EventDataWrapper<TArgs, TEvent = {}> implements IEventData<TArgs, TEvent> {
	private _args;
	private _isPropagationStopped;
	private _isImmediatePropagationStopped;
	private _isDefaultPrevented;
	private _nativeEvent;
	private _returnValue;
	private _returnValues;
	constructor(event?: TEvent | null, args?: TArgs);
	get defaultPrevented(): boolean;
	preventDefault(): void;
	isDefaultPrevented(): any;
	/***
	 * Stops event from propagating up the DOM tree.
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
	get args(): TArgs;
	addReturnValue(value: any): void;
	getReturnValues(): any[];
	getReturnValue(): any;
	get nativeEvent(): TEvent | null | undefined;
}
/***
 * A simple publisher-subscriber implementation.
 */
export declare class EventEmitter<TArgs = any, TEvent = {}> {
	private _handlers;
	/***
	 * Adds an event handler to be called when the event is fired.
	 * <p>Event handler will receive two arguments - an <code>EventData</code> and the <code>data</code>
	 * object the event was fired with.<p>
	 * @param fn {Function} Event handler.
	 */
	subscribe(fn: EventCallback<TArgs, TEvent>): void;
	/***
	 * Removes an event handler added with <code>subscribe(fn)</code>.
	 * @param fn {Function} Event handler to be removed.
	 */
	unsubscribe(fn: EventCallback<TArgs, TEvent>): void;
	/***
	 * Fires an event notifying all subscribers.
	 * @param args {Object} Additional data object to be passed to all handlers.
	 * @param e {EventDataWrapper}
	 *      Optional.
	 *      An <code>EventData</code> object to be passed to all handlers.
	 *      For DOM events, an existing W3C/jQuery event object can be passed in.
	 * @param scope {Object}
	 *      Optional.
	 *      The scope ("this") within which the handler will be executed.
	 *      If not specified, the scope will be set to the <code>Event</code> instance.
	 */
	notify(args?: TArgs, e?: TEvent, scope?: object): EventData<TArgs, TEvent>;
	clear(): void;
}
export declare class EventSubscriber {
	private _handlers;
	subscribe<TArgs, TEvent>(event: EventEmitter<TArgs, TEvent>, handler: EventCallback<TArgs, TEvent>): this;
	unsubscribe<TArgs, TEvent>(event: EventEmitter<TArgs, TEvent>, handler: EventCallback<TArgs, TEvent>): this;
	unsubscribeAll(): EventSubscriber;
}
/** @deprecated */
export declare const keyCode: {
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
export interface Position {
	bottom?: number;
	height?: number;
	left?: number;
	right?: number;
	top?: number;
	visible?: boolean;
	width?: number;
}
export interface ValidationResult {
	valid: boolean;
	msg?: string;
}
export interface RowCell {
	row: number;
	cell: number;
}
export interface EditorHost {
	getActiveCell(): RowCell;
	navigateNext(): boolean;
	navigatePrev(): boolean;
	onCompositeEditorChange: EventEmitter<any>;
	getEditorFactory(): EditorFactory;
}
export interface CompositeEditorOptions {
	formValues: any;
}
export interface EditorOptions {
	grid: EditorHost;
	gridPosition?: Position;
	position?: Position;
	editorCellNavOnLRKeys?: boolean;
	column?: Column;
	columnMetaData?: ColumnMetadata<any>;
	compositeEditorOptions?: CompositeEditorOptions;
	container?: HTMLElement;
	item?: any;
	event?: EventData;
	commitChanges?: () => void;
	cancelChanges?: () => void;
}
export interface EditorFactory {
	getEditor(column: Column, row?: number): EditorClass;
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
export interface EditorClass {
	new (options: EditorOptions): Editor;
	suppressClearOnEdit?: boolean;
}
export interface Editor {
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
export interface EditController {
	commitCurrentEdit(): boolean;
	cancelCurrentEdit(): boolean;
}
/***
 * A locking helper to track the active edit controller and ensure that only a single controller
 * can be active at a time.  This prevents a whole class of state and validation synchronization
 * issues.  An edit controller (such as SleekGrid) can query if an active edit is in progress
 * and attempt a commit or cancel before proceeding.
 * @class EditorLock
 */
export declare class EditorLock {
	private activeEditController;
	/***
	 * Returns true if a specified edit controller is active (has the edit lock).
	 * If the parameter is not specified, returns true if any edit controller is active.
	 * @param editController {EditController}
	 * @return {Boolean}
	 */
	isActive(editController?: EditController): boolean;
	/***
	 * Sets the specified edit controller as the active edit controller (acquire edit lock).
	 * If another edit controller is already active, and exception will be thrown.
	 * @param editController {EditController} edit controller acquiring the lock
	 */
	activate(editController: EditController): void;
	/***
	 * Unsets the specified edit controller as the active edit controller (release edit lock).
	 * If the specified edit controller is not the active one, an exception will be thrown.
	 * @param editController {EditController} edit controller releasing the lock
	 */
	deactivate(editController: EditController): void;
	/***
	 * Attempts to commit the current edit by calling "commitCurrentEdit" method on the active edit
	 * controller and returns whether the commit attempt was successful (commit may fail due to validation
	 * errors, etc.).  Edit controller's "commitCurrentEdit" must return true if the commit has succeeded
	 * and false otherwise.  If no edit controller is active, returns true.
	 * @return {Boolean}
	 */
	commitCurrentEdit(): boolean;
	/***
	 * Attempts to cancel the current edit by calling "cancelCurrentEdit" method on the active edit
	 * controller and returns whether the edit was successfully cancelled.  If no edit controller is
	 * active, returns true.
	 * @return {Boolean}
	 */
	cancelCurrentEdit(): boolean;
}
/***
 * A global singleton editor lock.
 */
export declare const GlobalEditorLock: EditorLock;
export interface ArgsGrid {
	grid: ISleekGrid;
}
export interface ArgsColumn extends ArgsGrid {
	column: Column;
}
export interface ArgsDrag extends ArgsGrid {
	mode: string;
	row: number;
	cell: number;
	item: any;
	helper: HTMLElement;
}
export interface ArgsColumnNode extends ArgsColumn {
	node: HTMLElement;
}
export type ArgsSortCol = {
	sortCol: Column;
	sortAsc: boolean;
};
export interface ArgsSort extends ArgsGrid {
	multiColumnSort: boolean;
	sortAsc: boolean;
	sortCol: Column;
	sortCols: ArgsSortCol[];
}
export interface ArgsSelectedRowsChange extends ArgsGrid {
	rows: number[];
	changedSelectedRows: number[];
	changedUnselectedRows: number[];
	previousSelectedRows: number[];
	caller: any;
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
	editor: Editor;
	column: Column;
	cellNode: HTMLElement;
	validationResults: ValidationResult;
}
export type CellEvent = EventData<ArgsCell>;
export type CellKeyboardEvent = EventData<ArgsCell, KeyboardEvent>;
export type CellMouseEvent = EventData<ArgsCell, MouseEvent>;
export type HeaderColumnEvent = EventData<ArgsColumn, Event>;
export type HeaderMouseEvent = EventData<ArgsColumn, MouseEvent>;
export type HeaderRenderEvent = EventData<ArgsColumnNode, Event>;
export type FooterColumnEvent = HeaderColumnEvent;
export type FooterMouseEvent = HeaderMouseEvent;
export type FooterRenderEvent = HeaderRenderEvent;
export type GridEvent = EventData<ArgsGrid>;
export type GridDragEvent = EventData<ArgsDrag, UIEvent>;
export type GridMouseEvent = EventData<ArgsGrid, MouseEvent>;
export type GridSortEvent = EventData<ArgsSort>;
export interface GridPlugin {
	init(grid: ISleekGrid): void;
	pluginName?: string;
	destroy?: () => void;
}
/** @deprecated Use GridPlugin instead */
export interface IPlugin extends GridPlugin {
}
export interface GridPluginHost {
	getPluginByName(name: string): GridPlugin;
	registerPlugin(plugin: GridPlugin): void;
	unregisterPlugin(plugin: GridPlugin): void;
}
export interface GridSignals {
	readonly showColumnHeader: Signal<boolean>;
	readonly hideColumnHeader: Computed<boolean>;
	readonly showTopPanel: Signal<boolean>;
	readonly hideTopPanel: Computed<boolean>;
	readonly showHeaderRow: Signal<boolean>;
	readonly hideHeaderRow: Computed<boolean>;
	readonly showFooterRow: Signal<boolean>;
	readonly hideFooterRow: Computed<boolean>;
	readonly pinnedStartCols: Signal<number>;
	readonly pinnedEndCols: Signal<number>;
	readonly frozenTopRows: Signal<number>;
	readonly frozenBottomRows: Signal<number>;
}
export interface ViewportInfo {
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
export type BandKey = "start" | "main" | "end";
export type PaneKey = "top" | "body" | "bottom";
export interface GridBandRefs {
	key: BandKey;
	headerCols?: HTMLElement;
	headerRowCols?: HTMLElement;
	canvas: {
		top?: HTMLElement;
		body: HTMLElement;
		bottom?: HTMLElement;
	};
	footerRowCols?: HTMLElement;
	readonly cellOffset: number;
	canvasWidth: number;
}
export type GridLayoutRefs = {
	readonly start: GridBandRefs;
	readonly main: GridBandRefs;
	readonly end: GridBandRefs;
	topPanel?: HTMLElement;
	readonly pinnedStartCols: number;
	readonly pinnedStartLast: number;
	readonly pinnedEndCols: number;
	readonly pinnedEndFirst: number;
	readonly frozenTopRows: number;
	readonly frozenTopLast: number;
	readonly frozenBottomRows: number;
	readonly frozenBottomFirst: number;
	config: {
		pinnedStartCols?: number;
		pinnedEndCols?: number;
		pinnedLimit?: number | null;
		colCount?: number;
		frozenTopRows?: number;
		frozenBottomRows?: number;
		frozenLimit?: number | null;
		dataLength?: number;
	};
};
export interface LayoutHost extends Pick<ISleekGrid, "getAllColumns" | "getColumns" | "getOptions" | "getContainerNode" | "getDataLength" | "onAfterInit">, GridPluginHost {
	getSignals(): GridSignals;
	getViewportInfo(): ViewportInfo;
	removeNode(node: HTMLElement): void;
	readonly refs: GridLayoutRefs;
}
export interface LayoutEngine {
	layoutName: string;
	init(host: LayoutHost): void;
	destroy(): void;
	adjustFrozenRowsOption?(): void;
	afterSetOptions(args: GridOptions): void;
	/** this might be called before init, chicken egg situation */
	reorderViewColumns?(viewCols: Column[], refs: GridLayoutRefs): Column[];
	supportPinnedCols?: boolean;
	supportPinnedEnd?: boolean;
	supportFrozenRows?: boolean;
	supportFrozenBottom?: boolean;
}
/***
 * Information about a group of rows.
 */
export declare class Group<TEntity = any> extends NonDataRow {
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
	/** Returns a text representation of the group value. */
	formatValue: (ctx: FormatterContext<Group<TEntity>>) => FormatterResult;
	/***
	 * Compares two Group instances.
	 * @return {Boolean}
	 * @param group {Group} Group instance to compare to.
	 */
	equals(group: Group): boolean;
}
export interface IGroupTotals<TEntity = any> {
	__nonDataRow?: boolean;
	__groupTotals?: boolean;
	group?: Group<TEntity>;
	initialized?: boolean;
	sum?: Record<string, any>;
	avg?: Record<string, any>;
	min?: Record<string, any>;
	max?: Record<string, any>;
}
/***
 * Information about group totals.
 * An instance of GroupTotals will be created for each totals row and passed to the aggregators
 * so that they can store arbitrary data in it.  That data can later be accessed by group totals
 * formatters during the display.
 * @class GroupTotals
 * @extends NonDataRow
 */
export declare class GroupTotals<TEntity = any> extends NonDataRow implements IGroupTotals<TEntity> {
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
	sum?: Record<string, any>;
	/**
	 * Contains avg
	 */
	avg?: Record<string, any>;
	/**
	 * Contains min
	 */
	min?: Record<string, any>;
	/**
	 * Contains max
	 */
	max?: Record<string, any>;
}
/**
 * Configuration options for the SleekGrid component.
 *
 * @template TItem - The type of items in the grid.
 */
export interface GridOptions<TItem = any> {
	/**
	 * CSS class applied to newly added rows for custom styling. Default is `"new-row"`.
	 */
	addNewRowCssClass?: string;
	/**
	 * Defaults to `false`. If `true`, a horizontal scrollbar is always visible regardless of content width.
	 */
	alwaysAllowHorizontalScroll?: boolean;
	/**
	 * Defaults to `false`. If `true`, a vertical scrollbar is always visible, useful for fixed-height grids or menus.
	 */
	alwaysShowVerticalScroll?: boolean;
	/**
	 * Defaults to `100`. Delay in milliseconds before asynchronous loading of editors.
	 */
	asyncEditorLoadDelay?: number;
	/**
	 * Defaults to `false`. If `true`, editors are loaded asynchronously, reducing initial rendering load.
	 */
	asyncEditorLoading?: boolean;
	/**
	 * Defaults to `40`. Delay in milliseconds before cleaning up post-rendered elements.
	 */
	asyncPostCleanupDelay?: number;
	/**
	 * Defaults to `-1` which means immediate execution. Delay in milliseconds before starting asynchronous post-rendering.
	 */
	asyncPostRenderDelay?: number;
	/**
	 * Defaults to `true`. If `true`, automatically opens the cell editor when a cell gains focus.
	 */
	autoEdit?: boolean;
	/**
	 * Defaults to `false`. If `true`, automatically adjusts the grid's height to fit the entire content without scrolling.
	 */
	autoHeight?: boolean;
	/**
	 * CSS class applied to cells with a flashing effect. Default is `"flashing"`.
	 */
	cellFlashingCssClass?: string;
	/**
	 * Function to handle clearing a DOM node, used for custom cleanup logic. Default is `null`.
	 */
	emptyNode?: (node: Element) => void;
	/**
	 * Array of column definitions for the grid.
	 */
	columns?: Column<TItem>[];
	/**
	 * @deprecated Use showGroupingPanel option instead.
	 */
	createPreHeaderPanel?: boolean;
	/**
	 * Function to extract column values from data items, used for custom copy buffer operations. Default is `null`.
	 */
	dataItemColumnValueExtractor?: (item: TItem, column: Column<TItem>) => void;
	/**
	 * Defaults to `80`. Default width of columns in pixels.
	 */
	defaultColumnWidth?: number;
	/**
	 * Default formatting options for columns. Default is `defaultColumnFormat`.
	 */
	defaultFormat?: ColumnFormat<TItem>;
	/**
	 * Default formatter function for cells.
	 */
	defaultFormatter?: CompatFormatter<TItem>;
	/**
	 * Defaults to `false`. If `true`, cells can be edited inline.
	 */
	editable?: boolean;
	/**
	 * Function to handle edit commands, useful for implementing custom undo support. Default is `null`.
	 */
	editCommandHandler?: (item: TItem, column: Column<TItem>, command: EditCommand) => void;
	/**
	 * Defaults to `false`. If `true`, enables navigation between cells using left and right arrow keys within the editor.
	 */
	editorCellNavOnLRKeys?: boolean;
	/**
	 * Factory function for creating custom editors. Default is `null`.
	 */
	editorFactory?: EditorFactory;
	/**
	 * Global editor lock instance, used for managing concurrent editor access. Default is `GlobalEditorLock`.
	 */
	editorLock?: EditorLock;
	/**
	 * Defaults to `false`. If `true`, enables the ability to add new rows to the grid.
	 */
	enableAddRow?: boolean;
	/**
	 * Defaults to `false`. If `true`, enables asynchronous post-rendering.
	 */
	enableAsyncPostRender?: boolean;
	/**
	 * Defaults to `false`. If `true`, enables cleanup after asynchronous post-rendering.
	 */
	enableAsyncPostRenderCleanup?: boolean;
	/**
	 * Defaults to `true`. If `true`, enables cell navigation with arrow keys.
	 */
	enableCellNavigation?: boolean;
	/**
	 * Defaults to `false`. If `true`, allows selection of cell ranges.
	 */
	enableCellRangeSelection?: boolean;
	/**
	 * Defaults to `true`. If `true`, enables column reordering.
	 */
	enableColumnReorder?: boolean;
	/**
	 * Allow returning raw HTML strings from formatters and use `innerHTML` to render them. Defaults to `false` for tighter security.
	 * It is recommended to leave this as `false` for better security and to avoid XSS vulnerabilities. In that case, formatters should return plain text or DOM elements.
	 */
	enableHtmlRendering?: boolean;
	/**
	 * Defaults to `false`. If `true`, enables row reordering.
	 */
	enableRowReordering?: boolean;
	/**
	 * Defaults to `true`. If `true`, enables navigation between cells using the Tab key.
	 */
	enableTabKeyNavigation?: boolean;
	/**
	 * Defaults to `false`. If `true`, enables text selection within cells.
	 */
	enableTextSelectionOnCells?: boolean;
	/**
	 * Defaults to `false`. If `true`, requires explicit initialization of the grid.
	 */
	explicitInitialization?: boolean;
	/**
	 * Defaults to null which means the footer row height is calculated based on CSS rules.
	 */
	footerRowHeight?: number;
	/**
	 * Defaults to `false`. If `true`, forces columns to fit the grid width.
	 */
	forceFitColumns?: boolean;
	/**
	 * Defaults to `false`. If `true`, synchronizes scrolling between the grid and its container.
	 */
	forceSyncScrolling?: boolean;
	/**
	 * Defaults to `250`. Interval in milliseconds for synchronizing scrolling when `forceSyncScrolling` is enabled.
	 */
	forceSyncScrollInterval?: number;
	/**
	 * Factory function for creating custom formatters. Default is `null`.
	 */
	formatterFactory?: FormatterFactory;
	/**
	 * Defaults to `false`. If `true`, places frozen rows at the bottom edge of the grid.
	 */
	frozenBottom?: boolean | number;
	/**
	 * Defaults to `undefined`. If specified, freezes the given number of columns on the left edge of the grid.
	 * Prefer setting column.frozen = 'true' for individual columns as this is only for compatibility.
	 */
	frozenColumns?: number;
	/**
	 * Defaults to `undefined`. If specified, freezes the given number of rows at the top or bottom
	 * edge (if frozenBottom === true).
	 */
	frozenRows?: number;
	/**
	 * Defaults to `false`. If `true`, makes rows take the full width of the grid.
	 */
	fullWidthRows?: boolean;
	/**
	 * Defaults to `false`. If `true`, shows the grouping panel for grouping columns.
	 */
	groupingPanel?: boolean;
	/**
	 * Defaults to null, e.g. calculated based on CSS. Height of the grouping panel in pixels.
	 */
	groupingPanelHeight?: number;
	/**
	 * Function to format group totals for display in the grouping panel.
	 */
	groupTotalsFormat?: (ctx: FormatterContext<IGroupTotals<TItem>>) => FormatterResult;
	/**
	 * Function to format group totals for display in the grouping panel.
	 * @deprecated Use `groupTotalsFormat` with `FormatterContext<IGroupTotals>` signature instead.
	 */
	groupTotalsFormatter?: (totals?: IGroupTotals<TItem>, column?: Column<TItem>, grid?: ISleekGrid) => string;
	/**
	 * Defaults to null, e.g. calculated based on CSS. Height of the header row in pixels.
	 */
	headerRowHeight?: number;
	/**
	 * jQuery object for compatibility or custom integration purposes. Default is `undefined` unless jQuery is available in the global object (e.g. window).
	 */
	jQuery?: {
		ready: any;
		fn: any;
	};
	/**
	 * Defaults to `false`. If `true`, leaves space for new rows in the DOM visible buffer.
	 */
	leaveSpaceForNewRows?: boolean;
	/**
	 * Layout engine for custom grid layouts. Default is `BasicLayout`. Use FrozenLayout to enable frozen columns / rows.
	 */
	layoutEngine?: LayoutEngine | (() => LayoutEngine);
	/**
	 * Defaults to `3`. Minimum number of rows to keep in the buffer.
	 */
	minBuffer?: number;
	/**
	 * Defaults to `false`. If `true`, allows sorting by multiple columns simultaneously.
	 */
	multiColumnSort?: boolean;
	/**
	 * Defaults to `true`. If `true`, enables multiple cell selection.
	 */
	multiSelect?: boolean;
	/**
	 * @deprecated Use groupingPanelHeight option instead.
	 */
	preHeaderPanelHeight?: number;
	/**
	 * Defaults to `false`. If `true`, renders all cells (row columns) in the viewport, at the cost of higher memory usage and reduced performance.
	 */
	renderAllCells?: boolean;
	/**
	 * Defaults to `false`. If `true`, renders all rows in the viewport, at the cost of higher memory usage and reduced performance.
	 * When both renderAllCells and renderAllRows are true, all cells in the grid are rendered (e.g. virtualization is disabled),
	 * which can be very slow for large datasets, but may be desired to keep all rows and cells in the DOM for accessibility purposes,
	 * proper tabbing and screen reader support.
	 */
	renderAllRows?: boolean;
	/**
	 * Function to handle removing a DOM node, used for custom cleanup logic. Default is `null` or jQuery.remove if available.
	 */
	removeNode?: (node: Element) => void;
	/**
	 * Defaults to `30`. Height of rows in pixels.
	 */
	rowHeight?: number;
	/**
	 * Default is based on document element's (`<html/>`) `dir` property.. If `true`, enables right-to-left text direction.
	 */
	rtl?: boolean;
	/**
	 * Optional function for sanitizing HTML strings to avoid XSS attacks.
	 * Default is `DOMPurify.sanitize` if available globally, otherwise falls back to `basicDOMSanitizer`.
	 */
	sanitizer?: (dirtyHtml: string) => string;
	/**
	 * CSS class applied to selected cells. Default is `"selected"`.
	 */
	selectedCellCssClass?: string;
	/**
	 * Defaults to `true`. If `true`, shows cell selection indicators.
	 */
	showCellSelection?: boolean;
	/**
	 * Defaults to `true`. If `true`, displays the column header.
	 */
	showColumnHeader?: boolean;
	/**
	 * Defaults to `false`. If `true`, displays the footer row.
	 */
	showFooterRow?: boolean;
	/**
	 * Defaults to `false`. If `true`, displays the grouping panel.
	 */
	showGroupingPanel?: boolean;
	/**
	 * Defaults to `false`. If `true`, displays the header row.
	 */
	showHeaderRow?: boolean;
	/**
	 * @deprecated Use showGroupingPanel option instead.
	 */
	showPreHeaderPanel?: boolean;
	/**
	 * Defaults to `false`. If `true`, displays the post-header panel for additional controls or information.
	 */
	showTopPanel?: boolean;
	/**
	 * Defaults to `false`. If `true`, suppresses the activation of cells when they contain an editor and are clicked.
	 */
	suppressActiveCellChangeOnEdit?: boolean;
	/**
	 * Nonce value for CSP (Content Security Policy) when `useCssVars` is `false`. Applied to the dynamically
	 * created `<style>` element to allow inline CSS injection without violating CSP rules.
	 * If not provided, the grid will attempt to detect a nonce from a meta element with `csp-nonce` name or from existing `<style>` or `<script>` elements on the page.
	 */
	styleNonce?: string;
	/**
	 * Defaults to `false`. If `true`, synchronizes column resizing with cell resizing.
	 */
	syncColumnCellResize?: boolean;
	/**
	 * Defaults to null which means the top panel height is calculated based on CSS rules.
	 */
	topPanelHeight?: number;
	/**
	 * @deprecated This option has no effect.
	 */
	useLegacyUI?: boolean;
	/**
	 * Defaults to `true` which is equivalent to 100. If `true`, uses CSS variables for styling (for up to 100 cols).
	 * If set to a number, enables CSS variables only if column count is less than or equal to that number.
	 * This is dependent on the stylesheet which only supports up to 100 columns by default.
	 * But if you defined your own stylesheet with more columns, you can set this to a higher number.
	 *
	 * If set to `false`, uses a dynamic `<style>` element (with optional `styleNonce` for CSP) to inject CSS rules
	 * for positioning, avoiding inline styles entirely.
	 *
	 */
	useCssVars?: boolean | number;
	/**
	 * CSS class applied to the viewport container. Default is `undefined`.
	 */
	viewportClass?: string;
}
export declare const gridDefaults: GridOptions;
export interface SelectionModel extends GridPlugin {
	setSelectedRanges(ranges: CellRange[]): void;
	onSelectedRangesChanged: EventEmitter<CellRange[]>;
	refreshSelections?(): void;
}
export interface ViewRange {
	top?: number;
	bottom?: number;
	leftPx?: number;
	rightPx?: number;
}
export interface ISleekGrid<TItem = any> extends CellNavigation, EditorHost, GridPluginHost {
	readonly onActiveCellChanged: EventEmitter<ArgsCell>;
	readonly onActiveCellPositionChanged: EventEmitter<ArgsGrid>;
	readonly onAddNewRow: EventEmitter<ArgsAddNewRow>;
	readonly onAfterInit: EventEmitter<ArgsGrid>;
	readonly onBeforeCellEditorDestroy: EventEmitter<ArgsEditorDestroy>;
	readonly onBeforeDestroy: EventEmitter<ArgsGrid>;
	readonly onBeforeEditCell: EventEmitter<ArgsCellEdit>;
	readonly onBeforeFooterRowCellDestroy: EventEmitter<ArgsColumnNode>;
	readonly onBeforeHeaderCellDestroy: EventEmitter<ArgsColumnNode>;
	readonly onBeforeHeaderRowCellDestroy: EventEmitter<ArgsColumnNode>;
	readonly onCellChange: EventEmitter<ArgsCellChange>;
	readonly onCellCssStylesChanged: EventEmitter<ArgsCssStyle>;
	readonly onClick: EventEmitter<ArgsCell, MouseEvent>;
	readonly onColumnsReordered: EventEmitter<ArgsGrid>;
	readonly onColumnsResized: EventEmitter<ArgsGrid>;
	readonly onContextMenu: EventEmitter<ArgsGrid, UIEvent>;
	readonly onDblClick: EventEmitter<ArgsCell, MouseEvent>;
	readonly onDrag: EventEmitter<ArgsDrag, UIEvent>;
	readonly onDragEnd: EventEmitter<ArgsDrag, UIEvent>;
	readonly onDragInit: EventEmitter<ArgsDrag, UIEvent>;
	readonly onDragStart: EventEmitter<ArgsDrag, UIEvent>;
	readonly onFooterRowCellRendered: EventEmitter<ArgsColumnNode>;
	readonly onHeaderCellRendered: EventEmitter<ArgsColumnNode>;
	readonly onHeaderClick: EventEmitter<ArgsColumn, MouseEvent>;
	readonly onHeaderContextMenu: EventEmitter<ArgsColumn, MouseEvent>;
	readonly onHeaderMouseEnter: EventEmitter<ArgsColumn, MouseEvent>;
	readonly onHeaderMouseLeave: EventEmitter<ArgsColumn, MouseEvent>;
	readonly onHeaderRowCellRendered: EventEmitter<ArgsColumnNode>;
	readonly onKeyDown: EventEmitter<ArgsCell, KeyboardEvent>;
	readonly onMouseEnter: EventEmitter<ArgsGrid, MouseEvent>;
	readonly onMouseLeave: EventEmitter<ArgsGrid, MouseEvent>;
	readonly onScroll: EventEmitter<ArgsScroll>;
	readonly onSelectedRowsChanged: EventEmitter<ArgsSelectedRowsChange>;
	readonly onSort: EventEmitter<ArgsSort>;
	readonly onValidationError: EventEmitter<ArgsValidationError>;
	readonly onViewportChanged: EventEmitter<ArgsGrid>;
	init(): void;
	addCellCssStyles(key: string, hash: CellStylesHash): void;
	autosizeColumns(): void;
	canCellBeActive(row: number, cell: number): boolean;
	canCellBeSelected(row: number, cell: number): boolean;
	clearTextSelection(): void;
	columnsResized(invalidate?: boolean): void;
	commitCurrentEdit(): boolean;
	destroy(): void;
	editActiveCell(editor?: EditorClass): void;
	flashCell(row: number, cell: number, speed?: number): void;
	focus(): void;
	getAbsoluteColumnMinWidth(): number;
	getActiveCanvasNode(e?: {
		target: EventTarget;
	}): HTMLElement;
	getActiveCellNode(): HTMLElement;
	getActiveViewportNode(e?: {
		target: EventTarget;
	}): HTMLElement;
	/** Returns all columns in the grid, including hidden ones, the order might not match visible columns due to pinning, ordering etc. */
	getAllColumns(): Column<TItem>[];
	getCanvases(): any | HTMLElement[];
	getCanvasNode(row?: number, cell?: number): HTMLElement;
	getCellCssStyles(key: string): CellStylesHash;
	getCellEditor(): Editor;
	getCellFromEvent(e: any): {
		row: number;
		cell: number;
	};
	getCellFromNode(cellNode: Element): number;
	getCellFromPoint(x: number, y: number): {
		row: number;
		cell: number;
	};
	getCellNode(row: number, cell: number): HTMLElement;
	getCellNodeBox(row: number, cell: number): {
		top: number;
		right: number;
		bottom: number;
		left: number;
	};
	getColspan(row: number, cell: number): number;
	/** Gets a column by its ID. May also return hidden columns. */
	getColumnById(id: string): Column<TItem>;
	getColumnFromNode(cellNode: Element): Column<TItem>;
	/** Returns a column's index in the visible columns list by its column ID. If opt.inAll is true, it will return index in all columns. */
	getColumnIndex(id: string, opt?: {
		inAll?: boolean;
	}): number;
	/** Returns only the visible columns in order */
	getColumns(): Column<TItem>[];
	getContainerNode(): HTMLElement;
	getData(): any;
	getDataItem(row: number): TItem;
	getDataItemValueForColumn(item: TItem, columnDef: Column<TItem>): any;
	getDataLength(): number;
	getDisplayedScrollbarDimensions(): {
		width: number;
		height: number;
	};
	getEditController(): EditController;
	getEditorLock(): EditorLock;
	getFooterRow(): HTMLElement;
	getFooterRowColumn(columnIdOrIdx: string | number): HTMLElement;
	getFormatter(row: number, column: Column<TItem>): ColumnFormat<TItem>;
	getFormatterContext(row: number, cell: number): FormatterContext;
	getGridPosition(): Position;
	getGroupingPanel(): HTMLElement;
	getHeader(): HTMLElement;
	getHeaderColumn(columnIdOrIdx: string | number): HTMLElement;
	getHeaderRow(): HTMLElement;
	getHeaderRowColumn(columnIdOrIdx: string | number): HTMLElement;
	getLayoutInfo(): GridLayoutInfo;
	getOptions(): GridOptions<TItem>;
	getPreHeaderPanel(): HTMLElement;
	getRenderedRange(viewportTop?: number, viewportLeft?: number): ViewRange;
	getRowFromNode(rowNode: Element): number;
	getScrollBarDimensions(): {
		width: number;
		height: number;
	};
	getSelectedRows(): number[];
	getSelectionModel(): SelectionModel;
	getSortColumns(): ColumnSort[];
	getTopPanel(): HTMLElement;
	getTotalsFormatter(column: Column<TItem>): ColumnFormat<TItem>;
	getUID(): string;
	/** Gets the viewport range */
	getViewport(viewportTop?: number, viewportLeft?: number): ViewRange;
	getViewportNode(row?: number, cell?: number): HTMLElement;
	getVisibleRange(viewportTop?: number, viewportLeft?: number): ViewRange;
	gotoCell(row: number, cell: number, forceEdit?: boolean): void;
	invalidate(): void;
	invalidateAllRows(): void;
	/**
	 * Invalidates various elements after properties of columns have changed.
	 * Call this if you change columns properties that don't require a full setColumns call (e.g. width, name, visible etc.)
	 */
	invalidateColumns(): void;
	invalidateRow(row: number): void;
	invalidateRows(rows: number[]): void;
	removeCellCssStyles(key: string): void;
	render: () => void;
	/**
	 * Reorders columns based on their IDs and notifies onColumnsReordered by default.
	 * @param columnIds
	 * @param opt Whether to notify onColumnsReordered (default true). If setVisible is provided, it will also set visibility based on that.
	 * This function is used by column picker and other plugins to reorder columns and set visibility in one shot.
	 */
	reorderColumns(columnIds: string[], opt?: {
		notify?: boolean;
		setVisible?: string[];
	}): void;
	resetActiveCell(): void;
	resizeCanvas: () => void;
	scrollActiveCellIntoView(): void;
	scrollCellIntoView(row: number, cell: number, doPaging?: boolean): void;
	scrollColumnIntoView(cell: number): void;
	scrollRowIntoView(row: number, doPaging?: boolean): void;
	scrollRowToTop(row: number): void;
	setActiveCell(row: number, cell: number): void;
	setActiveRow(row: number, cell: number, suppressScrollIntoView?: boolean): void;
	setCellCssStyles(key: string, hash: CellStylesHash): void;
	setColumnHeaderVisibility(visible: boolean): void;
	setColumns(columns: Column<TItem>[]): void;
	/**
	 * Sets the visible columns based on their IDs and reorders them to provided order
	 * unless specified otherwise.
	 * @param columnIds The IDs of the columns to be made visible.
	 * @param opt Whether to reorder the visible columns based on the provided IDs (default true),
	 * and notify onColumnsReordered (default true).
	 */
	setVisibleColumns(columnIds: string[], opt?: {
		reorder?: boolean;
		notify?: boolean;
	}): void;
	setData(newData: any, scrollToTop?: boolean): void;
	setFooterRowVisibility(visible: boolean): void;
	setGroupingPanelVisibility(visible: boolean): void;
	setHeaderRowVisibility(visible: boolean): void;
	setOptions(args: GridOptions<TItem>, suppressRender?: boolean, suppressColumnSet?: boolean, suppressSetOverflow?: boolean): void;
	setPreHeaderPanelVisibility(visible: boolean): void;
	setSelectedRows(rows: number[]): void;
	setSelectionModel(model: SelectionModel): void;
	setSortColumn(columnId: string, ascending: boolean): void;
	setSortColumns(cols: ColumnSort[]): void;
	setTopPanelVisibility(visible: boolean): void;
	updateCell(row: number, cell: number): void;
	updateColumnHeader(columnId: string, title?: string | ColumnFormat<any>, toolTip?: string): void;
	updatePagingStatusFromView(pagingInfo: {
		pageSize: number;
		pageNum: number;
		totalPages: number;
	}): void;
	updateRow(row: number): void;
	updateRowCount(): void;
}
export type GridLayoutInfo = {
	frozenTopRows: number;
	frozenBottomRows: number;
	pinnedStartCols: number;
	pinnedEndCols: number;
	supportFrozenRows: boolean;
	supportFrozenBottom: boolean;
	supportPinnedCols: boolean;
	supportPinnedEnd: boolean;
};
/**
 * Context object for column formatters. It provides access to the
 * current cell value, row index, column index, etc.
 * Use grid.getFormatterContext() or the @see formatterContext helper to create a new instance.
 */
export interface FormatterContext<TItem = any> {
	/**
	 * Additional attributes to be added to the cell node.
	 */
	addAttrs?: {
		[key: string]: string;
	};
	/**
	 * Additional classes to be added to the cell node.
	 */
	addClass?: string;
	/**
	 * True if the formatter is allowed to return raw HTML that will be set using innerHTML.
	 * This is set from grid options and defaults to false which means the formatter
	 * should return plain text and the result will be set using textContent and
	 * the escape() method is a noop. If true, the formatter can return HTML strings but should
	 * take care to avoid script injection attacks by using ctx.escape() method.
	 */
	readonly enableHtmlRendering: boolean;
	/**
	 * When enableHtmlRendering is false (default), this simply returns the value as string.
	 * When enableHtmlRendering is true, returns html escaped value / ctx.value if called without
	 * arguments. Prefer this over ctx.value when returning HTML strings to avoid html injection
	 * attacks when enableHtmlRendering is true. You don't have to use this inside JSX
	 * style formatters as JSX automatically escapes values.
	 */
	escape(value?: any): string;
	/**
	 * The row index of the cell.
	 */
	row?: number;
	/**
	 * The column index of the cell.
	 */
	cell?: number;
	/**
	 * The column definition of the cell.
	 */
	column?: Column<TItem>;
	/**
	 * The grid instance.
	 */
	grid?: ISleekGrid;
	/**
	 * The item of the row.
	 */
	item?: TItem;
	/**
	 * Purpose of the call, e.g. "auto-width", "excel-export", "group-header", "header-filter", "pdf-export", "print".
	 */
	purpose?: "auto-width" | "excel-export" | "group-header" | "grand-totals" | "group-totals" | "header-filter" | "pdf-export" | "print";
	/**
	 * Sanitizer function to clean up dirty HTML.
	 */
	sanitizer: (dirtyHtml: string) => string;
	/**
	 * Tooltip text to be added to the cell node as title attribute.
	 */
	tooltip?: string;
	/** when returning a formatter result as HTML string, prefer ctx.escape() to avoid script injection attacks! */
	value?: any;
}
export type FormatterResult = (string | HTMLElement | SVGElement | MathMLElement | DocumentFragment);
export type ColumnFormat<TItem = any> = (ctx: FormatterContext<TItem>) => FormatterResult;
export interface CompatFormatterResult {
	addClasses?: string;
	text?: FormatterResult;
	toolTip?: string;
}
export type CompatFormatter<TItem = any> = (row: number, cell: number, value: any, column: Column<TItem>, item: TItem, grid?: ISleekGrid) => string | CompatFormatterResult;
export interface FormatterFactory<TItem = any> {
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
export declare function defaultColumnFormat(ctx: FormatterContext): any;
export declare function convertCompatFormatter(compatFormatter: CompatFormatter): ColumnFormat;
export declare function applyFormatterResultToCellNode(ctx: FormatterContext, fmtResult: FormatterResult, node: HTMLElement, opt?: {
	contentOnly?: boolean;
}): void;
export declare function formatterContext<TItem = any>(opt?: Partial<Exclude<FormatterContext<TItem>, "addAttrs" | "addClass" | "tooltip">>): FormatterContext<TItem>;
export interface Column<TItem = any> {
	asyncPostRender?: AsyncPostRender<TItem>;
	asyncPostRenderCleanup?: AsyncPostCleanup<TItem>;
	behavior?: any;
	cannotTriggerInsert?: boolean;
	cssClass?: string;
	defaultSortAsc?: boolean;
	editor?: EditorClass;
	editorFixedDecimalPlaces?: number;
	field?: string;
	frozen?: boolean | "start" | "end";
	focusable?: boolean;
	footerCssClass?: string;
	format?: ColumnFormat<TItem>;
	/** @deprecated, use @see format */
	formatter?: CompatFormatter<TItem>;
	groupTotalsFormat?: (ctx: FormatterContext<IGroupTotals<TItem>>) => FormatterResult;
	/** @deprecated, use @see groupTotalsFormat */
	groupTotalsFormatter?: (totals?: IGroupTotals<TItem>, column?: Column<TItem>, grid?: unknown) => string;
	headerCssClass?: string;
	id?: string;
	maxWidth?: any;
	minWidth?: number;
	name?: string;
	nameFormat?: (ctx: FormatterContext<TItem>) => FormatterResult;
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
export declare const columnDefaults: Partial<Column>;
export interface ColumnMetadata<TItem = any> {
	colspan: number | "*";
	cssClasses?: string;
	focusable?: boolean;
	editor?: EditorClass;
	format?: ColumnFormat<TItem>;
	/** @deprecated */
	formatter?: CompatFormatter<TItem>;
	selectable?: boolean;
}
export interface ColumnSort {
	columnId: string;
	sortAsc?: boolean;
}
export interface ItemMetadata<TItem = any> {
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
export declare function initColumnProps(columns: Column[], defaults: Partial<Column<any>>): void;
export declare function titleize(str: string): string;
export interface IDataView<TItem = any> {
	/** Gets the grand totals for all aggregated data. */
	getGrandTotals(): IGroupTotals;
	/** Gets the total number of rows in the view. */
	getLength(): number;
	/** Gets the item at the specified row index. */
	getItem(row: number): (TItem | Group<TItem> | IGroupTotals);
	/** Gets metadata for the item at the specified row index. */
	getItemMetadata?(row: number): ItemMetadata<TItem>;
	/** Event fired when the underlying data changes */
	readonly onDataChanged?: EventEmitter<{}>;
	/** Event fired when the row count changes */
	readonly onRowCountChanged?: EventEmitter<{
		previous: number;
		current: number;
	}>;
	/** Event fired when specific rows change */
	readonly onRowsChanged?: EventEmitter<{
		rows: number[];
	}>;
}
export declare function addClass(el: Element, cls: string): void;
export declare function escapeHtml(s: any): any;
export declare function basicDOMSanitizer(dirtyHtml: string): string;
export declare function disableSelection(target: HTMLElement): void;
export declare function removeClass(el: Element, cls: string): void;
export declare function parsePx(str: string): number;
export declare class BasicLayout implements LayoutEngine {
	protected host: LayoutHost;
	protected refs: GridLayoutRefs;
	init(host: LayoutHost): void;
	destroy(): void;
	afterSetOptions(): void;
	readonly layoutName = "BasicLayout";
}
export declare class FrozenLayout implements LayoutEngine {
	private host;
	private refs;
	init(host: LayoutHost): void;
	reorderViewColumns(viewCols: Column[], refs: GridLayoutRefs): Column[];
	afterSetOptions(arg: GridOptions): void;
	adjustFrozenRowsOption(): void;
	destroy(): void;
	readonly layoutName = "FrozenLayout";
	supportPinnedCols: true;
	supportFrozenRows: true;
}
export declare const Header: ({ band, refs, signals }: {
	band: BandKey;
	refs: GridLayoutRefs;
	signals: Pick<GridSignals, "hideColumnHeader" | "pinnedStartCols" | "pinnedEndCols">;
}) => import("@serenity-is/domwise").JSXElement;
export declare const HeaderRow: ({ band, refs, signals }: {
	band: BandKey;
	refs: GridLayoutRefs;
	signals: Pick<GridSignals, "hideHeaderRow" | "pinnedStartCols" | "pinnedEndCols">;
}) => import("@serenity-is/domwise").JSXElement;
export declare const TopPanel: ({ refs, signals }: {
	refs: GridLayoutRefs;
	signals: Pick<GridSignals, "hideTopPanel">;
}) => import("@serenity-is/domwise").JSXElement;
export declare const Viewport: ({ band, pane, refs, signals }: {
	band: BandKey;
	pane: PaneKey;
	refs: GridLayoutRefs;
	signals: Pick<GridSignals, "frozenTopRows" | "frozenBottomRows" | "pinnedStartCols" | "pinnedEndCols">;
}) => import("@serenity-is/domwise").JSXElement;
export declare const FooterRow: ({ band, refs, signals }: {
	band: BandKey;
	refs: GridLayoutRefs;
	signals: Pick<GridSignals, "hideFooterRow" | "pinnedStartCols" | "pinnedEndCols">;
}) => import("@serenity-is/domwise").JSXElement;
export declare class SleekGrid<TItem = any> implements ISleekGrid<TItem> {
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
	private _cssColRulesL;
	private _cssColRulesR;
	private _cssVarRules;
	private _columnSortHandler;
	private _currentEditor;
	private _data;
	private _draggableInstance;
	private _editController;
	private _emptyNode;
	private _headerColumnWidthDiff;
	private _hEditorLoader;
	private _hPostRender;
	private _hPostRenderCleanup;
	private _hRender;
	private _ignoreScrollUntil;
	private _allCols;
	private _allColsById;
	private _initialized;
	private _jQuery;
	private _jumpinessCoefficient;
	private _lastRenderTime;
	private _layout;
	private _ignorePinChangeUntil;
	private _numberOfPages;
	private _on;
	private _off;
	private _options;
	private _signals;
	private _signalsDisposers;
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
	private _refs;
	private _mapBands;
	private _forEachBand;
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
	private _trigger;
	private static _nextUid;
	private _uid;
	private _viewportInfo;
	private _vScrollDir;
	private _boundAncestorScroll;
	private _colResizeDisposer;
	private _container;
	private _focusSink1;
	private _focusSink2;
	private _groupingPanel;
	private _eventDisposer;
	readonly onActiveCellChanged: EventEmitter<ArgsCell, {}>;
	readonly onActiveCellPositionChanged: EventEmitter<ArgsGrid, {}>;
	readonly onAddNewRow: EventEmitter<ArgsAddNewRow, {}>;
	static readonly onAfterInit: EventEmitter<ArgsGrid, {}>;
	readonly onAfterInit: EventEmitter<ArgsGrid, {}>;
	readonly onBeforeCellEditorDestroy: EventEmitter<ArgsEditorDestroy, {}>;
	readonly onBeforeDestroy: EventEmitter<ArgsGrid, {}>;
	readonly onBeforeEditCell: EventEmitter<ArgsCellEdit, {}>;
	readonly onBeforeFooterRowCellDestroy: EventEmitter<ArgsColumnNode, {}>;
	readonly onBeforeHeaderCellDestroy: EventEmitter<ArgsColumnNode, {}>;
	readonly onBeforeHeaderRowCellDestroy: EventEmitter<ArgsColumnNode, {}>;
	readonly onCellChange: EventEmitter<ArgsCellChange, {}>;
	readonly onCellCssStylesChanged: EventEmitter<ArgsCssStyle, {}>;
	readonly onClick: EventEmitter<ArgsCell, MouseEvent>;
	readonly onColumnsReordered: EventEmitter<ArgsGrid, {}>;
	readonly onColumnsResized: EventEmitter<ArgsGrid, {}>;
	readonly onCompositeEditorChange: EventEmitter<ArgsGrid, {}>;
	readonly onContextMenu: EventEmitter<ArgsGrid, UIEvent>;
	readonly onDblClick: EventEmitter<ArgsCell, MouseEvent>;
	readonly onDrag: EventEmitter<ArgsDrag, UIEvent>;
	readonly onDragEnd: EventEmitter<ArgsDrag, UIEvent>;
	readonly onDragInit: EventEmitter<ArgsDrag, UIEvent>;
	readonly onDragStart: EventEmitter<ArgsDrag, UIEvent>;
	readonly onFooterRowCellRendered: EventEmitter<ArgsColumnNode, {}>;
	readonly onHeaderCellRendered: EventEmitter<ArgsColumnNode, {}>;
	readonly onHeaderClick: EventEmitter<ArgsColumn, MouseEvent>;
	readonly onHeaderContextMenu: EventEmitter<ArgsColumn, MouseEvent>;
	readonly onHeaderMouseEnter: EventEmitter<ArgsColumn, MouseEvent>;
	readonly onHeaderMouseLeave: EventEmitter<ArgsColumn, MouseEvent>;
	readonly onHeaderRowCellRendered: EventEmitter<ArgsColumnNode, {}>;
	readonly onKeyDown: EventEmitter<ArgsCell, KeyboardEvent>;
	readonly onMouseEnter: EventEmitter<ArgsGrid, MouseEvent>;
	readonly onMouseLeave: EventEmitter<ArgsGrid, MouseEvent>;
	readonly onScroll: EventEmitter<ArgsScroll, {}>;
	readonly onSelectedRowsChanged: EventEmitter<ArgsSelectedRowsChange, {}>;
	readonly onSort: EventEmitter<ArgsSort, {}>;
	readonly onValidationError: EventEmitter<ArgsValidationError, {}>;
	readonly onViewportChanged: EventEmitter<ArgsGrid, {}>;
	constructor(container: string | HTMLElement | ArrayLike<HTMLElement>, data: any, columns: Column<TItem>[], options: GridOptions<TItem>);
	private applyLegacyHeightOptions;
	private createGroupingPanel;
	private getSignals;
	init(): void;
	registerPlugin(plugin: GridPlugin): void;
	unregisterPlugin(plugin: GridPlugin): void;
	getPluginByName(name: string): GridPlugin;
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
	private getBandRefsForCell;
	getLayoutInfo(): GridLayoutInfo;
	getCanvasNode(row?: number, cell?: number): HTMLElement;
	getCanvases(): any | HTMLElement[];
	getActiveCanvasNode(e?: {
		target: EventTarget;
	}): HTMLElement;
	getViewportNode(row?: number, cell?: number): HTMLElement;
	private getViewportInfo;
	private getViewports;
	getActiveViewportNode(e?: {
		target: EventTarget;
	}): HTMLElement;
	private getAvailableWidth;
	private applyColumnWidths;
	private adjustPinnedColsLimit;
	private calcCanvasBandWidths;
	private updateBandCanvasWidths;
	private updateCanvasWidth;
	private bindAncestorScrollEvents;
	private unbindAncestorScrollEvents;
	updateColumnHeader(columnId: string, title?: string | ColumnFormat<any>, toolTip?: string): void;
	getHeader(): HTMLElement;
	getHeaderColumn(cell: number | string): HTMLElement;
	getGroupingPanel(): HTMLElement;
	getPreHeaderPanel(): HTMLElement;
	getHeaderRow(): HTMLElement;
	getHeaderRowColumn(cell: string | number): HTMLElement;
	getFooterRow(): HTMLElement;
	getFooterRowColumn(cell: string | number): HTMLElement;
	private createColumnFooters;
	private createColumnHeaders;
	private setupColumnSort;
	private static offset;
	private sortableColInstances;
	private hasPinnedCols;
	private scrollColumnsLeft;
	private scrollColumnsRight;
	private setupColumnReorder;
	private colResizing;
	private setupColumnResize;
	columnsResized(invalidate?: boolean): void;
	private setOverflow;
	private measureCellPaddingAndBorder;
	private removeCssRules;
	private createCssRules;
	destroy(): void;
	getEditorFactory(): EditorFactory;
	getEditorLock(): EditorLock;
	getEditController(): EditController;
	getColumnById(id: string): Column<TItem>;
	getColumnIndex(id: string, opt?: {
		inAll?: boolean;
	}): number;
	autosizeColumns(): void;
	private applyColumnHeaderWidths;
	setSortColumn(columnId: string, ascending: boolean): void;
	setSortColumns(cols: ColumnSort[]): void;
	getSortColumns(): ColumnSort[];
	private handleSelectedRangesChanged;
	getAllColumns(): Column<TItem>[];
	getColumns(): Column<TItem>[];
	private updateViewColLeftRight;
	private updateViewCols;
	/** Set the initial columns, also calls initColumnProps unless opt.initProps is false */
	private setAllCols;
	private handleFrozenColsOption;
	setColumns(columns: Column<TItem>[]): void;
	private internalSetVisibleColumns;
	reorderColumns(columnIds: string[], opt?: {
		notify?: boolean;
		setVisible?: string[];
	}): void;
	setVisibleColumns(columnIds: string[], opt?: {
		reorder?: boolean;
		notify?: boolean;
	}): void;
	invalidateColumns(): void;
	getOptions(): GridOptions<TItem>;
	setOptions(args: GridOptions<TItem>, suppressRender?: boolean, suppressColumnSet?: boolean, suppressSetOverflow?: boolean): void;
	private validateAndEnforceOptions;
	private setOptionDependentSignals;
	private viewOnRowCountChanged;
	private viewOnRowsChanged;
	private viewOnDataChanged;
	private bindToData;
	private unbindFromData;
	setData(newData: any, scrollToTop?: boolean): void;
	getData(): any;
	getDataLength(): number;
	private getDataLengthIncludingAddNew;
	getDataItem(row: number): TItem;
	getTopPanel(): HTMLElement;
	setTopPanelVisibility(visible: boolean): void;
	setColumnHeaderVisibility(visible: boolean): void;
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
	getTotalsFormatter(column: Column<TItem>): ColumnFormat<TItem>;
	private getEditor;
	getDataItemValueForColumn(item: TItem, columnDef: Column<TItem>): any;
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
	getScrollContainerX(): HTMLElement;
	getScrollContainerY(): HTMLElement;
	updateRowCount(): void;
	private setPaneHeights;
	private setVirtualHeight;
	getViewport(viewportTop?: number, viewportLeft?: number): ViewRange;
	getVisibleRange(viewportTop?: number, viewportLeft?: number): ViewRange;
	getRenderedRange(viewportTop?: number, viewportLeft?: number): ViewRange;
	private ensureCellNodesInRowsCache;
	private isFrozenRow;
	private cleanUpCells;
	private cleanUpAndRenderCells;
	private createRowCellRenderArgs;
	private renderRows;
	private startPostProcessing;
	private startPostProcessingCleanup;
	private invalidatePostProcessingResults;
	private updateRowPositions;
	private updateGrandTotals;
	render(): void;
	private handleHeaderFooterRowScroll;
	private handleMouseWheel;
	private handleScroll;
	private _handleScroll;
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
	private getTextSelection;
	private setTextSelection;
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
	private getColumnCount;
	private isRTL;
	private setTabbingDirection;
	navigate(dir: string): boolean;
	getCellNode(row: number, cell: number): HTMLElement;
	setActiveCell(row: number, cell: number): void;
	setActiveRow(row: number, cell: number, suppressScrollIntoView?: boolean): void;
	canCellBeActive(row: number, cell: number): boolean;
	canCellBeSelected(row: number, cell: number): boolean;
	gotoCell(row: number, cell: number, forceEdit?: boolean): void;
	commitCurrentEdit(): boolean;
	private cancelCurrentEdit;
	private rowsToRanges;
	getSelectedRows(): number[];
	setSelectedRows(rows: number[]): void;
}
export declare function PercentCompleteFormatter(ctx: FormatterContext): HTMLSpanElement | "-";
export declare function PercentCompleteBarFormatter(ctx: FormatterContext): FormatterResult;
export declare function YesNoFormatter(ctx: FormatterContext): FormatterResult;
export declare function CheckboxFormatter(ctx: FormatterContext): FormatterResult;
export declare function CheckmarkFormatter(ctx: FormatterContext): FormatterResult;
export declare namespace Formatters {
	function PercentComplete(_row: number, _cell: number, value: any): HTMLSpanElement | "-";
	function PercentCompleteBar(_row: number, _cell: number, value: any): FormatterResult;
	function YesNo(_row: number, _cell: number, value: any): FormatterResult;
	function Checkbox(_row: number, _cell: number, value: any): FormatterResult;
	function Checkmark(_row: number, _cell: number, value: any): FormatterResult;
}
declare abstract class BaseCellEdit {
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
export declare class TextCellEdit extends BaseCellEdit {
	_input: HTMLInputElement;
	init(): void;
}
export declare class IntegerCellEdit extends TextCellEdit {
	serializeValue(): number;
	validate(): ValidationResult;
}
export declare class FloatCellEdit extends TextCellEdit {
	static AllowEmptyValue: boolean;
	static DefaultDecimalPlaces: number;
	getDecimalPlaces(): number;
	loadValue(item: any): void;
	serializeValue(): any;
	validate(): ValidationResult;
}
export declare class DateCellEdit extends TextCellEdit {
	private _calendarOpen;
	init(): void;
	destroy(): void;
	show(): void;
	hide(): void;
	position(position: Position): void;
}
export declare class YesNoSelectCellEdit extends BaseCellEdit {
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
export declare class CheckboxCellEdit extends BaseCellEdit {
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
export declare class PercentCompleteCellEdit extends IntegerCellEdit {
	protected _picker: HTMLDivElement;
	init(): void;
	destroy(): void;
}
export declare class LongTextCellEdit extends BaseCellEdit {
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
export declare namespace Editors {
	const Text: typeof TextCellEdit;
	const Integer: typeof IntegerCellEdit;
	const Float: typeof FloatCellEdit;
	const Date: typeof DateCellEdit;
	const YesNoSelect: typeof YesNoSelectCellEdit;
	const Checkbox: typeof CheckboxCellEdit;
	const PercentComplete: typeof PercentCompleteCellEdit;
	const LongText: typeof LongTextCellEdit;
}
export interface GroupItemMetadataProviderOptions {
	enableExpandCollapse?: boolean;
	groupCellCssClass?: string;
	groupCssClass?: string;
	groupIndentation?: number;
	groupFocusable?: boolean;
	groupFormat?: ColumnFormat<Group>;
	/** @deprecated see groupFormat */
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
	totalsFormat?: ColumnFormat<IGroupTotals>;
	/** @deprecated use totalsFormat */
	totalsFormatter?: CompatFormatter<IGroupTotals>;
}
export declare class GroupItemMetadataProvider implements GridPlugin {
	protected grid: ISleekGrid;
	private options;
	constructor(opt?: GroupItemMetadataProviderOptions);
	static readonly defaults: GroupItemMetadataProviderOptions;
	static defaultGroupFormat(ctx: FormatterContext, opt?: GroupItemMetadataProviderOptions): FormatterResult;
	static defaultTotalsFormat(ctx: FormatterContext, grid?: ISleekGrid): FormatterResult;
	init(grid: ISleekGrid): void;
	readonly pluginName = "GroupItemMetadataProvider";
	destroy(): void;
	getOptions(): GroupItemMetadataProviderOptions;
	setOptions(value: GroupItemMetadataProviderOptions): void;
	handleGridClick: (e: CellMouseEvent) => void;
	handleGridKeyDown: (e: CellKeyboardEvent) => void;
	groupCellPosition: () => {
		cell: number;
		colspan: (number | "*");
	};
	getGroupRowMetadata: ((item: Group) => ItemMetadata);
	getTotalsRowMetadata: ((item: IGroupTotals) => ItemMetadata);
}
export interface AutoTooltipsOptions {
	enableForCells?: boolean;
	enableForHeaderCells?: boolean;
	maxToolTipLength?: number;
	replaceExisting?: boolean;
}
export declare class AutoTooltips implements GridPlugin {
	private grid;
	private options;
	constructor(options?: AutoTooltipsOptions);
	static readonly defaults: AutoTooltipsOptions;
	init(grid: ISleekGrid): void;
	destroy(): void;
	private handleMouseEnter;
	private handleHeaderMouseEnter;
	pluginName: string;
}
export interface RowMoveManagerOptions {
	cancelEditOnDrag?: boolean;
}
export interface ArgsMoveRows {
	rows: number[];
	insertBefore: number;
}
export declare class RowMoveManager implements GridPlugin {
	private grid;
	private options;
	private dragging;
	private handler;
	onBeforeMoveRows: EventEmitter<ArgsMoveRows, {}>;
	onMoveRows: EventEmitter<ArgsMoveRows, {}>;
	constructor(options?: RowMoveManagerOptions);
	static readonly defaults: RowMoveManagerOptions;
	init(grid: ISleekGrid): void;
	destroy(): void;
	private handleDragInit;
	private handleDragStart;
	private handleDrag;
	private handleDragEnd;
}
export interface RowSelectionModelOptions {
	selectActiveRow?: boolean;
}
export declare class RowSelectionModel implements GridPlugin, SelectionModel {
	private grid;
	private handler;
	private options;
	private ranges;
	onSelectedRangesChanged: EventEmitter<CellRange[], {}>;
	constructor(options?: RowSelectionModelOptions);
	static readonly defaults: RowSelectionModelOptions;
	init(grid: ISleekGrid): void;
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

export {
	SleekGrid as Grid,
};

export {};
