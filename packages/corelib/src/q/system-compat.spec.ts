import { coalesce, isValue, today } from "./system-compat";

describe("coalesce", () => {
    it('returns first value if not null', function () {
        expect(coalesce(1, 2)).toBe(1);
    });

    it('returns second value if null', function () {
        expect(coalesce(null, 2)).toBe(2);
        expect(coalesce(undefined, 2)).toBe(2);
    });

    it('returns second if both null', function () {
        expect(coalesce(null, null)).toBe(null);
        expect(coalesce(undefined, null)).toBe(null);
        expect(coalesce(undefined, undefined)).toBe(undefined);
        expect(coalesce(null, undefined)).toBe(undefined);
    });
});

describe("isValue", () => {
    it('isValue returns false for null or undefined', function () {
        expect(isValue(null)).toBe(false);
        expect(isValue(undefined)).toBe(false);
    });

    it('isValue returns true for non null or undefined', function () {
        expect(isValue(true)).toBe(true);
        expect(isValue(false)).toBe(true);
        expect(isValue("")).toBe(true);
    });
});

describe("today", () => {
    it('returns a date without time part', function () {
        var d = today();
        expect(d.getHours()).toBe(0);
        expect(d.getMinutes()).toBe(0);
        expect(d.getSeconds()).toBe(0);
        expect(d.getMilliseconds()).toBe(0);
    });
});