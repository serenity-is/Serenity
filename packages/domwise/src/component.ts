import type { ComponentChildren, ComponentClass, JSXElement, Ref } from "../types";
import { setRef } from "./ref";

/**
 * Base class for creating JSX components with optional props, children, and ref support.
 * Extend this class and override the `render` method to return a `JSXElement`.
 * @typeParam T - The type of the component's props.
 */
export class Component<T = any> {
    static isComponent = true;

    constructor(props: T & { children?: ComponentChildren; ref?: Ref<any> }) {
        this.props = props
    }

    readonly props: T & { children?: ComponentChildren; ref?: Ref<any> };

    render(): JSXElement | null {
        return null
    }
}

export function initComponentClass(Class: ComponentClass, attr: any, children: any): JSXElement | null {
    attr = { ...attr, children };
    const instance = new Class(attr);
    const node = instance.render();
    if ("ref" in attr) {
        setRef(attr.ref, instance);
    }
    return node;
}