export const initPropHookSymbol = Symbol.for("Serenity.initPropHook");

declare module "../types" {
    interface PropHook<TNode extends Element = Element> {
        [initPropHookSymbol](node: TNode, propName: string): void;
    }
}
