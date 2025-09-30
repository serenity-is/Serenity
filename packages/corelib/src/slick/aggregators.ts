import { Column, FormatterContext, FormatterResult, IGroupTotals, NonDataRow, applyFormatterResultToCellNode, convertCompatFormatter, formatterContext } from "@serenity-is/sleekgrid";
import { formatNumber, htmlEncode, localText } from "../base";

export interface IAggregator {
    init(): void;
    accumulate(item: any): void;
    storeResult(totals: IGroupTotals): void;
}

export namespace Aggregators {
    export class Avg implements IAggregator {
        public count: number;
        public nonNullCount: number;
        public sum: number;

        constructor(public readonly field: string) {
        }

        init() {
            this.count = 0;
            this.nonNullCount = 0;
            this.sum = 0;
        };

        accumulate(item: any) {
            const val = item[this.field];
            this.count++;
            if (val != null && val !== "" && !isNaN(val)) {
                this.nonNullCount++;
                this.sum += typeof val === "number" ? val : parseFloat(val);
            }
        }

        storeResult(groupTotals: IGroupTotals) {
            if (!groupTotals.avg) {
                groupTotals.avg = {};
            }
            groupTotals.avg[this.field] = this.nonNullCount != 0 ? this.sum / this.nonNullCount : null;
        }
    }

    export class WeightedAvg implements IAggregator {
        public sum: number;
        public weightedSum: number;

        constructor(public readonly field: string,
            public readonly weightedField: string) {
        }

        init() {
            this.sum = 0;
            this.weightedSum = 0;
        }

        accumulate(item: any) {
            const val = item[this.field];
            let valWeighted = item[this.weightedField];
            if (WeightedAvg.isValid(val) && WeightedAvg.isValid(valWeighted)) {
                valWeighted = typeof valWeighted === "number" ? valWeighted : parseFloat(valWeighted);
                this.weightedSum += valWeighted;
                this.sum += valWeighted * (typeof val === "number" ? val : parseFloat(val));
            }
        }

        storeResult(groupTotals: any) {
            if (!groupTotals.avg) {
                groupTotals.avg = {};
            }
            if (this.sum && this.weightedSum) {
                groupTotals.avg[this.field] = this.sum / this.weightedSum;
            }
        }

        static isValid(val: any): boolean {
            return val !== null && val !== "" && !isNaN(val);
        }
    }

    export class Min implements IAggregator {
        public readonly field: string;
        public min: any;

        constructor(field: string) {
            this.field = field;
        }

        init() {
            this.min = null;
        }

        accumulate(item: any) {
            const val = item[this.field];
            if (val != null && val !== "" && !isNaN(val)) {
                if (this.min == null || val < this.min) {
                    this.min = val;
                }
            }
        }

        storeResult(groupTotals: any) {
            if (!groupTotals.min) {
                groupTotals.min = {};
            }
            groupTotals.min[this.field] = this.min;
        }
    }

    export class Max implements IAggregator {
        public max: any;

        constructor(public readonly field: string) {
        }

        init() {
            this.max = null;
        }

        accumulate(item: any) {
            const val = item[this.field];
            if (val != null && val !== "" && !isNaN(val)) {
                if (this.max == null || val > this.max) {
                    this.max = val;
                }
            }
        }

        storeResult(groupTotals: any) {
            if (!groupTotals.max) {
                groupTotals.max = {};
            }
            groupTotals.max[this.field] = this.max;
        }
    }

    export class Sum implements IAggregator {
        public readonly field: string;
        public sum: number;

        constructor(field: string) {
            this.field = field;
        }

        init() {
            this.sum = 0;
        }

        accumulate(item: any) {
            const val = item[this.field];
            if (val != null && val !== "" && !isNaN(val)) {
                this.sum += typeof val === "number" ? val : parseFloat(val);
            }
        }

        storeResult(groupTotals: any) {
            if (!groupTotals.sum) {
                groupTotals.sum = {};
            }
            groupTotals.sum[this.field] = this.sum;
        }
    }
}

export namespace AggregateFormatting {
    function formatMarkup<TItem = any>(ctx: FormatterContext<IGroupTotals<TItem>>, aggType: string): FormatterResult {
        const textKey = (aggType.substring(0, 1).toUpperCase() + aggType.substring(1));
        const column = ctx.column;
        const value = (ctx.item as any)[aggType][column.field];
        const span = document.createElement("span");
        span.className = 'aggregate agg-' + aggType;
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
            var item = new NonDataRow();
            (item as any)[column.field] = value;
            try {
                fmtResult = formatter(formatterContext({ column, item, value, purpose: "grouptotal" }));
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

    export function groupTotalsFormat(ctx: FormatterContext<IGroupTotals>): FormatterResult {
        if (!ctx.item || !ctx.column)
            return "";

        let text: FormatterResult = null;

        ["sum", "avg", "min", "max", "cnt"].forEach(function (aggType) {
            if (text == null && (ctx.item as any)[aggType] && (ctx.item as any)[aggType][ctx.column.field] != null) {
                text = formatMarkup(ctx, aggType);
                return false;
            }
        });

        return text ?? "";
    }

    /** @deprecated use groupTotalsFormat */
    export function groupTotalsFormatter<TItem = any>(totals: IGroupTotals, column: Column<TItem>): string {
        if (!totals || !column)
            return "";

        const fmtResult = groupTotalsFormat(formatterContext<IGroupTotals<TItem>>({ item: totals, column, purpose: "grouptotal" }));
        const node = document.createElement("div");
        applyFormatterResultToCellNode(formatterContext({
            sanitizer: (dirtyHtml: string) => dirtyHtml
        }), fmtResult, node);
        return node.innerHTML;
    }
}