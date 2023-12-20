import sQuery from "@optionaldeps/squery";
import { htmlEncode, isArrayLike, localText } from "@serenity-is/base";

export function isJQueryReal(val: any): val is JQuery {
    return isArrayLike(val) && !(val as any).isMock && typeof (val as JQuery).outerHeight === "function";
}

/**
 * Adds an empty option to the select.
 * @param select the select element
 */
export function addEmptyOption(select: ArrayLike<HTMLElement> | HTMLSelectElement) {
    addOption(select, '', localText("Controls.SelectEditor.EmptyItemText"));
}

/**
 * Adds an option to the select.
 */
export function addOption(select: ArrayLike<HTMLElement> | HTMLSelectElement, key: string, text: string) {
    sQuery('<option/>').attr("value", key ?? "").text(text ?? "").appendTo(select as any);
}

/** @deprecated use htmlEncode as it also encodes quotes */
export const attrEncode = htmlEncode;

/** Clears the options in the select element */
export function clearOptions(select: ArrayLike<HTMLElement>) {
    (select as any).html('');
}

/**
 * Finds the first element with the given relative id to the source element.
 * It can handle underscores in the source element id.
 * @param element the source element
 * @param relativeId the relative id to the source element
 * @param context the context element (optional)
 * @returns the element with the given relative id to the source element.
 */
export function findElementWithRelativeId(element: ArrayLike<HTMLElement>, relativeId: string, context?: HTMLElement): JQuery;
/**
 * Finds the first element with the given relative id to the source element.
 * It can handle underscores in the source element id.
 * @param element the source element
 * @param relativeId the relative id to the source element
 * @param context the context element (optional)
 * @returns the element with the given relative id to the source element.
 */
export function findElementWithRelativeId(element: HTMLElement, relativeId: string, context?: HTMLElement): HTMLElement;
export function findElementWithRelativeId(element: HTMLElement | ArrayLike<HTMLElement>, relativeId: string, context?: HTMLElement): JQuery | HTMLElement {

    const from: HTMLElement = isArrayLike(element) ? element[0] : element as HTMLElement;
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
        var res = context?.querySelector("#" + fromId + relativeId) as HTMLElement;
        
        if (!res && noContext)
            res = doc?.getElementById(fromId + relativeId);

        if (!res && fromId.length) {
            res = context?.querySelector("#" + fromId + "_" + relativeId);
            if (!res && noContext)
                res = doc?.getElementById(fromId + "_" + relativeId);
        }

        if (res || !fromId.length)
            return isArrayLike(element) ? sQuery(res ?? null) : (res ?? null);

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
    return sQuery('<div/>').appendTo(document.body);
}

/**
 * Returns the outer HTML of the element.
 */
export function outerHtml(element: ArrayLike<HTMLElement>) {
    return sQuery('<i/>').append((element as any).eq(0).clone()).html();
}
