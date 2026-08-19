/**
 * Represents a rectangular range of cells in the grid.
 * Coordinates are inclusive and automatically normalized so that `from*` is always
 * the top-left corner and `to*` is the bottom-right corner.
 */
export class CellRange {

    /**
     * Top-most row index of the range (inclusive).
     */
    declare public fromRow: number;

    /**
     * Left-most cell/column index of the range (inclusive).
     */
    declare public fromCell: number;

    /**
     * Bottom-most row index of the range (inclusive).
     */
    declare public toRow: number;

    /**
     * Right-most cell/column index of the range (inclusive).
     */
    declare public toCell: number;

    /**
     * Creates a new cell range. When `toRow` / `toCell` are omitted the range
     * represents a single cell at `fromRow` / `fromCell`.
     * @param fromRow - Starting row index.
     * @param fromCell - Starting cell/column index.
     * @param toRow - Ending row index; defaults to `fromRow`.
     * @param toCell - Ending cell index; defaults to `fromCell`.
     */
    constructor(fromRow: number, fromCell: number, toRow?: number, toCell?: number) {
        if (toRow === undefined && toCell === undefined) {
            toRow = fromRow;
            toCell = fromCell;
        }

        this.fromRow = Math.min(fromRow, toRow);
        this.fromCell = Math.min(fromCell, toCell);
        this.toRow = Math.max(fromRow, toRow);
        this.toCell = Math.max(fromCell, toCell);
    }

    /**
     * Returns `true` when the range spans exactly one row.
     * @returns Whether the range covers a single row.
     */
    isSingleRow(): boolean {
        return this.fromRow == this.toRow;
    }

    /**
     * Returns `true` when the range covers exactly one cell.
     * @returns Whether the range is a single cell.
     */
    isSingleCell(): boolean {
        return this.fromRow == this.toRow && this.fromCell == this.toCell;
    }

    /**
     * Tests whether the range contains the given cell.
     * @param row - Row index to test.
     * @param cell - Cell/column index to test.
     * @returns `true` if the cell lies inside the range (inclusive bounds).
     */
    contains(row: number, cell: number): boolean {
        return row >= this.fromRow && row <= this.toRow &&
            cell >= this.fromCell && cell <= this.toCell;
    }

    /**
     * Returns a human-readable representation, e.g. `"(2:3)"` for a single cell
     * or `"(0:0 - 4:5)"` for a multi-cell range.
     * @returns Readable string for debugging/logging.
     */
    toString(): string {
        if (this.isSingleCell()) {
            return "(" + this.fromRow + ":" + this.fromCell + ")";
        }
        else {
            return "(" + this.fromRow + ":" + this.fromCell + " - " + this.toRow + ":" + this.toCell + ")";
        }
    }
}
