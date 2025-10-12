export interface ConfigureJSXElement {
    svg: boolean;
}

export type JSXElement = HTMLElement & (ConfigureJSXElement["svg"] extends false ? never : SVGElement);

export type RefType<T> = { current: T | null };
