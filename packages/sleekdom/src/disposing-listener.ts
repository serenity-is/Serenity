const disposingListenersSymbol = Symbol.for("Serenity.disposingListeners");

export function getDisposingListeners(): WeakMap<EventTarget, ({
    callback: () => void,
    regKey?: string
})[]> {
    return (globalThis as any)[disposingListenersSymbol] ||= new WeakMap();
}

const disposingEventListener = (ev: Event) => {
    onElementDisposing(ev?.target as EventTarget);
};

/**
 * Called when an element is getting disposed. This will invoke all registered disposing listeners for the element.
 * This is normally called automatically when `disposing` event is dispatched on the element.
 * It may also be called internally by Fluent events while disposing an element by `remove`, or `empty` (for descendants).
 * You should not normally call this method directly.
 * @param target The element that is being disposed.
 */
export const onElementDisposing = (target: EventTarget) => {
    if (!target)
        return;

    const disposingListeners = getDisposingListeners();
    const handlers = disposingListeners.get(target);
    if (!handlers)
        return;
    disposingListeners.delete(target);
    target.removeEventListener("disposing", disposingEventListener);
    for (const disposer of handlers) {
        try {
            disposer.callback();
        } catch {
            // ignore
        }
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
export function addDisposingListener<T extends EventTarget>(target: T, handler: () => void, regKey?: string): T {
    if (!target || !handler)
        return target;
    const disposingListeners = getDisposingListeners();
    let listeners = disposingListeners.get(target);
    if (!listeners) {
        if (typeof target.addEventListener !== "function")
            return target;
        disposingListeners.set(target, listeners = []);
        target.addEventListener("disposing", disposingEventListener, { once: true });
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
export function removeDisposingListener<T extends EventTarget>(target: T, handler: () => void, regKey?: string): T {
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