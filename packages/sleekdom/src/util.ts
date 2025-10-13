import type { ComponentClass } from "./types";

export interface SignalLike<T> {
    value: T;
    peek(): T;
    subscribe(fn: (value: T) => void): () => void;
}

export const keys: <T>(obj: T) => Array<keyof T> = Object.keys as any;

export function forEach<V = any>(value: { [key: string]: V }, fn: (value: V, key: string) => void) {
    if (!value) return;
    for (const key of keys(value)) {
        fn(value[key], key as any);
    }
}

export function identity<T>(value: T) {
    return value;
}

export function isBoolean(val: any): val is boolean {
    return typeof val === "boolean";
}

export function isElement(val: any): val is Element {
    return val && typeof val.nodeType === "number";
}

export function isString(val: any): val is string {
    return typeof val === "string";
}

export function isNumber(val: any): val is number {
    return typeof val === "number";
}

export function isObject(val: any) {
    return typeof val === "object" ? val !== null : isFunction(val);
}

export function isFunction(val: any): val is Function {
    return typeof val === "function";
}

export function isComponentClass(val: Function & { isComponent?: boolean }): val is ComponentClass {
    return !!(val && val.isComponent);
}

export function isArrayLike(val: any): val is ArrayLike<any> {
    return isObject(val) && typeof val.length === "number" && typeof val.nodeType !== "number";
}

export function isAttributeHook(val: any): val is { jsxDomAttributeHook: (node: HTMLElement | SVGElement, attr: string) => any } {
    return isObject(val) && isFunction(val.jsxDomAttributeHook);
}

export function isChildrenHook(val: any): val is { jsxDomChildrenHook: (parent: HTMLElement | SVGElement) => any } {
    return isObject(val) && isFunction(val.jsxDomChildrenHook);
}

export function isSignalLike<T = any>(val: any): val is SignalLike<T> {
    return isObject(val) && isFunction(val.subscribe) && isFunction(val.peek) && 'value' in val;
}

// https://facebook.github.io/react/docs/jsx-in-depth.html#booleans-null-and-undefined-are-ignored
// Emulate JSX Expression logic to ignore certain type of children or className.
export function isVisibleChild(value: any): boolean {
    return !isBoolean(value) && value != null;
}