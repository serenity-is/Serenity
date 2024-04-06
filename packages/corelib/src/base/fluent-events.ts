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

type EventHandler = Function & {
    callable: EventListenerOrEventListenerObject;
    delegationSelector?: string | Function;
    oneOff?: boolean;
    uidEvent?: string;
}

type EventHandlers = Record<string, EventHandler>;
type ElementEvents = Record<string, EventHandlers>;

const eventRegistry: WeakMap<EventTarget, ElementEvents> = new WeakMap();

export function triggerRemoveAndClearAll(element: EventTarget): void {
    let events = eventRegistry.get(element);
    if (!events)
        return;

    var removeHandlers = events["remove"];
    if (removeHandlers) {
        for (const [_, handler] of Object.entries(removeHandlers)) {
            if (typeof handler.callable === "function") {
                try {
                    handler.callable.call(element, { target: element });
                }
                catch {
                }
            }
        }
    }
    for (const [typeEvent, handlers] of Object.entries(events)) {
        for (const [handlerKey, handler] of Object.entries(handlers)) {
            element.removeEventListener(typeEvent, handler as any, Boolean(handler.delegationSelector));
            delete handlers[handlerKey];
        }
    }

    eventRegistry.delete(element);
}

function getElementEvents(element: EventTarget): ElementEvents {
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
            removeListener(element, event.type, fn)
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
                    removeListener(element, event.type, selector, fn)
                }

                return fn.apply(target, [event])
            }
        }
    }
}

function findHandler(handlers: EventHandlers, callable: Function, delegationSelector: any = null) {
    return Object.values(handlers)
        .find((event) => event.callable === callable && event.delegationSelector === delegationSelector)
}

function normalizeParameters(originalTypeEvent: string, handler: any, delegationFunction: any) {
    const isDelegated = typeof handler === 'string'
    const callable = isDelegated ? delegationFunction : (handler || delegationFunction);
    let typeEvent = getTypeEvent(originalTypeEvent)
    if (originalTypeEvent.indexOf(".bs.") >= 0)
        typeEvent = originalTypeEvent;
    return [isDelegated, callable, typeEvent]
}

export function addListener(element: EventTarget, originalTypeEvent: string, handler: Function | string, delegationFunction?: Function, oneOff?: boolean) {
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
    const fn = (isDelegated ?
        delegationHandler(element, handler as string, callable) :
        baseHandler(element, callable)) as any as EventHandler;

    fn.delegationSelector = isDelegated ? handler : null;
    fn.callable = callable;
    fn.oneOff = oneOff;
    fn.uidEvent = uid;
    handlers[uid] = fn;

    element.addEventListener(typeEvent, fn as any, isDelegated);
}

function removeHandler(element: EventTarget, events: ElementEvents, typeEvent: string, handler: any, delegationSelector: string | Function) {
    const fn = findHandler(events[typeEvent], handler, delegationSelector)

    if (!fn) {
        return
    }

    element.removeEventListener(typeEvent, fn as any, Boolean(delegationSelector))
    delete events[typeEvent][(fn as any).uidEvent]
}

function removeNamespacedHandlers(element: EventTarget, events: ElementEvents, typeEvent: string, namespace: string) {
    const handlers = events[typeEvent] || {};

    for (const [handlerKey, handler] of Object.entries(handlers)) {
        if (handlerKey.includes(namespace)) {
            removeHandler(element, events, typeEvent, handler.callable, handler.delegationSelector);
        }
    }
}

function getTypeEvent(event: string) {
    // allow to get the native events from namespaced events ('click.bs.button' --> 'click')
    event = event.replace(stripNameRegex, '')
    return customEvents[event] || event
}

export function removeListener(element: EventTarget, originalTypeEvent: string, handler?: any, delegationHandler?: Function): void {
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
    const handlers = events[typeEvent] || {};
    const isNamespace = originalTypeEvent.startsWith('.');

    if (typeof callable !== 'undefined') {
        if (!Object.keys(handlers).length) {
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

    for (const [keyHandlers, handler] of Object.entries(handlers)) {
        const handlerKey = keyHandlers.replace(stripUidRegex, '');

        if (!inNamespace || originalTypeEvent.includes(handlerKey)) {
            removeHandler(element, events, typeEvent, handler.callable, handler.delegationSelector);
        }
    }
}

export function triggerEvent(element: EventTarget, type: string, args?: any): Event & { isDefaultPrevented?(): boolean } {
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