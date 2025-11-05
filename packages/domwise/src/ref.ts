import type { Ref, RefObject } from "../types";

/** Creates a new RefObject with current property */
export function createRef<T = any>(): RefObject<T> {
    return Object.seal({ current: null });
}

export function isRefObject<T = Node>(maybeRef: any): maybeRef is RefObject<T> {
    return maybeRef != null && typeof maybeRef === "object" && "current" in maybeRef;
}

/** Sets ref.current for a RefObject or a by calling a ref callback */
export function setRef<T = Node>(ref: Ref<T> | undefined, current: T) {
    if (isRefObject<T>(ref)) {
        ref.current = current;
    } else if (typeof ref === "function") {
        ref(current);
    }
}