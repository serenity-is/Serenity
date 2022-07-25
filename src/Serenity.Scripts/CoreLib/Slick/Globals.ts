declare namespace Slick {
    export interface AutoTooltipsOptions {
        enableForHeaderCells?: boolean;
        enableForCells?: boolean;
        maxToolTipLength?: number;
    }

    export class AutoTooltips {
        constructor(options: AutoTooltipsOptions);
        init(): void;
    }

    export type Format<TItem = any> = (ctx: Slick.FormatterContext<TItem>) => string;        

    export interface Column<TItem = any> {
        referencedFields?: string[];
        format?: Format<TItem>;
    }

    export interface FormatterContext<TItem = any> {
        addAttrs?: { [key: string]: string; };
        addClass?: string;
        cell?: number;
        column?: Column<TItem>;
        grid?: Grid<TItem>;
        item?: TItem;
        row?: number;
        toolTip?: string;
        value?: any;
    }
        
    export interface Formatter {
        format(ctx: FormatterContext): string;
    }
        
    export interface GroupItemMetadataProvider extends IPlugin {
        getGroupRowMetadata(item: any): any;
        getTotalsRowMetadata(item: any): any;
    }
        
    export interface GroupInfo<TItem> {
        getter?: any;
        formatter?: (p1: Group<TItem>) => string;
        comparer?: (a: Group<TItem>, b: Group<TItem>) => number;
        aggregators?: any[];
        aggregateCollapsed?: boolean;
        lazyTotalsCalculation?: boolean;
    }

    export interface PagerOptions {
        view?: any;
        showRowsPerPage?: boolean;
        rowsPerPage?: number;
        rowsPerPageOptions?: number[],
        onChangePage?: (newPage: number) => void;
        onRowsPerPageChange?: (n: number) => void;
    }
       
    export interface RowMoveManagerOptions {
        cancelEditOnDrag: boolean;
    }

    export class RowMoveManager implements IPlugin {
        constructor(options: RowMoveManagerOptions);
        init(): void;
        onBeforeMoveRows: Slick.Event;
        onMoveRows: Slick.Event;
    }

    export class RowSelectionModel implements SelectionModel {
        init(grid: Grid): void;
        destroy?: () => void;
        setSelectedRanges(ranges: Range[]): void;
        onSelectedRangesChanged: Slick.Event<Range[]>;
        refreshSelections?(): void;
    }

    export namespace Data {
        export class GroupItemMetadataProvider implements GroupItemMetadataProvider, IPlugin {
            constructor();
            init(grid: Grid): void;
            getGroupRowMetadata(item: any): Slick.ItemMetadata;
            getTotalsRowMetadata(item: any): Slick.ItemMetadata;
        }
    }
}