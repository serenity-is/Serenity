import type { BasicClassList, ClassNames, JSXElement, PropBinding } from "../types";
import { className } from "./class-name";
import { addDisposingListener, removeDisposingListener } from "./disposing-listener";
import { assignProp } from "./jsx-assign-props";
import { initPropHookSymbol } from "./prop-hook";

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

export function usePropBinding<T>(initialValue?: T | null | undefined | false): PropBinding<T> {
    const accessorThis: PropBindingThis<T> = {
        value: initialValue
    }
    accessorThis.dispose = propBindingDispose.bind(accessorThis);
    const hook = propBinding.bind(accessorThis) as PropBinding<T>;
    hook[initPropHookSymbol] = propBindingInit.bind(accessorThis);
    return hook;
}

export function useText(initialValue?: string) {
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
    return [text, setText] as const
}

export { createRef as useRef } from "./ref";
