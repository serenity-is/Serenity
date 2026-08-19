import { isObject, isVisibleChild } from "./util"

/**
 * Converts a heterogeneous class value to a normalized space-separated class string.
 *
 * Handles strings, nested arrays (recursively flattened), any iterable (e.g. `Set`),
 * and dictionary objects where only keys with truthy values are included.
 * Falsy primitives, `true`/`false`, `null` and `undefined` produce an empty string.
 *
 * @param value - The class value to normalize. May be a string, array, iterable,
 * dictionary (`Record<string, boolean>`), primitive, or nested combination thereof.
 * @returns A space-separated class name string, or `""` when the input yields no classes.
 * @example
 * ```ts
 * className("foo bar") // => "foo bar"
 * className(["foo", ["bar", { baz: true, qux: false }]]) // => "foo bar baz"
 * className(new Set(["a", "b"])) // => "a b"
 * ```
 */
export function className(value: any): string {
    if (Array.isArray(value)) {
        return value.map(className).filter(Boolean).join(" ")
    } else if (isObject(value)) {
        if (Symbol.iterator in value) {
            return className(Array.from(value))
        }
        return Object.keys(value)
            .filter(k => value[k])
            .join(" ")
    } else if (isVisibleChild(value)) {
        return "" + value
    } else {
        return ""
    }
}