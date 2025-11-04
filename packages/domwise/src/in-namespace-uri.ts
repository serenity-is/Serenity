import { MathMLNamespace } from "./mathml-consts";
import { SVGNamespace } from "./svg-consts";
import type { ComponentChildren } from "../types";

let namespaceURI: string | null = null;

export function currentNamespaceURI(value?: string): string {
    if (!arguments.length) {
        return namespaceURI;
    }
    const prev = namespaceURI;
    namespaceURI = value;
    return prev;
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