import { formatterContext } from "@serenity-is/sleekgrid";
import { FreightFormatter } from "../../Modules/Order/FreightFormatter";

function render(result: any): string {
    const div = document.createElement("div");
    if (result != null)
        div.append(result);
    return div.textContent;
}

describe("FreightFormatter", () => {
    it("returns empty string for null values", () => {
        expect(new FreightFormatter().format(formatterContext({ value: null }))).toBe("");
    });

    it("renders the freight value with an icon", () => {
        expect(render(new FreightFormatter().format(formatterContext({ value: 12.5 })))).toContain("12.5");
    });
});
