import { EditorUtils } from "./editorutils";
import { TimeEditor, TimeSpanEditor } from "./timeeditor";

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

    it("value getter returns hourAndMin", () => {
        const editor = new TimeSpanEditor({});
        editor["domNode"].value = "10";
        editor["minutes"].val("20");
        expect(editor.value).toBe("10:20");
        expect((editor as any).get_value()).toBe("10:20");
        editor.destroy();
    });

    it("value setter parses value", () => {
        const editor = new TimeSpanEditor({});
        editor.value = "08:05";
        expect(editor["domNode"].value).toBe("8");
        expect(editor["minutes"].val()).toBe("5");
        editor.destroy();
    });

    it("set_value delegates to value", () => {
        const editor = new TimeSpanEditor({});
        (editor as any).set_value("12:30");
        expect(editor.value).toBe("12:30");
        editor.destroy();
    });
});

describe("TimeEditor", () => {
    it("value getter computes minutes", () => {
        const editor = new TimeEditor({});
        editor["domNode"].value = "16";
        editor["minutes"].val("30");
        expect(editor.value).toBe(990);
        expect((editor as any).get_value()).toBe(990);
        editor.destroy();
    });

    it("value setter sets hour and minute", () => {
        const editor = new TimeEditor({});
        editor.value = 990;
        expect(editor["domNode"].value).toBe("16");
        expect(editor["minutes"].val()).toBe("30");
        editor.destroy();
    });

    it("value setter handles null and NaN", () => {
        const editor = new TimeEditor({});
        editor.value = null as any;
        expect(editor["domNode"].value).toBe("");
        editor.value = NaN;
        expect(editor["domNode"].value).toBe("");
        editor.destroy();
    });

    it("hourAndMin getter returns formatted value", () => {
        const editor = new TimeEditor({});
        editor["domNode"].value = "9";
        editor["minutes"].val("5");
        expect(editor.hourAndMin).toBe("09:05");
        editor.destroy();
    });

    it("hourAndMin setter parses value", () => {
        const editor = new TimeEditor({});
        editor.hourAndMin = "16:30";
        expect(editor["domNode"].value).toBe("16");
        expect(editor["minutes"].val()).toBe("30");
        editor.destroy();
    });

    it("hourAndMin setter handles empty value", () => {
        const editor = new TimeEditor({});
        editor.hourAndMin = "";
        expect(editor["domNode"].value).toBe("");
        editor.destroy();
    });

    it("hourAndMin setter handles empty with noEmptyOption", () => {
        const editor = new TimeEditor({ noEmptyOption: true, startHour: 8 } as any);
        editor.hourAndMin = null as any;
        expect(editor["domNode"].value).toBe("8");
        editor.destroy();
    });

    it("hour and minute getters", () => {
        const editor = new TimeEditor({});
        editor["domNode"].value = "14";
        editor["minutes"].val("45");
        expect(editor.hour).toBe(14);
        expect(editor.minute).toBe(45);
        editor.destroy();
    });

    it("set_value delegates to value", () => {
        const editor = new TimeEditor({});
        (editor as any).set_value(60);
        expect(editor["domNode"].value).toBe("1");
        expect(editor["minutes"].val()).toBe("0");
        editor.destroy();
    });
});