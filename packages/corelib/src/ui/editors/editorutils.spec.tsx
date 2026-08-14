import { Fluent } from "../../base";
import { UploadHelper } from "../helpers/uploadhelper";
import { Widget } from "../widgets/widget";
import { BooleanEditor } from "./booleaneditor";
import { Combobox } from "./combobox";
import { ComboboxEditor } from "./comboboxeditor";
import { DecimalEditor } from "./decimaleditor";
import { EditorUtils } from "./editorutils";
import { StringEditor } from "./stringeditor";
import { FileUploadEditor } from "./uploadeditors";

describe("EditorUtils.setReadOnly", () => {
    it("can set readonly for single element", () => {
        const el = document.createElement("div");
        EditorUtils.setReadOnly(el, true);
        expect(el.classList.contains("readonly")).toBeTruthy();
        expect(el.hasAttribute("readonly")).toBeTruthy();
    });

    it("can unset readonly for single element", () => {
        const el = document.createElement("div");
        el.classList.add("readonly");
        el.setAttribute("readonly", "readonly");
        EditorUtils.setReadOnly(el, false);
        expect(el.classList.contains("readonly")).toBeFalsy();
        expect(el.hasAttribute("readonly")).toBeFalsy();
    });

    it("can set readonly for Fluent element", () => {
        const el = document.createElement("div");
        EditorUtils.setReadOnly(Fluent(el), true);
        expect(el.classList.contains("readonly")).toBeTruthy();
        expect(el.hasAttribute("readonly")).toBeTruthy();
    });

    it("can unset readonly for Fluent element", () => {
        const el = document.createElement("div");
        el.classList.add("readonly");
        el.setAttribute("readonly", "readonly");
        EditorUtils.setReadOnly(Fluent(el), false);
        expect(el.classList.contains("readonly")).toBeFalsy();
        expect(el.hasAttribute("readonly")).toBeFalsy();
    });

    it("can unset readonly for Fluent element", () => {
        const el = document.createElement("div");
        el.classList.add("readonly");
        el.setAttribute("readonly", "readonly");
        EditorUtils.setReadOnly(Fluent(el), false);
        expect(el.classList.contains("readonly")).toBeFalsy();
        expect(el.hasAttribute("readonly")).toBeFalsy();
    });

    it("calls set_readOnly if available in the widget", () => {
        class MyWidget extends Widget {
            set_readOnly = vi.fn();
        }
        const widget = new MyWidget({});
        EditorUtils.setReadOnly(widget, true);
        expect(widget.set_readOnly).toHaveBeenCalledTimes(1);
        expect(widget.set_readOnly).toHaveBeenCalledWith(true);
        EditorUtils.setReadOnly(widget, false);
        expect(widget.set_readOnly).toHaveBeenCalledTimes(2);
        expect(widget.set_readOnly).toHaveBeenLastCalledWith(false);
    });

    it("skips null elements", () => {
        expect(() => EditorUtils.setReadOnly(null, true)).not.toThrow();
        expect(() => EditorUtils.setReadOnly([null], true)).not.toThrow();
        expect(() => EditorUtils.setReadOnly([document.createElement("div"), null, document.createElement("div")], true)).not.toThrow();
    });

    it("skips widgets with null domNode", () => {
        const widget = new Widget({});
        widget.destroy();
        expect(() => EditorUtils.setReadOnly(widget, true)).not.toThrow();
    });

    it("works with widgets that does not have a set_readOnly method", () => {
        class MyWidget extends Widget {
        }
        const widget = new MyWidget({});
        EditorUtils.setReadOnly(widget, true);
        expect(widget.domNode.classList.contains("readonly")).toBeTruthy();
        expect(widget.domNode.hasAttribute("readonly")).toBeTruthy();
    });

    it("finds attached widget from the element", () => {
        class MyWidget extends Widget {
            set_readOnly = vi.fn();
        }
        const widget = new MyWidget({});
        EditorUtils.setReadOnly(widget.domNode, true);
        expect(widget.set_readOnly).toHaveBeenCalledTimes(1);
        expect(widget.set_readOnly).toHaveBeenCalledWith(true);
        EditorUtils.setReadOnly(widget, false);
        expect(widget.set_readOnly).toHaveBeenCalledTimes(2);
        expect(widget.set_readOnly).toHaveBeenLastCalledWith(false);
    });

    it("reads and writes values from input-like editors", () => {
        const input = document.createElement("input");
        input.value = "before";
        const editor: any = { domNode: input };

        expect(EditorUtils.getValue(editor)).toBe("before");
        EditorUtils.setValue(editor, "after");
        expect(input.value).toBe("after");
        expect(EditorUtils.getDisplayText(editor)).toBe("after");
    });

    it("uses legacy getEditValue and setEditValue hooks", () => {
        const input = document.createElement("div");
        const editor: any = {
            domNode: input,
            getEditValue: (item: any, target: any) => target[item.name] = "saved",
            setEditValue: (source: any, item: any) => input.textContent = source[item.name]
        };

        expect(EditorUtils.getValue(editor)).toBe("saved");
        EditorUtils.setValue(editor, "loaded");
        expect(input.textContent).toBe("loaded");
    });

    it("adds and removes required field markers", () => {
        const field = document.createElement("div");
        field.className = "field";
        field.innerHTML = "<div class='caption'></div><input>";
        document.body.appendChild(field);
        const editor: any = { domNode: field.querySelector("input") };

        EditorUtils.setRequired(editor, true);
        expect(editor.domNode.classList.contains("required")).toBe(true);
        expect(field.querySelector("sup")).toBeTruthy();
        EditorUtils.setRequired(editor, false);
        expect(editor.domNode.classList.contains("required")).toBe(false);
        expect(field.querySelector("sup")).toBeNull();
        field.remove();
    });

    it("toggles readonly state for an editor container", () => {
        const container = document.createElement("div");
        const first = document.createElement("input");
        const second = document.createElement("input");
        first.className = second.className = "editor";
        second.readOnly = true;
        container.append(first, second);
        document.body.appendChild(container);

        EditorUtils.setContainerReadOnly(container, true);
        expect(container.classList.contains("readonly-container")).toBe(true);
        expect(first.classList.contains("container-readonly")).toBe(true);
        expect(second.classList.contains("container-readonly")).toBe(false);
        EditorUtils.setContainerReadOnly(container, false);
        expect(container.classList.contains("readonly-container")).toBe(false);
        expect(first.classList.contains("container-readonly")).toBe(false);
        container.remove();
    });

    it("serializes interface-based editor values", () => {
        const item: any = { name: "value" };
        const getEditor: any = { domNode: document.createElement("div"), getEditValue: vi.fn((item, target) => target[item.name] = "edit") };
        expect(EditorUtils.getValue(getEditor)).toBe("edit");

        const stringValue: any = { domNode: document.createElement("div"), getEditValue: (_: any, target: any) => target.value = "text", setEditValue: vi.fn() };
        EditorUtils.saveValue(stringValue, item, {});
        EditorUtils.loadValue(stringValue, item, { value: 12 });
        expect(stringValue.setEditValue).toHaveBeenCalledWith({ value: 12 }, item);

        const booleanValue: any = { domNode: document.createElement("div"), getEditValue: (_: any, target: any) => target.value = true, setEditValue: vi.fn() };
        EditorUtils.saveValue(booleanValue, item, {});
        EditorUtils.loadValue(booleanValue, item, { value: 1 });
        expect(booleanValue.setEditValue).toHaveBeenCalledWith({ value: 1 }, item);

        const doubleValue: any = { domNode: document.createElement("div"), getEditValue: (_: any, target: any) => target.value = NaN, setEditValue: vi.fn() };
        const target: any = {};
        EditorUtils.saveValue(doubleValue, item, target);
        expect(target.value).toBeNaN();
        EditorUtils.loadValue(doubleValue, item, { value: "  " });
        EditorUtils.loadValue(doubleValue, item, { value: "12.5" });
        EditorUtils.loadValue(doubleValue, item, { value: true });
        expect(doubleValue.setEditValue).toHaveBeenLastCalledWith({ value: true }, item);
    });

    it("formats boolean and null display values", () => {
        const editor: any = { domNode: document.createElement("input"), getEditValue: (item: any, target: any) => target[item.name] = true };
        expect(EditorUtils.getDisplayText(editor)).toBe("True");
        const falseEditor: any = { domNode: document.createElement("input"), getEditValue: (item: any, target: any) => target[item.name] = false };
        expect(EditorUtils.getDisplayText(falseEditor)).toBe("False");
        const nullEditor: any = { domNode: document.createElement("input"), getEditValue: (item: any, target: any) => target[item.name] = null };
        expect(EditorUtils.getDisplayText(nullEditor)).toBe("");
    });

    it("formats display text from an attached combobox", () => {
        const getInstanceSpy = vi.spyOn(Combobox, "getInstance");
        const editor: any = { domNode: document.createElement("input") };
        getInstanceSpy.mockReturnValue({ getSelectedItems: () => [{ text: "One" }, { text: "Two" }] } as any);
        expect(EditorUtils.getDisplayText(editor)).toBe("One, Two");
        getInstanceSpy.mockReturnValue({ getSelectedItems: () => null } as any);
        expect(EditorUtils.getDisplayText(editor)).toBe("");
        getInstanceSpy.mockRestore();
    });

    it("serializes values through the string interface", () => {
        const editor = new StringEditor({});
        const item: any = { name: "Field" };
        const target: any = {};
        editor.value = "abc";
        EditorUtils.saveValue(editor, item, target);
        expect(target.Field).toBe("abc");
        EditorUtils.loadValue(editor, item, { Field: 5 });
        expect(editor.value).toBe("5");
        EditorUtils.loadValue(editor, item, { Field: null });
        expect(editor.value).toBe("");
        editor.destroy();
    });

    it("serializes values through the boolean interface", () => {
        const editor = new BooleanEditor({});
        const item: any = { name: "Field" };
        const target: any = {};
        editor.value = true;
        EditorUtils.saveValue(editor, item, target);
        expect(target.Field).toBe(true);
        EditorUtils.loadValue(editor, item, { Field: 1 });
        expect(editor.value).toBe(true);
        EditorUtils.loadValue(editor, item, { Field: 0 });
        expect(editor.value).toBe(false);
        EditorUtils.loadValue(editor, item, { Field: "x" });
        expect(editor.value).toBe(true);
        editor.destroy();
    });

    it("serializes values through the double interface", () => {
        const editor = new DecimalEditor({});
        const item: any = { name: "Field" };
        const target: any = {};
        EditorUtils.loadValue(editor, item, { Field: null });
        expect(editor.value).toBeNull();
        EditorUtils.loadValue(editor, item, { Field: "  " });
        expect(editor.value).toBeNull();
        EditorUtils.loadValue(editor, item, { Field: "12.5" });
        expect(editor.value).toBe(12.5);
        EditorUtils.loadValue(editor, item, { Field: true });
        expect(editor.value).toBe(1);
        EditorUtils.loadValue(editor, item, { Field: 3 });
        expect(editor.value).toBe(3);
        EditorUtils.saveValue(editor, item, target);
        expect(target.Field).toBe(3);
        editor.destroy();
    });

    it("serializes values through the edit-value interfaces", () => {
        class TestCombo extends ComboboxEditor<any, any> { }
        const editor: any = new TestCombo({});
        editor["combobox"] = { getValue: vi.fn(() => "1"), setValue: vi.fn(), dispose: vi.fn() } as any;
        const item: any = { name: "Field" };
        const target: any = {};
        EditorUtils.saveValue(editor, item, target);
        expect(target.Field).toBe("1");
        EditorUtils.loadValue(editor, item, { Field: 2 });
        expect(editor["combobox"].setValue).toHaveBeenCalledWith("2", true);
        editor.destroy();
    });

    it("marks required through the required interface", () => {
        vi.spyOn(UploadHelper, "addUploadInput").mockImplementation(() => Fluent(document.createElement("input")));
        const editor = new FileUploadEditor({ element: el => document.body.appendChild(el) } as any);
        EditorUtils.setRequired(editor, true);
        expect(editor.get_required()).toBe(true);
        EditorUtils.setRequired(editor, false);
        expect(editor.get_required()).toBe(false);
        editor.destroy();
        vi.restoreAllMocks();
    });

    it("formats numeric display values via toString", () => {
        const editor: any = { domNode: document.createElement("input"), getEditValue: (item: any, target: any) => target[item.name] = 42 };
        expect(EditorUtils.getDisplayText(editor)).toBe("42");
    });

    it("returns early when clearing a non-readonly container", () => {
        const container = document.createElement("div");
        document.body.appendChild(container);
        expect(() => EditorUtils.setContainerReadOnly(container, false)).not.toThrow();
        expect(container.classList.contains("readonly-container")).toBe(false);
        container.remove();
    });

    it("skips readonly and disabled elements when making containers readonly", () => {
        const container = document.createElement("div");
        const ro = document.createElement("input");
        ro.className = "editor";
        ro.setAttribute("readonly", "readonly");
        const dis = document.createElement("input");
        dis.className = "editor";
        dis.setAttribute("disabled", "disabled");
        container.append(ro, dis);
        document.body.appendChild(container);
        EditorUtils.setContainerReadOnly(container, true);
        expect(ro.classList.contains("container-readonly")).toBe(false);
        expect(dis.classList.contains("container-readonly")).toBe(false);
        container.remove();
    });

    it("skips widget elements without get_readOnly that are readonly", () => {
        class PlainWidget extends Widget { }
        const container = document.createElement("div");
        const el = document.createElement("input");
        el.className = "editor";
        el.setAttribute("readonly", "readonly");
        new PlainWidget({ element: el });
        container.append(el);
        document.body.appendChild(container);
        EditorUtils.setContainerReadOnly(container, true);
        expect(el.classList.contains("container-readonly")).toBe(false);
        container.remove();
    });

    it("handles widgets while making containers readonly", () => {
        class ReadOnlyWidget extends Widget {
            public ro = false;
            get_readOnly() { return this.ro; }
            set_readOnly(value: boolean) { this.ro = value; }
        }
        const container = document.createElement("div");
        const first = document.createElement("div");
        first.className = "editor";
        const widget = new ReadOnlyWidget({ element: first });
        const second = document.createElement("div");
        second.className = "editor";
        const readonlyWidget = new ReadOnlyWidget({ element: second });
        readonlyWidget.ro = true;
        container.append(first, second);
        document.body.appendChild(container);

        EditorUtils.setContainerReadOnly(container, true);
        expect(first.classList.contains("container-readonly")).toBe(true);
        expect(widget.ro).toBe(true);
        expect(second.classList.contains("container-readonly")).toBe(false);
        expect(readonlyWidget.ro).toBe(true);
        EditorUtils.setContainerReadOnly(container, false);
        expect(first.classList.contains("container-readonly")).toBe(false);
        container.remove();
    });
});