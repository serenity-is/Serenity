export interface ConfigureJSXElement {
    svg: boolean;
}

/**
 * This technically should include `DocumentFragment` as well, but a lot of web APIs expect an `Element`.
 */
export type JSXElement = HTMLElement | (ConfigureJSXElement["svg"] extends false ? never : SVGElement);
