import { BooleanEditor } from "./booleaneditor";

describe("BooleanEditor", () => {
    it("updates value when checkbox is checked", () => {
        var editor = new BooleanEditor({});
        editor.domNode.checked = true;
        expect(editor.value).toBe(true);
    })

    it("updates value when set is used", () => {
        var editor = new BooleanEditor({});
        editor.value = true;
        expect(editor.value).toBe(true);
        expect(editor.domNode.checked).toBe(true);
    })
});