import { describe, it, expect } from "vitest";
import { TextAreaEditor } from "./textareaeditor";

describe("TextAreaEditor", () => {
    it("creates textarea with default cols and rows", () => {
        const editor = new TextAreaEditor({});
        expect(editor.domNode.tagName).toBe("TEXTAREA");
        expect(editor.domNode.getAttribute("cols")).toBe("80");
        expect(editor.domNode.getAttribute("rows")).toBe("6");
        editor.destroy();
    });

    it("skips cols and rows when zero", () => {
        const editor = new TextAreaEditor({ cols: 0, rows: 0 } as any);
        expect(editor.domNode.getAttribute("cols")).toBeNull();
        expect(editor.domNode.getAttribute("rows")).toBeNull();
        editor.destroy();
    });

    it("uses custom cols and rows", () => {
        const editor = new TextAreaEditor({ cols: 10, rows: 3 } as any);
        expect(editor.domNode.getAttribute("cols")).toBe("10");
        expect(editor.domNode.getAttribute("rows")).toBe("3");
        editor.destroy();
    });

    it("gets and sets value", () => {
        const editor = new TextAreaEditor({});
        editor.value = "hello";
        expect(editor.value).toBe("hello");
        expect((editor as any).get_value()).toBe("hello");
        editor.value = null as any;
        expect(editor.value).toBe("");
        editor.destroy();
    });
});