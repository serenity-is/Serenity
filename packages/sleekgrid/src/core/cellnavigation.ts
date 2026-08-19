/**
 * Discrete directions that the active cell can be moved programmatically.
 * Values map to arrow keys and special keys: `home`/`end` for row boundaries,
 * `next`/`prev` for tab-like sequential navigation, and `up`/`down`/`left`/`right`
 * for arrow-key navigation.
 */
export type CellNavigationDirection = "up" | "down" | "left" | "right" | "next" | "prev" | "home" | "end";

/**
 * Contract for keyboard / programmatic navigation of the active cell.
 * Implemented by the grid so that editors, plugins and external code can move
 * focus without coupling to internal navigation logic.
 */
export interface CellNavigation {
    /**
     * Moves the active cell to the last row of the data set.
     */
    navigateBottom(): void;

    /**
     * Moves the active cell one row down.
     * @returns `true` if the active cell changed, `false` if already at the bottom or blocked.
     */
    navigateDown(): boolean;

    /**
     * Moves the active cell one column to the left.
     * @returns `true` if the active cell changed.
     */
    navigateLeft(): boolean;

    /**
     * Moves the active cell to the next focusable cell (row-major order, wrapping rows).
     * @returns `true` if the active cell changed.
     */
    navigateNext(): boolean;

    /**
     * Scrolls one page down and moves the active cell accordingly.
     */
    navigatePageDown(): void;

    /**
     * Scrolls one page up and moves the active cell accordingly.
     */
    navigatePageUp(): void;

    /**
     * Moves the active cell to the previous focusable cell (reverse row-major order).
     * @returns `true` if the active cell changed.
     */
    navigatePrev(): boolean;

    /**
     * Moves the active cell one column to the right.
     * @returns `true` if the active cell changed.
     */
    navigateRight(): boolean;

    /**
     * Moves the active cell to the last cell of the current row.
     * @returns `true` if the active cell changed.
     */
    navigateRowEnd(): boolean;

    /**
     * Moves the active cell to the first cell of the current row.
     * @returns `true` if the active cell changed.
     */
    navigateRowStart(): boolean;

    /**
     * Moves the active cell to the first row of the data set.
     */
    navigateTop(): void;

    /**
     * Moves the active cell to the specified row, keeping the current column if possible.
     * @param row - Zero-based row index to navigate to.
     * @returns `true` if the active cell changed.
     */
    navigateToRow(row: number): boolean;

    /**
     * Moves the active cell one row up.
     * @returns `true` if the active cell changed.
     */
    navigateUp(): boolean;

    /**
     * Navigate the active cell in the specified direction.
     * @param dir - Navigation direction.
     * @returns Whether navigation resulted in a change of the active cell.
     */
    navigate(dir: CellNavigationDirection): boolean;
}
