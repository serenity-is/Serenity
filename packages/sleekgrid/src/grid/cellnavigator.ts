import { GoToResult } from "./internal";

export interface CellNavigatorHost {
    getColumnCount(): number;
    getRowCount(): number;
    getColspan(row: number, cell: number): number;
    canCellBeActive(row: number, cell: number): boolean;
    setTabbingDirection(dir: number): void;
    isRTL(): boolean;
}

export class CellNavigator {

    declare private host: CellNavigatorHost;

    constructor(h: CellNavigatorHost) {
        this.host = h;
    }

    private findFirstFocusableCell(row: number): number {
        var cell = 0;
        var cols = this.host.getColumnCount();
        while (cell < cols) {
            if (this.host.canCellBeActive(row, cell)) {
                return cell;
            }
            cell += this.host.getColspan(row, cell);
        }
        return null;
    }

    private findLastFocusableCell(row: number): number {
        var cell = 0;
        var lastFocusableCell = null;
        var cols = this.host.getColumnCount();
        while (cell < cols) {
            if (this.host.canCellBeActive(row, cell)) {
                lastFocusableCell = cell;
            }
            cell += this.host.getColspan(row, cell);
        }
        return lastFocusableCell;
    }


    private gotoRight(row?: number, cell?: number): GoToResult {
        var cols = this.host.getColumnCount();
        if (cell >= cols) {
            return null;
        }

        do {
            cell += this.host.getColspan(row, cell);
        }
        while (cell < cols && !this.host.canCellBeActive(row, cell));

        if (cell < cols) {
            return {
                row: row,
                cell: cell,
                posX: cell
            };
        }
        return null;
    }

    private gotoLeft(row?: number, cell?: number): GoToResult {
        if (cell <= 0) {
            return null;
        }

        var firstFocusableCell = this.findFirstFocusableCell(row);
        if (firstFocusableCell === null || firstFocusableCell >= cell) {
            return null;
        }

        var prev = {
            row: row,
            cell: firstFocusableCell,
            posX: firstFocusableCell
        };
        var pos;
        while (true) {
            pos = this.gotoRight(prev.row, prev.cell);
            if (!pos) {
                return null;
            }
            if (pos.cell >= cell) {
                return prev;
            }
            prev = pos;
        }
    }

    private gotoDown(row?: number, cell?: number, posX?: number): GoToResult {
        var prevCell;
        var rowCount = this.host.getRowCount();
        while (true) {
            if (++row >= rowCount) {
                return null;
            }

            prevCell = cell = 0;
            while (cell <= posX) {
                prevCell = cell;
                cell += this.host.getColspan(row, cell);
            }

            if (this.host.canCellBeActive(row, prevCell)) {
                return {
                    row: row,
                    cell: prevCell,
                    posX: posX
                };
            }
        }
    }

    private gotoUp(row?: number, cell?: number, posX?: number): GoToResult {
        var prevCell;
        while (true) {
            if (--row < 0) {
                return null;
            }

            prevCell = cell = 0;
            while (cell <= posX) {
                prevCell = cell;
                cell += this.host.getColspan(row, cell);
            }

            if (this.host.canCellBeActive(row, prevCell)) {
                return {
                    row: row,
                    cell: prevCell,
                    posX: posX
                };
            }
        }
    }

    private gotoNext(row?: number, cell?: number, posX?: number): GoToResult {
        if (row == null && cell == null) {
            row = cell = posX = 0;
            if (this.host.canCellBeActive(row, cell)) {
                return {
                    row: row,
                    cell: cell,
                    posX: cell
                };
            }
        }

        var pos = this.gotoRight(row, cell);
        if (pos) {
            return pos;
        }

        var firstFocusableCell = null;
        var dataLengthIncludingAddNew = this.host.getRowCount();
        while (++row < dataLengthIncludingAddNew) {
            firstFocusableCell = this.findFirstFocusableCell(row);
            if (firstFocusableCell != null) {
                return {
                    row: row,
                    cell: firstFocusableCell,
                    posX: firstFocusableCell
                };
            }
        }
        return null;
    }

    private gotoPrev(row?: number, cell?: number, posX?: number): { row: number; cell: number; posX: number; } {
        var cols = this.host.getColumnCount();
        if (row == null && cell == null) {
            row = this.host.getRowCount() - 1;
            cell = posX = cols - 1;
            if (this.host.canCellBeActive(row, cell)) {
                return {
                    row: row,
                    cell: cell,
                    posX: cell
                };
            }
        }

        var pos;
        var lastSelectableCell;
        while (!pos) {
            pos = this.gotoLeft(row, cell);
            if (pos) {
                break;
            }
            if (--row < 0) {
                return null;
            }

            cell = 0;
            lastSelectableCell = this.findLastFocusableCell(row);
            if (lastSelectableCell != null) {
                pos = {
                    row: row,
                    cell: lastSelectableCell,
                    posX: lastSelectableCell
                };
            }
        }
        return pos;
    }

    private gotoRowStart(row: number) {
        var newCell = this.findFirstFocusableCell(row);
        if (newCell === null)
            return null;

        return {
            row: row,
            cell: newCell,
            posX: newCell
        };
    }

    private gotoRowEnd(row: number) {
        var newCell = this.findLastFocusableCell(row);
        if (newCell === null)
            return null;

        return {
            row: row,
            cell: newCell,
            posX: newCell
        };
    }

    /**
     * @param {string} dir Navigation direction.
     * @return {boolean} Whether navigation resulted in a change of active cell.
     */
    navigate(dir: string, activeRow: number, activeCell: number, activePosX: number): GoToResult {
        var tabbingDirections: Record<string, number> = {
            up: -1,
            down: 1,
            prev: -1,
            next: 1,
            home: -1,
            end: 1
        };

        const rtl = this.host.isRTL();
        tabbingDirections[rtl ? 'right' : 'left'] = -1;
        tabbingDirections[rtl ? 'left' : 'right'] = 1;

        this.host.setTabbingDirection(tabbingDirections[dir]);

        var stepFunctions: Record<string, Function> = {
            up: this.gotoUp,
            down: this.gotoDown,
            prev: this.gotoPrev,
            next: this.gotoNext,
            home: this.gotoRowStart,
            end: this.gotoRowEnd
        };

        stepFunctions[rtl ? 'right' : 'left'] = this.gotoLeft;
        stepFunctions[rtl ? 'left' : 'right'] = this.gotoRight;

        var stepFn = stepFunctions[dir].bind(this);
        return stepFn(activeRow, activeCell, activePosX) as GoToResult;
    }
}
