import { formatterContext as ctx } from "@serenity-is/sleekgrid";
import { NumberFormatter } from "./numberformatter";

describe("NumberFormatter", () => {
    it("shows empty string if value is null", () => {
        var formatter = new NumberFormatter();
        expect(formatter.format(ctx({ value: null }))).toBe("");
        expect(formatter.format(ctx({ value: NaN }))).toBe("");
    });

    it("shows formatted number if value type is number", () => {
        var formatter = new NumberFormatter();
        expect(formatter.format(ctx({ value: 123456.789 }))).toBe("123456.79");
    });

    it("parses shows formatted number if value type is string", () => {
        var formatter = new NumberFormatter();
        expect(formatter.format(ctx({ value: "123456.789" }))).toBe("123456.79");
    });

    it("shows given string value if value type is string and it is not a number", () => {
        var formatter = new NumberFormatter();
        expect(formatter.format(ctx({ value: "this is not a number" }))).toBe("this is not a number");
    });

    it("uses given numberformat", () => {
        var formatter = new NumberFormatter();
        formatter.displayFormat = "0.###"
        expect(formatter.format(ctx({ value: "123456.789" }))).toBe("123456.789");
    });
})
