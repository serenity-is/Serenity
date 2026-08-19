import type { ComponentChildren, Ref, ShadowRootContainer } from "../types";

const jsxTypeKeySymbol = Symbol.for("Serenity.jsxTypeKey")
const shadowRootKey = "ShadowRoot";

/**
 * Creates a virtual `ShadowRoot` descriptor recognized by the JSX factory.
 *
 * When a `ShadowRootContainer` produced by this function appears among a
 * parent element's children (e.g. `<div><ShadowRootNode mode="open">…</ShadowRootNode></div>`),
 * the factory calls `parent.attachShadow(init)` and appends the `children`
 * into the resulting `ShadowRoot`. An optional `ref` is forwarded to the
 * created `ShadowRoot`.
 *
 * @param options - Shadow root init options (`mode`, `delegatesFocus`, etc.) plus optional `ref` and `children`.
 * @param options.mode - Shadow root mode (`"open"` or `"closed"`).
 * @param options.delegatesFocus - Whether focus delegation is enabled.
 * @param options.slotAssignment - How slots are assigned (`"manual"` or `"named"`).
 * @param options.clonable - Whether the shadow root is clonable.
 * @param options.serializable - Whether the shadow root is serializable.
 * @param options.customElementRegistry - Custom element registry for the shadow tree.
 * @param options.ref - Optional ref to receive the created `ShadowRoot`.
 * @param options.children - Children to render inside the shadow root.
 * @returns A virtual node descriptor that the JSX factory consumes to create the shadow root.
 * @example
 * ```tsx
 * <div><ShadowRootNode mode="open"><span>inside shadow</span></ShadowRootNode></div>
 * ```
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
 * Type guard that checks whether a value is a `ShadowRootContainer` descriptor
 * produced by {@link ShadowRootNode} (identified by the internal `jsxTypeKey` symbol).
 * @param el - Value to test.
 * @returns `true` if `el` is a `ShadowRootContainer`.
 */
export function isShadowRoot(el: any): el is ShadowRootContainer {
    return el != null && el[jsxTypeKeySymbol] === shadowRootKey
}
