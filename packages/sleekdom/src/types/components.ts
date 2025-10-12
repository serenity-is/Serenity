import type { JSXElement } from "./jsx-element";
import type { StyleAttributes } from "./style-attributes";

export type ShadowRootContainer = {
    ref: Ref<ShadowRoot> | ((value: ShadowRoot) => void);
    attr: {
        mode: ShadowRootMode;
    };
    children: any;
}

type ComponentChild =
    | string
    | number
    | Iterable<ComponentChild>
    | Array<ComponentChild>
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

export type RefObject<T> = { current: T | null };
export type RefCallback<T> = (instance: T) => void

export type Ref<T> = RefCallback<T> | RefObject<T> | null

