import { Column, IGroupTotals, NonDataRow, convertCompatFormatter, formatterContext } from "@serenity-is/sleekgrid";
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
    export function formatMarkup<TItem = any>(totals: IGroupTotals, column: Column<TItem>, aggType: string): string {
        var textKey = (aggType.substring(0, 1).toUpperCase() + aggType.substring(1));
        var text = localText("Enums.Serenity.SummaryType." + textKey, textKey);

        var value = (totals as any)[aggType][column.field];
        var formattedValue = formatValue(column, value);

        return "<span class='aggregate agg-" + aggType + "'  title='" + htmlEncode(text) + "'>" +
            formattedValue +
            "</span>";
    }

    export function formatValue(column: Column, value: number): string {

        var formatter = column.format ?? ((column as any).formatter ? convertCompatFormatter((column as any).formatter) : null);

        if (formatter != null) {
            var item = new NonDataRow();
            (item as any)[column.field] = value;
            try {
                var result = formatter(formatterContext({ column, item, value, purpose: "grouptotal" }));
                if (result instanceof Element)
                    return result.outerHTML;
                else if (result instanceof DocumentFragment)
                    return Array.from((result as any as DocumentFragment).childNodes)
                        .map(x => x instanceof Element ? x.outerHTML : (x instanceof Text ? htmlEncode(x.textContent) : '')).join("")
                return result;
            }
            catch (e) {
            }
        }

        if (typeof value === "number") {
            var displayFormat = column.sourceItem?.displayFormat ?? "#,##0.##";
            return htmlEncode(formatNumber(value, displayFormat));
        }
        else
            return htmlEncode(value);
    }

    export function groupTotalsFormatter<TItem = any>(totals: IGroupTotals, column: Column<TItem>): string {
        if (!totals || !column)
            return "";

        var text: string = null;

        ["sum", "avg", "min", "max", "cnt"].forEach(function (aggType) {
            if (text == null && (totals as any)[aggType] && (totals as any)[aggType][column.field] != null) {
                text = formatMarkup(totals, column, aggType);
                return false;
            }
        });

        return text || "";
    }
}