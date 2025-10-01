import { SummaryType } from "../base";
import { Aggregators, IAggregator } from "./aggregators";

export interface IAggregatorConstructor {
    new(field: string, ...args: any[]): IAggregator;
    aggregateType?: string;
    displayName?: string;
    summaryType?: SummaryType;
}

export namespace AggregatorTypeRegistry {
    let byAggregateType: { [key: string]: IAggregatorConstructor } = Object.create(null);

    export function register(cls: IAggregatorConstructor) {
        byAggregateType[cls.aggregateType ?? (cls.name.toLowerCase())] = cls;
        if (cls.summaryType != null && cls.summaryType !== SummaryType.Disabled && cls.summaryType !== SummaryType.None)
            byAggregateType[cls.summaryType] = cls;
    }

    export function reset() {
        byAggregateType = Object.create(null);
        for (const cls of Object.keys(Aggregators).map(k => (Aggregators as any)[k]) as IAggregatorConstructor[]) {
            if (cls.aggregateType != null || cls.summaryType != null)
                AggregatorTypeRegistry.register(cls);
        }
    }

    export function tryGet(type: string | SummaryType): IAggregatorConstructor | undefined {
        if (type == null || type == SummaryType.Disabled || type == SummaryType.None)
            return void 0;

        return byAggregateType[type];
    }
}

AggregatorTypeRegistry.reset();
