import type { ComponentChildren, Ref, ShadowRootContainer } from "./types"

const jsxDomType = Symbol.for("jsx-dom:type")

const enum JsxDomType {
    ShadowRoot = "ShadowRoot",
}

export function ShadowRoot({
    children,
    ref,
    ...attr
}: ShadowRootInit & {
    ref?: Ref<ShadowRoot>
    children?: ComponentChildren
}) {
    return {
        [jsxDomType]: JsxDomType.ShadowRoot,
        ref,
        attr,
        children,
    }
}

export function isShadowRoot(el: any): el is ShadowRootContainer {
    return el != null && el[jsxDomType] === JsxDomType.ShadowRoot
}
