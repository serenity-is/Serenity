import type { ComponentChildren, JSXElement, SignalOrValue } from "../types";
import { addDisposingListener, invokeDisposingListeners, removeDisposingListener } from "./disposing-listener";
import { isChildCollection } from "./jsx-append-children";
import { derivedSignal, isSignalLike, observeSignal } from "./signal-util";

function disposeContent(content: any): void {
    if (content == null)
        return;

    if (content instanceof DocumentFragment) {
        for (const child of Array.from(content.childNodes))
            invokeDisposingListeners(child as unknown as EventTarget, { descendants: true });
    }
    else if (content instanceof EventTarget) {
        invokeDisposingListeners(content, { descendants: true });
    }
    else if (isChildCollection(content)) {
        for (const child of Array.from(content as any))
            disposeContent(child);
    }
}

/**
 * Finds a live `EventTarget` to anchor the branch lifecycle to.
 *
 * A `DocumentFragment` (and arrays/collections) has no stable presence in the
 * DOM once its children are inserted, so we walk down to the first live node.
 * This ensures disposing listeners are registered on a node that will actually
 * receive the `disposing` event.
 */
function findAnchor(content: any): EventTarget | null {
    if (content instanceof DocumentFragment) {
        for (const child of Array.from(content.childNodes)) {
            const anchor = findAnchor(child);
            if (anchor)
                return anchor;
        }
        return null;
    }
    if (content instanceof EventTarget)
        return content;
    if (isChildCollection(content)) {
        for (const child of Array.from(content as any)) {
            const anchor = findAnchor(child);
            if (anchor)
                return anchor;
        }
    }
    return null;
}

/**
 * Returns the object whose `disposing` listeners should be invoked for a
 * branch. A `DocumentFragment` is emptied when inserted, so we snapshot its
 * current children (taken before insertion) and dispose those instead.
 */
function contentToDispose(content: any): object | null {
    if (content instanceof DocumentFragment)
        return Array.from(content.childNodes);
    if (content != null && typeof content === "object")
        return content;
    return null;
}

/**
 * Conditional rendering helper similar to Solid's `<Show>`.
 *
 * Renders `children` when `when` is truthy, otherwise renders `fallback`.
 * Either slot may be a plain `ComponentChildren` value or a function that
 * receives the resolved `when` value. When `when` is a signal, the output is
 * a derived signal node so the DOM updates reactively; its lifecycle is
 * bound to the rendered node so the subscription is disposed with it.
 *
 * When a slot is a function, its result is considered owned by `Show`: the
 * previous result is disposed (via its `disposing` listeners) when the branch
 * changes, since a new result is produced on every evaluation and the old one
 * can never be reused. Plain (non-function) children are caller-owned and are
 * never disposed on switch, as the same instance is reused each time. When the
 * currently shown element is disposed, any branch held by `Show` (including a
 * hidden plain branch) is disposed as well. Fragments and arrays are disposed
 * through a snapshot of their rendered nodes, anchored to their first live
 * node.
 *
 * @remarks
 * A `DocumentFragment` is one-shot: appending it moves its children out and
 * leaves the fragment empty. To switch branches properly, pass fragments from
 * the function form (`{() => buildFragment()}`) so a fresh fragment is produced
 * on every switch. A directly passed fragment renders only once and inserts
 * nothing when the branch is shown again.
 *
 * Branch values that are not DOM nodes (e.g. a raw string or number) have no
 * node for `Show` to bind its lifecycle to, so they cannot be disposed by
 * `Show` itself. Their subscription cleanup is handled by the layer that
 * inserts the returned derived signal (normally `appendChildren`).
 *
 * @typeParam TWhen - Type of the condition value.
 * @param props - Props bag.
 * @param props.when - Condition; truthiness controls which branch is shown. May be a plain value or a signal.
 * @param props.fallback - Content rendered when `when` is falsy. May be children or a function `(when) => children`.
 * @param props.children - Content rendered when `when` is truthy. May be children or a function `(when) => children`.
 * @param props.autoDispose - When `true` (default), disposes previous function-slot results on switch and disposes held branches when the shown element is disposed. Pass `false` to opt out of all disposal/lifecycle handling.
 * @returns A `JSXElement` (or derived-signal node) representing the active branch.
 * @example
 * ```tsx
 * const loggedIn = signal(false);
 * <Show when={loggedIn} fallback="Please sign in">Welcome!</Show>
 * <Show when={loggedIn}>{() => <Dashboard user={loggedIn.value} />}</Show>
 * ```
 */
