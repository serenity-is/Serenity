import { Column } from "../core/column";
import { GridOptions } from "../core/gridoptions";
import { ViewRange } from "../core/viewrange";
import type { LayoutHost } from "./layout-host";
import type { GridLayoutRefs } from "./layout-refs";

export interface LayoutEngine {
    layoutName: string;
    init(host: LayoutHost): void;
    destroy(): void;

    afterHeaderColumnDrag(): void;
    afterRenderRows(rendered: ViewRange): void;
    afterSetOptions(args: GridOptions): void;
    appendCachedRow(row: number, rowNodeS: HTMLElement, rowNodeC: HTMLElement, rowNodeE: HTMLElement): void;
    beforeCleanupAndRenderCells(rendered: ViewRange): void;
    calcCanvasWidth(): number;
    getCanvasNodeFor(cell: number, row: number): HTMLElement;
    getCanvasWidth(): number;
    getFooterRowColsFor(cell: number): HTMLElement;
    getFooterRowColumn(cell: number): HTMLElement;
    getFrozenRowOffset(row: number): number;
    getHeaderColsFor(cell: number): HTMLElement;
    getHeaderColumn(cell: number): HTMLElement;
    getHeaderRowColsFor(cell: number): HTMLElement;
    getHeaderRowColumn(cell: number): HTMLElement;
    getRowFromCellNode(cellNode: HTMLElement, clientX: number, clientY: number): number;
    getRefs(): GridLayoutRefs;
    getViewportNodeFor(cell: number, row: number): HTMLElement;
    isFrozenRow(row: number): boolean;
    realScrollHeightChange(): void;
    /** this might be called before init, chicken egg situation */
    reorderViewColumns(viewCols: Column[], options?: GridOptions): Column[];
    resizeCanvas(): void;
    setOverflow(): void;
    setPaneVisibility(): void;
    setScroller(): void;
    updateCanvasWidth(): boolean;
    updateHeadersWidth(): void;
}
