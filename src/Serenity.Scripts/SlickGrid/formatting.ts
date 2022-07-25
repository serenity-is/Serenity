import type { Column } from "./column";
import type { Grid } from "./grid";

export interface FormatterFactory<TItem = any> {
    getFormatter(column: Column<TItem>): ColumnFormatter<TItem>;
}

export interface FormatterResult {
    addClass?: string;
    addAttrs?: { [key: string]: string };
    text?: string;
    toolTip?: string;
}

export type ColumnFormatter<TItem = any> = (row: number, cell: number, value: any, column: Column<TItem>, item: TItem, grid?: Grid<TItem>) => string | FormatterResult;
export type AsyncPostRender<TItem = any> = (cellNode: HTMLElement, row: number, item: TItem, column: Column<TItem>, reRender: boolean) => void;
export type AsyncPostCleanup<TItem = any> = (cellNode: HTMLElement, row?: number, column?: Column<TItem>) => void;

export type CellStylesHash = { [row: number]: { [cell: number]: string } }