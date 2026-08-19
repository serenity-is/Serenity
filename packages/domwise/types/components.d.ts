import type { Ref } from "./basic-types";
import type { ComponentChildren, JSXElement } from "./jsx-namespace";

/**
 * A class-based JSX component. Extend `Component` or implement this interface
 * and override `render` to return a `JSXElement`.
 * @typeParam P - The type of the component's props.
 * @typeParam T - The type of the DOM node the component renders.
 */
export interface ComponentClass<P = {}, T extends Node = JSXElement> {
    /**
     * Constructs the component with the given props.
     * @param props - Props including optional `children`.
     */
    new(props: P): ComponentClass<P, T>
    /**
     * Renders the component.
     * @returns The rendered `JSXElement` or `null`.
     */
    render(): JSXElement | null
    /** Optional default prop values merged in by the JSX factory before construction. */
    defaultProps?: Partial<P> | undefined
    /** Props passed to the instance, including optional `children`. */
    readonly props?: P & { children?: ComponentChildren }
    /** Optional display name used in devtools / error messages. */
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
 * Recognized by the JSX factory to create a shadow root on the parent element
 * via `attachShadow`. See {@link ShadowRootNode}.
 */
export type ShadowRootContainer = {
    /** Optional ref that receives the created `ShadowRoot`. */
    ref: Ref<ShadowRoot>;
    /** `ShadowRootInit` options forwarded to `attachShadow`. */
    attr: {
        /** Whether the shadow root should be clonable. */
        clonable?: boolean;
        /** Custom element registry for the shadow tree. */
        customElementRegistry?: CustomElementRegistry;
        /** Whether focus should delegate to the shadow host. */
        delegatesFocus?: boolean;
        /** Shadow root mode (`"open"` or `"closed"`). */
        mode: ShadowRootMode;
        /** Whether the shadow root is serializable. */
        serializable?: boolean;
        /** Slot assignment mode (`"manual"` or `"named"`). */
        slotAssignment?: SlotAssignmentMode;
    };
    /** Children rendered inside the shadow root. */
    children: ComponentChildren;
}

