export interface BasicClassList {
  (value: Element): void
  readonly size: number
  readonly value: string
  add(...tokens: string[]): void
  remove(...tokens: string[]): void
  toggle(token: string, force?: boolean): void
  contains(token: string): boolean
}
type ClassName = string | { [key: string]: boolean } | false | null | undefined | ClassName[]
export type ClassNames = ClassName | BasicClassList | Iterable<string> | DOMTokenList;

