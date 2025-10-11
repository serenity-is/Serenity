
/** CSSStyleDeclaration contains methods, readonly properties and an index signature, which we all need to filter out. */
export type StyleProperties = Partial<
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

