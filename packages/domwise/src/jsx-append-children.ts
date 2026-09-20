import type { ComponentChildren, SignalLike } from "../types";
import { setRef } from "./ref";
import { isShadowRoot } from "./shadow";
import { isSignalLike, observeSignal } from "./signal-util";
import { isArrayLike, isElement, isNumber, isString, isVisibleChild } from "./util";

function appendChild(parent: Node, child: Node) {
    if (parent instanceof window.HTMLTemplateElement) {
        parent.content.appendChild(child);
    } else {
        parent.appendChild(child);
    }
}

let fragmentPlaceholderIdx = 0;
const placeholderPrefix = "__domwisefrag_";

function isPlaceholder(node: Node | null): node is Comment {
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
    if (value instanceof DocumentFragment) {
        ++fragmentPlaceholderIdx;
        value.prepend(document.createComment(placeholderPrefix + fragmentPlaceholderIdx));
        value.append(document.createComment(placeholderPrefix + fragmentPlaceholderIdx));
        return value;
    }
    if (value instanceof Node) {
        return value;
    }
    if (!isVisibleChild(value)) {
        return document.createComment("");
    }
    return document.createTextNode(value);
}

function appendChildrenWithSignal(parent: Node, signal: SignalLike<any>) {
    let prevNode: Node;
    observeSignal(signal, (args) => {
        if (args.isInitial) {
            prevNode = wrapAsNode(args.newValue);
            const prevNodeNew = isFragmentWithPlaceholder(prevNode) ? prevNode.firstChild! : prevNode;
            appendChildren(parent, prevNode as any);
            prevNode = prevNodeNew ?? parent;
            return;
        }

        if (!args.hasChanged) {
            return;
        }

        const newNode = wrapAsNode(args.newValue);
        if (isPlaceholder(prevNode)) {
            let n: Node | null;
            while (n = prevNode.nextSibling) {
                n.parentNode?.removeChild(n);
                if (n instanceof Comment && n.data === prevNode.data) {
                    break;
                }
            }
        }
        const prevNodeNew = isFragmentWithPlaceholder(newNode) ? newNode.firstChild! : newNode;
        replaceNode(prevNode, newNode);
        prevNode = prevNodeNew ?? parent;
    }, {
        // scope the subscription to the parent, not the rendered content: the
        // content may be disposed by an owner (e.g. Show disposing a factory
        // branch) while this subscription must keep rendering subsequent values
        lifecycleNode: parent
    });

}

export function appendChildren(
    parent: Node,
    children: ComponentChildren,
): void {
    if (!isVisibleChild(children)) return;
    if (isArrayLike(children)) {
        for (const child of [...(children as any[])]) {
            appendChildren(parent, child);
        }
    } else if (isString(children) || isNumber(children)) {
        appendChild(parent, document.createTextNode(children as any));
    } else if (isElement(children)) {
        appendChild(parent, children);
    } else if (isShadowRoot(children)) {
        const shadowRoot = (parent as HTMLElement).attachShadow(children.attr);
        appendChildren(shadowRoot, children.children);
        setRef(children.ref, shadowRoot);
    } else if (isSignalLike(children)) {
        appendChildrenWithSignal(parent, children);
    }
}
