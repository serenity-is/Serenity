import { BooleanFormatter } from "./booleanformatter";

describe("BooleanFormatter", () => {
    it("shows empty string if value is null", () => {
        var formatter = new BooleanFormatter();
        expect(formatter.format({ value: null, escape: (s) => s })).toBe("");
    });

    it("shows true text from localizer if value is true and true text is not null", () => {
        var formatter = new BooleanFormatter();
        formatter.trueText = "trueText";
        expect(formatter.format({ value: true, escape: (s) => s })).toBe("trueText");
    });

    it("shows false text from localizer if value is false and false text is not null", () => {
        var formatter = new BooleanFormatter();
        formatter.falseText = "falseText";
        expect(formatter.format({ value: false, escape: (s) => s })).toBe("falseText");
    });

    it("shows Dialogs.YesButton text from localizer if value is true and true text is null", () => {
        var formatter = new BooleanFormatter();
        expect(formatter.format({ value: true, escape: (s) => s })).toBe("Yes");
    });

    it("shows Dialogs.NoButton text from localizer if value is false and false text is null", () => {
        var formatter = new BooleanFormatter();
        expect(formatter.format({ value: false, escape: (s) => s })).toBe("No");
    });

    it("shows Yes text from localizer if value is true and Dialogs.YesButton is null", () => {
        var formatter = new BooleanFormatter();
        expect(formatter.format({ value: true, escape: (s) => s })).toBe("Yes");
    });

    it("shows No text from localizer if value is false and Dialogs.NoButton is null", () => {
        var formatter = new BooleanFormatter();
        expect(formatter.format({ value: false, escape: (s) => s })).toBe("No");
    });
})
