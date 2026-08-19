import type { Ref, RefObject } from "../types";

/**
 * Creates a new sealed `RefObject` whose `current` is initially `null`.
 *
 * The returned object is `Object.seal`ed so that no new properties can be
 * added, matching the `React.createRef` contract.
 *
 * @typeParam T - Type of the value held by the ref.
 * @returns A sealed `RefObject<T>` with `current` set to `null`.
 * @example
 * ```tsx
 * const inputRef = createRef<HTMLInputElement>();
 * return <input ref={inputRef} />;
 * // later: inputRef.current?.focus();
 * ```
 */
export function createRef<T = any>(): RefObject<T> {
    return Object.seal({ current: null });
}

/**
 * Type guard that checks whether a value is a `RefObject` — any non-null object with a `current` property.
 * @param maybeRef - Value to test.
 * @returns `true` if `maybeRef` is a `RefObject<T>`.
 */
export function isRefObject<T = Node>(maybeRef: any): maybeRef is RefObject<T> {
    return maybeRef != null && typeof maybeRef === "object" && "current" in maybeRef;
}

/**
 * Assigns a value to a ref, handling both object and callback forms.
 *
 * - If `ref` is a `RefObject`, its `current` property is set to `current`.
 * - If `ref` is a function, it is invoked with `current`.
 * - If `ref` is `null`/`undefined` or neither form, no action is taken.
 *
 * @typeParam T - Type of the node/value being assigned.
 * @param ref - Target `RefObject`, callback, or `null`/`undefined`.
 * @param current - Value to assign to the ref.
 */
export function setRef<T = Node>(ref: Ref<T> | undefined, current: T): void {
    if (isRefObject<T>(ref)) {
        ref.current = current;
    } else if (typeof ref === "function") {
        ref(current);
    }
}