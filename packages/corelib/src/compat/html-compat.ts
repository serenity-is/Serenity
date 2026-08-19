import { htmlEncode, isArrayLike, SelectEditorTexts } from "../base";

/**
 * Appends an empty (placeholder) option to a `<select>` element.
 * @param select - Target `<select>` or array-like/jQuery-like wrapper containing it.
 * @remarks Uses {@link SelectEditorTexts.EmptyItemText} as the display text and `""` as the value; delegates to {@link addOption}. Compat helper from `Q.addEmptyOption`.
 */
export function addEmptyOption(select: ArrayLike<HTMLElement> | HTMLSelectElement) {
    addOption(select, '', SelectEditorTexts.EmptyItemText);
}

/**
 * Appends an `<option>` to a `<select>` element.
 * @param select - Target `<select>` or array-like/jQuery-like wrapper containing it.
 * @param key - Value attribute for the option (`null`/`undefined` → `""`).
 * @param text - Display text for the option (`null`/`undefined` → `""`).
 * @remarks Creates an `HTMLOptionElement` via `document.createElement("option")`. No-op if the resolved select element is falsy. Compat helper from `Q.addOption`.
 */
export function addOption(select: ArrayLike<HTMLElement> | HTMLSelectElement, key: string, text: string) {
    const option = document.createElement("option");
    option.value = key ?? "";
    option.textContent = text ?? "";
    (isArrayLike(select) ? select[0] : select)?.append(option);
}

/**
 * Legacy alias for {@link htmlEncode}.
 * @deprecated Use {@link htmlEncode} directly (it also encodes quotes). Retained as `Q.attrEncode` compat shim.
 * @see {@link htmlEncode}
 */
export const attrEncode = htmlEncode;

/**
 * Removes all child options/content from a `<select>` element.
 * @param select - Target element or array-like/jQuery-like wrapper containing it.
 * @remarks Resolves array-like wrappers via `isArrayLike` and clears with `innerHTML = ''`. No-op if the resolved element is falsy. Compat helper from `Q.clearOptions`.
 */
export function clearOptions(select: HTMLElement | ArrayLike<HTMLElement>) {
    select = isArrayLike(select) ? select[0] : select;
    if (select)
        select.innerHTML = '';
}

/**
 * Resolves a sibling/related element by a suffix relative to a source element's id.
 * @param element - Source element or array-like/jQuery-like wrapper containing it.
 * @param relativeId - Suffix to append to the source id (with/without leading `_`) when searching.
 * @param context - Scope element for `querySelector`; defaults to the source element's root node. When omitted the search also falls back to `document.getElementById`.
 * @returns The matched `HTMLElement`, or `null` if the source is `null` or no match is found.
 * @remarks Tries `"#" + fromId + relativeId` then `"#" + fromId + "_" + relativeId`, progressively stripping trailing `"_segment"` segments from `fromId` until a match or exhaustion. Compat helper from `Q.findElementWithRelativeId`.
 * @example
 * findElementWithRelativeId(document.getElementById("Customer_Name"), "_City"); // finds #Customer_City if present
 */
export function findElementWithRelativeId(element: HTMLElement | ArrayLike<HTMLElement>, relativeId: string, context?: HTMLElement): HTMLElement {

    const from: HTMLElement = isArrayLike(element) ? element[0] : element as HTMLElement;
    const doc = typeof document === "undefined" ? null : document;

    if (from == null)
        return null;

    let noContext = false;
    if (context === undefined) {
        context = from.getRootNode() as HTMLElement;
        noContext = true;
    }

    let fromId = from.id ?? "";
    while (true) {
        let res = context?.querySelector("#" + fromId + relativeId) as HTMLElement;

        if (!res && noContext)
            res = doc?.getElementById(fromId + relativeId);

        if (!res && fromId.length) {
            res = context?.querySelector("#" + fromId + "_" + relativeId);
            if (!res && noContext)
                res = doc?.getElementById(fromId + "_" + relativeId);
        }

        if (res || !fromId.length)
            return res ?? null;

        let idx = fromId.lastIndexOf('_');
        if (idx <= 0)
            fromId = "";
        else
            fromId = fromId.substring(0, idx);
    }
}

/**
 * Creates a new `<div>` and appends it to `document.body`.
 * @returns The newly created and appended `HTMLDivElement`.
 * @remarks Compat helper from `Q.newBodyDiv`; prefer `document.createElement` + explicit append in new code.
 */
export function newBodyDiv(): HTMLDivElement {
    const element = document.createElement("div");
    document.body.append(element);
    return element;
}

/**
 * Returns the outer HTML markup of an element (including the element itself).
 * @param element - Target element, `Element`, or array-like/jQuery-like wrapper containing it.
 * @returns Outer HTML string. For non-Elements, clones the node into a temporary `<i>` wrapper and returns `innerHTML`; yields `""` for falsy targets.
 * @remarks Compat helper from `Q.outerHtml`; for new code prefer `element.outerHTML` directly.
 */
export function outerHtml(element: Element | ArrayLike<HTMLElement>) {
    const el = document.createElement('i');
    const target = isArrayLike(element) ? element[0] : element;
    if (target != null)
        el.append(target.cloneNode(true));
    return el.innerHTML;
}
