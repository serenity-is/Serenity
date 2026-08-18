
/**
 * Base interface for JSX prop hooks. A prop hook is a callable object that can
 * be assigned as a value to a JSX attribute to reactively bind to an element.
 * @typeParam TNode - The type of the DOM node the hook binds to.
 */
export interface PropHook<TNode extends Element = Element> {
}

/**
 * A class list manager created by `useClassList`. It wraps a `DOMTokenList`
 * and provides a subset of the native `classList` API (`add`, `remove`,
 * `toggle`, `contains`, `size`, `value`). It can also be used as a JSX prop
 * hook to reactively bind the `class` attribute.
 */
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
/**
 * A value that can be used as a `class` attribute: a string, an array of class
 * names, an iterable, a dictionary of boolean flags, or a `DOMTokenList`.
 */
export type ClassNames = ClassName | Iterable<string> | DOMTokenList;

/**
 * A mutable reference container with a `current` property.
 * @typeParam T - The type of the referenced value.
 */
export type RefObject<T> = { current: T | null };
/**
 * A callback invoked with the referenced instance.
 * @typeParam T - The type of the referenced instance.
 */
export type RefCallback<T> = (instance: T) => void;
/**
 * A reference to a DOM node or component instance: either a `RefObject`, a ref
 * callback, or `null`.
 * @typeParam T - The type of the referenced value.
 */
export type Ref<T> = RefCallback<T> | RefObject<T> | null;

/**
 * A function that disposes an effect or subscription.
 */
export type EffectDisposer = (() => void) | null;

/**
 * A read-only signal-like value that can be subscribed to and peeked.
 * @typeParam T - The type of the signal's value.
 */
export interface SignalLike<T> {
    get value(): T;
    peek(): T;
    subscribe(fn: (value: T) => void): EffectDisposer;
}

/**
 * A writable signal whose `value` can be set.
 * @typeParam T - The type of the signal's value.
 */
export interface Signal<T> extends SignalLike<T> {
    set value(value: T);
}

/**
 * A read-only (computed) signal.
 * @typeParam T - The type of the computed value.
 */
export interface Computed<T> extends SignalLike<T> {
}

/**
 * A value or a signal-like value that can be used interchangeably.
 * @typeParam T - The type of the value.
 */
export type SignalOrValue<T> = T | SignalLike<T>;

/**
 * A two-way prop binding hook created by `usePropBinding`. It can be called
 * with no arguments to read the current value, or with a value to set it.
 * @typeParam T - The type of the bound value.
 * @typeParam TElement - The type of the element the binding is attached to.
 */
export interface PropBinding<T = any, TElement extends Element = Element> extends PropHook<TElement> {
    (): T;
    (value: T): T;
}

/**
 * A value that can be assigned to a JSX attribute: a plain value, a prop hook,
 * or a signal-like value.
 * @typeParam T - The type of the attribute value.
 */
export type PropValue<T> = T | PropHook<Element> | SignalLike<T>;