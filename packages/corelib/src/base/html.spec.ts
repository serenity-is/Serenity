import type { FormatterContext } from "@serenity-is/sleekgrid";
import { Config } from "./config";
import { addClass, appendToNode, cssEscape, getElementReadOnly, getReturnUrl, htmlEncode, parseQueryString, removeClass, sanitizeHtml, sanitizeUrl, setElementReadOnly, toggleClass } from "./html";
import * as sleekgrid from "@serenity-is/sleekgrid";

vi.mock("@serenity-is/sleekgrid", async (importActual) => {
    return {
        formatterContext: vi.fn(),
        gridDefaults: { sanitizer: null }
    };
});


describe("htmlEncode", () => {
    it("encodes html", () => {
        expect(htmlEncode("<div>test</div>")).toBe("&lt;div&gt;test&lt;/div&gt;");
        expect(htmlEncode("'&><\"")).toBe("&#39;&amp;&gt;&lt;&quot;");
    });

    it("handles null and empty string values", () => {
        expect(htmlEncode(null)).toBe("");
        expect(htmlEncode(undefined)).toBe("");
        expect(htmlEncode("")).toBe("");
    });

    it("converts other types to string", () => {
        expect(htmlEncode(1)).toBe("1");
        expect(htmlEncode(true)).toBe("true");
        expect(htmlEncode(false)).toBe("false");
    });
});

describe("toggleClass", () => {
    it("does nothing if class is null or empty", () => {
        const el = document.createElement("div");
        el.setAttribute("class", "test1 test2");

        toggleClass(el, null, true);
        expect(el.getAttribute("class")).toBe("test1 test2");

        toggleClass(el, undefined, false);
        expect(el.getAttribute("class")).toBe("test1 test2");

        toggleClass(el, '', false);
        expect(el.getAttribute("class")).toBe("test1 test2");
    });


    it("can toggle single class", () => {
        const el = document.createElement("div");
        el.setAttribute("class", "test1 test2");

        toggleClass(el, "test1");
        expect(el.getAttribute("class")).toBe("test2");

        toggleClass(el, "test3");
        expect(el.getAttribute("class")).toBe("test2 test3");

        toggleClass(el, "test3");
        expect(el.getAttribute("class")).toBe("test2");
    });

    it("can toggle multiple classes", () => {
        const el = document.createElement("div");
        el.setAttribute("class", "test1 test2");

        toggleClass(el, "test1 test2");
        expect(el.getAttribute("class")).toBe("");

        toggleClass(el, "test3 test4");
        expect(el.getAttribute("class")).toBe("test3 test4");

        toggleClass(el, "test5 test3 test6");
        expect(el.getAttribute("class")).toBe("test4 test5 test6");
    });

    it("adds classes if third parameter is true", () => {
        const el = document.createElement("div");
        el.setAttribute("class", "test1 test2");

        toggleClass(el, "test1 test2", true);
        expect(el.getAttribute("class")).toBe("test1 test2");

        toggleClass(el, "test3 test4", true);
        expect(el.getAttribute("class")).toBe("test1 test2 test3 test4");

        toggleClass(el, "test5 test3 test6", true);
        expect(el.getAttribute("class")).toBe("test1 test2 test3 test4 test5 test6");
    });

    it("removes classes if third parameter is false", () => {
        const el = document.createElement("div");
        el.setAttribute("class", "test1 test2");

        toggleClass(el, "test1 test2", false);
        expect(el.getAttribute("class")).toBe("");
    });

    it("ignores whitespace", () => {
        const el = document.createElement("div");
        el.setAttribute("class", "test1 test2");

        toggleClass(el, "test1      test2", false);
        expect(el.getAttribute("class")).toBe("");
    });
});

describe("addClass", () => {
    it("adds class to the element", () => {
        const el = document.createElement("div");
        el.setAttribute("class", "test1");

        addClass(el, "test2");
        expect(el.getAttribute("class")).toBe("test1 test2");
    });

    it("does nothing if class is null or empty", () => {
        const el = document.createElement("div");
        el.setAttribute("class", "test1");

        addClass(el, null);
        expect(el.getAttribute("class")).toBe("test1");

        addClass(el, undefined);
        expect(el.getAttribute("class")).toBe("test1");

        addClass(el, '');
        expect(el.getAttribute("class")).toBe("test1");
    });

    it("does not add duplicate classes", () => {
        const el = document.createElement("div");
        el.setAttribute("class", "test1");

        addClass(el, "test1");
        expect(el.getAttribute("class")).toBe("test1");
    });
});

