import { type DateEditorOptions } from "./dateeditor";
import { mockJQuery } from "../../mocks";

beforeEach(() => {
    jest.useRealTimers();
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
        jest.useFakeTimers({ now: new Date(2020, 0, 1) });
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


describe("DateEditor_WithFlatPicker", () => {

    it("uses flatpickr date if it is found", async () => {
        var old = (window as any).flatpickr;
        (window as any).flatpickr = jest.fn().mockImplementation(() => old);
        var editor = await newEditor({});
        expect(editor.domNode.getAttribute("type")).toBe("text");
        expect((window as any).flatpickr).toHaveBeenCalled();
    });

    commonTests();
});


describe("DateEditor_WithJQueryDatePicker", () => {

    it("uses jquery date if it is found", async () => {
        let $ = mockJQuery({
            datepicker: jest.fn().mockImplementation(function() { return this })
        });
        var editor = await newEditor({});
        expect(editor.domNode.getAttribute("type")).toBe("text");
        expect($.fn.datepicker).toHaveBeenCalled();
    })

    commonTests();
});