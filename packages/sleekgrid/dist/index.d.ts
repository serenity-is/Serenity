import { Computed, JSXElement, Signal } from '@serenity-is/domwise';

/**
 * Base class for special rows that do not represent regular data items.
 * Group headers and group totals derive from this to allow the grid and
 * data view to distinguish them from plain data rows via the marker property.
 */
export declare class NonDataRow {
	/**
	 * Marker flag used at runtime to identify non-data rows.
	 * Checked by the grid and `DataView` to skip data-specific handling.
	 */
	__nonDataRow: boolean;
}
/**
 * CSS class applied to a cell that received a mousedown immediately before
 * an editor is activated. Editors can check for this class to decide whether
 * to select text or preserve the click target.
 */
export declare const preClickClassName = "slick-edit-preclick";
/**
 * Discrete directions that the active cell can be moved programmatically.
 * Values map to arrow keys and special keys: `home`/`end` for row boundaries,
 * `next`/`prev` for tab-like sequential navigation, and `up`/`down`/`left`/`right`
 * for arrow-key navigation.
 */
export type CellNavigationDirection = "up" | "down" | "left" | "right" | "next" | "prev" | "home" | "end";
/**
 * Contract for keyboard / programmatic navigation of the active cell.
 * Implemented by the grid so that editors, plugins and external code can move
 * focus without coupling to internal navigation logic.
 */
export interface CellNavigation {
	/**
	 * Moves the active cell to the last row of the data set.
	 */
	navigateBottom(): void;
	/**
	 * Moves the active cell one row down.
	 * @returns `true` if the active cell changed, `false` if already at the bottom or blocked.
	 */
	navigateDown(): boolean;
	/**
	 * Moves the active cell one column to the left.
	 * @returns `true` if the active cell changed.
	 */
	navigateLeft(): boolean;
	/**
	 * Moves the active cell to the next focusable cell (row-major order, wrapping rows).
	 * @returns `true` if the active cell changed.
	 */
	navigateNext(): boolean;
	/**
	 * Scrolls one page down and moves the active cell accordingly.
	 */
	navigatePageDown(): void;
	/**
	 * Scrolls one page up and moves the active cell accordingly.
	 */
	navigatePageUp(): void;
	/**
	 * Moves the active cell to the previous focusable cell (reverse row-major order).
	 * @returns `true` if the active cell changed.
	 */
	navigatePrev(): boolean;
	/**
	 * Moves the active cell one column to the right.
	 * @returns `true` if the active cell changed.
	 */
	navigateRight(): boolean;
	/**
	 * Moves the active cell to the last cell of the current row.
	 * @returns `true` if the active cell changed.
	 */
	navigateRowEnd(): boolean;
	/**
	 * Moves the active cell to the first cell of the current row.
	 * @returns `true` if the active cell changed.
	 */
	navigateRowStart(): boolean;
	/**
	 * Moves the active cell to the first row of the data set.
	 */
	navigateTop(): void;
	/**
	 * Moves the active cell to the specified row, keeping the current column if possible.
	 * @param row - Zero-based row index to navigate to.
	 * @returns `true` if the active cell changed.
	 */
	navigateToRow(row: number): boolean;
	/**
	 * Moves the active cell one row up.
	 * @returns `true` if the active cell changed.
	 */
	navigateUp(): boolean;
	/**
	 * Navigate the active cell in the specified direction.
	 * @param dir - Navigation direction.
	 * @returns Whether navigation resulted in a change of the active cell.
	 */
	navigate(dir: CellNavigationDirection): boolean;
}
/**
 * Represents a rectangular range of cells in the grid.
 * Coordinates are inclusive and automatically normalized so that `from*` is always
 * the top-left corner and `to*` is the bottom-right corner.
 */
export declare class CellRange {
	/**
	 * Top-most row index of the range (inclusive).
	 */
	fromRow: number;
	/**
	 * Left-most cell/column index of the range (inclusive).
	 */
	fromCell: number;
	/**
	 * Bottom-most row index of the range (inclusive).
	 */
	toRow: number;
	/**
	 * Right-most cell/column index of the range (inclusive).
	 */
	toCell: number;
	/**
	 * Creates a new cell range. When `toRow` / `toCell` are omitted the range
	 * represents a single cell at `fromRow` / `fromCell`.
	 * @param fromRow - Starting row index.
	 * @param fromCell - Starting cell/column index.
	 * @param toRow - Ending row index; defaults to `fromRow`.
	 * @param toCell - Ending cell index; defaults to `fromCell`.
	 */
	constructor(fromRow: number, fromCell: number, toRow?: number, toCell?: number);
	/**
	 * Returns `true` when the range spans exactly one row.
	 * @returns Whether the range covers a single row.
	 */
	isSingleRow(): boolean;
	/**
	 * Returns `true` when the range covers exactly one cell.
	 * @returns Whether the range is a single cell.
	 */
	isSingleCell(): boolean;
	/**
	 * Tests whether the range contains the given cell.
	 * @param row - Row index to test.
	 * @param cell - Cell/column index to test.
	 * @returns `true` if the cell lies inside the range (inclusive bounds).
	 */
	contains(row: number, cell: number): boolean;
	/**
	 * Returns a human-readable representation, e.g. `"(2:3)"` for a single cell
	 * or `"(0:0 - 4:5)"` for a multi-cell range.
	 * @returns Readable string for debugging/logging.
	 */
	toString(): string;
}
/**
 * Core event object passed to every grid event handler. Mirrors W3C/jQuery event
 * semantics with propagation and default-prevent controls.
 * @template TArgs - Payload specific to the event.
 * @template TEvent - Native DOM event wrapped by this object.
 */
export interface IEventData<TArgs = {}, TEvent = {}> {
	/** Payload supplied by the event emitter (e.g. `{row, cell, grid}`). */
	args: TArgs;
	/** Whether {@link IEventData.preventDefault} has been called. */
	defaultPrevented: boolean;
	/**
	 * Prevents the default action associated with the event.
	 */
	preventDefault(): void;
	/**
	 * Stops the event from bubbling further, but remaining handlers on the same
	 * emitter still run. Also calls `stopPropagation` on the native event when present.
	 */
	stopPropagation(): void;
	/**
	 * Prevents remaining handlers from being executed. Also stops DOM propagation.
	 */
	stopImmediatePropagation(): void;
	/** Returns `true` if {@link IEventData.preventDefault} has been called or the native event is default-prevented. */
	isDefaultPrevented(): boolean;
	/** Returns `true` if {@link IEventData.stopImmediatePropagation} has been called. */
	isImmediatePropagationStopped(): boolean;
	/** Returns `true` if {@link IEventData.stopPropagation} has been called. */
	isPropagationStopped(): boolean;
	/** Returns the last non-`undefined` return value from the handlers that have run. */
	getReturnValue(): any;
	/** Returns all return values collected from handlers. */
	getReturnValues(): any[];
	/** The wrapped native DOM event, if any. */
	nativeEvent: TEvent | null | undefined;
}
/** Shorthand keys that are hoisted from `args` onto `EventData` for ergonomic access (`e.grid`, `e.row`, …). */
export type MergeArgKeys = "grid" | "column" | "node" | "row" | "cell" | "item";
/**
 * Union type representing the actual event object handlers receive. It merges
 * {@link IEventData} with the native event and hoisted arg keys so callers can
 * access `e.row`, `e.cell`, `e.grid`, etc. directly.
 */
export type EventData<TArgs = {}, TEvent = {}> = IEventData<TArgs, TEvent> & TEvent & {
	[key in keyof TArgs & (MergeArgKeys)]: TArgs[key];
};
/**
 * Handler signature for SleekGrid events.
 * @template TArgs - Event payload type.
 * @template TEvent - Wrapped native event type.
 * @param e - Event object with propagation controls and merged fields.
 * @param args - Optional duplicate of `e.args` for convenience.
 */
export type EventCallback<TArgs = {}, TEvent = {}> = (e: EventData<TArgs, TEvent>, args?: TArgs) => void;
/**
 * Wraps a native DOM event and a payload object, exposing propagation controls.
 * Property access for common DOM fields (e.g. `clientX`, `key`, `target`) and arg keys
 * (`grid`, `row`, `cell`) is dynamically proxied via getters installed by
 * `initializeEventDataProps()`.
 * @template TArgs - Event payload type.
 * @template TEvent - Wrapped native event type.
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
	isDefaultPrevented(): boolean;
	/**
	 * Stops event from propagating up the DOM tree and marks it as propagation-stopped.
	 */
	stopPropagation(): void;
	/**
	 * Returns whether {@link EventDataWrapper.stopPropagation} was called on this event object.
	 * @returns `true` if propagation was stopped.
	 */
	isPropagationStopped(): boolean;
	/**
	 * Prevents remaining handlers from being executed and stops DOM propagation.
	 */
	stopImmediatePropagation(): void;
	/**
	 * Returns whether {@link EventDataWrapper.stopImmediatePropagation} was called on this event object.
	 * @returns `true` if immediate propagation was stopped.
	 */
	isImmediatePropagationStopped(): boolean;
	get args(): TArgs;
	addReturnValue(value: any): void;
	getReturnValues(): any[];
	getReturnValue(): any;
	get nativeEvent(): TEvent | null | undefined;
}
/**
 * Lightweight publish–subscribe implementation used for all SleekGrid events.
 * @template TArgs - Payload type.
 * @template TEvent - Wrapped native event type.
 */
export declare class EventEmitter<TArgs = any, TEvent = {}> {
	private _handlers;
	/**
	 * Registers an event handler to be invoked when the event is fired.
	 * Handlers receive `(eventData, args)` and run in insertion order.
	 * @param fn - Handler to register.
	 */
	subscribe(fn: EventCallback<TArgs, TEvent>): void;
	/**
	 * Removes a previously registered handler.
	 * @param fn - Handler to remove; must be the exact function reference passed to {@link EventEmitter.subscribe}.
	 */
	unsubscribe(fn: EventCallback<TArgs, TEvent>): void;
	/**
	 * Fires the event, invoking all subscribers in order until propagation is stopped.
	 * @param args - Payload passed to handlers as `e.args`.
	 * @param e - Optional native DOM event to wrap.
	 * @param scope - `this` value for handlers; defaults to the emitter itself.
	 * @returns The {@link EventData} object created for this notification (carries return values and propagation flags).
	 */
	notify(args?: TArgs, e?: TEvent, scope?: object): EventData<TArgs, TEvent>;
	/**
	 * Removes all registered handlers.
	 */
	clear(): void;
}
/**
 * Aggregates subscriptions across multiple emitters and allows bulk unsubscribe.
 * Useful for plugins/components that subscribe to many grid events and need
 * a single `unsubscribeAll()` on destroy.
 */
export declare class EventSubscriber {
	private _handlers;
	/**
	 * Subscribes `handler` to `event` and tracks the pair for later bulk cleanup.
	 * @param event - Emitter to subscribe to.
	 * @param handler - Handler to register.
	 * @returns `this` for chaining.
	 */
	subscribe<TArgs, TEvent>(event: EventEmitter<TArgs, TEvent>, handler: EventCallback<TArgs, TEvent>): this;
	/**
	 * Unsubscribes a previously tracked handler.
	 * @param event - Emitter the handler was subscribed to.
	 * @param handler - Handler to remove.
	 * @returns `this` for chaining.
	 */
	unsubscribe<TArgs, TEvent>(event: EventEmitter<TArgs, TEvent>, handler: EventCallback<TArgs, TEvent>): this;
	/**
	 * Unsubscribes all tracked handlers.
	 * @returns `this` for chaining.
	 */
	unsubscribeAll(): EventSubscriber;
}
/**
 * Legacy key-code constants.
 * @deprecated Prefer `KeyboardEvent.key`/`KeyboardEvent.code` checks over numeric codes.
 */
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
/**
 * Pixel bounds for positioning an editor or other overlay relative to the grid viewport.
 */
export interface Position {
	/** Bottom offset in pixels. */
	bottom?: number;
	/** Height in pixels. */
	height?: number;
	/** Left offset in pixels. */
	left?: number;
	/** Right offset in pixels. */
	right?: number;
	/** Top offset in pixels. */
	top?: number;
	/** Whether the positioned element is currently visible. */
	visible?: boolean;
	/** Width in pixels. */
	width?: number;
}
/**
 * Result of validating an editor value before commit.
 */
export interface ValidationResult {
	/** Whether the value is valid. */
	valid: boolean;
	/** Human-readable error message when `valid` is `false`. */
	msg?: string;
}
/**
 * Row/cell coordinate pair that identifies a single cell.
 */
export interface RowCell {
	/** Zero-based row index. */
	row: number;
	/** Zero-based cell/column index. */
	cell: number;
}
/**
 * Minimal grid/host surface required by cell editors to navigate and resolve editors.
 */
export interface EditorHost {
	/** Returns the currently active cell, or `null` when no cell is active. */
	getActiveCell(): RowCell;
	/** Moves focus to the next focusable cell. @returns `true` if focus moved. */
	navigateNext(): boolean;
	/** Moves focus to the previous focusable cell. @returns `true` if focus moved. */
	navigatePrev(): boolean;
	/** Emits when a composite-editor field value changes; editors forward changes through this. */
	onCompositeEditorChange: EventEmitter<any>;
	/** Resolves the editor factory for a given column/row. */
	getEditorFactory(): EditorFactory;
}
/**
 * Options supplied to a composite editor that edits multiple fields at once.
 */
export interface CompositeEditorOptions {
	/** Current form values keyed by column id or field name. */
	formValues: any;
}
/**
 * Options passed to every editor instance on construction.
 */
export interface EditorOptions {
	/** Host grid instance the editor operates within. */
	grid: EditorHost;
	/** Bounds of the entire grid container in viewport coordinates. */
	gridPosition?: Position;
	/** Bounds of the target cell where the editor should be positioned. */
	position?: Position;
	/** Whether left/right arrow keys should navigate between cells while editing. */
	editorCellNavOnLRKeys?: boolean;
	/** Column definition for the cell being edited. */
	column?: Column;
	/** Metadata overrides for the column/row being edited. */
	columnMetaData?: ColumnMetadata<any>;
	/** Options from a parent composite editor, if any. */
	compositeEditorOptions?: CompositeEditorOptions;
	/** DOM container the editor should render into. */
	container?: HTMLElement;
	/** Data item (row) being edited. */
	item?: any;
	/** Event that triggered the edit, if available. */
	event?: EventData;
	/** Callback to commit pending editor changes. */
	commitChanges?: () => void;
	/** Callback to cancel pending editor changes. */
	cancelChanges?: () => void;
}
/**
 * Factory that resolves an editor class for a given column/row.
 */
export interface EditorFactory {
	/**
	 * Returns the editor class for the given column and optional row.
	 * @param column - Column to resolve an editor for.
	 * @param row - Optional row index for row-specific resolution.
	 * @returns Editor constructor.
	 */
	getEditor(column: Column, row?: number): EditorClass;
}
/**
 * Command produced by an editor commit; supports undo/redo when the grid's
 * `editCommandHandler` queues it.
 */
export interface EditCommand {
	/** Row index that was edited. */
	row: number;
	/** Cell/column index that was edited. */
	cell: number;
	/** Editor instance that produced the change. */
	editor: Editor;
	/** New serialized value from the editor. */
	serializedValue: any;
	/** Previous serialized value before the edit. */
	prevSerializedValue: any;
	/** Applies the edit. */
	execute: () => void;
	/** Reverts the edit. */
	undo: () => void;
}
/**
 * Constructor type for cell editors.
 */
export interface EditorClass {
	/** Instantiates the editor for the given options. */
	new (options: EditorOptions): Editor;
	/**
	 * When `true`, the editor is not cleared on first key press; the existing
	 * value is preserved and the key is forwarded to the editor.
	 */
	suppressClearOnEdit?: boolean;
}
/**
 * Contract that all cell editors must implement.
 */
export interface Editor {
	/** Tears down DOM and listeners created by the editor. */
	destroy(): void;
	/**
	 * Writes the edited value back to the data item.
	 * @param item - Row item to mutate.
	 * @param value - Serialized value from {@link Editor.serializeValue}.
	 */
	applyValue(item: any, value: any): void;
	/** Focuses the editor's input element. */
	focus(): void;
	/**
	 * Checks whether the current editor value differs from the original.
	 * @param args - Flags influencing the check (e.g. whether a commit is in progress).
	 * @returns `true` if the value has changed.
	 */
	isValueChanged(args: {
		commitEdit?: boolean;
	}): boolean;
	/** Key codes the editor captures even when the grid also handles them. */
	keyCaptureList?: number[];
	/**
	 * Loads an existing cell value into the editor.
	 * @param value - The value to load.
	 */
	loadValue(value: any): void;
	/** Serializes the current editor value for commit. */
	serializeValue(): any;
	/**
	 * Repositions the editor overlay.
	 * @param pos - New pixel bounds.
	 */
	position?(pos: Position): void;
	/** Hook invoked when the cell received a pre-click; editors may skip selection. */
	preClick?(): void;
	/** Hides the editor without destroying it. */
	hide?(): void;
	/** Shows a previously hidden editor. */
	show?(): void;
	/** Validates the current value. @returns Validation result. */
	validate?(): ValidationResult;
}
/**
 * Active edit controller implemented by the grid; managed by {@link EditorLock}.
 */
export interface EditController {
	/**
	 * Commits the current edit if any.
	 * @returns `true` if committed or no edit was active, `false` if validation failed.
	 */
	commitCurrentEdit(): boolean;
	/**
	 * Cancels the current edit if any.
	 * @returns `true` if cancelled or no edit was active.
	 */
	cancelCurrentEdit(): boolean;
}
/**
 * Locking helper that ensures only a single {@link EditController} is active at a time.
 * Prevents concurrent edits and validation races; the grid queries this before
 * navigating, sorting, or scrolling while an edit is open.
 */
