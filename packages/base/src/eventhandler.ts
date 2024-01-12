/**
 * --------------------------------------------------------------------------
 * Adapted from: Bootstrap dom/event-handler.js
 * Licensed under MIT (https://github.com/twbs/bootstrap/blob/main/LICENSE)
 * --------------------------------------------------------------------------
 */

import { getjQuery } from "./environment"

const namespaceRegex = /[^.]*(?=\..*)\.|.*/
const stripNameRegex = /\..*/
const stripUidRegex = /::\d+$/
let uidEvent = 1
const customEvents: Record<string, string> = {
    mouseenter: 'mouseover',
    mouseleave: 'mouseout'
}

function makeEventUid(prefix: string): string {
    return `${prefix}::${uidEvent++}`;
}

const eventRegistry: WeakMap<EventTarget, any> = new WeakMap();

export function triggerRemoveAndClearAll(element: EventTarget): void {
    let events = eventRegistry.get(element);
    if (!events)
        return;

    var removeEvents = events["remove"];
    if (removeEvents) {
        for (const [_, event] of Object.entries(removeEvents)) {
            if (typeof (event as any).callable === "function") {
                try {
                    (event as any).callable.call(element, { target: element });
                }
                catch {
                }
            }
        }
    }

    eventRegistry.delete(element);
}

function getElementEvents(element: EventTarget): any {
    var events = eventRegistry.get(element);
    if (!events)
        eventRegistry.set(element, events = {});
    return events;
}

function hydrateObj(obj: any, meta = {}) {
    for (const [key, value] of Object.entries(meta)) {
        try {
            obj[key] = value
        } catch {
            Object.defineProperty(obj, key, {
                configurable: true,
                get() {
                    return value
                }
            })
        }
    }

    return obj;
}

function baseHandler(element: EventTarget, fn: any) {
    return function handler(event: Event) {
        hydrateObj(event, { delegateTarget: element })

        if ((handler as any).oneOff) {
            EventHandler.off(element, event.type as any, fn)
        }

        return fn.apply(element, [event])
    }
}

function delegationHandler(element: EventTarget, selector: string, fn: Function) {
    return function handler(event: Event) {
        const domElements = (element as any).querySelectorAll(selector)

        for (let { target } = event; target && target !== this; target = (target as any).parentNode) {
            for (const domElement of (domElements as any)) {
                if (domElement !== target) {
                    continue
                }

                hydrateObj(event, { delegateTarget: target })

                if ((handler as any).oneOff) {
                    EventHandler.off(element, event.type, selector, fn)
                }

                return fn.apply(target, [event])
            }
        }
    }
}

function findHandler(events: any, callable: any, delegationSelector: any = null) {
    return Object.values(events)
        .find((event: any) => event.callable === callable && event.delegationSelector === delegationSelector)
}

function normalizeParameters(originalTypeEvent: string, handler: any, delegationFunction: any) {
    const isDelegated = typeof handler === 'string'
    const callable = isDelegated ? delegationFunction : (handler || delegationFunction);
    let typeEvent = getTypeEvent(originalTypeEvent)
    if (originalTypeEvent.indexOf(".bs.") >= 0)
        typeEvent = originalTypeEvent;
    return [isDelegated, callable, typeEvent]
}

function addHandler(element: EventTarget, originalTypeEvent: string, handler: Function | string, delegationFunction: Function, oneOff?: boolean) {
    if (typeof originalTypeEvent !== 'string' || !element) {
        return;
    }

    const $ = getjQuery();
    if ($) {
        let $element = $(element);
        if (oneOff)
            $element.one(originalTypeEvent, handler, delegationFunction);
        else
            $element.on(originalTypeEvent, handler, delegationFunction);
        return;
    }
    
    let [isDelegated, callable, typeEvent] = normalizeParameters(originalTypeEvent, handler, delegationFunction);
    if (!callable)
        return;

    if (originalTypeEvent in customEvents) {
        const wrapFunction = (fn: Function) => {
            return function (event: Event & { relatedTarget?: any, delegateTarget: any }) {
                if (!event.relatedTarget || (event.relatedTarget !== event.delegateTarget && !event.delegateTarget.contains(event.relatedTarget))) {
                    return fn.call(this, event)
                }
            }
        }

        callable = wrapFunction(callable)
    }

    const events = getElementEvents(element);
    const handlers = events[typeEvent] || (events[typeEvent] = {});
    const previousFunction = findHandler(handlers, callable, isDelegated ? handler : null);

    if (previousFunction) {
        (previousFunction as any).oneOff = (previousFunction as any).oneOff && oneOff
        return;
    }

    const uid = makeEventUid(originalTypeEvent.replace(namespaceRegex, ''))
    const fn: any = isDelegated ?
        delegationHandler(element, handler as string, callable) :
        baseHandler(element, callable);

    fn.delegationSelector = isDelegated ? handler : null;
    fn.callable = callable;
    fn.oneOff = oneOff;
    fn.uidEvent = uid;
    handlers[uid] = fn;

    element.addEventListener(typeEvent, fn, isDelegated);
}

