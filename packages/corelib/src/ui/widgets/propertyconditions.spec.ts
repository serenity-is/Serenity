import { describe, expect, it } from "vitest";
import { hasPropertyValue, isPropertyConditionMet } from "./propertyconditions";

describe("hasPropertyValue", () => {
    it("is false for null, an empty string, an empty list and false", () => {
        for (const value of [null, undefined, "", [], false])
            expect(hasPropertyValue(value)).toBe(false);
    });

    it("is true for anything else, including zero", () => {
        for (const value of ["a", 0, 1, true, ["a"], {}])
            expect(hasPropertyValue(value)).toBe(true);
    });
});

describe("isPropertyConditionMet", () => {
    it("is met by any of the values, compared as text, like enum numbers and booleans", () => {
        const condition = { field: "Action", values: [2, 3] };
        expect(isPropertyConditionMet(condition, 2)).toBe(true);
        expect(isPropertyConditionMet(condition, "3")).toBe(true);
        expect(isPropertyConditionMet(condition, 1)).toBe(false);
        expect(isPropertyConditionMet(condition, null)).toBe(false);
        expect(isPropertyConditionMet({ field: "Flag", values: [true] }, true)).toBe(true);
        expect(isPropertyConditionMet({ field: "Flag", values: [true] }, false)).toBe(false);
    });

    it("is met by a list that has any of the values as an item", () => {
        const condition = { field: "Tags", values: ["a"] };
        expect(isPropertyConditionMet(condition, ["b", "a"])).toBe(true);
        expect(isPropertyConditionMet(condition, ["b"])).toBe(false);
        expect(isPropertyConditionMet(condition, [])).toBe(false);
    });

    it("is met by having a value when there are no values, e.g. a checked checkbox", () => {
        expect(isPropertyConditionMet({ field: "Column" }, "Day")).toBe(true);
        expect(isPropertyConditionMet({ field: "Column" }, "")).toBe(false);
        expect(isPropertyConditionMet({ field: "Show", values: [] }, true)).toBe(true);
        expect(isPropertyConditionMet({ field: "Show" }, false)).toBe(false);
    });

    it("is the opposite with isNot", () => {
        expect(isPropertyConditionMet({ field: "Column", isNot: true }, "")).toBe(true);
        expect(isPropertyConditionMet({ field: "Column", isNot: true }, "Day")).toBe(false);
        expect(isPropertyConditionMet({ field: "Action", values: [2], isNot: true }, 1)).toBe(true);
        expect(isPropertyConditionMet({ field: "Action", values: [2], isNot: true }, 2)).toBe(false);
    });
});
