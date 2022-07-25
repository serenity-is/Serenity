import type { Event } from "./event";
import type { Grid } from "./grid";
import type { Range } from "./range";

export interface IPlugin {
    init(grid: Grid): void;
    pluginName?: string;
    destroy?: () => void;
}

export interface Position {
    bottom?: number;
    height?: number;
    left?: number;
    right?: number;
    top?: number;
    visible?: boolean;
    width?: number;
}

export interface RowCell {
    row: number;
    cell: number;
}

export interface SelectionModel {
    init(grid: Grid): void;
    destroy?: () => void;
    setSelectedRanges(ranges: Range[]): void;
    onSelectedRangesChanged: Event<Range[]>;
    refreshSelections?(): void;
}

export interface ViewRange {
    top?: number;
    bottom?: number;
    leftPx?: number;
    rightPx?: number;
}