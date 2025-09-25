import { formatterContext as ctx } from "@serenity-is/sleekgrid";
import { DateFormatter } from "./dateformatter";

describe("DateFormatter", () => {
    it("shows empty string if value is null", () => {
        var formatter = new DateFormatter();
        expect(formatter.format(ctx({ value: null }))).toBe("");
    });

    it("shows formatted date if value type is Date", () => {
        var formatter = new DateFormatter();
        var date = new Date(2023, 0, 1); // 01-01-2023
        expect(formatter.format(ctx({ value: date }))).toBe("01/01/2023");
    });

    it("shows formatted date if value type string and it is a date", () => {
        var formatter = new DateFormatter();
        expect(formatter.format(ctx({ value: "2023-01-01T00:00:00.000" }))).toBe("01/01/2023");
    });

    it("shows given value if value type is string and it is not a date", () => {
        var formatter = new DateFormatter();
        expect(formatter.format(ctx({ value: "this is not a date" }))).toBe("this is not a date");
    });

    it("shows given value if value type is string and it is empty", () => {
        var formatter = new DateFormatter();
        expect(formatter.format(ctx({ value: "" }))).toBe("");
    });

    it("uses given display format", () => {
        var formatter = new DateFormatter();
        formatter.displayFormat = "dd-MM-yyyy";
        expect(formatter.format(ctx({ value: "2023-01-01T00:00:00.000" }))).toBe("01-01-2023");
    });
})
