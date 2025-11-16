import { bindThis } from "@serenity-is/domwise";
import { EventEmitter, EventSubscriber, type ISleekGrid, type GridPlugin, type EventData } from "../core";

export interface RowMoveManagerOptions {
    cancelEditOnDrag?: boolean;
}

interface RowMoveManagerDragData {
    selectedRows: number[],
    selectionProxy: HTMLDivElement,
    guide: HTMLDivElement,
    insertBefore: number,
    canMove: boolean
}

export interface ArgsMoveRows {
    rows: number[];
    insertBefore: number;
}

export class RowMoveManager implements GridPlugin {
    declare private grid: ISleekGrid;
    declare private options: RowMoveManagerOptions;
    declare private dragging: boolean;
    private handler = new EventSubscriber();
    onBeforeMoveRows = new EventEmitter<ArgsMoveRows>();
    onMoveRows = new EventEmitter<ArgsMoveRows>();

    constructor(options?: RowMoveManagerOptions) {
        this.options = Object.assign({}, RowMoveManager.defaults, options);
    }

    public static readonly defaults: RowMoveManagerOptions = {
        cancelEditOnDrag: false
    }

    init(grid: ISleekGrid) {
        this.grid = grid;
        const boundThis = bindThis(this);
        this.handler.subscribe(grid.onDragInit, boundThis.handleDragInit)
            .subscribe(grid.onDragStart, boundThis.handleDragStart)
            .subscribe(grid.onDrag, boundThis.handleDrag)
            .subscribe(grid.onDragEnd, boundThis.handleDragEnd);
    }

    destroy() {
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
                dd.guide.style.top = "-1000";
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
