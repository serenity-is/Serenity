/**
 * A unique symbol used to mark a value as a JSX prop hook. When a prop hook is
 * assigned to a JSX attribute, the JSX factory calls this method on the hook
 * with the target node and property name to initialize the binding.
 */
export const initPropHookSymbol: unique symbol = Symbol.for("Serenity.initPropHook");

declare module "../types" {
    interface PropHook<TNode extends Element = Element> {
        [initPropHookSymbol](node: TNode, propName: string): void;
    }
}
