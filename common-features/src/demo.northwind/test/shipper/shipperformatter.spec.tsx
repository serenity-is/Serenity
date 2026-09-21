import { formatterContext } from "@serenity-is/sleekgrid";
import { describe, expect, it } from "vitest";
import { ShipperFormatter } from "../../Modules/Shipper/ShipperFormatter";

function render(result: any): string {
    const div = document.createElement("div");
    if (result != null)
        div.append(result);
    return div.textContent;
}

describe("ShipperFormatter", () => {
    it("renders nothing for empty values", () => {
        expect(render(new ShipperFormatter().format(formatterContext({ value: "" })))).toBe("");
    });

    it("renders the speedy express plane icon", () => {
        expect(render(new ShipperFormatter().format(formatterContext({ value: "Speedy Express" })))).toContain("Speedy Express");
    });

    it("renders the federal shipping ship icon", () => {
        expect(render(new ShipperFormatter().format(formatterContext({ value: "Federal Shipping" })))).toContain("Federal Shipping");
    });

    it("renders a default truck icon", () => {
        expect(render(new ShipperFormatter().format(formatterContext({ value: "United Package" })))).toContain("United Package");
    });
});
