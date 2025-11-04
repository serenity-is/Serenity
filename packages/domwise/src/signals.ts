import * as signals from "@preact/signals-core";
import { type Computed, type Signal, type SignalLike } from "../types/signal-like";

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

export const signal = signals.signal as unknown as {
    <T>(value: T, options?: SignalOptions<T>): Signal<T>;
    <T = undefined>(): Signal<T | undefined>;
};

export const computed: (<T>(fn: () => T, options?: SignalOptions<T>) => Computed<T>) = signals.computed as any;
export const effect: ((fn: EffectFn, options?: EffectOptions) => () => void) = signals.effect;
export const batch: (<T>(fn: () => T) => T) = signals.batch;
export const untracked: (<T>(fn: () => T) => T) = signals.untracked;