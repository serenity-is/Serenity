import { Fluent, Validator } from "../../base";
import { mockJQuery, unmockBSAndJQuery } from "../../test/mocks";
import { DateEditor } from "./dateeditor";
import { DateTimeEditor } from "./datetimeeditor";

beforeEach(() => {
    vi.useRealTimers();
    document.body.innerHTML = "";
    delete (window as any)["$"];
    delete (window as any)["jQuery"];
    delete (window as any)["flatpickr"];
    delete (window as any)["bootstrap"];
    DateEditor.useFlatpickr = false;
});

afterEach(() => {
    unmockBSAndJQuery();
    DateEditor.useFlatpickr = false;
});

const newEditor = (opt: any = {}) => new DateTimeEditor(opt as any);

describe("DateTimeEditor_WithDefaultInput", () => {
    it("uses a datetime input when no picker is found", () => {
        const editor = newEditor({});
        expect(editor.domNode.getAttribute("type")).toBe("datetime");
        expect(editor.domNode.classList.contains("dateTimeQ")).toBe(true);
        expect(editor.domNode.nextElementSibling?.classList.contains("inplace-now")).toBe(true);
        editor.destroy();
    });

    it("builds an input-only editor", () => {
        const editor = newEditor({ inputOnly: true });
        expect(editor.domNode.classList.contains("dateTimeQ")).toBe(true);
        expect(editor.domNode.getAttribute("type")).toBe("text");
        editor.destroy();
    });

    it("reads and writes values without a time select", () => {
        const editor = newEditor({});
        editor.set_value("2020-01-01T10:30:00");
        expect(editor.get_value()).toContain("2020-01-01T10:30:00");
        editor.value = "2020-02-02T09:00:00";
        expect(editor.get_value()).toContain("2020-02-02T09:00:00");
        editor.set_value("");
        expect(editor.get_value()).toBeNull();
        editor.set_value(null);
        expect(editor.get_value()).toBeNull();
        editor.destroy();
    });

    it("sets the value to today without a time select", () => {
        const editor = newEditor({});
        expect(() => editor.set_value("today")).not.toThrow();
        expect(editor.get_value()).toBeTruthy();
        editor.destroy();
    });

    it("formats values in UTC when useUtc is set", () => {
        const editor = newEditor({ useUtc: true });
        editor.set_value("2020-01-01T10:30:00");
        expect(editor.get_value()).toBeTruthy();
        editor.destroy();
    });

    it("sets and gets valueAsDate", () => {
        const editor = newEditor({});
        editor.valueAsDate = new Date(2020, 0, 1, 10, 30);
        expect(editor.valueAsDate).toEqual(new Date(2020, 0, 1, 10, 30));
        editor.valueAsDate = null;
        expect(editor.value).toBeNull();
        expect(editor.valueAsDate).toBeNull();
        editor.destroy();
    });

    it("sets and gets min and max dates", () => {
        const editor = newEditor({});
        editor.set_minDate(new Date(2020, 0, 1, 10, 30));
        expect(editor.get_minDate()).toEqual(new Date(2020, 0, 1, 10, 30));
        editor.set_maxDate(new Date(2020, 11, 31, 23, 59));
        expect(editor.get_maxDate()).toEqual(new Date(2020, 11, 31, 23, 59));
        editor.destroy();
    });

    it("sets and clears SQL min/max bounds", () => {
        const editor = newEditor({});
        editor.set_sqlMinMax(true);
        expect(editor.get_sqlMinMax()).toBe(true);
        editor.set_sqlMinMax(false);
        expect(editor.get_minValue()).toBeNull();
        expect(editor.get_maxValue()).toBeNull();
        expect(editor.get_sqlMinMax()).toBe(false);
        editor.destroy();
    });

    it("respects minValue/maxValue options instead of SQL defaults", () => {
        const editor = newEditor({ minValue: "2020-01-01T00:00:00", maxValue: "2020-12-31T23:59:00" });
        expect(editor.get_minValue()).toBe("2020-01-01T00:00:00");
        expect(editor.get_maxValue()).toBe("2020-12-31T23:59:00");
        expect(editor.get_sqlMinMax()).toBe(false);
        editor.destroy();
    });

    it("keeps SQL default for unspecified bound when only one is provided", () => {
        const editor = newEditor({ minValue: "2020-01-01T00:00:00" });
        expect(editor.get_minValue()).toBe("2020-01-01T00:00:00");
        expect(editor.get_maxValue()).toBe("9999-12-31");
        editor.destroy();

        const editor2 = newEditor({ maxValue: "2020-12-31T23:59:00" });
        expect(editor2.get_minValue()).toBe("1753-01-01");
        expect(editor2.get_maxValue()).toBe("2020-12-31T23:59:00");
        editor2.destroy();
    });

    it("does not apply SQL min/max when sqlMinMax is false", () => {
        const editor = newEditor({ sqlMinMax: false });
        expect(editor.get_minValue()).toBeUndefined();
        expect(editor.get_maxValue()).toBeUndefined();
        expect(editor.get_sqlMinMax()).toBe(false);
        editor.destroy();
    });

    it("toggles readonly state", () => {
        const editor = newEditor({});
        expect(editor.get_readOnly()).toBe(false);
        editor.set_readOnly(true);
        expect(editor.get_readOnly()).toBe(true);
        editor.set_readOnly(false);
        expect(editor.get_readOnly()).toBe(false);
        editor.destroy();
    });

    it("setToNow updates the value and optionally triggers change", () => {
        const editor = newEditor({});
        const changeSpy = vi.spyOn(Fluent, "trigger");
        editor.setToNow(true);
        expect(editor.get_value()).toBeTruthy();
        expect(changeSpy).toHaveBeenCalledWith(editor.domNode, "change");
        editor.destroy();
    });

    it("handles space key and ignores readonly keyboard input", () => {
        const editor = newEditor({});
        const nowSpy = vi.spyOn(editor as any, "setToNow").mockImplementation(() => { });
        editor.domNode.dispatchEvent(new KeyboardEvent("keydown", { key: "a" }));
        editor.domNode.dispatchEvent(new KeyboardEvent("keyup", { key: "1" }));
        editor.domNode.dispatchEvent(new KeyboardEvent("keydown", { key: " ", cancelable: true }));
        expect(nowSpy).toHaveBeenCalledWith(true);

        editor.set_readOnly(true);
        nowSpy.mockClear();
        editor.domNode.dispatchEvent(new KeyboardEvent("keydown", { key: " " }));
        expect(nowSpy).not.toHaveBeenCalled();
        editor.domNode.dispatchEvent(new KeyboardEvent("keyup", { key: "1" }));
        editor.destroy();
    });

    it("sets to now from the inplace button and respects readonly", () => {
        const editor = newEditor({});
        const nowSpy = vi.spyOn(editor as any, "setToNow");
        const btn = editor.domNode.nextElementSibling as HTMLElement;
        btn.click();
        expect(nowSpy).toHaveBeenCalledWith(true);
        editor.set_readOnly(true);
        nowSpy.mockClear();
        btn.click();
        expect(nowSpy).not.toHaveBeenCalled();
        editor.destroy();
    });

    it("rounds to minutes and builds time options", () => {
        expect(DateTimeEditor.roundToMinutes(new Date(2020, 0, 1, 10, 37, 45), 5).getMinutes()).toBe(35);
        expect(DateTimeEditor.roundToMinutes(new Date(2020, 0, 1, 10, 37, 45), 5).getSeconds()).toBe(0);
        const opts = DateTimeEditor.getTimeOptions(0, 0, 23, 59, 5);
        expect(opts).toContain("00:00");
        expect(opts).toContain("23:55");
        expect(opts.length).toBe(288);
        expect(DateTimeEditor.getTimeOptions(23, 55, 23, 59, 10)).toContain("23:55");
        expect(DateTimeEditor.getTimeOptions(0, 0, 23, 60, 5)).toContain("23:55");
    });

    it("returns early for a missing dom node trigger", () => {
        const editor: any = Object.create(DateTimeEditor.prototype);
        expect(editor.createFlatPickrTrigger()).toBeUndefined();
    });
});

