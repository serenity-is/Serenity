import * as sleekgrid from "@serenity-is/sleekgrid";
import { Config } from "./config";
import { isArrayLike, isPromiseLike } from "./system";

export type RenderableContent = string | HTMLElement | SVGElement | MathMLElement | DocumentFragment;

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

    const k = cls.split(' ').map(x => x.trim()).filter(x => x.length);
    for (const a of k)
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
        for (let i = 0; i < child.length; i++) {
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

/**
 * Gets readonly state of an element. If the element is null, returns null.
 * It does not check for attached widgets. It returns true if the element has readonly class,
 * disabled attribute (select, radio, checkbox) or readonly attribute (other inputs).
 * @param el element
 */
export function getElementReadOnly(el: Element): boolean | null {
    if (el == null)
        return null;

    if (el.classList.contains('readonly'))
        return true;

    const type = el.getAttribute('type');
    if (el.tagName == 'SELECT' || type === 'radio' || type === 'checkbox')
        return el.hasAttribute('disabled');

    return el.hasAttribute('readonly');
}

/**
 * Sets readonly class and disabled (for select, radio, checkbox) or readonly attribute (for other inputs) on given element.
 * It does not check for attached widgets.
 * @param el Element
 * @param value Readonly state
 */
export function setElementReadOnly(elements: Element | ArrayLike<Element>, value: boolean) {
    if (!elements)
        return;
    elements = isArrayLike(elements) ? elements : [elements];
    for (let i = 0; i < elements.length; i++) {
        let el = elements[i];
        if (!el)
            continue;
        const type = el.getAttribute('type');
        el.classList.toggle('readonly', !!value);
        const attr = el.tagName == 'SELECT' || type === 'radio' || type === 'checkbox' ? 'disabled' : 'readonly';
        value ? el.setAttribute(attr, attr) : el.removeAttribute(attr);
    }
}

/**
 * Parses a query string into an object.
 * @param s Query string to parse, if not specified, location.search will be used.
 * @return An object with key/value pairs from the query string.
 */
export function parseQueryString(s?: string): Record<string, string> {
    let qs: string;
    if (s === undefined)
        qs = location.search.substring(1, location.search.length);
    else
        qs = s || '';
    let result: Record<string, string> = {};
    let parts = qs.split('&');
    for (let i = 0; i < parts.length; i++) {
        let part = parts[i];
        if (!part.length)
            continue;
        let pair = parts[i].split('=');
        let name = decodeURIComponent(pair[0]);
        result[name] = (pair.length >= 2 ? decodeURIComponent(pair[1]) : name);
    }
    return result;
}

/**
 * Checks whether a return URL is safe for redirects. Must be relative, start with a single slash,
 * and contain only allowed characters (no protocol, no backslashes, no control chars, etc).
 */
export function isSafeReturnUrl(url: string): boolean {
    if (!url || typeof url !== "string")
        return false;
    // Must start with exactly one /
    if (!/^\//.test(url))
        return false;
    // Reject any : to prevent protocol-relative and absolute URLs
    if (url.includes(':'))
        return false;
    // No backslash, control chars, or double slashes after initial /
    if (url.includes('\\') ||
        /[\0-\x1F\x7F]/.test(url) ||
        /\/\//.test(url.substring(1)))
        return false;
    // Only allow URLs of reasonable length and valid characters (path/query)
    if (!/^\/[\w\-./?&=%]*$/.test(url))
        return false;
    return true;
}

/**
 * Gets the return URL from the query string.
 * @param opt Options for getting the return URL.
 */
export function getReturnUrl(opt?: {
    /** Whether to only consider the query string. If true, the function will not check the default return URL. */
    queryOnly?: boolean;
    /** Whether to ignore unsafe URLs. If false or null (default), the function will only return safe URLs. */
    ignoreUnsafe?: boolean;
    /** The purpose of the return URL. This can be used to determine the default return URL if none is found in the query string. */
    purpose?: string;
}) {
    const q = parseQueryString();
    let returnUrl = q['returnUrl'] || q['ReturnUrl'] || q["ReturnURL"] || q["returnURL"];

    if (returnUrl && (!opt?.ignoreUnsafe)) {
        if (!isSafeReturnUrl(returnUrl))
            returnUrl = null;
    }

    if (!returnUrl && !opt?.queryOnly)
        returnUrl = Config.defaultReturnUrl(opt?.purpose);

    return returnUrl;
}

/**
 * Escapes a CSS selector.
 * @param selector The CSS selector to escape.
 */
export function cssEscape(selector: string) {
    if (typeof CSS !== 'undefined' && typeof CSS.escape === "function")
        return CSS.escape(selector);

    const string = String(selector);
    const length = string.length;
    let index = -1;
    let codeUnit: number;
    let result = '';
    const firstCodeUnit = string.charCodeAt(0);

    if (length == 1 && firstCodeUnit == 0x002D)
        return '\\' + string;

    while (++index < length) {
        codeUnit = string.charCodeAt(index);
        if (codeUnit == 0x0000) {
            result += '\uFFFD';
            continue;
        }

        if ((codeUnit >= 0x0001 && codeUnit <= 0x001F) || codeUnit == 0x007F ||
            (index == 0 && codeUnit >= 0x0030 && codeUnit <= 0x0039) ||
            (index == 1 && codeUnit >= 0x0030 && codeUnit <= 0x0039 && firstCodeUnit == 0x002D)
        ) {
            result += '\\' + codeUnit.toString(16) + ' ';
            continue;
        }

        if (codeUnit >= 0x0080 || codeUnit == 0x002D || codeUnit == 0x005F || codeUnit >= 0x0030 && codeUnit <= 0x0039 ||
            codeUnit >= 0x0041 && codeUnit <= 0x005A || codeUnit >= 0x0061 && codeUnit <= 0x007A
        ) {
            result += string.charAt(index);
            continue;
        }

        result += '\\' + string.charAt(index);
    }
    return result;
}

const maybeHtmlRegex = /<|>|&|"|'/;

/** 
 * Sanitizes HTML by removing dangerous elements and attributes.
 * Need to duplicate basicDomSanitizer logic here as corelib does 
 * not bundle sleekgrid, and should work standalone with/without 
 * sleekgrid loaded.
 * @param dirtyHtml The HTML string to sanitize.
 * @returns The sanitized HTML string.
 */
export function sanitizeHtml(dirtyHtml: string): string {
    if (!dirtyHtml) {
        return "";
    }

    // Fast path: if the input contains no HTML tags or entities, it's safe to return as-is
    // This avoids the expensive DOMParser overhead for simple text content
    if (!maybeHtmlRegex.test(dirtyHtml)) {
        return dirtyHtml;
    }

    let sanitizer: (dirtyHtml: string) => string;
    // use sanitizer logic from sleekgrid if available
    if (typeof sleekgrid !== "undefined") {
        if (typeof sleekgrid.formatterContext === "function")
            sanitizer = sleekgrid.formatterContext()?.sanitizer;
        if (!sanitizer && typeof sleekgrid.gridDefaults?.sanitizer === "function")
            sanitizer = sleekgrid.gridDefaults.sanitizer;
    }

    if (!sanitizer && typeof (globalThis as any).DOMPurify?.sanitize === "function")
        sanitizer = (globalThis as any).DOMPurify.sanitize;

    if (sanitizer)
        return sanitizer(dirtyHtml);

    // Check if DOMParser is available (should be in all modern browsers)
    if (typeof DOMParser === 'undefined') {
        // Fallback to basic escaping if DOMParser is not available
        return htmlEncode(dirtyHtml);
    }

    // Pattern for safe URLs - blocks dangerous protocols while allowing safe ones
    // Based on Bootstrap's implementation but extended to block more dangerous protocols
    const SAFE_URL_PATTERN = /^(?!javascript:|data:|vbscript:)(?:[a-z0-9+.-]+:|[^&:/?#]*(?:[/?#]|$))/i;

    try {
        // Use DOMParser for safer HTML parsing than innerHTML
        const parser = new DOMParser();
        const doc = parser.parseFromString(dirtyHtml, 'text/html');
        const body = doc.body || (typeof document !== 'undefined' ? document.createElement('body') : null);

        if (!body) {
            return htmlEncode(dirtyHtml);
        }

        // For HTML fragments, DOMParser might put content in different places
        // If body is empty but we have content, it might be in the document root
        let targetElement = body;
        if (body.innerHTML.trim() === '' && doc.documentElement && doc.documentElement.innerHTML.trim()) {
            // Move content from documentElement to body for consistent processing
            body.innerHTML = doc.documentElement.innerHTML;
        }

        // If body is still empty after moving content, the target might be documentElement
        if (body.innerHTML.trim() === '' && doc.documentElement) {
            targetElement = doc.documentElement;
        }

        // Remove potentially dangerous elements completely
        const dangerousElements = targetElement.querySelectorAll('script, iframe, object, embed, form, input, button, textarea, select, style, link');
        dangerousElements.forEach(el => el.remove());

        // Remove dangerous attributes from remaining elements
        const allElements = targetElement.querySelectorAll('*');
        allElements.forEach(el => {
            const element = el as HTMLElement;
            // Remove event handler attributes and dangerous href/src values
            Array.from(element.attributes).forEach(attr => {
                const name = attr.name.toLowerCase();
                const value = attr.value;

                // Remove all event handlers
                if (name.startsWith('on')) {
                    element.removeAttribute(attr.name);
                    return;
                }

                // Validate href/src/xlink:href attributes with safe URL pattern
                if (['href', 'src', 'xlink:href'].indexOf(name) >= 0) {
                    if (!SAFE_URL_PATTERN.test(value)) {
                        element.removeAttribute(attr.name);
                        return;
                    }
                }

                // Remove any attribute containing javascript anywhere in its value
                if (value.toLowerCase().indexOf('javascript') >= 0) {
                    element.removeAttribute(attr.name);
                }
            });
        });

        return targetElement.innerHTML;
    } catch (e) {
        // In case of any parsing error, fall back to basic escaping
        return htmlEncode(dirtyHtml);
    }
}