import type { JSXElement } from "../types";
import { addDisposingListener, removeDisposingListener } from "./disposing-listener";
import { assignClass } from "./jsx-assign-class";
import { assignStyle } from "./jsx-assign-style";
import { initPropHookSymbol } from "./prop-hook";
import { isSignalLike, observeSignal } from "./signal-util";
import { nonPresentationSVGAttributes } from "./svg-consts";
import { isObject, isPropHook, isVisibleChild } from "./util";

const XLinkNamespace = "http://www.w3.org/1999/xlink";
const XMLNamespace = "http://www.w3.org/XML/1998/namespace";

function normalizeAttribute(s: string, separator: string) {
    return s.replace(/[A-Z]/g, match => separator + match.toLowerCase());
}

const mappedKeys: Map<string, string> = new Map<string, string>([
    ["acceptCharset", "accept-charset"],
    ["autoComplete", "autocomplete"],
    ["autoFocus", "autofocus"],
    ["className", "class"],
    ["colSpan", "colspan"],
    ["contentEditable", "contenteditable"],
    ["formNoValidate", "formnovalidate"],
    ["htmlFor", "for"],
    ["httpEquiv", "http-equiv"],
    ["innerText", "textContent"],
    ["minLength", "minlength"],
    ["maxLength", "maxlength"],
    ["readOnly", "readonly"],
    ["referrerPolicy", "referrerpolicy"],
    ["rowSpan", "rowspan"],
    ["spellCheck", "spellcheck"],
    ["tabIndex", "tabindex"],
    ["onDoubleClick", "onDblClick"],
    ["onDoubleClickCapture", "onDblClickCapture"]
]);

export function assignProp(node: JSXElement, key: string, value: any, prev?: any): void {

    switch (key) {
        case "dataset":
            if (prev != null) {
                Object.entries(prev).forEach(([k, v]) => {
                    if (v != null) {
                        delete node.dataset[k];
                    }
                });
            }
            if (value != null) {
                Object.entries(value).forEach(([dataKey, dataValue]) => {
                    if (dataValue != null) {
                        node.dataset[dataKey] = dataValue as string;
                    }
                })
            }
            return;

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
                if (node.multiple && Array.isArray(value)) {
                    const values = value.map(v => String(v));
                    node.querySelectorAll("option")
                        .forEach(option => (option.selected = values.includes(option.value)));
                    return;
                }
                node.value = value;
                return;
            } else if (node instanceof window.HTMLTextAreaElement) {
                node.value = value;
                return;
            }
            // use attribute for other elements
            break;

        case "class":
            assignClass(node, value, prev);
            return;

        case "spellcheck":
            (node as HTMLElement).spellcheck = value === "" || value === true || value === "true" ? true : (value === false || value === "false") ? false : value;
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
                Object.entries(prev).forEach(([eventName, eventHandler]) => {
                    if (value == null || value[eventName] !== eventHandler) {
                        node.removeEventListener(eventName, eventHandler as any, useCapture);
                    }
                });
            }
            if (value != null) {
                Object.entries(value).forEach(([eventName, eventHandler]) => {
                    if (prev == null || prev[eventName] !== eventHandler) {
                        node.addEventListener(eventName, eventHandler as any, useCapture);
                    }
                });
            }
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

    if (key[0] === "o" && key[1] === "n" && (typeof value === "function" || typeof prev === "function")) {
        assignEventProp(node, key, value, prev);
        return;
    }

    if (typeof value === "function") {
        console.warn(`A function was provided for JSX ${node.tagName} element ${key} which does not start with "on":`, value, node.tagName);
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
            node.removeAttribute(normalizeAttribute(key, "-"));
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

export function assignProps(node: JSXElement, props: Record<string, any>): void {
    for (let [key, value] of Object.entries(props)) {

        key = mappedKeys.get(key) ?? key;

        if (isSignalLike(value)) {
            observeSignal(value, (args) => assignProp(node, key, args.newValue, args.prevValue), {
                lifecycleNode: node
            });
        }
        else if (isPropHook(value)) {
            (value as any)[initPropHookSymbol](node, key);
        }
        else {
            assignProp(node, key, value);
        }
    }
}

function getEventName(key: string, attribute: string): { eventName: string, useCapture: boolean } {
    const useCapture = attribute.endsWith("capture");
    if (useCapture) {
        return { eventName: attribute.substring(2, attribute.length - 7), useCapture };
    }
    if (attribute in window) {
        // standard event
        // the JSX attribute could have been "onMouseOver" and the
        // member name "onmouseover" is on the window's prototype
        // so let's add the listener "mouseover", which is all lowercased
        return { eventName: attribute.substring(2), useCapture };
    }
    // custom event
    // the JSX attribute could have been "onMyCustomEvent"
    // so let's trim off the "on" prefix and lowercase the first character
    // and add the listener "myCustomEvent"
    // except for the first character, we keep the event name case
    return { eventName: attribute[2] + key.slice(3), useCapture };
}

function assignEventProp(node: JSXElement, key: string, value: any, prev?: any): void {
    const attribute = key.toLowerCase();
    const current = (node as any)[attribute];
    const regKey = "domwise:event:" + attribute;
    const { eventName, useCapture } = getEventName(key, attribute);

    if (prev != null && prev !== value) {
        if (current === prev) {
            // previously installed as a DOM property, clear it directly
            (node as any)[attribute] = null;
        } else if (useCapture) {
            node.removeEventListener(eventName, prev, true);
        } else {
            node.removeEventListener(eventName, prev);
        }
        removeDisposingListener(node, null, regKey);
    }

    if (typeof value !== "function") {
        // no new handler, previous one (if any) was cleared above
        return;
    }

    if (prev === value) {
        return;
    }

    if (!useCapture && (current === null || (prev != null && current === prev))) {
        // use property when possible jsx-dom PR #17
        (node as any)[attribute] = value;
        addDisposingListener(node, () => { (node as any)[attribute] = null; }, regKey);
    } else if (useCapture) {
        node.addEventListener(eventName, value, true);
        addDisposingListener(node, () => { node.removeEventListener(eventName, value, true); }, regKey);
    } else {
        node.addEventListener(eventName, value);
        addDisposingListener(node, () => { node.removeEventListener(eventName, value); }, regKey);
    }
}
