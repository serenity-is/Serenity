import type { CellStylesHash, Column, Editor, EventData, ValidationResult } from ".";
import type { ISleekGrid } from "./isleekgrid";

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


let a: EventData<ArgsGrid>;


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
}

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
    editor: Editor,
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


