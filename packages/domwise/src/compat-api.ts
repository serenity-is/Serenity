import type { Ref } from "../types";
import { jsx } from "./jsx-factory";
import { setRef } from "./ref";
import { isString } from "./util";

/** Required for classic (non-automatic) jsx factory. Prefer jsx function */
export function createElement(tag: any, attr: any, ...children: any[]) {
    if (isString(attr) || Array.isArray(attr)) {
        children.unshift(attr);
        attr = {};
    }

    attr = attr || {};

    if (attr.children != null && !children.length) {
        ({ children, ...attr } = attr);
    }

    return jsx(tag, { ...attr, children });
}

/** For compatibility with React's useImperativeHandle, use setRef instead */
export function useImperativeHandle<T>(ref: Ref<T>, init: () => T) {
    setRef(ref, init());
}
