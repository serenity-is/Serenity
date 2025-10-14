import { className } from "./classname";
import { setStyle } from "./set-style";
import { nonPresentationSVGAttributes } from "./svg-consts";
import { forEach, isAttributeHook, isFunction, isObject, isSignalLike, isVisibleChild, keys } from "./util";

const XLinkNamespace = "http://www.w3.org/1999/xlink";
const XMLNamespace = "http://www.w3.org/XML/1998/namespace";

function normalizeAttribute(s: string, separator: string) {
    return s.replace(/[A-Z]/g, match => separator + match.toLowerCase());
}

const propToAttr: Record<string, string> = {
    className: "class",
    htmlFor: "for",
    tabIndex: "tabindex",
    spellCheck: "spellcheck",
    readOnly: "readonly",
    maxLength: "maxlength"
}

function setProperty(node: Element & HTMLOrSVGElement, key: string, value: any, prevValue?: any) {

    const propAttr = propToAttr[key];
    if (propAttr) {
        key = propAttr;
    }

    switch (key) {
        case "dataset":
            if (prevValue != null) {
                forEach(prevValue, (v, k) => {
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
            else if (isVisibleChild(prevValue)) {
                // clear only when the previous value was visible
                (node as any)[key] = ""
            }
            return;

        case "dangerouslySetInnerHTML":
            if (isObject(value)) {
                node.innerHTML = value["__html"]
            }
            else if (isObject(prevValue)) {
                // clear only when the previous value was visible
                node.innerHTML = ""
            }
            return;

        case "value":
            if (value == null) {
                if (prevValue != null) {
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

        case "spellcheck":
            (node as HTMLInputElement).spellcheck = value;
            return;

        case "class":
            // ideally we should check prevValue and toggle only changed classes
            // to preserve externally added classes but we already have
            // useClassList for that purpose. maybe in future we can implement
            // a better class merging algorithm here.
            if (value == null || value === false) {
                node.removeAttribute("class");
                return;
            } else if (isFunction(value)) {
                value(node)
            } else {
                node.setAttribute("class", className(value));
            }
            return;

        case "ref":
        case "namespaceURI":
            return;

        case "style":
            // again ideally we should diff prevValue and value
            // and only set changed properties but for now we just
            // set the whole style object
            setStyle(node, value)
            return;

        case "on":
        case "onCapture":
            const useCapture = key === "onCapture";
            if (prevValue != null) {
                forEach(prevValue, (eventHandler, eventName) => {
                    if (value == null || value[eventName] !== eventHandler) {
                        node.removeEventListener(eventName, eventHandler, useCapture);
                    }
                });
            }
            forEach(value, (eventHandler, eventName) => {
                if (prevValue == null || prevValue[eventName] !== eventHandler) {
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

    if (isFunction(value)) {
        if (key[0] === "o" && key[1] === "n") {
            if (prevValue != null && prevValue === value)
                return;

            let attribute = key.toLowerCase();
            const useCapture = attribute.endsWith("capture");
            if (attribute === "ondoubleclick") {
                attribute = "ondblclick";
            } else if (useCapture && attribute === "ondoubleclickcapture") {
                attribute = "ondblclickcapture";
            }

            let eventName;
            if (!useCapture && ((node as any)[attribute] === null || (prevValue != null && (node as any)[attribute] === prevValue))) {
                // use property when possible PR #17
                (node as any)[attribute] = value;
            } else if (useCapture) {
                eventName = attribute.substring(2, attribute.length - 7);
                if (prevValue != null) {
                    node.removeEventListener(eventName, prevValue);
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
                if (prevValue != null) {
                    node.removeEventListener(eventName, prevValue);
                }
                node.addEventListener(eventName, value);
            }
        }
    }

    if (isObject(prevValue)) {
        if (prevValue === value) {
            return;
        }
        delete (node as any)[key];
    }

    if (prevValue === true) {
        if (prevValue === value) {
            return;
        }
        node.removeAttribute(key);
    }

    if (prevValue !== false && prevValue != null) {
        if (prevValue === value) {
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

function setPropertyWithSignal(node: Element & HTMLOrSVGElement, key: string, signal: any) {
    let prevValue = signal.peek();
    setProperty(node, key, prevValue);
    let immediate = true;
    // preact signals executes the callback immediately, so skip the first call.
    // another signal library may not do that (unsure), so we guard with
    // `immediate` flag to ensure the first call is skipped
    const dispose = signal.subscribe((newValue: any) => {
        if (!immediate) {
            setProperty(node, key, newValue, prevValue);
            prevValue = newValue;
        }
    });
    immediate = false;
    if (dispose) {
        node.addEventListener("disposing", dispose, { once: true });
    }
}

export function setProperties(node: HTMLElement | SVGElement, props: Record<string, any>) {
    for (const key of keys(props)) {
        let value = props[key];
        if (isSignalLike(value)) {
            setPropertyWithSignal(node, key, value);
        }
        else {
            setProperty(node, key, value);
        }
    }
    return node;
}