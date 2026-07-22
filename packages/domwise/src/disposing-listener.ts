const disposingListenersSymbol = Symbol.for("Serenity.disposingListeners");
const lifecycleRootSymbol = Symbol.for("Serenity.lifecycleRoot");

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
 * Dispatches a `disposing` event on the target element.
 * @param target - The target element to dispatch the event on.
 * @param opt - Optional event configuration.
 * @param opt.bubbles - Whether the event bubbles. Defaults to `false`.
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
 * Invokes all registered disposing listeners for the element and removes the
 * global `disposing` event listener from the element as it is no longer needed.
 * Note that this does not dispatch a `disposing` event; to do that,
 * use `dispatchDisposingEvent` instead.
 * @param node - The node that is being disposed.
 * @param opt - Optional configuration.
 * @param opt.descendants - If true, also invokes listeners on descendant nodes.
 * @param opt.excludeSelf - If true, skips invoking listeners on the node itself (only descendants).
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

    if (opt?.descendants && node instanceof Element && node.hasChildNodes()) {
        const descendants: Node[] = [];
        const iterator = document.createNodeIterator(
            node as Node,
            NodeFilter.SHOW_ELEMENT | NodeFilter.SHOW_TEXT | NodeFilter.SHOW_COMMENT);
        let currentNode: Node | null;
        while (currentNode = iterator.nextNode()) {
            if (currentNode !== node) {
                descendants.push(currentNode);
            }
        }
        for (let i = 0; i < descendants.length; i++) {
            invokeFor(descendants[i] as EventTarget);
        }
    }

    if (!opt?.excludeSelf) {
        invokeFor(node);
    }
};

/**
 * Adds a disposing listener to an element. Note that the listener itself is not added as an event listener,
 * but will be called when the `disposing` event is dispatched on the element, along with other disposing listeners.
 * @param target The element to add the listener to.
 * @param handler The disposing listener to add.
 * @param regKey An optional registration key to identify the listener.
 * @returns The element that the listener was added to.
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
        target.addEventListener("disposing", disposingEventListener, { once: true });
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
 * Removes a disposing listener from an element. Note that this does not remove an event listener from the element,
 * but removes the listener from the list of disposing listeners that will be called when the `disposing` event
 * is dispatched on the element. If no more disposing listeners remain, the `disposing` event listener is also 
 * removed from the element.
 * @param target The element to remove the listener from.
 * @param handler The disposing listener to remove.
 * @param regKey An optional registration key to identify the listener.
 * @returns The element that the listener was removed from.
 */
export function removeDisposingListener<T extends EventTarget>(target: T | null | undefined, handler: (() => void) | undefined | null, regKey?: string | undefined | null): T | null | undefined {
    if (!target || (!handler && !regKey))
        return target;
    const disposingListeners = getDisposingListeners();
    const listeners = disposingListeners.get(target);
    if (listeners) {
        for (let index = listeners.length - 1; index >= 0; index--) {
            const listener = listeners[index];
            if ((regKey && listener.regKey === regKey) ||
                (handler && handler === listener.callback)) {
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
 * Gets or sets the current lifecycle root element.
 * When called with an argument, sets the lifecycle root and returns the previous value.
 * When called without arguments, returns the current lifecycle root.
 * @param args - If provided, the first element is set as the new lifecycle root.
 * @returns The current (or previous) lifecycle root element, or `null` if none is set.
 */
export function currentLifecycleRoot(...args: Element[]): Element | null {
    if (args.length > 0) {
        const prev: Element = (globalThis as any)[lifecycleRootSymbol] || null;
        (globalThis as any)[lifecycleRootSymbol] = args[0] || null;
        return prev;
    }
    return (globalThis as any)[lifecycleRootSymbol] || null;
}