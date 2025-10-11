import type { HTMLAttributes } from "./html-attributes";
import type { SVGAttributes } from "./svg-attributes";

type SpecialKeys<T extends Element> = T extends HTMLLabelElement | HTMLOutputElement
    ? "for" | "class" | "is"
    : "class" | "is";

/** Figure out which of the attributes exist for a specific element */
export type ElementAttributes<TElement extends Element, TAttributes extends HTMLAttributes | SVGAttributes> = {
    [TKey in (keyof TElement & keyof TAttributes) | SpecialKeys<TElement>]?: TAttributes[TKey];
};

export type PropertiesOfFix<TFixes, TName> = TName extends keyof TFixes ? TFixes[TName] : unknown;

/**
 * Some tags properties can't be inferred correctly.
 * To fix these properties, this manual override is defined.
 * Since it's an interface, users can even override them from outside.
 */
export interface HTMLTagFixes {
    meta: {
        charset?: string;
        property?: string;
    };
}

/** Figure out which of the HTML attributes exist for a specific element */
export type HTMLElementAttributes<TName extends keyof HTMLElementTagNameMap> = ElementAttributes<
    HTMLElementTagNameMap[TName],
    HTMLAttributes
> &
    PropertiesOfFix<HTMLTagFixes, TName>;

/** Figure out which of the SVG attributes exist for a specific element */
export type SVGElementAttributes<TName extends keyof SVGElementTagNameMap> = ElementAttributes<
    SVGElementTagNameMap[TName],
    SVGAttributes
>;

export type SVGOnlyElementKeys = Exclude<keyof SVGElementTagNameMap, SVGAndHTMLElementKeys>;
export type SVGAndHTMLElementKeys = keyof SVGElementTagNameMap & keyof HTMLElementTagNameMap;