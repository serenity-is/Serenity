import { NonDataRow } from "./base";
import type { FormatterContext, FormatterResult } from "./formatting";

/***
 * Information about a group of rows.
 */
export class Group<TEntity = any> extends NonDataRow {
    readonly __group = true;

    /**
     * Grouping level, starting with 0.
     * @property level
     * @type {Number}
     */
    level: number = 0;

    /***
     * Number of rows in the group.
     * @property count
     * @type {Number}
     */
    count: number = 0;

    /***
     * Grouping value.
     * @property value
     * @type {Object}
     */
    value: any;

    /***
     * Whether a group is collapsed.
     * @property collapsed
     * @type {Boolean}
     */
    collapsed: boolean = false;

    /***
     * GroupTotals, if any.
     * @property totals
     * @type {GroupTotals}
     */
    totals: GroupTotals<TEntity>;

    /**
     * Rows that are part of the group.
     * @property rows
     * @type {Array}
     */
    rows: TEntity[] = [];

    /**
     * Sub-groups that are part of the group.
     * @property groups
     * @type {Array}
     */
    groups: Group<TEntity>[];

    /**
     * A unique key used to identify the group.  This key can be used in calls to DataView
     * collapseGroup() or expandGroup().
     * @property groupingKey
     * @type {Object}
     */
    groupingKey: string;

    /** Returns a text representation of the group value. */
    formatValue: (ctx: FormatterContext<Group<TEntity>>) => FormatterResult;

    /***
     * Compares two Group instances.
     * @return {Boolean}
     * @param group {Group} Group instance to compare to.
     */
    equals(group: Group): boolean {
        return this.value === group.value &&
            this.count === group.count &&
            this.collapsed === group.collapsed;
    }
}

export interface IGroupTotals<TEntity = any> {
    __nonDataRow?: boolean;
    __groupTotals?: boolean;
    group?: Group<TEntity>;
    initialized?: boolean;
    sum?: Record<string, any>;
    avg?: Record<string, any>;
    min?: Record<string, any>;
    max?: Record<string, any>;
}

/***
 * Information about group totals.
 * An instance of GroupTotals will be created for each totals row and passed to the aggregators
 * so that they can store arbitrary data in it.  That data can later be accessed by group totals
 * formatters during the display.
 * @class GroupTotals
 * @extends NonDataRow
 */
export class GroupTotals<TEntity = any> extends NonDataRow implements IGroupTotals<TEntity> {

    readonly __groupTotals = true;

    /***
     * Parent Group.
     * @param group
     * @type {Group}
     */
    group: Group<TEntity>;

    /***
     * Whether the totals have been fully initialized / calculated.
     * Will be set to false for lazy-calculated group totals.
     * @param initialized
     * @type {Boolean}
     */
    initialized: boolean = false;

    /**
     * Contains sum
     */
    sum?: Record<string, any>;

    /**
     * Contains avg
     */
    avg?: Record<string, any>;

    /**
     * Contains min
     */
    min?: Record<string, any>;

    /**
     * Contains max
     */
    max?: Record<string, any>;
}
