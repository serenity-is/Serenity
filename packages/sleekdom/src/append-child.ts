import { attachRef } from "./ref"
import { isShadowRoot } from "./shadow"
import type { ComponentChildren } from "./types"
import { isArrayLike, isChildrenHook, isElement, isNumber, isString } from "./util"

export function appendChild(
    child: ComponentChildren,
    node: Node
) {
    if (isArrayLike(child)) {
        for (const c of [...(child as any[])]) {
            appendChild(c, node)
        }        
    } else if (isString(child) || isNumber(child)) {
        appendChildToNode(document.createTextNode(child as any), node)
    } else if (child === null) {
        appendChildToNode(document.createComment(""), node)
    } else if (isElement(child)) {
        appendChildToNode(child, node)
    } else if (isShadowRoot(child)) {
        const shadowRoot = (node as HTMLElement).attachShadow(child.attr)
        appendChild(child.children, shadowRoot)
        attachRef(child.ref, shadowRoot)
    } else if (isChildrenHook(child)) {
        appendChild(child.jsxDomChildrenHook.call(child, node), node)
    }
}

function appendChildToNode(child: Node, node: Node) {
    if (node instanceof window.HTMLTemplateElement) {
        node.content.appendChild(child)
    } else {
        node.appendChild(child)
    }
}