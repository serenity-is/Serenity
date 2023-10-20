import { escape, Column, GroupTotals, NonDataRow, convertCompatFormatter } from "@serenity-is/sleekgrid";
import { formatNumber, htmlEncode, tryGetText } from "../q";

export {}

export namespace Aggregators
{
    export function Avg(field: string): void {
        this.field_ = field;
        this.type_ = "Avg";

        this.init = function () {
            this.count_ = 0;
            this.nonNullCount_ = 0;
            this.sum_ = 0;
        };

        this.accumulate = function (item: any) {
            var val = item[this.field_];
            this.count_++;
            if (val != null && val !== "" && !isNaN(val)) {
                this.nonNullCount_++;
                this.sum_ += parseFloat(val);
            }
        };

        this.storeResult = function (groupTotals: any) {
            if (!groupTotals.avg) {
                groupTotals.avg = {};
            }
            if (this.nonNullCount_ != 0) {
                groupTotals.avg[this.field_] = this.sum_ / this.nonNullCount_;
            }
        };
    }

    export function WeightedAvg(field: string, weightedField: string) {
        this.field_ = field;
        this.type_ = "WeightedAvg";
        this.weightedField_ = weightedField;

        this.init = function () {
            this.sum_ = 0;
            this.weightedSum_ = 0;
        };

        this.accumulate = function (item: any) {
            var val = item[this.field_];
            var valWeighted = item[this.weightedField_];
            if (this.isValid(val) && this.isValid(valWeighted)) {
                this.weightedSum_ += parseFloat(valWeighted);
                this.sum_ += parseFloat(val) * parseFloat(valWeighted);
            }
        };

        this.storeResult = function (groupTotals: any) {
            if (!groupTotals.avg) {
                groupTotals.avg = {};
            }

            if (this.sum_ && this.weightedSum_) {
                groupTotals.avg[this.field_] = this.sum_ / this.weightedSum_;
            }
        };

        this.isValid = function (val: any) {
            return val !== null && val !== "" && !isNaN(val);
        };
    }

    export function Min(field: string): void {
        this.field_ = field;
        this.type_ = "Min";

        this.init = function () {
            this.min_ = null;
        };

        this.accumulate = function (item: any) {
            var val = item[this.field_];
            if (val != null && val !== "" && !isNaN(val)) {
                if (this.min_ == null || val < this.min_) {
                    this.min_ = val;
                }
            }
        };

        this.storeResult = function (groupTotals: any) {
            if (!groupTotals.min) {
                groupTotals.min = {};
            }
            groupTotals.min[this.field_] = this.min_;
        }
    }

    export function Max(field: string): void {
        this.field_ = field;
        this.type_ = "Max";

        this.init = function () {
            this.max_ = null;
        };

        this.accumulate = function (item: any) {
            var val = item[this.field_];
            if (val != null && val !== "" && !isNaN(val)) {
                if (this.max_ == null || val > this.max_) {
                    this.max_ = val;
                }
            }
        };

        this.storeResult = function (groupTotals: any) {
            if (!groupTotals.max) {
                groupTotals.max = {};
            }
            groupTotals.max[this.field_] = this.max_;
        }
    }

    export function Sum(field: string): void {
        this.field_ = field;
        this.type_ = "Sum";

        this.init = function () {
            this.sum_ = null;
        };

        this.accumulate = function (item: any) {
            var val = item[this.field_];
            if (val != null && val !== "" && !isNaN(val)) {
                this.sum_ += parseFloat(val);
            }
        };

        this.storeResult = function (groupTotals: any) {
            if (!groupTotals.sum) {
                groupTotals.sum = {};
            }
            groupTotals.sum[this.field_] = this.sum_;
        }
    }
}

export namespace AggregateFormatting {
    export function formatMarkup<TItem = any>(totals: GroupTotals, column: Column<TItem>, aggType: string): string {
        var textKey = (aggType.substring(0, 1).toUpperCase() + aggType.substring(1));
        var text = tryGetText("Enums.Serenity.SummaryType." + textKey) ?? textKey;
    
        var value = (totals as any)[aggType][column.field];
        var formattedValue = formatValue(column, value);
    
        return "<span class='aggregate agg-" + aggType + "'  title='" + htmlEncode(text) + "'>" +
            formattedValue +
            "</span>";
    }

    export function formatValue(column: Column, value: number): string {

        var formatter = column.format ?? (column.formatter ? convertCompatFormatter(column.formatter) : null);

        if (formatter != null) {
            var item = new NonDataRow();
            (item as any)[column.field] = value;
            try {
                return formatter({ column, escape, item, value });
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

    export function groupTotalsFormatter<TItem = any>(totals: GroupTotals, column: Column<TItem>): string {
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