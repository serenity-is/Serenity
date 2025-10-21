import type { SignalOrValue } from "@serenity-is/sleekdom";

export function addClass(el: Element, cls: string) {
    if (cls == null || !cls.length)
        return;

    if (cls.indexOf(' ') >= 0) {
        var arr = cls.split(' ').map(x => x.trim()).filter(x => x.length);
        for (var a of arr)
            el.classList.add(a);
    }
    else
        el.classList.add(cls);
}

const esc: Record<string, string> = {
    '<': '&lt;',
    '>': '&gt;',
    '"': '&quot;',
    "'": "&apos;",
    '&': '&amp;',
}

function escFunc(a: string) {
    return esc[a];
}

export function escapeHtml(s: any) {
    if (!arguments.length && this && this !== globalThis) {
        s = this.value;
    }

    if (s == null)
        return '';

    if (typeof s !== "string")
        s = "" + s;

    if (this && this !== globalThis && this.enableHtmlRendering === false)
        return s;

    return s.replace(/[<>"'&]/g, escFunc)
}

const maybeHtmlRegex = /<|>|&|"|'/;

export function basicDOMSanitizer(dirtyHtml: string): string {
    if (!dirtyHtml) {
        return "";
    }

    // Fast path: if the input contains no HTML tags or entities, it's safe to return as-is
    // This avoids the expensive DOMParser overhead for simple text content
    if (!maybeHtmlRegex.test(dirtyHtml)) {
        return dirtyHtml;
    }

    // Check if DOMParser is available (should be in all modern browsers)
    if (typeof DOMParser === 'undefined') {
        // Fallback to basic escaping if DOMParser is not available
        return escapeHtml(dirtyHtml);
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
            return escapeHtml(dirtyHtml);
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
        return escapeHtml(dirtyHtml);
    }
}

export function disableSelection(target: HTMLElement) {
    if (target) {
        target.style.userSelect = "none";
        target.addEventListener('selectstart', () => false);
    }
}

export function removeClass(el: Element, cls: string) {
    if (cls == null || !cls.length)
        return;

    if (cls.indexOf(' ') >= 0) {
        var arr = cls.split(' ').map(x => x.trim()).filter(x => x.length);
        for (var a of arr)
            el.classList.remove(a);
    }
    else
        el.classList.remove(cls);
}

export function parsePx(str: string) {
    var value = parseFloat(str);
    if (isNaN(value))
        return 0;
    return value;
}
