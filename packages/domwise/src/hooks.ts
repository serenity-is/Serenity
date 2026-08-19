import type { BasicClassList, ClassNames, JSXElement, PropBinding } from "../types";
import { className } from "./class-name";
import { addDisposingListener, removeDisposingListener } from "./disposing-listener";
import { assignProp } from "./jsx-assign-props";
import { initPropHookSymbol } from "./prop-hook";

/**
 * Creates a `classList`-like manager that can be used as a JSX prop hook for the `class` attribute.
 *
 * The returned {@link BasicClassList} is a callable that also implements
 * `add`/`remove`/`toggle`/`contains` plus `value`/`size`, mirroring the native
 * `DOMTokenList` API. When assigned to `class` (e.g. `<div class={cls} />`),
 * the hook binds to the element's `classList` and keeps it in sync; the
 * binding is cleaned up automatically when the element is disposed.
 * Before binding, an optional `initialValue` is used to seed a detached
 * `classList` so that `add`/`remove` calls prior to attachment are preserved.
 *
 * @param initialValue - Optional initial class value (string, array, dictionary, iterable, or `DOMTokenList`).
 * @returns A `BasicClassList` instance that is both callable and a prop hook.
 * @example
 * ```tsx
 * const cls = useClassList("foo");
 * cls.add("bar");
 * return <div class={cls} />;
 * ```
 */
export function useClassList(initialValue?: ClassNames): BasicClassList {
    let temp: Element | null = document.createElement("div") as Element;
    if (initialValue != null) {
        temp.className = className(initialValue)
    }

    let list: DOMTokenList = temp.classList;

    function ClassList() {
        return list;
    }

    function dispose() {
        list = null as any;
        temp = null as any;
    }

    Object.defineProperties(
        ClassList,
        Object.getOwnPropertyDescriptors({
            [initPropHookSymbol](node: Element, prop: string) {
                if (prop !== "class")
                    throw new Error("useClassList can only be used for 'class' attribute.");
                temp && removeDisposingListener(temp, dispose);
                list && node?.setAttribute("class", list.value);
                addDisposingListener(temp = node, dispose);
                list = node?.classList ?? list;
            },
            get size() {
                return list.length
            },
            get value() {
                return list.value
            },
            add(...tokens: string[]) {
                list.add(...tokens)
            },
            remove(...tokens: string[]) {
                list.remove(...tokens)
            },
            toggle(token: string, force?: boolean) {
                list.toggle(token, force)
            },
            contains(token: string) {
                return list.contains(token)
            },
        })
    )

    return ClassList as unknown as BasicClassList
}


type PropBindingThis<T> = {
    dispose?: (() => void);
    node?: JSXElement | undefined;
    prop?: string | undefined;
    value?: T | null | undefined | false;
}

function propBinding<T>(this: PropBindingThis<T>, value?: T | null | undefined | false) {
    if (arguments.length && value !== this.value) {
        this.node && this.prop && assignProp(this.node as JSXElement, this.prop, value, this.value);
        this.value = value;
    }
    return this.value;
}

function propBindingDispose(this: PropBindingThis<any>) {
    delete this.node;
}

function propBindingInit(this: PropBindingThis<any>, el: Element, key: string) {
    if ((this.node && el && this.node !== el) ||
        (this.prop && key && this.prop !== key)) {
        throw new Error("usePropBinding can only be used with one element and one attribute. Create a new setter for each element / prop.");
    }

    if (this.node)
        removeDisposingListener(this.node, this.dispose);

    this.node = el as JSXElement;

    if (this.node) {
        addDisposingListener(this.node, this.dispose);
        assignProp(this.node, this.prop ??= key, this.value);
    }
}

/**
 * Creates a two-way prop binding hook that synchronizes a value to a single element attribute.
 *
 * The returned {@link PropBinding} is a callable getter/setter that also
 * implements the prop-hook protocol. When the binding is assigned to a JSX
 * attribute (e.g. `<input value={binding} />`), it attaches to that element
 * and calls `assignProp` on every subsequent `binding(newValue)`. The hook
 * may only be bound once — reusing the same binding on a different element
 * or attribute throws.
 *
 * @typeParam T - Type of the bound prop value.
 * @param initialValue - Optional initial value stored prior to element attachment.
 * @returns A `PropBinding<T>` callable that reads the current value when called
 * with no arguments, and writes a new value when called with an argument.
 * @example
 * ```tsx
 * const value = usePropBinding("hello");
 * return <><input value={value} /><button onClick={() => value("world")} /></>;
 * ```
 */
export function usePropBinding<T>(initialValue?: T | null | undefined | false): PropBinding<T> {
    const accessorThis: PropBindingThis<T> = {
        value: initialValue
    }
    accessorThis.dispose = propBindingDispose.bind(accessorThis);
    const hook = propBinding.bind(accessorThis) as PropBinding<T>;
    hook[initPropHookSymbol] = propBindingInit.bind(accessorThis);
    return hook;
}

/**
 * Creates a `Text` node and a setter to update its content.
 *
 * The node's `toString()` is overridden to return `textContent`, so the
 * returned `Text` can be interpolated directly as a JSX child and will
 * render its string value.
 *
 * @param initialValue - Optional initial text content. If omitted the node starts empty.
 * @returns A readonly tuple `[textNode, setText]` where `setText` assigns `textContent`.
 * @example
 * ```tsx
 * const [label, setLabel] = useText("hello");
 * return <><span>{label}</span><button onClick={() => setLabel("world")} /></>;
 * ```
 */
export function useText(initialValue?: string): readonly [text: Text, setText: (value: string) => void] {
    const text = new Text()
    Object.defineProperty(text, "toString", {
        value() {
            return this.textContent
        },
    })
    function setText(value: string) {
        text.textContent = value
    }
    if (initialValue != null) {
        setText(initialValue)
    }
    return [text, setText] as const;
}

export { createRef as useRef } from "./ref";
