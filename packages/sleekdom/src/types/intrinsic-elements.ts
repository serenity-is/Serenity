import type { BaseProps, Ref } from "./components";
import type { HTMLElementAttributes, SVGElementAttributes, SVGOnlyElementKeys } from "./element-attributes";
import type { EventAttributes } from "./event-attributes";
import type { ConfigureJSXElement } from "./jsx-element";

// eslint-disable-next-line @typescript-eslint/no-empty-object-type
export interface CustomElementsHTML {}

export interface HTMLComponentProps<T extends Element> extends BaseProps {
    dangerouslySetInnerHTML?: { __html: string };

    on?: Record<string, Function>;
    onCapture?: Record<string, Function>;

    /**
     * This is essentially a reverse "is" attribute.
     * If you specify it, the generated tag will be tsxTag and it will receive an "is" attribute with the tag you specified in your JSX.
     * This is needed because we can't make the is-property associate with the correct component props.
     */
    tsxTag?: keyof HTMLElementTagNameMap | keyof SVGElementTagNameMap;
    ref?: Ref<T>;
}

export type DOMAttributes<T extends Element> = EventAttributes<T> & HTMLComponentProps<T>;

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

export type IntrinsicElementsCombined = IntrinsicElementsHTML &
    (ConfigureJSXElement["svg"] extends false ? void : IntrinsicElementsSVG);

// eslint-disable-next-line @typescript-eslint/no-empty-object-type
export interface CustomElementsHTML {}