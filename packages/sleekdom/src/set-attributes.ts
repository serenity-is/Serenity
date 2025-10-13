import { className } from "./classname";
import { setStyle } from "./set-style";
import { nonPresentationSVGAttributes } from "./svg-consts";
import { forEach, isAttributeHook, isFunction, isObject, isVisibleChild, keys } from "./util";

const XLinkNamespace = "http://www.w3.org/1999/xlink";
const XMLNamespace = "http://www.w3.org/XML/1998/namespace";

function normalizeAttribute(s: string, separator: string) {
    return s.replace(/[A-Z]/g, match => separator + match.toLowerCase());
}

const propToAttr: Record<string, string> = {
    htmlFor: "for",
    tabIndex: "tabindex",
    spellCheck: "spellcheck",
    readOnly: "readonly",
    maxLength: "maxlength"
}


function setAttr(node: Element, key: string, value: string | number) {
    node.setAttribute(key, value as any);
}

function setAttrNS(node: Element, namespace: string, key: string, value: string | number) {
    node.setAttributeNS(namespace, key, value as any);
}

function setAttribute(key: string, value: any, node: Element & HTMLOrSVGElement) {

    const propAttr = propToAttr[key];
    if (propAttr) {
        key = propAttr;
    }

    switch (key) {
        case "dataset":
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
            return;

        case "dangerouslySetInnerHTML":
            if (isObject(value)) {
                node.innerHTML = value["__html"]
            }
            return;

        case "value":
            if (value == null || node instanceof window.HTMLSelectElement) {
                // skip nullish values
                // for `<select>` apply value after appending `<option>` elements
                return;
            } else if (node instanceof window.HTMLTextAreaElement) {
                node.value = value
                return;
            }
            // use attribute for other elements
            break;

        case "spellcheck":
            (node as HTMLInputElement).spellcheck = value;
            return;

        case "class":
        case "className":
            if (isFunction(value)) {
                value(node)
            } else {
                setAttr(node, "class", className(value));
            }
            return;

        case "ref":
        case "namespaceURI":
            return;

        case "style":
            setStyle(node, value)
            return;

        case "on":
        case "onCapture":
            const useCapture = key === "onCapture";
            forEach(value, (eventHandler, eventName) => {
                node.addEventListener(eventName, eventHandler, useCapture);
            })
            return;

        case "xlinkActuate":
        case "xlinkArcrole":
        case "xlinkHref":
        case "xlinkRole":
        case "xlinkShow":
        case "xlinkTitle":
        case "xlinkType":
            setAttrNS(node, XLinkNamespace, normalizeAttribute(key, ":"), value);
            return;

        case "xmlnsXlink":
            setAttr(node, normalizeAttribute(key, ":"), value);
            return;

        case "xmlBase":
        case "xmlLang":
        case "xmlSpace":
            setAttrNS(node, XMLNamespace, normalizeAttribute(key, ":"), value);
            return;

        // fallthrough
    }

    if (isFunction(value)) {
        if (key[0] === "o" && key[1] === "n") {
            let attribute = key.toLowerCase();
            const useCapture = attribute.endsWith("capture");
            if (attribute === "ondoubleclick") {
                attribute = "ondblclick";
            } else if (useCapture && attribute === "ondoubleclickcapture") {
                attribute = "ondblclickcapture";
            }

            if (!useCapture && (node as any)[attribute] === null) {
                // use property when possible PR #17
                (node as any)[attribute] = value;
            } else if (useCapture) {
                node.addEventListener(attribute.substring(2, attribute.length - 7), value, true);
            } else {
                let eventName;
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
                node.addEventListener(eventName, value);
            }
        }
    } else if (isObject(value)) {
        (node as any)[key] = value;
    } else if (value === true) {
        setAttr(node, key, "");
    } else if (value !== false && value != null) {
        if (node instanceof SVGElement && !nonPresentationSVGAttributes.test(key)) {
            setAttr(node, normalizeAttribute(key, "-"), value);
        } else {
            setAttr(node, key, value);
        }
    }
}

export function setAttributes(attr: object, node: HTMLElement | SVGElement) {
    for (const key of keys(attr)) {
        let value: any = attr[key];
        if (isAttributeHook(value)) {
            value = value.jsxDomAttributeHook(node, key);
        }
        setAttribute(key, value, node);
    }
    return node;
}