import { derivedSignal, isSignalLike, observeSignal } from "../signal-util";
import type { ComponentChildren, JSXElement, SignalOrValue } from "../../types";

export function Show<TWhen>(props: {
    when: SignalOrValue<TWhen | undefined | null>;
    fallback?: ComponentChildren | ((when: SignalOrValue<TWhen | undefined | null>) => ComponentChildren);
    children: ComponentChildren | ((when: SignalOrValue<TWhen | undefined | null>) => ComponentChildren);
}): JSXElement {
    function getContent(flag: boolean): JSXElement {
        let content = flag ? props.children : props.fallback;
        if (typeof content === "function")
            content = content(props.when);
        content ??= new Text("");
        return content as unknown as JSXElement;
    }

    if (isSignalLike(props.when)) {
        const sig = derivedSignal<JSXElement>(props.when, getContent);
        observeSignal(sig, function(args) {
            args.lifecycleNode = args.newValue;
        });
        return sig as unknown as JSXElement;
    }

    return getContent(!!props.when);
}
