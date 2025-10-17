import { PlainSignal } from "./plain-signal";
import type { ComponentChildren, JSXElement } from "./types";
import type { SignalOrValue } from "./types/signal-like";
import { isSignalLike, observeSignal } from "./util";

type FlowWhen<TWhen> = SignalOrValue<TWhen | undefined | null>;

export function Show<TWhen>(props: {
  when: FlowWhen<TWhen>;
  fallback?: ComponentChildren;
  children: ComponentChildren | ((when: FlowWhen<TWhen>) => ComponentChildren);
}): JSXElement {

    function getContent(flag: boolean): JSXElement {
        if (typeof props.children === "function")
            return props.children(props.when) as unknown as JSXElement;
        
        if (flag)
            return props.children as unknown as JSXElement;

        return props.fallback as unknown as JSXElement;
    }

    if (isSignalLike(props.when)) {
        const sig = new PlainSignal<JSXElement>();        
        observeSignal(props.when, (value) => {
            sig.value = getContent(!!value);
        });
        return sig as unknown as JSXElement;
    }

    return getContent(!!props.when);
}
