import { BooleanEditor } from "./booleaneditor";

describe("BooleanEditor", () => {
    it("updates value when checkbox is checked", () => {
        var editor = new BooleanEditor({});
        editor.domNode.checked = true;
        expect(editor.value).toBe(true);
        editor.destroy();
    })

    it("updates value when set is used", () => {
        var editor = new BooleanEditor({});
        editor.value = true;
        expect(editor.value).toBe(true);
        expect(editor.domNode.checked).toBe(true);
        editor.destroy();
    })

    it("get_value returns checked state", () => {
        var editor = new BooleanEditor({});
        editor.domNode.checked = true;
        expect((editor as any).get_value()).toBe(true);
        editor.destroy();
    })

    it("set_value sets checked state", () => {
        var editor = new BooleanEditor({});
        (editor as any).set_value(true);
        expect(editor.domNode.checked).toBe(true);
        (editor as any).set_value(false);
        expect(editor.domNode.checked).toBe(false);
        editor.destroy();
    })
});