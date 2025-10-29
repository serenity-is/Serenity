import type { CellRange, EventEmitter } from ".";
import type { GridPlugin } from "./grid-plugin";

export interface SelectionModel extends GridPlugin {
    setSelectedRanges(ranges: CellRange[]): void;
    onSelectedRangesChanged: EventEmitter<CellRange[]>;
    refreshSelections?(): void;
}

