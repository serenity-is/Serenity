import type { Ref } from "./basic-types";
import type { ShadowRootContainer } from "./components";
import type { HTMLElementTags, MathMLElementTags, SVGElementTags } from "./jsx-elements";

/**
 * The DOM node type returned by JSX expressions.
 *
 * Union of `HTMLElement` plus `SVGElement`/`MathMLElement` when those
 * namespaces are enabled in {@link JSX.ConfigureElement}. Technically this
 * could also include `DocumentFragment`, but many DOM APIs expect `Element`,
 * so fragments are typed separately where needed.
 */
export type JSXElement = HTMLElement |
    (JSX.ConfigureElement["svg"] extends false ? never : SVGElement) |
    (JSX.ConfigureElement["mathml"] extends false ? never : MathMLElement);

/**
 * A single child that can be rendered inside a JSX element.
 *
 * Includes primitives (`string`/`number`), DOM nodes, iterables/arrays of
 * children, signal-like wrappers, shadow root containers, and the ignorable
 * `boolean`/`null`/`undefined` values (filtered similarly to React).
 */
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

/**
 * The children of a JSX element: a single child or an array of children.
 */
export type ComponentChildren = ComponentChild[] | ComponentChild;

export namespace JSX {

    // eslint-disable-next-line @typescript-eslint/no-empty-object-type
    /** Augment to declare custom HTML element tag/type mappings. */
    export interface CustomElementsHTML { }

    /** Toggles for optional JSX element namespaces. Set to `false` to exclude SVG/MathML from `JSXElement` / `IntrinsicElements`. */
    export interface ConfigureElement {
        /** When `false`, SVG elements are excluded from the JSX element union. */
        svg: boolean;
        /** When `false`, MathML elements are excluded from the JSX element union. */
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
