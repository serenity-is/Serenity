import { formatterContext as ctx } from "@serenity-is/sleekgrid";
import { FileDownloadFormatter } from "./filedownloadformatter";

describe("FileDownloadFormatter", () => {
    it("shows empty string if value is null", () => {
        const formatter = new FileDownloadFormatter();
        expect(formatter.format(ctx({ value: null }))).toBe("");
    });


    it("replaces all backward slashes to forward", () => {
        const formatter = new FileDownloadFormatter();
        expect((formatter.format(ctx({ value: "file\\with\\backward\\slashes" })) as HTMLElement).outerHTML)
            .toContain("file/with/backward/slashes");
    });

    it("shows empty string if fileOriginalName is not specified", () => {
        const formatter = new FileDownloadFormatter();
        expect((formatter.format(ctx({ value: "file" })) as HTMLElement).outerHTML.replace(/\s/g, ""))
            .toContain("</i></a>");
    });

    it("shows empty string if fileOriginalName is specified but not found", () => {
        const formatter = new FileDownloadFormatter();
        formatter.originalNameProperty = "fileOriginalName";
        expect((formatter.format(ctx({ value: "file", item: {} })) as HTMLElement).outerHTML.replace(/\s/g, ""))
            .toContain("</i></a>");
    });

    it("shows fileOriginalName if fileOriginalName is specified and found", () => {
        const formatter = new FileDownloadFormatter();
        formatter.originalNameProperty = "fileOriginalName";
        expect((formatter.format(ctx({ value: "file", item: { fileOriginalName: "test" } })) as HTMLElement).outerHTML)
            .toContain("</i> test</a>");
    });

    it("uses displayFormat if displayFormat is specified", () => {
        const formatter = new FileDownloadFormatter();
        formatter.originalNameProperty = "fileOriginalName";
        formatter.displayFormat = "originalName: {0} dbFile: {1} downloadUrl: {2}"
        expect((formatter.format(ctx({ value: "file", item: { fileOriginalName: "test" } })) as HTMLElement).outerHTML)
            .toContain("</i> originalName: test dbFile: file downloadUrl: /upload/file</a>");
    });

    it("uses icon if specified", () => {
        const formatter = new FileDownloadFormatter();
        formatter.iconClass = "testicon"
        expect((formatter.format(ctx({ value: "file", item: { fileOriginalName: "test" } })) as HTMLElement).outerHTML)
            .toContain("testicon");
    });

    it("adds fa if icon starts with fa", () => {
        const formatter = new FileDownloadFormatter();
        formatter.iconClass = "fa-testicon"
        expect((formatter.format(ctx({ value: "file", item: { fileOriginalName: "test" } })) as HTMLElement).outerHTML)
            .toContain("fa fa-testicon");
    });

    it("uses default icon if not specified", () => {
        const formatter = new FileDownloadFormatter();
        expect((formatter.format(ctx({ value: "file", item: { fileOriginalName: "test" } })) as HTMLElement).outerHTML)
            .toContain("fa fa-download");
    });

    it("adds originalNameProperty to referencedFields if specified", () => {
        const formatter = new FileDownloadFormatter();
        formatter.originalNameProperty = "fileOriginalName";
        const column = {};
        formatter.initializeColumn(column);
        expect(column).toEqual({ referencedFields: ["fileOriginalName"] });
    })
});
