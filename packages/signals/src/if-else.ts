import { addDisposingListener, removeDisposingListener, type ComponentChildren, type JSXElement, type SignalOrValue } from "@serenity-is/sleekdom";
import { signal } from "./signals";
import { isSignalLike, observeSignal } from "./util";

export function IfElse<TWhen>(props: {
    when: SignalOrValue<TWhen | undefined | null>;
    else?: ComponentChildren;
    children: ComponentChildren | ((when: SignalOrValue<TWhen | undefined | null>) => ComponentChildren);
}): JSXElement {
    function getContent(flag: boolean): JSXElement {
        let content = props.children;
        if (typeof content === "function")
            content = content(props.when);
        content = flag ? content : props.else;
        content ??= new Text("");
        return content as unknown as JSXElement;
    }

    if (isSignalLike(props.when)) {
        let dispose: null | (() => void);
        const sig = signal<JSXElement>();
        dispose = observeSignal(props.when, (value) => {
            const content = getContent(!!value);
            if (sig.peek() !== content && dispose)
                removeDisposingListener(sig.peek() as any, dispose);
            sig.value = addDisposingListener(content, dispose);
        });
        addDisposingListener(sig.peek() as any, dispose);
        return sig as unknown as JSXElement;
    }

    return getContent(!!props.when);
}
