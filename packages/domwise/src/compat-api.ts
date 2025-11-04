import type { Ref } from "../types"
import { jsx } from "./jsx-factory"
import { attachRef } from "./ref"
import { isString } from "./util"

export { createRef as useRef } from "./ref"

export function useImperativeHandle<T>(ref: Ref<T>, init: () => T) {
    attachRef(ref, init());
}

export function createElement(tag: any, attr: any, ...children: any[]) {
    if (isString(attr) || Array.isArray(attr)) {
        children.unshift(attr)
        attr = {}
    }

    attr = attr || {}

    if (attr.children != null && !children.length) {
        ({ children, ...attr } = attr);
    }

    return jsx(tag, { ...attr, children });
}

export const h = createElement;