import { attachRef } from "./ref";
import { isShadowRoot } from "./shadow";
import type { ComponentChildren } from "./types";
import { isArrayLike, isChildrenHook, isElement, isNumber, isString } from "./util";

function appendChild(parent: Node, child: Node) {
    if (parent instanceof window.HTMLTemplateElement) {
        parent.content.appendChild(child);
    } else {
        parent.appendChild(child);
    }
}

export function appendChildren(
    parent: Node,
    children: ComponentChildren,
) {
    if (isArrayLike(children)) {
        for (const child of [...(children as any[])]) {
            appendChildren(parent, child);
        }        
    } else if (isString(children) || isNumber(children)) {
        appendChild(parent, document.createTextNode(children as any));
    } else if (children === null) {
        appendChild(parent, document.createComment(""));
    } else if (isElement(children)) {
        appendChild(parent, children);
    } else if (isShadowRoot(children)) {
        const shadowRoot = (parent as HTMLElement).attachShadow(children.attr);
        appendChildren(shadowRoot, children.children);
        attachRef(children.ref, shadowRoot);
    } else if (isChildrenHook(children)) {
        appendChildren(parent, children.jsxDomChildrenHook.call(children, parent));
    }
}
