export interface jsPDF {
    new(...args: any[]): jsPDF.jsPDFInstance;
    API?: {
        autoTable?: any;
    };
}

export namespace jsPDF {

    export interface jsPDFInstance {
        autoPrint?: () => any;
        autoTableEndPosY?: number;
        autoTableHtmlToJson?(table: HTMLElement): any;
        autoTable?(options: jsPDF.AutoTableOptions): any;
        autoTableText?(text: string, x: number, y: number, styles: jsPDF.AutoTableStyles): any;
        setFontSize?(size: number): any;
        setFont?(fontName: string, fontStyle?: string): any;
        internal?: {
            pageSize?: {
                width: number;
                height: number;
            };
        },
        output?(type: string, options?: any): any;
        putTotalPages?(totalPagesString: string): any;
        save?(fileName: string): any;
    }

    export interface AutoTableColumn {
        title?: string;
        dataKey?: string;
    }

    export interface AutoTableOptions {
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
        head?: [(AutoTableColumn | string)[]],
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

    export interface CellHookData extends HookData {
        cell?: { x?: number, y?: number },
        row?: any,
        column?: AutoTableColumn,
        section?: 'head' | 'body' | 'foot';
    }

    export interface AutoTableMargin {
        horizontal?: number;
        top?: number;
        left?: number;
        right?: number;
        bottom?: number;
    }

    export interface AutoTableStyles {
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