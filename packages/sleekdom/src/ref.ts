import type { RefObject } from "./types"
import { isFunction, isObject } from "./util"

export function createRef<T = any>(): RefObject<T> {
    return Object.seal({ current: null })
}

export function isRefObject<T = Node>(maybeRef: any): maybeRef is RefObject<T> {
    return isObject(maybeRef) && "current" in maybeRef
}

export function attachRef<T = Node>(ref: any | undefined, node: T) {
    if (isRefObject<T>(ref)) {
        ref.current = node
    } else if (isFunction(ref)) {
        ref(node)
    }
}