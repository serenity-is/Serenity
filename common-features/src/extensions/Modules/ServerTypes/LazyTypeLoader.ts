import { Config } from "@serenity-is/corelib";

const loaderByKey = {
    "Serenity.Extensions.BasicProgressDialog": async () => (await import("../BulkActions/BasicProgressDialog")).BasicProgressDialog,
    "Serenity.Extensions.EnumSelectFormatter": async () => (await import("../Formatters/EnumSelectFormatter")).EnumSelectFormatter,
    "Serenity.Extensions.PromptDialog": async () => (await import("../Widgets/PromptDialog")).PromptDialog,
    "Serenity.Extensions.ReportDialog": async () => (await import("../Reporting/ReportDialog")).ReportDialog,
    "Serenity.Extensions.ReportPage": async () => (await import("../Reporting/ReportPage")).ReportPage,
    "Serenity.Extensions.SelectableEntityGrid": async () => (await import("../Widgets/SelectableEntityGrid")).SelectableEntityGrid,
    "Serenity.Extensions.SingleLineTextFormatter": async () => (await import("../Formatters/SingleLineTextFormatter")).SingleLineTextFormatter,
    "Serenity.Extensions.StaticTextBlock": async () => (await import("../Widgets/StaticTextBlock")).StaticTextBlock
}

Config.lazyTypeLoader = (function(org: any) {
    return (key: string, type: any) => loaderByKey[key]?.() || org?.(key, type);
})(Config.lazyTypeLoader);