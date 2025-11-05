import type { Ref, SignalOrValue } from "./basic-types";
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
        tabIndex?: SignalOrValue<number | string | RemoveAttribute>;
        namespaceURI?: string | undefined;
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

    interface TdHTMLAttributes<T> {
        colSpan?: SignalOrValue<number | string | RemoveAttribute>;
        rowSpan?: SignalOrValue<number | string | RemoveAttribute>;
    }
}
