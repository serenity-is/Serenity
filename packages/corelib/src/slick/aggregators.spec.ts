import { localText } from "../base";
import { AggregateFormatting, Aggregators } from "./aggregators";

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

describe("AggregateFormatting", () => {
    describe("formatValue", () => {
        it("formats number with displayFormat", () => {
            const column = {
                field: "price",
                sourceItem: { displayFormat: "#,##0.00" }
            } as any;

            const result = AggregateFormatting.formatValue(column, 1234.56);
            expect(result).toBe("1,234.56");
        });

        it("uses default displayFormat when not specified", () => {
            const column = {
                field: "price"
            } as any;

            const result = AggregateFormatting.formatValue(column, 1234.567);
            expect(result).toBe("1,234.57");
        });

        it("uses formatter when available", () => {
            const mockFormatter = vi.fn().mockReturnValue("formatted value");
            const column = {
                field: "price",
                format: mockFormatter
            } as any;

            const result = AggregateFormatting.formatValue(column, 1234.56);

            expect(mockFormatter).toHaveBeenCalledWith({
                column,
                escape: expect.any(Function),
                sanitizer: expect.any(Function),
                item: expect.any(Object),
                value: 1234.56,
                purpose: "grouptotal"
            });
            expect(result).toBe("formatted value");
        });

        it("handles Element return from formatter", () => {
            const element = document.createElement("span");
            element.textContent = "formatted";
            const mockFormatter = vi.fn().mockReturnValue(element);
            const column = {
                field: "price",
                format: mockFormatter
            } as any;

            const result = AggregateFormatting.formatValue(column, 1234.56);
            expect(result).toBe("<span>formatted</span>");
        });

        it("handles DocumentFragment return from formatter", () => {
            const fragment = document.createDocumentFragment();
            const span = document.createElement("span");
            span.textContent = "test";
            fragment.appendChild(span);
            fragment.appendChild(document.createTextNode(" text"));

            const mockFormatter = vi.fn().mockReturnValue(fragment);
            const column = {
                field: "price",
                format: mockFormatter
            } as any;

            const result = AggregateFormatting.formatValue(column, 1234.56);
            expect(result).toBe("<span>test</span> text");
        });

        it("handles formatter exceptions gracefully", () => {
            const mockFormatter = vi.fn().mockImplementation(() => {
                throw new Error("formatter error");
            });
            const column = {
                field: "price",
                format: mockFormatter
            } as any;

            const result = AggregateFormatting.formatValue(column, 1234.56);
            expect(result).toBe("1,234.56"); // falls back to number formatting
        });

        it("html encodes non-numeric values", () => {
            const column = {
                field: "name"
            } as any;

            const result = AggregateFormatting.formatValue(column, "test&value" as any);
            expect(result).toBe("test&amp;value");
        });
    });

    describe("formatMarkup", () => {
        it("formats markup with localized text", () => {
            const totals = {
                sum: { price: 100 }
            } as any;
            const column = {
                field: "price"
            } as any;

            (localText as any).mockReturnValue("Sum");            
            const result = AggregateFormatting.formatMarkup(totals, column, "sum");

            expect(result).toContain("class='aggregate agg-sum'");
            expect(result).toContain("title='Sum'");
            expect(result).toContain("100");
        });

        it("uses localized text when available", () => {
            // Mock tryGetText to return localized text
            (localText as any).mockReturnValue("Total Sum");

            const totals = {
                sum: { price: 100 }
            } as any;
            const column = {
                field: "price"
            } as any;

            const result = AggregateFormatting.formatMarkup(totals, column, "sum");

            expect(result).toContain("title='Total Sum'");
            expect(localText).toHaveBeenCalledWith("Enums.Serenity.SummaryType.Sum", "Sum");
        });
    });

    describe("groupTotalsFormatter", () => {
        it("returns empty string for null/undefined inputs", () => {
            expect(AggregateFormatting.groupTotalsFormatter(null, null)).toBe("");
            expect(AggregateFormatting.groupTotalsFormatter({} as any, null)).toBe("");
            expect(AggregateFormatting.groupTotalsFormatter(null, {} as any)).toBe("");
        });

        it("formats first available aggregate type", () => {
            const totals = {
                sum: { price: 100 },
                avg: { price: 50 }
            } as any;
            const column = {
                field: "price"
            } as any;

            const result = AggregateFormatting.groupTotalsFormatter(totals, column);
            expect(result).toContain("agg-sum"); // Should use sum first
        });

        it("prioritizes aggregates in order: sum, avg, min, max, cnt", () => {
            const column = {
                field: "price"
            } as any;

            // Test avg when sum is not available
            const totalsAvg = {
                avg: { price: 50 }
            } as any;
            const resultAvg = AggregateFormatting.groupTotalsFormatter(totalsAvg, column);
            expect(resultAvg).toContain("agg-avg");

            // Test min when sum and avg are not available
            const totalsMin = {
                min: { price: 10 }
            } as any;
            const resultMin = AggregateFormatting.groupTotalsFormatter(totalsMin, column);
            expect(resultMin).toContain("agg-min");

            // Test max when others are not available
            const totalsMax = {
                max: { price: 100 }
            } as any;
            const resultMax = AggregateFormatting.groupTotalsFormatter(totalsMax, column);
            expect(resultMax).toContain("agg-max");

            // Test cnt when others are not available
            const totalsCnt = {
                cnt: { price: 5 }
            } as any;
            const resultCnt = AggregateFormatting.groupTotalsFormatter(totalsCnt, column);
            expect(resultCnt).toContain("agg-cnt");
        });

        it("returns empty string when no aggregates available", () => {
            const totals = {
                sum: {}
            } as any;
            const column = {
                field: "price"
            } as any;

            const result = AggregateFormatting.groupTotalsFormatter(totals, column);
            expect(result).toBe("");
        });
    });
});