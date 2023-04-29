import { localText } from "./localtext";
import $ from "@optionaldeps/jquery";

/**
 * Adds an empty option to the select.
 * @param select the select element
 */
export function addEmptyOption(select: JQuery | HTMLSelectElement) {
    addOption(select, '', localText("Controls.SelectEditor.EmptyItemText"));
}

/**
 * Adds an option to the select.
 */
export function addOption(select: JQuery | HTMLSelectElement, key: string, text: string) {
    $('<option/>').attr("value", key ?? "").text(text ?? "").appendTo(select);
}

/** @obsolete use htmlEncode as it also encodes quotes */
export const attrEncode = htmlEncode;

/** Clears the options in the select element */
export function clearOptions(select: JQuery) {
    select.html('');
}

/**
 * Finds the first element with the given relative id to the source element.
 * It can handle underscores in the source element id.
 * @param element the source element
 * @param relativeId the relative id to the source element
 * @param context the context element (optional)
 * @returns the element with the given relative id to the source element.
 */
export function findElementWithRelativeId(element: JQuery, relativeId: string, context?: HTMLElement): JQuery;
/**
 * Finds the first element with the given relative id to the source element.
 * It can handle underscores in the source element id.
 * @param element the source element
 * @param relativeId the relative id to the source element
 * @param context the context element (optional)
 * @returns the element with the given relative id to the source element.
 */
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

    let fromId = from.id ?? "";
    while (true) {
        var res = context?.querySelector("#" + fromId + relativeId);
        
        if (!res && noContext)
            res = doc?.getElementById(fromId + relativeId);

        if (!res && fromId.length) {
            res = context?.querySelector("#" + fromId + "_" + relativeId);
            if (!res && noContext)
                res = doc?.getElementById(fromId + "_" + relativeId);
        }

        if (res || !fromId.length)
            return isJQuery ? $(res ?? null) : (res ?? null);

        let idx = fromId.lastIndexOf('_');
        if (idx <= 0)
            fromId = "";
        else
            fromId = fromId.substring(0, idx);
    }
}

const esc: Record<string, string> = {
    '<': '&lt;',
    '>': '&gt;',
    '"': '&quot;',
    "'": "&#39;",
    '&': '&amp;',
}

function escFunc(a: string): string {
    return esc[a];
}

/**
 * Html encodes a string (encodes single and double quotes, & (ampersand), > and < characters)
 * @param s String (or number etc.) to be HTML encoded
 */
export function htmlEncode(s: any): string {
    if (s == null)
        return '';

    if (typeof s !== "string")
        s = "" + s;

    return s.replace(/[<>"'&]/g, escFunc)
}

/**
 * Creates a new DIV and appends it to the body.
 * @returns the new DIV element.
 */
export function newBodyDiv(): JQuery {
    return $('<div/>').appendTo(document.body);
}

/**
 * Returns the outer HTML of the element.
 */
export function outerHtml(element: JQuery) {
    return $('<i/>').append(element.eq(0).clone()).html();
}


/** 
 * Toggles the class on the element handling spaces like jQuery addClass does.
 * @param el the element
 * @param cls the class to toggle
 * @param remove if true, the class will be added, if false the class will be removed, otherwise it will be toggled.
 */
export function toggleClass(el: Element, cls: string, remove?: boolean) {
    if (cls == null || !cls.length)
        return;

    if (cls.indexOf(' ') < 0) {
        el.classList.toggle(cls, remove);
        return;
    }

    var k = cls.split(' ').map(x => x.trim()).filter(x => x.length);
    for (var a of k)
        el.classList.toggle(a, remove);
}
