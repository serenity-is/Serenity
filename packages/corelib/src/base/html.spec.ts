import { addClass, appendToNode, htmlEncode, removeClass, toggleClass } from "./html";

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