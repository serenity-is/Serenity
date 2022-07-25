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

    export type Format = (ctx: Slick.FormatterContext) => string;        

    export interface Column<TItem = any> {
        referencedFields?: string[];
        format?: Format;
    }

    export interface FormatterContext<TItem = any> {
        row?: number;
        cell?: number;
        value?: any;
        column?: Column<TItem>;
        item?: TItem;
    }
        
    export interface Formatter {
        format(ctx: FormatterContext): string;
    }
         
    export interface GroupItemMetadataProvider {
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
        onBeforeMoveRows: Event;
        onMoveRows: Event;
    }

    export class RowSelectionModel {
    }

    export namespace Data {
        export class GroupItemMetadataProvider implements GroupItemMetadataProvider {
            constructor();
            getGroupRowMetadata(item: any): any;
            getTotalsRowMetadata(item: any): any;
        }
    }
}