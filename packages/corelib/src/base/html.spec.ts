import { Config } from "./config";
import { addClass, appendToNode, getElementReadOnly, getReturnUrl, htmlEncode, parseQueryString, removeClass, sanitizeUrl, setElementReadOnly, toggleClass } from "./html";

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

    it("handles rejected promise", async () => {
        const parent = document.createElement("div");
        const div = document.createElement("div");
        div.innerHTML = "test";
        appendToNode(parent, Promise.reject("some reject reason"));
        expect(parent.innerHTML).toBe("<!--Loading content...-->");
        await Promise.resolve();
        expect(parent.innerHTML).toBe("<!--Error loading content: some reject reason-->");
    });

    it("calls append for other content types", () => {
        const parent = document.createElement("div");
        const content = { abc: 5 };
        appendToNode(parent, content);
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
        var oldLocation = window.location.href;
        changeJSDOMURL("http://localhost?param1=value1&param2=value2");
        try {
            const result = parseQueryString();
            expect(result).toEqual({ param1: "value1", param2: "value2" });
        } finally {
            changeJSDOMURL(oldLocation);
        }
    });
});

describe("getReturnUrl", () => {
    it("returns returnUrl from query string if it is safe", () => {
        const oldLocation = window.location.href;
        changeJSDOMURL("http://localhost?returnUrl=/safe/path");
        try {
            const result = getReturnUrl();
            expect(result).toBe("/safe/path");
        } finally {
            changeJSDOMURL(oldLocation);
        }
    });

    it("returns null if returnUrl from query string is unsafe", () => {
        const oldLocation = window.location.href;
        changeJSDOMURL("http://localhost?returnUrl=http://unsafe.com");
        try {
            const result = getReturnUrl();
            expect(result).toBeNull();
        } finally {
            changeJSDOMURL(oldLocation);
        }
    });

    it("returns default returnUrl if queryOnly is false and no returnUrl in query string", () => {
        const oldLocation = window.location.href;
        changeJSDOMURL("http://localhost");
        try {
            const result = getReturnUrl({ queryOnly: false });
            expect(result).toBe(Config.defaultReturnUrl());
        } finally {
            changeJSDOMURL(oldLocation);
        }
    });

    it("returns undefined if queryOnly is true and no returnUrl in query string", () => {
        const oldLocation = window.location.href;
        changeJSDOMURL("http://localhost");
        try {
            const result = getReturnUrl({ queryOnly: true });
            expect(result).toBeUndefined();
        } finally {
            changeJSDOMURL(oldLocation);
        }
    });

    it("returns returnUrl from query string if ignoreUnsafe is true", () => {
        const oldLocation = window.location.href;
        changeJSDOMURL("http://localhost?returnUrl=http://unsafe.com");
        try {
            const result = getReturnUrl({ ignoreUnsafe: true });
            expect(result).toBe("http://unsafe.com");
        } finally {
            changeJSDOMURL(oldLocation);
        }
    });

    it("returns default returnUrl with purpose if provided", () => {
        const oldLocation = window.location.href;
        changeJSDOMURL("http://localhost");
        try {
            const result = getReturnUrl({ purpose: "test" });
            expect(result).toBe(Config.defaultReturnUrl("test"));
        } finally {
            changeJSDOMURL(oldLocation);
        }
    });
});

function changeJSDOMURL(url: string) {
    (globalThis as any).jsdom.reconfigure({ url: url });
}
