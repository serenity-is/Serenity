import type { HTMLElementAttributes, SVGElementAttributes, SVGOnlyElementKeys } from "./element-attributes";
import type { EventAttributes } from "./event-attributes";
import type { IfTsxDomTypeConfig, JSXElement, RefType } from "./core-types";
import type { StyleAttributes } from "./style-attributes";


export type ComponentChild = ComponentChild[] | JSXElement | string | number | boolean | undefined | null;
export type ComponentChildren = ComponentChild | ComponentChild[];
export interface BaseProps {
    children?: ComponentChildren;
}
export type FC<T = BaseProps> = (props: T) => JSXElement;
export type ComponentAttributes = {
    [s: string]: string | number | boolean | undefined | null | StyleAttributes | EventListenerOrEventListenerObject;
};

// eslint-disable-next-line @typescript-eslint/no-empty-object-type
export interface CustomElementsHTML {}

export interface HTMLComponentProps<T extends Element> extends BaseProps {
    dangerouslySetInnerHTML?: string;
    /**
     * This is essentially a reverse "is" attribute.
     * If you specify it, the generated tag will be tsxTag and it will receive an "is" attribute with the tag you specified in your JSX.
     * This is needed because we can't make the is-property associate with the correct component props.
     */
    tsxTag?: keyof HTMLElementTagNameMap | keyof SVGElementTagNameMap;
    ref?: RefType<T>;
}

export type IntrinsicElementsHTML = {
    [TKey in keyof HTMLElementTagNameMap]?: HTMLElementAttributes<TKey> &
        HTMLComponentProps<HTMLElementTagNameMap[TKey]> &
        EventAttributes<HTMLElementTagNameMap[TKey]>;
};

export type IntrinsicElementsSVG = {
    [TKey in SVGOnlyElementKeys]?: SVGElementAttributes<TKey> &
        HTMLComponentProps<SVGElementTagNameMap[TKey]> &
        EventAttributes<SVGElementTagNameMap[TKey]>;
};

export type IntrinsicElementsHTMLAndSVG = IntrinsicElementsHTML & IntrinsicElementsSVG;

export type IntrinsicElementsCombined = IfTsxDomTypeConfig<"html", IntrinsicElementsHTML, unknown> &
    IfTsxDomTypeConfig<"svg", IntrinsicElementsSVG, unknown>;

// eslint-disable-next-line @typescript-eslint/no-empty-object-type
export interface CustomElementsHTML {}