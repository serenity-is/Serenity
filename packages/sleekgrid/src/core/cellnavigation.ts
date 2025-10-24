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
    navigate(dir: CellNavigationDirection): boolean;
}
