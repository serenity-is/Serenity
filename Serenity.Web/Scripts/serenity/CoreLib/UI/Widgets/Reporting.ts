declare namespace Serenity {
    namespace Reporting {
        class ReportDialog extends TemplatedDialog<ReportDialogOptions> {
            constructor(opt: ReportDialogOptions);
            createPropertyGrid(): void;
            loadReport(reportKey: string): void;
            executeReport(targetFrame: string, exportType: string): void;
        }

        interface ReportDialogOptions {
            reportKey?: string;
        }

        interface ReportExecuteRequest extends ServiceRequest {
            ExportType?: string;
            ReportKey?: string;
            DesignId?: string;
            Parameters?: any;
        }

        class ReportPage extends Serenity.Widget<any> {
            constructor(div: JQuery);
        }

        interface ReportRetrieveRequest extends ServiceRequest {
            ReportKey?: string;
        }

        interface ReportRetrieveResponse extends ServiceResponse {
            ReportKey?: string;
            Properties?: PropertyItem[];
            Title?: string;
            InitialSettings?: any;
            IsDataOnlyReport?: boolean;
        }
    }
}