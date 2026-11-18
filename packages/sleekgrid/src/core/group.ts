import { NonDataRow } from "./base";
import type { ItemMetadata } from "./column";
import type { FormatterContext, FormatterResult } from "./formatting";

/**
 * The metadata hooks of a group item metadata provider, i.e. what a view and a grid need from it to
 * render the rows of a grouping. Implemented by `GroupItemMetadataProvider`, which is a grid plugin,
 * so the grid and the view can share the same instance.
 */
export interface IGroupItemMetadataProvider {
    /**
     * Returns the metadata of the row of a group.
     * @param item - The group of the row.
     * @returns Metadata describing how the row should be rendered.
     */
    getGroupRowMetadata(item: Group): ItemMetadata;
    /**
     * Returns the metadata of the row that shows the totals of a group.
     * @param item - The totals of the row.
     * @returns Metadata describing how the row should be rendered.
     */
    getTotalsRowMetadata(item: IGroupTotals): ItemMetadata;
    /**
     * Returns the metadata of a data row that is inside a group, e.g. its indentation.
     * @param row - The row index in the view.
     * @param level - The grouping level of the row.
     * @param baseMetadata - The metadata the view has already built for the row, if any.
     * @returns Metadata for the row, or `null` when there is none.
     */
    getDataRowMetadata?(row: number, level: number, baseMetadata?: ItemMetadata): ItemMetadata;
}

/**
 * Represents a group of rows produced by a `DataView` grouping.
 * @template TEntity - Row item type being grouped.
 */
export class Group<TEntity = any> extends NonDataRow {
    /** Marker flag identifying this row as a group header. */
    readonly __group = true;

    /**
     * Grouping level, starting with `0` for top-level groups.
     */
    level: number = 0;

    /**
     * Number of leaf rows in the group (excluding group headers/totals).
     */
    count: number = 0;

    /**
     * Grouping value that all rows in this group share (e.g. the field value).
     */
    value: any;

    /**
     * Whether the group is currently collapsed (children hidden).
     */
    collapsed: boolean = false;

    /**
     * Associated totals row for the group, if aggregation is enabled.
     */
    totals: GroupTotals<TEntity>;

    /**
     * Leaf rows that are part of the group.
     */
    rows: TEntity[] = [];

    /**
     * Child groups when multiple grouping levels are active.
     */
    groups: Group<TEntity>[];

    /**
     * Unique key used to identify the group; pass to `DataView.collapseGroup()` / `expandGroup()`.
     */
    groupingKey: string;

    /** Formatter that renders the group value as text. */
    formatValue: (ctx: FormatterContext<Group<TEntity>>) => FormatterResult;

    /**
     * Compares two groups by `value`, `count` and `collapsed` state.
     * @param group - Group instance to compare to.
     * @returns `true` if the groups are equal by the above fields.
     */
    equals(group: Group): boolean {
        return this.value === group.value &&
            this.count === group.count &&
            this.collapsed === group.collapsed;
    }
}

/**
 * Minimal totals information attached to a {@link Group}. Aggregators populate
 * `sum`/`avg`/`min`/`max` and arbitrary data on this object.
 * @template TEntity - Row item type.
 */
export interface IGroupTotals<TEntity = any> {
    /** Whether the row is a non-data row (inherited from {@link NonDataRow}). */
    __nonDataRow?: boolean;
    /** Marker identifying the row as a group-totals row. */
    __groupTotals?: boolean;
    /** Parent group this totals row belongs to. */
    group?: Group<TEntity>;
    /** Whether totals have been fully calculated; `false` for lazy totals. */
    initialized?: boolean;
    /** Per-field sum values. */
    sum?: Record<string, any>;
    /** Per-field average values. */
    avg?: Record<string, any>;
    /** Per-field minimum values. */
    min?: Record<string, any>;
    /** Per-field maximum values. */
    max?: Record<string, any>;
}

/**
 * Totales row for a {@link Group}. Created for each group and passed to aggregators
 * so they can store computed data that is later accessed by group-totals formatters.
 * @template TEntity - Row item type.
 */
export class GroupTotals<TEntity = any> extends NonDataRow implements IGroupTotals<TEntity> {

    /** Marker identifying this row as a group-totals row. */
    readonly __groupTotals = true;

    /**
     * Parent group this totals row belongs to.
     */
    group: Group<TEntity>;

    /**
     * Whether the totals have been fully initialized/calculated.
     * Set to `false` for lazy-calculated totals.
     */
    initialized: boolean = false;

    /**
     * Per-field sum values computed by aggregators.
     */
    sum?: Record<string, any>;

    /**
     * Per-field average values computed by aggregators.
     */
    avg?: Record<string, any>;

    /**
     * Per-field minimum values computed by aggregators.
     */
    min?: Record<string, any>;

    /**
     * Per-field maximum values computed by aggregators.
     */
    max?: Record<string, any>;
}
