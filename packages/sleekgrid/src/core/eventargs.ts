import type { CellStylesHash, Column, Editor, SleekEvent, ValidationResult } from ".";
import type { DragItem } from "./draggable";
import type { ISleekGrid } from "./isleekgrid";

export interface ArgsGrid {
    grid: ISleekGrid;
}

export interface ArgsColumn extends ArgsGrid {
    column: Column;
}

export interface DragData extends DragItem {
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
}

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
    editor: Editor,
    column: Column;
    cellNode: HTMLElement;
    validationResults: ValidationResult;
}

export type CellEvent = SleekEvent<ArgsCell>;
export type CellKeyboardEvent = SleekEvent<ArgsCell, KeyboardEvent>;
export type CellMouseEvent = SleekEvent<ArgsCell, MouseEvent>;
export type HeaderColumnEvent = SleekEvent<ArgsColumn, Event>;
export type HeaderMouseEvent = SleekEvent<ArgsColumn, MouseEvent>;
export type HeaderRenderEvent = SleekEvent<ArgsColumnNode, Event>;
export type FooterColumnEvent = HeaderColumnEvent;
export type FooterMouseEvent = HeaderMouseEvent;
export type FooterRenderEvent = HeaderRenderEvent;
export type GridEvent = SleekEvent<ArgsGrid>;
export type GridDragEvent = SleekEvent<ArgsGrid, UIEvent> & { dragData: DragData };
export type GridMouseEvent = SleekEvent<ArgsGrid, MouseEvent>;


