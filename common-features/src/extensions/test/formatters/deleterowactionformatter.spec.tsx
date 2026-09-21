import { formatterContext } from "@serenity-is/sleekgrid";
import { describe, expect, it } from "vitest";
import { DeleteRowActionFormatter } from "../../Modules/Formatters/DeleteRowActionFormatter";

function makeCtx(item: any, idProperty = "id") {
    return formatterContext({
        item,
        grid: {
            getData: () => ({
                getIdPropertyName: () => idProperty
            })
        } as any
    });
}

describe("DeleteRowActionFormatter", () => {
    it("returns empty string when there is no item", () => {
        const formatter = new DeleteRowActionFormatter();
        expect(formatter.format(makeCtx(null))).toBe("");
    });

    it("returns empty string for non-data rows", () => {
        const formatter = new DeleteRowActionFormatter();
        expect(formatter.format(makeCtx({ __nonDataRow: true, id: 1 }))).toBe("");
    });

    it("returns empty string when id is null", () => {
        const formatter = new DeleteRowActionFormatter();
        expect(formatter.format(makeCtx({ id: null }))).toBe("");
    });

    it("returns a delete action link for a valid row", () => {
        const formatter = new DeleteRowActionFormatter();
        const link = formatter.format(makeCtx({ id: 5 })) as HTMLElement;
        expect(link.tagName).toBe("A");
        expect(link.getAttribute("data-action")).toBe("delete-row");
        expect(link.querySelector("i")).toBeTruthy();
    });
});
