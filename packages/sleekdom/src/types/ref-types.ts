
export type RefObject<T> = { current: T | null };
export type RefCallback<T> = (instance: T) => void;
export type Ref<T> = RefCallback<T> | RefObject<T> | null;