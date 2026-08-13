import { describe, it, expect } from "vitest";
import { StringEditor } from "./stringeditor";

describe("StringEditor", () => {
    it("creates a text input with empty value", () => {
        const editor = new StringEditor({});
        expect(editor.domNode).toBeInstanceOf(HTMLInputElement);
        expect(editor.domNode.type).toBe("text");
        expect(editor.value).toBe("");
        editor.destroy();
    });

    it("gets and sets value", () => {
        const editor = new StringEditor({});
        editor.value = "abc";
        expect(editor.value).toBe("abc");
        expect((editor as any).get_value()).toBe("abc");
        editor.destroy();
    });

    it("sets null value to empty string", () => {
        const editor = new StringEditor({});
        editor.value = null as any;
        expect(editor.value).toBe("");
        editor.destroy();
    });

    it("set_value updates the value", () => {
        const editor = new StringEditor({});
        (editor as any).set_value("xyz");
        expect(editor.value).toBe("xyz");
        editor.destroy();
    });

    it("readOnly getter and setter use the element", () => {
        const editor = new StringEditor({});
        expect(editor.readOnly).toBe(false);
        editor.readOnly = true;
        expect(editor.readOnly).toBe(true);
        editor.destroy();
    });
});