import * as signals from "@preact/signals-core";
import { type SignalLike } from "@serenity-is/sleekdom";

export interface Signal<T> extends SignalLike<T> {
    set value(value: T);
}

export interface SignalOptions<T> {
    watched?: (this: Signal<T>) => void;
    unwatched?: (this: Signal<T>) => void;
    name?: string;
}

export interface EffectOptions {
    name?: string;
}

export type ReadonlySignal<T> = SignalLike<T>;

type EffectFn = ((this: {
    dispose: () => void;
}) => void | (() => void)) | (() => void | (() => void));

export const signal = signals.signal as unknown as {
    <T>(value: T, options?: SignalOptions<T>): Signal<T>;
    <T = undefined>(): Signal<T | undefined>;
};

export const computed: (<T>(fn: () => T, options?: SignalOptions<T>) => ReadonlySignal<T>) = signals.computed as any;
export const effect: ((fn: EffectFn, options?: EffectOptions) => () => void) = signals.effect;
export const batch: (<T>(fn: () => T) => T) = signals.batch;
export const untracked: (<T>(fn: () => T) => T) = signals.untracked;
