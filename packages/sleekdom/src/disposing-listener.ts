const disposingListenersSymbol = Symbol("sleekdom:disposingListeners");

function getDisposingListeners(): WeakMap<EventTarget, (() => void)[]> {
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
    for (const disposer of handlers) {
        try {
            disposer();
        } catch {
            // ignore
        }
    }
    disposingListeners.delete(target);
    target.removeEventListener("disposing", disposingEventListener);
};

/**
 * Adds a disposing listener to an element. Note that the listener itself is not added as an event listener,
 * but will be called when the `disposing` event is dispatched on the element, along with other disposing listeners.
 * @param target The element to add the listener to.
 * @param handler The disposing listener to add.
 * @returns The element that the listener was added to.
 */
export function addDisposingListener<T extends EventTarget>(target: T, handler: () => void): T {
    if (!target || !handler)
        return target;
    const disposingListeners = getDisposingListeners();
    let listeners = disposingListeners.get(target);
    if (!listeners) {
        disposingListeners.set(target, listeners = []);
        target.addEventListener("disposing", disposingEventListener, { once: true });
    }
    listeners.push(handler);
    return target;
}

/**
 * Removes a disposing listener from an element. Note that this does not remove an event listener from the element,
 * but removes the listener from the list of disposing listeners that will be called when the `disposing` event
 * is dispatched on the element. If no more disposing listeners remain, the `disposing` event listener is also 
 * removed from the element.
 * @param target The element to remove the listener from.
 * @param handler The disposing listener to remove.
 * @returns The element that the listener was removed from.
 */
export function removeDisposingListener<T extends EventTarget>(target: T, handler: () => void): T {
    if (!target || !handler)
        return target;
    const disposingListeners = getDisposingListeners();
    const listeners = disposingListeners.get(target);
    if (listeners) {
        const index = listeners.indexOf(handler);
        if (index >= 0) {
            listeners.splice(index, 1);
        }
        if (listeners.length === 0) {
            disposingListeners.delete(target);
        }
    }
    return target;
}