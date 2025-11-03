import type { HTMLElementTags, MathMLElementTags, SVGElementTags } from "./dom-expressions-jsx";
import type { ConfigureJSXElement } from "./jsx-element";

export type IntrinsicElementsCombined = HTMLElementTags &
    (ConfigureJSXElement["svg"] extends false ? void : SVGElementTags) &
    (ConfigureJSXElement["mathml"] extends false ? void : MathMLElementTags);

// eslint-disable-next-line @typescript-eslint/no-empty-object-type
export interface CustomElementsHTML {}