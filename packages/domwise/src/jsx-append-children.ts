import type { ComponentChildren, SignalLike } from "../types";
import { invokeDisposingListeners } from "./disposing-listener";
import { setRef } from "./ref";
import { isShadowRoot } from "./shadow";
import { isSignalLike, observeSignal, retainSignalValuesSymbol } from "./signal-util";
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

export function isChildCollection(value: any): boolean {
    if (value == null || typeof value === "string" || typeof value === "number" || typeof value === "function")
        return false;
    if (value instanceof Node)
        return false;
    return Array.isArray(value) || isArrayLike(value) || typeof value[Symbol.iterator] === "function";
}

function replaceNode(oldNode: Node, newNode: Node) {
    if (typeof (oldNode as any).replaceWith === "function")
        (oldNode as any).replaceWith(newNode);
    else
        (oldNode.parentNode)?.replaceChild(newNode, oldNode);
}

const fragmentEndNodes = new WeakMap<Comment, Comment>();

function wrapFragment(fragment: DocumentFragment): Node {
    ++fragmentPlaceholderIdx;
    const start = document.createComment(placeholderPrefix + fragmentPlaceholderIdx);
    const end = document.createComment(placeholderPrefix + fragmentPlaceholderIdx);
    fragment.prepend(start);
    fragment.append(end);
    // remember the end node so replacement matches by identity, not by the
    // placeholder text (which can collide across copies of the library)
    fragmentEndNodes.set(start, end);
    return fragment;
}

function wrapAsNode(value: any): Node {
    if (value instanceof DocumentFragment) {
        return wrapFragment(value);
    }
    if (value instanceof Node) {
        return value;
    }
    if (isChildCollection(value)) {
        // arrays / iterables / collections are expanded into a range so they
        // can be replaced as a whole when the signal value changes
        const fragment = document.createDocumentFragment();
        appendChildren(fragment, value);
        return wrapFragment(fragment);
    }
    if (!isVisibleChild(value)) {
        return document.createComment("");
    }
    return document.createTextNode(value);
}

function disposeSubtree(node: Node | null | undefined) {
    if (node && node instanceof EventTarget)
        invokeDisposingListeners(node, { descendants: true });
}

function appendChildrenWithSignal(parent: Node, signal: SignalLike<any>) {
    let prevNode: Node;
    // Values produced by an owning signal (e.g. Show) are reused or disposed by
    // their producer, so the renderer must not dispose the outgoing value.
    const disposeOutgoing = !(signal as any)[retainSignalValuesSymbol];
    // A DocumentFragment is emptied as soon as it is inserted, so it can never
    // receive a `disposing` event. Anchor the subscription to a placeholder
    // comment that travels with the content into the real DOM instead, so the
    // subscription is still torn down when the host is disposed.
    const anchor = parent instanceof DocumentFragment ? document.createComment("") : null;
    if (anchor)
        parent.appendChild(anchor);
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
            // collect the whole range first: disposal may mutate the DOM, so the
            // live sibling walk must finish before anything is removed
            const endNode = fragmentEndNodes.get(prevNode);
            const range: Node[] = [];
            let n: Node | null = prevNode.nextSibling;
            while (n) {
                range.push(n);
                if (endNode ? n === endNode : (n instanceof Comment && n.data === prevNode.data))
                    break;
                n = n.nextSibling;
            }
            for (const node of range) {
                node.parentNode?.removeChild(node);
                if (disposeOutgoing)
                    disposeSubtree(node);
            }
            if (disposeOutgoing)
                disposeSubtree(prevNode);
        }
        else if (disposeOutgoing && prevNode !== parent) {
            disposeSubtree(prevNode);
        }
        const prevNodeNew = isFragmentWithPlaceholder(newNode) ? newNode.firstChild! : newNode;
        replaceNode(prevNode, newNode);
        prevNode = prevNodeNew ?? parent;
    }, {
        // scope the subscription to the parent, not the rendered content: the
        // content may be disposed by an owner (e.g. Show disposing a factory
        // branch) while this subscription must keep rendering subsequent values
        lifecycleNode: anchor ?? parent
    });

}

export function appendChildren(
    parent: Node,
    children: ComponentChildren,
): void {
    if (!isVisibleChild(children)) return;
    if (isString(children) || isNumber(children)) {
        appendChild(parent, document.createTextNode(children as any));
    } else if (isElement(children)) {
        appendChild(parent, children);
    } else if (isShadowRoot(children)) {
        if (parent instanceof Element) {
            const shadowRoot = (parent as HTMLElement).attachShadow(children.attr);
            appendChildren(shadowRoot, children.children);
            setRef(children.ref, shadowRoot);
        } else {
            // no host element to attach to (e.g. a ShadowRootNode inside a
            // signal collection rendered through a DocumentFragment): render
            // the children directly instead of failing with a TypeError
            appendChildren(parent, children.children);
        }
    } else if (isSignalLike(children)) {
        appendChildrenWithSignal(parent, children);
    } else if (isChildCollection(children)) {
        // Array.from handles both iterables (arrays, Set, ...) and array-likes
        // (NodeList, HTMLCollection, { length }) without throwing
        for (const child of Array.from(children as any))
            appendChildren(parent, child as ComponentChildren);
    }
}