describe("removeClass", () => {
    it("removes class from the element", () => {
        const el = document.createElement("div");
        el.setAttribute("class", "test1 test2");

        removeClass(el, "test2");
        expect(el.getAttribute("class")).toBe("test1");
    });

    it("does nothing if class is null or empty", () => {
        const el = document.createElement("div");
        el.setAttribute("class", "test1");

        removeClass(el, null);
        expect(el.getAttribute("class")).toBe("test1");

        removeClass(el, undefined);
        expect(el.getAttribute("class")).toBe("test1");

        removeClass(el, '');
        expect(el.getAttribute("class")).toBe("test1");
    });

    it("does not remove non-existing classes", () => {
        const el = document.createElement("div");
        el.setAttribute("class", "test1");

        removeClass(el, "test2");
        expect(el.getAttribute("class")).toBe("test1");
    });
});

describe("appendToNode", () => {
    it("throws on null parent", () => {
        expect(() => appendToNode(null, document.createElement("div"))).toThrow();
    });

    it("ignores null content", () => {
        const parent = document.createElement("div");
        appendToNode(parent, null);
        expect(parent.innerHTML).toBe("");
    });

    it("ignores false content", () => {
        const parent = document.createElement("div");
        appendToNode(parent, false);
        expect(parent.innerHTML).toBe("");
    });

    it("ignores array with false and null content", () => {
        const parent = document.createElement("div");
        appendToNode(parent, [false, null]);
        expect(parent.innerHTML).toBe("");
    });

    it("appends array elements", () => {
        const parent = document.createElement("div");
        const div1 = document.createElement("div");
        div1.innerHTML = "1";
        const div2 = document.createElement("div");
        div2.innerHTML = "2";
        appendToNode(parent, [div1, div2]);
        expect(parent.innerHTML).toBe("<div>1</div><div>2</div>");
        expect(parent.firstChild).toBe(div1);
        expect(parent.lastChild).toBe(div2);
    });

    it("creates text node from string", () => {
        const parent = document.createElement("div");
        appendToNode(parent, "test");
        expect(parent.innerHTML).toBe("test");
        expect(parent.firstChild?.nodeType).toBe(Node.TEXT_NODE);
        expect(parent.firstChild.textContent).toBe("test");
    });

    it("can wait for promise to resolve", async () => {
        const parent = document.createElement("div");
        const div = document.createElement("div");
        div.innerHTML = "test";
        appendToNode(parent, Promise.resolve(div));
        expect(parent.innerHTML).toBe("<!--Loading content...-->");
        await Promise.resolve();
        expect(parent.innerHTML).toBe("<div>test</div>");
    });

    it("handles rejected promise", () => new Promise(async done => {
        const parent = document.createElement("div");
        const div = document.createElement("div");
        div.innerHTML = "test";
        const unhandledRejection = () => {
            done(void 0);
            globalThis.process?.off ? globalThis.process.off("unhandledRejection", unhandledRejection) :
                window.removeEventListener("unhandledrejection", unhandledRejection);
        };
        globalThis.process?.on ? globalThis.process.on("unhandledRejection", unhandledRejection) :
            window.addEventListener("unhandledrejection", unhandledRejection);
        appendToNode(parent, Promise.reject("some reject reason"));
        expect(parent.innerHTML).toBe("<!--Loading content...-->");
        await Promise.resolve();
        expect(parent.innerHTML).toBe("<!--Error loading content: some reject reason-->");
    }));

    it("calls append for other content types", () => {
        const parent = document.createElement("div");
        const content = { abc: 5 };
        appendToNode(parent, content as any);
        expect(parent.innerHTML).toBe("[object Object]");
    });

});

