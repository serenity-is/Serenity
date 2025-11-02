import { attachRef } from "./ref";
import { isShadowRoot } from "./shadow";
import { isSignalLike, observeSignal } from "./signal-util";
import type { ComponentChildren, SignalLike } from "./types";
import { isArrayLike, isElement, isNumber, isString } from "./util";

function appendChild(parent: Node, child: Node) {
    if (parent instanceof window.HTMLTemplateElement) {
        parent.content.appendChild(child);
    } else {
        parent.appendChild(child);
    }
}

let fragmentPlaceholderIdx = 0;
const placeholderPrefix = "__sleekdomfrag_";

function isPlaceholder(node: Node): node is Comment {
    return node instanceof Comment && node.data.startsWith(placeholderPrefix);
}

function isFragmentWithPlaceholder(node: Node): node is DocumentFragment {
    return node instanceof DocumentFragment && isPlaceholder(node.firstChild);
}

function replaceNode(oldNode: Node, newNode: Node) {
    if (typeof (oldNode as any).replaceWith === "function")
        (oldNode as any).replaceWith(newNode);
    else
        (oldNode.parentNode)?.replaceChild(newNode, oldNode);
}

function wrapAsNode(value: any): Node {
    if (typeof value === "string") {
        return document.createTextNode(value);
    }
    if (typeof value === "number") {
        return document.createTextNode(value.toString());
    }
    if (value instanceof DocumentFragment) {
        ++fragmentPlaceholderIdx;
        value.prepend(document.createComment(placeholderPrefix + fragmentPlaceholderIdx));
        value.append(document.createComment(placeholderPrefix + fragmentPlaceholderIdx));
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
    let prevNode: Node;
    observeSignal(signal, (args) => {
        if (args.isInitial) {
            prevNode = wrapAsNode(args.newValue);
            let prevNodeNew = isFragmentWithPlaceholder(prevNode) ? prevNode.firstChild : prevNode;
            appendChildren(parent, prevNode as any);
            args.lifecycleNode = (prevNode = prevNodeNew) ?? parent;
            return;
        }

        if (!args.hasChanged) {
            return;
        }

        const newNode = wrapAsNode(args.newValue);
        if (isPlaceholder(prevNode)) {
            let n: Node;
            while (n = prevNode.nextSibling) {
                n.parentNode?.removeChild(n);
                if (n instanceof Comment && n.data === prevNode.data) {
                    break;
                }
            }
        }
        let prevNodeNew = isFragmentWithPlaceholder(newNode) ? newNode.firstChild : newNode;
        replaceNode(prevNode, newNode);
        args.lifecycleNode = (prevNode = prevNodeNew) ?? parent;
    });

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
