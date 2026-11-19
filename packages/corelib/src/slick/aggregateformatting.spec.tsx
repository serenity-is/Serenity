import { formatterContext, gridDefaults } from "@serenity-is/sleekgrid";
import { describe, expect, it, vi } from "vitest";
import { AggregateFormatting } from "./aggregateformatting";

// Mock localText to return simple names for testing
vi.mock("../base", async (importOriginal) => {
    const actual = await importOriginal() as any;
    return {
        ...actual,
        localText: vi.fn((key: string, defaultValue?: string) => {
            if (key === "Enums.Serenity.SummaryType.Sum") return "Sum";
            if (key === "Enums.Serenity.SummaryType.Avg") return "Avg";
            if (key === "Enums.Serenity.SummaryType.Min") return "Min";
            if (key === "Enums.Serenity.SummaryType.Max") return "Max";
            return defaultValue || key;
        }),
        formatNumber: (value: number, format?: string) => {
            if (typeof value === "number") {
                return value.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
            }
            return String(value);
        }
    };
});

// Mock gridDefaults
vi.mock("@serenity-is/sleekgrid", async () => {
    const actual = await vi.importActual("@serenity-is/sleekgrid");
    return {
        ...actual,
        gridDefaults: {},
        formatterContext: (actual as any).formatterContext,
        applyFormatterResultToCellNode: (actual as any).applyFormatterResultToCellNode,
        convertCompatFormatter: (actual as any).convertCompatFormatter,
        FormatterContext: (actual as any).FormatterContext,
        FormatterResult: (actual as any).FormatterResult,
        IGroupTotals: (actual as any).IGroupTotals,
        NonDataRow: (actual as any).NonDataRow
    };
});

describe("AggregateFormatting.groupTotalsFormat", () => {

    it("should format sum correctly", () => {
        const result = AggregateFormatting.groupTotalsFormat(formatterContext({
            item: {
                sum: {
                    Amount: 1234.56
                }
            },
            column: {
                field: "Amount",
                summaryType: "sum"
            }
        }));

        expect(result).toStrictEqual(<span class="aggregate agg-sum" title="Sum">1,234.56</span>);
    });

    it("should format average correctly", () => {
        const result = AggregateFormatting.groupTotalsFormat(formatterContext({
            item: {
                avg: {
                    Amount: 123.456
                }
            },
            column: {
                field: "Amount",
                summaryType: "avg"
            }
        }));

        expect(result).toStrictEqual(<span class="aggregate agg-avg" title="Avg">123.46</span>);
    });

    it("should format minimum correctly", () => {
        const result = AggregateFormatting.groupTotalsFormat(formatterContext({
            item: {
                min: {
                    Amount: 100
                }
            },
            column: {
                field: "Amount",
                summaryType: "min"
            }
        }));

        expect(result).toStrictEqual(<span class="aggregate agg-min" title="Min">100.00</span>);
    });

    it("should format maximum correctly", () => {
        const result = AggregateFormatting.groupTotalsFormat(formatterContext({
            item: {
                max: {
                    Amount: 500
                }
            },
            column: {
                field: "Amount",
                summaryType: "max"
            }
        }));

        expect(result).toStrictEqual(<span class="aggregate agg-max" title="Max">500.00</span>);
    });

    it("should infer aggregator type when summaryType is not provided", () => {
        // note that column might not have summaryType if CustomSummaryMixin is not used
        const result = AggregateFormatting.groupTotalsFormat(formatterContext({
            item: {
                avg: {
                    Amount: 250.5
                }
            },
            column: {
                field: "Amount"
            }
        }));

        expect(result).toStrictEqual(<span class="aggregate agg-avg" title="Avg">250.50</span>);
    });

    it("should use custom formatter when provided", () => {
        const customFormatter = vi.fn(() => "Custom: 999");
        const result = AggregateFormatting.groupTotalsFormat(formatterContext({
            item: {
                sum: {
                    Amount: 1234.56
                }
            },
            column: {
                field: "Amount",
                summaryType: "sum",
                format: customFormatter
            }
        }));

        expect(customFormatter).toHaveBeenCalled();
        expect(result).toStrictEqual(<span class="aggregate agg-sum" title="Sum">Custom: 999</span>);
    });

    it("should fallback to default formatting when custom formatter throws error", () => {
        const errorFormatter = vi.fn(() => { throw new Error("Formatter error"); });
        const result = AggregateFormatting.groupTotalsFormat(formatterContext({
            item: {
                sum: {
                    Amount: 1234.56
                }
            },
            column: {
                field: "Amount",
                summaryType: "sum",
                format: errorFormatter
            }
        }));

        expect(errorFormatter).toHaveBeenCalled();
        expect(result).toStrictEqual(<span class="aggregate agg-sum" title="Sum">1,234.56</span>);
    });

    it("should handle non-numeric values", () => {
        const result = AggregateFormatting.groupTotalsFormat(formatterContext({
            item: {
                sum: {
                    Name: "Total Items"
                }
            },
            column: {
                field: "Name",
                summaryType: "sum"
            }
        }));

        expect(result).toStrictEqual(<span class="aggregate agg-sum" title="Sum">Total Items</span>);
    });

    it("should use displayFormat from sourceItem when available", () => {
        const result = AggregateFormatting.groupTotalsFormat(formatterContext({
            item: {
                sum: {
                    Amount: 1234.56
                }
            },
            column: {
                field: "Amount",
                summaryType: "sum",
                sourceItem: {
                    name: "Amount",
                    displayFormat: "#,##0.00"
                }
            }
        }));

        expect(result).toStrictEqual(<span class="aggregate agg-sum" title="Sum">1,234.56</span>);
    });

    it("should return empty string when no totals provided", () => {
        // note that column might not have summaryType if CustomSummaryMixin is not used
        const result = AggregateFormatting.groupTotalsFormat(formatterContext({
            item: null,
            column: {
                field: "Amount",
                summaryType: "sum"
            }
        }));

        expect(result).toBe("");
    });

    it("should return empty string when no field provided", () => {
        const result = AggregateFormatting.groupTotalsFormat(formatterContext({
            item: {
                sum: {
                    Amount: 1234.56
                }
            },
            column: {
                summaryType: "sum"
            }
        }));

        expect(result).toStrictEqual(<span class="aggregate-disabled" />);
    });

    it("should return empty string when no aggregator key found", () => {
        const result = AggregateFormatting.groupTotalsFormat(formatterContext({
            item: {
                "": {
                    Amount: 1234.56
                }
            } as any,
            column: {
                field: "Amount"
            }
        }));

        expect(result).toBe("");
    });

    it("should handle null values in totals", () => {
        const result = AggregateFormatting.groupTotalsFormat(formatterContext({
            item: {
                avg: {
                    Amount: null
                }
            },
            column: {
                field: "Amount",
                summaryType: "avg"
            }
        }));

        expect(result).toStrictEqual(<span class="aggregate agg-avg" title="Avg"></span>);
    });

    it("should return empty string when column has no summaryType and no totals for the column are available", () => {
        // note that column might not have summaryType if CustomSummaryMixin is not used
        const result = AggregateFormatting.groupTotalsFormat(formatterContext({
            item: {
                sum: {
                    SomeOther: 0,
                }
            },
            column: {
                field: "Amount"
            }
        }));

        expect(result).toBe("");
    });

    it("falls back to total with null value when no non-null total found and summaryType is not provided", () => {
        // note that column might not have summaryType if CustomSummaryMixin is not used
        const result = AggregateFormatting.groupTotalsFormat(formatterContext({
            item: {
                sum: {
                    Amount: void 0
                },
                avg: {
                    Amount: null
                }
            },
            column: {
                field: "Amount"
            }
        }));

        expect(result).toStrictEqual(<span class="aggregate agg-avg" title="Avg"></span>);
    });
});

