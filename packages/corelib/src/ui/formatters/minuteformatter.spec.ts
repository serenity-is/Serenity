import { MinuteFormatter } from "./minuteformatter";

describe("MinuteFormatter", () => {
    it("shows empty string if value is null", () => {
        var formatter = new MinuteFormatter();
        expect(formatter.format({ value: null, escape: (s) => s })).toBe("");
        expect(formatter.format({ value: NaN, escape: (s) => s })).toBe("");
    })

    it("shows correctly formatted minute", () => {
        var formatter = new MinuteFormatter();

        expect(formatter.format({ value: 0, escape: (s) => s })).toBe("00:00");
        expect(formatter.format({ value: 12, escape: (s) => s })).toBe("00:12");
        expect(formatter.format({ value: 72, escape: (s) => s })).toBe("01:12");
        expect(formatter.format({ value: 680, escape: (s) => s })).toBe("11:20");
        expect(formatter.format({ value: 1360, escape: (s) => s })).toBe("22:40");
    })
})
