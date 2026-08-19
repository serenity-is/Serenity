import { bindThis } from "@serenity-is/domwise";
import { EventEmitter, EventSubscriber, type ISleekGrid, type GridPlugin, type EventData } from "../core";

/**
 * Options for {@link RowMoveManager}.
 */
export interface RowMoveManagerOptions {
    /** When `true`, cancels the active cell edit when a drag starts. */
    cancelEditOnDrag?: boolean;
}

interface RowMoveManagerDragData {
    selectedRows: number[],
    selectionProxy: HTMLDivElement,
    guide: HTMLDivElement,
    insertBefore: number,
    canMove: boolean
}

/**
 * Payload for row-move events ({@link RowMoveManager.onBeforeMoveRows} / {@link RowMoveManager.onMoveRows}).
 */
export interface ArgsMoveRows {
    /** Data rows being moved (view indices, in display order). */
    rows: number[];
    /** Insertion index before which the rows should be placed. */
    insertBefore: number;
}

/**
 * Drag-and-drop plugin that lets users reorder rows via a proxy and guide.
 * Works only when the target column `behavior` is `"move"` or `"selectAndMove"`.
 * Emits {@link RowMoveManager.onBeforeMoveRows} (cancelable) and {@link RowMoveManager.onMoveRows}.
 */
export class RowMoveManager implements GridPlugin {
    /** Host grid set during {@link RowMoveManager.init}. */
    declare private grid: ISleekGrid;
    /** Resolved options merged with {@link RowMoveManager.defaults}. */
    declare private options: RowMoveManagerOptions;
    /** True while a drag is in progress. */
    declare private dragging: boolean;
    private handler = new EventSubscriber();
    /** Fired before the drop position is accepted; handlers may return `false` to reject the insertion point. */
    onBeforeMoveRows: EventEmitter<ArgsMoveRows> = new EventEmitter<ArgsMoveRows>();
    /** Fired on successful drop; subscribers should reorder data accordingly. */
    onMoveRows: EventEmitter<ArgsMoveRows> = new EventEmitter<ArgsMoveRows>();

    /**
     * Creates the manager.
     * @param options - Partial options merged with {@link RowMoveManager.defaults}.
     */
    constructor(options?: RowMoveManagerOptions) {
        this.options = Object.assign({}, RowMoveManager.defaults, options);
    }

    /** Default option values. */
    public static readonly defaults: RowMoveManagerOptions = {
        cancelEditOnDrag: false
    }

    /**
     * Subscribes to the grid's drag lifecycle to implement row moving.
     * @param grid - Host grid instance.
     */
    init(grid: ISleekGrid): void {
        this.grid = grid;
        const boundThis = bindThis(this);
        this.handler.subscribe(grid.onDragInit, boundThis.handleDragInit)
            .subscribe(grid.onDragStart, boundThis.handleDragStart)
            .subscribe(grid.onDrag, boundThis.handleDrag)
            .subscribe(grid.onDragEnd, boundThis.handleDragEnd);
    }

    /**
     * Unsubscribes all grid drag handlers.
     */
    destroy(): void {
        this.handler?.unsubscribeAll();
    }

    private handleDragInit(e: EventData<{}, UIEvent>) {
        // prevent the grid from cancelling drag'n'drop by default
        e.stopImmediatePropagation();
    }

    private handleDragStart(e: EventData<{}, UIEvent>, dd: RowMoveManagerDragData) {
        let cell = this.grid.getCellFromEvent(e);

        if (this.options.cancelEditOnDrag && this.grid.getEditorLock().isActive()) {
            this.grid.getEditorLock().cancelCurrentEdit();
        }

        if (this.grid.getEditorLock().isActive() ||
            !/move|selectAndMove/.test(this.grid.getColumns()[cell.cell].behavior)) {
            return false;
        }

        this.dragging = true;
        e.stopImmediatePropagation();

        let selectedRows = this.grid.getSelectedRows();

        if (selectedRows.length == 0 || selectedRows.indexOf(cell.row) == -1) {
            selectedRows = [cell.row];
            this.grid.setSelectedRows(selectedRows);
        }

        let rowHeight = this.grid.getOptions().rowHeight;

        dd.selectedRows = selectedRows;
        let canvas = this.grid.getCanvasNode();
        const sp = dd.selectionProxy = canvas.appendChild(document.createElement('div'));
        sp.style = `position: absolute; z-index: 9999; width: ${canvas.clientWidth}px; height: ${rowHeight * selectedRows.length}px`;
        sp.className = 'slick-row-move-proxy';
        const g = dd.guide = canvas.appendChild(document.createElement('div'));
        g.style = `position: absolute; z-index: 9998; width: ${canvas.clientWidth}px; height: ${rowHeight}px; top: -1000px`;
        g.className = 'slick-row-move-guide';

        dd.insertBefore = -1;
    }

    private handleDrag(e: EventData<{}, UIEvent>, dd: RowMoveManagerDragData) {
        if (!this.dragging)
            return;

        e.stopImmediatePropagation();
        let canvas = this.grid.getCanvasNode();

        let box = canvas.getBoundingClientRect();
        let docElem = document.documentElement;
        let canvasTop = box.top + window.scrollY - docElem.clientTop;

        let top = (e as any).pageY - canvasTop;
        dd.selectionProxy.style.top = (top - 5) + 'px';

        let insertBefore = Math.max(0, Math.min(Math.round(top / this.grid.getOptions().rowHeight), this.grid.getDataLength()));
        if (insertBefore !== dd.insertBefore) {
            let sgEvent = {
                rows: dd.selectedRows,
                insertBefore: insertBefore
            };

            if (this.onBeforeMoveRows.notify(sgEvent).getReturnValue() === false) {
                dd.guide.style.top = "-1000px";
                dd.canMove = false;
            } else {
                dd.guide.style.top = (insertBefore * this.grid.getOptions().rowHeight) + 'px';
                dd.canMove = true;
            }

            dd.insertBefore = insertBefore;
        }
    }

    private handleDragEnd(e: EventData<{}, UIEvent>, dd: RowMoveManagerDragData) {
        if (!this.dragging)
            return;

        this.dragging = false;
        e.stopImmediatePropagation();

        dd.guide.remove();
        dd.selectionProxy.remove();

        if (dd.canMove) {
            let eventData = {
                rows: dd.selectedRows,
                insertBefore: dd.insertBefore
            };
            // TODO: _grid.remapCellCssClasses ?
            this.onMoveRows.notify(eventData);
        }
    }
}
