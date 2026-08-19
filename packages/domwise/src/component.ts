import type { ComponentChildren, ComponentClass, JSXElement, Ref } from "../types";
import { setRef } from "./ref";

/**
 * Base class for class-based JSX components.
 *
 * Extend this class and override {@link render} to return a {@link JSXElement}.
 * Props (including optional `children` and `ref`) are available via {@link props}.
 *
 * @typeParam T - The type of the component's props (excluding `children` and `ref` which are added automatically).
 * @example
 * ```tsx
 * class Greeting extends Component<{ name: string }> {
 *   render() {
 *     return <div>Hello, {this.props.name}!</div>;
 *   }
 * }
 * // usage: <Greeting name="World" />
 * ```
 */
export class Component<T = any> {
    /**
     * Marker used by the JSX factory to distinguish class components from function components.
     * Do not modify.
     */
    static isComponent = true;

    /**
     * Creates a component instance.
     * @param props - Props passed to the component, including optional `children` and `ref`.
     */
    constructor(props: T & { children?: ComponentChildren; ref?: Ref<any> }) {
        this.props = props
    }

    /** Props passed to this component instance, including optional `children` and `ref`. */
    readonly props: T & { children?: ComponentChildren; ref?: Ref<any> };

    /**
     * Renders the component's output.
     * Override in subclasses to return a DOM node, fragment, or `null`.
     * @returns The rendered {@link JSXElement}, or `null` to render nothing.
     */
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