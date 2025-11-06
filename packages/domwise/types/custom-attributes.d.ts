import type { Ref, PropValue } from "./basic-types";
import type { ComponentChildren } from "./jsx-namespace";

export interface CustomDomAttributes<T> {
    children?: ComponentChildren;
    dangerouslySetInnerHTML?: { __html: string };
    ref?: Ref<T>;

    /** compat from jsx-dom/react */
    on?: Record<string, Function>;
    onCapture?: Record<string, Function>;
}

declare module "./jsx-elements" {
    interface ElementAttributes<T> {
        className?: ElementAttributes<T>["class"];
        tabIndex?: PropValue<number | string | RemoveAttribute>;
        namespaceURI?: string | undefined;
        onClickCapture?: EventHandlerUnion<T, MouseEvent> | undefined;
        onDblClickCapture?: EventHandlerUnion<T, MouseEvent> | undefined;
        onDoubleClick?: EventHandlerUnion<T, MouseEvent> | undefined;
        onDoubleClickCapture?: EventHandlerUnion<T, MouseEvent> | undefined;
    }

    interface HTMLAttributes<T> {
        contentEditable?: PropValue<EnumeratedPseudoBoolean | EnumeratedAcceptsEmpty | "plaintext-only" | "inherit" | RemoveAttribute>;
        dataset?: { [key: string]: string } | undefined
        spellCheck?: PropValue<EnumeratedPseudoBoolean | EnumeratedAcceptsEmpty | RemoveAttribute>;
    }

    interface SVGAttributes<T> {
        tabIndex?: PropValue<number | string | RemoveAttribute>;
    }

    interface AnchorHTMLAttributes<T> {
        /** @deprecated use referrerpolicy */
        referrerPolicy?: PropValue<HTMLReferrerPolicy | RemoveAttribute>;
    }

    interface ButtonHTMLAttributes<T> {
        autoFocus?: PropValue<boolean | RemoveAttribute>;
        formNoValidate?: PropValue<boolean | RemoveAttribute>;
    }

    interface FormHTMLAttributes<T> extends HTMLAttributes<T> {
        autoComplete?: PropValue<"on" | "off" | RemoveAttribute>;
    }

    interface InputHTMLAttributes<T> {
        autoComplete?: PropValue<HTMLAutocomplete | RemoveAttribute>;
        maxLength?: PropValue<string | number | RemoveAttribute>;
        minLength?: PropValue<string | number | RemoveAttribute>;
        readOnly?: PropValue<boolean | RemoveAttribute>;
    }

    interface LabelHTMLAttributes<T> {
        htmlFor?: PropValue<string | RemoveAttribute>;
    }

    interface TdHTMLAttributes<T> {
        colSpan?: PropValue<number | string | RemoveAttribute>;
        rowSpan?: PropValue<number | string | RemoveAttribute>;
    }
}
