import type { JSXElement, Ref } from "../types";
import { jsx } from "./jsx-factory";
import { setRef } from "./ref";
import { isString } from "./util";

/**
 * Creates a JSX element using the classic (non-automatic) JSX factory signature.
 *
 * Children are passed as variadic rest arguments after `attr`. For compatibility,
 * if `attr` itself is a string or array it is treated as the first child and
 * `attr` is replaced with `{}`. If `attr.children` is set and no explicit
 * `children` were supplied, the `children` property is extracted from `attr`.
 *
 * Prefer {@link jsx} when using the automatic JSX runtime (`"jsx": "automatic"`).
 *
 * @param tag - HTML/SVG tag name or a component function/class.
 * @param attr - Attributes/props for the element, or the first child when a
 * string or array. May be `null`/`undefined` when no attributes are needed.
 * @param children - Child elements passed as rest arguments.
 * @returns The created JSX DOM node.
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
 *
 * Evaluates `init()` and forwards the result to `ref` via {@link setRef}.
 * Prefer calling {@link setRef} directly in new code.
 *
 * @typeParam T - Type of the value exposed through the ref.
 * @param ref - Target `RefObject` or ref callback to update.
 * @param init - Factory that produces the value to assign to the ref.
 */
export function useImperativeHandle<T>(ref: Ref<T>, init: () => T): void {
    setRef(ref, init());
}
