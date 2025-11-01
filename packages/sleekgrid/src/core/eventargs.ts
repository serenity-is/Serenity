import type { CellStylesHash, Column, Editor, IEventData, ValidationResult } from ".";
import type { DragItem, DragPosition } from "./draggable";
import type { IGrid } from "./igrid";

export interface ArgsGrid {
    grid: IGrid;
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

export type CellEvent = IEventData & ArgsCell;
export type CellKeyboardEvent = KeyboardEvent & ArgsCell;
export type CellMouseEvent = MouseEvent & ArgsCell;
export type HeaderColumnEvent = IEventData & ArgsColumn;
export type HeaderMouseEvent = MouseEvent & ArgsColumn;
export type HeaderRenderEvent = IEventData & ArgsColumnNode;
export type FooterColumnEvent = HeaderColumnEvent;
export type FooterMouseEvent = HeaderMouseEvent;
export type FooterRenderEvent = HeaderRenderEvent;
export type GridEvent = IEventData & ArgsGrid;
export type GridDragEvent = UIEvent & { dragData: DragData };
export type GridMouseEvent = MouseEvent & ArgsGrid;


