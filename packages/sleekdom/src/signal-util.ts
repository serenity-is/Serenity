import { addDisposingListener } from "./disposing-listener";
import { type EffectDisposer, type SignalLike } from "./types";

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
    callback: ((this: { dispose?: EffectDisposer }, value: T, prev: T, initial: boolean) => void)): EffectDisposer {
    let prev: T;
    let immediate = true;
    const dispose = signal.subscribe(function (this: { dispose?: EffectDisposer }, value: T) {
        if (immediate) {
            if (this?.dispose) {
                immediate = false;
                callback.call(this, prev = value, undefined, true);
                return;
            }
        }
        else {
            try {
                callback.call({ dispose }, value, prev, false);
            }
            finally {
                prev = value;
            }
        }
    });
    if (immediate) {
        immediate = false;
        prev = signal.peek();
        callback.call({ dispose }, prev, undefined, true);
    }

    return dispose;
}

/**
 * Observes a signal and ties its lifecycle to a node by adding a disposing listener to the node
 * if the signal library supports unsubscription.
 * @param signal Signal to observe
 * @param node Node to tie the signal's lifecycle to
 * @param callback Callback to call when the signal value changes
 */
export function observeSignalForNode<T>(signal: SignalLike<T>, node: EventTarget,
    callback: ((value: T, prev: T, initial: boolean) => void)): void {
    const dispose = observeSignal(signal, callback);
    if (dispose) {
        addDisposingListener(node, dispose);
    }
}   


