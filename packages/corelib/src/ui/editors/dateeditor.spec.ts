import { Fluent } from "../../base";
import { mockJQuery } from "../../test/mocks";
import { DateEditor, type DateEditorOptions } from "./dateeditor";

beforeEach(() => {
    vi.useRealTimers();
    document.body.innerHTML = "";
    delete (window as any)["$"]
    delete (window as any)["jQuery"];
    delete (window as any)["flatpickr"];
    delete (window as any)["bootstrap"];
});

const newEditor = async (opt: DateEditorOptions) => new (await import("./dateeditor")).DateEditor(opt);


function commonTests() {

    it("gets current value correctly", async () => {
        var editor = await newEditor({});
        editor.domNode.value = "2020-01-01";
        expect(editor.value).toEqual("2020-01-01");
    })

    it("sets value to empty string if it is set to null", async () => {
        var editor = await newEditor({});
        editor.value = null;
        expect(editor.value).toEqual(null);
    })

    it("sets value to now if given value is now or today", async () => {
        var editor = await newEditor({});
        vi.useFakeTimers({ now: new Date(2020, 0, 1) });
        editor.value = "now";
        expect(editor.value).toEqual("2020-01-01");
        editor.value = "today";
        expect(editor.value).toEqual("2020-01-01");
    })

    it("gets current value correctly", async () => {
        var editor = await newEditor({});
        editor.domNode.value = "2020-01-01";

        expect(editor.value).toEqual("2020-01-01");
    })

    it("sets value when valueAsDate used", async () => {
        var editor = await newEditor({});
        editor.valueAsDate = new Date(2020, 0, 1);
        expect(editor.get_value()).toEqual("2020-01-01");
    })

    it("sets value when valueAsDate used", async () => {
        var editor = await newEditor({});
        editor.domNode.value = "2020-01-01";
        expect(editor.valueAsDate).toEqual(new Date(2020, 0, 1));
    })
}


describe("DateEditor_WithDefaultHtmlInput", () => {

    it("uses default date input if flatpicker or jquery date picker is not found", async () => {
        var editor = await newEditor({});
        expect(editor.domNode.getAttribute("type")).toBe("date");
    });

    commonTests();
})

