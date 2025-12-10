import { DataGrid, deepClone, Fluent, formatDate, getGlobalObject, ListRequest, ListResponse, notifyError, resolveUrl, serviceCall, stringFormat, ToolButton } from "@serenity-is/corelib";
import { applyFormatterResultToCellNode, Column, FormatterResult, ISleekGrid } from "@serenity-is/sleekgrid";
import { type jsPDF } from "./JsPdfAutoTable";

export interface PdfExportOptions {
    grid: DataGrid<any, any>;
    onViewSubmit?: () => boolean;
    title?: string;
    hint?: string;
    separator?: boolean;
    reportTitle?: string;
    titleTop?: number;
    titleFontSize?: number;
    fileName?: string;
    pageNumbers?: boolean;
    columnTitles?: { [key: string]: string };
    tableOptions?: jsPDF.AutoTableOptions;
    output?: string;
    autoPrint?: boolean;
    printDateTimeHeader?: boolean;
}

const contentOnly = {
    contentOnly: true
}

export namespace PdfExportHelper {

    function toAutoTableColumns(srcColumns: Column[], columnStyles: { [dataKey: string]: jsPDF.AutoTableStyles; },
        columnTitles: { [key: string]: string }) {
        return srcColumns.map(src => {
            let col: jsPDF.AutoTableColumn = {
                dataKey: src.id || src.field,
                title: src.name || ''
            };

            if (columnTitles && columnTitles[col.dataKey] != null)
                col.title = columnTitles[col.dataKey];

            let style: jsPDF.AutoTableStyles = {};
            if ((src.cssClass || '').indexOf("align-right") >= 0)
                style.halign = 'right';
            else if ((src.cssClass || '').indexOf("align-center") >= 0)
                style.halign = 'center';

            columnStyles[col.dataKey] = style;

            return col;
        });
    }

    function toAutoTableData(sleekGrid: ISleekGrid, entities: any[], keys: string[], srcColumns: Column[]) {
        let el = document.createElement('span');
        let row = 0;
        return entities.map(item => {
            let dst = [];
            for (let cell = 0; cell < srcColumns.length; cell++) {
                const col = srcColumns[cell];
                const format = sleekGrid.getFormatter(row, col);
                const ctx = sleekGrid.getFormatterContext(row, cell);
                ctx.purpose = "pdf-export";
                ctx.item = item;
                ctx.value = item[col.field];
                let fmtResult: FormatterResult = format ? (format(ctx) ?? "") : '';
                if (typeof fmtResult === "string" && (!fmtResult.length && (fmtResult.indexOf('<') < 0 || fmtResult.indexOf('&') < 0))) {
                    dst.push(fmtResult);
                }
                else {
                    Fluent.empty(el);
                    applyFormatterResultToCellNode(ctx, fmtResult, el, contentOnly);
                    el.querySelectorAll(".unicode-emoji").forEach(ue => ue.remove());
                    if (el.children.length == 1 &&
                        el.children[0]?.nodeName === 'SELECT') {
                        dst.push(el.children[0].querySelector("[selected]")?.textContent ?? '');
                    }
                    else if (el.children.length == 1 &&
                        Fluent.isInputLike(el.children[0])) {
                        dst.push((el.children[0] as any).value);
                    }
                    else if (el.children.length == 1 &&
                        el.children[0].classList.contains('.check-box')) {
                        dst.push(el.children[0].classList.contains("checked") ? "X" : "")
                    }
                    else {
                        dst.push(el.textContent || '');
                    }
                }
            }
            row++;
            return dst;
        });
    }

