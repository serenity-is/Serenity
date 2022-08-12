import { coalesce, extend, isValue, today } from "@/q/system";

test('coalesce returns first value if not null', function() {
    expect(coalesce(1, 2)).toBe(1);
});

test('coalesce returns second value if null', function() {
    expect(coalesce(null, 2)).toBe(2);
    expect(coalesce(undefined, 2)).toBe(2);
});

test('coalesce returns second if both null', function() {
    expect(coalesce(null, null)).toBe(null);
    expect(coalesce(undefined, null)).toBe(null);
    expect(coalesce(undefined, undefined)).toBe(undefined);
    expect(coalesce(null, undefined)).toBe(undefined);
});

test('isValue returns false for null or undefined', function() {
    expect(isValue(null)).toBe(false);
    expect(isValue(undefined)).toBe(false);
});

test('isValue returns true for non null or undefined', function() {
    expect(isValue(true)).toBe(true);
    expect(isValue(false)).toBe(true);
    expect(isValue("")).toBe(true);
});

test('today returns a date without time part', function() {
    var d = today();
    expect(d.getHours()).toBe(0);
    expect(d.getMinutes()).toBe(0);
    expect(d.getSeconds()).toBe(0);
    expect(d.getMilliseconds()).toBe(0);
});