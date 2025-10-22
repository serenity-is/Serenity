import { addDisposingListener } from "./disposing-listener";
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
    let prevAsNode: Node;
    const dispose = observeSignal(signal, (value, prev) => {
        if (prevAsNode === undefined) {
            prevAsNode = wrapAsNode(value);
            return;
        }

        if (value === prev) {
            return;
        }

        const valueAsNode = wrapAsNode(value);
        if (prevAsNode instanceof Comment && prevAsNode.data.startsWith("__sleekdomfragmentplaceholder")) {
            let n: Node;
            while (n = prevAsNode.nextSibling) {
                n.parentNode?.removeChild(n);
                if (n instanceof Comment && n.data === prevAsNode.data) {
                    break;
                }
            }
        }

        if (valueAsNode instanceof DocumentFragment &&
            valueAsNode.firstChild instanceof Comment &&
            valueAsNode.firstChild.data.startsWith(fragmentPlaceholderPrefix)) {
            const comment = valueAsNode?.firstChild;
            if (typeof (prevAsNode as any).replaceWith === "function")
                (prevAsNode as any).replaceWith(valueAsNode);
            else
                (prevAsNode.parentNode)?.replaceChild(valueAsNode, prevAsNode);
            prevAsNode = addDisposingListener(comment, this?.dispose);
        }
        else {
            if (typeof (prevAsNode as any).replaceWith === "function")
                (prevAsNode as any).replaceWith(valueAsNode);
            else
                (prevAsNode.parentNode)?.replaceChild(valueAsNode, prevAsNode);
            prevAsNode = addDisposingListener(valueAsNode, this?.dispose);
        }
    });

    if (prevAsNode instanceof DocumentFragment &&
        prevAsNode.firstChild instanceof Comment &&
        prevAsNode.firstChild.data.startsWith(fragmentPlaceholderPrefix)) {
        const o = prevAsNode;
        prevAsNode = addDisposingListener(o.firstChild, dispose);
        appendChildren(parent, o);
    }
    else
        appendChildren(parent, addDisposingListener(prevAsNode as any, dispose));
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
