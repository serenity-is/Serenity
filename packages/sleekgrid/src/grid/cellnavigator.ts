import { bindThis } from "@serenity-is/domwise";
import { GoToResult } from "./internal";

/**
 * Host surface consumed by {@link CellNavigator} to query grid navigability.
 */
export interface CellNavigatorHost {
    /** Returns total column count. */
    getColumnCount(): number;
    /** Returns total row count including the add-new row when enabled. */
    getRowCount(): number;
    /**
     * Returns column span for the cell at the given row/cell.
     * @param row - View row index.
     * @param cell - Cell/column index.
     * @returns Number of columns spanned (at least `1`).
     */
    getColspan(row: number, cell: number): number;
    /**
     * Checks whether a cell may become active/focusable.
     * @param row - Row index.
     * @param cell - Cell/column index.
     * @param tab - When `true`, checks for tab-navigation eligibility.
     * @returns `true` if the cell is navigable.
     */
    canCellBeActive(row: number, cell: number, tab?: boolean): boolean;
    /**
     * Sets the current tabbing direction (e.g. for focus-sink ordering).
     * @param dir - `-1` for backward, `1` for forward.
     */
    setTabbingDirection(dir: number): void;
    /** Whether the grid is rendered right-to-left. */
    isRTL(): boolean;
}

/**
 * Implements keyboard navigation between focusable cells, handling colspans,
 * RTL mirroring and per-direction stepping helpers.
 */
export class CellNavigator {

    /** Host provided at construction. */
    declare private host: CellNavigatorHost;

    /**
     * Creates a navigator bound to `h`.
     * @param h - Host implementing {@link CellNavigatorHost}.
     */
    constructor(h: CellNavigatorHost) {
        this.host = h;
    }

    private findFirstFocusableCell(row: number, tab?: boolean): number {
        var cell = 0;
        var cols = this.host.getColumnCount();
        while (cell < cols) {
            if (this.host.canCellBeActive(row, cell, tab)) {
                return cell;
            }
            cell += this.host.getColspan(row, cell);
        }
        return null;
    }

    private findLastFocusableCell(row: number, tab: boolean): number {
        var cell = 0;
        var lastFocusableCell = null;
        var cols = this.host.getColumnCount();
        while (cell < cols) {
            if (this.host.canCellBeActive(row, cell, tab)) {
                lastFocusableCell = cell;
            }
            cell += this.host.getColspan(row, cell);
        }
        return lastFocusableCell;
    }


    private gotoRight(row?: number, cell?: number, tab?: boolean): GoToResult {
        var cols = this.host.getColumnCount();
        if (cell >= cols) {
            return null;
        }

        do {
            cell += this.host.getColspan(row, cell);
        }
        while (cell < cols && !this.host.canCellBeActive(row, cell, tab));

        if (cell < cols) {
            return {
                row: row,
                cell: cell,
                posX: cell
            };
        }
        return null;
    }

    private gotoLeft(row?: number, cell?: number, tab?: boolean): GoToResult {
        if (cell <= 0) {
            return null;
        }

        var firstFocusableCell = this.findFirstFocusableCell(row, tab);
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
            pos = this.gotoRight(prev.row, prev.cell, tab);
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

            if (this.host.canCellBeActive(row, prevCell, false)) {
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

            if (this.host.canCellBeActive(row, prevCell, false)) {
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
            if (this.host.canCellBeActive(row, cell, true)) {
                return {
                    row: row,
                    cell: cell,
                    posX: cell
                };
            }
        }

        var pos = this.gotoRight(row, cell, true);
        if (pos) {
            return pos;
        }

        var firstFocusableCell = null;
        var dataLengthIncludingAddNew = this.host.getRowCount();
        while (++row < dataLengthIncludingAddNew) {
            firstFocusableCell = this.findFirstFocusableCell(row, true);
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
            if (this.host.canCellBeActive(row, cell, true)) {
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
            pos = this.gotoLeft(row, cell, true);
            if (pos) {
                break;
            }
            if (--row < 0) {
                return null;
            }

            cell = 0;
            lastSelectableCell = this.findLastFocusableCell(row, true);
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
        var newCell = this.findFirstFocusableCell(row, false);
        if (newCell === null)
            return null;

        return {
            row: row,
            cell: newCell,
            posX: newCell
        };
    }

    private gotoRowEnd(row: number) {
        var newCell = this.findLastFocusableCell(row, false);
        if (newCell === null)
            return null;

        return {
            row: row,
            cell: newCell,
            posX: newCell
        };
    }

    /**
     * Navigates the active cell in the given direction.
     * @param dir - Direction key (`"up"`, `"down"`, `"left"`, `"right"`, `"next"`, `"prev"`, `"home"`, `"end"`).
     * @param activeRow - Current active row index.
     * @param activeCell - Current active cell index.
     * @param activePosX - Visual X position used for vertical moves (preserves column alignment).
     * @returns Target navigation result with new row/cell/posX, or `null` when no movement is possible.
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

        const boundThis = bindThis(this);
        var stepFunctions: Record<string, Function> = {
            up: boundThis.gotoUp,
            down: boundThis.gotoDown,
            prev: boundThis.gotoPrev,
            next: boundThis.gotoNext,
            home: boundThis.gotoRowStart,
            end: boundThis.gotoRowEnd
        };

        stepFunctions[rtl ? 'right' : 'left'] = boundThis.gotoLeft;
        stepFunctions[rtl ? 'left' : 'right'] = boundThis.gotoRight;

        var stepFn = stepFunctions[dir];
        return stepFn(activeRow, activeCell, activePosX) as GoToResult;
    }
}
