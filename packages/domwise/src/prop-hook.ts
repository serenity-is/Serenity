/**
 * Well-known symbol (`Symbol.for("Serenity.initPropHook")`) that marks a value
 * as a JSX prop hook.
 *
 * When a prop value that carries this symbol is assigned as a JSX attribute,
 * the renderer invokes `value[initPropHookSymbol](node, propName)` to let the
 * hook bind itself to the DOM node (e.g. to observe signals or install
 * class-list synchronization). See also the {@link PropHook} interface.
 */
export const initPropHookSymbol: unique symbol = Symbol.for("Serenity.initPropHook");

declare module "../types" {
    interface PropHook<TNode extends Element = Element> {
        [initPropHookSymbol](node: TNode, propName: string): void;
    }
}
