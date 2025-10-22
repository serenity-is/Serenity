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
    let prevValueAsNode: Node;
    observeSignal(signal, (args) => {
        if (args.isInitial) {
            prevValueAsNode = wrapAsNode(args.newValue);
            if (prevValueAsNode instanceof DocumentFragment &&
                prevValueAsNode.firstChild instanceof Comment &&
                prevValueAsNode.firstChild.data.startsWith(fragmentPlaceholderPrefix)) {
                const o = prevValueAsNode;
                args.lifecycleNode = (prevValueAsNode = o.firstChild) ?? parent;
                appendChildren(parent, o);
            }
            else {
                appendChildren(parent, prevValueAsNode as any)
                args.lifecycleNode = prevValueAsNode ?? parent;
            }
            return;
        }

        if (!args.hasChanged) {
            return;
        }

        const newValueAsNode = wrapAsNode(args.newValue);
        if (prevValueAsNode instanceof Comment && prevValueAsNode.data.startsWith("__sleekdomfragmentplaceholder")) {
            let n: Node;
            while (n = prevValueAsNode.nextSibling) {
                n.parentNode?.removeChild(n);
                if (n instanceof Comment && n.data === prevValueAsNode.data) {
                    break;
                }
            }
        }

        if (newValueAsNode instanceof DocumentFragment &&
            newValueAsNode.firstChild instanceof Comment &&
            newValueAsNode.firstChild.data.startsWith(fragmentPlaceholderPrefix)) {
            const comment = newValueAsNode?.firstChild;
            if (typeof (prevValueAsNode as any).replaceWith === "function")
                (prevValueAsNode as any).replaceWith(newValueAsNode);
            else
                (prevValueAsNode.parentNode)?.replaceChild(newValueAsNode, prevValueAsNode);
            args.lifecycleNode = (prevValueAsNode = comment) ?? parent;
        }
        else {
            if (typeof (prevValueAsNode as any).replaceWith === "function")
                (prevValueAsNode as any).replaceWith(newValueAsNode);
            else
                (prevValueAsNode.parentNode)?.replaceChild(newValueAsNode, prevValueAsNode);
            args.lifecycleNode = (prevValueAsNode = newValueAsNode) ?? parent;
        }
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
