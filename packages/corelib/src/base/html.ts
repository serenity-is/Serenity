import { isArrayLike, isPromiseLike } from "./system";

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
 * Toggles the class on the element handling spaces like addClass does.
 * @param el the element
 * @param cls the class to toggle
 * @param add if true, the class will be added, if false the class will be removed, otherwise it will be toggled.
 */
export function toggleClass(el: Element, cls: string, add?: boolean) {
    if (!el || cls == null || !cls.length)
        return;

    if (cls.indexOf(' ') < 0) {
        el.classList.toggle(cls, add);
        return;
    }

    var k = cls.split(' ').map(x => x.trim()).filter(x => x.length);
    for (var a of k)
        el.classList.toggle(a, add);
}

/**
 * Adds a CSS class to the specified element.
 * 
 * @param el - The element to add the class to.
 * @param cls - The CSS class to add.
 * @returns A boolean value indicating whether the class was successfully added.
 */
export function addClass(el: Element, cls: string) {
    return toggleClass(el, cls, true);
}

/**
 * Removes a CSS class from an element.
 * 
 * @param el - The element from which to remove the class.
 * @param cls - The CSS class to remove.
 * @returns A boolean indicating whether the class was successfully removed.
 */
export function removeClass(el: Element, cls: string) {
    return toggleClass(el, cls, false);
}

/**
 * Appends content like DOM nodes, string, number or an array of these to the parent node.
 * Undefined, null, false values are ignored. Promises are awaited.
 * @param parent Target parent element
 * @param child The content
 */
export function appendToNode(parent: ParentNode, child: any) {

    if (child == null || child === false)
        return;

    if (isArrayLike(child)) {
        for (var i = 0; i < child.length; i++) {
            appendToNode(parent, child[i]);
        }
    } else if (typeof child === "string") {
        parent.appendChild(document.createTextNode(child));
    }
    else if (child instanceof Node) {
        parent.appendChild(child);
    }
    else if (isPromiseLike(child)) {
        const placeholder = parent.appendChild(document.createComment("Loading content..."));
        child.then(result => {
            const fragment = document.createDocumentFragment();
            appendToNode(fragment, result);
            placeholder.parentElement?.replaceChild(fragment, placeholder);
        }, error => {
            placeholder.textContent = "Error loading content: " + error;
            throw error;
        });
    }
    else {
        parent.append(child);
    }
}

// From https://pragmaticwebsecurity.com/articles/spasecurity/react-xss-part1
const SAFE_URL_PATTERN = /^(?:(?:https?|mailto|ftp|tel|file|sms):|[^&:/?#]*(?:[/?#]|$))/gi;

/** A pattern that matches safe data URLs. It only matches image, video, and audio types. */
const DATA_URL_PATTERN = /^data:(?:image\/(?:bmp|gif|jpeg|jpg|png|tiff|webp)|video\/(?:mpeg|mp4|ogg|webm)|audio\/(?:mp3|oga|ogg|opus));base64,[a-z0-9+\/]+=*$/i;

export function sanitizeUrl(url: string): string {
    url = String(url).trim();
    if (url === "null" || url.length === 0 || url === "about:blank") return "about:blank";
    if (url.match(SAFE_URL_PATTERN) || url.match(DATA_URL_PATTERN)) return url;
    if (url === "javascript:void(0)" || url === "javascript:;") return url;
    return `unsafe:${url}`;
}
