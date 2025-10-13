import { appendChildren } from "./append-children"
import { initComponentClass } from "./component"
import { attachRef } from "./ref"
import { setProperties } from "./set-properties"
import { SVGNamespace, svgTags } from "./svg-consts"
import type { ComponentChildren, ComponentType, Ref } from "./types"
import type { HTMLAttributes } from "./types/html-attributes"
import type { DOMAttributes } from "./types/intrinsic-elements"
import type { SVGAttributes } from "./types/svg-attributes"
import { isComponentClass, isFunction, isObject, isString } from "./util"

// DOM Elements
export function jsx<K extends keyof HTMLElementTagNameMap, T extends HTMLElementTagNameMap[K]>(
    type: K,
    props?: (HTMLAttributes<T> & { children?: ComponentChildren; ref?: Ref<T> }) | null,
    key?: string
): T
export function jsx<K extends keyof SVGElementTagNameMap, T extends SVGElementTagNameMap[K]>(
    type: K,
    props?: (SVGAttributes<T> & { children?: ComponentChildren; ref?: Ref<T> }) | null,
    key?: string
): SVGElement
export function jsx<T extends Element>(
    type: string,
    props?: { children?: ComponentChildren; ref?: Ref<T> } & DOMAttributes<T> | null,
    key?: string
): T
// Custom components
export function jsx<P extends {}, T extends Element>(
    type: ComponentType<P, T>,
    props?: P & { children?: ComponentChildren; ref?: Ref<T> } | null,
    key?: string
): T
export function jsx<T extends Element>(
    type: string,
    props?: { children?: ComponentChildren } | null,
    key?: string
): T
// eslint-disable-next-line @typescript-eslint/no-unused-vars
export function jsx(tag: any, { children, ...attr }: { children?: ComponentChildren, [key: string]: any }, _key?: string) {
    if (!attr.namespaceURI && svgTags[tag] === 0)
        attr.namespaceURI = SVGNamespace

    let node: HTMLElement | SVGElement
    if (isString(tag)) {
        node = attr.namespaceURI
            ? document.createElementNS(attr.namespaceURI, tag)
            : document.createElement(tag)
        setProperties(node, attr)
        appendChildren(node, children)

        if (node instanceof window.HTMLSelectElement && attr.value != null) {
            if (attr.multiple === true && Array.isArray(attr.value)) {
                const values = attr.value.map(value => String(value))
                node
                    .querySelectorAll("option")
                    .forEach(option => (option.selected = values.includes(option.value)))
            } else {
                node.value = attr.value
            }
        }

        attachRef(attr.ref, node)
    } else if (isFunction(tag)) {
        // Custom elements.
        if (isObject(tag.defaultProps)) {
            attr = { ...tag.defaultProps, ...attr }
        }

        node = isComponentClass(tag)
            ? initComponentClass(tag, attr, children)
            : tag({ ...attr, children })
    } else {
        throw new TypeError(`Invalid JSX element type: ${tag}`)
    }
    return node
}
