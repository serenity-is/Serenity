import { describe, expect, it } from "vitest";
import { BooleanEditor, CheckLabelPosition } from "./booleaneditor";

describe("BooleanEditor", () => {
    it("updates value when checkbox is checked", () => {
        const editor = new BooleanEditor({});
        editor.domNode.checked = true;
        expect(editor.value).toBe(true);
        editor.destroy();
    })

    it("updates value when set is used", () => {
        const editor = new BooleanEditor({});
        editor.value = true;
        expect(editor.value).toBe(true);
        expect(editor.domNode.checked).toBe(true);
        editor.destroy();
    })

    it("has no label classes by default or at the start", () => {
        for (const options of [{}, { labelPosition: CheckLabelPosition.Start }]) {
            const editor = new BooleanEditor(options);
            expect(editor.domNode.classList.contains("label-on-end")).toBe(false);
            expect(editor.domNode.classList.contains("align-with-editors")).toBe(false);
            editor.destroy();
        }
    })

    it("adds the label-on-end class when the label is on the end, aligned or not", () => {
        const end = new BooleanEditor({ labelPosition: CheckLabelPosition.End });
        expect(end.domNode.classList.contains("label-on-end")).toBe(true);
        expect(end.domNode.classList.contains("align-with-editors")).toBe(false);
        end.destroy();

        const aligned = new BooleanEditor({ labelPosition: CheckLabelPosition.EndAligned });
        expect(aligned.domNode.classList.contains("label-on-end")).toBe(true);
        expect(aligned.domNode.classList.contains("align-with-editors")).toBe(true);
        aligned.destroy();
    })

    it("reads the position as an enum editor saves it, as a string", () => {
        const editor = new BooleanEditor({ labelPosition: String(CheckLabelPosition.EndAligned) as any });
        expect(editor.domNode.classList.contains("align-with-editors")).toBe(true);
        editor.destroy();
    })

    it("get_value returns checked state", () => {
        const editor = new BooleanEditor({});
        editor.domNode.checked = true;
        expect((editor as any).get_value()).toBe(true);
        editor.destroy();
    })

    it("set_value sets checked state", () => {
        const editor = new BooleanEditor({});
        (editor as any).set_value(true);
        expect(editor.domNode.checked).toBe(true);
        (editor as any).set_value(false);
        expect(editor.domNode.checked).toBe(false);
        editor.destroy();
    })
});