
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
 * hook to reactively bind the `class` attribute; see {@link useClassList}.
 */
export interface BasicClassList extends PropHook<Element> {
    /** Returns the underlying `DOMTokenList` (detached before binding, live after). */
    (): DOMTokenList;
    /** Number of tokens in the list. */
    readonly size: number
    /** Space-separated string of all tokens (mirrors `DOMTokenList.value`). */
    readonly value: string
    /**
     * Adds one or more tokens to the list. Duplicate tokens are ignored.
     * @param tokens - Class names to add.
     */
    add(...tokens: string[]): void
    /**
     * Removes one or more tokens from the list.
     * @param tokens - Class names to remove.
     */
    remove(...tokens: string[]): void
    /**
     * Toggles a token, optionally forcing the presence or absence.
     * @param token - Class name to toggle.
     * @param force - When provided, forces add (`true`) or remove (`false`).
     */
    toggle(token: string, force?: boolean): void
    /**
     * Checks whether the list contains the given token.
     * @param token - Class name to test.
     * @returns `true` if the token is present.
     */
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
 * Compatible with `@preact/signals-core` and any duck-typed signal that
 * exposes `value`, `peek`, and `subscribe`.
 * @typeParam T - The type of the signal's value.
 */
export interface SignalLike<T> {
    /** Current value; reading may track a dependency when inside an effect/computed. */
    get value(): T;
    /**
     * Returns the current value without creating a dependency.
     * @returns The current value.
     */
    peek(): T;
    /**
     * Subscribes to value changes.
     * @param fn - Callback invoked with each new value (and typically immediately with the current value).
     * @returns A disposer that unsubscribes, or `null` if unsubscription is not supported.
     */
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
 * A two-way prop binding hook created by `usePropBinding`. It acts as a
 * getter when called with no arguments and a setter when called with a value;
 * when attached as a prop hook it synchronizes that value to the bound element
 * attribute.
 * @typeParam T - The type of the bound value.
 * @typeParam TElement - The type of the element the binding is attached to.
 */
export interface PropBinding<T = any, TElement extends Element = Element> extends PropHook<TElement> {
    /**
     * Gets the current bound value.
     * @returns The current value.
     */
    (): T;
    /**
     * Sets the bound value and synchronizes it to the attached element (if any).
     * @param value - New value to store and propagate to the DOM.
     * @returns The value that was set.
     */
    (value: T): T;
}

/**
 * A value that can be assigned to a JSX attribute: a plain value, a prop hook,
 * or a signal-like value.
 * @typeParam T - The type of the attribute value.
 */
export type PropValue<T> = T | PropHook<Element> | SignalLike<T>;