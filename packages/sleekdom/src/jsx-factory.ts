import { initComponentClass } from "./component"
import { appendChildren } from "./jsx-append-children"
import { setProperties } from "./jsx-set-properties"
import { attachRef } from "./ref"
import { SVGNamespace, svgTags } from "./svg-consts"
import type { ComponentType, JSX, Ref } from "./types"
import type { ComponentChildren } from "./types/custom-attributes"
import type { ElementAttributes, HTMLElementTags, SVGElementTags } from "./types/dom-expressions-jsx"
import { isComponentClass, isFunction, isObject, isString } from "./util"

// DOM Elements
export function jsx<K extends keyof HTMLElementTagNameMap, T extends HTMLElementTagNameMap[K]>(
    type: K,
    props?: HTMLElementTags[K] | null,
    key?: string
): T
export function jsx<K extends (keyof SVGElementTagNameMap & keyof SVGElementTags), T extends SVGElementTagNameMap[K]>(
    type: K,
    props?: SVGElementTags[K] | null,
    key?: string
): SVGElement
export function jsx<T extends Element>(
    type: string,
    props?: ElementAttributes<T> | null,
    key?: string
): T
// Custom components
export function jsx<P extends {}, T extends Element>(
    type: ComponentType<P, T>,
    props?: P & { children?: ComponentChildren; ref?: Ref<T> } | null,
    key?: string
): T
// eslint-disable-next-line @typescript-eslint/no-unused-vars
export function jsx(tag: any, { children, ...attr }: { children?: ComponentChildren, [key: string]: any }, _key?: string) {
    if (!attr.namespaceURI && svgTags.has(tag))
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
