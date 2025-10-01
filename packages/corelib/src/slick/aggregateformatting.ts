import { applyFormatterResultToCellNode, convertCompatFormatter, formatterContext, FormatterContext, FormatterResult, IGroupTotals, NonDataRow } from "@serenity-is/sleekgrid";
import { formatNumber, localText } from "../base";

export namespace AggregateFormatting {
    const aggTypeKeys = ["sum", "avg", "min", "max", "weightedAvg"];

    export function groupTotalsFormat(ctx: FormatterContext<IGroupTotals>): FormatterResult {
        const totals = ctx.item as any;
        const column = ctx.column;
        const field = column?.field;
        if (!totals || !field)
            return "";

        const aggType = aggTypeKeys.find(aggType => totals[aggType]?.[field] != null);
        if (!aggType)
            return;

        const value = totals[aggType][field];
        const span = document.createElement("span");
        span.className = 'aggregate agg-' + aggType;
        const textKey = (aggType.substring(0, 1).toUpperCase() + aggType.substring(1));
        span.title = localText("Enums.Serenity.SummaryType." + textKey, textKey);
        const formatter = column.format ?? ((column as any).formatter ? convertCompatFormatter((column as any).formatter) : null);

        function defaultFormatValue() {
            if (typeof value === "number") {
                const displayFormat = column.sourceItem?.displayFormat ?? "#,##0.##";
                return ctx.escape(formatNumber(value, displayFormat));
            }
            return ctx.escape(value);
        }

        let fmtResult: FormatterResult;
        if (formatter != null) {
            const cellItem = new NonDataRow();
            (cellItem as any)[field] = value;
            try {
                fmtResult = formatter(formatterContext({ column, item: cellItem, value, purpose: ctx.purpose ?? "group-totals" }));
            }
            catch (e) {
                fmtResult = defaultFormatValue();
            }
        }
        else {
            fmtResult = defaultFormatValue();
        }

        applyFormatterResultToCellNode(formatterContext({
            sanitizer: ctx.sanitizer
        }), fmtResult, span);
        return span;
    }
}