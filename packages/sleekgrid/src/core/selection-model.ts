import type { CellRange, EventEmitter } from ".";
import type { IPlugin } from "./iplugin";

export interface SelectionModel extends IPlugin {
    setSelectedRanges(ranges: CellRange[]): void;
    onSelectedRangesChanged: EventEmitter<CellRange[]>;
    refreshSelections?(): void;
}

