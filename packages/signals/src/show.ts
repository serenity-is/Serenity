import type { ComponentChildren, JSXElement, SignalOrValue } from "@serenity-is/sleekdom";
import { signal } from "./signals";
import { isSignalLike, observeSignal } from "./util";

type IfCondition<TWhen> = SignalOrValue<TWhen | undefined | null>;

export function Show<TWhen>(props: {
  when: IfCondition<TWhen>;
  fallback?: ComponentChildren;
  children: ComponentChildren | ((when: IfCondition<TWhen>) => ComponentChildren);
}): JSXElement {

    function getContent(flag: boolean): JSXElement {
        if (typeof props.children === "function")
            return props.children(props.when) as unknown as JSXElement;
        
        if (flag)
            return props.children as unknown as JSXElement;

        return props.fallback as unknown as JSXElement;
    }

    if (isSignalLike(props.when)) {
        const sig = signal<JSXElement>();
        observeSignal(props.when, (value) => {
            sig.value = getContent(!!value);
        });
        return sig as unknown as JSXElement;
    }

    return getContent(!!props.when);
}
