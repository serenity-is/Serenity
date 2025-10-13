import { appendChildren } from "./append-children"
import type { ComponentChildren } from "./types"

export function Fragment(attr: { children?: ComponentChildren | undefined }): any {
    const fragment = document.createDocumentFragment()
    appendChildren(fragment, attr.children)
    return fragment
}