describe("getElementReadOnly", () => {
    it("returns null if element is null", () => {
        expect(getElementReadOnly(null)).toBeNull();
    });

    it("returns true if element has readonly class", () => {
        const el = document.createElement("div");
        el.classList.add("readonly");
        expect(getElementReadOnly(el)).toBe(true);
    });

    it("returns true if element is a select and has disabled attribute", () => {
        const el = document.createElement("select");
        el.setAttribute("disabled", "disabled");
        expect(getElementReadOnly(el)).toBe(true);
    });

    it("returns true if element is a radio and has disabled attribute", () => {
        const el = document.createElement("input");
        el.setAttribute("type", "radio");
        el.setAttribute("disabled", "disabled");
        expect(getElementReadOnly(el)).toBe(true);
    });

    it("returns true if element is a checkbox and has disabled attribute", () => {
        const el = document.createElement("input");
        el.setAttribute("type", "checkbox");
        el.setAttribute("disabled", "disabled");
        expect(getElementReadOnly(el)).toBe(true);
    });

    it("returns true if element has readonly attribute", () => {
        const el = document.createElement("input");
        el.setAttribute("readonly", "readonly");
        expect(getElementReadOnly(el)).toBe(true);
    });

    it("returns false if element does not have readonly class or attributes", () => {
        const el = document.createElement("input");
        expect(getElementReadOnly(el)).toBe(false);
    });

    it("returns false if element is a select without disabled attribute", () => {
        const el = document.createElement("select");
        expect(getElementReadOnly(el)).toBe(false);
    });

    it("returns false if element is a radio without disabled attribute", () => {
        const el = document.createElement("input");
        el.setAttribute("type", "radio");
        expect(getElementReadOnly(el)).toBe(false);
    });

    it("returns false if element is a checkbox without disabled attribute", () => {
        const el = document.createElement("input");
        el.setAttribute("type", "checkbox");
        expect(getElementReadOnly(el)).toBe(false);
    });
});

describe("setElementReadOnly", () => {
    it("ignores null elements", () => {
        setElementReadOnly(null, true);
        setElementReadOnly([null], true);
    });

    it("sets readonly class and attribute for single element", () => {
        const el = document.createElement("input");
        setElementReadOnly(el, true);
        expect(el.classList.contains("readonly")).toBe(true);
        expect(el.hasAttribute("readonly")).toBe(true);
    });

    it("removes readonly class and attribute for single element", () => {
        const el = document.createElement("input");
        el.classList.add("readonly");
        el.setAttribute("readonly", "readonly");
        setElementReadOnly(el, false);
        expect(el.classList.contains("readonly")).toBe(false);
        expect(el.hasAttribute("readonly")).toBe(false);
    });

    it("sets disabled attribute for select element", () => {
        const el = document.createElement("select");
        setElementReadOnly(el, true);
        expect(el.classList.contains("readonly")).toBe(true);
        expect(el.hasAttribute("disabled")).toBe(true);
    });

    it("removes disabled attribute for select element", () => {
        const el = document.createElement("select");
        el.classList.add("readonly");
        el.setAttribute("disabled", "disabled");
        setElementReadOnly(el, false);
        expect(el.classList.contains("readonly")).toBe(false);
        expect(el.hasAttribute("disabled")).toBe(false);
    });

    it("sets disabled attribute for radio input", () => {
        const el = document.createElement("input");
        el.setAttribute("type", "radio");
        setElementReadOnly(el, true);
        expect(el.classList.contains("readonly")).toBe(true);
        expect(el.hasAttribute("disabled")).toBe(true);
    });

    it("removes disabled attribute for radio input", () => {
        const el = document.createElement("input");
        el.setAttribute("type", "radio");
        el.classList.add("readonly");
        el.setAttribute("disabled", "disabled");
        setElementReadOnly(el, false);
        expect(el.classList.contains("readonly")).toBe(false);
        expect(el.hasAttribute("disabled")).toBe(false);
    });

    it("sets disabled attribute for checkbox input", () => {
        const el = document.createElement("input");
        el.setAttribute("type", "checkbox");
        setElementReadOnly(el, true);
        expect(el.classList.contains("readonly")).toBe(true);
        expect(el.hasAttribute("disabled")).toBe(true);
    });

    it("removes disabled attribute for checkbox input", () => {
        const el = document.createElement("input");
        el.setAttribute("type", "checkbox");
        el.classList.add("readonly");
        el.setAttribute("disabled", "disabled");
        setElementReadOnly(el, false);
        expect(el.classList.contains("readonly")).toBe(false);
        expect(el.hasAttribute("disabled")).toBe(false);
    });

    it("sets readonly class and attribute for multiple elements", () => {
        const el1 = document.createElement("input");
        const el2 = document.createElement("select");
        setElementReadOnly([el1, el2], true);
        expect(el1.classList.contains("readonly")).toBe(true);
        expect(el1.hasAttribute("readonly")).toBe(true);
        expect(el2.classList.contains("readonly")).toBe(true);
        expect(el2.hasAttribute("disabled")).toBe(true);
    });

    it("removes readonly class and attribute for multiple elements", () => {
        const el1 = document.createElement("input");
        el1.classList.add("readonly");
        el1.setAttribute("readonly", "readonly");
        const el2 = document.createElement("select");
        el2.classList.add("readonly");
        el2.setAttribute("disabled", "disabled");
        setElementReadOnly([el1, el2], false);
        expect(el1.classList.contains("readonly")).toBe(false);
        expect(el1.hasAttribute("readonly")).toBe(false);
        expect(el2.classList.contains("readonly")).toBe(false);
        expect(el2.hasAttribute("disabled")).toBe(false);
    });
});

