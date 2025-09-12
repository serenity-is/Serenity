import { CheckboxFormatter } from "./checkboxformatter";

describe("CheckboxFormatter", () => {
    it("shows checked class if value is true", () => {
        var formatter = new CheckboxFormatter();
        expect(formatter.format({ value: true, escape: (s) => s })).toContain("checked");
    })

    it("removes checked class if value is false", () => {
        var formatter = new CheckboxFormatter();
        expect(formatter.format({ value: false, escape: (s) => s })).not.toContain("checked");
    })
})

