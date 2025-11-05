import { className } from "../src/class-name"

describe("className", () => {
    it("returns empty string for falsy values", () => {
        expect(className(null)).toBe("");
        expect(className(undefined)).toBe("");
        expect(className(false)).toBe("");
        expect(className(true)).toBe("");
    });

    it("returns string as is for string values", () => {
        expect(className("foo")).toBe("foo");
        expect(className("foo bar")).toBe("foo bar");
    });

    it("returns string representation for numbers including zero", () => {
        expect(className(0)).toBe("0");
        expect(className(42)).toBe("42");
        expect(className(3.14)).toBe("3.14");
    });

    it("handles arrays by joining class names", () => {
        expect(className(["foo", "bar"])).toBe("foo bar");
        expect(className(["foo", null, "bar", false])).toBe("foo bar");
        expect(className([["nested"], "foo"])).toBe("nested foo");
    });

    it("handles objects by joining truthy keys", () => {
        expect(className({ foo: true, bar: false, baz: true })).toBe("foo baz");
        expect(className({})).toBe("");
    });

    it("handles iterables by converting to array", () => {
        expect(className(new Set(["foo", "bar"]))).toBe("foo bar");
        expect(className(new Map([["a", true], ["b", false]]).keys())).toBe("a b");
    });

    it("handles nested structures", () => {
        expect(className([{ foo: true }, "bar", ["baz"]])).toBe("foo bar baz");
    });
});