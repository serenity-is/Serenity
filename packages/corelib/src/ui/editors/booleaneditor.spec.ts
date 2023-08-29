import { BooleanEditor } from "./booleaneditor";

describe("BooleanEditor", () => {
    it("updates value when checkbox is checked", () => {
        var editor = new BooleanEditor($("<input type='checkbox'/>"));
        editor.element.prop("checked", true);
        expect(editor.value).toBe(true);
    })

    it("updates value when set is used", () => {
        var editor = new BooleanEditor($("<input type='checkbox'/>"));
        editor.value = true;
        expect(editor.value).toBe(true);
        expect(editor.element.prop("checked")).toBe(true);
    })
});