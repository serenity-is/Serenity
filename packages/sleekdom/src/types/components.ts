import type { JSXElement } from "./jsx-element";
import type { Ref } from "./ref-types";
import type { Signalish } from "./signal-like";
import type { StyleAttributes } from "./style-attributes";

export type ShadowRootContainer = {
    ref: Ref<ShadowRoot>;
    attr: {
        clonable?: boolean;
        customElementRegistry?: CustomElementRegistry;
        delegatesFocus?: boolean;
        mode: ShadowRootMode;
        serializable?: boolean;
        slotAssignment?: SlotAssignmentMode;
    };
    children: ComponentChildren;
}

type ComponentChild =
    | string
    | number
    | Iterable<ComponentChild>
    | Array<ComponentChild>
    | { value: ComponentChild, peek: () => ComponentChild, subscribe: (cb: (newValue: ComponentChild) => void) => void }
    | JSXElement
    | NodeList
    | ChildNode
    | HTMLCollection
    | ShadowRootContainer
    | DocumentFragment
    | Text
    | Comment
    | boolean
    | null
    | undefined

export type ComponentChildren = ComponentChild[] | ComponentChild;

export interface ComponentClass<P = {}, T extends Node = JSXElement> {
    new(props: P): ComponentClass<P, T>
    render(): JSXElement | null
    defaultProps?: Partial<P> | undefined
    readonly props?: P & { children?: ComponentChildren }
    displayName?: string | undefined
}

export interface BaseProps {
    children?: ComponentChildren;
}

export type FunctionComponent<P = BaseProps, T extends Node = JSXElement> = (props: P & { children?: ComponentChildren }) => T | null;
export type ComponentType<P = {}, T extends Node = JSXElement> = ComponentClass<P, T> | FunctionComponent<P, T>
export type ComponentAttributes = {
    [s: string]: string | number | boolean | undefined | null | StyleAttributes | EventListenerOrEventListenerObject;
};
