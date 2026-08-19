import type { ComponentChildren, JSXElement, SignalOrValue } from "../types";
import { derivedSignal, isSignalLike, observeSignal } from "./signal-util";

/**
 * Conditional rendering helper similar to Solid's `<Show>`.
 *
 * Renders `children` when `when` is truthy, otherwise renders `fallback`.
 * Either slot may be a plain `ComponentChildren` value or a function that
 * receives the `when` signal/value. When `when` is a signal, the output is
 * a derived signal node so the DOM updates reactively; its lifecycle is
 * bound to the rendered node so the subscription is disposed with it.
 *
 * @typeParam TWhen - Type of the condition value.
 * @param props - Props bag.
 * @param props.when - Condition; truthiness controls which branch is shown. May be a plain value or a signal.
 * @param props.fallback - Content rendered when `when` is falsy. May be children or a function `(when) => children`.
 * @param props.children - Content rendered when `when` is truthy. May be children or a function `(when) => children`.
 * @returns A `JSXElement` (or derived-signal node) representing the active branch.
 * @example
 * ```tsx
 * const loggedIn = signal(false);
 * <Show when={loggedIn} fallback="Please sign in">Welcome!</Show>
 * <Show when={loggedIn}>{() => <Dashboard user={loggedIn.value} />}</Show>
 * ```
 */
export function Show<TWhen>(props: {
    when: SignalOrValue<TWhen | undefined | null>;
    fallback?: ComponentChildren | ((when: SignalOrValue<TWhen | undefined | null>) => ComponentChildren);
    children: ComponentChildren | ((when: SignalOrValue<TWhen | undefined | null>) => ComponentChildren);
}): JSXElement {
    function getContent(flag: any): JSXElement {
        let content = flag ? props.children : props.fallback;
        if (typeof content === "function")
            content = content(props.when);
        content ??= new Text("");
        return content as unknown as JSXElement;
    }

    if (isSignalLike(props.when)) {
        const sig = derivedSignal<JSXElement>(props.when, getContent);
        observeSignal(sig, function(args) {
            if (args.newValue instanceof EventTarget)
                args.lifecycleNode = args.newValue;
        });
        return sig as unknown as JSXElement;
    }

    return getContent(!!props.when);
}