export declare class EditorLock {
	private activeEditController;
	/**
	 * Checks whether an edit controller currently holds the edit lock.
	 * @param editController - Controller to test; when omitted, returns `true` if *any* controller is active.
	 * @returns Whether the given (or any) controller is active.
	 */
	isActive(editController?: EditController): boolean;
	/**
	 * Acquires the edit lock for the given controller.
	 * Throws if another controller already holds the lock or if the controller
	 * does not implement the required methods.
	 * @param editController - Controller acquiring the lock.
	 */
	activate(editController: EditController): void;
	/**
	 * Releases the edit lock held by the given controller.
	 * Throws if the controller is not the currently active one.
	 * @param editController - Controller releasing the lock.
	 */
	deactivate(editController: EditController): void;
	/**
	 * Attempts to commit the current edit via the active controller.
	 * @returns `true` if committed (or no edit was active), `false` if validation failed.
	 */
	commitCurrentEdit(): boolean;
	/**
	 * Attempts to cancel the current edit via the active controller.
	 * @returns `true` if cancelled (or no edit was active).
	 */
	cancelCurrentEdit(): boolean;
}
/**
 * Global singleton editor lock instance used by the grid by default.
 * Import this when you need a shared lock outside the grid.
 */
export declare const GlobalEditorLock: EditorLock;
/** Base payload for all grid events; carries the originating grid reference. */
export interface ArgsGrid {
	/** Grid instance that emitted the event. */
	grid: ISleekGrid;
}
/** Payload for events associated with a specific column. */
export interface ArgsColumn extends ArgsGrid {
	/** Column related to the event. */
	column: Column;
}
/** Payload for drag lifecycle events (drag init/start/drag/end). */
export interface ArgsDrag extends ArgsGrid {
	/** Drag mode/category (e.g. `"cell"`, `"column"`). */
	mode: string;
	/** Row index where the drag originated. */
	row: number;
	/** Cell/column index where the drag originated. */
	cell: number;
	/** Data item at `row`. */
	item: any;
	/** Visual helper element following the pointer during the drag. */
	helper: HTMLElement;
}
/** Payload for events that reference both a column and its DOM node. */
export interface ArgsColumnNode extends ArgsColumn {
	/** Header/footer cell node for the column. */
	node: HTMLElement;
}
/** Single entry within a multi-column sort description. */
export type ArgsSortCol = {
	/** Column being sorted. */
	sortCol: Column;
	/** Sort direction; `true` for ascending. */
	sortAsc: boolean;
};
/** Payload for `onSort` events. */
export interface ArgsSort extends ArgsGrid {
	/** Whether multiple columns are being sorted. */
	multiColumnSort: boolean;
	/** Primary sort direction (for single-column sort). */
	sortAsc: boolean;
	/** Primary sort column (for single-column sort). */
	sortCol: Column;
	/** All active sort columns for multi-sort. */
	sortCols: ArgsSortCol[];
}
/** Payload for `onSelectedRowsChanged`. */
export interface ArgsSelectedRowsChange extends ArgsGrid {
	/** Currently selected row indices after the change. */
	rows: number[];
	/** Rows that became selected in this change. */
	changedSelectedRows: number[];
	/** Rows that became unselected in this change. */
	changedUnselectedRows: number[];
	/** Selected rows before the change. */
	previousSelectedRows: number[];
	/** Caller token provided by the code that triggered the selection change. */
	caller: any;
}
/** Payload for `onScroll`. */
export interface ArgsScroll extends ArgsGrid {
	/** Horizontal scroll offset in pixels. */
	scrollLeft: number;
	/** Vertical scroll offset in pixels. */
	scrollTop: number;
}
/** Payload for `onCellCssStylesChanged` / cell-style setter events. */
export interface ArgsCssStyle extends ArgsGrid {
	/** Style key/bucket name. */
	key: string;
	/** Hash of `row -> columnId -> cssClass` describing the new styles. */
	hash: CellStylesHash;
}
/** Payload for events scoped to a specific cell. */
export interface ArgsCell extends ArgsGrid {
	/** Row index of the event cell. */
	row: number;
	/** Cell/column index of the event cell. */
	cell: number;
}
/** Payload for cell-change events where the row item is known. */
export interface ArgsCellChange extends ArgsCell {
	/** Data item at `row`. */
	item: any;
}
/** Payload for cell-edit lifecycle events. */
export interface ArgsCellEdit extends ArgsCellChange {
	/** Column definition for `cell`. */
	column: Column;
}
/** Payload for `onAddNewRow`. */
export interface ArgsAddNewRow extends ArgsColumn {
	/** Provisional new data item being added. */
	item: any;
}
/** Payload for `onBeforeCellEditorDestroy`. */
export interface ArgsEditorDestroy extends ArgsGrid {
	/** Editor instance about to be destroyed. */
	editor: Editor;
}
/** Payload for `onValidationError`. */
export interface ArgsValidationError extends ArgsCell {
	/** Editor that failed validation. */
	editor: Editor;
	/** Column being validated. */
	column: Column;
	/** DOM node of the failing cell. */
	cellNode: HTMLElement;
	/** Validation result containing `valid` flag and message. */
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
/**
 * Contract for grid plugins (e.g. selection models, overlays).
 */
export interface GridPlugin {
	/**
	 * Called by the grid when the plugin is registered.
	 * @param grid - Host grid instance the plugin attaches to.
	 */
	init(grid: ISleekGrid): void;
	/** Optional unique name used by {@link GridPluginHost.getPluginByName} for lookup. */
	pluginName?: string;
	/** Optional teardown hook; called when the grid or plugin is unregistered. */
	destroy?: () => void;
}
/**
 * Legacy alias for {@link GridPlugin}.
 * @deprecated Use {@link GridPlugin} instead.
 */
export interface IPlugin extends GridPlugin {
}
/**
 * Host surface implemented by the grid for managing {@link GridPlugin} lifetimes.
 */
export interface GridPluginHost {
	/**
	 * Retrieves a plugin by its {@link GridPlugin.pluginName}.
	 * @param name - Plugin name to look up.
	 * @returns The plugin instance, or `null`/`undefined` when not found.
	 */
	getPluginByName(name: string): GridPlugin;
	/**
	 * Registers a plugin and calls its {@link GridPlugin.init}.
	 * @param plugin - Plugin to register.
	 */
	registerPlugin(plugin: GridPlugin): void;
	/**
	 * Unregisters a plugin, calling {@link GridPlugin.destroy} if defined.
	 * @param plugin - Plugin to remove.
	 */
	unregisterPlugin(plugin: GridPlugin): void;
}
/**
 * Reactive signals surface for the grid's chrome and pinning state.
 * Backed by `@serenity-is/domwise` signals and used internally by the grid
 * and layout engine to drive visibility and pinning.
 */
export interface GridSignals {
	/** Whether the column header row is visible. */
	readonly showColumnHeader: Signal<boolean>;
	/** Inverse of {@link GridSignals.showColumnHeader}; `true` when the header is hidden. */
	readonly hideColumnHeader: Computed<boolean>;
	/** Whether the top panel row is visible. */
	readonly showTopPanel: Signal<boolean>;
	/** Inverse of {@link GridSignals.showTopPanel}. */
	readonly hideTopPanel: Computed<boolean>;
	/** Whether the header row (filter row) is visible. */
	readonly showHeaderRow: Signal<boolean>;
	/** Inverse of {@link GridSignals.showHeaderRow}. */
	readonly hideHeaderRow: Computed<boolean>;
	/** Whether the footer row is visible. */
	readonly showFooterRow: Signal<boolean>;
	/** Inverse of {@link GridSignals.showFooterRow}. */
	readonly hideFooterRow: Computed<boolean>;
	/** Number of columns pinned to the start (left in LTR, right in RTL) side. */
	readonly pinnedStartCols: Signal<number>;
	/** Number of columns pinned to the end (right in LTR, left in RTL) side. */
	readonly pinnedEndCols: Signal<number>;
	/** Number of rows frozen at the top of the viewport. */
	readonly frozenTopRows: Signal<number>;
	/** Number of rows frozen at the bottom of the viewport. */
	readonly frozenBottomRows: Signal<number>;
}
/**
 * Computed layout metrics for the grid viewport. Calculated during `computeViewportInfo()`
 * and used to size canvases, set scroll extents and decide virtualization bounds.
 */
export interface ViewportInfo {
	/** Height of the scrollable viewport in pixels. */
	height: number;
	/** Width of the scrollable viewport in pixels. */
	width: number;
	/** Whether a vertical scrollbar is currently present. */
	hasVScroll: boolean;
	/** Whether a horizontal scrollbar is currently present. */
	hasHScroll: boolean;
	/** Height of the column header row in pixels. */
	headerHeight: number;
	/** Height of the grouping panel in pixels. */
	groupingPanelHeight: number;
	/** Total virtual height of all rows (`rowHeight * rowCount`), before capping. */
	virtualHeight: number;
	/** Actual scrollable height applied to the canvas (capped for very large data sets). */
	realScrollHeight: number;
	/** Height of the top panel in pixels. */
	topPanelHeight: number;
	/** Height of the header-row (filter row) in pixels. */
	headerRowHeight: number;
	/** Height of the footer row in pixels. */
	footerRowHeight: number;
	/** Number of rows estimated to fit in the current viewport (`ceil(height/rowHeight)+1`). */
	numVisibleRows: number;
}
/** Logical horizontal band key. `start`/`end` are pinned side bands. */
export type BandKey = "start" | "main" | "end";
/** Logical vertical pane key within each band. */
export type PaneKey = "top" | "body" | "bottom";
/**
 * DOM and layout state for a single horizontal band (`start`/`main`/`end`).
 */
export interface GridBandRefs {
	/** Band identifier. */
	key: BandKey;
	/** Header column container for this band, if rendered. */
	headerCols?: HTMLElement;
	/** Header-row (filter row) column container, if rendered. */
	headerRowCols?: HTMLElement;
	/** Canvas elements per vertical pane. */
	canvas: {
		/** Top-frozen pane canvas, if enabled. */
		top?: HTMLElement;
		/** Main body viewport canvas. */
		body: HTMLElement;
		/** Bottom-frozen pane canvas, if enabled. */
		bottom?: HTMLElement;
	};
	/** Footer row column container, if rendered. */
	footerRowCols?: HTMLElement;
	/** Column index offset for cells inside this band (e.g. pinned count). */
	readonly cellOffset: number;
	/** Measured canvas width for this band in pixels. */
	canvasWidth: number;
}
/**
 * Aggregated refs for all bands and derived pinning/frozen indices.
 */
export type GridLayoutRefs = {
	/** Band refs for the pinned-start side. */
	readonly start: GridBandRefs;
	/** Band refs for the main (center, scrollable) band. */
	readonly main: GridBandRefs;
	/** Band refs for the pinned-end side. */
	readonly end: GridBandRefs;
	/** Top panel container element, if rendered. */
	topPanel?: HTMLElement;
	/** Number of columns pinned to the start (derived, bounded by `config`). */
	readonly pinnedStartCols: number;
	/** Last pinned-start column index or `-Infinity` when none. */
	readonly pinnedStartLast: number;
	/** Number of columns pinned to the end. */
	readonly pinnedEndCols: number;
	/** First pinned-end column index or `Infinity` when none. */
	readonly pinnedEndFirst: number;
	/** Number of top-frozen rows. */
	readonly frozenTopRows: number;
	/** Last top-frozen row index or `-Infinity` when none. */
	readonly frozenTopLast: number;
	/** Number of bottom-frozen rows. */
	readonly frozenBottomRows: number;
	/** First bottom-frozen row index or `Infinity` when none. */
	readonly frozenBottomFirst: number;
	/** Writable config inputs; setters trigger {@link createGridSignalsAndRefs} recalculation. */
	config: {
		/** Desired start-pinned column count. */
		pinnedStartCols?: number;
		/** Desired end-pinned column count. */
		pinnedEndCols?: number;
		/** Maximum total pinned columns, or `null` to allow all. */
		pinnedLimit?: number | null;
		/** Total column count driving index calculations. */
		colCount?: number;
		/** Desired top-frozen row count. */
		frozenTopRows?: number;
		/** Desired bottom-frozen row count. */
		frozenBottomRows?: number;
		/** Maximum total frozen rows, or `null` to allow all. */
		frozenLimit?: number | null;
		/** Total data row count driving frozen calculations. */
		dataLength?: number;
	};
};
/**
 * Minimal host surface exposed to {@link LayoutEngine} implementations.
 * Narrower than {@link ISleekGrid}; only what layouts need is exposed.
 */
export interface LayoutHost extends Pick<ISleekGrid, "getAllColumns" | "getColumns" | "getOptions" | "getContainerNode" | "getDataLength" | "onAfterInit">, GridPluginHost {
	/**
	 * Returns the shared reactive signals controlling visibility/pinning.
	 * @returns Grid signals object.
	 */
	getSignals(): GridSignals;
	/**
	 * Returns computed viewport metrics (dimensions, scroll flags, heights).
	 * @returns Current {@link ViewportInfo}.
	 */
	getViewportInfo(): ViewportInfo;
	/**
	 * Removes a DOM node via the grid's configured sanitizer/custom remover.
	 * @param node - Element to remove.
	 */
	removeNode(node: HTMLElement): void;
	/** Mutable refs tracking per-band DOM nodes and pinning/frozen state. */
	readonly refs: GridLayoutRefs;
}
/**
 * Pluggable layout strategy responsible for creating DOM panes and responding
 * to grid option changes. The grid instantiates one engine (typically
 * {@link BasicLayout} or {@link FrozenLayout}).
 */
export interface LayoutEngine {
	/** Human-readable layout name (e.g. `"BasicLayout"`). */
	layoutName: string;
	/**
	 * Initializes the layout, creating DOM inside `host.getContainerNode()`.
	 * @param host - Layout host providing grid state, signals and refs.
	 */
	init(host: LayoutHost): void;
	/** Tears down DOM and listeners created by {@link LayoutEngine.init}. */
	destroy(): void;
	/**
	 * Adjusts the frozen-row refs from the current grid options without a
	 * full re-layout. Called when `frozenRows` / `frozenBottom` change.
	 */
	adjustFrozenRowsOption?(): void;
	/**
	 * Called after `grid.setOptions(args)` merges new options.
	 * @param args - Options delta passed to `setOptions`.
	 */
	afterSetOptions(args: GridOptions): void;
	/**
	 * Optionally reorders the visible columns before they are laid out.
	 * May be called before {@link LayoutEngine.init} during early option setup.
	 * @param viewCols - Current visible columns in display order.
	 * @param refs - Mutable layout refs whose config may be updated.
	 * @returns Reordered columns, or `null` when no reorder is needed.
	 */
	reorderViewColumns?(viewCols: Column[], refs: GridLayoutRefs): Column[];
	/** Whether the engine supports pinned (frozen) columns. */
	supportPinnedCols?: boolean;
	/** Whether the engine supports end-pinned columns. */
	supportPinnedEnd?: boolean;
	/** Whether the engine supports top-frozen rows. */
	supportFrozenRows?: boolean;
	/** Whether the engine supports bottom-frozen rows. */
	supportFrozenBottom?: boolean;
}
/**
 * Represents a group of rows produced by a `DataView` grouping.
 * @template TEntity - Row item type being grouped.
 */
export declare class Group<TEntity = any> extends NonDataRow {
	/** Marker flag identifying this row as a group header. */
	readonly __group = true;
	/**
	 * Grouping level, starting with `0` for top-level groups.
	 */
	level: number;
	/**
	 * Number of leaf rows in the group (excluding group headers/totals).
	 */
	count: number;
	/**
	 * Grouping value that all rows in this group share (e.g. the field value).
	 */
	value: any;
	/**
	 * Whether the group is currently collapsed (children hidden).
	 */
	collapsed: boolean;
	/**
	 * Associated totals row for the group, if aggregation is enabled.
	 */
	totals: GroupTotals<TEntity>;
	/**
	 * Leaf rows that are part of the group.
	 */
	rows: TEntity[];
	/**
	 * Child groups when multiple grouping levels are active.
	 */
	groups: Group<TEntity>[];
	/**
	 * Unique key used to identify the group; pass to `DataView.collapseGroup()` / `expandGroup()`.
	 */
	groupingKey: string;
	/** Formatter that renders the group value as text. */
	formatValue: (ctx: FormatterContext<Group<TEntity>>) => FormatterResult;
	/**
	 * Compares two groups by `value`, `count` and `collapsed` state.
	 * @param group - Group instance to compare to.
	 * @returns `true` if the groups are equal by the above fields.
	 */
	equals(group: Group): boolean;
}
/**
 * Minimal totals information attached to a {@link Group}. Aggregators populate
 * `sum`/`avg`/`min`/`max` and arbitrary data on this object.
 * @template TEntity - Row item type.
 */
export interface IGroupTotals<TEntity = any> {
	/** Whether the row is a non-data row (inherited from {@link NonDataRow}). */
	__nonDataRow?: boolean;
	/** Marker identifying the row as a group-totals row. */
	__groupTotals?: boolean;
	/** Parent group this totals row belongs to. */
	group?: Group<TEntity>;
	/** Whether totals have been fully calculated; `false` for lazy totals. */
	initialized?: boolean;
	/** Per-field sum values. */
	sum?: Record<string, any>;
	/** Per-field average values. */
	avg?: Record<string, any>;
	/** Per-field minimum values. */
	min?: Record<string, any>;
	/** Per-field maximum values. */
	max?: Record<string, any>;
}
/**
 * Totales row for a {@link Group}. Created for each group and passed to aggregators
 * so they can store computed data that is later accessed by group-totals formatters.
 * @template TEntity - Row item type.
 */
export declare class GroupTotals<TEntity = any> extends NonDataRow implements IGroupTotals<TEntity> {
	/** Marker identifying this row as a group-totals row. */
	readonly __groupTotals = true;
	/**
	 * Parent group this totals row belongs to.
	 */
	group: Group<TEntity>;
	/**
	 * Whether the totals have been fully initialized/calculated.
	 * Set to `false` for lazy-calculated totals.
	 */
	initialized: boolean;
	/**
	 * Per-field sum values computed by aggregators.
	 */
	sum?: Record<string, any>;
	/**
	 * Per-field average values computed by aggregators.
	 */
	avg?: Record<string, any>;
	/**
	 * Per-field minimum values computed by aggregators.
	 */
	min?: Record<string, any>;
	/**
	 * Per-field maximum values computed by aggregators.
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
/**
 * Contract for a grid selection model (e.g. `CellSelectionModel`, `RowSelectionModel`).
 * Implements {@link GridPlugin} so it can be registered via `grid.setSelectionModel()`.
 */
export interface SelectionModel extends GridPlugin {
	/**
	 * Sets the current selection to the given cell ranges.
	 * @param ranges - New selected ranges; implementations should normalize/clamp them.
	 */
	setSelectedRanges(ranges: CellRange[]): void;
	/** Emits when the selected ranges change; payload is the new `CellRange[]`. */
	onSelectedRangesChanged: EventEmitter<CellRange[]>;
	/**
	 * Optional hook invoked when the grid re-renders rows; selection models can
	 * re-apply visual selection state here.
	 */
	refreshSelections?(): void;
}
/**
 * Describes the current view/buffer window that is (or should be) rendered.
 * Row bounds are view indices; column bounds are pixel offsets into the virtual canvas.
 */
export interface ViewRange {
	/** Top row index of the range (inclusive). */
	top?: number;
	/** Bottom row index of the range (exclusive or inclusive depending on caller; typically exclusive). */
	bottom?: number;
	/** Left pixel offset of the visible buffer window. */
	leftPx?: number;
	/** Right pixel offset of the visible buffer window. */
	rightPx?: number;
}
/**
 * Full grid surface exposed to plugins, editors and external code.
 * @template TItem - Row item type.
 */
export interface ISleekGrid<TItem = any> extends CellNavigation, EditorHost, GridPluginHost {
	/** Fired when the active cell changes. {@link ArgsCell} payload. */
	readonly onActiveCellChanged: EventEmitter<ArgsCell>;
	/** Fired when the active cell's pixel position changes (e.g. after scrolling). {@link ArgsGrid} payload. */
	readonly onActiveCellPositionChanged: EventEmitter<ArgsGrid>;
	/** Fired when a new row is about to be added via the add-new-row row. {@link ArgsAddNewRow} payload. */
	readonly onAddNewRow: EventEmitter<ArgsAddNewRow>;
	/** Fired once after `init()` completes. {@link ArgsGrid} payload. */
	readonly onAfterInit: EventEmitter<ArgsGrid>;
	/** Fired before a cell editor is destroyed; allows handlers to intercept. {@link ArgsEditorDestroy} payload. */
	readonly onBeforeCellEditorDestroy: EventEmitter<ArgsEditorDestroy>;
	/** Fired before the grid is destroyed. {@link ArgsGrid} payload. */
	readonly onBeforeDestroy: EventEmitter<ArgsGrid>;
	/** Fired before a cell enters edit mode; cancel with `e.preventDefault()`. {@link ArgsCellEdit} payload. */
	readonly onBeforeEditCell: EventEmitter<ArgsCellEdit>;
	/** Fired before a footer row cell is destroyed. {@link ArgsColumnNode} payload. */
	readonly onBeforeFooterRowCellDestroy: EventEmitter<ArgsColumnNode>;
	/** Fired before a header cell is destroyed. {@link ArgsColumnNode} payload. */
	readonly onBeforeHeaderCellDestroy: EventEmitter<ArgsColumnNode>;
	/** Fired before a header-row (filter) cell is destroyed. {@link ArgsColumnNode} payload. */
	readonly onBeforeHeaderRowCellDestroy: EventEmitter<ArgsColumnNode>;
	/** Fired after a cell value has changed and been committed. {@link ArgsCellChange} payload. */
	readonly onCellChange: EventEmitter<ArgsCellChange>;
	/** Fired when per-cell CSS styles change. {@link ArgsCssStyle} payload. */
	readonly onCellCssStylesChanged: EventEmitter<ArgsCssStyle>;
	/** Click on a body cell. {@link ArgsCell}, native `MouseEvent`. */
	readonly onClick: EventEmitter<ArgsCell, MouseEvent>;
	/** Fired after columns are reordered. {@link ArgsGrid} payload. */
	readonly onColumnsReordered: EventEmitter<ArgsGrid>;
	/** Fired after columns are resized. {@link ArgsGrid} payload. */
	readonly onColumnsResized: EventEmitter<ArgsGrid>;
	/** Context-menu event on the grid canvas. {@link ArgsGrid}, native `UIEvent`. */
	readonly onContextMenu: EventEmitter<ArgsGrid, UIEvent>;
	/** Double-click on a body cell. {@link ArgsCell}, native `MouseEvent`. */
	readonly onDblClick: EventEmitter<ArgsCell, MouseEvent>;
	/** Ongoing drag within the grid. {@link ArgsDrag}, native `UIEvent`. */
	readonly onDrag: EventEmitter<ArgsDrag, UIEvent>;
	/** Drag finished. {@link ArgsDrag}, native `UIEvent`. */
	readonly onDragEnd: EventEmitter<ArgsDrag, UIEvent>;
	/** Drag initialized (mousedown on a draggable surface). {@link ArgsDrag}, native `UIEvent`. */
	readonly onDragInit: EventEmitter<ArgsDrag, UIEvent>;
	/** Drag started (after minimum movement threshold). {@link ArgsDrag}, native `UIEvent`. */
	readonly onDragStart: EventEmitter<ArgsDrag, UIEvent>;
	/** Fired after a footer row cell is rendered. {@link ArgsColumnNode} payload. */
	readonly onFooterRowCellRendered: EventEmitter<ArgsColumnNode>;
	/** Fired after a header cell is rendered. {@link ArgsColumnNode} payload. */
	readonly onHeaderCellRendered: EventEmitter<ArgsColumnNode>;
	/** Click on a header cell. {@link ArgsColumn}, native `MouseEvent`. */
	readonly onHeaderClick: EventEmitter<ArgsColumn, MouseEvent>;
	/** Context menu on a header cell. {@link ArgsColumn}, native `MouseEvent`. */
	readonly onHeaderContextMenu: EventEmitter<ArgsColumn, MouseEvent>;
	/** Pointer entered a header cell. {@link ArgsColumn}, native `MouseEvent`. */
	readonly onHeaderMouseEnter: EventEmitter<ArgsColumn, MouseEvent>;
	/** Pointer left a header cell. {@link ArgsColumn}, native `MouseEvent`. */
	readonly onHeaderMouseLeave: EventEmitter<ArgsColumn, MouseEvent>;
	/** Fired after a header-row (filter) cell is rendered. {@link ArgsColumnNode} payload. */
	readonly onHeaderRowCellRendered: EventEmitter<ArgsColumnNode>;
	/** Key down while a body cell is active. {@link ArgsCell}, native `KeyboardEvent`. */
	readonly onKeyDown: EventEmitter<ArgsCell, KeyboardEvent>;
	/** Pointer entered the grid. {@link ArgsGrid}, native `MouseEvent`. */
	readonly onMouseEnter: EventEmitter<ArgsGrid, MouseEvent>;
	/** Pointer left the grid. {@link ArgsGrid}, native `MouseEvent`. */
	readonly onMouseLeave: EventEmitter<ArgsGrid, MouseEvent>;
	/** Grid scrolled. {@link ArgsScroll} payload. */
	readonly onScroll: EventEmitter<ArgsScroll>;
	/** Selected rows changed. {@link ArgsSelectedRowsChange} payload. */
	readonly onSelectedRowsChanged: EventEmitter<ArgsSelectedRowsChange>;
	/** Sorted columns changed. {@link ArgsSort} payload. */
	readonly onSort: EventEmitter<ArgsSort>;
	/** Editor validation failed. {@link ArgsValidationError} payload. */
	readonly onValidationError: EventEmitter<ArgsValidationError>;
	/** Visible viewport changed (scroll or resize). {@link ArgsGrid} payload. */
	readonly onViewportChanged: EventEmitter<ArgsGrid>;
	/**
	 * Initializes the grid DOM, binds events and performs the first render.
	 * Called automatically on construction unless `explicitInitialization` is set.
	 */
	init(): void;
	/**
	 * Adds per-cell CSS styles under the given key; multiple callers can coexist via different keys.
	 * @param key - Bucket name to group styles so callers can later remove only their own styles.
	 * @param hash - Map of `row -> columnId -> cssClass`.
	 */
	addCellCssStyles(key: string, hash: CellStylesHash): void;
	/**
	 * Auto-sizes columns to fit the container width, honouring `minWidth`/`maxWidth` and `forceFitColumns`.
	 */
	autosizeColumns(): void;
	/**
	 * Cancels the current cell edit without saving.
	 * @returns `true` if cancelled (or no edit was active).
	 */
	cancelCurrentEdit(): boolean;
	/**
	 * Checks whether a cell can become the active cell (focusable and selectable).
	 * @param row - Row index.
	 * @param cell - Cell/column index.
	 * @param tab - Whether the check is for tab navigation (affects `tabbable` handling).
	 * @returns `true` if the cell may become active.
	 */
	canCellBeActive(row: number, cell: number, tab?: boolean): boolean;
	/**
	 * Checks whether a cell may be selected.
	 * @param row - Row index.
	 * @param cell - Cell/column index.
	 * @returns `true` if the cell is selectable.
	 */
	canCellBeSelected(row: number, cell: number): boolean;
	/** Clears any browser text selection within the grid. */
	clearTextSelection(): void;
	/**
	 * Notifies the grid that column sizes have changed externally.
	 * @param invalidate - Whether to invalidate and re-render visible rows (default `true` behaviour).
	 */
	columnsResized(invalidate?: boolean): void;
	/**
	 * Commits the current edit, running validation.
	 * @param opt - Options; set `{ forceValueChange: true }` to force `onCellChange` even when the value did not appear to change.
	 * @returns `true` if committed (or no edit was active), `false` if validation failed.
	 */
	commitCurrentEdit(opt?: {
		forceValueChange?: boolean;
	}): boolean;
	/** Destroys the grid, removing DOM and event listeners. */
	destroy(): void;
	/**
	 * Activates the editor on the currently active cell.
	 * @param editor - Optional editor class override; defaults to the column's editor.
	 */
	editActiveCell(editor?: EditorClass): void;
	/**
	 * Flashes a cell briefly for visual feedback (e.g. successful update).
	 * @param row - Row index.
	 * @param cell - Cell/column index.
	 * @param speed - Flash duration in milliseconds; defaults to grid's configured speed.
	 */
	flashCell(row: number, cell: number, speed?: number): void;
	/**
	 * Focuses the grid's viewport so keyboard navigation works.
	 */
	focus(): void;
	/** Returns the minimum allowed column width, considering absolute minima. */
	getAbsoluteColumnMinWidth(): number;
	/**
	 * Gets the scrollable canvas node that is active for the given event target.
	 * @param e - Optional event target hint for viewport disambiguation.
	 */
	getActiveCanvasNode(e?: {
		target: EventTarget;
	}): HTMLElement;
	/** Gets the DOM node for the currently active cell, if any. */
	getActiveCellNode(): HTMLElement;
	/**
	 * Gets the viewport node that is active for the given event target.
	 * @param e - Optional event target hint for viewport disambiguation.
	 */
	getActiveViewportNode(e?: {
		target: EventTarget;
	}): HTMLElement;
	/** Returns all columns in the grid, including hidden ones; order may differ from visible columns due to pinning/reordering. */
	getAllColumns(): Column<TItem>[];
	/** Returns the grid's canvas elements (one per viewport when frozen rows/cols are used). */
	getCanvases(): any | HTMLElement[];
	/**
	 * Gets the canvas element for the given row/cell viewport.
	 * @param row - Optional row hint for viewport selection.
	 * @param cell - Optional cell hint for viewport selection.
	 */
	getCanvasNode(row?: number, cell?: number): HTMLElement;
	/**
	 * Gets the cell CSS styles hash associated with the given key.
	 * @param key - Style bucket name.
	 * @returns Hash of `row -> columnId -> cssClass`.
	 */
	getCellCssStyles(key: string): CellStylesHash;
	/** Returns the currently active editor instance, if any. */
	getCellEditor(): Editor;
	/**
	 * Resolves a row/cell coordinate from a mouse or keyboard event.
	 * @param e - Native event with target coordinates.
	 * @returns Row/cell indexes.
	 */
	getCellFromEvent(e: any): {
		row: number;
		cell: number;
	};
	/**
	 * Resolves the cell/column index from a cell DOM node.
	 * @param cellNode - Cell element.
	 */
	getCellFromNode(cellNode: Element): number;
	/**
	 * Resolves a row/cell from pixel coordinates relative to the canvas.
	 * @param x - Horizontal pixel offset.
	 * @param y - Vertical pixel offset.
	 * @returns Row/cell indexes.
	 */
	getCellFromPoint(x: number, y: number): {
		row: number;
		cell: number;
	};
	/**
	 * Gets the DOM node for a specific cell.
	 * @param row - Row index.
	 * @param cell - Cell/column index.
	 * @returns Cell element, or `null` when not rendered.
	 */
	getCellNode(row: number, cell: number): HTMLElement;
	/**
	 * Gets the bounding rectangle for a specific cell.
	 * @param row - Row index.
	 * @param cell - Cell/column index.
	 * @returns Box with `top`, `right`, `bottom`, `left` in pixels.
	 */
	getCellNodeBox(row: number, cell: number): {
		top: number;
		right: number;
		bottom: number;
		left: number;
	};
	/**
	 * Gets the column span for the cell at the given row/col, taking `colspan` metadata into account.
	 * @param row - Row index.
	 * @param cell - Cell/column index.
	 * @returns Number of columns spanned (at least `1`).
	 */
	getColspan(row: number, cell: number): number;
	/**
	 * Gets a column by its id; may return hidden columns.
	 * @param id - Column id.
	 * @returns Matching column definition or `undefined`.
	 */
	getColumnById(id: string): Column<TItem>;
	/**
	 * Resolves the column definition from a cell DOM node.
	 * @param cellNode - Cell element.
	 * @returns Corresponding column definition.
	 */
	getColumnFromNode(cellNode: Element): Column<TItem>;
	/**
	 * Gets the index of a column by its id.
	 * @param id - Column id.
	 * @param opt - When `opt.inAll` is `true`, searches all columns; otherwise only visible columns.
	 * @returns Column index or `-1` when not found.
	 */
	getColumnIndex(id: string, opt?: {
		inAll?: boolean;
	}): number;
	/** Returns only the visible columns in display order. */
	getColumns(): Column<TItem>[];
	/** Returns the root container element of the grid. */
	getContainerNode(): HTMLElement;
	/** Returns the data source / `DataView` attached to the grid. */
	getData(): any;
	/**
	 * Returns the data item at the given view row.
	 * @param row - View row index.
	 * @returns Data item for that row (or `Group`/`GroupTotals` for group rows).
	 */
	getDataItem(row: number): TItem;
	/**
	 * Extracts the raw cell value for a given column and item.
	 * @param item - Row data item.
	 * @param columnDef - Column definition.
	 * @returns Cell value.
	 */
	getDataItemValueForColumn(item: TItem, columnDef: Column<TItem>): any;
	/** Returns the number of rows in the grid's data source/view. */
	getDataLength(): number;
	/** Returns the currently displayed scrollbar dimensions (accounts for auto-hiding etc.). */
	getDisplayedScrollbarDimensions(): {
		width: number;
		height: number;
	};
	/** Returns the edit controller that manages the active editor lock. */
	getEditController(): EditController;
	/** Returns the `EditorLock` instance controlling concurrent edits. */
	getEditorLock(): EditorLock;
	/** Returns the footer row container element. */
	getFooterRow(): HTMLElement;
	/**
	 * Returns the footer row cell element for the given column.
	 * @param columnIdOrIdx - Column id or visible index.
	 */
	getFooterRowColumn(columnIdOrIdx: string | number): HTMLElement;
	/**
	 * Resolves the formatter to use for a body cell, considering column, row metadata and factory.
	 * @param row - Row index.
	 * @param column - Column definition.
	 * @returns Formatter function for that cell.
	 */
	getFormatter(row: number, column: Column<TItem>): ColumnFormat<TItem>;
	/**
	 * Creates a formatter context for a body cell.
	 * @param row - Row index.
	 * @param cell - Cell/column index.
	 * @returns Populated {@link FormatterContext}.
	 */
	getFormatterContext(row: number, cell: number): FormatterContext;
	/** Returns the grid container's bounding position (as used for editor placement). */
	getGridPosition(): Position;
	/** Returns the grouping panel container, if enabled. */
	getGroupingPanel(): HTMLElement;
	/** Returns the header row container element. */
	getHeader(): HTMLElement;
	/**
	 * Returns the header cell element for the given column.
	 * @param columnIdOrIdx - Column id or visible index.
	 */
	getHeaderColumn(columnIdOrIdx: string | number): HTMLElement;
	/** Returns the header-row (filter row) container element. */
	getHeaderRow(): HTMLElement;
	/**
	 * Returns the header-row cell element for the given column.
	 * @param columnIdOrIdx - Column id or visible index.
	 */
	getHeaderRowColumn(columnIdOrIdx: string | number): HTMLElement;
	/** Returns summarized layout/pinning information for the current grid configuration. */
	getLayoutInfo(): GridLayoutInfo;
	/** Returns the current grid options. */
	getOptions(): GridOptions<TItem>;
	/** Returns the pre-header panel element (grouping panel alternative). */
	getPreHeaderPanel(): HTMLElement;
	/**
	 * Returns the currently rendered view range as managed by the render loop.
	 * @param viewportTop - Optional scroll top override.
	 * @param viewportLeft - Optional scroll left override.
	 */
	getRenderedRange(viewportTop?: number, viewportLeft?: number): ViewRange;
	/**
	 * Resolves the view row index from a row DOM node.
	 * @param rowNode - Row element.
	 */
	getRowFromNode(rowNode: Element): number;
	/** Returns the native scrollbar width/height for the grid, measured from the layout. */
	getScrollBarDimensions(): {
		width: number;
		height: number;
	};
	/** Returns the currently selected row indices. */
	getSelectedRows(): number[];
	/** Returns the active selection model plugin, if any. */
	getSelectionModel(): SelectionModel;
	/** Returns the active sort column descriptors. */
	getSortColumns(): ColumnSort[];
	/** Returns the top panel container element. */
	getTopPanel(): HTMLElement;
	/**
	 * Resolves the group-totals formatter for a column.
	 * @param column - Column to resolve a totals formatter for.
	 * @returns Formatter for that column's totals row.
	 */
	getTotalsFormatter(column: Column<TItem>): ColumnFormat<TItem>;
	/** Returns the unique identifier assigned to this grid instance. */
	getUID(): string;
	/**
	 * Gets the viewport range for the active viewports.
	 * @param viewportTop - Optional scroll top override.
	 * @param viewportLeft - Optional scroll left override.
	 */
	getViewport(viewportTop?: number, viewportLeft?: number): ViewRange;
	/**
	 * Gets the viewport container node for the given row/cell.
	 * @param row - Optional row hint for viewport selection.
	 * @param cell - Optional cell hint for viewport selection.
	 */
	getViewportNode(row?: number, cell?: number): HTMLElement;
	/**
	 * Gets the visible (fully within viewport) row/cell range.
	 * @param viewportTop - Optional scroll top override.
	 * @param viewportLeft - Optional scroll left override.
	 */
	getVisibleRange(viewportTop?: number, viewportLeft?: number): ViewRange;
	/**
	 * Scrolls to and optionally edits the given cell.
	 * @param row - Row index to go to.
	 * @param cell - Cell/column index to go to.
	 * @param forceEdit - Whether to immediately enter edit mode.
	 */
	gotoCell(row: number, cell: number, forceEdit?: boolean): void;
	/** Invalidates the entire grid, requiring a full re-render on the next frame. */
	invalidate(): void;
	/** Invalidates all rows, forcing them to be re-rendered. */
	invalidateAllRows(): void;
	/**
	 * Invalidates header/column chrome after column properties change without a full `setColumns()` call
	 * (e.g. width, name, `visible` etc.). Forces header/footer re-rendering.
	 */
	invalidateColumns(): void;
	/**
	 * Invalidates a single row so it is re-rendered on the next frame.
	 * @param row - View row index to invalidate.
	 */
	invalidateRow(row: number): void;
	/**
	 * Invalidates multiple rows so they are re-rendered on the next frame.
	 * @param rows - View row indices to invalidate.
	 */
	invalidateRows(rows: number[]): void;
	/**
	 * Removes all cell CSS styles associated with the given key.
	 * @param key - Style bucket name.
	 */
	removeCellCssStyles(key: string): void;
	/**
	 * Immediately renders the grid (row/cell DOM), synchronizing canvases and headers.
	 * Usually called internally via `invalidate()` + animation frame; call manually after batch updates.
	 */
	render: () => void;
	/**
	 * Reorders columns based on their ids and optionally updates visibility.
	 * @param columnIds - Ordered list of column ids to become the new visible order.
	 * @param opt - When `opt.notify` is `false`, suppresses `onColumnsReordered`; when `opt.setVisible` is provided, visible columns are set to that list.
	 */
	reorderColumns(columnIds: string[], opt?: {
		notify?: boolean;
		setVisible?: string[];
	}): void;
	/** Clears the active cell without scrolling. */
	resetActiveCell(): void;
	/**
	 * Recalculates canvas/viewport sizes and re-renders headers and rows. Call after external
	 * container resize when `autoHeight` is off.
	 */
	resizeCanvas: () => void;
	/** Scrolls the viewport so the active cell is visible. */
	scrollActiveCellIntoView(): void;
	/**
	 * Scrolls a specific cell into view.
	 * @param row - Row index.
	 * @param cell - Cell/column index.
	 * @param doPaging - Whether to page the view when the row is far outside the viewport.
	 */
	scrollCellIntoView(row: number, cell: number, doPaging?: boolean): void;
	/**
	 * Scrolls a column into view without changing the active row.
	 * @param cell - Visible column index to bring into view.
	 */
	scrollColumnIntoView(cell: number): void;
	/**
	 * Scrolls a row into view.
	 * @param row - Row index.
	 * @param doPaging - Whether to page the view when the row is far outside the viewport.
	 */
	scrollRowIntoView(row: number, doPaging?: boolean): void;
	/**
	 * Scrolls so that the given row is at the top of the viewport.
	 * @param row - Row index to position at the top.
	 */
	scrollRowToTop(row: number): void;
	/**
	 * Sets the active cell, committing or cancelling any pending edit as needed.
	 * @param row - Row index to activate.
	 * @param cell - Cell/column index to activate.
	 */
	setActiveCell(row: number, cell: number): void;
	/**
	 * Sets the active row, optionally suppressing the automatic scroll into view.
	 * @param row - Row index to become active.
	 * @param cell - Cell/column index to become active.
	 * @param suppressScrollIntoView - When `true`, the grid does not scroll to show the cell.
	 */
	setActiveRow(row: number, cell: number, suppressScrollIntoView?: boolean): void;
	/**
	 * Sets per-cell CSS styles under the given key, replacing any previous styles for that key.
	 * @param key - Bucket name.
	 * @param hash - Map of `row -> columnId -> cssClass`.
	 */
	setCellCssStyles(key: string, hash: CellStylesHash): void;
	/**
	 * Shows or hides the column header row.
	 * @param visible - `true` to show, `false` to hide.
	 */
	setColumnHeaderVisibility(visible: boolean): void;
	/**
	 * Replaces the column set and re-renders headers/rows.
	 * @param columns - New ordered list of column definitions.
	 */
	setColumns(columns: Column<TItem>[]): void;
	/**
	 * Sets the visible columns by id and optionally reorders them.
	 * @param columnIds - Ids of columns to make visible, in desired order.
	 * @param opt - When `opt.reorder` is `false`, current order is preserved; when `opt.notify` is `false`, `onColumnsReordered` is suppressed.
	 */
	setVisibleColumns(columnIds: string[], opt?: {
		reorder?: boolean;
		notify?: boolean;
	}): void;
	/**
	 * Replaces the data source and refreshes the view.
	 * @param newData - New data array or `DataView`-like object.
	 * @param scrollToTop - Whether to scroll to the top after the replacement.
	 */
	setData(newData: any, scrollToTop?: boolean): void;
	/**
	 * Shows or hides the footer row.
	 * @param visible - `true` to show, `false` to hide.
	 */
	setFooterRowVisibility(visible: boolean): void;
	/**
	 * Shows or hides the grouping panel.
	 * @param visible - `true` to show, `false` to hide.
	 */
	setGroupingPanelVisibility(visible: boolean): void;
	/**
	 * Shows or hides the header row (filter row).
	 * @param visible - `true` to show, `false` to hide.
	 */
	setHeaderRowVisibility(visible: boolean): void;
	/**
	 * Merges the given options into the current options and optionally re-renders.
	 * @param args - Options to merge.
	 * @param suppressRender - When `true`, no render is triggered.
	 * @param suppressColumnSet - When `true`, columns are not re-set from `args.columns`.
	 * @param suppressSetOverflow - When `true`, the canvas overflow recalculation is skipped.
	 */
	setOptions(args: GridOptions<TItem>, suppressRender?: boolean, suppressColumnSet?: boolean, suppressSetOverflow?: boolean): void;
	/**
	 * Shows or hides the pre-header panel (deprecated grouping-panel variant).
	 * @param visible - `true` to show, `false` to hide.
	 */
	setPreHeaderPanelVisibility(visible: boolean): void;
	/**
	 * Selects the given rows (used by legacy row-selection integration).
	 * @param rows - Row indices to select.
	 */
	setSelectedRows(rows: number[]): void;
	/**
	 * Attaches a selection-model plugin.
	 * @param model - Selection model to activate.
	 */
	setSelectionModel(model: SelectionModel): void;
	/**
	 * Sets single-column sort state.
	 * @param columnId - Column id to sort by.
	 * @param ascending - `true` for ascending, `false` for descending.
	 */
	setSortColumn(columnId: string, ascending: boolean): void;
	/**
	 * Sets multi-column sort state.
	 * @param cols - Array of sort descriptors.
	 */
	setSortColumns(cols: ColumnSort[]): void;
	/**
	 * Shows or hides the top panel.
	 * @param visible - `true` to show, `false` to hide.
	 */
	setTopPanelVisibility(visible: boolean): void;
	/**
	 * Invalidates and re-renders a single cell.
	 * @param row - Row index.
	 * @param cell - Cell/column index.
	 */
	updateCell(row: number, cell: number): void;
	/**
	 * Updates a header cell's title/tooltip in place without a full column reset.
	 * @param columnId - Column id whose header should be updated.
	 * @param title - New title text or header formatter.
	 * @param toolTip - New tooltip text.
	 */
	updateColumnHeader(columnId: string, title?: string | ColumnFormat<any>, toolTip?: string): void;
	/**
	 * Updates the grid's paging UI from a view/page change.
	 * @param pagingInfo - Paging descriptor with `pageSize`, `pageNum` and `totalPages`.
	 */
	updatePagingStatusFromView(pagingInfo: {
		pageSize: number;
		pageNum: number;
		totalPages: number;
	}): void;
	/**
	 * Invalidates and re-renders an entire row.
	 * @param row - View row index to update.
	 */
	updateRow(row: number): void;
	/** Recalculates row count after the data view changes and re-renders as needed. */
	updateRowCount(): void;
}
/**
 * Summarized description of the grid's layout and pinning support/counters.
 */
export type GridLayoutInfo = {
	/** Number of rows frozen at the top. */
	frozenTopRows: number;
	/** Number of rows frozen at the bottom. */
	frozenBottomRows: number;
	/** Number of columns pinned at the start side. */
	pinnedStartCols: number;
	/** Number of columns pinned at the end side. */
	pinnedEndCols: number;
	/** Whether the current layout engine supports frozen rows. */
	supportFrozenRows: boolean;
	/** Whether the layout engine supports bottom-frozen rows. */
	supportFrozenBottom: boolean;
	/** Whether the layout engine supports pinned columns. */
	supportPinnedCols: boolean;
	/** Whether the layout engine supports end-pinned columns. */
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
/**
 * Value returned by a formatter. Strings are treated as text or HTML depending on
 * `enableHtmlRendering`; DOM nodes are appended directly.
 */
export type FormatterResult = (string | HTMLElement | SVGElement | MathMLElement | DocumentFragment);
/**
 * Modern formatter signature; receives a {@link FormatterContext} and returns a {@link FormatterResult}.
 * @template TItem - Row item type.
 * @param ctx - Formatter context containing value, row/cell coordinates, column, grid and helpers.
 * @returns Renderable result for the cell.
 */
export type ColumnFormat<TItem = any> = (ctx: FormatterContext<TItem>) => FormatterResult;
/**
 * Structured result for legacy formatters that need to convey extra metadata.
 */
export interface CompatFormatterResult {
	/** Extra CSS classes to add to the cell node. */
	addClasses?: string;
	/** Main cell content. */
	text?: FormatterResult;
	/** Tooltip text for the cell node. */
	toolTip?: string;
}
/**
 * Legacy formatter signature kept for backward compatibility.
 * @template TItem - Row item type.
 * @param row - Row index.
 * @param cell - Cell/column index.
 * @param value - Raw cell value.
 * @param column - Column definition.
 * @param item - Row data item.
 * @param grid - Grid instance, if available.
 * @returns Plain string or structured result with classes/tooltip.
 */
export type CompatFormatter<TItem = any> = (row: number, cell: number, value: any, column: Column<TItem>, item: TItem, grid?: ISleekGrid) => string | CompatFormatterResult;
/**
 * Factory that can provide formatters for columns, allowing centralized formatter resolution.
 * @template TItem - Row item type.
 */
export interface FormatterFactory<TItem = any> {
	/**
	 * Returns the modern {@link ColumnFormat} for the given column, if any.
	 * @param column - Column to resolve a formatter for.
	 * @returns Formatter function or `undefined`.
	 */
	getFormat?(column: Column<TItem>): ColumnFormat<TItem>;
	/**
	 * Returns the legacy {@link CompatFormatter} for the given column, if any.
	 * @param column - Column to resolve a formatter for.
	 * @returns Legacy formatter or `undefined`.
	 */
	getFormatter?(column: Column<TItem>): CompatFormatter<TItem>;
}
/**
 * Callback invoked asynchronously after a cell node has been rendered and attached.
 * @template TItem - Row item type.
 * @param cellNode - Rendered cell DOM node.
 * @param row - Row index.
 * @param item - Row data item.
 * @param column - Column definition.
 * @param reRender - Whether the call is due to a re-render of an already visible row.
 */
export type AsyncPostRender<TItem = any> = (cellNode: HTMLElement, row: number, item: TItem, column: Column<TItem>, reRender: boolean) => void;
/**
 * Cleanup counterpart to {@link AsyncPostRender}; invoked before the cell node is removed.
 * @template TItem - Row item type.
 * @param cellNode - Cell DOM node being cleaned up.
 * @param row - Row index, if known.
 * @param column - Column definition, if known.
 */
export type AsyncPostCleanup<TItem = any> = (cellNode: HTMLElement, row?: number, column?: Column<TItem>) => void;
/** Hash mapping `row -> columnId -> cssClass` for per-cell styling via `setCellCssStyles`. */
export type CellStylesHash = {
	[row: number]: {
		[columnId: string]: string;
	};
};
/**
 * Default column formatter; escapes or returns the value based on `enableHtmlRendering`.
 * Use as a safe fallback when no custom formatter is provided.
 * @param ctx - Formatter context whose `value` is rendered.
 * @returns Escaped or raw string representation of `ctx.value`.
 */
export declare function defaultColumnFormat(ctx: FormatterContext): FormatterResult;
/**
 * Wraps a legacy {@link CompatFormatter} as a modern {@link ColumnFormat} by adapting
 * the argument list and lifting `addClasses`/`toolTip` onto the context.
 * @param compatFormatter - Legacy formatter to convert.
 * @returns A {@link ColumnFormat} equivalent, or `null` if input was `null`.
 */
export declare function convertCompatFormatter(compatFormatter: CompatFormatter): ColumnFormat;
/**
 * Applies a formatter result to a DOM cell node, handling content, CSS classes,
 * attributes and tooltips tracked via the formatter context.
 * @param ctx - Active formatter context carrying `addClass`/`addAttrs`/`tooltip` and sanitizer flags.
 * @param fmtResult - Value returned by the formatter.
 * @param node - Cell DOM node to update.
 * @param opt - When `contentOnly` is `true`, only the inner content is updated; decoration cleanup is skipped.
 */
export declare function applyFormatterResultToCellNode(ctx: FormatterContext, fmtResult: FormatterResult, node: HTMLElement, opt?: {
	contentOnly?: boolean;
}): void;
/**
 * Creates a {@link FormatterContext} populated with sensible defaults from the grid
 * options and DOMPurify (when available).
 * @template TItem - Row item type.
 * @param opt - Partial context fields to pre-fill; `addAttrs`/`addClass`/`tooltip` are managed by the formatter itself.
 * @returns Fully initialized formatter context ready to pass to a {@link ColumnFormat}.
 */
export declare function formatterContext<TItem = any>(opt?: Partial<Exclude<FormatterContext<TItem>, "addAttrs" | "addClass" | "tooltip">>): FormatterContext<TItem>;
/**
 * Definition of a single grid column.
 * @template TItem - Row item type the column belongs to.
 */
export interface Column<TItem = any> {
	/** Async post-render hook invoked after the cell node is attached to the DOM. */
	asyncPostRender?: AsyncPostRender<TItem>;
	/** Cleanup counterpart to `asyncPostRender`; called before the node is removed or re-rendered. */
	asyncPostRenderCleanup?: AsyncPostCleanup<TItem>;
	/** Arbitrary behavior token consumed by plugins (e.g. `"selectAndMove"`). */
	behavior?: any;
	/** When `true`, editing this column cannot trigger insertion of a new row. */
	cannotTriggerInsert?: boolean;
	/** CSS class(es) applied to every body cell in this column. */
	cssClass?: string;
	/** Default sort direction for this column; `true` means ascending. */
	defaultSortAsc?: boolean;
	/** Editor class used when the cell enters edit mode. */
	editor?: EditorClass;
	/** Fixed number of decimal places the editor should preserve (if applicable). */
	editorFixedDecimalPlaces?: number;
	/** Property name on `TItem` that this column is bound to. */
	field?: string;
	/** Freezing / pinning of the column. `true`/`"start"` pins to the start side, `"end"` to the end side. */
	frozen?: boolean | "start" | "end";
	/** Whether cells in this column can receive focus. Defaults to `true`. */
	focusable?: boolean;
	/** CSS class(es) applied to footer row cells in this column. */
	footerCssClass?: string;
	/** Modern formatter for body cells. Prefer this over the deprecated `formatter`. */
	format?: ColumnFormat<TItem>;
	/**
	 * Legacy formatter for body cells.
	 * @deprecated Use {@link Column.format} instead.
	 */
	formatter?: CompatFormatter<TItem>;
	/** Formatter used to render group-totals rows for this column. */
	groupTotalsFormat?: (ctx: FormatterContext<IGroupTotals<TItem>>) => FormatterResult;
	/**
	 * Legacy group-totals formatter.
	 * @deprecated Use {@link Column.groupTotalsFormat} instead.
	 */
	groupTotalsFormatter?: (totals?: IGroupTotals<TItem>, column?: Column<TItem>, grid?: unknown) => string;
	/** CSS class(es) applied to the header cell. */
	headerCssClass?: string;
	/** Unique column identifier. Auto-generated from `field` or a fallback if omitted. */
	id?: string;
	/** Maximum pixel width the column may be resized to. */
	maxWidth?: any;
	/** Minimum pixel width the column may be resized to. */
	minWidth?: number;
	/** Display name shown in the header. Defaults to a titleized form of `field`/`id`. */
	name?: string;
	/** Formatter used to render the header `name` content. */
	nameFormat?: (ctx: FormatterContext<TItem>) => FormatterResult;
	/** Previous width before the last resize; managed internally for `forceFitColumns`. */
	previousWidth?: number;
	/** Extra field names the column depends on (besides `field`), used for dirty tracking. */
	referencedFields?: string[];
	/** When `true`, cells are re-rendered on column resize. */
	rerenderOnResize?: boolean;
	/** Whether the column can be resized by dragging its header border. */
	resizable?: boolean;
	/** Whether cells in this column can be selected. */
	selectable?: boolean;
	/** Whether cells in this column participate in tab navigation. */
	tabbable?: boolean;
	/** Whether clicking the header sorts by this column. */
	sortable?: boolean;
	/** Sort priority when multiple columns are sorted; lower numbers sort first. */
	sortOrder?: number;
	/** Tooltip text for the header cell. */
	toolTip?: string;
	/**
	 * Optional validator invoked by the editor.
	 * @param value - The value to validate.
	 * @param editorArgs - Additional editor context, if any.
	 * @returns Validation result indicating validity and an optional message.
	 */
	validator?: (value: any, editorArgs?: any) => ValidationResult;
	/** Whether the column is currently visible. Columns with `visible: false` are hidden but retained. */
	visible?: boolean;
	/** Current pixel width of the column. */
	width?: number;
}
/**
 * Default property values applied to each column when none is specified.
 * Used as a fallback by {@link initColumnProps}.
 */
export declare const columnDefaults: Partial<Column>;
/**
 * Per-cell metadata that can override column-level settings for a specific row.
 * @template TItem - Row item type.
 */
export interface ColumnMetadata<TItem = any> {
	/** Column span for this cell. Use `"*"` to span to the end of the row. */
	colspan?: number | "*";
	/** Extra CSS classes applied to the cell node. */
	cssClasses?: string;
	/** Whether the cell can receive focus. */
	focusable?: boolean;
	/** Editor class override for this cell. */
	editor?: EditorClass;
	/** Formatter override for this cell. */
	format?: ColumnFormat<TItem>;
	/**
	 * Legacy formatter override.
	 * @deprecated Use {@link ColumnMetadata.format} instead.
	 */
	formatter?: CompatFormatter<TItem>;
	/** Whether the cell can be selected. */
	selectable?: boolean;
	/** Whether the cell participates in tab navigation. */
	tabbable?: boolean;
}
/**
 * Describes a single active sort criterion.
 */
export interface ColumnSort {
	/** Column `id` to sort by. */
	columnId: string;
	/** Sort direction; `true` for ascending, `false` for descending. */
	sortAsc?: boolean;
}
/**
 * Row-level metadata that can influence rendering and interaction.
 * Returned by `DataView.getItemMetadata(row)`.
 * @template TItem - Row item type.
 */
export interface ItemMetadata<TItem = any> {
	/** Extra CSS classes applied to the row node. */
	cssClasses?: string;
	/** Per-column metadata overrides for this row. */
	columns?: {
		[key: string]: ColumnMetadata<TItem>;
	};
	/** Whether any cell in the row can receive focus. */
	focusable?: boolean;
	/** Default formatter for all cells in the row. */
	format?: ColumnFormat<TItem>;
	/**
	 * Legacy default formatter for the row.
	 * @deprecated Use {@link ItemMetadata.format} instead.
	 */
	formatter?: CompatFormatter<TItem>;
	/** Whether any cell in the row can be selected. */
	selectable?: boolean;
	/** Whether any cell in the row participates in tab navigation. */
	tabbable?: boolean;
}
/**
 * Normalizes column definitions: applies defaults, clamps widths and ensures unique ids/names.
 * Mutates the `columns` array in place.
 * @param columns - Column definitions to initialize.
 * @param defaults - Default values to fall back to for missing properties.
 */
export declare function initColumnProps(columns: Column[], defaults: Partial<Column<any>>): void;
/**
 * Converts a field/column identifier to a human-readable Title Case string.
 * Handles camelCase, PascalCase, snake_case, kebab-case and whitespace separated names.
 * @param str - Raw identifier to titleize.
 * @returns Title-cased, space-separated string (e.g. `"firstName"` → `"First Name"`).
 */
export declare function titleize(str: string): string;
/**
 * Starting scroll-adjusted position and logical range for a drag interaction.
 */
export interface DragPosition {
	/** Scroll-adjusted X coordinate where the drag started. */
	startX: number;
	/** Scroll-adjusted Y coordinate where the drag started. */
	startY: number;
	/** Logical grid range derived from pointer movement, if applicable. */
	range: DragRange;
}
/**
 * Full drag-state payload passed to drag callbacks; extends {@link DragPosition}
 * with live deltas and DOM references.
 */
export interface DragItem extends DragPosition {
	/** Element or document that the draggable was bound to. */
	dragSource: HTMLElement | Document | null;
	/** Element on which the drag gesture started. */
	dragHandle: HTMLElement | null;
	/** Horizontal delta in pixels since drag start. */
	deltaX: number;
	/** Vertical delta in pixels since drag start. */
	deltaY: number;
	/** Current DOM element under the pointer during the drag. */
	dragTarget: HTMLElement;
}
/**
 * Logical start/end row/cell range accumulated during a drag (e.g. for column reordering).
 */
export interface DragRange {
	/** Starting row/cell for the drag range. */
	start: {
		row?: number;
		cell?: number;
	};
	/** Ending row/cell for the drag range. */
	end: {
		row?: number;
		cell?: number;
	};
}
/**
 * Options for the {@link Draggable} helper.
 */
export interface DraggableOption {
	/**
	 * Container DOM element to listen for mousedown/touchstart on.
	 * Defaults to `document.body` when omitted.
	 */
	containerElement?: HTMLElement | Document;
	/**
	 * When defined, dragging is only allowed when the mousedown target
	 * matches this CSS selector (checked via `Element.matches()`).
	 */
	allowDragFrom?: string;
	/**
	 * When defined, dragging is allowed when the mousedown target or one of its
	 * closest ancestors matches this selector (checked via `Element.closest()`).
	 */
	allowDragFromClosest?: string;
	/**
	 * Keys that, when pressed during the interaction, prevent draggable events from firing.
	 * Defaults to `['ctrlKey', 'metaKey']` at the call site (e.g. prevents drag when Ctrl is held).
	 */
	preventDragFromKeys?: Array<"altKey" | "ctrlKey" | "metaKey" | "shiftKey">;
	/**
	 * Invoked on mousedown before any dragging starts. Return `false` to cancel the drag.
	 * @param e - Native drag/mouse event.
	 * @param dd - Current drag position and range.
	 */
	onDragInit?: (e: DragEvent, dd: DragPosition) => boolean | void;
	/**
	 * Invoked the first time the pointer moves after mousedown.
	 * @param e - Native drag/mouse event.
	 * @param dd - Current drag position and range.
	 */
	onDragStart?: (e: DragEvent, dd: DragPosition) => boolean | void;
	/**
	 * Invoked on every pointer move while dragging.
	 * @param e - Native drag/mouse event.
	 * @param dd - Current drag position and range including live deltas.
	 */
	onDrag?: (e: DragEvent, dd: DragPosition) => boolean | void;
	/**
	 * Invoked when the pointer is released after a drag has started.
	 * @param e - Native drag/mouse event.
	 * @param dd - Final drag position and range.
	 */
	onDragEnd?: (e: DragEvent, dd: DragPosition) => boolean | void;
}
/**
 * Attaches lightweight mouse/touch drag handling to a container element without jQuery.
 * Listens for mousedown/touchstart on `containerElement` and translates movements
 * into the `onDrag*` callbacks in {@link DraggableOption}.
 * @param options - Configuration controlling drag source, filters and callbacks.
 * @returns Handle with a `destroy` method to remove all listeners.
 */
export declare function Draggable(options: DraggableOption): {
	/** Removes all event listeners installed by this draggable instance. */
	destroy: () => void;
};
/**
 * Minimal data-view contract consumed by the grid. Implemented by `DataView`.
 * @template TItem - Row item type.
 */
export interface IDataView<TItem = any> {
	/**
	 * Gets grand totals aggregated over the entire data set.
	 * @returns Grand totals object containing `sum`/`avg`/`min`/`max`, if any.
	 */
	getGrandTotals(): IGroupTotals;
	/**
	 * Gets the total number of rows currently in the view (including group headers/totals).
	 * @returns Row count.
	 */
	getLength(): number;
	/**
	 * Gets the item at the specified view row.
	 * @param row - Zero-based view index.
	 * @returns Data item, `Group` header, or `IGroupTotals` row.
	 */
	getItem(row: number): (TItem | Group<TItem> | IGroupTotals);
	/**
	 * Gets row metadata (CSS classes, per-column overrides) for the specified view row.
	 * @param row - Zero-based view index.
	 * @returns Metadata object or `undefined` when none applies.
	 */
	getItemMetadata?(row: number): ItemMetadata<TItem>;
	/** Event fired when the underlying data set changes. */
	readonly onDataChanged?: EventEmitter<{}>;
	/**
	 * Event fired when the row count changes.
	 * Payload is `{ previous, current }` with the counts before and after the change.
	 */
	readonly onRowCountChanged?: EventEmitter<{
		previous: number;
		current: number;
	}>;
	/**
	 * Event fired when specific view rows change (values or metadata).
	 * Payload is `{ rows }` with the list of affected view indices.
	 */
	readonly onRowsChanged?: EventEmitter<{
		rows: number[];
	}>;
}
/**
 * Adds one or more CSS classes to an element, supporting space-separated lists.
 * @param el - Target element.
 * @param cls - Class name or space-separated class list to add. No-op when empty/null.
 */
export declare function addCssClass(el: Element, cls: string): void;
/**
 * Escapes a value for safe insertion as HTML when `enableHtmlRendering` is `true`.
 * When called as `ctx.escape()` (without arguments) inside a formatter, uses `this.value`.
 * When `this.enableHtmlRendering === false`, the value is returned as a plain string without escaping.
 * @param s - Value to escape; when omitted and called with a `FormatterContext` as `this`, escapes `this.value`.
 * @returns HTML-escaped string (or plain string when HTML rendering is disabled).
 */
export declare function escapeHtml(s: any): string;
/**
 * Lightweight HTML sanitizer using `DOMParser`. Strips scripts, iframes, event handlers
 * and dangerous URL protocols; falls back to {@link escapeHtml} when `DOMParser` is unavailable.
 * Prefer the grid's injected `sanitizer` (DOMPurify when present) for production; this is a safe default.
 * @param dirtyHtml - Raw HTML string to sanitize.
 * @returns Sanitized HTML string safe to assign to `innerHTML`.
 */
export declare function basicDOMSanitizer(dirtyHtml: string): string;
/**
 * Disables text selection on the target element.
 * @param target - Element to make unselectable.
 */
export declare function disableSelection(target: HTMLElement): void;
/**
 * Removes one or more CSS classes from an element, supporting space-separated lists.
 * @param el - Target element.
 * @param cls - Class name or space-separated class list to remove. No-op when empty/null.
 */
export declare function removeCssClass(el: Element, cls: string): void;
/**
 * Parses a CSS pixel string (e.g. `"20px"`) into a number, returning `0` for non-numeric input.
 * @param str - CSS length string to parse.
 * @returns Numeric pixel value or `0` when parsing fails.
 */
export declare function parsePx(str: string): number;
/**
 * Default single-pane layout. Renders header, header row, top panel, body
 * viewport and footer row in the main band without pinning or frozen panes.
 */
export declare class BasicLayout implements LayoutEngine {
	/** Host provided during {@link BasicLayout.init}. */
	protected host: LayoutHost;
	/** Refs snapshot provided during {@link BasicLayout.init}. */
	protected refs: GridLayoutRefs;
	/**
	 * Builds the basic layout DOM inside `host.getContainerNode()`.
	 * @param host - Layout host.
	 */
	init(host: LayoutHost): void;
	/**
	 * Clears host and refs references.
	 */
	destroy(): void;
	/**
	 * No-op for the basic layout; options require no layout-specific handling.
	 */
	afterSetOptions(): void;
	/** Layout identifier. */
	readonly layoutName = "BasicLayout";
}
/**
 * Frozen/pinned layout providing pinned columns and frozen top panes.
 * Renders `start`/`main` bands with `top`/`body` panes and handles
 * `frozenRows`/`frozenBottom` and legacy `frozenColumns` options.
 */
export declare class FrozenLayout implements LayoutEngine {
	/** Host provided during {@link FrozenLayout.init}. */
	private host;
	/** Refs provided during {@link FrozenLayout.init}. */
	private refs;
	/**
	 * Builds the frozen layout DOM (headers, header rows, viewports, footer rows)
	 * across `start`/`main` bands and top/body panes.
	 * @param host - Layout host.
	 */
	init(host: LayoutHost): void;
	/**
	 * Reorders visible columns so that pinned (non-`"end"`) columns come first.
	 * Also writes `refs.config.pinnedStartCols` for later layout calculations.
	 * @param viewCols - Visible columns in current order.
	 * @param refs - Mutable layout refs to update.
	 * @returns Reordered columns when pinning exists, `null` otherwise.
	 */
	reorderViewColumns(viewCols: Column[], refs: GridLayoutRefs): Column[];
	/**
	 * Reacts to grid option changes (frozen rows/columns).
	 * @param arg - Options delta from `grid.setOptions()`.
	 */
	afterSetOptions(arg: GridOptions): void;
	/**
	 * Syncs `refs.config.frozenTopRows` from `frozenRows`/`frozenBottom` grid options.
	 */
	adjustFrozenRowsOption(): void;
	/**
	 * Clears the host reference.
	 */
	destroy(): void;
	/** Layout identifier. */
	readonly layoutName = "FrozenLayout";
	/** Indicates this layout supports pinned columns. */
	supportPinnedCols: true;
	/** Indicates this layout supports top-frozen rows. */
	supportFrozenRows: true;
}
/**
 * Header shell component for a single band. Hosts the column-header container
 * and hides automatically when the band is empty or the header is hidden.
 * @param props.band - Target band key.
 * @param props.refs - Layout refs owning the `headerCols` node.
 * @param props.signals - Visibility/pinning signals.
 */
export declare const Header: ({ band, refs, signals }: {
	band: BandKey;
	refs: GridLayoutRefs;
	signals: Pick<GridSignals, "hideColumnHeader" | "pinnedStartCols" | "pinnedEndCols">;
}) => JSXElement;
/**
 * Header-row (filter row) shell for a single band.
 * @param props.band - Target band key.
 * @param props.refs - Layout refs owning the `headerRowCols` node.
 * @param props.signals - Visibility/pinning signals.
 */
export declare const HeaderRow: ({ band, refs, signals }: {
	band: BandKey;
	refs: GridLayoutRefs;
	signals: Pick<GridSignals, "hideHeaderRow" | "pinnedStartCols" | "pinnedEndCols">;
}) => JSXElement;
/**
 * Top panel container attached to the main band; hidden when `hideTopPanel` is true.
 * @param props.refs - Layout refs owning `topPanel`.
 * @param props.signals - Visibility signals.
 */
export declare const TopPanel: ({ refs, signals }: {
	refs: GridLayoutRefs;
	signals: Pick<GridSignals, "hideTopPanel">;
}) => JSXElement;
/**
 * Scrollable viewport + canvas pair for a single `band`/`pane` cell.
 * Hidden when the corresponding frozen/pinned count is `0`.
 * @param props.band - Horizontal band key.
 * @param props.pane - Vertical pane key.
 * @param props.refs - Layout refs owning `canvas[pane]`.
 * @param props.signals - Pinning/frozen count signals.
 */
export declare const Viewport: ({ band, pane, refs, signals }: {
	band: BandKey;
	pane: PaneKey;
	refs: GridLayoutRefs;
	signals: Pick<GridSignals, "frozenTopRows" | "frozenBottomRows" | "pinnedStartCols" | "pinnedEndCols">;
}) => JSXElement;
/**
 * Footer row shell for a single band.
 * @param props.band - Target band key.
 * @param props.refs - Layout refs owning the `footerRowCols` node.
 * @param props.signals - Visibility/pinning signals.
 */
export declare const FooterRow: ({ band, refs, signals }: {
	band: BandKey;
	refs: GridLayoutRefs;
	signals: Pick<GridSignals, "hideFooterRow" | "pinnedStartCols" | "pinnedEndCols">;
}) => JSXElement;
/**
 * Main virtualized grid implementation. Handles viewport layout, column
 * sizing, keyboard/cell navigation, editing, selection, and async post rendering.
 * Implements {@link ISleekGrid}.
 * @template TItem - Data item type.
 */
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
	/** Fired when the active cell changes ({@link ArgsCell}). */
	readonly onActiveCellChanged: EventEmitter<ArgsCell>;
	/** Fired when the active cell's box/position changes (scroll/ancestor scroll). */
	readonly onActiveCellPositionChanged: EventEmitter<ArgsGrid>;
	/** Fired when the Add-New row attempts to create a new item. */
	readonly onAddNewRow: EventEmitter<ArgsAddNewRow>;
	/** Static emitter also fired after any grid is initialized. */
	static readonly onAfterInit: EventEmitter<ArgsGrid>;
	/** Fired after {@link SleekGrid.init} completes for this instance. */
	readonly onAfterInit: EventEmitter<ArgsGrid>;
	/** Before a cell editor is destroyed (allows intercept). */
	readonly onBeforeCellEditorDestroy: EventEmitter<ArgsEditorDestroy>;
	/** Before the grid is destroyed. */
	readonly onBeforeDestroy: EventEmitter<ArgsGrid>;
	/** Cancelable; before a cell becomes editable. */
	readonly onBeforeEditCell: EventEmitter<ArgsCellEdit>;
	/** Before a footer-row column node is removed. */
	readonly onBeforeFooterRowCellDestroy: EventEmitter<ArgsColumnNode>;
	/** Before a header column node is removed. */
	readonly onBeforeHeaderCellDestroy: EventEmitter<ArgsColumnNode>;
	/** Before a header-row column node is removed. */
	readonly onBeforeHeaderRowCellDestroy: EventEmitter<ArgsColumnNode>;
	/** After an editor commits a cell change. */
	readonly onCellChange: EventEmitter<ArgsCellChange>;
	/** After `setCellCssStyles`/`addCellCssStyles`/`removeCellCssStyles`. */
	readonly onCellCssStylesChanged: EventEmitter<ArgsCssStyle>;
	/** Click on a cell's canvas. */
	readonly onClick: EventEmitter<ArgsCell, MouseEvent>;
	/** After columns are reordered (drag or API). */
	readonly onColumnsReordered: EventEmitter<ArgsGrid>;
	/** After columns are resized. */
	readonly onColumnsResized: EventEmitter<ArgsGrid>;
	/** Forwarded by editors in composite-edit mode when a field value changes. */
	readonly onCompositeEditorChange: EventEmitter<ArgsGrid>;
	/** Context menu on the grid canvas (opportunity to suppress/override). */
	readonly onContextMenu: EventEmitter<ArgsGrid, UIEvent>;
	/** Double-click on a cell. */
	readonly onDblClick: EventEmitter<ArgsCell, MouseEvent>;
	/** Ongoing drag (after threshold). */
	readonly onDrag: EventEmitter<ArgsDrag, UIEvent>;
	/** End of drag lifecycle. */
	readonly onDragEnd: EventEmitter<ArgsDrag, UIEvent>;
	/** Initial drag attempt (cancelable via `stopImmediatePropagation`). */
	readonly onDragInit: EventEmitter<ArgsDrag, UIEvent>;
	/** When drag start threshold is passed. */
	readonly onDragStart: EventEmitter<ArgsDrag, UIEvent>;
	/** After a footer-row cell is created. */
	readonly onFooterRowCellRendered: EventEmitter<ArgsColumnNode>;
	/** After a header cell is created. */
	readonly onHeaderCellRendered: EventEmitter<ArgsColumnNode>;
	/** Click on a header column. */
	readonly onHeaderClick: EventEmitter<ArgsColumn, MouseEvent>;
	/** Context menu on a header column. */
	readonly onHeaderContextMenu: EventEmitter<ArgsColumn, MouseEvent>;
	/** Mouse entered a header column. */
	readonly onHeaderMouseEnter: EventEmitter<ArgsColumn, MouseEvent>;
	/** Mouse left a header column. */
	readonly onHeaderMouseLeave: EventEmitter<ArgsColumn, MouseEvent>;
	/** After a header-row (filter) cell is created. */
	readonly onHeaderRowCellRendered: EventEmitter<ArgsColumnNode>;
	/** Keydown forwarded from focus sinks/canvases. */
	readonly onKeyDown: EventEmitter<ArgsCell, KeyboardEvent>;
	/** Mouse entered a cell's canvas target. */
	readonly onMouseEnter: EventEmitter<ArgsGrid, MouseEvent>;
	/** Mouse left a cell (entering the canvas background). */
	readonly onMouseLeave: EventEmitter<ArgsGrid, MouseEvent>;
	/** Raw scroll offsets after `handleScroll` (viewport and H-sync applied). */
	readonly onScroll: EventEmitter<ArgsScroll>;
	/** After the selected-rows set changes (via selection model). */
	readonly onSelectedRowsChanged: EventEmitter<ArgsSelectedRowsChange>;
	/** After header-driven sort toggling (single or multi). */
	readonly onSort: EventEmitter<ArgsSort>;
	/** When `commitCurrentEdit()` fails validation. */
	readonly onValidationError: EventEmitter<ArgsValidationError>;
	/** After the viewport is re-rendered following a scroll. */
	readonly onViewportChanged: EventEmitter<ArgsGrid>;
	/**
	 * Constructs and initializes a new SleekGrid inside `container`.
	 * Auto-initializes unless `explicitInitialization` is set.
	 * @param container - Selector, element or jQuery/array-like container.
	 * @param data - DataView or plain array of items.
	 * @param columns - Initial column definitions.
	 * @param options - Grid options merged with {@link gridDefaults}.
	 */
	constructor(container: string | HTMLElement | ArrayLike<HTMLElement>, data: any, columns: Column<TItem>[], options: GridOptions<TItem>);
	private applyLegacyHeightOptions;
	private createGroupingPanel;
	private getSignals;
	/**
	 * Performs one-time DOM and event binding after construction. No-ops if
	 * already initialized. Computes sizes, creates headers/footers and binds
	 * scroll/keyboard/mouse handlers.
	 */
	init(): void;
	/**
	 * Prepend-registers a plugin and calls its `init(this)` immediately.
	 * @param plugin - Grid plugin to add.
	 */
	registerPlugin(plugin: GridPlugin): void;
	/**
	 * Unregisters a plugin by identity, calling `destroy()` when available.
	 * @param plugin - Plugin instance previously passed to {@link SleekGrid.registerPlugin}.
	 */
	unregisterPlugin(plugin: GridPlugin): void;
	/**
	 * Looks up a registered plugin by its `pluginName`.
	 * @param name - Plugin name.
	 * @returns Matching plugin or `undefined`.
	 */
	getPluginByName(name: string): GridPlugin;
	/**
	 * Attaches a selection model, unregistering any previous one.
	 * @param model - The new selection model, or `null` to detach.
	 */
	setSelectionModel(model: SelectionModel): void;
	private unregisterSelectionModel;
	/**
	 * Returns native scrollbar thickness for the current environment.
	 * @returns Object with `width` and `height` in pixels.
	 */
	getScrollBarDimensions(): {
		width: number;
		height: number;
	};
	/**
	 * Returns the currently displayed (reserved) scrollbar space, accounting
	 * for auto/hidden scrollbars.
	 * @returns Object with `width` and `height` of displayed scrollbar area.
	 */
	getDisplayedScrollbarDimensions(): {
		width: number;
		height: number;
	};
	/**
	 * Returns the absolute minimum column width derived from header/cell box sizing.
	 */
	getAbsoluteColumnMinWidth(): number;
	/**
	 * Returns the currently attached selection model, if any.
	 * @returns The active {@link SelectionModel}.
	 */
	getSelectionModel(): SelectionModel;
	private getBandRefsForCell;
	/**
	 * Returns summarized layout support/indices for the current layout engine.
	 * @returns {@link GridLayoutInfo} with frozen/pinned counters and capability flags.
	 */
	getLayoutInfo(): GridLayoutInfo;
	/**
	 * Returns the canvas element for the band/pane that owns `row`/`cell`.
	 * @param row - Optional view row hint for frozen-pane disambiguation.
	 * @param cell - Optional cell hint for pinned-band disambiguation.
	 */
	getCanvasNode(row?: number, cell?: number): HTMLElement;
	/**
	 * Returns all rendered canvases across bands/panes (jQuery-wrapped when available).
	 */
	getCanvases(): any | HTMLElement[];
	/**
	 * Returns the canvas that last received focus/interaction, optionally
	 * resolving from an event for plugin compatibility.
	 * @param e - Optional event whose target is used to resolve the canvas.
	 */
	getActiveCanvasNode(e?: {
		target: EventTarget;
	}): HTMLElement;
	/**
	 * Returns the viewport that owns `row`/`cell` (the canvas's parent).
	 * @param row - Optional row hint.
	 * @param cell - Optional cell hint.
	 */
	getViewportNode(row?: number, cell?: number): HTMLElement;
	private getViewportInfo;
	private getViewports;
	/**
	 * Returns the active viewport, optionally resolving from an event for plugin compat.
	 * @param e - Optional event whose target is used to resolve the viewport.
	 */
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
	/**
	 * Updates a header's title/tooltip in place, re-triggering header lifecycle events.
	 * @param columnId - Target column id.
	 * @param title - New title text or formatter.
	 * @param toolTip - New title attribute.
	 */
	updateColumnHeader(columnId: string, title?: string | ColumnFormat<any>, toolTip?: string): void;
	/**
	 * Returns the header column container for the main band.
	 */
	getHeader(): HTMLElement;
	/**
	 * Returns the header cell node for `cell` (id or visible index).
	 * @param cell - Visible column index or column id.
	 */
	getHeaderColumn(cell: number | string): HTMLElement;
	/**
	 * Returns the grouping panel container, if created.
	 */
	getGroupingPanel(): HTMLElement;
	/**
	 * Returns the (legacy) pre-header panel node inside the grouping panel.
	 */
	getPreHeaderPanel(): HTMLElement;
	/**
	 * Returns the header-row (filter row) container for the main band.
	 */
	getHeaderRow(): HTMLElement;
	/**
	 * Returns the header-row cell node for `cell`.
	 * @param cell - Visible column index or column id.
	 */
	getHeaderRowColumn(cell: string | number): HTMLElement;
	/**
	 * Returns the footer-row container for the main band.
	 */
	getFooterRow(): HTMLElement;
	/**
	 * Returns the footer-row cell node for `cell`.
	 * @param cell - Visible column index or column id.
	 */
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
	/**
	 * Notifies the grid that column widths changed externally; updates limits,
	 * re-applies column widths and re-renders as needed.
	 * @param invalidate - When `true`, invalidates and re-renders visible rows.
	 */
	columnsResized(invalidate?: boolean): void;
	private setOverflow;
	private measureCellPaddingAndBorder;
	private removeCssRules;
	private createCssRules;
	/**
	 * Tears down the grid, unbinding events, destroying plugins and removing DOM.
	 * Clears all `on*` emitters and instance-owned properties.
	 */
	destroy(): void;
	/**
	 * Returns the `editorFactory` from current grid options.
	 */
	getEditorFactory(): EditorFactory;
	/**
	 * Returns the current `EditorLock` controlling concurrent edits.
	 */
	getEditorLock(): EditorLock;
	/**
	 * Returns the grid's internal `EditController` (commit/cancel) bound to this instance.
	 */
	getEditController(): EditController;
	/**
	 * Finds a column by its `id` including hidden columns.
	 * @param id - Column id.
	 * @returns Matching column or `null`.
	 */
	getColumnById(id: string): Column<TItem>;
	/**
	 * Returns the column index for `id`.
	 * @param id - Column id.
	 * @param opt.inAll - When `true`, searches all columns; otherwise visible columns.
	 * @returns Column index or `null` when not found.
	 */
	getColumnIndex(id: string, opt?: {
		inAll?: boolean;
	}): number;
	/**
	 * Auto-fits resizable column widths to the available viewport width.
	 */
	autosizeColumns(): void;
	private applyColumnHeaderWidths;
	/**
	 * Sets single-column sorting state.
	 * @param columnId - Column id to sort by.
	 * @param ascending - Whether ascending.
	 */
	setSortColumn(columnId: string, ascending: boolean): void;
	/**
	 * Sets multi-column sorting state and updates header sort indicators.
	 * @param cols - Sort descriptors (`columnId` + `sortAsc`).
	 */
	setSortColumns(cols: ColumnSort[]): void;
	/**
	 * Returns the active sort descriptors.
	 */
	getSortColumns(): ColumnSort[];
	private handleSelectedRangesChanged;
	/**
	 * Returns all columns including hidden ones (in `setColumns` order).
	 */
	getAllColumns(): Column<TItem>[];
	/**
	 * Returns only the currently visible columns in display order.
	 */
	getColumns(): Column<TItem>[];
	private updateViewColLeftRight;
	private updateViewCols;
	/** Set the initial columns, also calls initColumnProps unless opt.initProps is false */
	private setAllCols;
	private handleFrozenColsOption;
	/**
	 * Replaces the column set and invalidates layout. Tries to preserve
	 * identity when called with a permutation of `getColumns()`.
	 * @param columns - New columns in desired order.
	 */
	setColumns(columns: Column<TItem>[]): void;
	private internalSetVisibleColumns;
	/**
	 * Reorders columns by `columnIds` to become the new visible order.
	 * @param columnIds - Desired column id order.
	 * @param opt.notify - Whether to emit `onColumnsReordered` (default `true`).
	 * @param opt.setVisible - When provided, visibility is set to these ids before reorder.
	 */
	reorderColumns(columnIds: string[], opt?: {
		notify?: boolean;
		setVisible?: string[];
	}): void;
	/**
	 * Shows only the columns whose ids are in `columnIds`, optionally reordering them.
	 * @param columnIds - Ids of columns to make visible, in desired order.
	 * @param opt.reorder - When `true` (default), reorders to `columnIds` order.
	 * @param opt.notify - Whether to emit `onColumnsReordered`.
	 */
	setVisibleColumns(columnIds: string[], opt?: {
		reorder?: boolean;
		notify?: boolean;
	}): void;
	/**
	 * Invalidates column chrome and virtualization state after column changes.
	 * Recomputes pinning, re-creates headers/footers, rebuilds CSS rules and re-renders.
	 */
	invalidateColumns(): void;
	/**
	 * Returns current merged grid options.
	 */
	getOptions(): GridOptions<TItem>;
	protected prepareForOptionsChange(): void;
	/**
	 * Merges `args` into options, validates/updates layout signals and optionally
	 * re-renders. Commits or cancels the active edit before changing options.
	 * @param args - Partial options to merge.
	 * @param suppressRender - When `true`, suppresses render pass after set.
	 * @param suppressColumnSet - When `true`, suppresses `setColumns` from `args.columns`.
	 * @param suppressSetOverflow - When `true`, suppresses `setOverflow()` adjustment.
	 */
	setOptions(args: GridOptions<TItem>, suppressRender?: boolean, suppressColumnSet?: boolean, suppressSetOverflow?: boolean): void;
	private validateAndEnforceOptions;
	private setOptionDependentSignals;
	private viewOnRowCountChanged;
	private viewOnRowsChanged;
	private viewOnDataChanged;
	private bindToData;
	private unbindFromData;
	/**
	 * Replaces the data source and rebinds view events.
	 * @param newData - New DataView or plain array.
	 * @param scrollToTop - When `true`, scrolls to `y = 0`.
	 */
	setData(newData: any, scrollToTop?: boolean): void;
	/**
	 * Returns the current data source (DataView or array).
	 */
	getData(): any;
	/**
	 * Returns view length (via `getLength()` when a DataView is attached).
	 */
	getDataLength(): number;
	private getDataLengthIncludingAddNew;
	/**
	 * Returns the data item for a view row (group/totals rows may be `Group`/`IGroupTotals`).
	 * @param row - View row index.
	 */
	getDataItem(row: number): TItem;
	/**
	 * Returns the top panel container element.
	 */
	getTopPanel(): HTMLElement;
	/**
	 * Shows or hides the top panel.
	 * @param visible - Whether to show.
	 */
	setTopPanelVisibility(visible: boolean): void;
	/**
	 * Shows or hides column headers.
	 * @param visible - Whether to show.
	 */
	setColumnHeaderVisibility(visible: boolean): void;
	/**
	 * Shows or hides the footer row and updates grand totals when becoming visible.
	 * @param visible - Whether to show.
	 */
	setFooterRowVisibility(visible: boolean): void;
	/**
	 * Shows or hides the grouping panel.
	 * @param visible - Whether to show.
	 */
	setGroupingPanelVisibility(visible: boolean): void;
	/**
	 * Legacy alias for {@link SleekGrid.setGroupingPanelVisibility}.
	 * @param visible - Whether to show.
	 */
	setPreHeaderPanelVisibility(visible: boolean): void;
	/**
	 * Shows or hides the header row (filter row).
	 * @param visible - Whether to show.
	 */
	setHeaderRowVisibility(visible: boolean): void;
	/**
	 * Returns the grid's container element.
	 */
	getContainerNode(): HTMLElement;
	/**
	 * Returns the unique CSS-namespace UID for this grid instance.
	 */
	getUID(): string;
	private getRowTop;
	private getRowFromPosition;
	private scrollTo;
	/**
	 * Resolves the formatter for a cell, accounting for row/column metadata,
	 * `formatterFactory` and fallbacks (`defaultFormat`/`defaultFormatter`).
	 * @param row - View row index.
	 * @param column - Column definition.
	 */
	getFormatter(row: number, column: Column<TItem>): ColumnFormat<TItem>;
	/**
	 * Creates a {@link FormatterContext} for the given `row`/`cell`.
	 * @param row - View row index.
	 * @param cell - Cell/column index.
	 */
	getFormatterContext(row: number, cell: number): FormatterContext;
	/**
	 * Resolves the group-totals formatter for a column (or its totals variant).
	 * @param column - Column whose totals representation is needed.
	 */
	getTotalsFormatter(column: Column<TItem>): ColumnFormat<TItem>;
	private getEditor;
	/**
	 * Extracts the raw cell value for `columnDef` from `item` (or via
	 * `dataItemColumnValueExtractor` when configured).
	 * @param item - Row data item.
	 * @param columnDef - Column definition.
	 */
	getDataItemValueForColumn(item: TItem, columnDef: Column<TItem>): any;
	private cleanupRows;
	/**
	 * Invalidates and re-renders the entire grid (rows and totals).
	 */
	invalidate(): void;
	/**
	 * Invalidates all cached rows, forcing a full re-render of visible rows.
	 */
	invalidateAllRows(): void;
	private queuePostProcessedRowForCleanup;
	private queuePostProcessedCellForCleanup;
	private removeRowFromCache;
	/**
	 * Invalidates specific view rows so they are re-rendered.
	 * @param rows - View row indices to invalidate.
	 */
	invalidateRows(rows: number[]): void;
	/**
	 * Invalidates a single view row.
	 * @param row - View row index.
	 */
	invalidateRow(row: number): void;
	/**
	 * Re-renders a single cell via the cell's formatter and invalidates async post results.
	 * @param row - View row index.
	 * @param cell - Cell/column index.
	 */
	updateCell(row: number, cell: number): void;
	private updateCellWithFormatter;
	/**
	 * Re-renders all cells of the given row.
	 * @param row - View row index.
	 */
	updateRow(row: number): void;
	private calcViewportSize;
	/**
	 * Recalculates viewport size and updates virtual height/scroll bounds.
	 * Call when the container size changes externally.
	 */
	resizeCanvas: () => void;
	/**
	 * Updates add-new-row paging state from a paging descriptor.
	 * @param pagingInfo - Page size/num/totalPages.
	 */
	updatePagingStatusFromView(pagingInfo: {
		pageSize: number;
		pageNum: number;
		totalPages: number;
	}): void;
	/**
	 * Returns the horizontal scroll container (main body viewport).
	 */
	getScrollContainerX(): HTMLElement;
	/**
	 * Returns the vertical scroll container (main body viewport).
	 */
	getScrollContainerY(): HTMLElement;
	/**
	 * Recomputes virtual/real scroll heights, page offsets and active cell state
	 * after row count or scrollbar visibility changes.
	 */
	updateRowCount(): void;
	private setPaneHeights;
	private setVirtualHeight;
	/**
	 * Returns the current visible viewport range.
	 * @param viewportTop - Optional scroll top override.
	 * @param viewportLeft - Optional scroll left override.
	 */
	getViewport(viewportTop?: number, viewportLeft?: number): ViewRange;
	/**
	 * Returns the visible (clipped to viewport) range.
	 * @param viewportTop - Optional scroll top override.
	 * @param viewportLeft - Optional scroll left override.
	 */
	getVisibleRange(viewportTop?: number, viewportLeft?: number): ViewRange;
	/**
	 * Returns the rendered range including buffers (expanded beyond the viewport).
	 * @param viewportTop - Optional scroll top override.
	 * @param viewportLeft - Optional scroll left override.
	 */
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
	/**
	 * Synchronously renders rows/cells for the current viewport, with throttling.
	 * Coalesces calls via a pending `_hRender` timeout when scrolling fast.
	 */
	render(): void;
	private handleHeaderFooterRowScroll;
	private handleMouseWheel;
	private handleScroll;
	private asyncPostProcessRows;
	private asyncPostProcessCleanupRows;
	private updateCellCssStylesOnRenderedRows;
	/**
	 * Adds a per-cell CSS hash under `key` and applies it to rendered rows.
	 * @param key - Namespace key.
	 * @param hash - Hash of `row -> columnId -> cssClass`.
	 */
	addCellCssStyles(key: string, hash: CellStylesHash): void;
	/**
	 * Removes styles previously added via {@link SleekGrid.addCellCssStyles}.
	 * @param key - Namespace key.
	 */
	removeCellCssStyles(key: string): void;
	/**
	 * Replaces styles for `key` and notifies `onCellCssStylesChanged`.
	 * @param key - Namespace key.
	 * @param hash - New hash.
	 */
	setCellCssStyles(key: string, hash: CellStylesHash): void;
	/**
	 * Returns the hash for `key` as stored by `setCellCssStyles`/`addCellCssStyles`.
	 * @param key - Namespace key.
	 */
	getCellCssStyles(key: string): CellStylesHash;
	/**
	 * Briefly toggles the `cellFlashingCssClass` on `row`/`cell` for animation.
	 * @param row - View row index.
	 * @param cell - Cell/column index.
	 * @param speed - Millisecond interval between toggles.
	 */
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
	/**
	 * Resolves a `row`/`cell` for the given content-space point.
	 * @param x - Horizontal pixel offset from the canvas origin.
	 * @param y - Vertical pixel offset.
	 */
	getCellFromPoint(x: number, y: number): {
		row: number;
		cell: number;
	};
	/**
	 * Reads the column index from a cell's `data-c` or legacy `.l#` class.
	 * @param cellNode - Cell element.
	 */
	getCellFromNode(cellNode: Element): number;
	/**
	 * Resolves the column definition from a cell's node.
	 * @param cellNode - Cell element.
	 */
	getColumnFromNode(cellNode: Element): Column<TItem>;
	/**
	 * Resolves the view row index from a row node (`data-row` or cache).
	 * @param rowNode - Row element.
	 */
	getRowFromNode(rowNode: Element): number;
	/**
	 * Resolves `row`/`cell` for an event targeting a cell.
	 * @param e - DOM event whose `target` lies inside the desired cell.
	 */
	getCellFromEvent(e: any): {
		row: number;
		cell: number;
	};
	/**
	 * Returns the pixel bounds of a cell's box in canvas coordinates.
	 * @param row - Row index.
	 * @param cell - Cell index.
	 */
	getCellNodeBox(row: number, cell: number): {
		top: number;
		right: number;
		bottom: number;
		left: number;
	};
	/**
	 * Clears the currently active cell (without scrolling).
	 */
	resetActiveCell(): void;
	/**
	 * Focuses the active focus sink so subsequent keystrokes reach the grid.
	 */
	focus(): void;
	private setFocus;
	/**
	 * Ensures `row`/`cell` is visible, paging when needed.
	 * @param row - Target view row.
	 * @param cell - Target cell.
	 * @param doPaging - When `true`, pages before scrolling.
	 */
	scrollCellIntoView(row: number, cell: number, doPaging?: boolean): void;
	/**
	 * Horizontally scrolls `cell` into view.
	 * @param cell - Target visible cell index.
	 */
	scrollColumnIntoView(cell: number): void;
	private internalScrollColumnIntoView;
	private setActiveCellInternal;
	/**
	 * Clears any active text selection, handling IE `selection` when present.
	 */
	clearTextSelection(): void;
	private isCellPotentiallyEditable;
	private makeActiveCellNormal;
	/**
	 * Forces the active cell into edit mode (or keeps it active) using `editor` when provided.
	 * @param editor - Optional editor class override.
	 */
	editActiveCell(editor?: EditorClass): void;
	private makeActiveCellEditable;
	private commitEditAndSetFocus;
	private cancelEditAndSetFocus;
	private getActiveCellPosition;
	/**
	 * Returns the absolute box of the grid container (for editor positioning).
	 */
	getGridPosition(): Position;
	private handleActiveCellPositionChange;
	/**
	 * Returns the currently active editor, if any.
	 */
	getCellEditor(): Editor;
	/**
	 * Returns the active `row`/`cell`, or `null` when none is active.
	 */
	getActiveCell(): RowCell;
	/**
	 * Returns the DOM node for the active cell, or `null`.
	 */
	getActiveCellNode(): HTMLElement;
	/**
	 * Scrolls the active cell into view if one exists.
	 */
	scrollActiveCellIntoView(): void;
	/**
	 * Vertically scrolls `row` into view (pads for frozen rows).
	 * @param row - View row index.
	 * @param doPaging - When `true`, page-bumps instead of minimal scroll.
	 */
	scrollRowIntoView(row: number, doPaging?: boolean): void;
	/**
	 * Scrolls so that `row` is at the top of the viewport.
	 * @param row - Target row.
	 */
	scrollRowToTop(row: number): void;
	private scrollPage;
	/**
	 * Scrolls by one page downward (or paging gap) and updates active cell if navigable.
	 */
	navigatePageDown(): void;
	/**
	 * Scrolls by one page upward and updates active cell if navigable.
	 */
	navigatePageUp(): void;
	/**
	 * Navigates to the first data row.
	 */
	navigateTop(): void;
	/**
	 * Navigates to the last data row.
	 */
	navigateBottom(): void;
	/**
	 * Navigates to a specific row, preserving current column when possible.
	 * @param row - Target row index.
	 * @returns `true` (always reported as handled).
	 */
	navigateToRow(row: number): boolean;
	/**
	 * Returns column span for `row`/`cell` via row metadata (`colspan`), or `1`.
	 * @param row - View row index.
	 * @param cell - Cell index.
	 */
	getColspan(row: number, cell: number): number;
	/**
	 * Navigates one cell to the right.
	 */
	navigateRight(): boolean;
	/**
	 * Navigates one cell to the left.
	 */
	navigateLeft(): boolean;
	/**
	 * Navigates one row downward.
	 */
	navigateDown(): boolean;
	/**
	 * Navigates one row upward.
	 */
	navigateUp(): boolean;
	/**
	 * Navigates to the next tabbable cell (including next row wrap).
	 */
	navigateNext(): boolean;
	/**
	 * Navigates to the previous tabbable cell (including wrap to prior row).
	 */
	navigatePrev(): boolean;
	/**
	 * Navigates to the first focusable cell in the active row.
	 */
	navigateRowStart(): boolean;
	/**
	 * Navigates to the last focusable cell in the active row.
	 */
	navigateRowEnd(): boolean;
	private getColumnCount;
	private isRTL;
	private setTabbingDirection;
	/**
	 * Generic navigation dispatcher used by the key handler.
	 * @param dir - Direction (`"up"`, `"down"`, `"left"`, `"right"`, `"next"`, `"prev"`, `"home"`, `"end"`).
	 */
	navigate(dir: string): boolean;
	/**
	 * Returns the cell DOM node for `row`/`cell` if rendered (`rowsCache` hit).
	 * @param row - View row index.
	 * @param cell - Cell index.
	 */
	getCellNode(row: number, cell: number): HTMLElement;
	/**
	 * Activates the cell at `row`/`cell` (no-op if un-navigable or out of bounds).
	 * @param row - Target row.
	 * @param cell - Target cell.
	 */
	setActiveCell(row: number, cell: number): void;
	/**
	 * Marks a row as active (for row-selection integration) without necessarily changing the active cell DOM.
	 * @param row - Row to activate.
	 * @param cell - Preferred cell to anchor on.
	 * @param suppressScrollIntoView - When `true`, does not scroll the row/cell into view.
	 */
	setActiveRow(row: number, cell: number, suppressScrollIntoView?: boolean): void;
	/**
	 * Checks whether `row`/`cell` may become the active (focusable) cell.
	 * Consults row/column metadata and `focusable` flags.
	 * @param row - Row index.
	 * @param cell - Cell index.
	 * @param tab - When `true`, additionally checks `tabbable`.
	 */
	canCellBeActive(row: number, cell: number, tab?: boolean): boolean;
	/**
	 * Checks whether `row`/`cell` is selectable (from row/column metadata `selectable`).
	 * @param row - Row index.
	 * @param cell - Cell index.
	 */
	canCellBeSelected(row: number, cell: number): boolean;
	/**
	 * Navigates to `row`/`cell`, optionally forcing edit mode.
	 * @param row - Target row.
	 * @param cell - Target cell.
	 * @param forceEdit - When `true`, forces editor activation.
	 */
	gotoCell(row: number, cell: number, forceEdit?: boolean): void;
	/**
	 * Commits the active editor value (if any), running validation and either
	 * executing an {@link EditCommand} via `editCommandHandler` or directly.
	 * @param opt.forceValueChange - When `true`, treats unchanged values as changed.
	 * @returns `true` when the commit succeeds (or no edit was active).
	 */
	commitCurrentEdit(opt?: {
		forceValueChange?: boolean;
	}): boolean;
	/**
	 * Cancels the active editor (if any) by delegating to `makeActiveCellNormal()`.
	 * @returns Always `true`.
	 */
	cancelCurrentEdit(): boolean;
	private rowsToRanges;
	/**
	 * Returns selected view rows (delegates to the attached selection model).
	 * @throws When no selection model is attached.
	 */
	getSelectedRows(): number[];
	/**
	 * Sets selection from view row indices via the attached selection model.
	 * @param rows - Row indices to select.
	 * @throws When no selection model is attached.
	 */
	setSelectedRows(rows: number[]): void;
}
/**
 * Renders a numeric percent value as bold colored text (red < 50%, green otherwise).
 * Returns `"-"` when the value is empty/null.
 * @param ctx - Formatter context whose `value` is the numeric percentage (0–100).
 * @returns A `<span>` element with colored text, or `"-"` for empty values.
 */
export declare function PercentCompleteFormatter(ctx: FormatterContext): FormatterResult;
/**
 * Renders a numeric percent value as a horizontal bar whose color varies by
 * threshold (red < 30, silver < 70, green otherwise).
 * @param ctx - Formatter context whose `value` is the numeric percentage (0–100).
 * @returns A `<span>` bar element, or empty string for empty values.
 */
export declare function PercentCompleteBarFormatter(ctx: FormatterContext): FormatterResult;
/**
 * Renders a boolean value as `"Yes"` or `"No"`.
 * @param ctx - Formatter context whose `value` is coerced to boolean.
 * @returns `"Yes"` when truthy, `"No"` otherwise.
 */
export declare function YesNoFormatter(ctx: FormatterContext): FormatterResult;
/**
 * Renders a boolean value as a styled checkbox icon (`<i>` with
 * `slick-checkbox` / `checked` classes).
 * @param ctx - Formatter context whose `value` is coerced to boolean.
 * @returns An `<i>` element representing the checkbox state.
 */
export declare function CheckBoxFormatter(ctx: FormatterContext): FormatterResult;
/**
 * Renders a boolean value as a checkmark icon; nothing when falsy.
 * @param ctx - Formatter context whose `value` is coerced to boolean.
 * @returns An `<i>` with `slick-checkmark` when truthy, otherwise empty string.
 */
export declare function CheckmarkFormatter(ctx: FormatterContext): FormatterResult;
/**
 * Legacy namespace exposing formatters with the old `(row, cell, value)` signature.
 * Each adapter wraps the modern `*Formatter(ctx)` via {@link formatterContext}.
 * @deprecated Prefer importing the named formatters from `"./formatters"` directly
 * and passing a {@link FormatterContext}.
 */
export declare namespace Formatters {
	/** Legacy adapter for {@link PercentCompleteFormatter}. @param _row - Unused row index. @param _cell - Unused cell index. @param value - Cell value (0–100). @returns Rendered percent text element. */
	function PercentComplete(_row: number, _cell: number, value: any): FormatterResult;
	/** Legacy adapter for {@link PercentCompleteBarFormatter}. @param _row - Unused row index. @param _cell - Unused cell index. @param value - Cell value (0–100). @returns Rendered percent bar element. */
	function PercentCompleteBar(_row: number, _cell: number, value: any): FormatterResult;
	/** Legacy adapter for {@link YesNoFormatter}. @param _row - Unused row index. @param _cell - Unused cell index. @param value - Truthy/falsy cell value. @returns `"Yes"` or `"No"`. */
	function YesNo(_row: number, _cell: number, value: any): FormatterResult;
	/** Legacy adapter for {@link CheckBoxFormatter}. @param _row - Unused row index. @param _cell - Unused cell index. @param value - Truthy/falsy cell value. @returns Checkbox icon element. */
	function Checkbox(_row: number, _cell: number, value: any): FormatterResult;
	/** Legacy adapter for {@link CheckmarkFormatter}. @param _row - Unused row index. @param _cell - Unused cell index. @param value - Truthy/falsy cell value. @returns Checkmark icon or empty string. */
	function Checkmark(_row: number, _cell: number, value: any): FormatterResult;
}
declare abstract class BaseCellEdit {
	protected _input: HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement;
	protected _defaultValue: any;
	protected _args: EditorOptions;
	constructor(args: EditorOptions);
	/** Creates and attaches the editor DOM; called by the constructor. */
	abstract init(): void;
	/** Removes the editor input from the DOM. */
	destroy(): void;
	/** Focuses the editor's input element. */
	focus(): void;
	/**
	 * Reads the current input value as a string.
	 * @returns Raw string value from the input.
	 */
	getValue(): string;
	/**
	 * Writes a string value into the input.
	 * @param val - Value to set; `null`/`undefined` becomes empty string.
	 */
	setValue(val: string): void;
	/**
	 * Loads the item's field value into the editor and selects it.
	 * @param item - Row data item whose field is being edited.
	 */
	loadValue(item: any): void;
	/**
	 * Serializes the current input value for commit.
	 * @returns String content of the input.
	 */
	serializeValue(): any;
	/**
	 * Writes the serialized value back to the data item's field.
	 * @param item - Row data item to mutate.
	 * @param state - Value returned by {@link BaseCellEdit.serializeValue}.
	 */
	applyValue(item: any, state: any): void;
	/**
	 * Tests whether the editor content differs from the loaded default.
	 * @returns `true` if the value has changed.
	 */
	isValueChanged(): boolean;
	/**
	 * Validates the current value using the column's `validator`, if any.
	 * @returns Validation result; always valid when no validator is configured.
	 */
	validate(): ValidationResult;
}
/**
 * Text editor backed by an `<input type="text">`. Honors `editorCellNavOnLRKeys`
 * for arrow-key navigation between cells.
 */
export declare class TextCellEdit extends BaseCellEdit {
	_input: HTMLInputElement;
	init(): void;
}
/**
 * Integer editor extending {@link TextCellEdit}. Serializes to `number` and
 * validates that the input is a valid integer.
 */
export declare class IntegerCellEdit extends TextCellEdit {
	serializeValue(): any;
	validate(): ValidationResult;
}
/**
 * Float/decimal editor extending {@link TextCellEdit}. Supports fixed decimal
 * places via `column.editorFixedDecimalPlaces` or {@link FloatCellEdit.DefaultDecimalPlaces}.
 */
export declare class FloatCellEdit extends TextCellEdit {
	/** When `true`, empty input serializes to empty string rather than `0`. */
	static AllowEmptyValue: boolean;
	/** Default number of fixed decimal places when the column does not specify `editorFixedDecimalPlaces`. `null` means no rounding. */
	static DefaultDecimalPlaces: number;
	/**
	 * Resolves the number of fixed decimal places to use.
	 * @returns Number of places, or `null` when none is configured.
	 */
	getDecimalPlaces(): number;
	loadValue(item: any): void;
	serializeValue(): any;
	validate(): ValidationResult;
}
/**
 * Date editor extending {@link TextCellEdit} with a jQuery UI datepicker.
 * Manages calendar open/close and repositions via {@link DateCellEdit.position}.
 */
export declare class DateCellEdit extends TextCellEdit {
	private _calendarOpen;
	init(): void;
	destroy(): void;
	show(): void;
	hide(): void;
	position(position: Position): void;
}
/**
 * Two-option select editor mapping `"yes"`/`"no"` to boolean `true`/`false`.
 */
export declare class YesNoSelectCellEdit extends BaseCellEdit {
	_input: HTMLSelectElement;
	init(): void;
	loadValue(item: any): void;
	serializeValue(): any;
	isValueChanged(): boolean;
	validate(): ValidationResult;
}
/**
 * Checkbox editor backed by `<input type="checkbox">`. Supports `preClick` to
 * toggle on the click that activates the editor.
 */
export declare class CheckboxCellEdit extends BaseCellEdit {
	_input: HTMLInputElement;
	init(): void;
	loadValue(item: any): void;
	preClick(): void;
	serializeValue(): any;
	applyValue(item: any, state: any): void;
	isValueChanged(): boolean;
	validate(): {
		valid: boolean;
		msg: string;
	};
}
/**
 * Percent-complete editor combining {@link IntegerCellEdit} with a vertical
 * jQuery UI slider and preset buttons (0/50/100%).
 */
export declare class PercentCompleteCellEdit extends IntegerCellEdit {
	protected _picker: HTMLDivElement;
	protected _slider: HTMLDivElement;
	init(): void;
	loadValue(item: any): void;
	destroy(): void;
}
/**
 * Detached multi-line text editor using a floating `<textarea>` overlay.
 * Renders attached to `document.body` (or inline for composite editors) and
 * implements `show`/`hide`/`position` for the overlay lifecycle.
 */
export declare class LongTextCellEdit extends BaseCellEdit {
	_input: HTMLTextAreaElement;
	protected _container: HTMLElement;
	protected _wrapper: HTMLDivElement;
	init(): void;
	/**
	 * Handles overlay-specific keys: Ctrl+Enter to save, Esc to cancel, Tab/Shift+Tab
	 * to navigate cells, and optionally Left/Right to navigate when at string bounds.
	 * @param e - Keyboard event from the textarea.
	 */
	handleKeyDown(e: KeyboardEvent): void;
	/** Commits the current textarea value via the grid. */
	save(): void;
	/**
	 * Cancels editing, restoring the default value and notifying the grid.
	 */
	cancel(): void;
	/** Hides the detached overlay wrapper. */
	hide(): void;
	/** Shows the detached overlay wrapper. */
	show(): void;
	/**
	 * Positions the detached overlay relative to the cell bounds.
	 * @param position - Pixel bounds of the target cell.
	 */
	position(position: Position): void;
	destroy(): void;
}
/**
 * Legacy namespace providing stable aliases for cell editors.
 * Prefers named imports from `"./editors"` when possible.
 */
export declare namespace Editors {
	/** Legacy alias for {@link TextCellEdit}. */
	const Text: typeof TextCellEdit;
	/** Legacy alias for {@link IntegerCellEdit}. */
	const Integer: typeof IntegerCellEdit;
	/** Legacy alias for {@link FloatCellEdit}. */
	const Float: typeof FloatCellEdit;
	/** Legacy alias for {@link DateCellEdit}. */
	const Date: typeof DateCellEdit;
	/** Legacy alias for {@link YesNoSelectCellEdit}. */
	const YesNoSelect: typeof YesNoSelectCellEdit;
	/** Legacy alias for {@link CheckboxCellEdit}. */
	const Checkbox: typeof CheckboxCellEdit;
	/** Legacy alias for {@link PercentCompleteCellEdit}. */
	const PercentComplete: typeof PercentCompleteCellEdit;
	/** Legacy alias for {@link LongTextCellEdit}. */
	const LongText: typeof LongTextCellEdit;
}
/**
 * Options controlling how {@link GroupItemMetadataProvider} renders group and totals rows.
 */
export interface GroupItemMetadataProviderOptions {
	/** Whether group rows show an expand/collapse toggle and respond to clicks/keys. Defaults to `true`. */
	enableExpandCollapse?: boolean;
	/** CSS class applied to the group cell (the spanned cell). Defaults to `"slick-group-cell"`. */
	groupCellCssClass?: string;
	/** CSS class applied to the entire group row. Defaults to `"slick-group"`. */
	groupCssClass?: string;
	/** Indentation in pixels per grouping level for the toggle. Defaults to `15`. */
	groupIndentation?: number;
	/** Whether group rows can receive focus. Defaults to `true`. */
	groupFocusable?: boolean;
	/** Modern formatter for the group title/aggregated content. */
	groupFormat?: ColumnFormat<Group>;
	/**
	 * Legacy formatter for group rows.
	 * @deprecated Use {@link GroupItemMetadataProviderOptions.groupFormat} instead.
	 */
	groupFormatter?: CompatFormatter<Group>;
	/** CSS class prefix for grouping level (appended with level number). Defaults to `"slick-group-level-"`. */
	groupLevelPrefix?: string;
	/** Whether totals rows should be considered part of the group row span calculation. */
	groupRowTotals?: boolean;
	/** CSS class applied to the title span inside the group cell. Defaults to `"slick-group-title"`. */
	groupTitleCssClass?: string;
	/**
	 * Predicate determining whether a column has a summary/aggregate.
	 * Used to locate the spanned group cell position.
	 * @param column - Column to test.
	 * @returns `true` if the column contributes a total/summary.
	 */
	hasSummaryType?: (column: Column) => boolean;
	/** CSS class for the expand/collapse toggle element. Defaults to `"slick-group-toggle"`. */
	toggleCssClass?: string;
	/** CSS class added when the toggle represents an expanded group. Defaults to `"expanded"`. */
	toggleExpandedCssClass?: string;
	/** CSS class added when the toggle represents a collapsed group. Defaults to `"collapsed"`. */
	toggleCollapsedCssClass?: string;
	/** CSS class applied to totals rows. Defaults to `"slick-group-totals"`. */
	totalsCssClass?: string;
	/** Whether totals rows can receive focus. Defaults to `false`. */
	totalsFocusable?: boolean;
	/** Modern formatter for totals rows. */
	totalsFormat?: ColumnFormat<IGroupTotals>;
	/**
	 * Legacy formatter for totals rows.
	 * @deprecated Use {@link GroupItemMetadataProviderOptions.totalsFormat} instead.
	 */
	totalsFormatter?: CompatFormatter<IGroupTotals>;
}
/**
 * Grid plugin that provides row metadata and formatters for group headers and
 * group totals rows. Handles expand/collapse UI via click and keyboard
 * (Space, `+`, `-`) and delegates metadata through `getGroupRowMetadata` /
 * `getTotalsRowMetadata` for use by `DataView`.
 */
export declare class GroupItemMetadataProvider implements GridPlugin {
	/** Host grid instance set during {@link GroupItemMetadataProvider.init}. */
	protected grid: ISleekGrid;
	/** Resolved options merged with {@link GroupItemMetadataProvider.defaults}. */
	private options;
	/**
	 * Creates a new provider.
	 * @param opt - Partial options merged with {@link GroupItemMetadataProvider.defaults}.
	 */
	constructor(opt?: GroupItemMetadataProviderOptions);
	/**
	 * Default option values. Override per instance via constructor or {@link GroupItemMetadataProvider.setOptions}.
	 */
	static readonly defaults: GroupItemMetadataProviderOptions;
	/**
	 * Default group row formatter. Renders the group title with an optional
	 * expand/collapse toggle indented by `group.level`.
	 * @param ctx - Formatter context whose `item` is the {@link Group} to render.
	 * @param opt - Options controlling indentation and toggle classes; defaults to {@link GroupItemMetadataProvider.defaults}.
	 * @returns Rendered group row content as DOM/JSX.
	 */
	static defaultGroupFormat(ctx: FormatterContext, opt?: GroupItemMetadataProviderOptions): FormatterResult;
	/**
	 * Default totals row formatter. Delegates to the grid's column totals formatter
	 * (or the column's own `groupTotalsFormat`/`groupTotalsFormatter`).
	 * @param ctx - Formatter context whose `item` is the {@link IGroupTotals} row.
	 * @param grid - Optional grid fallback when `ctx.grid` is unavailable.
	 * @returns Rendered totals content, or empty string when no formatter is found.
	 */
	static defaultTotalsFormat(ctx: FormatterContext, grid?: ISleekGrid): FormatterResult;
	/**
	 * Initializes the plugin, attaching click and key handlers for expand/collapse.
	 * @param grid - Host grid instance.
	 */
	init(grid: ISleekGrid): void;
	/** Plugin name used for lookup via `grid.getPluginByName()`. */
	readonly pluginName = "GroupItemMetadataProvider";
	/**
	 * Detaches event handlers added during {@link GroupItemMetadataProvider.init}.
	 */
	destroy(): void;
	/**
	 * Returns the current resolved options.
	 * @returns Current options object.
	 */
	getOptions(): GroupItemMetadataProviderOptions;
	/**
	 * Merges the given values into the current options.
	 * @param value - Partial options to apply.
	 */
	setOptions(value: GroupItemMetadataProviderOptions): void;
	/**
	 * Click handler that toggles group collapse when the toggle element is clicked.
	 * @param e - Cell mouse event from the grid's `onClick`.
	 */
	handleGridClick: (e: CellMouseEvent) => void;
	/**
	 * Key handler that toggles group collapse on Space / `+` / `-` when a group row is active.
	 * @param e - Cell keyboard event from the grid's `onKeyDown`.
	 */
	handleGridKeyDown: (e: CellKeyboardEvent) => void;
	/**
	 * Computes the cell index and colspan for the spanned group cell, taking
	 * summary columns and frozen columns into account.
	 * @returns Object with `cell` start index and `colspan` span width (`"*"` means full row when no totals).
	 */
	groupCellPosition: () => {
		cell: number;
		colspan: (number | "*");
	};
	/**
	 * Returns row metadata for a group header row. The grid/DataView calls this
	 * to obtain CSS classes, focusability and the spanned column formatter.
	 * @param item - Group row item.
	 * @returns Metadata describing how the group row should be rendered.
	 */
	getGroupRowMetadata: ((item: Group) => ItemMetadata);
	/**
	 * Returns row metadata for a group totals row.
	 * @param item - Totals row item.
	 * @returns Metadata describing how the totals row should be rendered.
	 */
	getTotalsRowMetadata: ((item: IGroupTotals) => ItemMetadata);
}
/**
 * Options for {@link AutoTooltips}.
 */
export interface AutoTooltipsOptions {
	/** Auto-assign tooltips for body cells when text overflows. Defaults to `true`. */
	enableForCells?: boolean;
	/** Auto-assign tooltips for header cells when text overflows. Defaults to `false`. */
	enableForHeaderCells?: boolean;
	/** Maximum tooltip length before truncation with `"..."`; `null` means no limit. */
	maxToolTipLength?: number;
	/** When `true`, overwrites existing `title` attributes. */
	replaceExisting?: boolean;
}
/**
 * Grid plugin that automatically sets `title` tooltips for truncated cell content.
 * Handles overflow detection via `clientWidth < scrollWidth` and optional truncation.
 */
export declare class AutoTooltips implements GridPlugin {
	/** Host grid set during {@link AutoTooltips.init}. */
	private grid;
	/** Resolved options merged with {@link AutoTooltips.defaults}. */
	private options;
	/**
	 * Creates the plugin.
	 * @param options - Partial options merged with {@link AutoTooltips.defaults}.
	 */
	constructor(options?: AutoTooltipsOptions);
	/** Default option values. */
	static readonly defaults: AutoTooltipsOptions;
	/**
	 * Attaches overflow-tooltip handlers based on current options.
	 * @param grid - Host grid instance.
	 */
	init(grid: ISleekGrid): void;
	/**
	 * Detaches handlers installed by {@link AutoTooltips.init}.
	 */
	destroy(): void;
	private handleMouseEnter;
	private handleHeaderMouseEnter;
	/** Plugin name for lookup via `grid.getPluginByName()`. */
	pluginName: string;
}
/**
 * Options for {@link RowMoveManager}.
 */
export interface RowMoveManagerOptions {
	/** When `true`, cancels the active cell edit when a drag starts. */
	cancelEditOnDrag?: boolean;
}
/**
 * Payload for row-move events ({@link RowMoveManager.onBeforeMoveRows} / {@link RowMoveManager.onMoveRows}).
 */
export interface ArgsMoveRows {
	/** Data rows being moved (view indices, in display order). */
	rows: number[];
	/** Insertion index before which the rows should be placed. */
	insertBefore: number;
}
/**
 * Drag-and-drop plugin that lets users reorder rows via a proxy and guide.
 * Works only when the target column `behavior` is `"move"` or `"selectAndMove"`.
 * Emits {@link RowMoveManager.onBeforeMoveRows} (cancelable) and {@link RowMoveManager.onMoveRows}.
 */
export declare class RowMoveManager implements GridPlugin {
	/** Host grid set during {@link RowMoveManager.init}. */
	private grid;
	/** Resolved options merged with {@link RowMoveManager.defaults}. */
	private options;
	/** True while a drag is in progress. */
	private dragging;
	private handler;
	/** Fired before the drop position is accepted; handlers may return `false` to reject the insertion point. */
	onBeforeMoveRows: EventEmitter<ArgsMoveRows>;
	/** Fired on successful drop; subscribers should reorder data accordingly. */
	onMoveRows: EventEmitter<ArgsMoveRows>;
	/**
	 * Creates the manager.
	 * @param options - Partial options merged with {@link RowMoveManager.defaults}.
	 */
	constructor(options?: RowMoveManagerOptions);
	/** Default option values. */
	static readonly defaults: RowMoveManagerOptions;
	/**
	 * Subscribes to the grid's drag lifecycle to implement row moving.
	 * @param grid - Host grid instance.
	 */
	init(grid: ISleekGrid): void;
	/**
	 * Unsubscribes all grid drag handlers.
	 */
	destroy(): void;
	private handleDragInit;
	private handleDragStart;
	private handleDrag;
	private handleDragEnd;
}
/**
 * Options for {@link RowSelectionModel}.
 */
export interface RowSelectionModelOptions {
	/** When `true`, moving the active cell also selects the new row. Defaults to `true`. */
	selectActiveRow?: boolean;
}
/**
 * Selection model that treats selection as whole rows (full-width ranges).
 * Supports ActiveCell-driven selection, Shift+Up/Down range extension and
 * Ctrl/Meta/Shift-click row toggling. Implements {@link SelectionModel}.
 */
export declare class RowSelectionModel implements GridPlugin, SelectionModel {
	/** Host grid set during {@link RowSelectionModel.init}. */
	private grid;
	private handler;
	/** Resolved options merged with {@link RowSelectionModel.defaults}. */
	private options;
	/** Internal full-width ranges representing selected rows. */
	private ranges;
	/** Emits when selected ranges change; used by the grid to update UI state. */
	onSelectedRangesChanged: EventEmitter<CellRange[]>;
	/**
	 * Creates the selection model.
	 * @param options - Partial options merged with {@link RowSelectionModel.defaults}.
	 */
	constructor(options?: RowSelectionModelOptions);
	/** Default option values. */
	static readonly defaults: RowSelectionModelOptions;
	/**
	 * Attaches to `onActiveCellChanged`, `onKeyDown` and `onClick` on `grid`.
	 * @param grid - Host grid instance.
	 */
	init(grid: ISleekGrid): void;
	/**
	 * Unsubscribes handlers installed by {@link RowSelectionModel.init}.
	 */
	destroy(): void;
	private wrapHandler;
	private rowsToRanges;
	/**
	 * Returns selected view row indices derived from the internal ranges.
	 * @returns Array of selected row indices.
	 */
	getSelectedRows(): number[];
	/**
	 * Sets selection from a list of row indices.
	 * @param rows - Row indices to select.
	 */
	setSelectedRows(rows: number[]): void;
	/**
	 * Sets selection from explicit ranges (each range should span the full row width).
	 * @param ranges - Cell ranges representing row selection.
	 */
	setSelectedRanges(ranges: CellRange[]): void;
	/**
	 * Returns the current selection as full-width {@link CellRange} objects.
	 * @returns Current selected ranges.
	 */
	getSelectedRanges(): CellRange[];
	private handleActiveCellChange;
	private handleKeyDown;
	private handleClick;
}

export {
	SleekGrid as Grid,
};

export {};
