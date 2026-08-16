import { formatterContext as ctx } from "../../src/core/formatting";
import {
    CheckBoxFormatter,
    CheckmarkFormatter,
    PercentCompleteBarFormatter,
    PercentCompleteFormatter,
    YesNoFormatter
} from "../../src/formatters/formatters";
import { Formatters } from "../../src/formatters";

describe('PercentCompleteFormatter', () => {
    it('returns "-" for null or empty value', () => {
        expect(PercentCompleteFormatter(ctx({ value: null }))).toBe("-");
        expect(PercentCompleteFormatter(ctx({ value: "" }))).toBe("-");
    });

    it('creates a bold span with the value followed by a percent sign', () => {
        const result = PercentCompleteFormatter(ctx({ value: 75 })) as HTMLElement;
        expect(result.tagName).toBe("SPAN");
        expect(result.textContent).toBe("75%");
        expect(result.style.fontWeight).toBe("bold");
    });

    it('colors the text red when value is less than 50', () => {
        const result = PercentCompleteFormatter(ctx({ value: 49 })) as HTMLElement;
        expect(result.style.color).toBe("red");
    });

    it('colors the text green when value is 50 or more', () => {
        expect((PercentCompleteFormatter(ctx({ value: 50 })) as HTMLElement).style.color).toBe("green");
        expect((PercentCompleteFormatter(ctx({ value: 99 })) as HTMLElement).style.color).toBe("green");
    });
});

describe('PercentCompleteBarFormatter', () => {
    it('returns an empty string for null or empty value', () => {
        expect(PercentCompleteBarFormatter(ctx({ value: null }))).toBe("");
        expect(PercentCompleteBarFormatter(ctx({ value: "" }))).toBe("");
    });

    it('uses red background for values under 30', () => {
        const result = PercentCompleteBarFormatter(ctx({ value: 29 })) as HTMLElement;
        expect(result.style.background).toBe("red");
    });

    it('uses silver background for values between 30 and 69', () => {
        const result = PercentCompleteBarFormatter(ctx({ value: 30 })) as HTMLElement;
        expect(result.style.background).toBe("silver");
        expect((PercentCompleteBarFormatter(ctx({ value: 69 })) as HTMLElement).style.background).toBe("silver");
    });

    it('uses green background for values 70 or more', () => {
        const result = PercentCompleteBarFormatter(ctx({ value: 70 })) as HTMLElement;
        expect(result.style.background).toBe("green");
    });

    it('sets the class, width and title attributes on the bar', () => {
        const result = PercentCompleteBarFormatter(ctx({ value: 42 })) as HTMLElement;
        expect(result.className).toBe("percent-complete-bar slick-percentcomplete-bar");
        expect(result.style.width).toBe("42%");
        expect(result.title).toBe("42%");
    });
});

describe('YesNoFormatter', () => {
    it('returns "Yes" for truthy values', () => {
        expect(YesNoFormatter(ctx({ value: true }))).toBe("Yes");
        expect(YesNoFormatter(ctx({ value: 1 }))).toBe("Yes");
        expect(YesNoFormatter(ctx({ value: "x" }))).toBe("Yes");
    });

    it('returns "No" for falsy values', () => {
        expect(YesNoFormatter(ctx({ value: false }))).toBe("No");
        expect(YesNoFormatter(ctx({ value: 0 }))).toBe("No");
        expect(YesNoFormatter(ctx({ value: null }))).toBe("No");
        expect(YesNoFormatter(ctx({ value: undefined }))).toBe("No");
        expect(YesNoFormatter(ctx({ value: "" }))).toBe("No");
    });
});

describe('CheckBoxFormatter', () => {
    it('creates an <i> element with checkbox classes when unchecked', () => {
        const result = CheckBoxFormatter(ctx({ value: false })) as HTMLElement;
        expect(result.tagName).toBe("I");
        expect(result.className).toBe("slick-checkbox slick-edit-preclick");
    });

    it('adds the checked class when value is truthy', () => {
        const result = CheckBoxFormatter(ctx({ value: true })) as HTMLElement;
        expect(result.tagName).toBe("I");
        expect(result.className).toBe("slick-checkbox slick-edit-preclick checked");
    });
});

describe('CheckmarkFormatter', () => {
    it('returns an empty string when value is falsy', () => {
        expect(CheckmarkFormatter(ctx({ value: false }))).toBe("");
        expect(CheckmarkFormatter(ctx({ value: null }))).toBe("");
        expect(CheckmarkFormatter(ctx({ value: 0 }))).toBe("");
        expect(CheckmarkFormatter(ctx({ value: "" }))).toBe("");
    });

    it('creates an <i> element with the checkmark class when value is truthy', () => {
        const result = CheckmarkFormatter(ctx({ value: true })) as HTMLElement;
        expect(result.tagName).toBe("I");
        expect(result.className).toBe("slick-checkmark");
    });
});

describe('Formatters namespace wrappers', () => {
    it('PercentComplete wraps PercentCompleteFormatter', () => {
        expect(Formatters.PercentComplete(0, 0, null)).toBe("-");
        const result = Formatters.PercentComplete(0, 0, 80) as HTMLElement;
        expect(result.textContent).toBe("80%");
        expect(result.style.color).toBe("green");
    });

    it('PercentCompleteBar wraps PercentCompleteBarFormatter', () => {
        expect(Formatters.PercentCompleteBar(0, 0, "")).toBe("");
        const result = Formatters.PercentCompleteBar(0, 0, 55) as HTMLElement;
        expect(result.className).toBe("percent-complete-bar slick-percentcomplete-bar");
        expect(result.style.background).toBe("silver");
    });

    it('YesNo wraps YesNoFormatter', () => {
        expect(Formatters.YesNo(0, 0, true)).toBe("Yes");
        expect(Formatters.YesNo(0, 0, false)).toBe("No");
    });

    it('Checkbox wraps CheckBoxFormatter', () => {
        expect((Formatters.Checkbox(0, 0, true) as HTMLElement).className).toBe("slick-checkbox slick-edit-preclick checked");
        expect((Formatters.Checkbox(0, 0, false) as HTMLElement).className).toBe("slick-checkbox slick-edit-preclick");
    });

    it('Checkmark wraps CheckmarkFormatter', () => {
        expect(Formatters.Checkmark(0, 0, false)).toBe("");
        expect((Formatters.Checkmark(0, 0, true) as HTMLElement).className).toBe("slick-checkmark");
    });
});
