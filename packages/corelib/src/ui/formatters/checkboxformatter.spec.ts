import { formatterContext as ctx } from "@serenity-is/sleekgrid";
import { CheckboxFormatter } from "./checkboxformatter";

describe("CheckboxFormatter", () => {
    it("shows checked class if value is true", () => {
        const formatter = new CheckboxFormatter();
        expect((formatter.format(ctx({ value: true })) as Element)?.classList).toContain("checked");
    })

    it("removes checked class if value is false", () => {
        const formatter = new CheckboxFormatter();
        expect((formatter.format(ctx({ value: false })) as Element)?.classList).not.toContain("checked");
    })

    it("shows no icon for false value if falseIcon is set to empty string", () => {
        const formatter = new CheckboxFormatter({
            falseIcon: ""
        });
        expect(formatter.format(ctx({ value: false }))).toBe("");
    });

    it("shows null icon/text if value is null", () => {
        const formatter = new CheckboxFormatter({ nullText: "N/A", nullIcon: "icon-null" });
        const result = formatter.format(ctx({ value: null })) as HTMLElement;
        expect(result instanceof HTMLElement).toBe(true);
        expect(result.title).toBe("N/A");
        expect(result.classList).toContain("icon-null");
    });

    it("shows custom true/false text as hint by default", () => {
        const formatter = new CheckboxFormatter({ trueText: "Yes!", falseText: "No!" });
        const falseResult = formatter.format(ctx({ value: false })) as HTMLElement;
        expect(falseResult?.title).toBe("No!");
        const trueResult = formatter.format(ctx({ value: true })) as HTMLElement;
        expect(trueResult?.title).toBe("Yes!");
    });

    it("shows text value if called with a purpose of headerfilter", () => {
        const formatter = new CheckboxFormatter();
        expect(formatter.format(ctx({ value: true, purpose: "header-filter" }))).toBe("Yes");
        expect(formatter.format(ctx({ value: false, purpose: "header-filter" }))).toBe("No");
        expect(formatter.format(ctx({ value: null, purpose: "header-filter" }))).toBe("");
    });

    it("shows text alongside icon when showText is true for true value", () => {
        const formatter = new CheckboxFormatter({ showText: true });
        const result = formatter.format(ctx({ value: true })) as HTMLElement;
        expect(result.tagName.toLowerCase()).toBe("span");
        expect(result.textContent?.trim()).toContain("Yes");
        expect(result.querySelector(".check-box.checked")).toBeTruthy();
    });

    it("shows text alongside icon when showText is true for false value", () => {
        const formatter = new CheckboxFormatter({ showText: true });
        const result = formatter.format(ctx({ value: false })) as HTMLElement;
        expect(result.tagName.toLowerCase()).toBe("span");
        expect(result.textContent?.trim()).toContain("No");
        expect(result.querySelector(".check-box:not(.checked)")).toBeTruthy();
    });

    it("shows text alongside icon when showText is true for null value", () => {
        const formatter = new CheckboxFormatter({ showText: true, nullText: "N/A", nullIcon: "fa fa-question" });
        const result = formatter.format(ctx({ value: null })) as HTMLElement;
        expect(result.tagName.toLowerCase()).toBe("span");
        expect(result.textContent?.trim()).toContain("N/A");
        expect(result.querySelector(".fa.fa-question")).toBeTruthy();
    });

    it("shows hint by default when custom text is provided and showText is false", () => {
        const formatter = new CheckboxFormatter({ trueText: "Active", falseText: "Inactive" });
        const result = formatter.format(ctx({ value: true })) as HTMLElement;
        expect(result.title).toBe("Active");
    });

    it("does not show hint when showHint is false", () => {
        const formatter = new CheckboxFormatter({ trueText: "Active", falseText: "Inactive", showHint: false });
        const result = formatter.format(ctx({ value: true })) as HTMLElement;
        expect(result.title).toBe("");
    });

    it("shows hint when showHint is explicitly true", () => {
        const formatter = new CheckboxFormatter({ trueText: "Active", showHint: true });
        const result = formatter.format(ctx({ value: true })) as HTMLElement;
        expect(result.title).toBe("Active");
    });

    it("shows custom true icon when trueIcon is specified", () => {
        const formatter = new CheckboxFormatter({ trueIcon: "fa fa-check-circle" });
        const result = formatter.format(ctx({ value: true })) as HTMLElement;
        expect(result.matches(".fa.fa-check-circle")).toBeTruthy();
        expect(result.classList).toContain("slick-edit-preclick");
    });

    it("shows custom false icon when falseIcon is specified", () => {
        const formatter = new CheckboxFormatter({ falseIcon: "fa fa-times-circle" });
        const result = formatter.format(ctx({ value: false })) as HTMLElement;
        expect(result.matches(".fa.fa-times-circle")).toBeTruthy();
    });

    it("shows custom null icon when nullIcon is specified", () => {
        const formatter = new CheckboxFormatter({ nullIcon: "fa fa-question-circle" });
        const result = formatter.format(ctx({ value: null })) as HTMLElement;
        expect(result.matches(".fa.fa-question-circle")).toBeTruthy();
    });

    it("shows no icon when trueIcon is empty string", () => {
        const formatter = new CheckboxFormatter({ trueIcon: "" });
        const result = formatter.format(ctx({ value: true })) as HTMLElement;
        expect(result).toBe("");
    });

    it("shows custom icon and text when showText is true with custom trueIcon", () => {
        const formatter = new CheckboxFormatter({
            showText: true,
            trueIcon: "fa fa-check",
            trueText: "Enabled"
        });
        const result = formatter.format(ctx({ value: true })) as HTMLElement;
        expect(result.tagName.toLowerCase()).toBe("span");
        expect(result.querySelector(".fa.fa-check")).toBeTruthy();
        expect(result.textContent?.trim()).toContain("Enabled");
    });

    it("shows text only when showText is true and no icon is specified", () => {
        const formatter = new CheckboxFormatter({
            showText: true,
            trueText: "Active",
            showHint: false
        });
        const result = formatter.format(ctx({ value: true })) as HTMLElement;
        expect(result.tagName.toLowerCase()).toBe("span");
        expect(result.textContent?.trim()).toBe("Active");
        expect(result.title).toBe("");
    });

    it("shows icon with hint when showText is false and showHint is true", () => {
        const formatter = new CheckboxFormatter({
            showText: false,
            showHint: true,
            trueText: "Active",
            trueIcon: "fa fa-check"
        });
        const result = formatter.format(ctx({ value: true })) as HTMLElement;
        expect(result.matches(".fa.fa-check")).toBeTruthy();
        expect(result.title).toBe("Active");
        expect(result.textContent?.trim()).toBe("");
    });

    it("handles undefined value as null", () => {
        const formatter = new CheckboxFormatter({ nullText: "Undefined", nullIcon: "fa fa-question" });
        const result = formatter.format(ctx({ value: undefined })) as HTMLElement;
        expect(result.title).toBe("Undefined");
    });

    it("handles non-boolean truthy values as true", () => {
        const formatter = new CheckboxFormatter();
        const result = formatter.format(ctx({ value: 1 })) as HTMLElement;
        expect(result.classList).toContain("checked");
    });

    it("handles non-boolean falsy values as false", () => {
        const formatter = new CheckboxFormatter();
        const result = formatter.format(ctx({ value: 0 })) as HTMLElement;
        expect(result.classList).not.toContain("checked");
    });

    it("returns empty string when no icon and no text for null value", () => {
        const formatter = new CheckboxFormatter({ showText: false, showHint: false });
        const result = formatter.format(ctx({ value: null }));
        expect(result).toBe("");
    });

    it("uses default Yes/No text when no custom text provided and showText is true", () => {
        const formatter = new CheckboxFormatter({ showText: true });
        const trueResult = formatter.format(ctx({ value: true })) as HTMLElement;
        expect(trueResult.textContent?.trim()).toContain("Yes");
        const falseResult = formatter.format(ctx({ value: false })) as HTMLElement;
        expect(falseResult.textContent?.trim()).toContain("No");
    });
})