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
    return typeof val === "object" && val !== null;
}

export function isComponentClass(val: Function & { isComponent?: boolean }): val is ComponentClass {
    return !!(val && val.isComponent);
}

export function isArrayLike(val: any): val is ArrayLike<any> {
    return isObject(val) && typeof val.length === "number" && typeof val.nodeType !== "number";
}

export function isSignalLike(val: any): val is SignalLike<any> {
    return val != null && typeof val === "object" && typeof val.subscribe === "function" && typeof val.peek === "function" && 'value' in val;
}

// https://facebook.github.io/react/docs/jsx-in-depth.html#booleans-null-and-undefined-are-ignored
// Emulate JSX Expression logic to ignore certain type of children or className.
// though unexpected, true is also ignored as per react behavior.
export function isVisibleChild(val: any): boolean {
    return val != null && typeof val !== "boolean";
}