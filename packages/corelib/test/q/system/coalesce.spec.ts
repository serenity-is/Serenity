import { coalesce } from "@/q/system";

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