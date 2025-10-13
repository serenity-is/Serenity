import type { ComponentChildren, Ref, ShadowRootContainer } from "./types"

const sleekDomTypeSymbol = Symbol.for("sleekdom:type")

const enum SleekDomTypeKeys {
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
        [sleekDomTypeSymbol]: SleekDomTypeKeys.ShadowRoot,
        ref,
        attr,
        children,
    } as any;
}

export function isShadowRoot(el: any): el is ShadowRootContainer {
    return el != null && el[sleekDomTypeSymbol] === SleekDomTypeKeys.ShadowRoot
}
