import type { Ref } from "./basic-types";
import type { ComponentChildren, JSXElement } from "./jsx-namespace";

/**
 * A class-based JSX component. Extend `Component` or implement this interface
 * and override `render` to return a `JSXElement`.
 * @typeParam P - The type of the component's props.
 * @typeParam T - The type of the DOM node the component renders.
 */
export interface ComponentClass<P = {}, T extends Node = JSXElement> {
    new(props: P): ComponentClass<P, T>
    render(): JSXElement | null
    defaultProps?: Partial<P> | undefined
    readonly props?: P & { children?: ComponentChildren }
    displayName?: string | undefined
}

/**
 * A function-based JSX component that receives props (including `children`)
 * and returns a `JSXElement` or `null`.
 * @typeParam P - The type of the component's props.
 * @typeParam T - The type of the DOM node the component renders.
 */
export type FunctionComponent<P = {}, T extends Node = JSXElement> = (props: P & { children?: ComponentChildren }) => T | null;
/**
 * A JSX component: either a class-based or a function-based component.
 * @typeParam P - The type of the component's props.
 * @typeParam T - The type of the DOM node the component renders.
 */
export type ComponentType<P = {}, T extends Node = JSXElement> = ComponentClass<P, T> | FunctionComponent<P, T>

/**
 * A virtual descriptor for a `ShadowRoot` created by `ShadowRootNode`.
 * Recognized by the JSX factory to create a shadow root on the parent element.
 */
export type ShadowRootContainer = {
    ref: Ref<ShadowRoot>;
    attr: {
        clonable?: boolean;
        customElementRegistry?: CustomElementRegistry;
        delegatesFocus?: boolean;
        mode: ShadowRootMode;
        serializable?: boolean;
        slotAssignment?: SlotAssignmentMode;
    };
    children: ComponentChildren;
}

