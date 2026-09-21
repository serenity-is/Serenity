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
type StylePropertyKeys = {
    [K in keyof CSSStyleDeclaration]: K extends string
        ? CSSStyleDeclaration[K] extends string
        ? K
        : never
        : never;
}[keyof CSSStyleDeclaration];

/**
 * Style properties for the `style` JSX attribute, where each CSS property may
 * be a string (e.g. `"14px"`), a number (raw value, or `px` for non-unitless
 * properties), or a signal-like value that updates reactively. CSS custom
 * properties (`--*`) are supported too.
 */
export type StyleProperties = {
    [K in StylePropertyKeys]?: SignalOrValue<string | number | undefined>;
} & {
    [key: `--${string}`]: SignalOrValue<string | number | undefined>;
}
