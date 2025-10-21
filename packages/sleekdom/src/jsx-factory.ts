import { initComponentClass } from "./component"
import { appendChildren } from "./jsx-append-children"
import { assignProps } from "./jsx-assign-props"
import { attachRef } from "./ref"
import { SVGNamespace, svgTags } from "./svg-consts"
import type { JSXElement } from "./types"
import type { ComponentChildren } from "./types/custom-attributes"
import type { ElementAttributes, HTMLElementTags, SVGElementTags } from "./types/dom-expressions-jsx"
import { isComponentClass, isObject, isString } from "./util"


type DataKeys = `data-${string}`

// DOM Elements
export function jsx<THtmlTag extends keyof HTMLElementTagNameMap, TElement extends HTMLElementTagNameMap[THtmlTag]>(
    type: THtmlTag,
    props?: (HTMLElementTags[THtmlTag] & Record<DataKeys, string | number>) | null,
    key?: string
): TElement
export function jsx<TSvgTag extends (keyof SVGElementTagNameMap & keyof SVGElementTags), TElement extends SVGElementTagNameMap[TSvgTag]>(
    type: TSvgTag,
    props?: (SVGElementTags[TSvgTag] & Record<DataKeys, string | number>) | null,
    key?: string
): TElement
export function jsx(
    type: string,
    props?: (ElementAttributes<JSXElement> & Record<DataKeys, string | number>) | null,
    key?: string
): JSXElement
// Custom components
//export function jsx<P extends {}, TElement extends JSXElement = JSXElement>(
//    type: ComponentType<P, TElement>,
//    props?: P & { children?: ComponentChildren; ref?: Ref<TElement> } | null,
//    key?: string
//): TElement
// eslint-disable-next-line @typescript-eslint/no-unused-vars
export function jsx(tag: any, props?: { children?: ComponentChildren, [key: string]: any }, _key?: string) {

    let { children, ...attr } = props || {};
    if (!attr.namespaceURI && svgTags.has(tag))
        attr.namespaceURI = SVGNamespace

    let node: HTMLElement | SVGElement
    if (isString(tag)) {
        node = attr.namespaceURI
            ? document.createElementNS(attr.namespaceURI, tag)
            : document.createElement(tag)
        assignProps(node, attr)
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
    } else if (typeof tag === "function") {
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
