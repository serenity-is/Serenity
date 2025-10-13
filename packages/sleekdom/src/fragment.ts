import { appendChildren } from "./append-child"
import type { ComponentChildren } from "./types"

export function Fragment(attr: { children?: ComponentChildren | undefined }): any {
    const fragment = document.createDocumentFragment()
    appendChildren(fragment, attr.children)
    return fragment
}
