import * as signals from "@preact/signals-core";
import { type SignalLike } from "@serenity-is/sleekdom";

interface SignalOptions<T> {
    watched?: (this: SignalLike<T>) => void;
    unwatched?: (this: SignalLike<T>) => void;
    name?: string;
}

interface EffectOptions {
    name?: string;
}

type ReadonlySignalLike<T> = Readonly<SignalLike<T>>;

type EffectFn = ((this: {
    dispose: () => void;
}) => void | (() => void)) | (() => void | (() => void));

export const signal = signals.signal as unknown as {
    <T>(value: T, options?: SignalOptions<T>): SignalLike<T>;
    <T = undefined>(): SignalLike<T | undefined>;
};

export const computed: (<T>(fn: () => T, options?: SignalOptions<T>) => ReadonlySignalLike<T>) = signals.computed as any;
export const effect: ((fn: EffectFn, options?: EffectOptions) => () => void) = signals.effect;
export const batch: (<T>(fn: () => T) => T) = signals.batch;
export const untracked: (<T>(fn: () => T) => T) = signals.untracked;
