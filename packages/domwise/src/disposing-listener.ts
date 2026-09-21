const disposingListenersSymbol = Symbol.for("Serenity.disposingListeners");
const lifecycleRootSymbol = Symbol.for("Serenity.lifecycleRoot");

/**
 * Returns the global `WeakMap` that stores disposing listeners keyed by
 * target `EventTarget`. The map is lazily created on `globalThis` under the
 * `Serenity.disposingListeners` symbol.
 * @returns The shared `WeakMap` of target → listener array.
 */
export function getDisposingListeners(): WeakMap<EventTarget, ({
    callback: (el: EventTarget) => void,
    regKey: string | undefined
})[]> {
    return (globalThis as any)[disposingListenersSymbol] ||= new WeakMap();
}

function disposingEventListener(ev: Event): void {
    if (ev && ev.target && (!ev.currentTarget || ev.currentTarget === ev.target))
        invokeDisposingListeners(ev?.target);
};

/**
 * Dispatches a `disposing` event on the target element, causing any
 * listeners registered via {@link addDisposingListener} to be invoked.
 * No-ops when `target` is falsy or `CustomEvent` is unavailable.
 * @param target - Event target to dispatch the event on.
 * @param opt - Optional event configuration.
 * @param opt.bubbles - Whether the event should bubble. Defaults to `false`.
 * @param opt.cancelable - Whether the event is cancelable. Defaults to `false`.
 */
export function dispatchDisposingEvent(target: EventTarget, opt?: { bubbles?: boolean, cancelable?: boolean }): void {
    if (!target || typeof CustomEvent !== "function")
        return;

    const event = new CustomEvent("disposing", {
        bubbles: opt?.bubbles ?? false,
        cancelable: opt?.cancelable ?? false
    });

    target.dispatchEvent(event);
}

/**
 * Synchronously invokes all disposing listeners registered for `node` and
 * removes the internal `disposing` DOM listener from the target.
 *
 * This does **not** dispatch a `disposing` DOM event; use
 * {@link dispatchDisposingEvent} for that. Listener errors are swallowed.
 *
 * @param node - Target whose disposing listeners should be invoked. No-op when falsy.
 * @param opt - Optional behavior flags.
 * @param opt.descendants - When `true`, also invokes listeners registered on descendant nodes, descending into shadow roots, `<template>` content and `DocumentFragment`s as well as elements/text/comments.
 * @param opt.excludeSelf - When `true`, skips listeners registered directly on `node` itself (only descendants are invoked, in combination with `descendants`).
 */
export function invokeDisposingListeners(node: EventTarget, opt?: {
    descendants?: boolean,
    excludeSelf?: boolean,
}): void {
    if (!node)
        return;

    const disposingListeners = getDisposingListeners();

    function invokeFor(el: EventTarget) {
        const listeners = disposingListeners.get(el);
        if (!listeners)
            return;
        disposingListeners.delete(el);
        el.removeEventListener("disposing", disposingEventListener);
        for (const disposer of listeners) {
            try {
                disposer.callback(el);
            } catch {
                // ignore
            }
        }
    }

    if (opt?.descendants) {
        const descendants: Node[] = [];
        collectDescendants(node as unknown as Node, descendants);
        for (let i = 0; i < descendants.length; i++) {
            invokeFor(descendants[i] as EventTarget);
        }
    }

    if (!opt?.excludeSelf) {
        invokeFor(node);
    }
};

/**
 * Collects all descendant nodes of `root`, descending into shadow roots,
 * `<template>` content and `DocumentFragment`s (which a `NodeIterator` does
 * not reach).
 */
function collectDescendants(root: Node, out: Node[]): void {
    if (!root || typeof root.nodeType !== "number")
        return;
    const shadowRoot = (root as any).shadowRoot as Node | null | undefined;
    if (shadowRoot)
        collectDescendants(shadowRoot, out);
    if ((root as any).content instanceof DocumentFragment)
        collectDescendants((root as any).content, out);
    let child = root.firstChild;
    while (child) {
        out.push(child);
        collectDescendants(child, out);
        child = child.nextSibling;
    }
};

/**
 * Registers a disposing listener for an element.
 *
 * The `handler` is not added as a direct DOM event listener; instead it is
 * stored in an internal `WeakMap` and invoked when a `disposing` event is
 * dispatched on `target` (via {@link dispatchDisposingEvent} or
 * {@link invokeDisposingListeners}). The first registration on a given target
 * also installs a `disposing` event listener to drive the callback list. Duplicate `handler` references are ignored (with optional `regKey`
 * tracking), so calling this multiple times with the same callback is safe.
 *
 * @typeParam T - Type of the target event target.
 * @param target - Element/event target to attach the listener to. No-op when `null`/`undefined`.
 * @param handler - Callback invoked with the element when it is disposing. No-op when `null`/`undefined`.
 * @param regKey - Optional registration key used to de-duplicate or later remove this listener.
 * @returns The `target` that was passed in, for chaining.
 * @throws {Error} When the same `handler` is already registered with a different `regKey`.
 */
