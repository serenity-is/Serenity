import type { SignalOrValue } from "./basic-types";

type RemoveIndex<T> = { [K in keyof T as string extends K ? never : number extends K ? never : K]: T[K] };
// eslint-disable-next-line @typescript-eslint/no-unsafe-function-type
type ExcludeMethods<T> = Pick<T, { [K in keyof T]: T[K] extends Function ? never : K }[keyof T]>;
/**
 * Style properties that can be assigned to the `style` attribute, with methods,
 * readonly properties, and the index signature filtered out of
 * `CSSStyleDeclaration`.
 */
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

/**
 * Style properties whose values may be plain values or signals.
 */
export type StyleProperties = {
    [K in keyof StylePropertiesBase]: SignalOrValue<StylePropertiesBase[K]>;
}
