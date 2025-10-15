import { attachRef } from "./ref";
import { isShadowRoot } from "./shadow";
import type { ComponentChildren } from "./types";
import { isArrayLike, isElement, isNumber, isSignalLike, isString, type SignalLike } from "./util";

function appendChild(parent: Node, child: Node) {
    if (parent instanceof window.HTMLTemplateElement) {
        parent.content.appendChild(child);
    } else {
        parent.appendChild(child);
    }
}

let fragmentPlaceholderIdx = 0;
const fragmentPlaceholderPrefix = "__sleekdomfragmentplaceholder";

function wrapAsNode(value: any): Node {
    if (typeof value === "string") {
        return document.createTextNode(value);
    }
    if (typeof value === "number") {
        return document.createTextNode(value.toString());
    }
    if (value instanceof DocumentFragment) {
        ++fragmentPlaceholderIdx;
        value.prepend(document.createComment(fragmentPlaceholderPrefix + fragmentPlaceholderIdx));
        value.append(document.createComment(fragmentPlaceholderPrefix + fragmentPlaceholderIdx));
        return value;
    }
    if (value instanceof Node) {
        return value;
    }
    if (value == null || value === false) {
        return document.createComment("");
    }
    return value;
}

function appendChildrenWithSignal(parent: Node, signal: SignalLike<any>) {
    let prevValAsNode = wrapAsNode(signal.peek());
    let immediate = true;
    signal.subscribe((newValue: any) => {
        if (immediate) {
            return;
        }
        const newValAsNode = wrapAsNode(newValue);
        if (prevValAsNode instanceof Comment && prevValAsNode.data.startsWith("__sleekdomfragmentplaceholder")) {
            let n: Node;
            while (n = prevValAsNode.nextSibling) {
                n.parentNode?.removeChild(n);
                if (n instanceof Comment && n.data === prevValAsNode.data) {
                    break;
                }
            }
        }

        if (newValAsNode instanceof DocumentFragment && 
            newValAsNode.firstChild instanceof Comment && 
            newValAsNode.firstChild.data.startsWith(fragmentPlaceholderPrefix)) {
            const comment = newValAsNode?.firstChild;
            if (typeof (prevValAsNode as any).replaceWith === "function")
                (prevValAsNode as any).replaceWith(newValAsNode);
            else 
                (prevValAsNode.parentNode)?.replaceChild(newValAsNode, prevValAsNode);
            prevValAsNode = comment;
        }
        else {
            if (typeof (prevValAsNode as any).replaceWith === "function")
                (prevValAsNode as any).replaceWith(newValAsNode);
            else 
                (prevValAsNode.parentNode)?.replaceChild(newValAsNode, prevValAsNode);
            prevValAsNode = newValAsNode;
        }
    });
    immediate = false;
    
    if (prevValAsNode instanceof DocumentFragment && 
        prevValAsNode.firstChild instanceof Comment && 
        prevValAsNode.firstChild.data.startsWith(fragmentPlaceholderPrefix)) {
        const o = prevValAsNode;
        prevValAsNode = o.firstChild;
        appendChildren(parent, o);
    }
    else
        appendChildren(parent, prevValAsNode as any);
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
    } else if (isSignalLike(children)) {
        appendChildrenWithSignal(parent, children);
    }
}
