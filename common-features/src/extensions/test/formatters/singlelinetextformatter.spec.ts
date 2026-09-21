import { describe, expect, it } from "vitest";
import { SingleLineTextFormatter } from "../../Modules/Formatters/SingleLineTextFormatter";

describe("SingleLineTextFormatter", () => {
    it("formats the ctx value", () => {
        const formatter = new SingleLineTextFormatter();
        expect(formatter.format({ value: "hello" } as any)).toBe("hello");
    });

    it("strips html tags and collapses newlines", () => {
        expect(SingleLineTextFormatter.formatValue("<div>line1</div> line2")).toBe("line1 line2");
        expect(SingleLineTextFormatter.formatValue("line1\r\nline2\nline3")).toBe("line1 line2 line3");
    });

    it("handles null value", () => {
        expect(SingleLineTextFormatter.formatValue(null)).toBe("");
    });

    it("html encodes special characters", () => {
        expect(SingleLineTextFormatter.formatValue("<b>&amp;</b>")).toBe("&amp;");
    });
});
