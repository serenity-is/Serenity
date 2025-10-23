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
     * Gets the lifecycle root at the time of subscription if useLifecycleRoot option was true.
     */
    readonly lifecycleRoot: EventTarget | undefined;
    /**
     * Gets the lifecycle node to tie the signal's lifecycle to.
     */
    get lifecycleNode(): EventTarget | undefined,
    /**
     * Sets the lifecycle node to tie the signal's lifecycle to.
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
    declare lifecycleRoot: EventTarget | undefined;

    constructor(signal: SignalLike<T>, lifecycleRoot: EventTarget | undefined, lifecycleNode: EventTarget | undefined) {
        this.signal = signal;
        this.isInitial = true;
        this.hasChanged = false;
        this.lifecycleRoot = lifecycleRoot;
        this._lifecycleNode = lifecycleNode;
    }

    get lifecycleNode(): EventTarget | undefined {
        return this._lifecycleNode;
    }

    set lifecycleNode(value: EventTarget | undefined) {
        if (value !== this._lifecycleNode)
        {
            removeDisposingListener(this._lifecycleNode, this.effectDisposer);
            this._lifecycleNode = value;
            addDisposingListener(this._lifecycleNode, this.effectDisposer);
        }
    }
}

export type ObserveSignalCallback<T> = (args: SignalObserveArgs<T>) => void;

/**
 * This calls the callback whenever the signal value changes. It is 
 * called immediately upon subscription with the current value. The callback
 * is called with an argument object that provides information about the signal
 * and the change.
 * @param signal Signal to observe
 * @param callback Callback to execute when the signal value changes.
 */
export function observeSignal<T>(signal: SignalLike<T>, callback: ObserveSignalCallback<T>, opt?: {
    /** 
     * If true, `currentLifecycleRoot()` at the time of subscription will be recorded
     * to be potentially used as the lifecycle node.
     */
    useLifecycleRoot?: boolean,
    /**
     * Optional node to tie the signal's lifecycle to.
     */
    lifecycleNode?: EventTarget
}): EffectDisposer {

    const lifecycleRoot = opt?.useLifecycleRoot ? currentLifecycleRoot() : void 0;
    const args = new SignalObserveArgsImpl(signal, lifecycleRoot, opt?.lifecycleNode);
    const disposer = args.signal.subscribe(function (this: { dispose: EffectDisposer }, value: T) {
        args.newValue = value;
        if (args.isInitial && this?.dispose) {
            args.effectDisposer = this.dispose.bind(this);
            args.lifecycleNode && addDisposingListener(args.lifecycleNode, args.effectDisposer);
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
        args.effectDisposer = disposer;
        args.lifecycleNode && addDisposingListener(args.lifecycleNode, args.effectDisposer);
    }
    return args.effectDisposer;
}



