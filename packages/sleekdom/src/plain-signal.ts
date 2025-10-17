import type { SignalLike } from "./types/signal-like";

/**
 * A minimal, non-tracking SignalLike implementation.
 * - No dependency tracking, effects, or batching
 * - Synchronous notifications to subscribed listeners
 */
export class PlainSignal<T = any> implements SignalLike<T> {
    _value: T;
    _listeners: Array<(value: T) => void> = [];

    constructor(initialValue?: T) {
        this._value = initialValue;
    }

    get value(): T {
        return this._value;
    }

    set value(newValue: T) {
        if (this._value !== newValue) {
            this._value = newValue;
            for (const listener of this._listeners) {
                listener(newValue);
            }
        }
    }

    peek(): T {
        return this._value;
    }

    /**
     * Subscribe to value changes. The callback is called immediately with the current value.
     * Note that this does not do any batching or dependency tracking; callbacks are called synchronously when the value changes.
     * @param callback 
     * @returns 
     */
    subscribe(callback: (value: T) => void): () => void {
        callback(this._value);
        this._listeners.push(callback);
        return () => {
            this.unsubscribe(callback);
        };
    }

    unsubscribe(callback: (value: T) => void): void {
        this._listeners = this._listeners.filter(listener => listener !== callback);
    }
}