import { IconClassName, ToolButton, postToUrl } from "@serenity-is/corelib";

export interface ReportExecuteOptions {
    reportKey: string;
    download?: boolean;
    extension?: 'pdf' | 'htm' | 'html' | 'xlsx' | 'docx';
    getParams?: () => any;
    params?: { [key: string]: any }
    target?: string;
}

export interface ReportButtonOptions extends ReportExecuteOptions {
    title?: string;
    cssClass?: string;
    icon?: IconClassName;
}

export namespace ReportHelper {

    export function createToolButton(options: ReportButtonOptions): ToolButton {
        return {
            title: options.title ?? 'Report',
            cssClass: options.cssClass ?? 'print-button',
            icon: options.icon,
            onClick: () => {
                ReportHelper.execute(options);
            }
        };
    }

    export function execute(options: ReportExecuteOptions) {
        var opt = options.getParams ? options.getParams() : options.params;

        postToUrl({
            url: '~/Serenity.Extensions/Report/' + (options.download ? 'Download' : 'Render'),
            params: {
                key: options.reportKey,
                ext: options.extension ?? 'pdf',
                opt: opt ? JSON.stringify(opt) : ''
            },
            target: options.target ?? '_blank'
        });
    }
}