import type { ComponentChildren, Ref, ShadowRootContainer } from "../types";

const jsxTypeKeySymbol = Symbol.for("Serenity.jsxTypeKey")
const shadowRootKey = "ShadowRoot";

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

export function isShadowRoot(el: any): el is ShadowRootContainer {
    return el != null && el[jsxTypeKeySymbol] === shadowRootKey
}
