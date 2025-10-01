import { AggregatorTypeRegistry } from "./aggregatortyperegistry";
import { SummaryType } from "../base";

describe('AggregatorTypeRegistry', () => {

    it('should retrieve aggregators for all standard aggregate types', () => {
        const types = ['avg', 'weightedAvg', 'min', 'max', 'sum'];
        for (const type of types) {
            const aggregator = AggregatorTypeRegistry.tryGet(type);
            expect(aggregator).toBeDefined();
            expect(aggregator?.aggregateKey).toBe(type);
        }
    });

    it('should retrieve aggregators by SummaryType', () => {
        const sumAggregator = AggregatorTypeRegistry.tryGet(SummaryType.Sum);
        expect(sumAggregator).toBeDefined();
        expect(sumAggregator?.aggregateKey).toBe('sum');

        const avgAggregator = AggregatorTypeRegistry.tryGet(SummaryType.Avg);
        expect(avgAggregator).toBeDefined();
        expect(avgAggregator?.aggregateKey).toBe('avg');

        const minAggregator = AggregatorTypeRegistry.tryGet(SummaryType.Min);
        expect(minAggregator).toBeDefined();
        expect(minAggregator?.aggregateKey).toBe('min');

        const maxAggregator = AggregatorTypeRegistry.tryGet(SummaryType.Max);
        expect(maxAggregator).toBeDefined();
        expect(maxAggregator?.aggregateKey).toBe('max');
    });

    it('should return undefined for invalid aggregate types', () => {
        const aggregator = AggregatorTypeRegistry.tryGet('invalid');
        expect(aggregator).toBeUndefined();
    });

    it('should return undefined for null, undefined, Disabled, or None SummaryType', () => {
        expect(AggregatorTypeRegistry.tryGet(null)).toBeUndefined();
        expect(AggregatorTypeRegistry.tryGet(undefined)).toBeUndefined();
        expect(AggregatorTypeRegistry.tryGet(SummaryType.Disabled)).toBeUndefined();
        expect(AggregatorTypeRegistry.tryGet(SummaryType.None)).toBeUndefined();
    });

    it('should allow registering custom aggregators', () => {
        class CustomAggregator {
            static aggregateKey = 'custom';
            constructor(public field: string) {}
            init() {}
            accumulate() {}
            storeResult() {}
        }

        AggregatorTypeRegistry.register(CustomAggregator);
        const aggregator = AggregatorTypeRegistry.tryGet('custom');
        expect(aggregator).toBe(CustomAggregator);
    });

    it('should reset the registry and re-register standard aggregators', () => {
        // First, register a custom one
        class TempAggregator {
            static aggregateKey = 'temp';
            constructor(public field: string) {}
            init() {}
            accumulate() {}
            storeResult() {}
        }
        AggregatorTypeRegistry.register(TempAggregator);
        expect(AggregatorTypeRegistry.tryGet('temp')).toBe(TempAggregator);

        // Reset
        AggregatorTypeRegistry.reset();

        // Custom one should be gone
        expect(AggregatorTypeRegistry.tryGet('temp')).toBeUndefined();

        // Standard ones should be back
        expect(AggregatorTypeRegistry.tryGet('sum')).toBeDefined();
    });
    
});