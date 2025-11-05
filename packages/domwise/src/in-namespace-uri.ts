import type { ComponentChildren } from "../types";
import { MathMLNamespace } from "./mathml-consts";
import { SVGNamespace } from "./svg-consts";

const jsxNamespaceURISymbol = Symbol.for("Serenity.jsxNamespaceURI");

export function currentNamespaceURI(value?: string): string {
    const current = (globalThis as any)[jsxNamespaceURISymbol] as string | undefined;
    if (!arguments.length) {
        return current;
    }
    (globalThis as any)[jsxNamespaceURISymbol] = value;
    return current;
}

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

export function inSVGNamespace(fn: () => ComponentChildren): ComponentChildren {
    return inNamespaceURI(SVGNamespace, fn);
}

export function inMathMLNamespace(fn: () => ComponentChildren): ComponentChildren {
    return inNamespaceURI(MathMLNamespace, fn);
}

export function inHTMLNamespace(fn: () => ComponentChildren): ComponentChildren {
    return inNamespaceURI(null, fn);
}