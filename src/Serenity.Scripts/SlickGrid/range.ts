/*
 * A structure containing a range of cells.
 * @param fromRow {Integer} Starting row.
 * @param fromCell {Integer} Starting cell.
 * @param toRow {Integer} Optional. Ending row. Defaults to <code>fromRow</code>.
 * @param toCell {Integer} Optional. Ending cell. Defaults to <code>fromCell</code>.
 */
export class Range {

    public fromRow: number;
    public fromCell: number;
    public toRow: number;
    public toCell: number;

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

    /***
     * Returns whether a range represents a single row.
     */
    isSingleRow(): boolean {
        return this.fromRow == this.toRow;
    }

    /***
     * Returns whether a range represents a single cell.
     */
    isSingleCell(): boolean {
        return this.fromRow == this.toRow && this.fromCell == this.toCell;
    }

    /***
     * Returns whether a range contains a given cell.
     */
    contains(row: number, cell: number): boolean {
        return row >= this.fromRow && row <= this.toRow &&
            cell >= this.fromCell && cell <= this.toCell;
    }

    /***
     * Returns a readable representation of a range.
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