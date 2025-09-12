import { UrlFormatter } from "./urlformatter";

describe("UrlFormatter", () => {
    it("shows empty string if value is null or empty", () => {
        var formatter = new UrlFormatter();
        expect(formatter.format({ value: null, escape: (s) => s })).toBe("");
        expect(formatter.format({ value: "", escape: (s) => s })).toBe("");
    })

    it("shows empty string if urlProperty value is null or empty", () => {
        var formatter = new UrlFormatter();
        formatter.urlProperty = "url";
        expect(formatter.format({ value: null, item: { url: null }, escape: (s) => s })).toBe("");
        expect(formatter.format({ value: null, item: { url: "" }, escape: (s) => s })).toBe("");
    })

    it("shows link if url is specified", () => {
        var formatter = new UrlFormatter();
        expect(formatter.format({ value: "test", escape: (s) => s })).toContain("'test'");
    })

    it("formats url if format is specified", () => {
        var formatter = new UrlFormatter();
        formatter.urlFormat = "test/{0}";
        expect(formatter.format({ value: "test", escape: (s) => s })).toContain("'test/test'");
    })

    it("resolves url if it starts with tilda", () => {
        var formatter = new UrlFormatter();
        expect(formatter.format({ value: "~/test", escape: (s) => s })).toContain("'/test'");
    })

    it("uses display format property for showing if specified", () => {
        var formatter = new UrlFormatter();
        formatter.displayFormat = "displayFormat {0}"
        expect(formatter.format({ value: "~/test", escape: (s) => s })).toContain("displayFormat ~/test");
    })

    it("adds target if specified", () => {
        var formatter = new UrlFormatter();
        formatter.target = "_blank"
        expect(formatter.format({ value: "~/test", escape: (s) => s })).toContain("target='_blank'");
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