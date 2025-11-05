
export interface PropHook<TNode extends Element = Element> {
}

export interface BasicClassList extends PropHook<Element> {
    (): DOMTokenList;
    readonly size: number
    readonly value: string
    add(...tokens: string[]): void
    remove(...tokens: string[]): void
    toggle(token: string, force?: boolean): void
    contains(token: string): boolean
}

type ClassName = string | { [key: string]: boolean } | false | null | undefined | ClassName[]
export type ClassNames = ClassName | Iterable<string> | DOMTokenList;

export type RefObject<T> = { current: T | null };
export type RefCallback<T> = (instance: T) => void;
export type Ref<T> = RefCallback<T> | RefObject<T> | null;

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

export interface PropBinding<T = any, TElement extends Element = Element> extends PropHook<TElement> {
    (): T;
    (value: T): T;
}

export type PropValue<T> = T | PropHook<Element> | SignalLike<T>;