export interface SignalLike<T> {
	value: T;
	peek(): T;
	subscribe(fn: (value: T) => void): ((() => void) | null | undefined | void);
}

export type SignalOrValue<T> = T | SignalLike<T>;
