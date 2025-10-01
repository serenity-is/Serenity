import { Aggregators } from "./aggregators";

vi.mock("../base", () => ({
    tryGetText: vi.fn(),
    localText: vi.fn(),
    formatNumber: vi.fn((value, format) => {
        if (format === '#,##0.00') {
            return value.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        }
        if (format === '#,##0.##') {
            return value.toLocaleString('en-US', { minimumFractionDigits: 0, maximumFractionDigits: 2 });
        }
        return value.toString();
    }),
    htmlEncode: vi.fn((value) => {
        if (value == null) return "";
        return String(value)
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#39;');
    })
}));

describe("Aggregators", () => {
    describe("Avg", () => {
        it("initializes with field and type", () => {
            const avg = new Aggregators.Avg("price");
            expect(avg.field).toBe("price");
        });

        it("initializes counters in init", () => {
            const avg = new Aggregators.Avg("price");
            avg.init();
            expect(avg.count).toBe(0);
            expect(avg.nonNullCount).toBe(0);
            expect(avg.sum).toBe(0);
        });

        it("accumulates valid numeric values", () => {
            const avg = new Aggregators.Avg("price");
            avg.init();

            avg.accumulate({ price: 10 });
            expect(avg.count).toBe(1);
            expect(avg.nonNullCount).toBe(1);
            expect(avg.sum).toBe(10);

            avg.accumulate({ price: 20 });
            expect(avg.count).toBe(2);
            expect(avg.nonNullCount).toBe(2);
            expect(avg.sum).toBe(30);
        });

        it("ignores null, empty, and non-numeric values", () => {
            const avg = new Aggregators.Avg("price");
            avg.init();

            avg.accumulate({ price: null });
            avg.accumulate({ price: "" });
            avg.accumulate({ price: "abc" });
            avg.accumulate({ price: NaN });

            expect(avg.count).toBe(4);
            expect(avg.nonNullCount).toBe(0);
            expect(avg.sum).toBe(0);
        });

        it("stores average result in groupTotals", () => {
            const avg = new Aggregators.Avg("price");
            avg.init();
            avg.accumulate({ price: 10 });
            avg.accumulate({ price: 20 });

            const groupTotals: any = {};
            avg.storeResult(groupTotals);

            expect(groupTotals.avg.price).toBe(15);
        });

        it("does not store result when no valid values", () => {
            const avg = new Aggregators.Avg("price");
            avg.init();
            avg.accumulate({ price: null });

            const groupTotals: any = {};
            avg.storeResult(groupTotals);

            expect(groupTotals.avg).toEqual({ price: null});
        });
    });

    describe("WeightedAvg", () => {
        it("initializes with field, weightedField and type", () => {
            const weightedAvg = new Aggregators.WeightedAvg("price", "weight");
            expect(weightedAvg.field).toBe("price");
            expect(weightedAvg.weightedField).toBe("weight");
        });

        it("initializes sums in init", () => {
            const weightedAvg = new Aggregators.WeightedAvg("price", "weight");
            weightedAvg.init();
            expect(weightedAvg.sum).toBe(0);
            expect(weightedAvg.weightedSum).toBe(0);
        });

        it("accumulates valid weighted values", () => {
            const weightedAvg = new Aggregators.WeightedAvg("price", "weight");
            weightedAvg.init();

            weightedAvg.accumulate({ price: 10, weight: 2 });
            expect(weightedAvg.sum).toBe(20); // 10 * 2
            expect(weightedAvg.weightedSum).toBe(2);

            weightedAvg.accumulate({ price: 20, weight: 3 });
            expect(weightedAvg.sum).toBe(80); // 20 * 3 + 20
            expect(weightedAvg.weightedSum).toBe(5); // 2 + 3
        });

        it("ignores invalid values", () => {
            const weightedAvg = new Aggregators.WeightedAvg("price", "weight");
            weightedAvg.init();

            weightedAvg.accumulate({ price: null, weight: 2 });
            weightedAvg.accumulate({ price: 10, weight: null });
            weightedAvg.accumulate({ price: "", weight: 2 });
            weightedAvg.accumulate({ price: 10, weight: "" });

            expect(weightedAvg.sum).toBe(0);
            expect(weightedAvg.weightedSum).toBe(0);
        });

        it("stores weighted average result", () => {
            const weightedAvg = new Aggregators.WeightedAvg("price", "weight");
            weightedAvg.init();
            weightedAvg.accumulate({ price: 10, weight: 2 }); // 20
            weightedAvg.accumulate({ price: 20, weight: 3 }); // 60, total sum=80, weightedSum=5

            const groupTotals: any = {};
            weightedAvg.storeResult(groupTotals);

            expect(groupTotals.avg.price).toBe(16); // 80 / 5
        });

        it("isValid returns true for valid numbers", () => {
            const weightedAvg = new Aggregators.WeightedAvg("price", "weight");
            expect(Aggregators.WeightedAvg.isValid(10)).toBe(true);
            expect(Aggregators.WeightedAvg.isValid("10")).toBe(true);
            expect(Aggregators.WeightedAvg.isValid(0)).toBe(true);
        });

        it("isValid returns false for invalid values", () => {
            const weightedAvg = new Aggregators.WeightedAvg("price", "weight");
            expect(Aggregators.WeightedAvg.isValid(null)).toBe(false);
            expect(Aggregators.WeightedAvg.isValid("")).toBe(false);
            expect(Aggregators.WeightedAvg.isValid(NaN)).toBe(false);
            expect(Aggregators.WeightedAvg.isValid("abc")).toBe(false);
        });
    });

    describe("Min", () => {
        it("initializes with field and type", () => {
            const min = new Aggregators.Min("price");
            expect(min.field).toBe("price");
        });

        it("initializes min to null in init", () => {
            const min = new Aggregators.Min("price");
            min.init();
            expect(min.min).toBe(null);
        });

        it("finds minimum value", () => {
            const min = new Aggregators.Min("price");
            min.init();

            min.accumulate({ price: 20 });
            expect(min.min).toBe(20);

            min.accumulate({ price: 10 });
            expect(min.min).toBe(10);

            min.accumulate({ price: 30 });
            expect(min.min).toBe(10);
        });

        it("ignores null, empty, and non-numeric values", () => {
            const min = new Aggregators.Min("price");
            min.init();

            min.accumulate({ price: null });
            min.accumulate({ price: "" });
            min.accumulate({ price: "abc" });

            expect(min.min).toBe(null);
        });

        it("stores minimum result", () => {
            const min = new Aggregators.Min("price");
            min.init();
            min.accumulate({ price: 20 });
            min.accumulate({ price: 10 });

            const groupTotals: any = {};
            min.storeResult(groupTotals);

            expect(groupTotals.min.price).toBe(10);
        });
    });

    describe("Max", () => {
        it("initializes with field and type", () => {
            const max = new Aggregators.Max("price");
            expect(max.field).toBe("price");
        });

        it("initializes max to null in init", () => {
            const max = new Aggregators.Max("price");
            max.init();
            expect(max.max).toBe(null);
        });

        it("finds maximum value", () => {
            const max = new Aggregators.Max("price");
            max.init();

            max.accumulate({ price: 10 });
            expect(max.max).toBe(10);

            max.accumulate({ price: 30 });
            expect(max.max).toBe(30);

            max.accumulate({ price: 20 });
            expect(max.max).toBe(30);
        });

        it("ignores null, empty, and non-numeric values", () => {
            const max = new Aggregators.Max("price");
            max.init();

            max.accumulate({ price: null });
            max.accumulate({ price: "" });
            max.accumulate({ price: "abc" });

            expect(max.max).toBe(null);
        });

        it("stores maximum result", () => {
            const max = new Aggregators.Max("price");
            max.init();
            max.accumulate({ price: 10 });
            max.accumulate({ price: 30 });

            const groupTotals: any = {};
            max.storeResult(groupTotals);

            expect(groupTotals.max.price).toBe(30);
        });
    });

    describe("Sum", () => {
        it("initializes with field and type", () => {
            const sum = new Aggregators.Sum("price");
            expect(sum.field).toBe("price");
        });

        it("initializes sum to null in init", () => {
            const sum = new Aggregators.Sum("price");
            sum.init();
            expect(sum.sum).toBe(0);
        });

        it("accumulates sum of valid values", () => {
            const sum = new Aggregators.Sum("price");
            sum.init();

            sum.accumulate({ price: 10 });
            expect(sum.sum).toBe(10);

            sum.accumulate({ price: 20 });
            expect(sum.sum).toBe(30);
        });

        it("ignores null, empty, and non-numeric values", () => {
            const sum = new Aggregators.Sum("price");
            sum.init();

            sum.accumulate({ price: null });
            sum.accumulate({ price: "" });
            sum.accumulate({ price: "abc" });

            expect(sum.sum).toBe(0);
        });

        it("stores sum result", () => {
            const sum = new Aggregators.Sum("price");
            sum.init();
            sum.accumulate({ price: 10 });
            sum.accumulate({ price: 20 });

            const groupTotals: any = {};
            sum.storeResult(groupTotals);

            expect(groupTotals.sum.price).toBe(30);
        });
    });
});
