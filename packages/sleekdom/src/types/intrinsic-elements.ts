import type { HTMLElementTags, SVGElementTags } from "./dom-expressions-jsx";
import type { ConfigureJSXElement } from "./jsx-element";

export type IntrinsicElementsCombined = HTMLElementTags &
    (ConfigureJSXElement["svg"] extends false ? void : SVGElementTags);

// eslint-disable-next-line @typescript-eslint/no-empty-object-type
export interface CustomElementsHTML {}