    export function exportToPdf(options: PdfExportOptions): void {

        const dataGrid = options.grid;

        if ((options.onViewSubmit && !options.onViewSubmit()) ||
            (!options.onViewSubmit && !dataGrid.prepareSubmit())) {
            return;
        }

        var request = deepClone(dataGrid.view.params) as ListRequest;
        request.Take = 0;
        request.Skip = 0;

        var sortBy = dataGrid.view.sortBy;
        if (sortBy != null)
            request.Sort = sortBy;

        var gridColumns = dataGrid.columns;
        gridColumns = gridColumns.filter(x => x.id !== "__select__");

        request.IncludeColumns = [];
        for (var column of gridColumns)
            request.IncludeColumns.push(column.id || column.field);

        serviceCall({
            url: dataGrid.view.url,
            request: request,
            onSuccess: response => autoTableImport(({ jsPDF }) => {
                let doc = new jsPDF('l', 'pt');
                let srcColumns = gridColumns;
                let columnStyles: { [dataKey: string]: jsPDF.AutoTableStyles; } = {};
                let columns = toAutoTableColumns(srcColumns, columnStyles, options.columnTitles);
                var keys = columns.map(x => x.dataKey);
                let entities = (<ListResponse<any>>response).Entities || [];
                let data = toAutoTableData(dataGrid.sleekGrid, entities, keys, srcColumns);

                doc.setFontSize(options.titleFontSize || 10);
                doc.setFont('helvetica', 'bold');
                let reportTitle = options.reportTitle || dataGrid.getTitle() || "Report";

                doc.autoTableText(reportTitle, doc.internal.pageSize.width / 2,
                    options.titleTop || 25, { halign: 'center' });

                var totalPagesExp = "{{T}}";

                let pageNumbers = options.pageNumbers == null || options.pageNumbers;
                var autoOptions = Object.assign({
                    margin: { top: 25, left: 25, right: 25, bottom: pageNumbers ? 25 : 30 },
                    startY: 60,
                    styles: {
                        fontSize: 8,
                        overflow: 'linebreak',
                        cellPadding: 2,
                        valign: 'middle'
                    },
                    columnStyles: columnStyles
                }, options.tableOptions);

                var footer: (data: any) => void;
                var header: (data: any) => void;
                if (pageNumbers) {
                    footer = function (data) {
                        var str = (data.pageNumber ?? data.pageCount)?.toString() ?? "?";
                        // Total page number plugin only available in jspdf v1.0+
                        if (typeof doc.putTotalPages === 'function') {
                            str = str + " / " + totalPagesExp;
                        }
                        doc.autoTableText(str, doc.internal.pageSize.width / 2,
                            doc.internal.pageSize.height - autoOptions.margin.bottom, {
                            halign: 'center'
                        });
                    };
                }

                // Print header of page
                if (options.printDateTimeHeader == null || options.printDateTimeHeader) {
                    header = function (data) {
                        doc.setFont('helvetica', 'normal');
                        doc.setFontSize(8);

                        // Date and time of the report
                        doc.autoTableText(formatDate(new Date(), "dd-MM-yyyy HH:mm"),
                            doc.internal.pageSize.width - autoOptions.margin.right, 13,
                            {
                                halign: 'right'
                            });
                    };
                }

                autoOptions.didDrawPage = (data) => {
                    if (!!header) header(data);
                    if (!!footer) footer(data);
                };

                autoOptions.head = [columns.map(x => x.title)];
                autoOptions.body = data;

                doc.autoTable(autoOptions);

                if (typeof doc.putTotalPages === 'function') {
                    doc.putTotalPages(totalPagesExp);
                }


                if (!options.output || options.output == "file") {
                    var fileName = options.fileName || options.reportTitle || "{0}_{1}.pdf";
                    fileName = stringFormat(fileName, dataGrid.getTitle() || "report",
                        formatDate(new Date(), "yyyyMMdd_HHmm"));
                    doc.save(fileName);
                    return;
                }

                if (options.autoPrint)
                    doc.autoPrint();

                var output = options.output;
                if (output == 'newwindow' || '_blank')
                    output = 'dataurlnewwindow';
                else if (output == 'window')
                    output = 'datauri';

                doc.output(output);
            })
        });
    }

    export function createToolButton(options: PdfExportOptions) {

        return <ToolButton>{
            title: options.title || '',
            hint: options.hint || 'PDF',
            cssClass: 'export-pdf-button',
            onClick: () => exportToPdf(options),
            separator: options.separator
        };
    }

    const jspdfModule = "jspdf";

    function jsPdfImport(then: ({ jsPDF }: { jsPDF: jsPDF }) => void) {

        const globalObj = getGlobalObject();

        let jsPDF = globalObj.jsPDF ?? globalObj.jspdf?.jsPDF as jsPDF;
        if (jsPDF)
            return then({ jsPDF });

        import(`${jspdfModule}`).then(jsPDFModule => {
            jsPDF = jsPDFModule?.jsPDF;
            if (jsPDF) {
                globalObj.jsPDF = jsPDF;
                then({ jsPDF });
            }
            else 
                notifyError("Failed to load jsPDF module from import map!");
        }).catch(() => {
            import(resolveUrl("~/Serenity.Assets/jspdf/jspdf-autotable.bundle.js")).then(jsPDFModule => {
                jsPDF = jsPDFModule?.jsPDF;
                if (jsPDF) {
                    globalObj.jsPDF = jsPDF;
                    then({ jsPDF });
                }
                else
                    notifyError("Failed to load jsPDF module from assets!");
            }).catch(() => {
                notifyError("Failed to load jsPDF module!");
            });
        });
    }

    function autoTableImport(then: ({ jsPDF }: { jsPDF: jsPDF }) => void) {
        jsPdfImport(({ jsPDF }) => {
            if (jsPDF?.API?.autoTable)
                return then({ jsPDF });

            import("jspdf-autotable" as any).then(({ applyPlugin }) => {
                if (typeof applyPlugin === "function") {
                    applyPlugin(jsPDF);
                    then({ jsPDF });
                }
                else if (jsPDF?.API?.autoTable) {
                    then({ jsPDF });
                }
                else 
                    notifyError("Failed to load jsPDF AutoTable module from import map!");
            }).catch(() => {
                import(resolveUrl("~/Serenity.Assets/jspdf/jspdf-autotable.bundle.js")).then(({ applyPlugin }) => {
                    if (typeof applyPlugin === "function") {
                        applyPlugin(jsPDF);
                        then({ jsPDF });
                    }
                    else if (jsPDF?.API?.autoTable) {
                        then({ jsPDF });
                    }
                    else
                        notifyError("Failed to load jsPDF AutoTable module from assets!");
                })
                .catch(() => {
                    notifyError("Failed to load jsPDF AutoTable module!");
                });
            });
        });
    }
}