import { type EventEmitter, type EventData } from "../core/event";
import type { ArgsGrid } from "../core/eventargs";
import type { ISleekGrid } from "../core/isleekgrid";

/**
 * Fires an `EventEmitter` on behalf of the grid, injecting `args.grid = this`
 * before delegating to `evt.notify`.
 * @template TArgs - Event args type (includes `grid`).
 * @template TEventData - Underlying native event type.
 * @param evt - Event emitter to notify.
 * @param args - Event args without `grid`; `grid` is injected.
 * @param e - Optional native event payload.
 * @returns Wrapped {@link EventData} including propagation helpers and return values.
 */
export function triggerGridEvent<TArgs extends ArgsGrid, TEventData = {}>(this: ISleekGrid,
    evt: EventEmitter<TArgs, TEventData>, args?: Omit<TArgs, "grid">, e?: TEventData): EventData & { getReturnValue(): any; getReturnValues(): any[]; args: TArgs } {
    args ??= {} as any;
    (args as TArgs).grid = this;
    return evt.notify(args as TArgs, e, this);
}

/**
 * Adds an event listener, preferring `jQuery.on` when available for namespaced
 * handling, otherwise `addEventListener` with `eventDisposer` signal.
 * @template K - DOM event name.
 * @param el - Target element.
 * @param type - Event type.
 * @param listener - Event listener.
 * @param args - Options (`capture`, `oneOff`, `signal`, `passive`).
 */
export function addListener<K extends keyof HTMLElementEventMap>(this: {
    jQuery: (el: HTMLElement) => { on: (type: string, listener: any) => void },
    eventDisposer: AbortController,
    uid: string
}, el: HTMLElement, type: K, listener: (this: HTMLElement, ev: HTMLElementEventMap[K]) => any, args?: { capture?: boolean, oneOff?: boolean, signal?: AbortSignal, passive?: boolean }): void {
    // can't use jQuery on with options, so we fallback to native addEventListener
    if (!args?.capture && !args?.signal && !args?.passive && this.jQuery) {
        this.jQuery(el).on(type + "." + this.uid, listener as any);
    }
    else {
        el.addEventListener(type, listener, {
            signal: this.eventDisposer?.signal,
            ...args
        });
    }
}

/**
 * Removes an event listener, mirroring {@link addListener} (jQuery or native).
 * @template K - DOM event name.
 * @param el - Target element.
 * @param type - Event type.
 * @param listener - Event listener to remove.
 * @param args - Options; only `capture` is relevant for native removal.
 */
export function removeListener<K extends keyof HTMLElementEventMap>(this: {
    jQuery: (el: HTMLElement) => { off: (type: string, listener: any) => void },
    uid: string
}, el: HTMLElement, type: K, listener: (this: HTMLElement, ev: HTMLElementEventMap[K]) => any, args?: { capture?: boolean }): void {
    // can't use jQuery off with options, so we fallback to native removeEventListener
    if (this.jQuery) {
        this.jQuery(el).off(type + "." + this.uid, listener as any);
    }
    else {
        el.removeEventListener(type, listener, !!args?.capture);
    }
};

