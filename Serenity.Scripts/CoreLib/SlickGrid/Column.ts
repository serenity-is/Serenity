declare namespace Slick {
    interface Column {
        asyncPostRender?: Slick.AsyncPostRender;
        behavior?: any;
        cannotTriggerInsert?: boolean;
        cssClass?: string;
        defaultSortAsc?: boolean;
        editor?: Function;
        field: string;
        focusable?: boolean;
        formatter?: Slick.ColumnFormatter;
        headerCssClass?: string;
        id?: string;
        maxWidth?: any;
        minWidth?: number;
        name?: string;
        rerenderOnResize?: boolean;
        resizable?: boolean;
        selectable?: boolean;
        sortable?: boolean;
        toolTip?: string;
        width?: number;
        format?: (ctx: Slick.FormatterContext) => string;
        referencedFields?: string[];
        sourceItem?: Serenity.PropertyItem;
        sortOrder?: number;
        groupTotalsFormatter?: (p1?: GroupTotals<any>, p2?: Column) => string;
        visible?: boolean;
    }
}