describe("DateTimeEditor_WithFlatPicker", () => {
    it("uses flatpickr when available", () => {
        const destroy = vi.fn();
        (window as any).flatpickr = vi.fn().mockImplementation((el: any) => {
            (el as any)._flatpickr = { destroy, calendarContainer: document.createElement("div") };
        });
        DateEditor.useFlatpickr = true;
        const editor = newEditor({ seconds: true, intervalMinutes: 15 });
        expect(editor.domNode.classList.contains("dateTimeQ")).toBe(true);
        expect((window as any).flatpickr).toHaveBeenCalled();
        const options = editor.getFlatpickrOptions();
        expect(options.enableSeconds).toBe(true);
        expect(options.minuteIncrement).toBe(15);
        expect(options.disable[0](new Date())).toBe(false);
        expect(() => options.onChange()).not.toThrow();
        editor.set_readOnly(true);
        editor.domNode.value = "2000-01-01";
        expect(options.disable[0](new Date(2020, 0, 1))).toBe(true);
        editor.destroy();
        expect(destroy).toHaveBeenCalled();
        delete (window as any).flatpickr;
    });

    it("appends flatpickr options to a modal", () => {
        const modal = document.createElement("div");
        modal.className = "modal";
        const input = document.createElement("input");
        modal.appendChild(input);
        document.body.appendChild(modal);
        const editor = newEditor({ element: input, inputOnly: true });
        const options = editor.getFlatpickrOptions();
        expect(options.appendTo).toBe(modal);
        editor.destroy();
        modal.remove();
    });

    it("moves the flatpickr calendar into a modal once visible", async () => {
        const destroy = vi.fn();
        const calendarContainer = document.createElement("div");
        (window as any).flatpickr = vi.fn().mockImplementation((el: any) => {
            (el as any)._flatpickr = { destroy, calendarContainer };
        });
        DateEditor.useFlatpickr = true;
        const editor = newEditor({});
        editor.getFlatpickrOptions();
        const modal = document.createElement("div");
        modal.className = "modal";
        modal.appendChild(editor.domNode);
        document.body.appendChild(modal);
        await new Promise(r => setTimeout(r, 10));
        expect(calendarContainer.parentElement).toBe(modal);
        editor.destroy();
        modal.remove();
        delete (window as any).flatpickr;
    });
});

