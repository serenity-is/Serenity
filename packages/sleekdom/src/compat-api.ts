import { jsx } from "./jsx-factory"
import { attachRef, createRef } from "./ref"
import type { FunctionComponent, JSXElement, Ref } from "./types"
import { isString } from "./util"

export { Fragment as StrictMode } from "./fragment"
export { createRef as useRef } from "./ref"
export { identity as memo, identity as useCallback } from "./util"

export function useMemo<T>(factory: () => T): T {
  return factory()
}

export function forwardRef<T = Node, P = {}>(
  render: (props: P, ref: Ref<T>) => JSXElement
): FunctionComponent<P & { ref?: Ref<T> }> {
  return ({ ref, ...props }) => render(props as P, ref ?? createRef<T>())
}

export function useImperativeHandle<T>(ref: Ref<T>, init: () => T, _deps?: unknown) {
  attachRef(ref, init())
}

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

export const h = createElement;

export function createFactory(tag: string | FunctionComponent<any>) {
    return createElement.bind(null, tag)
}
