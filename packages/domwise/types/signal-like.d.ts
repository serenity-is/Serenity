export type EffectDisposer = (() => void) | null;

export interface SignalLike<T> {
	get value(): T;
	peek(): T;
	subscribe(fn: (value: T) => void): EffectDisposer;
}

export interface Signal<T> extends SignalLike<T> {
    set value(value: T);
}

export interface Computed<T> extends SignalLike<T> {
}

export type SignalOrValue<T> = T | SignalLike<T>;