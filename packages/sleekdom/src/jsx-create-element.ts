import { jsx } from "./jsx-factory"
import { isString } from "./util"

export function createElement(tag: any, attr: any, ...children: any[]) {
    if (isString(attr) || Array.isArray(attr)) {
        children.unshift(attr)
        attr = {}
    }

    attr = attr || {}

    if (attr.children != null && !children.length) {
        ; ({ children, ...attr } = attr)
    }

    return jsx(tag, { ...attr, children }, attr.key)
}
