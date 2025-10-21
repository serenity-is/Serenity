import { assignClass } from "./jsx-assign-class";
import { assignStyle } from "./jsx-assign-style";
import { isSignalLike, observeSignalForNode } from "./signal-util";
import { nonPresentationSVGAttributes } from "./svg-consts";
import { forEach, isObject, isVisibleChild, keys } from "./util";

const XLinkNamespace = "http://www.w3.org/1999/xlink";
const XMLNamespace = "http://www.w3.org/XML/1998/namespace";

function normalizeAttribute(s: string, separator: string) {
    return s.replace(/[A-Z]/g, match => separator + match.toLowerCase());
}

const propToAttr: Record<string, string> = {
    className: "class",
    contentEditable: "contenteditable",
    htmlFor: "for",
    tabIndex: "tabindex",
    spellCheck: "spellcheck",
    readOnly: "readonly",
    maxLength: "maxlength"
}

function assignProp(node: Element & HTMLOrSVGElement, key: string, value: any, prev?: any) {

    const propAttr = propToAttr[key];
    if (propAttr) {
        key = propAttr;
    }

    switch (key) {
        case "dataset":
            if (prev != null) {
                forEach(prev, (v, k) => {
                    if (v != null) {
                        delete node.dataset[k];
                    }
                });
            }
            forEach(value, (dataValue, dataKey) => {
                if (dataValue != null) {
                    node.dataset[dataKey] = dataValue
                }
            })
            return;

        case "innerHTML":
        case "innerText":
        case "textContent":
            if (isVisibleChild(value)) {
                (node as any)[key] = value
            }
            else if (isVisibleChild(prev)) {
                // clear only when the previous value was visible
                (node as any)[key] = ""
            }
            return;

        case "dangerouslySetInnerHTML":
            if (isObject(value)) {
                node.innerHTML = value["__html"]
            }
            else if (isObject(prev)) {
                // clear only when the previous value was visible
                node.innerHTML = ""
            }
            return;

        case "value":
            if (value == null) {
                if (prev != null) {
                    (node as any)[key] = null;
                }
                return;
            }
            else if (node instanceof window.HTMLSelectElement) {
                return;
            } else if (node instanceof window.HTMLTextAreaElement) {
                node.value = value
                return;
            }
            // use attribute for other elements
            break;

        case "class":
            assignClass(node, value, prev);
            return;

        case "spellcheck":
            (node as HTMLInputElement).spellcheck = value === "" || value === true || value === "true" ? true : (value === false || value === "false") ? false : value;
            return;

        case "draggable":
            if (value === false || value === "false")
                value = "false";
            else if (value === "")
                value = null;
            else if (value === true || value === "true")
                value = "true";
            break;

        case "contenteditable":
            // these attributes are special in that they support
            // pseudo-boolean values "true" and "false"
            if (value === false)
                value = "false";
            else if (value === "" || value === true)
                value = "true";
            break;

        case "ref":
        case "namespaceURI":
            return;

        case "style":
            assignStyle(node, value, prev)
            return;

        case "on":
        case "onCapture":
            const useCapture = key === "onCapture";
            if (prev != null) {
                forEach(prev, (eventHandler, eventName) => {
                    if (value == null || value[eventName] !== eventHandler) {
                        node.removeEventListener(eventName, eventHandler, useCapture);
                    }
                });
            }
            forEach(value, (eventHandler, eventName) => {
                if (prev == null || prev[eventName] !== eventHandler) {
                    node.addEventListener(eventName, eventHandler, useCapture);
                }
            });
            return;

        case "xlinkActuate":
        case "xlinkArcrole":
        case "xlinkHref":
        case "xlinkRole":
        case "xlinkShow":
        case "xlinkTitle":
        case "xlinkType":
            node.setAttributeNS(XLinkNamespace, normalizeAttribute(key, ":"), value);
            return;

        case "xmlnsXlink":
            node.setAttribute("xmlns:xlink", value);
            return;

        case "xmlBase":
        case "xmlLang":
        case "xmlSpace":
            node.setAttributeNS(XMLNamespace, normalizeAttribute(key, ":"), value);
            return;

        // fallthrough
    }

    if (typeof value === "function") {
        if (key[0] === "o" && key[1] === "n") {
            if (prev != null && prev === value)
                return;

            let attribute = key.toLowerCase();
            const useCapture = attribute.endsWith("capture");
            if (attribute === "ondoubleclick") {
                attribute = "ondblclick";
            } else if (useCapture && attribute === "ondoubleclickcapture") {
                attribute = "ondblclickcapture";
            }

            let eventName;
            if (!useCapture && ((node as any)[attribute] === null || (prev != null && (node as any)[attribute] === prev))) {
                // use property when possible jsx-dom PR #17
                (node as any)[attribute] = value;
            } else if (useCapture) {
                eventName = attribute.substring(2, attribute.length - 7);
                if (prev != null) {
                    node.removeEventListener(eventName, prev);
                }
                node.addEventListener(eventName, value, true);
            } else {
                if (attribute in window) {
                    // standard event
                    // the JSX attribute could have been "onMouseOver" and the
                    // member name "onmouseover" is on the window's prototype
                    // so let's add the listener "mouseover", which is all lowercased
                    const standardEventName = attribute.substring(2);
                    eventName = standardEventName;
                } else {
                    // custom event
                    // the JSX attribute could have been "onMyCustomEvent"
                    // so let's trim off the "on" prefix and lowercase the first character
                    // and add the listener "myCustomEvent"
                    // except for the first character, we keep the event name case
                    const customEventName = attribute[2] + key.slice(3);
                    eventName = customEventName;
                }
                if (prev != null) {
                    node.removeEventListener(eventName, prev);
                }
                node.addEventListener(eventName, value);
            }
        }
        else {
            console.warn(`A function was provided for JSX ${node.tagName} element ${key} which does not start with "on":`, value, node.tagName);
        }
        return;
    }

    if (isObject(prev)) {
        if (prev === value) {
            return;
        }
        delete (node as any)[key];
    }

    if (prev === true) {
        if (prev === value) {
            return;
        }
        node.removeAttribute(key);
    }

    if (prev !== false && prev != null) {
        if (prev === value) {
            return;
        }

        if (node instanceof SVGElement && !nonPresentationSVGAttributes.test(key)) {
            node.removeAttributeNS(normalizeAttribute(key, "-"), value);
        } else {
            node.removeAttribute(key);
        }
    }

    if (isObject(value)) {
        (node as any)[key] = value;
        return;
    }

    if (value === true) {
        node.setAttribute(key, "");
        return;
    }

    if (value !== false && value != null) {
        if (node instanceof SVGElement && !nonPresentationSVGAttributes.test(key)) {
            node.setAttribute(normalizeAttribute(key, "-"), value);
        } else {
            node.setAttribute(key, value);
        }
    }
}

export function assignProps(node: HTMLElement | SVGElement, props: Record<string, any>) {
    for (const key of keys(props)) {
        let value = props[key];
        if (isSignalLike(value)) {
            observeSignalForNode(value, node, (val, prev) => assignProp(node, key, val, prev));
        }
        else {
            assignProp(node, key, value);
        }
    }
    return node;
}