export interface TsxDomTypeConfig {
    [s: string]: boolean;
}

export type IfTsxDomTypeConfig<T extends string, TIF, TELSE> = TsxDomTypeConfig[T] extends false ? TELSE : TIF;

export type JSXElement = IfTsxDomTypeConfig<"html", HTMLElement, never> | IfTsxDomTypeConfig<"svg", SVGElement, never>;

export type RefType<T> = { current: T | null };
