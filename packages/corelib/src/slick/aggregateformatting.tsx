import { applyFormatterResultToCellNode, Column, convertCompatFormatter, formatterContext, FormatterContext, FormatterResult, gridDefaults, IGroupTotals, NonDataRow } from "@serenity-is/sleekgrid";
import { formatNumber, htmlEncode, localText, SummaryType } from "../base";
import { AggregatorTypeRegistry, IAggregatorConstructor } from "./aggregatortyperegistry";

declare module "@serenity-is/sleekgrid" {
    interface Column<TItem = any> {
        summaryType?: SummaryType | string;
    }
}

/** Formatting helpers for aggregate / group-totals rows. */
export namespace AggregateFormatting {

    const contentOnly = {
        contentOnly: true
    };

    /**
     * Formats the value of an aggregate, e.g. a total, as the cells of a totals row show it, so that a text
     * like the totals of a pager uses the same formatting. It uses the formatter of the column, and falls
     * back to a number formatted by its display format.
     * @param column - The column the aggregate belongs to.
     * @param value - The value of the aggregate for that column.
     * @param ctx - The formatter context to take the grid, escaping and sanitizer from, when available.
     * @returns The formatted value, which may be markup when the column formatter returns an element.
     */
    export function formatAggregateValue(column: Column, value: any,
        ctx?: Partial<FormatterContext>): FormatterResult {
        const field = column?.field;
        const formatter = column?.format ?? ((column as any)?.formatter ?
            convertCompatFormatter((column as any).formatter) : null);

        const escape = ctx?.escape ?? htmlEncode;
        const defaultFormatValue = (): FormatterResult => {
            if (typeof value === "number")
                return escape(formatNumber(value, column?.sourceItem?.displayFormat ?? "#,##0.##"));
            return escape(value);
        };

        if (formatter == null)
            return defaultFormatValue();

        const cellItem = new NonDataRow();
        if (field)
            (cellItem as any)[field] = value;

        try {
            return formatter(formatterContext({
                grid: ctx?.grid,
                escape,
                sanitizer: ctx?.sanitizer,
                column,
                item: cellItem,
                value,
                purpose: ctx?.purpose ?? "group-totals"
            }));
        }
        catch {
            return defaultFormatValue();
        }
    }

    /**
     * Creates an element that shows the value of an aggregate, formatted as the cells of a totals row show
     * it, e.g. to put it in the content a pager shows next to its controls. Use {@link formatAggregateValue}
     * when the formatted value goes into a node of your own.
     * @param column - The column the aggregate belongs to.
     * @param value - The value of the aggregate for that column.
     * @param ctx - The formatter context to take the grid, escaping and sanitizer from, when available.
     * @returns An element with the formatted value.
     */
    export function createAggregateValueNode(column: Column, value: any,
        ctx?: Partial<FormatterContext>): HTMLElement {
        const span = document.createElement("span");
        applyFormatterResultToCellNode({
            ...(ctx as FormatterContext),
            escape: ctx?.escape ?? htmlEncode,
            sanitizer: ctx?.sanitizer ?? gridDefaults?.sanitizer,
            enableHtmlRendering: true
        } as FormatterContext, formatAggregateValue(column, value, ctx), span, contentOnly);
        return span;
    }

    /**
     * Formats a group totals cell (aggregate badge/value) based on the column's summaryType.
     * @param ctx - Formatter context containing group totals item and column.
     * @returns Rendered aggregate markup or empty string.
     */
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
        const fmtResult = formatAggregateValue(column, value, ctx);
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
