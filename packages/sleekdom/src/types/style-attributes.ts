import type { SignalOrValue } from "./signal-like";

type RemoveIndex<T> = { [K in keyof T as string extends K ? never : number extends K ? never : K]: T[K] };
// eslint-disable-next-line @typescript-eslint/no-unsafe-function-type
type ExcludeMethods<T> = Pick<T, { [K in keyof T]: T[K] extends Function ? never : K }[keyof T]>;
export type StyleAttributes = Partial<ExcludeMethods<RemoveIndex<Omit<CSSStyleDeclaration, "length" | "parentRules">>>>;

/** CSSStyleDeclaration contains methods, readonly properties and an index signature, which we all need to filter out. */
type StylePropertiesBase = Partial<
    Pick<
        CSSStyleDeclaration,
        {
            [K in keyof CSSStyleDeclaration]: K extends string
                ? CSSStyleDeclaration[K] extends string
                    ? K
                    : never
                : never;
        }[keyof CSSStyleDeclaration]
    >
>;

export type StyleProperties = {
    [K in keyof StylePropertiesBase]: SignalOrValue<StylePropertiesBase[K]>;
}
