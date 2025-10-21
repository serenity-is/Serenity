import { ItemMetadata } from "./column";
import { EventEmitter, IEventData } from "./event";
import { Group, IGroupTotals } from "./group";

export interface IDataView<TItem = any> {
    /** Gets the grand totals for all aggregated data. */
    getGrandTotals(): IGroupTotals;
    /** Gets the total number of rows in the view. */
    getLength(): number;
    /** Gets the item at the specified row index. */
    getItem(row: number): (TItem | Group<TItem> | IGroupTotals);
    /** Gets metadata for the item at the specified row index. */
    getItemMetadata?(row: number): ItemMetadata<TItem>;
    /** Event fired when the underlying data changes */
    readonly onDataChanged?: EventEmitter<any, IEventData>;
    /** Event fired when the row count changes */
    readonly onRowCountChanged?: EventEmitter<any, IEventData>;
    /** Event fired when specific rows change */
    readonly onRowsChanged?: EventEmitter<any, IEventData>;
}
