import type { ComponentChildren, ComponentClass, JSXElement, Ref } from "../types";
import { setRef } from "./ref";

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

export function initComponentClass(Class: ComponentClass, attr: any, children: any) {
    attr = { ...attr, children }
    const instance = new Class(attr)
    const node = instance.render()
    if ("ref" in attr) {
        setRef(attr.ref, instance)
    }
    return node
}