import { RowCell } from "../core";
import { Column } from "../core/column";
import { GridOptions } from "../core/gridoptions";
import { ViewportInfo } from "../core/viewportinfo";
import { ViewRange } from "../core/viewrange";
import type { GridSignals } from "../core/grid-signals";

export interface LayoutHost {
    cleanUpAndRenderCells(range: ViewRange): void;
    getAvailableWidth(): number;
    getCellFromPoint(x: number, y: number): RowCell;
    getColumns(): Column[];
    getInitialColumns(): Column[];
    getContainerNode(): HTMLElement;
    getDataLength(): number;
    getOptions(): GridOptions;
    getSignals(): GridSignals;
    getRowFromNode(rowNode: HTMLElement): number;
    getScrollDims(): { width: number, height: number };
    getScrollLeft(): number;
    getScrollTop(): number;
    getViewportInfo(): ViewportInfo;
    removeNode(node: HTMLElement): void;
    renderRows(range: ViewRange): void;
}
