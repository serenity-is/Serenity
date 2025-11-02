import type { ComponentChildren, Ref, ShadowRootContainer } from "./types"

const sleekDomTypeSymbol = Symbol.for("wisdom:type")

const enum WisDomTypeKeys {
    ShadowRoot = "ShadowRoot",
}

export function ShadowRootNode({
    children,
    ref,
    ...attr
}: ShadowRootInit & {
    ref?: Ref<ShadowRoot>
    children?: ComponentChildren
}) {
    return {
        [sleekDomTypeSymbol]: WisDomTypeKeys.ShadowRoot,
        ref,
        attr,
        children,
    } as any;
}

export function isShadowRoot(el: any): el is ShadowRootContainer {
    return el != null && el[sleekDomTypeSymbol] === WisDomTypeKeys.ShadowRoot
}
