import type { ComponentChildren, Ref, ShadowRootContainer } from "../types";

const jsxTypeKeySymbol = Symbol.for("Serenity.jsxTypeKey")
const shadowRootKey = "ShadowRoot";

/**
 * Creates a virtual node descriptor for a `ShadowRoot` that can be used
 * during JSX element creation. The returned object is recognized by the
 * JSX factory to create a shadow root on the parent element.
 * @param options - An object with `ShadowRootInit` properties plus optional `ref` and `children`.
 * @returns A virtual node descriptor recognized by the JSX factory.
 */
export function ShadowRootNode({
    children,
    ref,
    ...attr
}: ShadowRootInit & {
    ref?: Ref<ShadowRoot>
    children?: ComponentChildren
}) {
    return {
        [jsxTypeKeySymbol]: shadowRootKey,
        ref,
        attr,
        children,
    } as any;
}

/**
 * Type guard that checks whether a given value is a `ShadowRootContainer`
 * (i.e., a virtual shadow root node descriptor).
 * @param el - The value to check.
 * @returns `true` if the value is a shadow root container descriptor.
 */
export function isShadowRoot(el: any): el is ShadowRootContainer {
    return el != null && el[jsxTypeKeySymbol] === shadowRootKey
}
