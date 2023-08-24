
jest.mock("@serenity-is/corelib/q", () => ({
    ...jest.requireActual("@serenity-is/corelib/q"),
    tryGetText: jest.fn().mockImplementation((key: string) => key)
}));


import { BooleanFormatter, CheckboxFormatter, DateFormatter } from "./formatters";
import { tryGetText } from "@serenity-is/corelib/q"

describe("BooleanFormatter", () => {
    it("shows empty string if value is null", () => {
        var formatter = new BooleanFormatter();
        expect(formatter.format({value: null, escape: (s) => s})).toBe("");
    });

    it("shows true text from localizer if value is true and true text is not null", () => {
        var formatter = new BooleanFormatter();
        formatter.trueText = "trueText";
        expect(formatter.format({value: true, escape: (s) => s})).toBe("trueText");
    });

    it("shows false text from localizer if value is false and false text is not null", () => {
        var formatter = new BooleanFormatter();
        formatter.falseText = "falseText";
        expect(formatter.format({value: false, escape: (s) => s})).toBe("falseText");
    });

    it("shows Dialogs.YesButton text from localizer if value is true and true text is null", () => {
        var formatter = new BooleanFormatter();
        expect(formatter.format({value: true, escape: (s) => s})).toBe("Dialogs.YesButton");
    });

    it("shows Dialogs.NoButton text from localizer if value is false and false text is null", () => {
        var formatter = new BooleanFormatter();
        expect(formatter.format({value: false, escape: (s) => s})).toBe("Dialogs.NoButton");
    });

    it("shows Yes text from localizer if value is true and Dialogs.YesButton is null", () => {
        var formatter = new BooleanFormatter();
        (tryGetText as any).mockReturnValueOnce(null);
        (tryGetText as any).mockReturnValueOnce(null);
        expect(formatter.format({value: true, escape: (s) => s})).toBe("Yes");
    });

    it("shows No text from localizer if value is false and Dialogs.NoButton is null", () => {
        var formatter = new BooleanFormatter();
        (tryGetText as any).mockReturnValueOnce(null);
        (tryGetText as any).mockReturnValueOnce(null);
        expect(formatter.format({value: false, escape: (s) => s})).toBe("No");
    });
})

describe("CheckboxFormatter", () => {
    it("shows checked class if value is true", () => {
        var formatter = new CheckboxFormatter();
        expect(formatter.format({value: true, escape: (s) => s})).toBe(`<span class="check-box no-float readonly slick-edit-preclick  checked"></span>`);
    })

    it("removes checked class if value is false", () => {
        var formatter = new CheckboxFormatter();
        expect(formatter.format({value: false, escape: (s) => s})).toBe(`<span class="check-box no-float readonly slick-edit-preclick "></span>`);
    })
})

describe("DateFormatter", () => {
    it("shows empty string if value is null", () => {
        var formatter = new DateFormatter();
        expect(formatter.format({value: null, escape: (s) => s})).toBe("");
    });


})