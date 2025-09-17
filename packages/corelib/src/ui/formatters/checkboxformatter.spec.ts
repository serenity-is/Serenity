import { CheckboxFormatter } from "./checkboxformatter";

describe("CheckboxFormatter", () => {
    it("shows checked class if value is true", () => {
        const formatter = new CheckboxFormatter();
        expect((formatter.format({ value: true, escape: (s) => s }) as Element)?.classList).toContain("checked");
    })

    it("removes checked class if value is false", () => {
        const formatter = new CheckboxFormatter();
        expect((formatter.format({ value: false, escape: (s) => s }) as Element)?.classList).not.toContain("checked");
    })

    it("shows no icon for false value if falseIcon is set to empty string", () => {
        const formatter = new CheckboxFormatter({
            falseIcon: ""
        });
        expect(formatter.format({ value: false, escape: (s) => s })).toBe("");
    });

    it("shows null icon/text if value is null", () => {
        const formatter = new CheckboxFormatter({ nullText: "N/A", nullIcon: "icon-null" });
        const result = formatter.format({ value: null, escape: (s) => s }) as HTMLElement;
        expect(result instanceof HTMLElement).toBe(true);
        expect(result.title).toBe("N/A");
        expect(result.classList).toContain("icon-null");
    });

    it("shows custom true/false text as hint by default", () => {
        const formatter = new CheckboxFormatter({ trueText: "Yes!", falseText: "No!" });
        const falseResult = formatter.format({ value: false, escape: (s) => s }) as HTMLElement;
        expect(falseResult?.title).toBe("No!");
        const trueResult = formatter.format({ value: true, escape: (s) => s }) as HTMLElement;
        expect(trueResult?.title).toBe("Yes!");
    });

    it("shows text value if called with a purpose of headerfilter", () => {
        const formatter = new CheckboxFormatter();
        expect(formatter.format({ value: true, escape: (s) => s, purpose: "headerfilter" })).toBe("Yes");
        expect(formatter.format({ value: false, escape: (s) => s, purpose: "headerfilter" })).toBe("No");
        expect(formatter.format({ value: null, escape: (s) => s, purpose: "headerfilter" })).toBe("");
    });
})

