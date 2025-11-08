import { applyFormatterResultToCellNode, convertCompatFormatter, formatterContext, FormatterContext, FormatterResult, gridDefaults, IGroupTotals, NonDataRow } from "@serenity-is/sleekgrid";
import { formatNumber, localText, SummaryType } from "../base";
import { AggregatorTypeRegistry, IAggregatorConstructor } from "./aggregatortyperegistry";

declare module "@serenity-is/sleekgrid" {
    interface Column<TItem = any> {
        summaryType?: SummaryType | string;
    }
}

export namespace AggregateFormatting {

    const contentOnly = {
        contentOnly: true
    };

    export function groupTotalsFormat(ctx: FormatterContext<IGroupTotals>): FormatterResult {
        const totals = ctx.item as any;
        const column = ctx.column;
        const field = column?.field;
        if (!totals)
            return "";
    
        const summaryType = column.summaryType ?? column?.sourceItem?.summaryType;
        if (!column?.field || summaryType === SummaryType.Disabled) {
            return <span class="aggregate-disabled"></span>;
        }
        if (summaryType === SummaryType.None) {
            return <span class="aggregate-none"></span>;
        }

        let aggType: IAggregatorConstructor;
        let aggKey: string;
        if (column.summaryType != null) {
            aggType = AggregatorTypeRegistry.tryGet(column.summaryType);
            aggKey = aggType?.aggregateKey;
        }
        else {
            // first try to find an aggregate key that has a value, then fallback to first null value
            aggKey = (Object.keys(totals).find(aggKey => totals[aggKey]?.[field] != null) ??
                Object.keys(totals).find(aggKey => totals[aggKey]?.[field] === null));
            if (aggKey)
                aggType = AggregatorTypeRegistry.tryGet(aggKey);
        }

        if (!aggKey)
            return "";

        const value = totals[aggKey]?.[field];
        let displayName = aggType?.displayName;
        if (!displayName) {
            const textKey = (aggKey.substring(0, 1).toUpperCase() + aggKey.substring(1));
            displayName = localText("Enums.Serenity.SummaryType." + textKey, textKey);
        }
        const span = <span class={"aggregate agg-" + aggKey} title={displayName}></span> as HTMLElement;
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
                fmtResult = formatter(formatterContext({ 
                    grid: ctx.grid,
                    escape: ctx.escape,
                    sanitizer: ctx.sanitizer,
                    column, 
                    item: cellItem, 
                    value, 
                    purpose: ctx.purpose ?? "group-totals" 
                }));
            }
            catch (e) {
                fmtResult = defaultFormatValue();
            }
        }
        else {
            fmtResult = defaultFormatValue();
        }

        applyFormatterResultToCellNode(ctx, fmtResult, span, contentOnly);
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