describe("sanitizeUrl", () => {
    it("returns 'about:blank' for null, empty string, or 'about:blank'", () => {
        expect(sanitizeUrl(null)).toBe("about:blank");
        expect(sanitizeUrl("")).toBe("about:blank");
        expect(sanitizeUrl("about:blank")).toBe("about:blank");
    });

    it("returns the same URL if it matches SAFE_URL_PATTERN", () => {
        expect(sanitizeUrl("http://example.com")).toBe("http://example.com");
        expect(sanitizeUrl("https://example.com")).toBe("https://example.com");
        expect(sanitizeUrl("mailto:test@example.com")).toBe("mailto:test@example.com");
        expect(sanitizeUrl("ftp://example.com")).toBe("ftp://example.com");
        expect(sanitizeUrl("tel:+1234567890")).toBe("tel:+1234567890");
        expect(sanitizeUrl("file:///C:/path/to/file")).toBe("file:///C:/path/to/file");
        expect(sanitizeUrl("sms:+1234567890")).toBe("sms:+1234567890");
    });

    it("returns the same URL if it matches DATA_URL_PATTERN", () => {
        expect(sanitizeUrl("data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAUA")).toBe("data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAUA");
        expect(sanitizeUrl("data:video/mp4;base64,AAAAFGZ0eXBtcDQyAAAAAG1w")).toBe("data:video/mp4;base64,AAAAFGZ0eXBtcDQyAAAAAG1w");
        expect(sanitizeUrl("data:audio/mp3;base64,SUQzAwAAAAAA")).toBe("data:audio/mp3;base64,SUQzAwAAAAAA");
    });

    it("returns 'unsafe:' prefixed URL for unsafe URLs", () => {
        expect(sanitizeUrl("javascript:alert('XSS')")).toBe("unsafe:javascript:alert('XSS')");
        expect(sanitizeUrl("data:text/html;base64,PHNjcmlwdD5hbGVydCgnWFNTJyk8L3NjcmlwdD4=")).toBe("unsafe:data:text/html;base64,PHNjcmlwdD5hbGVydCgnWFNTJyk8L3NjcmlwdD4=");
    });

    it("returns 'javascript:void(0)' and 'javascript:;' as is", () => {
        expect(sanitizeUrl("javascript:void(0)")).toBe("javascript:void(0)");
        expect(sanitizeUrl("javascript:;")).toBe("javascript:;");
    });

    it("trims the URL before sanitizing", () => {
        expect(sanitizeUrl("  http://example.com  ")).toBe("http://example.com");
        expect(sanitizeUrl("  javascript:alert('XSS')  ")).toBe("unsafe:javascript:alert('XSS')");
    });
});

