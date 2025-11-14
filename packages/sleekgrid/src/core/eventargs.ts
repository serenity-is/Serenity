import type { CellStylesHash, Column, Editor, EventData, ValidationResult } from ".";
import type { ISleekGrid } from "./isleekgrid";

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

let a: EventData<ArgsGrid>;


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


