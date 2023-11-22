import { htmlEncode } from "@serenity-is/base";
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
