import { forEach, isNumber, isObject, isString, isVisibleChild, keys } from "./util"

/**
 * Convert a `value` to a className string.
 * `value` can be a string, an array or a `Dictionary<boolean>`.
 */
export function className(value: any): string {
    if (Array.isArray(value)) {
        return value.map(className).filter(Boolean).join(" ")
    } else if (isObject(value)) {
        if (Symbol.iterator in value) {
            return className(Array.from(value))
        }
        return keys(value)
            .filter(k => value[k])
            .join(" ")
    } else if (isVisibleChild(value)) {
        return "" + value
    } else {
        return ""
    }
}