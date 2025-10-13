import { appendChild } from "./append-child"
import type { ComponentChildren } from "./types"

export function Fragment(attr: { children?: ComponentChildren | undefined }): any {
    const fragment = document.createDocumentFragment()
    appendChild(attr.children, fragment)
    return fragment
}
