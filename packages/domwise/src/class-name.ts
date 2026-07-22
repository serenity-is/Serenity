import { isObject, isVisibleChild } from "./util"

/**
 * Converts a value to a className string.
 * Supports strings, arrays (flattened), iterables, and `Dictionary<boolean>` objects
 * where truthy keys are included.
 * @param value - The value to convert. Can be a string, array, iterable, or dictionary.
 * @returns A space-separated className string.
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