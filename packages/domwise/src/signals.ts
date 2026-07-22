import * as signals from "@preact/signals-core";
import { type Computed, type Signal, type SignalLike } from "../types";

export interface SignalOptions<T> {
    watched?: (this: SignalLike<T>) => void;
    unwatched?: (this: SignalLike<T>) => void;
    name?: string;
}

export interface EffectOptions {
    name?: string;
}

type EffectFn = ((this: {
    dispose: () => void;
}) => void | (() => void)) | (() => void | (() => void));

/**
 * Creates a new writable signal with an optional initial value.
 * Re-exported from `@preact/signals-core` with typed overloads.
 * @typeParam T - The type of the signal's value.
 * @param value - Optional initial value.
 * @param options - Optional signal options (`watched`, `unwatched`, `name`).
 * @returns A writable `Signal<T>`.
 */
export const signal = signals.signal as unknown as {
    <T>(value: T, options?: SignalOptions<T>): Signal<T>;
    <T = undefined>(): Signal<T | undefined>;
};

/**
 * Creates a computed (derived) signal that re-computes when its dependencies change.
 * Re-exported from `@preact/signals-core`.
 * @typeParam T - The type of the computed value.
 * @param fn - A computation function that returns the derived value.
 * @param options - Optional signal options.
 * @returns A read-only `Computed<T>` signal.
 */
export const computed: (<T>(fn: () => T, options?: SignalOptions<T>) => Computed<T>) = signals.computed as any;
/**
 * Creates an effect that runs whenever its signal dependencies change.
 * Re-exported from `@preact/signals-core`.
 * @param fn - The effect function. May optionally return a cleanup callback.
 * @param options - Optional effect options (`name`).
 * @returns A disposer function to stop the effect.
 */
export const effect: ((fn: EffectFn, options?: EffectOptions) => () => void) = signals.effect;
/**
 * Batches multiple signal updates into a single notification.
 * Re-exported from `@preact/signals-core`.
 * @typeParam T - The return type of the batch function.
 * @param fn - A function that performs batched signal updates.
 * @returns The return value of `fn`.
 */
export const batch: (<T>(fn: () => T) => T) = signals.batch;
/**
 * Reads signal values without creating a dependency tracking context.
 * Re-exported from `@preact/signals-core`.
 * @typeParam T - The return type of the function.
 * @param fn - A function that reads signals without tracking them.
 * @returns The return value of `fn`.
 */
export const untracked: (<T>(fn: () => T) => T) = signals.untracked;

/**
 * Creates a writable signal with the given initial value.
 * Convenience wrapper around the `signal()` function.
 * @typeParam T - The type of the signal's value.
 * @param initialValue - The initial value.
 * @returns A `Signal<T>` instance.
 */
export function useSignal<T>(initialValue: T): Signal<T> {
    return signal(initialValue);
}

/**
 * Creates a factory for computed signals that can be manually refreshed as a batch.
 * Returns an object with a `computed` method that creates computed signals tied to an
 * internal updater signal, and an `update` method that triggers a refresh of all created
 * computed signals.
 * @returns An object with `computed` factory and `update` trigger.
 */
export function useUpdatableComputed(): { computed: <T>(fn: () => T) => Computed<T>; update: () => void; } {
    const updater = signal(0);
    
    const factory = <T, >(fn: () => T): Computed<T> => {
        return computed(() => {
            updater.value;
            return fn();
        });
    };
    
    const update = (): void => {
        updater.value++;
    };
    
    return { computed: factory, update };
}