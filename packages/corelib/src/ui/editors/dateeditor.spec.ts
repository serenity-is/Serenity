import validator from "@optionaldeps/jquery.validation"
import { DateEditor } from "./dateeditor";

[validator]

afterEach(() => {
    jest.useRealTimers();
    document.body.innerHTML = "";
});


function commonTests() {
    it("gets current value correctly", () => {
        var editor = new DateEditor($("<input type='text'/>"));
        editor.element.val("2020-01-01");


        expect(editor.value).toEqual("2020-01-01");
    })

    it("sets value to empty string if it is set to null", () => {
        var editor = new DateEditor($("<input type='text'/>"));
        editor.value = null;
        expect(editor.value).toEqual(null);
    })

    it("sets value to now if given value is now or today", () => {
        var editor = new DateEditor($("<input type='text'/>"));
        jest.useFakeTimers({now: new Date(2020, 0, 1)});
        editor.value = "now";
        expect(editor.value).toEqual("2020-01-01");
        editor.value = "today";
        expect(editor.value).toEqual("2020-01-01");
    })

    it("gets current value correctly", () => {
        var editor = new DateEditor($("<input type='text'/>"));
        editor.element.val("2020-01-01");

        expect(editor.value).toEqual("2020-01-01");
    })

    it("sets value when valueAsDate used", () => {
        var editor = new DateEditor($("<input type='text'/>"));
        editor.valueAsDate = new Date(2020, 0, 1);
        expect(editor.get_value()).toEqual("2020-01-01");
    })

    it("sets value when valueAsDate used", () => {
        var editor = new DateEditor($("<input type='text'/>"));
        editor.element.val("2020-01-01");
        expect(editor.valueAsDate).toEqual(new Date(2020, 0, 1));
    })
}

describe("DateEditor_WithDefaultHtmlInput", () => {
    beforeEach(() => {
        ($.fn as any).datepicker = undefined;
    })
    it("uses default date input if flatpicker or jquery date picker is not found", () => {
        var editor = new DateEditor($("<input type='text'/>"));
        expect(editor.element.attr("type")).toBe("date");
    })

    commonTests();
})

describe("DateEditor_WithFlatPicker", () => {
    beforeEach(() => {
        // @ts-ignore
        import("../../../../../node_modules/flatpickr/dist/flatpickr.js");
    })

    it("uses flatpickr date if it is found", () => {
        var old = (window as any).flatpickr;
        (window as any).flatpickr = jest.fn().mockImplementation(() => old);
        var editor = new DateEditor($("<input type='text'/>"));
        expect(editor.element.attr("type")).toBe("text");
        expect((window as any).flatpickr).toBeCalled();
    })

    commonTests();
});



describe("DateEditor_WithJQueryDatePicker", () => {
    beforeEach(() => {
        // @ts-ignore
        import("../../../../../src/Serenity.Assets/wwwroot/Scripts/jquery-ui.js");
    })

    it("uses jquery date if it is found", () => {
        var old = (window as any).$.fn.datepicker;
        (window as any).$.fn.datepicker = jest.fn().mockImplementation(() => old);
        var editor = new DateEditor($("<input type='text'/>"));
        expect(editor.element.attr("type")).toBe("text");
        expect((window as any).$.fn.datepicker).toBeCalled();
    })

   commonTests();
});