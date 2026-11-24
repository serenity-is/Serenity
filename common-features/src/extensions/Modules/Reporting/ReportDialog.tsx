import { BaseDialog, PropertyGrid, WidgetProps, faIcon, resolveUrl, serviceCall } from "@serenity-is/corelib";
import { nsExtensions } from "../ServerTypes/Namespaces";
import { ReportRetrieveResult } from "../ServerTypes/Reporting";
import { ReportHelper } from "./ReportHelper";

export class ReportDialog<P extends ReportDialogOptions = ReportDialogOptions> extends BaseDialog<P> {
    static override[Symbol.typeInfo] = this.registerClass(nsExtensions);

    declare private report: ReportRetrieveResult;
    declare private propertyGrid: PropertyGrid;

    constructor(props: WidgetProps<P>) {
        super(props);

        this.updateInterface();
        this.loadReport(this.options.reportKey);
    }

    protected override getDialogButtons() {
        return null;
    }

    protected createPropertyGrid() {
        this.propertyGrid && this.byId('PropertyGrid').empty().class('');
        this.propertyGrid = new PropertyGrid({
            element: this.byId('PropertyGrid'),
            idPrefix: this.idPrefix,
            items: this.report.Properties
        }).init();
    }

    protected loadReport(reportKey: string) {
        serviceCall({
            url: resolveUrl('~/Serenity.Extensions/Report/Retrieve'),
            request: {
                ReportKey: reportKey
            },
            onSuccess: response => {
                this.report = response as ReportRetrieveResult;
                this.dialogTitle = this.report.Title;
                this.createPropertyGrid();
                this.propertyGrid.load(this.report.InitialSettings || {});
                this.updateInterface();
                this.dialogOpen();
            }
        });
    }

    protected updateInterface() {
        const report = this.report;
        const designed = !!report && !report.IsDataOnlyReport && !report.IsExternalReport && !report.IsStructuredReport;

        this.toolbar.findButton('print-preview-button')
            .toggle(designed);

        this.toolbar.findButton('run-button')
            .toggle(!!report?.IsExternalReport);

        this.toolbar.findButton('export-pdf-button')
            .toggle(designed);

        this.toolbar.findButton('export-xlsx-button')
            .toggle(!!report && (!!report.IsDataOnlyReport || !!report.IsStructuredReport) && !report.IsExternalReport);

        this.toolbar.findButton('export-csv-button')
            .toggle(!!report?.IsStructuredReport);
    }

    executeReport(target: string, ext: string, download: boolean) {
        if (!this.validateForm()) {
            return;
        }

        const opt = {};
        this.propertyGrid.save(opt);
        ReportHelper.execute({
            download: download,
            reportKey: this.report.ReportKey,
            extension: ext as any,
            target: target,
            params: opt
        });
    }

    protected override getToolbarButtons() {
        return [
            {
                title: 'Preview',
                cssClass: 'print-preview-button',
                onClick: () => this.executeReport('_blank', null, false)
            },
            {
                title: 'Run',
                cssClass: 'run-button',
                icon: faIcon("print", "blue"),
                onClick: () => this.executeReport('_blank', null, false)
            },
            {
                title: 'PDF',
                cssClass: 'export-pdf-button',
                onClick: () => this.executeReport('_blank', 'pdf', true)
            },
            {
                title: 'Excel',
                cssClass: 'export-xlsx-button',
                onClick: () => this.executeReport('_blank', 'xlsx', true)
            },
            {
                title: 'CSV',
                cssClass: 'export-csv-button',
                onClick: () => this.executeReport('_blank', 'csv', true)
            }
        ];
    }

    protected override renderContents(): any {
        const id = this.useIdPrefix();
        return (
            <div class="s-DialogContent">
                <div id={id.Toolbar} class="s-DialogToolbar"></div>
                <div class="s-Form">
                    <form id={id.Form} action="">
                        <div id={id.PropertyGrid}></div>
                    </form>
                </div>
            </div>
        );
    }
}

export interface ReportDialogOptions {
    reportKey: string;
}