import type { ComponentChildren, ComponentType, ElementAttributes, HTMLElementTags, JSXElement, Ref, SVGElementTags } from "../types"
import { initComponentClass } from "./component"
import { currentNamespaceURI } from "./in-namespace-uri"
import { appendChildren } from "./jsx-append-children"
import { assignProps } from "./jsx-assign-props"
import { MathMLNamespace, mathMLOnlyTags } from "./mathml-consts"
import { setRef } from "./ref"
import { SVGNamespace, svgOnlyTags } from "./svg-consts"
import { isSignalLike } from "./signal-util"
import { isComponentClass, isObject, isString } from "./util"

type DataKeys = `data-${string}`

/**
 * Creates a JSX element. This is the automatic JSX factory used by the
 * compiler (imported as `jsx` and `jsxs`). Handles HTML/SVG/MathML elements
 * and custom function or class components.
 *
 * When `type` is a string, a real DOM element is created (with namespace
 * auto-detection for SVG/MathML), props are assigned via `assignProps`, and
 * children are appended. `select[value]` signals are resolved and applied.
 * When `type` is a function/class, it is invoked or instantiated as a
 * component and the resulting node is returned. `defaultProps` are respected
 * and `ref` is forwarded via {@link setRef}.
 *
 * Unlike {@link createElement} / `h`, children are expected inside `props`
 * (`props.children`) rather than as rest arguments.
 *
 * @param type - HTML/SVG/MathML tag name or a component function/class.
 * @param props - Attributes/props for the element. Children are read from `props.children`; may be `null`.
 * @returns The created DOM node (or component render result).
 */
// DOM Elements
export function jsx<THtmlTag extends (keyof HTMLElementTagNameMap & keyof HTMLElementTags), TElement extends HTMLElementTagNameMap[THtmlTag]>(
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
export function jsx<P extends {}, TElement extends JSXElement = JSXElement>(
    type: ComponentType<P, TElement>,
    props?: P & { children?: ComponentChildren; ref?: Ref<TElement> } | null,
): TElement
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
            const value = isSignalLike(attr.value) ? attr.value.peek() : attr.value;
            if (attr.multiple === true && Array.isArray(value)) {
                const values = value.map((v) => String(v));
                node
                    .querySelectorAll("option")
                    .forEach(option => (option.selected = values.includes(option.value)))
            } else {
                node.value = value;
            }
        }

        setRef(attr.ref, node)
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
