declare namespace Slick {
    interface FormatterContext {
        row?: number;
        cell?: number;
        value?: any;
        column?: any;
        item?: any;
    }

    interface Formatter {
        format(ctx: FormatterContext): string;
    }

    export type Format = (ctx: Slick.FormatterContext) => string;

    type AsyncPostRender = (cellNode: any, row: number, item: any, column: Slick.Column, clean?: boolean) => void;
    type ColumnFormatter = (row: number, cell: number, value: any, column: Slick.Column, item: any) => string;
}