export interface SignalLike<T> {
	value: T;
	peek(): T;
	subscribe(fn: (value: T) => void): () => void;
}

export type Signalish<T> = T | SignalLike<T>;
