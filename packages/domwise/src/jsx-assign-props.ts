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

function coerceBoolean(value: any): boolean {
    // matches the old attribute-path behavior: any present, non-false value
    // checks the control (e.g. 1, "yes", "checked", {})
    return value != null && value !== false && value !== "false";
}

function setNamespacedAttribute(node: JSXElement, ns: string, key: string, value: any): void {
    const qualified = normalizeAttribute(key, ":");
    if (value == null || value === false) {
        // `null`/`undefined`/`false` remove the attribute instead of
        // stringifying it (e.g. `xlink:href="undefined"`)
        node.removeAttributeNS(ns, qualified.substring(qualified.indexOf(":") + 1));
        node.removeAttribute(qualified);
        return;
    }
    node.setAttributeNS(ns, qualified, value);
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
            if (node instanceof window.HTMLSelectElement) {
                if (node.multiple && Array.isArray(value)) {
                    const values = value.map(v => String(v));
                    node.querySelectorAll("option")
                        .forEach(option => (option.selected = values.includes(option.value)));
                } else {
                    // a null with no previous value is a no-op: leave the
                    // browser's default selection alone
                    if (value == null && prev == null)
                        return;
                    node.value = value == null ? "" : value;
                }
                return;
            } else if (node instanceof window.HTMLTextAreaElement ||
                (node instanceof window.HTMLInputElement && (node as HTMLInputElement).type !== "file")) {
                // `value` is a property for text-like inputs/textarea: an
                // attribute binding stops reaching the field once the user
                // edits it (or when the value is reset through a signal).
                // A `null` with no previous value is a no-op (uncontrolled).
                if (value == null && prev == null)
                    return;
                const stringValue = value == null ? "" : String(value);
                (node as HTMLInputElement | HTMLTextAreaElement).value = stringValue;
                if (node instanceof window.HTMLInputElement) {
                    // keep the attribute in sync so serialized markup, CSS
                    // [value=...] and form.reset() reflect the current value
                    node.setAttribute("value", stringValue);
                }
                return;
            }
            if (value == null) {
                if (prev != null)
                    node.removeAttribute("value");
                return;
            }
            // other elements (option, button, li, progress, ...): attribute
            break;

        case "checked":
            if (node instanceof window.HTMLInputElement) {
                const checked = coerceBoolean(value);
                node.checked = checked;
                if (checked)
                    node.setAttribute("checked", "");
                else
                    node.removeAttribute("checked");
                return;
            }
            break;

        case "indeterminate":
            if (node instanceof window.HTMLInputElement) {
                node.indeterminate = coerceBoolean(value);
                return;
            }
            break;

        case "muted":
            if (node instanceof window.HTMLMediaElement) {
                const muted = coerceBoolean(value);
                node.muted = muted;
                if (muted)
                    node.setAttribute("muted", "");
                else
                    node.removeAttribute("muted");
                return;
            }
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
            const onRegKey = (eventName: string) => "domwise:onevent:" + (useCapture ? "capture:" : "") + eventName;
            if (prev != null) {
                Object.entries(prev).forEach(([eventName, eventHandler]) => {
                    if (value == null || value[eventName] !== eventHandler) {
                        node.removeEventListener(eventName, eventHandler as any, useCapture);
                        removeDisposingListener(node, null, onRegKey(eventName));
                    }
                });
            }
            if (value != null) {
                Object.entries(value).forEach(([eventName, eventHandler]) => {
                    if (prev == null || prev[eventName] !== eventHandler) {
                        node.addEventListener(eventName, eventHandler as any, useCapture);
                        // drop any prior disposer for the same event/phase so
                        // repeated assignments don't accumulate entries
                        removeDisposingListener(node, null, onRegKey(eventName));
                        addDisposingListener(node, () => node.removeEventListener(eventName, eventHandler as any, useCapture), onRegKey(eventName));
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
            setNamespacedAttribute(node, XLinkNamespace, key, value);
            return;

        case "xmlnsXlink":
            if (value == null || value === false)
                node.removeAttribute("xmlns:xlink");
            else
                node.setAttribute("xmlns:xlink", value);
            return;

        case "xmlBase":
        case "xmlLang":
        case "xmlSpace":
            setNamespacedAttribute(node, XMLNamespace, key, value);
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

    if (prev != null) {
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

// Standard events that some engines don't expose as an `on*` property, so the
// runtime property probe in `isStandardEvent` misses them (Chrome lacks
// focusin/focusout, composition* and touch*; jsdom lacks those plus more).
// Compressed as a regex to keep the bundle small; only consulted when the
// probe fails.
const fallbackEventNameRe =
    /^(?:animation(?:cancel|end|iteration|start)|before(?:copy|cut|paste|xrselect)|command|composition(?:end|start|update)|contentvisibilityautostatechange|dragexit|focus(?:in|out)|fullscreen(?:change|error)|gamepad(?:dis)?connected|page(?:reveal|swap)|scrollsnapchang(?:e|ing)|select(?:ionchange|start)|touch(?:cancel|end|move|start)|transition(?:cancel|end|run|start))$/;

function isStandardEvent(name: string): boolean {
    const prop = "on" + name;
    // HTMLElement.prototype inherits Element.prototype, so it covers both
    return (typeof window !== "undefined" && prop in window) ||
        (typeof HTMLElement !== "undefined" && prop in HTMLElement.prototype) ||
        fallbackEventNameRe.test(name);
}

function getEventName(key: string, attribute: string): { eventName: string, useCapture: boolean } {
    // `attribute` is the lowercased JSX key, e.g. "onfocusin"
    const name = attribute.substring(2);
    if (isStandardEvent(name))
        return { eventName: name, useCapture: false };

    // e.g. "onclickcapture" -> capture-phase "click". Standard events whose
    // own name ends in "capture" (gotpointercapture / lostpointercapture) were
    // already returned above, so they are not mistaken for capture phase.
    const captureSuffix = "capture";
    if (name.endsWith(captureSuffix))
        return { eventName: name.substring(0, name.length - captureSuffix.length), useCapture: true };

    // custom event: trim the "on" prefix, lowercase the first character and
    // keep the rest of the case (e.g. "onMyCustomEvent" -> "myCustomEvent")
    return { eventName: attribute[2] + key.slice(3), useCapture: false };
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
