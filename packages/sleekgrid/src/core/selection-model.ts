import type { CellRange, EventEmitter } from ".";
import type { GridPlugin } from "./grid-plugin";

/**
 * Contract for a grid selection model (e.g. `CellSelectionModel`, `RowSelectionModel`).
 * Implements {@link GridPlugin} so it can be registered via `grid.setSelectionModel()`.
 */
export interface SelectionModel extends GridPlugin {
    /**
     * Sets the current selection to the given cell ranges.
     * @param ranges - New selected ranges; implementations should normalize/clamp them.
     */
    setSelectedRanges(ranges: CellRange[]): void;
    /** Emits when the selected ranges change; payload is the new `CellRange[]`. */
    onSelectedRangesChanged: EventEmitter<CellRange[]>;
    /**
     * Optional hook invoked when the grid re-renders rows; selection models can
     * re-apply visual selection state here.
     */
    refreshSelections?(): void;
}

