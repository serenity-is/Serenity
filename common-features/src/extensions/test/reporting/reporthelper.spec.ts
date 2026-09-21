import * as corelib from "@serenity-is/corelib";
import { afterEach, describe, expect, it, vi } from "vitest";
import { ReportHelper } from "../../Modules/Reporting/ReportHelper";

describe("ReportHelper", () => {
    afterEach(() => vi.restoreAllMocks());

    it("creates a tool button with defaults", () => {
        const button = ReportHelper.createToolButton({ reportKey: "R" });
        expect(button.title).toBe("Report");
        expect(button.cssClass).toBe("print-button");
        expect(typeof button.onClick).toBe("function");
    });

    it("uses custom title, css class and icon", () => {
        const button = ReportHelper.createToolButton({
            reportKey: "R",
            title: "My Report",
            cssClass: "my-button",
            icon: "fa fa-print"
        });
        expect(button.title).toBe("My Report");
        expect(button.cssClass).toBe("my-button");
        expect(button.icon).toBe("fa fa-print");
    });

    it("posts to the render url on click", () => {
        const postSpy = vi.spyOn(corelib, "postToUrl").mockImplementation(() => { });
        const button = ReportHelper.createToolButton({ reportKey: "R" });
        (button.onClick as any)({});
        expect(postSpy).toHaveBeenCalledTimes(1);
        const arg: any = postSpy.mock.calls[0][0];
        expect(arg.url).toContain("Serenity.Extensions/Report/Render");
        expect(arg.params.key).toBe("R");
        expect(arg.params.ext).toBe("pdf");
        expect(arg.params.opt).toBe("");
        expect(arg.target).toBe("_blank");
    });

    it("posts to the download url with params when download is true", () => {
        const postSpy = vi.spyOn(corelib, "postToUrl").mockImplementation(() => { });
        ReportHelper.execute({
            reportKey: "R",
            download: true,
            extension: "xlsx",
            target: "self",
            params: { a: 1 }
        });
        const arg: any = postSpy.mock.calls[0][0];
        expect(arg.url).toContain("Report/Download");
        expect(arg.params.ext).toBe("xlsx");
        expect(arg.params.opt).toBe(JSON.stringify({ a: 1 }));
        expect(arg.target).toBe("self");
    });

    it("uses getParams when provided", () => {
        const postSpy = vi.spyOn(corelib, "postToUrl").mockImplementation(() => { });
        ReportHelper.execute({
            reportKey: "R",
            getParams: () => ({ b: 2 })
        });
        const arg: any = postSpy.mock.calls[0][0];
        expect(arg.params.opt).toBe(JSON.stringify({ b: 2 }));
    });
});