describe("parseQueryString", () => {
    it("parses query string into object", () => {
        const queryString = "param1=value1&param2=value2";
        const result = parseQueryString(queryString);
        expect(result).toEqual({ param1: "value1", param2: "value2" });
    });

    it("decodes URI components", () => {
        const queryString = "param1=value%201&param2=value%202";
        const result = parseQueryString(queryString);
        expect(result).toEqual({ param1: "value 1", param2: "value 2" });
    });

    it("handles empty query string", () => {
        const result = parseQueryString("");
        expect(result).toEqual({});
    });

    it("handles query string with no value", () => {
        const queryString = "param1&param2";
        const result = parseQueryString(queryString);
        expect(result).toEqual({ param1: "param1", param2: "param2" });
    });

    it("handles query string with empty value", () => {
        const queryString = "param1=&param2=";
        const result = parseQueryString(queryString);
        expect(result).toEqual({ param1: "", param2: "" });
    });

    it("handles query string with special characters", () => {
        const queryString = "param1=value1&param2=value@2&param3=value/3";
        const result = parseQueryString(queryString);
        expect(result).toEqual({ param1: "value1", param2: "value@2", param3: "value/3" });
    });

    it("parses query string from location.search if no argument is provided", () => {
        const oldLocation = window.location.href;
        if (changeJSDOMURL("http://localhost?param1=value1&param2=value2")) {
            try {
                const result = parseQueryString();
                expect(result).toEqual({ param1: "value1", param2: "value2" });
            } finally {
                changeJSDOMURL(oldLocation);
            }
        }
    });
});

describe("getReturnUrl", () => {
    it("returns returnUrl from query string if it is safe", () => {
        const oldLocation = window.location.href;
        if (changeJSDOMURL("http://localhost?returnUrl=/safe/path"))
            try {
                const result = getReturnUrl();
                expect(result).toBe("/safe/path");
            } finally {
                changeJSDOMURL(oldLocation);
            }
    });

    it("returns defaultReturnUrl if returnUrl from query string is unsafe", () => {
        const oldLocation = window.location.href;
        if (changeJSDOMURL("http://localhost?returnUrl=http://unsafe.com"))
            try {
                const result = getReturnUrl();
                expect(result).toBe(Config.defaultReturnUrl());
            } finally {
                changeJSDOMURL(oldLocation);
            }
    });

    it("returns default returnUrl if queryOnly is false and no returnUrl in query string", () => {
        const oldLocation = window.location.href;
        if (changeJSDOMURL("http://localhost"))
            try {
                const result = getReturnUrl({ queryOnly: false });
                expect(result).toBe(Config.defaultReturnUrl());
            } finally {
                changeJSDOMURL(oldLocation);
            }
    });

    it("returns undefined if queryOnly is true and no returnUrl in query string", () => {
        const oldLocation = window.location.href;
        if (changeJSDOMURL("http://localhost"))
            try {
                const result = getReturnUrl({ queryOnly: true });
                expect(result).toBeUndefined();
            } finally {
                changeJSDOMURL(oldLocation);
            }
    });

    it("returns returnUrl from query string if ignoreUnsafe is true", () => {
        const oldLocation = window.location.href;
        if (changeJSDOMURL("http://localhost?returnUrl=http://unsafe.com"))
            try {
                const result = getReturnUrl({ ignoreUnsafe: true });
                expect(result).toBe("http://unsafe.com");
            } finally {
                changeJSDOMURL(oldLocation);
            }
    });

    it("returns default returnUrl with purpose if provided", () => {
        const oldLocation = window.location.href;
        if (changeJSDOMURL("http://localhost"))
            try {
                const result = getReturnUrl({ purpose: "test" });
                expect(result).toBe(Config.defaultReturnUrl("test"));
            } finally {
                changeJSDOMURL(oldLocation);
            }
    });
});

function changeJSDOMURL(url: string) {
    if ((globalThis as any).jsdom?.reconfigure) {
        (globalThis as any).jsdom.reconfigure({ url: url });
        return true;
    }
    else
        return false;
}

