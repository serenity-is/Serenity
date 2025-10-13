import { createElement } from "./jsx-create-element"
import { attachRef, createRef } from "./ref"
import type { FunctionComponent, JSXElement, Ref } from "./types"

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

export function createFactory(tag: string | FunctionComponent<any>) {
    return createElement.bind(null, tag)
}
