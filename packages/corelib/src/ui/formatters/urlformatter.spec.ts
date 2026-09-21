import { describe, expect, it } from "vitest";
import { formatterContext as ctx } from "@serenity-is/sleekgrid";
import { UrlFormatter } from "./urlformatter";

describe("UrlFormatter", () => {
    it("shows empty string if value is null or empty", () => {
        const formatter = new UrlFormatter();
        expect(formatter.format(ctx({ value: null }))).toBe("");
        expect(formatter.format(ctx({ value: "" }))).toBe("");
    })

    it("shows empty string if urlProperty value is null or empty", () => {
        const formatter = new UrlFormatter();
        formatter.urlProperty = "url";
        expect(formatter.format(ctx({ value: null, item: { url: null } }))).toBe("");
        expect(formatter.format(ctx({ value: null, item: { url: "" } }))).toBe("");
    })

    it("shows link if url is specified", () => {
        const formatter = new UrlFormatter();
        expect((formatter.format(ctx({ value: "test" })) as HTMLElement).outerHTML).toContain("test");
    })

    it("formats url if format is specified", () => {
        const formatter = new UrlFormatter();
        formatter.urlFormat = "test/{0}";
        expect((formatter.format(ctx({ value: "test" })) as HTMLElement).outerHTML).toContain("test/test");
    })

    it("resolves url if it starts with tilda", () => {
        const formatter = new UrlFormatter();
        expect((formatter.format(ctx({ value: "~/test" })) as HTMLElement).outerHTML).toContain("/test");
    })

    it("uses display format property for showing if specified", () => {
        const formatter = new UrlFormatter();
        formatter.displayFormat = "displayFormat {0}"
        expect((formatter.format(ctx({ value: "~/test" })) as HTMLElement).outerHTML).toContain("displayFormat ~/test");
    })

    it("adds target if specified", () => {
        const formatter = new UrlFormatter();
        formatter.target = "_blank"
        expect((formatter.format(ctx({ value: "~/test" })) as HTMLElement).outerHTML).toContain("target=\"_blank\"");
    })

    it("adds diplayProperty and UrlProperty to referencedFields if specified", () => {
        const formatter = new UrlFormatter();
        formatter.displayProperty = "display";
        formatter.urlProperty = "url";
        const column = {};
        formatter.initializeColumn(column);
        expect(column).toEqual({ referencedFields: ["display", "url"] });
    });
})