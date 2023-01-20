import { localText } from "./localtext";
import { isEmptyOrNull } from "./strings";

export function addOption(select: JQuery, key: string, text: string) {
    $('<option/>').val(key).text(text).appendTo(select);
}

export function addEmptyOption(select: JQuery) {
    addOption(select, '', localText("Controls.SelectEditor.EmptyItemText"));
}

export function clearOptions(select: JQuery) {
    select.html('');
}

export function findElementWithRelativeId(element: JQuery, relativeId: string): JQuery {
    let elementId = element.attr('id');
    if (isEmptyOrNull(elementId)) {
        return $('#' + relativeId);
    }

    let result = $('#' + elementId + relativeId);
    if (result.length > 0) {
        return result;
    }

    result = $('#' + elementId + '_' + relativeId);

    if (result.length > 0) {
        return result;
    }

    while (true) {
        let idx = elementId.lastIndexOf('_');
        if (idx <= 0) {
            return $('#' + relativeId);
        }

        elementId = elementId.substr(0, idx);
        result = $('#' + elementId + '_' + relativeId);

        if (result.length > 0) {
            return result;
        }
    }
}

const esc = {
    '<': '&lt;',
    '>': '&gt;',
    '"': '&quot;',
    "'": "&apos;",
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

/** @obsolete use htmlEncode as it also encodes quotes */
export const attrEncode = htmlEncode;

export function newBodyDiv(): JQuery {
    return $('<div/>').appendTo(document.body);
}

export function outerHtml(element: JQuery) {
    return $('<i/>').append(element.eq(0).clone()).html();
}