function removeHandler(element: EventTarget, events: any, typeEvent: string, handler: any, delegationSelector: string) {
    const fn = findHandler(events[typeEvent], handler, delegationSelector)

    if (!fn) {
        return
    }

    element.removeEventListener(typeEvent, fn as any, Boolean(delegationSelector))
    delete events[typeEvent][(fn as any).uidEvent]
}

function removeNamespacedHandlers(element: EventTarget, events: any, typeEvent: string, namespace: string) {
    const storeElementEvent = events[typeEvent] || {};

    for (const [handlerKey, event] of Object.entries(storeElementEvent)) {
        if (handlerKey.includes(namespace)) {
            removeHandler(element, events, typeEvent, (event as any).callable, (event as any).delegationSelector);
        }
    }
}

function getTypeEvent(event: string) {
    // allow to get the native events from namespaced events ('click.bs.button' --> 'click')
    event = event.replace(stripNameRegex, '')
    return customEvents[event] || event
}

export namespace EventHandler {
    export function on<K extends keyof HTMLElementEventMap>(element: EventTarget, type: K, listener: (this: HTMLElement, ev: HTMLElementEventMap[K]) => any): void;
    export function on(element: EventTarget, type: string, listener: EventListener): void;
    export function on(element: EventTarget, type: string, selector: string, delegationHandler: Function): void;
    export function on(element: EventTarget, type: string, handler: any, delegationHandler?: Function): void {
        addHandler(element, type, handler, delegationHandler, /*oneOff*/ false);
    }

    export function one<K extends keyof HTMLElementEventMap>(element: EventTarget, type: K, listener: (this: HTMLElement, ev: HTMLElementEventMap[K]) => any): void;
    export function one(element: EventTarget, type: string, listener: EventListener): void;
    export function one(element: EventTarget, type: string, selector: string, delegationHandler: Function): void;
    export function one(element: EventTarget, type: string, handler: any, delegationHandler?: Function): void {
        addHandler(element, type, handler, delegationHandler, true);
    }

    export function off<K extends keyof HTMLElementEventMap>(element: EventTarget, type: K, listener?: (this: HTMLElement, ev: HTMLElementEventMap[K]) => any): void;
    export function off(element: EventTarget, type: string, listener?: EventListener): void;
    export function off(element: EventTarget, type: string, selector?: string, delegationHandler?: Function): void;
    export function off(element: EventTarget, originalTypeEvent: string, handler?: any, delegationHandler?: Function): void {
        if (typeof originalTypeEvent !== 'string' || !element) {
            return
        }

        const $ = getjQuery();
        if ($) {
            $(element).off(originalTypeEvent, handler, delegationHandler);
            return;
        }

        const [isDelegated, callable, typeEvent] = normalizeParameters(originalTypeEvent, handler, delegationHandler);
        const inNamespace = typeEvent !== originalTypeEvent;
        const events = getElementEvents(element);
        const storeElementEvent = events[typeEvent] || {};
        const isNamespace = originalTypeEvent.startsWith('.');

        if (typeof callable !== 'undefined') {
            if (!Object.keys(storeElementEvent).length) {
                return;
            }

            removeHandler(element, events, typeEvent, callable, isDelegated ? handler : null);
            return
        }

        if (isNamespace) {
            for (const elementEvent of Object.keys(events)) {
                removeNamespacedHandlers(element, events, elementEvent, originalTypeEvent.slice(1));
            }
        }

        for (const [keyHandlers, event] of Object.entries(storeElementEvent)) {
            const handlerKey = keyHandlers.replace(stripUidRegex, '');

            if (!inNamespace || originalTypeEvent.includes(handlerKey)) {
                removeHandler(element, events, typeEvent, (event as any).callable, (event as any).delegationSelector);
            }
        }
    }

    export function trigger(element: EventTarget, type: string, args?: any): Event & { isDefaultPrevented?(): boolean } {
        if (typeof type !== 'string' || !element) {
            return null;
        }

        const $ = getjQuery();
        const typeEvent = getTypeEvent(type);
        const inNamespace = type !== typeEvent;

        let jQueryEvent = null;
        let bubbles = true;
        let nativeDispatch = true;
        let defaultPrevented = false;

        if (inNamespace && $) {
            jQueryEvent = $.Event(type, args);
            $(element).trigger(jQueryEvent);
            bubbles = !jQueryEvent.isPropagationStopped();
            nativeDispatch = !jQueryEvent.isImmediatePropagationStopped();
            defaultPrevented = jQueryEvent.isDefaultPrevented();
        }

        const evt = hydrateObj(new Event(type, { bubbles, cancelable: true }), args);

        if (defaultPrevented) {
            evt.preventDefault();
        }

        if (nativeDispatch) {
            element.dispatchEvent(evt);
        }

        if (evt.defaultPrevented && jQueryEvent) {
            jQueryEvent.preventDefault();
        }

        return evt
    }
}
