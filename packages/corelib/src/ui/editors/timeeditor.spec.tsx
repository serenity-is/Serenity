import { EditorUtils } from "./editorutils";
import { TimeSpanEditor } from "./timeeditor";

describe("TimeSpanEditor", () => {
    it("sets readonly and disabled when setting to readonly", () => {
        const editor = new TimeSpanEditor({});
        expect(editor.domNode.classList.contains("readonly")).toBe(false);
        expect(editor.domNode.hasAttribute("disabled")).toBe(false);
        expect(editor["minutes"].getNode().classList.contains("readonly")).toBe(false);
        expect(editor["minutes"].getNode().hasAttribute("disabled")).toBe(false);

        editor.readOnly = true;

        expect(editor.domNode.classList.contains("readonly")).toBe(true);
        expect(editor.domNode.hasAttribute("disabled")).toBe(true);
        expect(editor["minutes"].getNode().classList.contains("readonly")).toBe(true);
        expect(editor["minutes"].getNode().hasAttribute("disabled")).toBe(true);
    });

    it("clears readonly and disabled when setting readonly to false again", () => {
        const editor = new TimeSpanEditor({});
        editor.readOnly = true;
        
        expect(editor.domNode.classList.contains("readonly")).toBe(true);
        expect(editor.domNode.hasAttribute("disabled")).toBe(true);
        expect(editor["minutes"].getNode().classList.contains("readonly")).toBe(true);
        expect(editor["minutes"].getNode().hasAttribute("disabled")).toBe(true);
        
        editor.readOnly = false;

        expect(editor.domNode.classList.contains("readonly")).toBe(false);
        expect(editor.domNode.hasAttribute("disabled")).toBe(false);
        expect(editor["minutes"].getNode().classList.contains("readonly")).toBe(false);
        expect(editor["minutes"].getNode().hasAttribute("disabled")).toBe(false);
    });

    it("works when setting to readOnly via EditorUtils", () => {
        const editor = new TimeSpanEditor({});
        EditorUtils.setReadOnly(editor, true);

        expect(editor.domNode.classList.contains("readonly")).toBe(true);
        expect(editor.domNode.hasAttribute("disabled")).toBe(true);
        expect(editor["minutes"].getNode().classList.contains("readonly")).toBe(true);
        expect(editor["minutes"].getNode().hasAttribute("disabled")).toBe(true);
    });    
});