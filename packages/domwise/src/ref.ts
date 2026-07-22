import type { Ref, RefObject } from "../types";

/**
 * Creates a new `RefObject` with `current` initially set to `null`.
 * The returned object is sealed to prevent extension.
 * @typeParam T - The type of the referenced value.
 * @returns A new sealed `RefObject<T>`.
 */
export function createRef<T = any>(): RefObject<T> {
    return Object.seal({ current: null });
}

/**
 * Type guard that checks whether a value is a `RefObject` (has a `current` property).
 * @param maybeRef - The value to check.
 * @returns `true` if the value is a `RefObject`.
 */
export function isRefObject<T = Node>(maybeRef: any): maybeRef is RefObject<T> {
    return maybeRef != null && typeof maybeRef === "object" && "current" in maybeRef;
}

/**
 * Sets the `current` property of a `RefObject`, or calls a ref callback with the given value.
 * @typeParam T - The type of the referenced node.
 * @param ref - A `RefObject` or a ref callback, or `undefined`.
 * @param current - The value to assign to the ref.
 */
export function setRef<T = Node>(ref: Ref<T> | undefined, current: T): void {
    if (isRefObject<T>(ref)) {
        ref.current = current;
    } else if (typeof ref === "function") {
        ref(current);
    }
}