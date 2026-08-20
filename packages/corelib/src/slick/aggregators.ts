import { IGroupTotals } from "@serenity-is/sleekgrid";
import { localText, SummaryType } from "../base";

/** Contract for group/total aggregators (avg/min/max/sum etc.). */
export interface IAggregator {
    /** Initializes state before a new group is processed. */
    init(): void;
    /** Accumulates a single item into the aggregator state. @param item - Row item. */
    accumulate(item: any): void;
    /** Writes computed totals into the group totals object. @param totals - Totals container keyed by aggregateKey. */
    storeResult(totals: IGroupTotals): void;
}

/** Built-in aggregator implementations. */
export namespace Aggregators {
    /** Average of a numeric field (ignores non-numeric / empty values). */
    export class Avg implements IAggregator {
        /** Number of items processed. */
        public count: number;
        /** Number of non-null numeric values. */
        public nonNullCount: number;
        /** Running sum of valid values. */
        public sum: number;

        /**
         * Creates a new average aggregator.
         * @param field - The field to average.
         */
        constructor(public readonly field: string) {
        }

        /** Initializes the aggregator state for a new group by resetting the item count, non-null count, and running sum to zero so the next group starts from a clean state. @inheritdoc */
        init() {
            this.count = 0;
            this.nonNullCount = 0;
            this.sum = 0;
        };

        /** Accumulates a single row item into the running average state for the configured field. @param item - Row item to accumulate; the configured field value is parsed as a number when valid. @inheritdoc */
        accumulate(item: any) {
            const val = item[this.field];
            this.count++;
            if (val != null && val !== "" && !isNaN(val)) {
                this.nonNullCount++;
                this.sum += typeof val === "number" ? val : parseFloat(val);
            }
        }

        /** Stores the computed average into the group totals container for the configured field. @param groupTotals - Totals container to write the computed average into, keyed by field name. @inheritdoc */
        storeResult(groupTotals: IGroupTotals) {
            if (!groupTotals.avg) {
                groupTotals.avg = {};
            }
            groupTotals.avg[this.field] = this.nonNullCount != 0 ? this.sum / this.nonNullCount : null;
        }

        /** Summary type for this aggregator. */
        static readonly summaryType = SummaryType.Avg;
        /** Key used to store/lookup this aggregator in totals. */
        static readonly aggregateKey = "avg";
        /** Localized display name for this aggregator. */
        static get displayName() {
            return localText("Enums.Serenity.SummaryType.Avg", "Avg")
        }
    }

    /** Weighted average given a value field and a weight field. */
    export class WeightedAvg implements IAggregator {
        /** Weighted sum of values. */
        public sum: number;
        /** Sum of weights. */
        public weightedSum: number;

        /**
         * Creates a new weighted average aggregator.
         * @param field - The value field name.
         * @param weightedField - The weight field name.
         */
        constructor(public readonly field: string,
            public readonly weightedField: string) {
        }

        /** Initializes the weighted average aggregator state for a new group by resetting the weighted value sum and total weight to zero for the next group's calculation. @inheritdoc */
        init() {
            this.sum = 0;
            this.weightedSum = 0;
        }

        /** Accumulates a single row item into the weighted average state using the value and weight fields. @param item - Row item to accumulate; both value and weight fields must contain valid numeric values. @inheritdoc */
        accumulate(item: any) {
            const val = item[this.field];
            let valWeighted = item[this.weightedField];
            if (WeightedAvg.isValid(val) && WeightedAvg.isValid(valWeighted)) {
                valWeighted = typeof valWeighted === "number" ? valWeighted : parseFloat(valWeighted);
                this.weightedSum += valWeighted;
                this.sum += valWeighted * (typeof val === "number" ? val : parseFloat(val));
            }
        }

        /** Stores the computed weighted average into the group totals container for the configured field. @param groupTotals - Totals container to write the computed weighted average into, keyed by field name. @inheritdoc */
        storeResult(groupTotals: any) {
            if (!groupTotals.avg) {
                groupTotals.avg = {};
            }
            if (this.sum && this.weightedSum) {
                groupTotals.avg[this.field] = this.sum / this.weightedSum;
            }
        }

        /**
         * Checks if a value is valid for aggregation.
         * @param val - The value to check.
         * @returns True if valid.
         */
        static isValid(val: any): boolean {
            return val !== null && val !== "" && !isNaN(val);
        }

        /** Key used to store/lookup this aggregator in totals. */
        static readonly aggregateKey = "weightedAvg";
        /** Localized display name for this aggregator. */
        static get displayName() {
            return localText("Enums.Serenity.SummaryType.WeightedAvg", "Weighted Avg");
        }
    }

