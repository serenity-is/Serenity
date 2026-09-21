import * as corelib from "@serenity-is/corelib";
import { afterEach, describe, expect, it, vi } from "vitest";
import { ExcelExportHelper } from "../../Modules/ExportImport/ExcelExportHelper";

function makeGrid(overrides: any = {}) {
    return {
        prepareSubmit: vi.fn(() => true),
        getView: () => ({
            params: { Skip: 10, Take: 20, Sort: undefined },
            sortBy: "Name"
        }),
        getGrid: () => ({
            getColumns: () => [
                { id: "id", field: "Id" },
                { field: "Name" }
            ]
        }),
        ...overrides
    } as any;
}

describe("ExcelExportHelper", () => {
    afterEach(() => vi.restoreAllMocks());

    it("creates a tool button with defaults", () => {
        const button = ExcelExportHelper.createToolButton({ grid: makeGrid(), service: "Test/List" });
        expect(button.hint).toBe("Excel");
        expect(button.title).toBe("");
        expect(button.cssClass).toBe("export-xlsx-button");
        expect(typeof button.onClick).toBe("function");
    });

    it("posts the export request on click", () => {
        const postSpy = vi.spyOn(corelib, "postToService").mockImplementation(() => { });
        const grid = makeGrid();
        const button = ExcelExportHelper.createToolButton({ grid, service: "Test/List" });
        (button.onClick as any)({});
        expect(postSpy).toHaveBeenCalledTimes(1);
        const arg = postSpy.mock.calls[0][0] as any;
        expect(arg.service).toBe("Test/List");
        expect(arg.target).toBe("_blank");
        expect(arg.request.Skip).toBe(0);
        expect(arg.request.Take).toBe(0);
        expect(arg.request.Sort).toBe("Name");
        expect(arg.request.ExportColumns).toEqual(["id", "Name"]);
    });

    it("does not post when onViewSubmit returns false", () => {
        const postSpy = vi.spyOn(corelib, "postToService").mockImplementation(() => { });
        const button = ExcelExportHelper.createToolButton({
            grid: makeGrid(),
            service: "Test/List",
            onViewSubmit: () => false
        });
        (button.onClick as any)({});
        expect(postSpy).not.toHaveBeenCalled();
    });

    it("does not post when prepareSubmit returns false", () => {
        const postSpy = vi.spyOn(corelib, "postToService").mockImplementation(() => { });
        const button = ExcelExportHelper.createToolButton({
            grid: makeGrid({ prepareSubmit: () => false }),
            service: "Test/List"
        });
        (button.onClick as any)({});
        expect(postSpy).not.toHaveBeenCalled();
    });

    it("uses onViewSubmit over prepareSubmit when provided", () => {
        const postSpy = vi.spyOn(corelib, "postToService").mockImplementation(() => { });
        const grid = makeGrid({ prepareSubmit: () => false });
        const button = ExcelExportHelper.createToolButton({
            grid,
            service: "Test/List",
            onViewSubmit: () => true
        });
        (button.onClick as any)({});
        expect(postSpy).toHaveBeenCalledTimes(1);
    });

    it("applies editRequest and custom options", () => {
        const postSpy = vi.spyOn(corelib, "postToService").mockImplementation(() => { });
        const button = ExcelExportHelper.createToolButton({
            grid: makeGrid(),
            service: "Test/List",
            title: "Download",
            hint: "Save",
            separator: true,
            editRequest: r => {
                r.Sort = ["Edited"];
                return r;
            }
        });
        expect(button.title).toBe("Download");
        expect(button.hint).toBe("Save");
        expect(button.separator).toBe(true);
        (button.onClick as any)({});
        expect((postSpy.mock.calls[0][0] as any).request.Sort).toEqual(["Edited"]);
    });
});
