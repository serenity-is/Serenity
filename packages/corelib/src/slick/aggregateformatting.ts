import { applyFormatterResultToCellNode, convertCompatFormatter, formatterContext, FormatterContext, FormatterResult, gridDefaults, IGroupTotals, NonDataRow } from "@serenity-is/sleekgrid";
import { formatNumber, localText, SummaryType } from "../base";
import { AggregatorTypeRegistry, IAggregatorConstructor } from "./aggregatortyperegistry";

declare module "@serenity-is/sleekgrid" {
    interface Column<TItem = any> {
        summaryType?: SummaryType | string;
    }
}

export namespace AggregateFormatting {

    export function groupTotalsFormat(ctx: FormatterContext<IGroupTotals>): FormatterResult {
        const totals = ctx.item as any;
        const column = ctx.column;
        const field = column?.field;
        if (!totals || !field)
            return "";

        let aggCons: IAggregatorConstructor;
        let aggType: string;
        if (column.summaryType) {
            aggCons = AggregatorTypeRegistry.tryGet(column.summaryType);
            aggType = aggCons?.aggregateType;
        }
        else {
            aggType = (Object.keys(totals).find(aggType => totals[aggType]?.[field] != null) ??
                Object.keys(totals).find(aggType => totals[aggType]?.[field] !== void 0));
            if (aggType)
                aggCons = AggregatorTypeRegistry.tryGet(aggType);
        }

        if (!aggType)
            return "";

        const value = totals[aggType][field];
        const span = document.createElement("span");
        span.className = 'aggregate agg-' + aggType;
        let displayName = aggCons?.displayName;
        if (!displayName) {
            const textKey = (aggType.substring(0, 1).toUpperCase() + aggType.substring(1));
            displayName = localText("Enums.Serenity.SummaryType." + textKey, textKey);
        }
        span.innerText = displayName + ": ";
        span.title = displayName;
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

    /**
     * Call this method to ensure that `gridDefaults.groupTotalsFormat` is set to `AggregateFormatting.groupTotalsFormat`.
     * It only sets it when it is not already set to some value. This is normally called by `RemoteView` constructor.
     */
    export function initGridDefaults() {
        if (gridDefaults != null && gridDefaults.groupTotalsFormat === void 0)
            gridDefaults.groupTotalsFormat = AggregateFormatting.groupTotalsFormat;
    }
}
