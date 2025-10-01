import { SummaryType } from "../base";
import { Aggregators, IAggregator } from "./aggregators";

export interface IAggregatorConstructor {
    new(field: string, ...args: any[]): IAggregator;
    /**
     * A unique key for the aggregator (like 'sum', 'avg', etc.). This is also used in the totals object
     * as a property key to store the results of this aggregator.
     */
    aggregateKey?: string;
    /**
     * A user-friendly display name for the aggregator (like "Sum", "Average", etc.)
     */
    displayName?: string;
    /**
     * Corresponding SummaryType enum value (like SummaryType.Sum, SummaryType.Avg, etc.),
     * if any.
     */
    summaryType?: SummaryType;
}

export namespace AggregatorTypeRegistry {
    let byKey: { [key: string]: IAggregatorConstructor } = Object.create(null);

    /**
     * Registers a new aggregator class.
     * @param cls The aggregator class to register
     */
    export function register(cls: IAggregatorConstructor) {
        byKey[cls.aggregateKey ?? (cls.name.toLowerCase())] = cls;
        if (cls.summaryType != null && cls.summaryType !== SummaryType.Disabled && cls.summaryType !== SummaryType.None)
            byKey[cls.summaryType] = cls;
    }

    /**
     * Resets the registry by clearing all registered aggregators and re-registering the standard ones.
     */
    export function reset() {
        byKey = Object.create(null);
        for (const cls of Object.keys(Aggregators).map(k => (Aggregators as any)[k]) as IAggregatorConstructor[]) {
            if (cls.aggregateKey != null || cls.summaryType != null)
                AggregatorTypeRegistry.register(cls);
        }
    }

    /**
     * Tries to get an aggregator constructor by its SummaryType or unique key.
     * @param aggKey The SummaryType enum value or the unique key of the aggregator (like 'sum', 'avg', etc.)
     * @returns The corresponding aggregator constructor, or undefined if not found.
     */
    export function tryGet(aggKey: string | SummaryType): IAggregatorConstructor | undefined {
        if (aggKey == null || aggKey == SummaryType.Disabled || aggKey == SummaryType.None)
            return void 0;

        return byKey[aggKey];
    }
}

AggregatorTypeRegistry.reset();
