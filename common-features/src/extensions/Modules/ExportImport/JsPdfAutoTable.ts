export { };

declare global {

    var jspdf: any;
    var jsPDF: any;

    interface jsPDF {
        autoTableEndPosY?: number;
        autoTableHtmlToJson(table: HTMLElement);
        autoTable(columns: string[] | jsPDF.AutoTableColumn[], data: any[], options: jsPDF.AutoTableOptions);
        autoTableText(text: string, x: number, y: number, styles: jsPDF.AutoTableStyles);
    }

    namespace jsPDF {

        interface AutoTableColumn {
            title?: string;
            dataKey?: string;
        }

        interface AutoTableOptions {
            tableWidth?: 'wrap';
            theme?: 'striped' | 'grid' | 'plain';
            startY?: number;
            styles?: AutoTableStyles;
            headerStyles?: AutoTableStyles;
            bodyStyles?: AutoTableStyles;
            columnStyles?: {
                [dataKey: string]: AutoTableStyles;
            };
            margin?: AutoTableMargin;
            didDrawCell?: (data: CellHookData) => void;
            didDrawPage?: (data: HookData) => void;
            head?: [AutoTableColumn[]],
            body?: {}[]
        }

        interface HookData {
            table?: any,
            pageNumber?: number,
            pageCount?: number,
            settings?: {},
            doc?: any,
            cursor?: { x?: number, y?: number }
        }

        interface CellHookData extends HookData {
            cell?: { x?: number, y?: number },
            row?: any,
            column?: AutoTableColumn,
            section?: 'head' | 'body' | 'foot';
        }

        interface AutoTableMargin {
            horizontal?: number;
            top?: number;
            left?: number;
            right?: number;
            bottom?: number;
        }

        interface AutoTableStyles {
            cellPadding?: number;
            fontSize?: number;
            font?: string;
            lineColor?: number | number[];
            lineWidth?: number;
            lineHeight?: number;
            fontStyle?: string;
            fillColor?: number | number[];
            textColor?: number | number[];
            halign?: 'left' | 'center' | 'right';
            valign?: 'top' | 'middle' | 'bottom';
            fillStyle?: 'S' | 'F' | 'DF';
            rowHeight?: number;
            columnWidth?: 'auto' | 'wrap' | number;
            cellWidth?: 'auto' | 'wrap' | number;
            overflow?: 'linebreak';
        }
    }
}