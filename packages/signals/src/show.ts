import type { ComponentChildren, JSXElement, SignalOrValue } from "@serenity-is/sleekdom";
import { signal } from "./signals";
import { isSignalLike, observeSignal } from "./util";

export function Show<TWhen>(props: {
  when: SignalOrValue<TWhen | undefined | null>;
  fallback?: ComponentChildren;
  children: ComponentChildren | ((when: SignalOrValue<TWhen | undefined | null>) => ComponentChildren);
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
