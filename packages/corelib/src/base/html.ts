import * as sleekgrid from "@serenity-is/sleekgrid";
import { Config } from "./config";
import { isArrayLike, isPromiseLike } from "./system";

/**
 * Content that can be rendered as a toast/notification message or appended to the DOM.
 * Accepts plain strings, DOM elements (HTML/SVG/MathML) and document fragments, which are
 * handled by {@link appendToNode} and the notification helpers.
 */
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
 * HTML-encodes a value by escaping `<`, `>`, `"`, `'`, and `&`.
 * @param s - Value to encode. Non-string values are coerced to string; `null`/`undefined` yields an empty string.
 * @returns The HTML-escaped string, safe for interpolation into HTML markup.
 * @example
 * ```ts
 * htmlEncode('<a href="x">a & b</a>'); // "&lt;a href=&quot;x&quot;&gt;a &amp; b&lt;/a&gt;"
 * ```
 */
export function htmlEncode(s: any): string {
    if (s == null)
        return '';

    if (typeof s !== "string")
        s = "" + s;

    return s.replace(/[<>"'&]/g, escFunc)
}

/**
 * Toggles one or more CSS classes on an element, supporting space-separated lists.
 * When `cls` contains spaces it is split and each token is toggled individually.
 * @param el - Target element. No-op if falsy.
 * @param cls - Single class or space-separated class list to toggle. No-op if `null`/empty.
 * @param add - Force mode: `true` to add, `false` to remove, `undefined` to toggle.
 * @remarks Delegates to `Element.classList.toggle` per token, preserving existing classes.
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
 * Adds one or more CSS classes to an element.
 * @param el - Target element.
 * @param cls - Class name or space-separated list of class names to add.
 * @remarks Wraps {@link toggleClass} with `add=true`; no-ops for empty/null inputs.
 */
export function addClass(el: Element, cls: string) {
    return toggleClass(el, cls, true);
}

/**
 * Removes one or more CSS classes from an element.
 * @param el - Target element.
 * @param cls - Class name or space-separated list of class names to remove.
 * @remarks Wraps {@link toggleClass} with `add=false`; no-ops for empty/null inputs.
 */
export function removeClass(el: Element, cls: string) {
    return toggleClass(el, cls, false);
}

/**
 * Appends heterogeneous content to a parent node.
 * Handles strings (as text nodes), `Node` instances, array-like collections (recursively),
 * promise-like values (async placeholder replaced on resolve/reject), and primitive values via `Node.append`.
 * Falsy values `null`, `undefined`, and `false` are ignored.
 * @param parent - Target parent node to append into.
 * @param child - Content to append: a single value, array-like collection, `Node`, string, or `PromiseLike`.
 * @remarks Promise children insert a comment placeholder synchronously and replace it with the resolved fragment when settled.
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

/**
 * Sanitizes a URL for safe use in `href`/`src` attributes.
 * Allows `http`, `https`, `mailto`, `ftp`, `tel`, `file`, `sms`, safe relative URLs, and safe `data:` image/video/audio URLs.
 * Preserves `about:blank` and `javascript:void(0)` idioms; otherwise prefixes unsafe values with `unsafe:`.
 * @param url - URL string to sanitize; trimmed before validation.
 * @returns A safe URL string, or `unsafe:<original>` if the input fails validation.
 * @example
 * ```ts
 * sanitizeUrl("javascript:alert(1)"); // "unsafe:javascript:alert(1)"
 * sanitizeUrl("/app/page?x=1"); // "/app/page?x=1"
 * ```
 */
export function sanitizeUrl(url: string): string {
    url = String(url).trim();
    if (url === "null" || url.length === 0 || url === "about:blank") return "about:blank";
    if (url.match(SAFE_URL_PATTERN) || url.match(DATA_URL_PATTERN)) return url;
    if (url === "javascript:void(0)" || url === "javascript:;") return url;
    return `unsafe:${url}`;
}

/**
 * Gets the read-only state of a DOM element without consulting attached widgets.
 * Considers the `readonly` CSS class, the `disabled` attribute for `select`/`radio`/`checkbox`,
 * and the `readonly` attribute for other inputs.
 * @param el - Element to inspect. Returns `null` if `el` is `null`/`undefined`.
 * @returns `true` if read-only/disabled, `false` otherwise, or `null` when `el` is absent.
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
 * Sets the read-only appearance and attribute on one or more elements without touching attached widgets.
 * Toggles the `readonly` CSS class and sets `disabled` (for `select`/`radio`/`checkbox`) or `readonly` (for other elements).
 * @param elements - Single element or array-like collection of elements. No-op if falsy.
 * @param value - `true` to make read-only/disabled, `false` to make editable.
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
 * Parses a URL query string into a key/value map.
 * @param s - Query string to parse (without leading `?` is also accepted). When `undefined`, `location.search` is used.
 * @returns An object mapping decoded keys to decoded values. Keys without `=` map to their own name; malformed percent-encodings are skipped.
 * @example
 * ```ts
 * parseQueryString("a=1&b=hello%20world"); // { a: "1", b: "hello world" }
 * ```
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
        try {
            let name = decodeURIComponent(pair[0]);
            result[name] = pair.length >= 2 ? decodeURIComponent(pair[1]) : name;
        }
        catch {
            // ignore malformed percent-encoding
        }
    }
    return result;
}

/**
 * Checks whether a return URL is safe for redirects.
 * A safe URL must be a relative path starting with exactly one `/`, contain no protocol (`:`), backslashes, control characters, or `//` after the leading slash, and use only `\w`, `-`, `.`, `/`, `?`, `&`, `=`, `%` characters.
 * @param url - Candidate return URL to validate.
 * @returns `true` if the URL is safe to use as a redirect target, `false` otherwise.
 */
export function isSafeReturnUrl(url: string): boolean {
    if (!url || typeof url !== "string")
        return false;
    // Must start with exactly one /, e.g. reject protocol-relative URLs like //host
    if (!/^\/(?!\/)/.test(url))
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
 * Retrieves the `returnUrl` from the current query string, falling back to application config.
 * @param opt - Options controlling lookup behavior.
 * @param opt.queryOnly - When `true`, only the query string is checked; skips {@link Config.defaultReturnUrl}.
 * @param opt.ignoreUnsafe - When `true`, unsafe URLs are returned as-is; otherwise unsafe values are discarded (`null`).
 * @param opt.purpose - Purpose key forwarded to `Config.defaultReturnUrl` when no query-string value is found.
 * @returns The validated return URL, the configured default, or `null`/`undefined` if none is available or the query value is unsafe.
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
 * Escapes a string for safe use as a CSS identifier/selector.
 * Delegates to `CSS.escape` when available; otherwise implements the CSSOM spec polyfill.
 * @param selector - Raw selector/identifier to escape.
 * @returns The escaped selector string safe for `querySelector` and CSS rules.
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
 * Sanitizes an HTML string by stripping dangerous elements and attributes.
 * Preference order: SleekGrid sanitizer (`sleekgrid.formatterContext()?.sanitizer` or `sleekgrid.gridDefaults.sanitizer`), `DOMPurify.sanitize` if present, otherwise a built-in `DOMParser` implementation that removes `script`/`iframe`/`object`/`embed`/`form`/`style`/`link` and event-handler / unsafe-URL attributes.
 * Falls back to {@link htmlEncode} if `DOMParser` is unavailable or parsing throws.
 * @param dirtyHtml - Untrusted HTML markup to sanitize. Falsy values return an empty string; strings without HTML tags/entities are returned as-is (fast path).
 * @returns The sanitized HTML string safe for insertion via `innerHTML`.
 * @remarks This duplicates the basic DOM sanitizer logic so corelib works standalone with or without SleekGrid loaded.
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