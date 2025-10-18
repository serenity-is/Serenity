import type { ComponentClass, SignalLike } from "./types";

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

/**
 * This calls the callback whenever the signal value changes.
 * The callback is called immediately upon subscription, whether the signal library
 * calls it immediately or not.
 * @param signal Signal to observe
 * @param callback Callback to call when the signal value changes with new and old value.
 * It is called immediately with the current value as newValue and prevValue as undefined.
 * The third parameter 'initial' is true when the callback is called immediately upon subscription.
 * @returns A function to dispose the effect if the signal library supports unsubscription
 */
export function observeSignal<T>(signal: SignalLike<T>, 
    callback: ((value: T, prev: T, initial: boolean) => void)) {
    let prev = signal.peek();
    callback(prev, undefined, true);
    let immediate = true;
    const dispose = signal.subscribe((value: any) => {
        if (!immediate) {
            callback(value, prev, false);
            prev = value;
        }
    });
    immediate = false;
    return dispose;
}