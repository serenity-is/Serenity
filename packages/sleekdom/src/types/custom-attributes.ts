import type { ClassNames } from "./classname";
import type { ShadowRootContainer } from "./components";
import type { JSXElement } from "./jsx-element";
import type { Ref } from "./ref-types";
import type { Signalish } from "./signal-like";

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

export interface CustomDomAttributes<T> {
    children?: ComponentChildren;
    dangerouslySetInnerHTML?: { __html: string };
    ref?: Ref<T>;

    /** @deprecated This is simply ignored as it only applies to v-dom  */
    key?: string | number;

    /** compat from jsx-dom/react */
    on?: Record<string, Function>;
    onCapture?: Record<string, Function>;

    /**
     * This is essentially a reverse "is" attribute.
     * If you specify it, the generated tag will be tsxTag and it will receive an "is" attribute with the tag you specified in your JSX.
     * This is needed because we can't make the is-property associate with the correct component props.
     */
    tsxTag?: keyof HTMLElementTagNameMap | keyof SVGElementTagNameMap;
}

declare module "./dom-expressions-jsx" {
    interface ElementAttributes<T> {
        className?: Signalish<string | ClassNames | RemoveAttribute>;
        tabIndex?: Signalish<number | string | RemoveAttribute>;
        onClickCapture?: EventHandlerUnion<T, MouseEvent> | undefined;
        onDblClickCapture?: EventHandlerUnion<T, MouseEvent> | undefined;
        onDoubleClick?: EventHandlerUnion<T, MouseEvent> | undefined;
        onDoubleClickCapture?: EventHandlerUnion<T, MouseEvent> | undefined;
    }

    interface HTMLAttributes<T> {
        /** @deprecated use contenteditable */
        contentEditable?: Signalish<EnumeratedPseudoBoolean | EnumeratedAcceptsEmpty | "plaintext-only" | "inherit" | RemoveAttribute>;
        /** @deprecated use spellcheck */
        spellCheck?: Signalish<EnumeratedPseudoBoolean | EnumeratedAcceptsEmpty | RemoveAttribute>;
        dataset?: { [key: string]: string } | undefined
    }

    interface SVGAttributes<T> {
        /** @deprecated use tabindex */
        tabIndex?: Signalish<number | string | RemoveAttribute>;
    }

    interface AnchorHTMLAttributes<T> {
        /** @deprecated use referrerpolicy */
        referrerPolicy?: Signalish<HTMLReferrerPolicy | RemoveAttribute>;
    }

    interface ButtonHTMLAttributes<T> {
        /** @deprecated use autofocus */
        autoFocus?: Signalish<boolean | RemoveAttribute>;
        /** @deprecated use formnovalidate */
        formNoValidate?: Signalish<boolean | RemoveAttribute>;
    }

    interface InputHTMLAttributes<T> {
        /** @deprecated use maxlength */
        maxLength?: Signalish<number | RemoveAttribute>;
        /** @deprecated use minlength */
        minLength?: Signalish<number | RemoveAttribute>;
        /** @deprecated use readonly */
        readOnly?: Signalish<boolean | RemoveAttribute>;
    }

    interface LabelHTMLAttributes<T> {
        /** @deprecated use for */
        htmlFor?: Signalish<string | RemoveAttribute>;
    }

    interface TdHTMLAttributes<T>{
        /** @deprecated use colspan */
        colSpan?: Signalish<number | string | RemoveAttribute>;
        /** @deprecated use rowspan */
        rowSpan?: Signalish<number | string | RemoveAttribute>;
    }
}
