import type { ClassNames } from "./classname";
import type { ShadowRootContainer } from "./components";
import type { JSXElement } from "./jsx-element";
import type { Ref } from "./ref-types";
import type { SignalOrValue } from "./signal-like";

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
        className?: ElementAttributes<T>["class"];
        tabIndex?: SignalOrValue<number | string | RemoveAttribute>;
        onClickCapture?: EventHandlerUnion<T, MouseEvent> | undefined;
        onDblClickCapture?: EventHandlerUnion<T, MouseEvent> | undefined;
        onDoubleClick?: EventHandlerUnion<T, MouseEvent> | undefined;
        onDoubleClickCapture?: EventHandlerUnion<T, MouseEvent> | undefined;
    }

    interface HTMLAttributes<T> {
        contentEditable?: SignalOrValue<EnumeratedPseudoBoolean | EnumeratedAcceptsEmpty | "plaintext-only" | "inherit" | RemoveAttribute>;
        dataset?: { [key: string]: string } | undefined
        spellCheck?: SignalOrValue<EnumeratedPseudoBoolean | EnumeratedAcceptsEmpty | RemoveAttribute>;
    }

    interface SVGAttributes<T> {
        tabIndex?: SignalOrValue<number | string | RemoveAttribute>;
    }

    interface AnchorHTMLAttributes<T> {
        /** @deprecated use referrerpolicy */
        referrerPolicy?: SignalOrValue<HTMLReferrerPolicy | RemoveAttribute>;
    }

    interface ButtonHTMLAttributes<T> {
        autoFocus?: SignalOrValue<boolean | RemoveAttribute>;
        formNoValidate?: SignalOrValue<boolean | RemoveAttribute>;
    }

    interface InputHTMLAttributes<T> {
        maxLength?: SignalOrValue<string | number | RemoveAttribute>;
        minLength?: SignalOrValue<string | number | RemoveAttribute>;
        readOnly?: SignalOrValue<boolean | RemoveAttribute>;
    }

    interface LabelHTMLAttributes<T> {
        htmlFor?: SignalOrValue<string | RemoveAttribute>;
    }

    interface TdHTMLAttributes<T>{
        colSpan?: SignalOrValue<number | string | RemoveAttribute>;
        rowSpan?: SignalOrValue<number | string | RemoveAttribute>;
    }
}
