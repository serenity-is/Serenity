import { formatterContext as ctx } from "@serenity-is/sleekgrid";
import { MinuteFormatter } from "./minuteformatter";

describe("MinuteFormatter", () => {
    it("shows empty string if value is null", () => {
        var formatter = new MinuteFormatter();
        expect(formatter.format(ctx({ value: null }))).toBe("");
        expect(formatter.format(ctx({ value: NaN }))).toBe("");
    })

    it("shows correctly formatted minute", () => {
        var formatter = new MinuteFormatter();

        expect(formatter.format(ctx({ value: 0 }))).toBe("00:00");
        expect(formatter.format(ctx({ value: 12 }))).toBe("00:12");
        expect(formatter.format(ctx({ value: 72 }))).toBe("01:12");
        expect(formatter.format(ctx({ value: 680 }))).toBe("11:20");
        expect(formatter.format(ctx({ value: 1360 }))).toBe("22:40");
    })
})
