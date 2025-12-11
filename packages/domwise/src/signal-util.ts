import { type Computed, type EffectDisposer, type Signal, type SignalLike } from "../types";
import { addDisposingListener, currentLifecycleRoot, removeDisposingListener } from "./disposing-listener";

/** A type guard that checks if an object is signal-like, meaning it has subscribe and peek methods, and a value property. */
export function isSignalLike<T = any>(obj: any): obj is SignalLike<T> {
    return obj != null && typeof obj === "object" && typeof obj.subscribe === "function" && typeof obj.peek === "function" && 'value' in obj;
}

/** A type guard that checks if an object is a writable signal-like, meaning it passes the isSignalLike check and has a value setter. */
export function isWritableSignal<T>(obj: any): obj is Signal<T> {
    if (!isSignalLike(obj))
        return false;

    // Walk the prototype chain to find the 'value' descriptor
    let descriptor: PropertyDescriptor | undefined;
    let current = obj;
    while (current && !descriptor) {
        if (descriptor = Object.getOwnPropertyDescriptor(current, "value")) {
            if ("writable" in descriptor) {
                return Boolean(descriptor.writable);
            }
            return typeof descriptor.set === "function";
        }
        current = Object.getPrototypeOf(current);
    }

    return false;
}

/** A type guard that checks if an object is a readonly signal-like, meaning it passes the isSignalLike check and has no value setter. */
export function isReadonlySignal<T = any>(obj: any): obj is Computed<T> {
    return isSignalLike(obj) && !isWritableSignal(obj);
}

/** Arguments for the observeSignal function */
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
    effectDisposer: EffectDisposer | null | undefined;
    /**
     * Gets the lifecycle root at the time of subscription if useLifecycleRoot option was true.
     */
    readonly lifecycleRoot: EventTarget | null | undefined;
    /**
     * Gets the lifecycle node to tie the signal's lifecycle to.
     */
    get lifecycleNode(): EventTarget | null | undefined,
    /**
     * Sets the lifecycle node to tie the signal's lifecycle to.
     */
    set lifecycleNode(value: EventTarget | null | undefined);
}

class SignalObserveArgsImpl<T> implements SignalObserveArgs<T> {
    declare readonly signal: SignalLike<T>;
    declare newValue: T | undefined;
    declare prevValue: T | undefined;
    declare isInitial: boolean;
    declare hasChanged: boolean;
    #dispose: EffectDisposer | undefined;
    #node: EventTarget | null | undefined;
    declare lifecycleRoot: EventTarget | null | undefined;

    constructor(signal: SignalLike<T>, lifecycleRoot: EventTarget | null | undefined, lifecycleNode: EventTarget | null | undefined) {
        this.signal = signal;
        this.isInitial = true;
        this.hasChanged = false;
        this.lifecycleRoot = lifecycleRoot;
        this.#node = lifecycleNode;
    }

    get lifecycleNode(): EventTarget | null | undefined {
        return this.#node;
    }

    #delDispose() {
        removeDisposingListener(this.#node, this.effectDisposer);
        removeDisposingListener(this.#node, (this.signal as DerivedSignalLike<T>)?.derivedDisposer);
    }

    #addDispose() {
        addDisposingListener(this.#node, this.effectDisposer);
        addDisposingListener(this.#node, (this.signal as DerivedSignalLike<T>)?.derivedDisposer);
    }

    get effectDisposer(): EffectDisposer | undefined {
        return this.#dispose;
    }

    set effectDisposer(value: EffectDisposer | undefined) {
        this.#delDispose();
        this.#dispose = value;
        this.#addDispose();
    }

    set lifecycleNode(value: EventTarget | undefined) {
        if (value !== this.#node) {
            this.#delDispose();
            this.#node = value;
            this.#addDispose();
        }
    }
}

export type ObserveSignalCallback<T> = (args: SignalObserveArgs<T>) => void;

/**
 * Observes a signal and calls the callback immediately upon subscription and when the signal changes.
 * @param signal Signal to observe
 * @param callback Callback to execute immediately upon subscription and when the signal value changes.
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
}): EffectDisposer | null | undefined {

    const lifecycleRoot = opt?.useLifecycleRoot ? currentLifecycleRoot() : void 0;
    const args = new SignalObserveArgsImpl(signal, lifecycleRoot, opt?.lifecycleNode);
    const disposer = args.signal.subscribe(function (this: { dispose: EffectDisposer }, value: T) {
        args.newValue = value;
        if (args.isInitial && this?.dispose) {
            args.effectDisposer = this.dispose.bind(this);
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
    }
    return args.effectDisposer;
}

export interface DerivedSignalLike<T> extends SignalLike<T> {
    derivedDisposer?: () => void;
}

/** Creates a derived signal from a computation function */
export function derivedSignal<TDerived, TInput = any>(input: SignalLike<TInput>, fn: (value: TInput) => TDerived): DerivedSignalLike<TDerived> {

    if (!isSignalLike(input)) {
        throw new Error("Input must be a SignalLike");
    }

    const callback = () => fn(input.value);

    if (typeof input.constructor === "function" && input.constructor !== {}.constructor) {
        try {
            let derived = new (input.constructor as any)(callback);
            let disposer: EffectDisposer | null | undefined;
            if (isSignalLike(derived)) {
                if (derived.peek() === callback) {
                    disposer = input.subscribe(() => {
                        (derived as any).value = callback();
                    });
                }
                if (disposer) {
                    (derived as DerivedSignalLike<TDerived>).derivedDisposer = function () {
                        disposer!();
                        delete (derived as DerivedSignalLike<TDerived>).derivedDisposer;
                    }
                }
                return derived;
            }
        } catch (error) {
        }
    }

    let primitive: PrimitiveComputed<TDerived> | undefined;
    const disposer = input.subscribe(() => {
        if (!primitive) {
            primitive = new PrimitiveComputed<TDerived>(callback);
            return;
        }
        primitive.update();
    });
    if (disposer) {
        (primitive as DerivedSignalLike<TDerived>).derivedDisposer = function () {
            disposer();
            delete (primitive as any).derivedDisposer;
        }
    }
    return primitive!;
}

export class PrimitiveComputed<T> {
    #subs: Set<(value: T) => void> = new Set();
    #value: T;
    #fn: () => T;

    constructor(fn: () => T) {
        this.#fn = fn;
        this.update(true);
    }

    update(force?: boolean): void {
        const newValue = this.#fn();
        if (newValue !== this.#value || force) {
            this.#value = newValue;
            this.#subs.forEach(sub => sub(newValue));
        }
    }

    subscribe(callback: (value: T) => void): EffectDisposer {
        callback(this.#value);
        this.#subs.add(callback);
        return () => this.#subs.delete(callback);
    }

    peek(): T {
        return this.#value;
    }

    get value(): T {
        return this.#value;
    }
}