describe("DateTimeEditor_WithJQueryDatePicker", () => {
    it("uses jquery datepicker and builds the time select", () => {
        const $ = mockJQuery({ datepicker: vi.fn().mockImplementation(function () { return this; }) });
        const editor = newEditor({ startHour: 0, endHour: 23, intervalMinutes: 5, yearRange: "-10:+10" });
        expect($.fn.datepicker).toHaveBeenCalled();
        const opts = ($.fn.datepicker as any).mock.calls[0][0];
        expect(opts.yearRange).toBe("-10:+10");
        expect(opts.beforeShow()).toBe(true);
        expect(editor.domNode.classList.contains("dateQ")).toBe(true);

        const time = editor["time"] as HTMLSelectElement;
        expect(time).toBeTruthy();
        expect(time.options.length).toBeGreaterThan(0);
        expect(time.value).toBe("00:00");
        editor.set_value("2020-01-01T10:37:00");
        expect(time.value).toBe("10:35");
        expect(editor.get_value()).toBe("2020-01-01T10:37:00");
        editor.set_value("today");
        expect(time.value).toBe("00:00");
        editor.set_readOnly(true);
        expect(opts.beforeShow()).toBe(false);
        editor.destroy();
    });

    it("places the time select relative to a datepicker trigger", () => {
        mockJQuery({ datepicker: vi.fn().mockImplementation(function () { return this; }) });
        const input = document.createElement("input");
        const trigger = document.createElement("button");
        trigger.className = "ui-datepicker-trigger";
        document.body.appendChild(input);
        input.after(trigger);
        const editor = newEditor({ element: input });
        expect(editor["time"].previousElementSibling).toBe(trigger);
        editor.destroy();

        const input2 = document.createElement("input");
        const trigger2 = document.createElement("button");
        trigger2.className = "ui-datepicker-trigger";
        document.body.appendChild(input2);
        input2.before(trigger2);
        const editor2 = newEditor({ element: input2 });
        expect(editor2["time"].nextElementSibling?.nextElementSibling).toBe(trigger2);
        editor2.destroy();
    });

    it("clears last value and forwards change events", () => {
        mockJQuery({ datepicker: vi.fn().mockImplementation(function () { return this; }) });
        const editor = newEditor({});
        const spy = vi.spyOn(DateEditor, "dateInputChange");
        editor.domNode.dispatchEvent(new Event("change." + (editor as any).uniqueName));
        expect(spy).toHaveBeenCalled();
        editor.destroy();
    });

    it("validates min and max values through the time select", () => {
        mockJQuery({ datepicker: vi.fn().mockImplementation(function () { return this; }) });
        const form = document.createElement("form");
        const editor = newEditor({});
        editor.set_minValue("2020-01-01T00:00:00");
        editor.set_maxValue("2020-12-31T23:59:00");
        form.appendChild(editor.domNode);
        document.body.appendChild(form);
        const validator = new Validator(form, {});
        editor.set_value("2020-06-01T10:00:00");
        expect(validator.element(editor.domNode)).toBe(true);
        editor.set_value("2019-12-31T10:00:00");
        expect(validator.element(editor.domNode)).toBe(false);
        editor.set_value("2021-01-01T10:00:00");
        expect(validator.element(editor.domNode)).toBe(false);
        editor.domNode.value = "";
        expect(validator.element(editor.domNode)).toBe(true);
        editor.destroy();
        form.remove();
    });

    it("triggers a change event when the time select changes", () => {
        mockJQuery({ datepicker: vi.fn().mockImplementation(function () { return this; }) });
        const editor = newEditor({});
        const changeSpy = vi.spyOn(Fluent, "trigger");
        const time = editor["time"] as HTMLSelectElement;
        time.dispatchEvent(new Event("change"));
        expect(changeSpy).toHaveBeenCalledWith(editor.domNode, "change");
        editor.destroy();
    });
});
