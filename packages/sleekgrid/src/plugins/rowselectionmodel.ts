import { CellRange, EventEmitter, EventSubscriber, EventData, type ArgsCell, type ISleekGrid, type GridPlugin, type SelectionModel, type CellEvent } from "../core";

export interface RowSelectionModelOptions {
    selectActiveRow?: boolean;
}

function getRowsRange(from: number, to: number): number[] {
    let i: number, rows: number[] = [];
    for (i = from; i <= to; i++) {
        rows.push(i);
    }
    for (i = to; i < from; i++) {
        rows.push(i);
    }
    return rows;
}

function rangesToRows(ranges: CellRange[]) {
    let rows = [];
    for (let i = 0; i < ranges.length; i++) {
        for (let j = ranges[i].fromRow; j <= ranges[i].toRow; j++) {
            rows.push(j);
        }
    }
    return rows;
}

export class RowSelectionModel implements GridPlugin, SelectionModel {
    declare private grid: ISleekGrid;
    private handler = new EventSubscriber();
    declare private options: RowSelectionModelOptions;
    declare private ranges: CellRange[];
    onSelectedRangesChanged = new EventEmitter<CellRange[]>();

    constructor(options?: RowSelectionModelOptions) {
        this.options = Object.assign({}, RowSelectionModel.defaults, options);
    }

    public static readonly defaults: RowSelectionModelOptions = {
        selectActiveRow: true
    }

    init(grid: ISleekGrid): void {
        this.grid = grid;
        this.handler.subscribe(grid.onActiveCellChanged, this.wrapHandler(this.handleActiveCellChange));
        this.handler.subscribe(grid.onKeyDown, this.wrapHandler(this.handleKeyDown));
        this.handler.subscribe(grid.onClick, this.wrapHandler(this.handleClick));
    }

    destroy(): void {
        this.handler?.unsubscribeAll();
    }

    private wrapHandler(handler: Function): () => void {
        return (function() {
            if (!this.inHandler) {
                this.inHandler = true;
                handler.apply(this, arguments);
                this.inHandler = false;
            }
        }).bind(this);
    }

    private rowsToRanges(rows: number[]): CellRange[] {
        let ranges = [];
        let lastCell = this.grid.getColumns().length - 1;
        for (let i = 0; i < rows.length; i++) {
            ranges.push(new CellRange(rows[i], 0, rows[i], lastCell));
        }
        return ranges;
    }


    getSelectedRows(): number[] {
        return rangesToRows(this.ranges);
    }

    setSelectedRows(rows: number[]): void {
        this.setSelectedRanges(this.rowsToRanges(rows));
    }

    setSelectedRanges(ranges: CellRange[]): void {
        // simle check for: empty selection didn't change, prevent firing onSelectedRangesChanged
        if ((!this.ranges || this.ranges.length === 0) && (!ranges || ranges.length === 0))
            return;
        this.ranges = ranges;
        this.onSelectedRangesChanged.notify(this.ranges);
    }

    getSelectedRanges(): CellRange[] {
        return this.ranges;
    }

    private handleActiveCellChange({ row }: CellEvent) {
        if (this.options.selectActiveRow && row != null) {
            this.setSelectedRanges([new CellRange(row, 0, row, this.grid.getColumns().length - 1)]);
        }
    }

    private handleKeyDown(e: KeyboardEvent) {
        let activeRow = this.grid.getActiveCell();
        if (!(activeRow && e.shiftKey && !e.ctrlKey && !e.altKey && !e.metaKey && ((e as any).which == 38 || (e as any).which == 40)))
            return;

        let selectedRows = this.getSelectedRows();
        selectedRows.sort(function (x, y) {
            return x - y
        });

        if (!selectedRows.length) {
            selectedRows = [activeRow.row];
        }

        let top = selectedRows[0];
        let bottom = selectedRows[selectedRows.length - 1];
        let active;

        if ((e as any).which == 40) {
            active = activeRow.row < bottom || top == bottom ? ++bottom : ++top;
        } else {
            active = activeRow.row < bottom ? --bottom : --top;
        }

        if (active >= 0 && active < this.grid.getDataLength()) {
            this.grid.scrollRowIntoView(active);
            this.ranges = this.rowsToRanges(getRowsRange(top, bottom));
            this.setSelectedRanges(this.ranges);
        }

        e.preventDefault();
        e.stopPropagation();
    }

    private handleClick(e: MouseEvent) {
        let cell = this.grid.getCellFromEvent(e);
        if (!cell || !this.grid.canCellBeActive(cell.row, cell.cell)) {
            return false;
        }

        if (!this.grid.getOptions().multiSelect || (
            !e.ctrlKey && !e.shiftKey && !e.metaKey)) {
            return false;
        }

        let selection = rangesToRows(this.ranges);
        let idx = selection.indexOf(cell.row);

        if (idx === -1 && (e.ctrlKey || e.metaKey)) {
            selection.push(cell.row);
            this.grid.setActiveCell(cell.row, cell.cell);
        } else if (idx !== -1 && (e.ctrlKey || e.metaKey)) {
            selection = selection.filter(o => {
                return (o !== cell.row);
            });
            this.grid.setActiveCell(cell.row, cell.cell);
        } else if (selection.length && e.shiftKey) {
            let last = selection.pop();
            let from = Math.min(cell.row, last);
            let to = Math.max(cell.row, last);
            selection = [];
            for (let i = from; i <= to; i++) {
                if (i !== last) {
                    selection.push(i);
                }
            }
            selection.push(last);
            this.grid.setActiveCell(cell.row, cell.cell);
        }

        this.ranges = this.rowsToRanges(selection);
        this.setSelectedRanges(this.ranges);
        e.stopImmediatePropagation();

        return true;
    }
}
