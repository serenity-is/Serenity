import { type SignalLike } from "@serenity-is/sleekdom";

export function isSignalLike(val: any): val is SignalLike<any> {
    return val != null && typeof val === "object" && typeof val.subscribe === "function" && typeof val.peek === "function" && 'value' in val;
}

/**
 * This calls the callback whenever the signal value changes.
 * The callback is called immediately upon subscription, whether the signal library
 * calls it immediately or not.
 * @param signal Signal to observe
 * @param callback Callback to call when the signal value changes with new and old value.
 * It is called immediately with the current value as newValue and prevValue as undefined.
 * The third parameter 'initial' is true when the callback is called immediately upon subscription.
 * @returns A function to dispose the effect if the signal library supports unsubscription
 */
export function observeSignal<T>(signal: SignalLike<T>, 
    callback: ((value: T, prev: T, initial: boolean) => void)): () => void {
    let prev = signal.peek();
    callback(prev, undefined, true);
    let immediate = true;
    const dispose = signal.subscribe((value: any) => {
        if (!immediate) {
            callback(value, prev, false);
            prev = value;
        }
    });
    immediate = false;
    return dispose;
}