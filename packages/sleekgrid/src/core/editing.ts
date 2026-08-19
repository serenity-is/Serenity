import { Column, ColumnMetadata } from "./column";
import { EventEmitter, EventData } from "./event";

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
    commitChanges?: () => void,
    /** Callback to cancel pending editor changes. */
    cancelChanges?: () => void
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
    new(options: EditorOptions): Editor;
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
    isValueChanged(args: { commitEdit?: boolean }): boolean;
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
export class EditorLock {
    declare private activeEditController: EditController;

    /**
     * Checks whether an edit controller currently holds the edit lock.
     * @param editController - Controller to test; when omitted, returns `true` if *any* controller is active.
     * @returns Whether the given (or any) controller is active.
     */
    isActive(editController?: EditController): boolean {
        return (editController ? this.activeEditController === editController : this.activeEditController != null);
    }

    /**
     * Acquires the edit lock for the given controller.
     * Throws if another controller already holds the lock or if the controller
     * does not implement the required methods.
     * @param editController - Controller acquiring the lock.
     */
    activate(editController: EditController): void {
        if (editController === this.activeEditController) { // already activated?
            return;
        }
        if (this.activeEditController != null) {
            throw "SleekGrid.EditorLock.activate: an editController is still active, can't activate another editController";
        }
        if (!editController.commitCurrentEdit) {
            throw "SleekGrid.EditorLock.activate: editController must implement .commitCurrentEdit()";
        }
        if (!editController.cancelCurrentEdit) {
            throw "SleekGrid.EditorLock.activate: editController must implement .cancelCurrentEdit()";
        }
        this.activeEditController = editController;
    }

    /**
     * Releases the edit lock held by the given controller.
     * Throws if the controller is not the currently active one.
     * @param editController - Controller releasing the lock.
     */
    deactivate(editController: EditController): void {
        if (this.activeEditController !== editController) {
            throw "SleekGrid.EditorLock.deactivate: specified editController is not the currently active one";
        }
        this.activeEditController = null;
    }

    /**
     * Attempts to commit the current edit via the active controller.
     * @returns `true` if committed (or no edit was active), `false` if validation failed.
     */
    commitCurrentEdit(): boolean {
        return (this.activeEditController ? this.activeEditController.commitCurrentEdit() : true);
    }

    /**
     * Attempts to cancel the current edit via the active controller.
     * @returns `true` if cancelled (or no edit was active).
     */
    cancelCurrentEdit(): boolean {
        return (this.activeEditController ? this.activeEditController.cancelCurrentEdit() : true);
    }
}

/**
 * Global singleton editor lock instance used by the grid by default.
 * Import this when you need a shared lock outside the grid.
 */
export const GlobalEditorLock: EditorLock = new EditorLock();
