import { formatterContext as ctx } from "@serenity-is/sleekgrid";
import { describe, expect, it } from "vitest";
import { FormatterTypeRegistry } from "../../types/formattertyperegistry";
import { BigIntFormatter } from "./bigintformatter";

describe("BigIntFormatter", () => {
    it("shows empty string if value is null or NaN", () => {
        const formatter = new BigIntFormatter();
        expect(formatter.format(ctx({ value: null }))).toBe("");
        expect(formatter.format(ctx({ value: NaN }))).toBe("");
    });

    it("formats safe numbers as plain integers by default", () => {
        const formatter = new BigIntFormatter();
        expect(formatter.format(ctx({ value: 123456 }))).toBe("123456");
    });

    it("formats safe numbers with grouping and custom formats", () => {
        const formatter = new BigIntFormatter({ displayFormat: "n0" });
        expect(formatter.format(ctx({ value: 123456 }))).toBe("123,456");
        expect(BigIntFormatter.formatValue(123456, "#,##0")).toBe("123,456");
    });

    it("formats large string values without losing precision", () => {
        expect(BigIntFormatter.formatValue("9223372036854775807")).toBe("9223372036854775807");
        expect(BigIntFormatter.formatValue("-9223372036854775808")).toBe("-9223372036854775808");
    });

    it("groups large values outside the safe integer range", () => {
        const formatter = new BigIntFormatter({ displayFormat: "n0" });
        expect(formatter.format(ctx({ value: "9223372036854775807" }))).toBe("9,223,372,036,854,775,807");
        expect(formatter.format(ctx({ value: "-9223372036854775808" }))).toBe("-9,223,372,036,854,775,808");
    });

    it("formats bigint values", () => {
        expect(BigIntFormatter.formatValue(9223372036854775807n)).toBe("9223372036854775807");
        expect(BigIntFormatter.formatValue(9223372036854775807n, "n0")).toBe("9,223,372,036,854,775,807");
    });

    it("shows the raw value for non-numeric strings", () => {
        expect(BigIntFormatter.formatValue("this is not a number")).toBe("this is not a number");
    });

    it("falls back to the number formatter for non-integer numbers", () => {
        expect(BigIntFormatter.formatValue(123456.789)).toBe("123456.79");
    });

    it("supports hexadecimal and padded formats", () => {
        expect(BigIntFormatter.formatValue("9223372036854775807", "x")).toBe("7fffffffffffffff");
        expect(BigIntFormatter.formatValue("9223372036854775807", "X")).toBe("7FFFFFFFFFFFFFFF");
        expect(BigIntFormatter.formatValue(42, "d5")).toBe("00042");
    });

    it("is resolvable by the formatter type registry as BigInt", () => {
        expect(FormatterTypeRegistry.get("BigInt")).toBe(BigIntFormatter);
    });
});
