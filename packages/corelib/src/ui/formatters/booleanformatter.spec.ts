import { formatterContext as ctx } from "@serenity-is/sleekgrid";
import { BooleanFormatter } from "./booleanformatter";

describe("BooleanFormatter", () => {
    it("shows empty string if value is null", () => {
        var formatter = new BooleanFormatter();
        expect(formatter.format(ctx({ value: null }))).toBe("");
    });

    it("shows true text from localizer if value is true and true text is not null", () => {
        var formatter = new BooleanFormatter();
        formatter.trueText = "trueText";
        expect(formatter.format(ctx({ value: true }))).toBe("trueText");
    });

    it("shows false text from localizer if value is false and false text is not null", () => {
        var formatter = new BooleanFormatter();
        formatter.falseText = "falseText";
        expect(formatter.format(ctx({ value: false }))).toBe("falseText");
    });

    it("shows Dialogs.YesButton text from localizer if value is true and true text is null", () => {
        var formatter = new BooleanFormatter();
        expect(formatter.format(ctx({ value: true }))).toBe("Yes");
    });

    it("shows Dialogs.NoButton text from localizer if value is false and false text is null", () => {
        var formatter = new BooleanFormatter();
        expect(formatter.format(ctx({ value: false }))).toBe("No");
    });

    it("shows Yes text from localizer if value is true and Dialogs.YesButton is null", () => {
        var formatter = new BooleanFormatter();
        expect(formatter.format(ctx({ value: true }))).toBe("Yes");
    });

    it("shows No text from localizer if value is false and Dialogs.NoButton is null", () => {
        var formatter = new BooleanFormatter();
        expect(formatter.format(ctx({ value: false }))).toBe("No");
    });
    it("renders an icon without text", () => {
        const formatter = new BooleanFormatter({ showText: false, trueIcon: "fa-check" });
        const result = formatter.format(ctx({ value: true })) as HTMLElement;
        expect(result).toBeInstanceOf(HTMLElement);
        expect(result.className).toContain("fa-check");
    });

    it("renders text and icon with a hint", () => {
        const formatter = new BooleanFormatter({ showHint: true, trueIcon: "fa-check" });
        const result = formatter.format(ctx({ value: true })) as HTMLElement;
        const icon = result.querySelector("i");
        expect(icon).toBeTruthy();
        expect(icon!.className).toContain("fa-check");
        expect(result.title).toBe("Yes");
        expect(result.textContent).toContain("Yes");
    });

    it("renders a hint span when the icon is missing", () => {
        const formatter = new BooleanFormatter({ showHint: true });
        const result = formatter.format(ctx({ value: true })) as HTMLElement;
        expect(result.tagName).toBe("SPAN");
        expect(result.title).toBe("Yes");
        expect(result.textContent).toBe("Yes");
    });

    it("returns plain text when icon and hint are absent", () => {
        const formatter = new BooleanFormatter({ showHint: false });
        expect(formatter.format(ctx({ value: true }))).toBe("Yes");
    });

    it("exposes true and false text getters", () => {
        const formatter = new BooleanFormatter();
        formatter.trueText = "T";
        formatter.falseText = "F";
        expect(formatter.trueText).toBe("T");
        expect(formatter.falseText).toBe("F");
    });})
