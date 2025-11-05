import { type ComponentClass, type PropHook } from "../types";
import { initPropHookSymbol } from "./prop-hook";

export function isElement(val: any): val is Element {
    return !!(val && typeof val.nodeType === "number");
}

export function isString(val: any): val is string {
    return typeof val === "string";
}

export function isNumber(val: any): val is number {
    return typeof val === "number";
}

export function isObject(val: any) {
    return typeof val === "object" && val !== null;
}

export function isComponentClass(val: Function & { isComponent?: boolean }): val is ComponentClass {
    return !!(val && val.isComponent);
}

export function isArrayLike(val: any): val is ArrayLike<any> {
    return isObject(val) && typeof val.length === "number" && typeof val.nodeType !== "number";
}

export function isPropHook(value: any): value is PropHook {
    return typeof value === "function" && typeof value[initPropHookSymbol] === "function";
}

// https://facebook.github.io/react/docs/jsx-in-depth.html#booleans-null-and-undefined-are-ignored
// Emulate JSX Expression logic to ignore certain type of children or className.
// though unexpected, true is also ignored as per react behavior.
export function isVisibleChild(val: any): boolean {
    return val != null && typeof val !== "boolean";
}
