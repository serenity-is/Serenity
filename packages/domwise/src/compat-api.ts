import type { JSXElement, Ref } from "../types";
import { jsx } from "./jsx-factory";
import { setRef } from "./ref";
import { isString } from "./util";

/**
 * Creates a JSX element using the classic (non-automatic) JSX factory signature.
 * Children are passed as additional arguments after `attr` (rest params).
 * If `attr` is a string or array, it is treated as the first child and `attr` becomes `{}`.
 * If `attr.children` exists and no additional children were given, `attr.children` is used.
 * Prefer using the `jsx` function directly when using the automatic JSX runtime.
 * @param tag - The HTML/SVG tag name or component function/class.
 * @param attr - The attributes/props for the element, or the first child if it is a string/array.
 * @param children - Child elements passed as rest arguments.
 * @returns The created JSX element.
 */
export function createElement(tag: any, attr: any, ...children: any[]): JSXElement {
    if (isString(attr) || Array.isArray(attr)) {
        children.unshift(attr);
        attr = {};
    }

    attr = attr || {};

    if (attr.children != null && !children.length) {
        ({ children, ...attr } = attr);
    }

    return jsx(tag, { ...attr, children });
}

/**
 * Compatibility helper similar to React's `useImperativeHandle`.
 * Calls `setRef` with the result of `init()`. Prefer using `setRef` directly.
 * @param ref - A `RefObject` or ref callback.
 * @param init - A factory function returning the value to assign to the ref.
 */
export function useImperativeHandle<T>(ref: Ref<T>, init: () => T): void {
    setRef(ref, init());
}
