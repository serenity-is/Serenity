import type { CellRange, EventEmitter } from "../core";
import type { Grid } from "./grid";

export interface IPlugin {
    init(grid: Grid): void;
    pluginName?: string;
    destroy?: () => void;
}

export interface SelectionModel extends IPlugin {
    setSelectedRanges(ranges: CellRange[]): void;
    onSelectedRangesChanged: EventEmitter<CellRange[]>;
    refreshSelections?(): void;
}

