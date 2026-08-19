import * as signals from "@preact/signals-core";
import { type Computed, type Signal, type SignalLike } from "../types";

/**
 * Options for creating a signal via {@link signal} / {@link computed}.
 * Re-exported from `@preact/signals-core`.
 * @typeParam T - Type of the signal's value.
 */
export interface SignalOptions<T> {
    /** Called when the signal gains its first subscriber. */
    watched?: (this: SignalLike<T>) => void;
    /** Called when the signal loses its last subscriber. */
    unwatched?: (this: SignalLike<T>) => void;
    /** Optional debug name for the signal. */
    name?: string;
}

/**
 * Options for creating an effect via {@link effect}.
 */
export interface EffectOptions {
    /** Optional debug name for the effect. */
    name?: string;
}

type EffectFn = ((this: {
    dispose: () => void;
}) => void | (() => void)) | (() => void | (() => void));

/**
 * Creates a new writable signal with an optional initial value.
 * Re-exported from `@preact/signals-core` with typed overloads.
 * @typeParam T - Type of the signal's value.
 * @param value - Optional initial value for the signal. When omitted the signal starts as `undefined`.
 * @param options - Optional signal options (`watched`, `unwatched`, `name`).
 * @returns A writable `Signal<T>`.
 * @example
 * ```ts
 * const count = signal(0);
 * count.value++;
 * ```
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
 * Convenience wrapper around {@link signal}.
 * @typeParam T - Type of the signal's value.
 * @param initialValue - Initial value for the signal.
 * @returns A `Signal<T>` instance.
 * @example
 * ```ts
 * const name = useSignal("Alice");
 * name.value = "Bob";
 * ```
 */
export function useSignal<T>(initialValue: T): Signal<T> {
    return signal(initialValue);
}

/**
 * Creates a factory for computed signals that can be manually invalidated in batch.
 *
 * Computed signals produced by the returned `computed` wrapper depend on an
 * internal `updater` signal; calling `update()` bumps that signal so every
 * derived computed re-evaluates on its next read, without wiring each one to
 * a separate source.
 *
 * @returns An object with:
 *  - `computed` — factory that wraps a computation so it tracks the shared updater.
 *  - `update` — bumps the updater, invalidating all computeds created from this factory.
 * @example
 * ```ts
 * const { computed: uc, update } = useUpdatableComputed();
 * const derived = uc(() => expensiveRead());
 * // later: after external state changes
 * update();
 * ```
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