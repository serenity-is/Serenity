import { CellRange, EventEmitter, EventSubscriber, EventData, type ArgsCell, type ISleekGrid, type GridPlugin, type SelectionModel, type CellEvent } from "../core";

/**
 * Options for {@link RowSelectionModel}.
 */
export interface RowSelectionModelOptions {
    /** When `true`, moving the active cell also selects the new row. Defaults to `true`. */
    selectActiveRow?: boolean;
}

function getRowsRange(from: number, to: number): number[] {
    let i: number, rows: number[] = [];
    if (from <= to) {
        for (i = from; i <= to; i++) {
            rows.push(i);
        }
    } else {
        for (i = to; i <= from; i++) {
            rows.push(i);
        }
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

/**
 * Selection model that treats selection as whole rows (full-width ranges).
 * Supports ActiveCell-driven selection, Shift+Up/Down range extension and
 * Ctrl/Meta/Shift-click row toggling. Implements {@link SelectionModel}.
 */
export class RowSelectionModel implements GridPlugin, SelectionModel {
    /** Host grid set during {@link RowSelectionModel.init}. */
    declare private grid: ISleekGrid;
    private handler = new EventSubscriber();
    /** Resolved options merged with {@link RowSelectionModel.defaults}. */
    declare private options: RowSelectionModelOptions;
    /** Internal full-width ranges representing selected rows. */
    private ranges: CellRange[] = [];

    /**
     * The row the selection was last made from, which the arrow keys extend it from when the grid has no
     * active cell to take that place, e.g. a grid without cell navigation.
     */
    private anchorRow: number | null = null;
    /** Emits when selected ranges change; used by the grid to update UI state. */
    onSelectedRangesChanged: EventEmitter<CellRange[]> = new EventEmitter<CellRange[]>();

    /**
     * Creates the selection model.
     * @param options - Partial options merged with {@link RowSelectionModel.defaults}.
     */
    constructor(options?: RowSelectionModelOptions) {
        this.options = Object.assign({}, RowSelectionModel.defaults, options);
    }

    /** Default option values. */
    public static readonly defaults: RowSelectionModelOptions = {
        selectActiveRow: true
    }

    /**
     * Attaches to `onActiveCellChanged`, `onKeyDown` and `onClick` on `grid`.
     * @param grid - Host grid instance.
     */
    init(grid: ISleekGrid): void {
        this.grid = grid;
        this.handler.subscribe(grid.onActiveCellChanged, this.wrapHandler(this.handleActiveCellChange));
        this.handler.subscribe(grid.onKeyDown, this.wrapHandler(this.handleKeyDown));
        this.handler.subscribe(grid.onClick, this.wrapHandler(this.handleClick));
    }

    /**
     * Unsubscribes handlers installed by {@link RowSelectionModel.init}.
     */
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


    /**
     * Returns selected view row indices derived from the internal ranges.
     * @returns Array of selected row indices.
     */
    getSelectedRows(): number[] {
        return rangesToRows(this.ranges);
    }

    /**
     * Sets selection from a list of row indices.
     * @param rows - Row indices to select.
     */
    setSelectedRows(rows: number[]): void {
        this.anchorRow = rows?.length ? rows[rows.length - 1] : null;
        this.setSelectedRanges(this.rowsToRanges(rows));
    }

    /**
     * Sets selection from explicit ranges (each range should span the full row width).
     * @param ranges - Cell ranges representing row selection.
     */
    setSelectedRanges(ranges: CellRange[]): void {
        // simle check for: empty selection didn't change, prevent firing onSelectedRangesChanged
        if ((!this.ranges || this.ranges.length === 0) && (!ranges || ranges.length === 0))
            return;
        this.ranges = ranges;
        this.onSelectedRangesChanged.notify(this.ranges);
    }

    /**
     * Returns the current selection as full-width {@link CellRange} objects.
     * @returns Current selected ranges.
     */
    getSelectedRanges(): CellRange[] {
        return this.ranges;
    }

    private handleActiveCellChange({ row }: CellEvent) {
        if (this.options.selectActiveRow && row != null) {
            this.anchorRow = row;
            this.setSelectedRanges([new CellRange(row, 0, row, this.grid.getColumns().length - 1)]);
        }
    }

    private handleKeyDown(e: KeyboardEvent) {
        const which = (e as any).which;
        if (!(e.shiftKey && !e.ctrlKey && !e.altKey && !e.metaKey && (which == 38 || which == 40)))
            return;

        const selectedRows = this.getSelectedRows();
        selectedRows.sort((x, y) => x - y);

        // the keys move the end of the selection that is at the side of the active cell, and a grid without
        // cell navigation has no active cell, so the selection itself is where they move from
        const activeRow = this.grid.getActiveCell()?.row ?? this.anchorRow;
        if (activeRow == null)
            return;

        if (!selectedRows.length)
            selectedRows.push(activeRow);

        let top = selectedRows[0];
        let bottom = selectedRows[selectedRows.length - 1];
        let active;

        if (which == 40) {
            active = activeRow < bottom || top == bottom ? ++bottom : ++top;
        } else {
            active = activeRow < bottom ? --bottom : --top;
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
        const cell = this.grid.getCellFromEvent(e);
        if (!cell || !this.grid.canCellBeSelected(cell.row, cell.cell)) {
            return false;
        }

        const withModifiers = e.ctrlKey || e.shiftKey || e.metaKey;
        const cellNavigation = this.grid.getOptions().enableCellNavigation;
        const canBeActive = this.grid.canCellBeActive(cell.row, cell.cell);
        const multiSelect = this.grid.getOptions().multiSelect;

        if (!withModifiers) {
            // a grid with cell navigation selects the row of the cell that becomes the active one, which
            // the grid does after this, and a cell that cannot be active leaves the selection alone
            if (cellNavigation)
                return false;

            // a grid without cell navigation has no active cell to follow, so a plain click selects the row
            // it is on, and the model works for a grid that selects rows rather than moving a cursor around
            // a click that selects nothing is left to the other handlers of the grid
            if (!this.options.selectActiveRow)
                return false;

            this.anchorRow = cell.row;
            this.setSelectedRanges(this.rowsToRanges([cell.row]));
            e.stopImmediatePropagation();
            return true;
        }

        // the rows a modified click extends are the ones of the selection, not the one of the active cell,
        // so it works in a grid without cell navigation too, where a cell that cannot be active is only one
        // that cannot be selected, which is checked above
        if (!multiSelect || (cellNavigation && !canBeActive)) {
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

        this.anchorRow = cell.row;
        this.ranges = this.rowsToRanges(selection);
        this.setSelectedRanges(this.ranges);
        e.stopImmediatePropagation();

        return true;
    }
}
