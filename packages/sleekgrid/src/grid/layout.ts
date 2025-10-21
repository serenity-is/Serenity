import type { Signal } from "@serenity-is/signals";
import { RowCell } from "../core";
import { Column } from "../core/column";
import { GridOptions } from "../core/gridoptions";
import { ViewportInfo } from "../core/viewportinfo";
import { ViewRange } from "../core/viewrange";

export interface GridOptionSignals {
    showColumnHeader: Signal<boolean>;
    showHeaderRow: Signal<boolean>;
    showFooterRow: Signal<boolean>;
    showTopPanel: Signal<boolean>;
}

export interface LayoutHost {
    bindAncestorScroll(el: HTMLElement): void;
    cleanUpAndRenderCells(range: ViewRange): void;
    getAvailableWidth(): number;
    getCellFromPoint(x: number, y: number): RowCell;
    getColumnCssRules(idx: number): { right: any; left: any; }
    getColumns(): Column[];
    getInitialColumns(): Column[];
    getContainerNode(): HTMLElement;
    getDataLength(): number;
    getOptions(): GridOptions;
    getOptionSignals(): GridOptionSignals;
    getRowFromNode(rowNode: HTMLElement): number;
    getScrollDims(): { width: number, height: number };
    getScrollLeft(): number;
    getScrollTop(): number;
    getViewportInfo(): ViewportInfo;
    renderRows(range: ViewRange): void;
}

export interface LayoutEngine {
    appendCachedRow(row: number, rowNodeS: HTMLElement, rowNodeC: HTMLElement, rowNodeE: HTMLElement): void;
    afterHeaderColumnDrag(): void;
    afterSetOptions(args: GridOptions): void;
    applyColumnWidths(): void;
    beforeCleanupAndRenderCells(rendered: ViewRange): void;
    afterRenderRows(rendered: ViewRange): void;
    bindAncestorScrollEvents(): void;
    calcCanvasWidth(): number;
    updateHeadersWidth(): void;
    isFrozenRow(row: number): boolean;
    destroy(): void;
    getCanvasNodeFor(cell: number, row: number): HTMLElement;
    getCanvasNodes(): HTMLElement[];
    getCanvasWidth(): number;
    getRowFromCellNode(cellNode: HTMLElement, clientX: number, clientY: number): number;
    getFooterRowCols(): HTMLElement[];
    getFooterRowColsFor(cell: number): HTMLElement;
    getFooterRowColumn(cell: number): HTMLElement;
    getFrozenTopLastRow(): number;
    getFrozenBottomFirstRow(): number;
    getFrozenRowOffset(row: number): number;
    getPinnedStartLastCol(): number;
    getPinnedEndFirstCol(): number;
    getHeaderCols(): HTMLElement[];
    getHeaderColsFor(cell: number): HTMLElement;
    getHeaderColumn(cell: number): HTMLElement;
    getHeaderRowCols(): HTMLElement[];
    getHeaderRowColsFor(cell: number): HTMLElement;
    getHeaderRowColumn(cell: number): HTMLElement;
    getScrollCanvasY(): HTMLElement;
    getScrollContainerX(): HTMLElement;
    getScrollContainerY(): HTMLElement;
    getTopPanelFor(arg0: number): HTMLElement;
    getViewportNodeFor(cell: number, row: number): HTMLElement;
    getViewportNodes(): HTMLElement[];
    handleScrollH(): void;
    handleScrollV(): void;
    init(host: LayoutHost): void;
    layoutName: string;
    realScrollHeightChange(): void;
    /** this might be called before init, chicken egg situation */
    reorderViewColumns(viewCols: Column[], options?: GridOptions): Column[];
    resizeCanvas(): void;
    setPaneVisibility(): void;
    setScroller(): void;
    setOverflow(): void;
    updateCanvasWidth(): boolean;
}
