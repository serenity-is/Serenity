import type { ComponentChildren } from "../types";
import { MathMLNamespace } from "./mathml-consts";
import { SVGNamespace } from "./svg-consts";

const jsxNamespaceURISymbol = Symbol.for("Serenity.jsxNamespaceURI");

/**
 * Gets or sets the current JSX namespace URI.
 * When called without arguments, returns the current namespace URI.
 * When called with a value, sets the namespace and returns the previous value.
 * @param value - If provided, sets the namespace URI to this value.
 * @returns The current (or previous) namespace URI, or `null` / `undefined`.
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
 * Executes a children factory within a specific namespace URI context.
 * The namespace is temporarily set for the duration of the call and restored afterwards.
 * @param namespaceURI - The namespace URI to use, or `null` for HTML namespace.
 * @param children - A factory function that returns children to be created in the given namespace.
 * @returns The children produced by the factory.
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
 * Executes a children factory within the SVG namespace.
 * @param fn - A factory function that returns children.
 * @returns The children produced by the factory.
 */
export function inSVGNamespace(fn: () => ComponentChildren): ComponentChildren {
    return inNamespaceURI(SVGNamespace, fn);
}

/**
 * Executes a children factory within the MathML namespace.
 * @param fn - A factory function that returns children.
 * @returns The children produced by the factory.
 */
export function inMathMLNamespace(fn: () => ComponentChildren): ComponentChildren {
    return inNamespaceURI(MathMLNamespace, fn);
}

/**
 * Executes a children factory within the HTML namespace (explicitly setting namespace to `null`).
 * @param fn - A factory function that returns children.
 * @returns The children produced by the factory.
 */
export function inHTMLNamespace(fn: () => ComponentChildren): ComponentChildren {
    return inNamespaceURI(null, fn);
}