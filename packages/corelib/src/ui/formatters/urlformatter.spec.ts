import { formatterContext as ctx } from "@serenity-is/sleekgrid";
import { UrlFormatter } from "./urlformatter";

describe("UrlFormatter", () => {
    it("shows empty string if value is null or empty", () => {
        var formatter = new UrlFormatter();
        expect(formatter.format(ctx({ value: null }))).toBe("");
        expect(formatter.format(ctx({ value: "" }))).toBe("");
    })

    it("shows empty string if urlProperty value is null or empty", () => {
        var formatter = new UrlFormatter();
        formatter.urlProperty = "url";
        expect(formatter.format(ctx({ value: null, item: { url: null } }))).toBe("");
        expect(formatter.format(ctx({ value: null, item: { url: "" } }))).toBe("");
    })

    it("shows link if url is specified", () => {
        var formatter = new UrlFormatter();
        expect(formatter.format(ctx({ value: "test" }))).toContain("'test'");
    })

    it("formats url if format is specified", () => {
        var formatter = new UrlFormatter();
        formatter.urlFormat = "test/{0}";
        expect(formatter.format(ctx({ value: "test" }))).toContain("'test/test'");
    })

    it("resolves url if it starts with tilda", () => {
        var formatter = new UrlFormatter();
        expect(formatter.format(ctx({ value: "~/test" }))).toContain("'/test'");
    })

    it("uses display format property for showing if specified", () => {
        var formatter = new UrlFormatter();
        formatter.displayFormat = "displayFormat {0}"
        expect(formatter.format(ctx({ value: "~/test" }))).toContain("displayFormat ~/test");
    })

    it("adds target if specified", () => {
        var formatter = new UrlFormatter();
        formatter.target = "_blank"
        expect(formatter.format(ctx({ value: "~/test" }))).toContain("target='_blank'");
    })

    it("adds diplayProperty and UrlProperty to referencedFields if specified", () => {
        var formatter = new UrlFormatter();
        formatter.displayProperty = "display";
        formatter.urlProperty = "url";
        var column = {};
        formatter.initializeColumn(column);
        expect(column).toEqual({ referencedFields: ["display", "url"] });
    });
})