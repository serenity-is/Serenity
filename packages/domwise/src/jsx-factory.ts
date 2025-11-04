import { initComponentClass } from "./component"
import { appendChildren } from "./jsx-append-children"
import { assignProps } from "./jsx-assign-props"
import { MathMLNamespace, mathMLOnlyTags as mathMLOnlyTags } from "./mathml-consts"
import { currentNamespaceURI } from "./in-namespace-uri"
import { attachRef } from "./ref"
import { SVGNamespace, svgOnlyTags as svgOnlyTags } from "./svg-consts"
import type { JSXElement } from "./types"
import type { ComponentChildren } from "./types/custom-attributes"
import type { ElementAttributes, HTMLElementTags, SVGElementTags } from "./types/dom-expressions-jsx"
import { isComponentClass, isObject, isString } from "./util"


type DataKeys = `data-${string}`

// DOM Elements
export function jsx<THtmlTag extends keyof HTMLElementTagNameMap, TElement extends HTMLElementTagNameMap[THtmlTag]>(
    type: THtmlTag,
    props?: (HTMLElementTags[THtmlTag] & Record<DataKeys, string | number>) | null
): TElement
export function jsx<TSVGTag extends (keyof SVGElementTagNameMap & keyof SVGElementTags), TElement extends SVGElementTagNameMap[TSVGTag]>(
    type: TSVGTag,
    props?: (SVGElementTags[TSVGTag] & Record<DataKeys, string | number>) | null
): TElement
export function jsx(
    type: string,
    props?: (ElementAttributes<JSXElement> & Record<DataKeys, string | number>) | null
): JSXElement
// Custom components
//export function jsx<P extends {}, TElement extends JSXElement = JSXElement>(
//    type: ComponentType<P, TElement>,
//    props?: P & { children?: ComponentChildren; ref?: Ref<TElement> } | null,
//): TElement
// eslint-disable-next-line @typescript-eslint/no-unused-vars
export function jsx(tag: any, props?: { children?: ComponentChildren, [key: string]: any }) {

    let { children, ...attr } = props || {};
    let ns = attr.namespaceURI;
    if (!ns) {
        if (svgOnlyTags.has(tag))
            ns = SVGNamespace;
        else if (mathMLOnlyTags.has(tag))
            ns = MathMLNamespace;
        else
            ns = currentNamespaceURI();
    }
    let node: HTMLElement | SVGElement
    if (isString(tag)) {
        node = ns
            ? document.createElementNS(ns, tag)
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
