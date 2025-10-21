import type { CellStylesHash, Column, Editor, ValidationResult } from "../core";
import type { Grid } from "./grid";

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

export interface ArgsSelectedRowsChange extends ArgsGrid {
    rows: number[];
    changedSelectedRows?: number[];
    changedUnselectedRows?: number[];
    previousSelectedRows?: number[];
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
