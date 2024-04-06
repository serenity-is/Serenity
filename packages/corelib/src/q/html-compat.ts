import { htmlEncode, isArrayLike, isPromiseLike, localText } from "../base";

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
    var option = document.createElement("option");
    option.value = key ?? "";
    option.textContent = text ?? "";
    (isArrayLike(select) ? select[0] : select)?.append(option);
}

/** @deprecated use htmlEncode as it also encodes quotes */
export const attrEncode = htmlEncode;

/** Clears the options in the select element */
export function clearOptions(select: HTMLElement | ArrayLike<HTMLElement>) {
    select = isArrayLike(select) ? select[0] : select;
    if (select)
        select.innerHTML = '';
}

/**
 * Finds the first element with the given relative id to the source element.
 * It can handle underscores in the source element id.
 * @param element the source element
 * @param relativeId the relative id to the source element
 * @param context the context element (optional)
 * @returns the element with the given relative id to the source element.
 */
export function findElementWithRelativeId(element: HTMLElement | ArrayLike<HTMLElement>, relativeId: string, context?: HTMLElement): HTMLElement {

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
            return res ?? null;

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
export function newBodyDiv(): HTMLDivElement {
    var element = document.createElement("div");
    document.body.append(element);
    return element;
}

/**
 * Returns the outer HTML of the element.
 */
export function outerHtml(element: Element | ArrayLike<HTMLElement>) {
    var el = document.createElement('i');
    el.append((isArrayLike(element) ? element[0] : element).cloneNode(true));
    return el.innerHTML;
}

/**
 * Appends child at first argument to given node at second argument. 
 * From https://github.com/alex-kinokon/jsx-dom.
 * @param child Child element or elements
 * @param node Target parent element
 */
export function appendChild(child: any, node: HTMLElement) {
    if (isArrayLike(child)) {
        appendChildren(child, node)
    } else if (typeof child === "string" || typeof child === "number") {
        appendChildToNode(document.createTextNode(child.toString()), node)
    } else if (child === null) {
        appendChildToNode(document.createComment(""), node)
    } else if (child != null && child.nodeType === "number") {
        appendChildToNode(child, node);
    }
    else if (isPromiseLike(child)) {
        child.then(result => appendChild(result, node));
    }
    else
        node.append(child);

}

function appendChildren(children: any, node: HTMLElement): HTMLElement {
    for (const child of [...children]) {
        appendChild(child, node)
    }
    return node;
}

function appendChildToNode(child: any, node: HTMLElement) {
    if (node instanceof HTMLTemplateElement) {
        node.content.appendChild(child)
    } else {
        node.appendChild(child)
    }
}