import type { Ref } from "./basic-types";
import type { ComponentChildren, JSXElement } from "./jsx-namespace";

export interface ComponentClass<P = {}, T extends Node = JSXElement> {
    new(props: P): ComponentClass<P, T>
    render(): JSXElement | null
    defaultProps?: Partial<P> | undefined
    readonly props?: P & { children?: ComponentChildren }
    displayName?: string | undefined
}

export type FunctionComponent<P = {}, T extends Node = JSXElement> = (props: P & { children?: ComponentChildren }) => T | null;
export type ComponentType<P = {}, T extends Node = JSXElement> = ComponentClass<P, T> | FunctionComponent<P, T>

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

