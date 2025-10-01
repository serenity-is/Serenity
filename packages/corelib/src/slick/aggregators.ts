import { IGroupTotals } from "@serenity-is/sleekgrid";
import { localText, SummaryType } from "../base";

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

        static readonly summaryType = SummaryType.Avg;
        static readonly aggregateKey = "avg";
        static get displayName() {
            return localText("Enums.Serenity.SummaryType.Avg")
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

        static readonly aggregateKey = "weightedAvg";
        static get displayName() {
            return localText("Enums.Serenity.SummaryType.WeightedAvg", "Weighted Avg");
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

        static readonly summaryType = SummaryType.Min;
        static readonly aggregateKey = "min";
        static get displayName() {
            return localText("Enums.Serenity.SummaryType.Min", "Min");
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

        static readonly summaryType = SummaryType.Max;
        static readonly aggregateKey = "max";
        static get displayName() {
            return localText("Enums.Serenity.SummaryType.Max", "Max");
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

        static readonly summaryType = SummaryType.Sum;
        static readonly aggregateKey = "sum";
        static get displayName() {
            return localText("Enums.Serenity.SummaryType.Sum", "Sum");
        }
    }
}