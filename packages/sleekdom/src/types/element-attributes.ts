import type { HTMLAttributes } from "./html-attributes";
import type { SVGAttributes } from "./svg-attributes";

type SpecialKeys<T extends Element> = T extends HTMLLabelElement | HTMLOutputElement
    ? "for" | "class" | "is"
    : "class" | "is" | "spellCheck";

/** Figure out which of the attributes exist for a specific element */
export type ElementAttributes<TElement extends Element, TAttributes extends HTMLAttributes<TElement> | SVGAttributes<TElement>> = {
    [TKey in (keyof TElement & keyof TAttributes) | SpecialKeys<TElement>]?: TAttributes[TKey];
};

type PropertiesOfFix<TFixes, TName> = TName extends keyof TFixes ? TFixes[TName] : unknown;

/**
 * Some tags properties can't be inferred correctly. To fix these properties, this manual override is defined.
 * Since it's an interface, users can even override them from outside.
 */
interface HTMLTagFixes {
    meta: {
        charset?: string;
        property?: string;
    };
}

/** Figure out which of the HTML attributes exist for a specific element */
export type HTMLElementAttributes<TName extends keyof HTMLElementTagNameMap> = ElementAttributes<
    HTMLElementTagNameMap[TName],
    HTMLAttributes<HTMLElementTagNameMap[TName]>
> &
    PropertiesOfFix<HTMLTagFixes, TName>;

/** Figure out which of the SVG attributes exist for a specific element */
export type SVGElementAttributes<TName extends keyof SVGElementTagNameMap> = ElementAttributes<
    SVGElementTagNameMap[TName],
    SVGAttributes<SVGElementTagNameMap[TName]>
>;

export type SVGOnlyElementKeys = Exclude<keyof SVGElementTagNameMap, SVGAndHTMLElementKeys>;

export type SVGAndHTMLElementKeys = keyof SVGElementTagNameMap & keyof HTMLElementTagNameMap;