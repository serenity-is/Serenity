import { htmlEncode, toggleClass } from "./html";

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
        var el = document.createElement("div");
        el.setAttribute("class", "test1 test2");

        toggleClass(el, null, true);
        expect(el.getAttribute("class")).toBe("test1 test2");

        toggleClass(el, undefined, false);
        expect(el.getAttribute("class")).toBe("test1 test2");

        toggleClass(el, '', false);
        expect(el.getAttribute("class")).toBe("test1 test2");
    });

    it("can toggle single class", () => {
        var el = document.createElement("div");
        el.setAttribute("class", "test1 test2");

        toggleClass(el, "test1");
        expect(el.getAttribute("class")).toBe("test2");

        toggleClass(el, "test3");
        expect(el.getAttribute("class")).toBe("test2 test3");

        toggleClass(el, "test3");
        expect(el.getAttribute("class")).toBe("test2");
    });

    it("can toggle multiple classes", () => {
        var el = document.createElement("div");
        el.setAttribute("class", "test1 test2");

        toggleClass(el, "test1 test2");
        expect(el.getAttribute("class")).toBe("");

        toggleClass(el, "test3 test4");
        expect(el.getAttribute("class")).toBe("test3 test4");

        toggleClass(el, "test5 test3 test6");
        expect(el.getAttribute("class")).toBe("test4 test5 test6");
    });

    it("adds classes if third parameter is true", () => {
        var el = document.createElement("div");
        el.setAttribute("class", "test1 test2");

        toggleClass(el, "test1 test2", true);
        expect(el.getAttribute("class")).toBe("test1 test2");

        toggleClass(el, "test3 test4", true);
        expect(el.getAttribute("class")).toBe("test1 test2 test3 test4");

        toggleClass(el, "test5 test3 test6", true);
        expect(el.getAttribute("class")).toBe("test1 test2 test3 test4 test5 test6");
    });

    it("removes classes if third parameter is false", () => {
        var el = document.createElement("div");
        el.setAttribute("class", "test1 test2");

        toggleClass(el, "test1 test2", false);
        expect(el.getAttribute("class")).toBe("");
    });

    it("ignores whitespace", () => {
        var el = document.createElement("div");
        el.setAttribute("class", "test1 test2");

        toggleClass(el, "test1      test2", false);
        expect(el.getAttribute("class")).toBe("");
    });       
});