    /** Minimum of a field. */
    export class Min implements IAggregator {
        /** The field to aggregate. */
        public readonly field: string;
        /** Current minimum value. */
        public min: any;

        /**
         * Creates a new minimum aggregator.
         * @param field - The field to aggregate.
         */
        constructor(field: string) {
            this.field = field;
        }

        /** Initializes the minimum aggregator state for a new group by resetting the tracked minimum value to null so the next group can determine its own minimum. @inheritdoc */
        init() {
            this.min = null;
        }

        /** Accumulates a single row item by updating the tracked minimum when the field value is smaller. @param item - Row item to accumulate; numeric field values are compared against the current minimum. @inheritdoc */
        accumulate(item: any) {
            const val = item[this.field];
            if (val != null && val !== "" && !isNaN(val)) {
                if (this.min == null || val < this.min) {
                    this.min = val;
                }
            }
        }

        /** Stores the computed minimum value into the group totals container for the configured field. @param groupTotals - Totals container to write the computed minimum into, keyed by field name. @inheritdoc */
        storeResult(groupTotals: any) {
            if (!groupTotals.min) {
                groupTotals.min = {};
            }
            groupTotals.min[this.field] = this.min;
        }

        /** Summary type for this aggregator. */
        static readonly summaryType = SummaryType.Min;
        /** Key used to store/lookup this aggregator in totals. */
        static readonly aggregateKey = "min";
        /** Localized display name for this aggregator. */
        static get displayName() {
            return localText("Enums.Serenity.SummaryType.Min", "Min");
        }
    }

    /** Maximum of a field. */
    export class Max implements IAggregator {
        /** Current maximum value. */
        public max: any;

        /**
         * Creates a new maximum aggregator.
         * @param field - The field to aggregate.
         */
        constructor(public readonly field: string) {
        }

        /** Initializes the maximum aggregator state for a new group by resetting the tracked maximum value to null so the next group can determine its own maximum. @inheritdoc */
        init() {
            this.max = null;
        }

        /** Accumulates a single row item by updating the tracked maximum when the field value is larger. @param item - Row item to accumulate; numeric field values are compared against the current maximum. @inheritdoc */
        accumulate(item: any) {
            const val = item[this.field];
            if (val != null && val !== "" && !isNaN(val)) {
                if (this.max == null || val > this.max) {
                    this.max = val;
                }
            }
        }

        /** Stores the computed maximum value into the group totals container for the configured field. @param groupTotals - Totals container to write the computed maximum into, keyed by field name. @inheritdoc */
        storeResult(groupTotals: any) {
            if (!groupTotals.max) {
                groupTotals.max = {};
            }
            groupTotals.max[this.field] = this.max;
        }

        /** Summary type for this aggregator. */
        static readonly summaryType = SummaryType.Max;
        /** Key used to store/lookup this aggregator in totals. */
        static readonly aggregateKey = "max";
        /** Localized display name for this aggregator. */
        static get displayName() {
            return localText("Enums.Serenity.SummaryType.Max", "Max");
        }
    }

    /** Sum of a numeric field. */
    export class Sum implements IAggregator {
        /** The field to aggregate. */
        public readonly field: string;
        /** Running sum of valid values. */
        public sum: number;

        /**
         * Creates a new sum aggregator.
         * @param field - The field to aggregate.
         */
        constructor(field: string) {
            this.field = field;
        }

        /** Initializes the sum aggregator state for a new group by resetting the running sum to zero so aggregation starts fresh for the next group. @inheritdoc */
        init() {
            this.sum = 0;
        }

        /** Accumulates a single row item into the running sum for the configured field. @param item - Row item to accumulate; the configured field value is parsed as a number when valid. @inheritdoc */
        accumulate(item: any) {
            const val = item[this.field];
            if (val != null && val !== "" && !isNaN(val)) {
                this.sum += typeof val === "number" ? val : parseFloat(val);
            }
        }

        /** Stores the computed sum into the group totals container for the configured field. @param groupTotals - Totals container to write the computed sum into, keyed by field name. @inheritdoc */
        storeResult(groupTotals: any) {
            if (!groupTotals.sum) {
                groupTotals.sum = {};
            }
            groupTotals.sum[this.field] = this.sum;
        }

        /** Summary type for this aggregator. */
        static readonly summaryType = SummaryType.Sum;
        /** Key used to store/lookup this aggregator in totals. */
        static readonly aggregateKey = "sum";
        /** Localized display name for this aggregator. */
        static get displayName() {
            return localText("Enums.Serenity.SummaryType.Sum", "Sum");
        }
    }
}