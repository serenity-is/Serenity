import type { Ref } from "./basic-types";
import type { ShadowRootContainer } from "./components";
import type { HTMLElementTags, MathMLElementTags, SVGElementTags } from "./jsx-elements";

/**
 * This technically should include `DocumentFragment` as well, but a lot of web APIs expect an `Element`.
 */
export type JSXElement = HTMLElement |
    (JSX.ConfigureElement["svg"] extends false ? never : SVGElement) |
    (JSX.ConfigureElement["mathml"] extends false ? never : MathMLElement);

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

export namespace JSX {

    // eslint-disable-next-line @typescript-eslint/no-empty-object-type
    export interface CustomElementsHTML { }

    export interface ConfigureElement {
        svg: boolean;
        mathml: boolean;
    }

    type Element = JSXElement;

    interface ElementAttributesProperty {
        props: unknown;
    }

    interface ElementChildrenAttribute {
        children: {};
    }

    interface IntrinsicClassAttributes<T> {
        ref?: Ref<T>;
    }

    type IntrinsicElementsCombined = HTMLElementTags &
        (ConfigureElement["svg"] extends false ? void : SVGElementTags) &
        (ConfigureElement["mathml"] extends false ? void : MathMLElementTags);

    // eslint-disable-next-line @typescript-eslint/no-empty-object-type
    export interface CustomElementsHTML {}

    interface IntrinsicElements extends IntrinsicElementsCombined, CustomElementsHTML { }
}
