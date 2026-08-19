import { type Computed, type EffectDisposer, type Signal, type SignalLike } from "../types";
import { addDisposingListener, currentLifecycleRoot, removeDisposingListener } from "./disposing-listener";

/**
 * A type guard that checks if an object is signal-like, meaning it has `subscribe` and `peek` methods,
 * and a `value` property.
 * @param obj - The object to check.
 * @returns `true` if the object is signal-like.
 */
export function isSignalLike<T = any>(obj: any): obj is SignalLike<T> {
    return obj != null && typeof obj === "object" && typeof obj.subscribe === "function" && typeof obj.peek === "function" && 'value' in obj;
}

/**
 * A type guard that checks if an object is a writable signal, meaning it passes the `isSignalLike` check
 * and the `value` property has a setter or is writable.
 * @param obj - The object to check.
 * @returns `true` if the object is a writable signal.
 */
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

/**
 * A type guard that checks if an object is a readonly (computed) signal, meaning it passes the `isSignalLike` check
 * but the `value` property is not writable.
 * @param obj - The object to check.
 * @returns `true` if the object is a readonly signal.
 */
export function isReadonlySignal<T = any>(obj: any): obj is Computed<T> {
    return isSignalLike(obj) && !isWritableSignal(obj);
}

/**
 * Arguments passed to the {@link observeSignal} callback on each invocation.
 * @typeParam T - Type of the observed signal's value.
 */