export function Show<TWhen>(props: {
    when: SignalOrValue<TWhen | undefined | null>;
    fallback?: ComponentChildren | ((when: TWhen | undefined | null) => ComponentChildren);
    children: ComponentChildren | ((when: TWhen | undefined | null) => ComponentChildren);
    autoDispose?: boolean;
}): JSXElement {
    const autoDispose = props.autoDispose !== false;
    const factoryNodes = new WeakSet<object>();
    const contentDisposables = new WeakMap<object, object>();
    const contentAnchors = new WeakMap<object, EventTarget>();
    const contentNodes = new Set<object>();
    let teardownNode: EventTarget | null = null;

    function disposeAll(): void {
        const contents = Array.from(contentNodes);
        contentNodes.clear();
        teardownNode = null;
        for (const content of contents) {
            const disposable = contentDisposables.get(content);
            factoryNodes.delete(content);
            contentDisposables.delete(content);
            contentAnchors.delete(content);
            if (disposable != null)
                disposeContent(disposable);
        }
    }

    function moveTeardown(node: EventTarget | null): void {
        if (teardownNode === node)
            return;
        if (teardownNode)
            removeDisposingListener(teardownNode, disposeAll);
        teardownNode = node;
        if (teardownNode)
            addDisposingListener(teardownNode, disposeAll);
    }

    function getContent(whenValue: any): JSXElement {
        let content = whenValue ? props.children : props.fallback;
        const isFactory = typeof content === "function";
        if (isFactory)
            content = (content as (when: any) => ComponentChildren)(whenValue);
        content ??= new Text("");
        if (autoDispose && content != null && typeof content === "object") {
            contentNodes.add(content);
            if (!contentDisposables.has(content)) {
                const disposable = contentToDispose(content);
                if (disposable != null)
                    contentDisposables.set(content, disposable);
            }
            if (!contentAnchors.has(content)) {
                const anchor = findAnchor(content);
                if (anchor)
                    contentAnchors.set(content, anchor);
            }
            if (isFactory)
                factoryNodes.add(content);
        }
        return content as unknown as JSXElement;
    }

    if (isSignalLike(props.when)) {
        const sig = derivedSignal<JSXElement>(props.when, getContent);
        observeSignal(sig, function(args) {
            if (!autoDispose)
                return;

            const prev = args.prevValue;
            const next = args.newValue;
            const nextAnchor = next != null && typeof next === "object"
                ? contentAnchors.get(next) ?? null
                : null;

            // repoint the subscription away from the outgoing branch before any
            // disposal below, otherwise disposing it would tear down this Show
            if (args.isInitial || args.hasChanged)
                args.lifecycleNode = nextAnchor;

            if (args.hasChanged && prev != null && typeof prev === "object" && factoryNodes.has(prev)) {
                const disposable = contentDisposables.get(prev);
                const prevAnchor = contentAnchors.get(prev);
                // detach our teardown listener before disposing the outgoing factory content
                if (prevAnchor && teardownNode === prevAnchor)
                    moveTeardown(null);
                contentNodes.delete(prev);
                factoryNodes.delete(prev);
                contentDisposables.delete(prev);
                contentAnchors.delete(prev);
                if (disposable != null)
                    disposeContent(disposable);
            }

            moveTeardown(nextAnchor);
        });
        return sig as unknown as JSXElement;
    }

    return getContent(props.when);
}
