import type { ComponentChildren } from "../types";
import { appendChildren } from "./jsx-append-children";

/**
 * Creates a `DocumentFragment` containing the given children.
 *
 * Intended as the JSX fragment factory (i.e. the target for the `<></>` syntax
 * when `jsxFragment` is set to `Fragment`). Accepts the standard
 * `{ children }` props bag produced by the JSX transform.
 *
 * @param attr - Props bag with optional `children` to append to the fragment.
 * @returns A `DocumentFragment` containing the appended children.
 */
export function Fragment(attr: { children?: ComponentChildren | undefined }): any {
    const fragment = document.createDocumentFragment()
    appendChildren(fragment, attr.children)
    return fragment;
}
