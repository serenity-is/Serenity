import { addDisposingListener, currentLifecycleRoot, removeDisposingListener } from "./disposing-listener";
import { type EffectDisposer, type SignalLike } from "./types";

export function isSignalLike(val: any): val is SignalLike<any> {
    return val != null && typeof val === "object" && typeof val.subscribe === "function" && typeof val.peek === "function" && 'value' in val;
}

export type SignalObserveArgs<T> = {
    /** True if this is the initial call upon subscription. */
    isInitial: boolean
    /** Previous value of the signal. Undefined if initial call. */
    prevValue: T | undefined,
    /** New value of the signal. Undefined if initial call. */
    newValue: T | undefined,
    /** True if the value has changed from previous value. False on initial call. */
    hasChanged: boolean,
    /** The observed signal. */
    readonly signal: SignalLike<T>,
    /**
     * Disposes the signal subscription. Only available if the signal library supports unsubscription.
     */
    effectDisposer: EffectDisposer | undefined;
    /**
     * Gets the lifecycle node to tie the signal's lifecycle to.
     */
    get lifecycleNode(): EventTarget | undefined,
    /**
     * Sets the lifecycle node to tie the signal's lifecycle to. If the useDisposableRoot option is true,
     * and there is a current disposable root, that node will be used instead and lifecycleNode will
     * return that node.
     */
    set lifecycleNode(value: EventTarget | undefined),
}

class SignalObserveArgsImpl<T> implements SignalObserveArgs<T> {
    declare readonly signal: SignalLike<T>;
    declare newValue: T | undefined;
    declare prevValue: T | undefined;
    declare isInitial: boolean;
    declare hasChanged: boolean;
    declare effectDisposer: EffectDisposer | undefined;
    declare private _lifecycleNode: EventTarget | undefined;
    declare private _lifecycleRoot: EventTarget | undefined;

    constructor(signal: SignalLike<T>, lifecycleRoot: EventTarget | undefined, lifecycleNode: EventTarget | undefined) {
        this.signal = signal;
        this.isInitial = true;
        this.hasChanged = false;
        this._lifecycleRoot = lifecycleRoot;
        this._lifecycleNode = lifecycleNode;
    }

    get lifecycleNode(): EventTarget | undefined {
        return this._lifecycleNode;
    }

    set lifecycleNode(value: EventTarget | undefined) {
        if (this._lifecycleRoot || value === this._lifecycleNode) return;
        removeDisposingListener(this._lifecycleNode, this.effectDisposer);
        this._lifecycleNode = value;
        addDisposingListener(this._lifecycleNode, this.effectDisposer);
    }
}

export type ObserveSignalCallback<T> = (args: SignalObserveArgs<T>) => void;

/**
 * This calls the callback whenever the signal value changes.
 * The callback is called immediately upon subscription, whether the signal library
 * calls it immediately or not (though unexpected).
 * @param signal Signal to observe
 * @param callback Callback to call when the signal value changes with the new value
 * and a data object containing the previous value, initial flag and the disposer.
 * It is called immediately and on every change. The data.isInitial is true on the first call.
 */
export function observeSignal<T>(signal: SignalLike<T>, callback: ObserveSignalCallback<T>, opt?: {
    /** 
     * If true (default), and there is a `currentDisposableRoot()` at the time of subscription,
     * the signal's lifecycle will be tied to that node by adding a disposing listener to it,
     * instead of the node passed in the `useNode` function.
     */
    useLifecycleRoot?: boolean,
    /**
     * Optional node to tie the signal's lifecycle to. Ignored if useLifecycleRoot is true
     * and there is a current lifecycle root.
     */
    lifecycleNode?: EventTarget
}): EffectDisposer {

    const lifecycleRoot = (opt?.useLifecycleRoot ?? true) ? currentLifecycleRoot() : void 0;
    const args = new SignalObserveArgsImpl(signal, lifecycleRoot, lifecycleRoot ?? opt?.lifecycleNode);
    const disposer = args.signal.subscribe(function (this: { dispose?: EffectDisposer }, value: T) {
        args.newValue = value;
        if (args.isInitial) {
            this?.dispose && (args.effectDisposer = this.dispose.bind(this));
        }
        args.hasChanged = !args.isInitial && args.prevValue !== args.newValue;
        try {
            callback(args);
        }
        finally {
            args.prevValue = args.newValue;
            args.isInitial = false;
        }
    });
    if (disposer && !args.effectDisposer) {
        if (args.lifecycleNode) {
            addDisposingListener(args.lifecycleNode, args.effectDisposer = disposer);
        }
    }
    return args.effectDisposer;
}



