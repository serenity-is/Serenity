import type { JSXElement } from "./jsx-element";
import type { CustomElementsHTML, IntrinsicElementsCombined } from "./intrinsic-elements";

export declare namespace JSX {
    type Element = JSXElement;

    interface ElementAttributesProperty {
        props: unknown;
    }

    interface ElementChildrenAttribute {
        children: unknown;
    }

    interface IntrinsicElements extends IntrinsicElementsCombined, CustomElementsHTML { }
}
