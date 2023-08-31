import { DateEditor } from "./dateeditor";


afterEach(() => {
    jest.restoreAllMocks();
    jest.useRealTimers();
    document.body.innerHTML = "";
});

describe("DateEditor", () => {
    it("uses default date input if flatpicker or jquery date picker is not found", () => {
        var editor = new DateEditor($("<input type='text'/>"));
        expect(editor.element.attr("type")).toBe("date");
    })

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
        expect(editor.value).toEqual("2020-01-01");
    })

    it("sets value when valueAsDate used", () => {
        var editor = new DateEditor($("<input type='text'/>"));
        editor.element.val("2020-01-01");
        expect(editor.valueAsDate).toEqual(new Date(2020, 0, 1));
    })
})