describe("AggregateFormatting.formatAggregateValue", () => {
    it("formats a number by the display format of its column", () => {
        const column: any = { field: "Amount", sourceItem: { displayFormat: "#,##0.00" } };
        expect(AggregateFormatting.formatAggregateValue(column, 62335.47))
            .toBe("62,335.47");
    });

    it("escapes a text value, so it can be used in a markup context", () => {
        const column: any = { field: "Name" };
        expect(AggregateFormatting.formatAggregateValue(column, "<b>x</b>"))
            .toBe("&lt;b&gt;x&lt;/b&gt;");
    });

    it("uses the formatter of the column, with the value as a field of a non data row", () => {
        const column: any = {
            field: "Amount",
            format: (ctx: any) => "fmt:" + ctx.value + ":" + ctx.item.Amount + ":" + ctx.purpose
        };
        expect(AggregateFormatting.formatAggregateValue(column, 5))
            .toBe("fmt:5:5:group-totals");
    });

    it("falls back to the display format when the column formatter throws", () => {
        const column: any = {
            field: "Amount",
            sourceItem: { displayFormat: "#,##0.00" },
            format: () => { throw new Error("bad formatter"); }
        };
        expect(AggregateFormatting.formatAggregateValue(column, 62335.47)).toBe("62,335.47");
    });
});

describe("AggregateFormatting.createAggregateValueNode", () => {
    it("creates an element with the formatted value", () => {
        const column: any = { field: "Amount", sourceItem: { displayFormat: "#,##0.00" } };
        const node = AggregateFormatting.createAggregateValueNode(column, 62335.47);
        expect(node.textContent).toBe("62,335.47");
    });

    it("appends a node the column formatter returns", () => {
        const column: any = {
            field: "Amount",
            format: () => {
                const b = document.createElement("b");
                b.textContent = "bold";
                return b;
            }
        };
        const node = AggregateFormatting.createAggregateValueNode(column, 1);
        expect(node.querySelector("b")?.textContent).toBe("bold");
    });
});

describe("AggregateFormatting.initGridDefaults", () => {
    it("should set groupTotalsFormat when not already set", () => {
        // Reset gridDefaults
        (gridDefaults as any).groupTotalsFormat = undefined;

        AggregateFormatting.initGridDefaults();

        expect((gridDefaults as any).groupTotalsFormat).toBe(AggregateFormatting.groupTotalsFormat);
    });

    it("should not override existing groupTotalsFormat", () => {
        const existingFormatter = () => "existing";
        (gridDefaults as any).groupTotalsFormat = existingFormatter;

        AggregateFormatting.initGridDefaults();

        expect((gridDefaults as any).groupTotalsFormat).toBe(existingFormatter);
    });
});
