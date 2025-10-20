import { addDisposingListener, removeDisposingListener, isSignalLike, type ComponentChildren, type JSXElement, type SignalOrValue, observeSignal } from "@serenity-is/sleekdom";
import { signal } from "./signals";

export function IfElse<TWhen>(props: {
    when: SignalOrValue<TWhen | undefined | null>;
    else?: ComponentChildren | ((when: SignalOrValue<TWhen | undefined | null>) => ComponentChildren);
    children: ComponentChildren | ((when: SignalOrValue<TWhen | undefined | null>) => ComponentChildren);
}): JSXElement {
    function getContent(flag: boolean): JSXElement {
        let content = flag ? props.children : props.else;
        if (typeof content === "function")
            content = content(props.when);
        content ??= new Text("");
        return content as unknown as JSXElement;
    }

    if (isSignalLike(props.when)) {
        const sig = signal<JSXElement>();
        observeSignal(props.when, function(value) {
            const content = getContent(!!value);
            if (sig.peek() !== content)
                removeDisposingListener(sig.peek() as any, this?.dispose);
            sig.value = addDisposingListener(content, this?.dispose);
        });
        return sig as unknown as JSXElement;
    }

    return getContent(!!props.when);
}
