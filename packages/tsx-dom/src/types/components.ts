import type { JSXElement } from "./base-types";
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