export function addDisposingListener<T extends EventTarget>(target: T | null | undefined, handler: ((el: T) => void) | undefined | null, regKey?: string): T | null | undefined {
    if (!target || !handler)
        return target;
    const disposingListeners = getDisposingListeners();
    let listeners = disposingListeners.get(target);
    if (!listeners) {
        if (typeof target.addEventListener !== "function")
            return target;
        disposingListeners.set(target, listeners = [{
            callback: handler!,
            regKey
        }]);
        target.addEventListener("disposing", disposingEventListener);
        return target;
    }
    const existing = listeners.find(x => x.callback === handler);
    if (existing) {
        if (existing.regKey && regKey &&
            existing.regKey !== regKey) {
            throw new Error("A disposing listener with the same callback but different regKey is already registered on the target element.");
        }
        if (regKey && !existing.regKey) {
            existing.regKey = regKey;
        }
        return target;
    }
    listeners.push({ callback: handler, regKey });
    return target;
}

/**
 * Removes a previously registered disposing listener from an element.
 *
 * This removes the entry from the internal disposing-listener registry, not a
 * direct DOM `EventListener`. When both `handler` and `regKey` are provided, a
 * listener matches only when **both** its callback reference and its `regKey`
 * match. When only one is provided, matching uses that one (by `regKey`, or by
 * callback reference). When the last listener is removed the underlying
 * `disposing` DOM listener is also detached from the target.
 *
 * @typeParam T - Type of the target event target.
 * @param target - Element/event target to remove the listener from. No-op when `null`/`undefined`.
 * @param handler - Callback whose registration should be removed. If `null`/`undefined`, matching falls back to `regKey`.
 * @param regKey - Optional registration key to match against.
 * @returns The `target` that was passed in, for chaining.
 */
export function removeDisposingListener<T extends EventTarget>(target: T | null | undefined, handler: (() => void) | undefined | null, regKey?: string | undefined | null): T | null | undefined {
    if (!target || (!handler && !regKey))
        return target;
    const disposingListeners = getDisposingListeners();
    const listeners = disposingListeners.get(target);
    if (listeners) {
        for (let index = listeners.length - 1; index >= 0; index--) {
            const listener = listeners[index];
            const matches = handler && regKey
                // both provided: require both to match
                ? (handler === listener.callback && listener.regKey === regKey)
                : (regKey ? listener.regKey === regKey : handler === listener.callback);
            if (matches) {
                listeners.splice(index, 1);
            }
        }
        if (listeners.length === 0) {
            disposingListeners.delete(target);
            if (typeof target.removeEventListener === "function") {
                target.removeEventListener("disposing", disposingEventListener);
            }
        }
    }
    return target;
}

/**
 * Gets or sets the current lifecycle root element used to scope signal subscriptions.
 *
 * The lifecycle root is the `EventTarget` whose `disposing` event will dispose
 * effects created during JSX construction (e.g. via `observeSignal` with
 * `useLifecycleRoot: true`).
 *
 * This value is **not** set automatically during JSX creation. It is stored on
 * `globalThis`, so callers that install a root should restore the previous one;
 * prefer {@link withLifecycleRoot} which restores it even when the callback
 * throws.
 *
 * @param args - When provided, the first element is installed as the new lifecycle root.
 * When called with no arguments the current root (or `null` if none) is returned.
 * @returns The current lifecycle root, or the previous root when a new one is being set. Returns `null` if none is set.
 */
export function currentLifecycleRoot(...args: Element[]): Element | null {
    if (args.length > 0) {
        const prev: Element = (globalThis as any)[lifecycleRootSymbol] || null;
        (globalThis as any)[lifecycleRootSymbol] = args[0] || null;
        return prev;
    }
    return (globalThis as any)[lifecycleRootSymbol] || null;
}

/**
 * Runs `fn` with the lifecycle root temporarily set to `root`, restoring the
 * previous root afterwards even if `fn` throws.
 *
 * Use this instead of calling {@link currentLifecycleRoot} directly so that a
 * thrown error cannot leave a stale root in the global state.
 *
 * @typeParam T - Return type of `fn`.
 * @param root - Root to install for the duration of `fn` (`null`/`undefined` clears it).
 * @param fn - Callback executed while the root is installed.
 * @returns The value returned by `fn`.
 */
export function withLifecycleRoot<T>(root: Element | null | undefined, fn: () => T): T {
    const prev = currentLifecycleRoot(root as Element);
    try {
        return fn();
    }
    finally {
        currentLifecycleRoot(prev as Element);
    }
}