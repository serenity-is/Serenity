import { htmlEncode } from "./html";

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