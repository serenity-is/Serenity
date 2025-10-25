import { EventData, type EventEmitter, type IEventData } from "../core/event";
import type { ArgsGrid } from "../core/eventargs";
import type { IGrid } from "../core/igrid";

export function triggerGridEvent<TArgs extends ArgsGrid, TEventData extends IEventData = IEventData>(this: IGrid,
    evt: EventEmitter<TArgs, TEventData>, args?: TArgs, e?: TEventData) {
    e = e || new EventData() as any;
    args = args || {} as any;
    args.grid = this;
    return evt.notify(args, e, this);
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

