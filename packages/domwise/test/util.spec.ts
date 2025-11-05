import { initPropHookSymbol } from "../src/prop-hook";
import { isArrayLike, isComponentClass, isElement, isNumber, isObject, isPropHook, isString, isVisibleChild } from "../src/util";

describe("isArrayLike", () => {
    it("returns true for arrays", () => {
        expect(isArrayLike([])).toBe(true);
        expect(isArrayLike([1, 2, 3])).toBe(true);
    });

    it("returns false for non-array-like objects", () => {
        expect(isArrayLike({})).toBe(false);
        expect(isArrayLike("string")).toBe(false);
        expect(isArrayLike(123)).toBe(false);
        expect(isArrayLike(null)).toBe(false);
        expect(isArrayLike(undefined)).toBe(false);
    });

    it("returns false for elements (which have nodeType)", () => {
        const div = document.createElement("div");
        expect(isArrayLike(div)).toBe(false);
    });
});

describe("isElement", () => {
    it("returns true for DOM elements", () => {
        const div = document.createElement("div");
        expect(isElement(div)).toBe(true);
    });

    it("returns false for non-elements", () => {
        expect(isElement({})).toBe(false);
        expect(isElement("string")).toBe(false);
        expect(isElement(123)).toBe(false);
        expect(isElement(null)).toBe(false);
        expect(isElement(undefined)).toBe(false);
    });
});

describe("isString", () => {
    it("returns true for strings", () => {
        expect(isString("")).toBe(true);
        expect(isString("hello")).toBe(true);
        expect(isString(String("test"))).toBe(true);
    });

    it("returns false for non-strings", () => {
        expect(isString(123)).toBe(false);
        expect(isString({})).toBe(false);
        expect(isString([])).toBe(false);
        expect(isString(null)).toBe(false);
        expect(isString(undefined)).toBe(false);
    });
});

describe("isNumber", () => {
    it("returns true for numbers", () => {
        expect(isNumber(0)).toBe(true);
        expect(isNumber(123)).toBe(true);
        expect(isNumber(-456)).toBe(true);
        expect(isNumber(3.14)).toBe(true);
        expect(isNumber(Infinity)).toBe(true);
        expect(isNumber(-Infinity)).toBe(true);
    });

    it("returns true for NaN", () => {
        expect(isNumber(NaN)).toBe(true);
    });

    it("returns false for non-numbers", () => {
        expect(isNumber("123")).toBe(false);
        expect(isNumber({})).toBe(false);
        expect(isNumber([])).toBe(false);
        expect(isNumber(null)).toBe(false);
        expect(isNumber(undefined)).toBe(false);
    });
});

describe("isObject", () => {
    it("returns true for objects", () => {
        expect(isObject({})).toBe(true);
        expect(isObject({ a: 1 })).toBe(true);
        expect(isObject(new Date())).toBe(true);
        expect(isObject([])).toBe(true); // arrays are objects
    });

    it("returns false for null", () => {
        expect(isObject(null)).toBe(false);
    });

    it("returns false for primitives", () => {
        expect(isObject("string")).toBe(false);
        expect(isObject(123)).toBe(false);
        expect(isObject(true)).toBe(false);
        expect(isObject(undefined)).toBe(false);
    });
});

describe("isComponentClass", () => {
    it("returns true for component classes", () => {
        function Component() {}
        Component.isComponent = true;
        expect(isComponentClass(Component)).toBe(true);
    });

    it("returns false for regular functions", () => {
        function regularFunction() {}
        expect(isComponentClass(regularFunction)).toBe(false);
    });

    it("returns false for non-functions", () => {
        expect(isComponentClass({} as any)).toBe(false);
        expect(isComponentClass("string" as any)).toBe(false);
        expect(isComponentClass(null as any)).toBe(false);
    });
});

describe("isPropHook", () => {
    it("returns true for prop hooks", () => {
        const mockHook = () => {};
        mockHook[initPropHookSymbol] = () => {};
        expect(isPropHook(mockHook)).toBe(true);
    });

    it("returns false for regular functions", () => {
        expect(isPropHook(() => {})).toBe(false);
    });

    it("returns false for non-functions", () => {
        expect(isPropHook({})).toBe(false);
        expect(isPropHook("string")).toBe(false);
        expect(isPropHook(null)).toBe(false);
    });
});

describe("isVisibleChild", () => {
    it("returns true for visible values", () => {
        expect(isVisibleChild("string")).toBe(true);
        expect(isVisibleChild(0)).toBe(true);
        expect(isVisibleChild(123)).toBe(true);
        expect(isVisibleChild([])).toBe(true);
        expect(isVisibleChild({})).toBe(true);
    });

    it("returns false for null and undefined", () => {
        expect(isVisibleChild(null)).toBe(false);
        expect(isVisibleChild(undefined)).toBe(false);
    });

    it("returns false for boolean values", () => {
        expect(isVisibleChild(true)).toBe(false);
        expect(isVisibleChild(false)).toBe(false);
    });
});