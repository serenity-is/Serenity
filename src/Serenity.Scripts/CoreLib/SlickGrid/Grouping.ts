export interface GroupItemMetadataProvider {
    getGroupRowMetadata(item: any): any;
    getTotalsRowMetadata(item: any): any;
}

export class Group<TEntity> {
    isGroup: boolean;
    level: number;
    count: number;
    value: any;
    title: string;
    collapsed: boolean;
    totals: any;
    rows: any;
    groups: Group<TEntity>[];
    groupingKey: string;
}

export class GroupTotals<TEntity> {
    isGroupTotals: boolean;
    group: Group<TEntity>;
    initialized: boolean;
    sum: any;
    avg: any;
    min: any;
    max: any;
}

export interface GroupInfo<TItem> {
    getter?: any;
    formatter?: (p1: Group<TItem>) => string;
    comparer?: (a: Group<TItem>, b: Group<TItem>) => number;
    aggregators?: any[];
    aggregateCollapsed?: boolean;
    lazyTotalsCalculation?: boolean;
}