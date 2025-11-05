import type { ComponentChildren } from "../types";
import { appendChildren } from "./jsx-append-children";

export function Fragment(attr: { children?: ComponentChildren | undefined }): any {
    const fragment = document.createDocumentFragment()
    appendChildren(fragment, attr.children)
    return fragment;
}
