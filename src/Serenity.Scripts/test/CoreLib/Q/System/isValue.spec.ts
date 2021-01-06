import { isValue } from "@Q/System";

test('isValue returns false for null or undefined', function() {
    expect(isValue(null)).toBe(false);
    expect(isValue(undefined)).toBe(false);
});

test('isValue returns true for non null or undefined', function() {
    expect(isValue(true)).toBe(true);
    expect(isValue(false)).toBe(true);
    expect(isValue("")).toBe(true);
});