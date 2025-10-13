import type { CustomElementsHTML, IntrinsicElementsCombined } from "./intrinsic-elements";
import type { JSXElement } from "./jsx-element";
import type { Ref } from "./ref-types";

export declare namespace JSX {
    type Element = JSXElement;

    interface ElementAttributesProperty {
        props: unknown;
    }

    interface ElementChildrenAttribute {
        children: {};
    }

    interface IntrinsicAttributes {

    }

    interface IntrinsicClassAttributes<T> {
        ref?: Ref<T>;
    }


    interface IntrinsicElements extends IntrinsicElementsCombined, CustomElementsHTML { }
}