describe("cssEscape", () => {
    it("escapes leading dash", () => {
        expect(cssEscape("-")).toBe("\\-");
        expect(cssEscape("-test")).toBe("-test");
    });

    it("escapes null character", () => {
        expect(cssEscape("\u0000")).toBe("\uFFFD");
        expect(cssEscape("a\u0000b")).toBe("a\uFFFDb");
    });

    it("escapes control characters and DEL", () => {
        expect(cssEscape("\u0001")).toBe("\\1 ");
        expect(cssEscape("\u001F")).toBe("\\1f ");
        expect(cssEscape("\u007F")).toBe("\\7f ");
    });

    it("escapes digit at start", () => {
        expect(cssEscape("1abc")).toBe("\\31 abc");
        expect(cssEscape("2")).toBe("\\32 ");
    });

    it("escapes digit at index 1 if first char is dash", () => {
        expect(cssEscape("-1abc")).toBe("-\\31 abc");
        expect(cssEscape("-2")).toBe("-\\32 ");
    });

    it("does not escape ASCII letters, digits (except at start), dash, underscore", () => {
        expect(cssEscape("abcABC123-_")).toBe("abcABC123-_");
    });

    it("escapes special characters", () => {
        expect(cssEscape("a b.c#d:e;f[g]")).toBe("a\\ b\\.c\\#d\\:e\\;f\\[g\\]");
    });

    it("escapes non-ASCII characters", () => {
        expect(cssEscape("üñîçødë")).toBe("üñîçødë");
        expect(cssEscape("测试")).toBe("测试");
    });

    it("returns empty string for empty input", () => {
        expect(cssEscape("")).toBe("");
    });

    it("uses CSS.escape if available", () => {
        const orig = (globalThis as any).CSS;
        (globalThis as any).CSS = { escape: (s: string) => "ESCAPED:" + s };
        expect(cssEscape("test")).toBe("ESCAPED:test");
        (globalThis as any).CSS = orig;
    });

    describe("sanitizeHtml ", () => {
        it("uses the sanitizer returned from sleekgrid formatterContext if available", () => {
            const mockSanitizer = vi.fn((html: string) => "SANITIZED:" + html);
            const mockFormatterContext = vi.fn(() => ({ sanitizer: mockSanitizer } as unknown as FormatterContext));
            const mockGridDefaults = { sanitizer: null };

            const sleekgridMocked = vi.mocked(sleekgrid);
            sleekgridMocked.formatterContext.mockImplementation(mockFormatterContext);
            sleekgridMocked.gridDefaults = mockGridDefaults;
            expect(sanitizeHtml("<div>test</div>")).toBe("SANITIZED:<div>test</div>");
            expect(mockFormatterContext).toHaveBeenCalled();
            expect(mockSanitizer).toHaveBeenCalledWith("<div>test</div>");
        });

        it("uses the sanitizer from sleekgrid gridDefaults if available and formatterContext does not provide one", () => {
            const mockSanitizer = vi.fn((html: string) => "SANITIZED_BY_DEFAULTS:" + html);
            const mockFormatterContext = vi.fn(() => ({ sanitizer: null } as unknown as FormatterContext));
            const mockGridDefaults = { sanitizer: mockSanitizer };

            const sleekgridMocked = vi.mocked(sleekgrid);
            sleekgridMocked.formatterContext.mockImplementation(mockFormatterContext);
            sleekgridMocked.gridDefaults = mockGridDefaults;
            expect(sanitizeHtml("<div>test</div>")).toBe("SANITIZED_BY_DEFAULTS:<div>test</div>");
            expect(mockFormatterContext).toHaveBeenCalled();
            expect(mockSanitizer).toHaveBeenCalledWith("<div>test</div>");
        });

        it("uses DOMPurify if available and no sleekgrid sanitizer", () => {
            const origDOMPurify = (globalThis as any).DOMPurify;
            (globalThis as any).DOMPurify = { sanitize: (html: string) => "DOMPURIFY_SANITIZED:" + html };
            const sleekgridMocked = vi.mocked(sleekgrid);
            sleekgridMocked.formatterContext.mockImplementation(() => ({ sanitizer: null } as unknown as FormatterContext));
            sleekgridMocked.gridDefaults = { sanitizer: null };
            expect(sanitizeHtml("<div>test</div>")).toBe("DOMPURIFY_SANITIZED:<div>test</div>");
            (globalThis as any).DOMPurify = origDOMPurify;
        });

        it("should return empty string for null/undefined input", () => {
            expect(sanitizeHtml(null)).toBe('');
            expect(sanitizeHtml(undefined)).toBe('');
            expect(sanitizeHtml('')).toBe('');
        });

        it("should use fast path for plain text without HTML characters", () => {
            // These should go through the fast path and return as-is
            expect(sanitizeHtml('Hello World')).toBe('Hello World');
            expect(sanitizeHtml('123456')).toBe('123456');
            expect(sanitizeHtml('a-z A-Z 0-9')).toBe('a-z A-Z 0-9');
            expect(sanitizeHtml('Special chars: @#$%^*()')).toBe('Special chars: @#$%^*()');
            expect(sanitizeHtml('Unicode: 你好 🌟')).toBe('Unicode: 你好 🌟');
        });

        it("should preserve safe HTML content", () => {
            const safe = '<div class="container"><p>Hello <strong>world</strong>!</p></div>';
            expect(sanitizeHtml(safe)).toBe(safe);
        });

        it("should remove script tags completely", () => {
            const dirty = '<div>Safe</div><script>alert("XSS")</script><p>Content</p>';
            const expected = '<div>Safe</div><p>Content</p>';
            expect(sanitizeHtml(dirty)).toBe(expected);
        });

        it("should remove iframe elements", () => {
            const dirty = '<p>Before</p><iframe src="malicious.com"></iframe><p>After</p>';
            const expected = '<p>Before</p><p>After</p>';
            expect(sanitizeHtml(dirty)).toBe(expected);
        });

        it("should remove dangerous elements", () => {
            const dangerousElements = ['object', 'embed', 'form', 'input', 'button', 'textarea', 'select', 'style', 'link'];
            dangerousElements.forEach(tag => {
                const dirty = `<p>Safe</p><${tag}>dangerous</${tag}><p>Content</p>`;
                const result = sanitizeHtml(dirty);
                expect(result).not.toContain(`<${tag}>`);
                expect(result).not.toContain(`</${tag}>`);
                expect(result).toContain('<p>Safe</p>');
                expect(result).toContain('<p>Content</p>');
            });
        });

        it("should remove event handler attributes", () => {
            const dirty = '<div onclick="alert(1)" onload="evil()" onmouseover="bad()">Content</div>';
            const expected = '<div>Content</div>';
            expect(sanitizeHtml(dirty)).toBe(expected);
        });

        it("should remove href attributes with javascript URLs", () => {
            const dirty = '<a href="javascript:alert(1)">Bad Link</a>';
            const expected = '<a>Bad Link</a>';
            expect(sanitizeHtml(dirty)).toBe(expected);
        });

        it("should remove src attributes with javascript URLs", () => {
            const dirty = '<img src="javascript:alert(1)">';
            const expected = '<img>';
            expect(sanitizeHtml(dirty)).toBe(expected);
        });

        it("should remove data URL attributes", () => {
            const dirty = '<img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg==">';
            const expected = '<img>';
            expect(sanitizeHtml(dirty)).toBe(expected);
        });

        it("should preserve safe href attributes", () => {
            const safe = '<a href="https://example.com">Safe Link</a>';
            expect(sanitizeHtml(safe)).toBe(safe);
        });

        it("should preserve safe src attributes", () => {
            const safe = '<img src="https://example.com/image.jpg" alt="Safe Image">';
            expect(sanitizeHtml(safe)).toBe(safe);
        });

        it("should remove attributes containing javascript anywhere", () => {
            const dirty = '<div data-js="javascript:void(0)" custom="some-javascript-code">Content</div>';
            const expected = '<div>Content</div>';
            expect(sanitizeHtml(dirty)).toBe(expected);
        });

        it("should handle complex nested dangerous content", () => {
            const dirty = `
            <div class="safe">
                <p>Safe paragraph</p>
                <script>alert('XSS1');</script>
                <a href="javascript:alert('XSS2')">Bad Link</a>
                <iframe src="evil.com">
                    <form action="hack.php">
                        <input type="hidden" name="csrf" value="stolen">
                        <button onclick="stealData()">Submit</button>
                    </form>
                </iframe>
                <img src="data:text/html,<script>alert('XSS3')</script>">
                <span>More safe content</span>
            </div>
        `;
            const result = sanitizeHtml(dirty);

            // Check that dangerous elements are removed
            expect(result).not.toContain('<script>');
            expect(result).not.toContain('<iframe>');
            expect(result).not.toContain('<form>');
            expect(result).not.toContain('<input>');
            expect(result).not.toContain('<button>');
            expect(result).not.toContain('javascript:');
            expect(result).not.toContain('data:');

            // Check that safe content is preserved
            expect(result).toContain('<div class="safe">');
            expect(result).toContain('<p>Safe paragraph</p>');
            expect(result).toContain('<span>More safe content</span>');
            expect(result).toContain('Bad Link'); // Text should remain even if href is removed
        });

        it("should handle malformed HTML gracefully", () => {
            const malformed = '<div><script>evil</script><p>Unclosed<div><a href="javascript:bad">Bad</a>';
            // Should not crash and should sanitize what's parseable
            const result = sanitizeHtml(malformed);
            expect(result).not.toContain('<script>');
            expect(result).not.toContain('javascript:');
            expect(result).toContain('Unclosed');
        });

        it("should handle HTML entities correctly", () => {
            const withEntities = '<p>&lt;script&gt;alert(1)&lt;/script&gt;</p>';
            expect(sanitizeHtml(withEntities)).toBe(withEntities);
        });

        it("should handle mixed case tags and attributes", () => {
            const mixedCase = '<DIV><SCRIPT>alert(1)</SCRIPT><A HREF="javascript:bad()">Link</A></DIV>';
            const expected = '<div><a>Link</a></div>';
            expect(sanitizeHtml(mixedCase)).toBe(expected);
        });

        it("should handle xlink:href attributes", () => {
            const dirty = '<svg><a xlink:href="javascript:alert(1)">Bad SVG Link</a></svg>';
            const expected = '<svg><a>Bad SVG Link</a></svg>';
            expect(sanitizeHtml(dirty)).toBe(expected);
        });

        it("should preserve safe xlink:href attributes", () => {
            const safe = '<svg><a xlink:href="#anchor">Safe SVG Link</a></svg>';
            expect(sanitizeHtml(safe)).toBe(safe);
        });

        it("should validate URLs with improved pattern", () => {
            // Should allow safe protocols
            expect(sanitizeHtml('<a href="https://example.com">HTTPS</a>')).toBe('<a href="https://example.com">HTTPS</a>');
            expect(sanitizeHtml('<a href="http://example.com">HTTP</a>')).toBe('<a href="http://example.com">HTTP</a>');
            expect(sanitizeHtml('<a href="mailto:test@example.com">Email</a>')).toBe('<a href="mailto:test@example.com">Email</a>');
            expect(sanitizeHtml('<a href="tel:+1234567890">Phone</a>')).toBe('<a href="tel:+1234567890">Phone</a>');

            // Should allow relative URLs
            expect(sanitizeHtml('<a href="/path">Relative Path</a>')).toBe('<a href="/path">Relative Path</a>');
            expect(sanitizeHtml('<a href="?query=value">Query</a>')).toBe('<a href="?query=value">Query</a>');
            expect(sanitizeHtml('<a href="#anchor">Anchor</a>')).toBe('<a href="#anchor">Anchor</a>');

            // Should block dangerous protocols
            expect(sanitizeHtml('<a href="javascript:alert(1)">JS</a>')).toBe('<a>JS</a>');
            expect(sanitizeHtml('<a href="data:text/html,<script>alert(1)</script>">Data</a>')).toBe('<a>Data</a>');
            expect(sanitizeHtml('<a href="vbscript:msgbox(1)">VBScript</a>')).toBe('<a>VBScript</a>');
        });

        it("should preserve whitespace between elements", () => {
            // Test whitespace preservation between safe elements
            const withSpace = '<i class="fa fa-something"></i> Sometext';
            expect(sanitizeHtml(withSpace)).toBe(withSpace);

            // Test multiple spaces
            const multipleSpaces = '<span>Before</span>   <span>After</span>';
            expect(sanitizeHtml(multipleSpaces)).toBe(multipleSpaces);

            // Test newlines
            const withNewlines = '<div>Line 1</div>\n<div>Line 2</div>';
            expect(sanitizeHtml(withNewlines)).toBe(withNewlines);

            // Test whitespace around dangerous elements (should preserve whitespace when elements are removed)
            const dangerousWithSpace = '<i class="fa fa-something"></i> <script>evil()</script> Sometext';
            const expectedDangerous = '<i class="fa fa-something"></i>  Sometext';
            expect(sanitizeHtml(dangerousWithSpace)).toBe(expectedDangerous);

            // Note: DOMParser normalizes invalid self-closing tags for non-void elements
            // This is expected HTML parsing behavior, not a bug in the sanitizer
            // Example: '<i class="fa fa-something" /> Sometext' becomes '<i class="fa fa-something"> Sometext</i>'
            // because <i> is not a void element in HTML5
        });
    });

});

