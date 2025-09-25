import { formatterContext as ctx } from "@serenity-is/sleekgrid";
import { FileDownloadFormatter } from "./filedownloadformatter";

describe("FileDownloadFormatter", () => {
    it("shows empty string if value is null", () => {
        var formatter = new FileDownloadFormatter();
        expect(formatter.format(ctx({ value: null }))).toBe("");
    });


    it("replaces all backward slashes to forward", () => {
        var formatter = new FileDownloadFormatter();
        expect(formatter.format(ctx({ value: "file\\with\\backward\\slashes" }))).toContain("file/with/backward/slashes");
    });

    it("shows empty string if fileOriginalName is not specified", () => {
        var formatter = new FileDownloadFormatter();
        expect(formatter.format(ctx({ value: "file" })).replace(/\s/g, "")).toContain("</i></a>");
    });

    it("shows empty string if fileOriginalName is specified but not found", () => {
        var formatter = new FileDownloadFormatter();
        formatter.originalNameProperty = "fileOriginalName";
        expect(formatter.format(ctx({ value: "file", item: {} })).replace(/\s/g, "")).toContain("</i></a>");
    });

    it("shows fileOriginalName if fileOriginalName is specified and found", () => {
        var formatter = new FileDownloadFormatter();
        formatter.originalNameProperty = "fileOriginalName";
        expect(formatter.format(ctx({ value: "file", item: { fileOriginalName: "test" } }))).toContain("</i> test</a>");
    });

    it("uses displayFormat if displayFormat is specified", () => {
        var formatter = new FileDownloadFormatter();
        formatter.originalNameProperty = "fileOriginalName";
        formatter.displayFormat = "originalName: {0} dbFile: {1} downloadUrl: {2}"
        expect(formatter.format(ctx({ value: "file", item: { fileOriginalName: "test" } })))
            .toContain("</i> originalName: test dbFile: file downloadUrl: /upload/file</a>");
    });

    it("uses icon if specified", () => {
        var formatter = new FileDownloadFormatter();
        formatter.iconClass = "testicon"
        expect(formatter.format(ctx({ value: "file", item: { fileOriginalName: "test" } })))
            .toContain("'testicon'");
    });

    it("adds fa if icon starts with fa", () => {
        var formatter = new FileDownloadFormatter();
        formatter.iconClass = "fa-testicon"
        expect(formatter.format(ctx({ value: "file", item: { fileOriginalName: "test" } })))
            .toContain("'fa fa-testicon'");
    });

    it("uses default icon if not specified", () => {
        var formatter = new FileDownloadFormatter();
        expect(formatter.format(ctx({ value: "file", item: { fileOriginalName: "test" } })))
            .toContain("'fa fa-download'");
    });

    it("adds originalNameProperty to referencedFields if specified", () => {
        var formatter = new FileDownloadFormatter();
        formatter.originalNameProperty = "fileOriginalName";
        var column = {};
        formatter.initializeColumn(column);
        expect(column).toEqual({ referencedFields: ["fileOriginalName"] });
    })
});
