import { localText } from "./localtext";
import $ from "@optionaldeps/jquery";

export function addEmptyOption(select: JQuery) {
    addOption(select, '', localText("Controls.SelectEditor.EmptyItemText"));
}

export function addOption(select: JQuery | HTMLSelectElement, key: string, text: string) {
    $('<option/>').attr("value", key ?? "").text(text ?? "").appendTo(select);
}

/** @obsolete use htmlEncode as it also encodes quotes */
export const attrEncode = htmlEncode;

export function clearOptions(select: JQuery) {
    select.html('');
}

export function findElementWithRelativeId(element: JQuery, relativeId: string, context?: HTMLElement): JQuery;
export function findElementWithRelativeId(element: HTMLElement, relativeId: string, context?: HTMLElement): HTMLElement;
export function findElementWithRelativeId(element: JQuery | HTMLElement, relativeId: string, context?: HTMLElement): JQuery | Element {

    const isJQuery = element instanceof $ && element != null;
    const from: HTMLElement = isJQuery ? (element as JQuery).get(0) : element as HTMLElement;
    const doc = typeof document === "undefined" ? null : document;

    if (from == null)
        return null;

    var noContext = false;
    if (context === undefined) {
        context = from.getRootNode() as HTMLElement;
        noContext = true;
    }

    let elementId = (element as HTMLElement).id ?? "";
    while (true) {
        var res = context?.querySelector("#" + elementId + relativeId);
        
        if (!res && noContext)
            res = doc?.getElementById(elementId + relativeId);

        if (!res && elementId.length) {
            res = context?.querySelector("#" + elementId + "_" + relativeId);
            if (!res && noContext)
                res = doc?.getElementById(elementId + "_" + relativeId);
        }

        if (res || !elementId.length)
            return isJQuery ? $(res ?? null) : (res ?? null);

        let idx = elementId.lastIndexOf('_');
        if (idx <= 0)
            elementId = "";
        else
            elementId = elementId.substring(0, idx);
    }
}

const esc: Record<string, string> = {
    '<': '&lt;',
    '>': '&gt;',
    '"': '&quot;',
    "'": "&#39;",
    '&': '&amp;',
}

function escFunc(a: string) {
    return esc[a];
}

/**
 * Html encodes a string (encodes single and double quotes, & (ampersand), > and < characters)
 * @param s String to be HTML encoded
 */
export function htmlEncode(s: any): string {
    if (s == null)
        return '';

    if (typeof s !== "string")
        s = "" + s;

    return s.replace(/[<>"'&]/g, escFunc)
}

export function newBodyDiv(): JQuery {
    return $('<div/>').appendTo(document.body);
}

export function outerHtml(element: JQuery) {
    return $('<i/>').append(element.eq(0).clone()).html();
}
