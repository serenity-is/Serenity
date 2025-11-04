export type CellNavigationDirection = "up" | "down" | "left" | "right" | "next" | "prev" | "home" | "end";

export interface CellNavigation {
    navigateBottom(): void;
    navigateDown(): boolean;
    navigateLeft(): boolean;
    navigateNext(): boolean;
    navigatePageDown(): void;
    navigatePageUp(): void;
    navigatePrev(): boolean;
    navigateRight(): boolean;
    navigateRowEnd(): boolean;
    navigateRowStart(): boolean;
    navigateTop(): void;
    navigateToRow(row: number): boolean;
    navigateUp(): boolean;
    /**
     * Navigate the active cell in the specified direction.
     * @param dir Navigation direction.
     * @return Whether navigation resulted in a change of active cell.
     */
    navigate(dir: CellNavigationDirection): boolean;
}
