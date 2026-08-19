import type { ComponentChildren } from "../types";
import { MathMLNamespace } from "./mathml-consts";
import { SVGNamespace } from "./svg-consts";

const jsxNamespaceURISymbol = Symbol.for("Serenity.jsxNamespaceURI");

/**
 * Gets or sets the ambient JSX namespace URI used for `createElement`/`jsx`.
 *
 * Stored on `globalThis` under `Serenity.jsxNamespaceURI`. When the active
 * namespace is `"http://www.w3.org/2000/svg"` (or MathML), elements created
 * without an explicit `namespaceURI` prop are created via `createElementNS`.
 *
 * @param value - When arguments are supplied, the namespace is set to this
 * value (use `null` to reset to the HTML namespace). When called with no
 * arguments the current value is simply returned.
 * @returns The current namespace URI (no-arg call), or the previous value
 * (setter call). May be `null`/`undefined` when no override is active.
 */
export function currentNamespaceURI(value?: string | null | undefined): string | null | undefined {
    const current = (globalThis as any)[jsxNamespaceURISymbol];
    if (!arguments.length) {
        return current;
    }
    (globalThis as any)[jsxNamespaceURISymbol] = value;
    return current;
}

/**
 * Executes a children factory within a scoped namespace URI.
 *
 * Temporarily sets {@link currentNamespaceURI} to `namespaceURI` for the
 * duration of `children()`, then restores the previous value (even if the
 * factory throws). This lets you create SVG/MathML subtrees imperatively
 * without setting `namespaceURI` on every element.
 *
 * @param namespaceURI - Namespace URI to activate, or `null` for the HTML namespace.
 * @param children - Factory that produces the children to render in the given namespace.
 * @returns The children returned by the factory.
 * @example
 * ```tsx
 * const icon = inNamespaceURI(SVGNamespace, () => <><circle r={10} /><path d="M0 0" /></>);
 * ```
 */
export function inNamespaceURI(namespaceURI: string | null, children: () => ComponentChildren): ComponentChildren {
    const prev = currentNamespaceURI(namespaceURI);
    let result: ComponentChildren;
    try {
        result = children();
    }
    finally {
        currentNamespaceURI(prev);
    }
    return result
}

/**
 * Executes a children factory within the SVG namespace (`http://www.w3.org/2000/svg`).
 * Sugar over {@link inNamespaceURI} with {@link SVGNamespace}.
 * @param fn - Factory that returns children to create as SVG elements.
 * @returns The children produced by the factory.
 */
export function inSVGNamespace(fn: () => ComponentChildren): ComponentChildren {
    return inNamespaceURI(SVGNamespace, fn);
}

/**
 * Executes a children factory within the MathML namespace (`http://www.w3.org/1998/Math/MathML`).
 * Sugar over {@link inNamespaceURI} with {@link MathMLNamespace}.
 * @param fn - Factory that returns children to create as MathML elements.
 * @returns The children produced by the factory.
 */
export function inMathMLNamespace(fn: () => ComponentChildren): ComponentChildren {
    return inNamespaceURI(MathMLNamespace, fn);
}

/**
 * Executes a children factory within the HTML namespace (clears any active SVG/MathML override).
 * Sugar over {@link inNamespaceURI} with `null`.
 * @param fn - Factory that returns children to create as HTML elements.
 * @returns The children produced by the factory.
 */
export function inHTMLNamespace(fn: () => ComponentChildren): ComponentChildren {
    return inNamespaceURI(null, fn);
}