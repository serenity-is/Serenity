import { describe, expect, it } from "vitest";
import { StaticTextBlock } from "../../Modules/Widgets/StaticTextBlock";

describe("StaticTextBlock", () => {
    it("creates a div default element", () => {
        const block = new StaticTextBlock({} as any);
        expect(block.domNode.tagName).toBe("DIV");
        block.destroy();
    });

    it("renders plain text", () => {
        const block = new StaticTextBlock({ text: "Hello" } as any);
        expect(block.domNode.textContent).toBe("Hello");
        block.destroy();
    });

    it("renders sanitized html when isHtml is set", () => {
        const block = new StaticTextBlock({ text: "<b>bold</b>", isHtml: true } as any);
        expect(block.domNode.innerHTML).toBe("<b>bold</b>");
        block.destroy();
    });

    it("resolves local text when isLocalText is set", () => {
        const block = new StaticTextBlock({ text: "Some.Key", isLocalText: true } as any);
        expect(block.domNode.textContent).toBe("Some.Key");
        block.destroy();
    });

    it("hides the caption label when hideLabel is set", () => {
        const field = document.createElement("div");
        field.className = "field";
        const caption = document.createElement("label");
        caption.className = "caption";
        field.appendChild(caption);
        const element = document.createElement("div");
        field.appendChild(element);

        const block = new StaticTextBlock({ element, hideLabel: true } as any);
        expect(caption.hidden).toBe(true);
        block.destroy();
    });

    it("setEditValue uses source value when text option is not set", () => {
        const block = new StaticTextBlock({} as any);
        block.setEditValue({ Value: "from source" }, { name: "Value" } as any);
        expect(block.domNode.textContent).toBe("from source");
        block.destroy();
    });

    it("setEditValue ignores source value when text option is set", () => {
        const block = new StaticTextBlock({ text: "fixed" } as any);
        block.setEditValue({ Value: "from source" }, { name: "Value" } as any);
        expect(block.domNode.textContent).toBe("fixed");
        block.destroy();
    });
});