export type SignalObserveArgs<T> = {
    /** `true` on the initial synchronous invocation right after subscription; `false` thereafter. */
    isInitial: boolean
    /** Value from the previous callback invocation. `undefined` on the initial call. */
    prevValue: T | undefined,
    /** Current value of the signal for this invocation. `undefined` if unavailable. */
    newValue: T | undefined,
    /** `true` when `newValue !== prevValue`; always `false` on the initial call. */
    hasChanged: boolean,
    /** The observed signal instance. */
    readonly signal: SignalLike<T>,
    /**
     * Disposer for the underlying subscription. Only non-null when the signal
     * library supports unsubscription; assign `null`/`undefined` to clear it.
     * Reassigning also updates the lifecycle-bound disposing listeners.
     */
    effectDisposer: EffectDisposer | null | undefined;
    /**
     * Lifecycle root captured at subscription time when `useLifecycleRoot` was
     * `true`; otherwise `undefined`. See {@link currentLifecycleRoot}.
     */
    readonly lifecycleRoot: EventTarget | null | undefined;
    /**
     * Lifecycle node that owns the subscription — the disposer is registered
     * as a disposing listener on this node. Getter returns the current node.
     */
    get lifecycleNode(): EventTarget | null | undefined,
    /**
     * Sets the lifecycle node that owns the subscription. Changing it moves
     * the disposing listener registration from the old node to the new one.
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

/**
 * Callback invoked by {@link observeSignal} on subscription and on each signal change.
 * @typeParam T - Type of the observed signal's value.
 * @param args - Mutable {@link SignalObserveArgs} describing the change.
 */
export type ObserveSignalCallback<T> = (args: SignalObserveArgs<T>) => void;

/**
 * Subscribes to a signal and invokes `callback` immediately and on every subsequent change.
 *
 * On subscription a {@link SignalObserveArgs} object is created and `callback` is
 * invoked synchronously with `isInitial: true`. Future notifications update
 * `newValue`/`prevValue`/`hasChanged` and invoke `callback` again. The
 * returned disposer (when non-null) can be used to unsubscribe; it is also
 * automatically registered as a disposing listener on `lifecycleNode` /
 * `lifecycleRoot` so it is cleaned up when the owning DOM node is disposed.
 *
 * @typeParam T - Type of the signal's value.
 * @param signal - Signal-like object to observe (must have `subscribe`/`peek`/`value`).
 * @param callback - Function called initially and on each change.
 * @param opt - Optional lifecycle wiring.
 * @param opt.useLifecycleRoot - When `true`, captures {@link currentLifecycleRoot} at call time as the lifecycle root.
 * @param opt.lifecycleNode - Explicit node whose `disposing` event will dispose the subscription.
 * @returns A disposer function for the subscription, or `null`/`undefined` if the signal does not expose one.
 */
export function observeSignal<T>(signal: SignalLike<T>, callback: ObserveSignalCallback<T>, opt?: {
    /**
     * When `true`, the current lifecycle root (see {@link currentLifecycleRoot})
     * at subscription time is recorded as {@link SignalObserveArgs.lifecycleRoot}.
     */
    useLifecycleRoot?: boolean,
    /**
     * Optional DOM node whose `disposing` event will automatically dispose the
     * subscription via {@link addDisposingListener}.
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

/**
 * A derived/computed signal that also exposes a `derivedDisposer` to tear down
 * the subscription to its source signal.
 * @typeParam T - Type of the derived value.
 */
export interface DerivedSignalLike<T> extends SignalLike<T> {
    /** Optional disposer that unsubscribes the derived signal from its source. */
    derivedDisposer?: () => void;
}

/**
 * Creates a derived (computed) signal from a source signal and a transform.
 *
 * When the source signal changes, the derived value is re-computed via `fn`.
 * If the source signal's constructor appears to be a computed-capable type,
 * a new instance of that constructor wrapping `() => fn(input.value)` is
 * attempted; otherwise a lightweight {@link PrimitiveComputed} fallback is
 * used. The returned signal exposes a `derivedDisposer` that unsubscribes
 * from the source.
 *
 * @typeParam TDerived - Type of the derived/computed value.
 * @typeParam TInput - Type of the source signal's value.
 * @param input - Source signal to derive from. Must be signal-like.
 * @param fn - Transform applied to the source value to produce the derived value.
 * @returns A `DerivedSignalLike<TDerived>` whose `value` tracks `fn(input.value)`.
 * @throws {Error} When `input` is not signal-like.
 */
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
    if (!primitive) {
        primitive = new PrimitiveComputed<TDerived>(callback);
    }
    if (disposer) {
        (primitive as DerivedSignalLike<TDerived>).derivedDisposer = function () {
            disposer();
            delete (primitive as any).derivedDisposer;
        }
    }
    return primitive;
}

/**
 * Minimal computed-like signal used as a fallback when the source signal's
 * constructor cannot produce a derived instance. Re-computes `fn()` on
 * `update()` and notifies subscribers.
 * @typeParam T - Type of the computed value.
 */
export class PrimitiveComputed<T> {
    #subs: Set<(value: T) => void> = new Set();
    #value: T;
    #fn: () => T;

    /**
     * Creates the primitive computed.
     * @param fn - Computation that produces the derived value. Invoked immediately to seed `value`.
     */
    constructor(fn: () => T) {
        this.#fn = fn;
        this.update(true);
    }

    /**
     * Re-evaluates `fn()` and notifies subscribers when the result has changed.
     * @param force - When `true`, notifies subscribers even when the value is referentially equal.
     */
    update(force?: boolean): void {
        const newValue = this.#fn();
        if (newValue !== this.#value || force) {
            this.#value = newValue;
            this.#subs.forEach(sub => sub(newValue));
        }
    }

    /**
     * Subscribes to value changes. The callback is invoked immediately with the current value.
     * @param callback - Function called with each new value.
     * @returns A disposer that removes the subscription.
     */
    subscribe(callback: (value: T) => void): EffectDisposer {
        callback(this.#value);
        this.#subs.add(callback);
        return () => this.#subs.delete(callback);
    }

    /**
     * Returns the current value without creating a tracking dependency.
     * @returns The current computed value.
     */
    peek(): T {
        return this.#value;
    }

    /** Current computed value. */
    get value(): T {
        return this.#value;
    }
}