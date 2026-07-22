import type { ComponentChildren } from "../types";
import { appendChildren } from "./jsx-append-children";

/**
 * Creates a document fragment containing the given children.
 * Useful as a JSX fragment factory (e.g. `<></>`).
 * @param attr - Object with an optional `children` property.
 * @returns A `DocumentFragment` with the appended children.
 */
export function Fragment(attr: { children?: ComponentChildren | undefined }): any {
    const fragment = document.createDocumentFragment()
    appendChildren(fragment, attr.children)
    return fragment;
}
