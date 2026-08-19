import { ItemMetadata } from "./column";
import { EventEmitter, EventData } from "./event";
import { Group, IGroupTotals } from "./group";


/**
 * Minimal data-view contract consumed by the grid. Implemented by `DataView`.
 * @template TItem - Row item type.
 */
export interface IDataView<TItem = any> {
    /**
     * Gets grand totals aggregated over the entire data set.
     * @returns Grand totals object containing `sum`/`avg`/`min`/`max`, if any.
     */
    getGrandTotals(): IGroupTotals;
    /**
     * Gets the total number of rows currently in the view (including group headers/totals).
     * @returns Row count.
     */
    getLength(): number;
    /**
     * Gets the item at the specified view row.
     * @param row - Zero-based view index.
     * @returns Data item, `Group` header, or `IGroupTotals` row.
     */
    getItem(row: number): (TItem | Group<TItem> | IGroupTotals);
    /**
     * Gets row metadata (CSS classes, per-column overrides) for the specified view row.
     * @param row - Zero-based view index.
     * @returns Metadata object or `undefined` when none applies.
     */
    getItemMetadata?(row: number): ItemMetadata<TItem>;
    /** Event fired when the underlying data set changes. */
    readonly onDataChanged?: EventEmitter<{}>;
    /**
     * Event fired when the row count changes.
     * Payload is `{ previous, current }` with the counts before and after the change.
     */
    readonly onRowCountChanged?: EventEmitter<{ previous: number; current: number }>;
    /**
     * Event fired when specific view rows change (values or metadata).
     * Payload is `{ rows }` with the list of affected view indices.
     */
    readonly onRowsChanged?: EventEmitter<{ rows: number[] }>;
}
