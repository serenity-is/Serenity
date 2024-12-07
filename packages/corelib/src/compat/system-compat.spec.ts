import { today } from "./system-compat";
import * as deprecations from "./system-compat"

describe("coalesce", () => {
    it('returns first value if not null', function () {
        expect((deprecations as any).coalesce(1, 2)).toBe(1);
    });

    it('returns second value if null', function () {
        expect((deprecations as any).coalesce(null, 2)).toBe(2);
        expect((deprecations as any).coalesce(undefined, 2)).toBe(2);
    });

    it('returns second if both null', function () {
        expect((deprecations as any).coalesce(null, null)).toBe(null);
        expect((deprecations as any).coalesce(undefined, null)).toBe(null);
        expect((deprecations as any).coalesce(undefined, undefined)).toBe(undefined);
        expect((deprecations as any).coalesce(null, undefined)).toBe(undefined);
    });
});

describe("isValue", () => {
    it('isValue returns false for null or undefined', function () {
        expect((deprecations as any).isValue(null)).toBe(false);
        expect((deprecations as any).isValue(undefined)).toBe(false);
    });

    it('isValue returns true for non null or undefined', function () {
        expect((deprecations as any).isValue(true)).toBe(true);
        expect((deprecations as any).isValue(false)).toBe(true);
        expect((deprecations as any).isValue("")).toBe(true);
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