describe("DateEditor additional behavior", () => {
    it("handles valueAsDate null and date bounds", async () => {
        const editor = await newEditor({});

        editor.valueAsDate = null;
        expect(editor.value).toBeNull();

        editor.set_minValue("2020-01-01");
        editor.set_maxValue("2020-12-31");
        expect(editor.get_minValue()).toBe("2020-01-01");
        expect(editor.get_maxValue()).toBe("2020-12-31");
        expect(editor.get_minDate()).toEqual(new Date(2020, 0, 1));
        expect(editor.get_maxDate()).toEqual(new Date(2020, 11, 31));

        editor.set_minDate(new Date(2021, 0, 1));
        editor.set_maxDate(new Date(2021, 11, 31));
        expect(editor.get_minValue()).toBe("2021-01-01");
        expect(editor.get_maxValue()).toBe("2021-12-31");
        editor.destroy();
    });

    it("sets and clears SQL min/max bounds", async () => {
        const editor = await newEditor({});
        editor.set_sqlMinMax(true);
        expect(editor.get_sqlMinMax()).toBe(true);
        editor.set_sqlMinMax(false);
        expect(editor.get_minValue()).toBeNull();
        expect(editor.get_maxValue()).toBeNull();
        expect(editor.get_sqlMinMax()).toBe(false);
        editor.destroy();
    });

    it("toggles readonly state", async () => {
        const editor = await newEditor({});
        expect(editor.get_readOnly()).toBe(false);
        editor.set_readOnly(true);
        expect(editor.get_readOnly()).toBe(true);
        editor.set_readOnly(false);
        expect(editor.get_readOnly()).toBe(false);
        editor.destroy();
    });

    it("setToToday updates the value and optionally triggers change", async () => {
        const editor = await newEditor({});
        const changeSpy = vi.spyOn(Fluent, "trigger");
        editor.setToToday(true);
        expect(editor.value).toBeTruthy();
        expect(changeSpy).toHaveBeenCalledWith(editor.domNode, "change");
        editor.destroy();
    });

    it("flatpickr options include modal append target and readonly disable callback", async () => {
        const editor = await newEditor({});
        const modal = document.createElement("div");
        modal.className = "modal";
        modal.appendChild(editor.domNode);
        const options = editor.getFlatpickrOptions(editor.domNode);
        expect(options.appendTo).toBe(modal);
        expect(options.disable[0](new Date())).toBe(false);
        editor.set_readOnly(true);
        expect(options.disable[0](new Date())).toBe(true);
        editor.destroy();
    });

    it("destroys an attached flatpickr instance", async () => {
        const editor = await newEditor({});
        const destroy = vi.fn();
        const domNode = editor.domNode;
        (domNode as any)._flatpickr = { destroy };
        editor.destroy();
        expect(destroy).toHaveBeenCalled();
        expect((domNode as any)._flatpickr).toBeUndefined();
    });

    it("handles space key and ignores readonly keyboard input", async () => {
        const editor = await newEditor({});
        const todaySpy = vi.spyOn(editor, "setToToday").mockImplementation(() => { });
        editor.domNode.dispatchEvent(new KeyboardEvent("keydown", { key: "a" }));
        editor.domNode.dispatchEvent(new KeyboardEvent("keyup", { key: "1" }));
        const event = new KeyboardEvent("keydown", { key: " ", cancelable: true });
        editor.domNode.dispatchEvent(event);
        expect(todaySpy).toHaveBeenCalledWith(true);

        editor.set_readOnly(true);
        todaySpy.mockClear();
        editor.domNode.dispatchEvent(new KeyboardEvent("keydown", { key: " " }));
        expect(todaySpy).not.toHaveBeenCalled();
        editor.domNode.dispatchEvent(new KeyboardEvent("keyup", { key: "1" }));
        editor.destroy();
    });

    it("invokes flatpickr option callbacks", async () => {
        const editor = await newEditor({});
        const options = editor.getFlatpickrOptions(editor.domNode);
        expect(() => options.onChange()).not.toThrow();
        editor.set_readOnly(false);
        expect(options.disable[0](new Date())).toBe(false);
        editor.set_readOnly(true);
        editor.domNode.value = "2000-01-01";
        expect(options.disable[0](new Date(2020, 0, 1))).toBe(true);
        editor.destroy();
    });

    it("supports jQuery datepicker beforeShow callback", async () => {
        const $ = mockJQuery({
            datepicker: vi.fn().mockImplementation(function () { return this; })
        });
        const editor = await newEditor({ yearRange: "-10:+10" });
        const datepicker = ($.fn.datepicker as any).mock;
        const options = datepicker.calls[0][0];
        expect(options.yearRange).toBe("-10:+10");
        expect(options.beforeShow(null, null)).toBe(true);
        editor.set_readOnly(true);
        expect(options.beforeShow(null, null)).toBe(false);
        editor.destroy();
    });

    it("static date input handlers and picker workaround are callable", async () => {
        const editor = await newEditor({});
        expect(() => DateEditor.dateInputChange({ target: editor.domNode } as any)).not.toThrow();
        expect(() => DateEditor.dateInputKeyup({ target: editor.domNode } as any)).not.toThrow();
        expect(() => DateEditor.uiPickerZIndexWorkaround(null)).not.toThrow();
        expect(() => DateEditor.uiPickerZIndexWorkaround([editor.domNode])).not.toThrow();
        editor.destroy();
    });
});


describe("DateEditor_WithFlatPicker", () => {

    it("uses flatpickr date if it is found", async () => {
        var old = (window as any).flatpickr;
        (window as any).flatpickr = vi.fn().mockImplementation(() => old);
        var editor = await newEditor({});
        expect(editor.domNode.getAttribute("type")).toBe("text");
        expect((window as any).flatpickr).toHaveBeenCalled();
    });

    commonTests();
});


describe("DateEditor_WithJQueryDatePicker", () => {

    it("uses jquery date if it is found", async () => {
        let $ = mockJQuery({
            datepicker: vi.fn().mockImplementation(function () { return this })
        });
        var editor = await newEditor({});
        expect(editor.domNode.getAttribute("type")).toBe("text");
        expect($.fn.datepicker).toHaveBeenCalled();
    })

    commonTests();
});