declare namespace Slick.Data {
    class GroupItemMetadataProvider implements Slick.GroupItemMetadataProvider {
        constructor();
        getGroupRowMetadata(item: any): any;
        getTotalsRowMetadata(item: any): any;
    }
}

declare namespace Slick {
    interface GroupItemMetadataProvider {
        getGroupRowMetadata(item: any): any;
        getTotalsRowMetadata(item: any): any;
    }

    class Group<TEntity> {
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
    
    class GroupTotals<TEntity> {
        isGroupTotals: boolean;
        group: Group<TEntity>;
        initialized: boolean;
        sum: any;
        avg: any;
        min: any;
        max: any;
    }

    interface GroupInfo<TItem> {
        getter?: any;
        formatter?: (p1: Slick.Group<TItem>) => string;
        comparer?: (a: Slick.Group<TItem>, b: Slick.Group<TItem>) => number;
        aggregators?: any[];
        aggregateCollapsed?: boolean;
        lazyTotalsCalculation?: boolean;
    }

}