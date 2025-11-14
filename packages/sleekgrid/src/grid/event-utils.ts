import { type EventEmitter, type EventData } from "../core/event";
import type { ArgsGrid } from "../core/eventargs";
import type { ISleekGrid } from "../core/isleekgrid";

export function triggerGridEvent<TArgs extends ArgsGrid, TEventData = {}>(this: ISleekGrid,
    evt: EventEmitter<TArgs, TEventData>, args?: Omit<TArgs, "grid">, e?: TEventData): EventData & { getReturnValue(): any; getReturnValues(): any[]; args: TArgs } {
    args ??= {} as any;
    (args as TArgs).grid = this;
    return evt.notify(args as TArgs, e, this);
}

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

