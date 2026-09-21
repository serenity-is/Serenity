import * as corelib from "@serenity-is/corelib";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { PdfExportHelper } from "../../Modules/ExportImport/PdfExportHelper";

function makeSleekGrid(columns: any[], formatter?: any) {
    return {
        getFormatter: () => formatter,
        getFormatterContext: () => ({ purpose: undefined, item: undefined, value: undefined })
    } as any;
}

function makeDataGrid(overrides: any = {}) {
    const columns = [
        { id: "id", field: "Id", name: "Id", cssClass: "align-right" },
        { id: "name", field: "Name", name: "Name", cssClass: "align-center" },
        { id: "other", field: "Other", name: "Other" }
    ];
    return {
        prepareSubmit: vi.fn(() => true),
        view: { params: { Skip: 0, Take: 10 }, sortBy: "Name", url: "Test/List" },
        columns,
        sleekGrid: makeSleekGrid(columns),
        getTitle: () => "My Report",
        ...overrides
    } as any;
}

class FakeJsPDF {
    static last: FakeJsPDF;
    static API = { autoTable: {} };
    calls: string[] = [];
    internal = { pageSize: { width: 800, height: 600 } };
    setFontSize = vi.fn();
    setFont = vi.fn();
    autoTableText = vi.fn();
    putTotalPages = vi.fn();
    save = vi.fn();
    output = vi.fn();
    autoPrint = vi.fn();
    constructor() {
        FakeJsPDF.last = this;
    }
    autoTable = vi.fn((opts: any) => {
        opts?.didDrawPage?.({ pageNumber: 1, pageCount: 2 });
    });
}

describe("PdfExportHelper", () => {
    beforeEach(() => {
        (globalThis as any).jsPDF = FakeJsPDF as any;
        vi.spyOn(corelib, "serviceCall").mockImplementation((opt: any) => {
            opt.onSuccess?.({ Entities: [{ Id: 1, Name: "One", Other: "X" }] });
            return { then: () => { } } as any;
        });
    });

    afterEach(() => {
        delete (globalThis as any).jsPDF;
        vi.restoreAllMocks();
    });

    it("creates a tool button", () => {
        const button = PdfExportHelper.createToolButton({ grid: makeDataGrid() });
        expect(button.cssClass).toBe("export-pdf-button");
        expect(button.hint).toBe("PDF");
        expect(typeof button.onClick).toBe("function");
    });

    it("does not export when onViewSubmit returns false", () => {
        const button = PdfExportHelper.createToolButton({ grid: makeDataGrid(), onViewSubmit: () => false });
        expect(() => (button.onClick as any)({})).not.toThrow();
    });

    it("does not export when prepareSubmit returns false", () => {
        PdfExportHelper.exportToPdf({ grid: makeDataGrid({ prepareSubmit: () => false }) });
        expect(corelib.serviceCall).not.toHaveBeenCalled();
    });

    it("exports to a file with page numbers and datetime header", () => {
        PdfExportHelper.exportToPdf({ grid: makeDataGrid() });
        const doc = FakeJsPDF.last;
        expect(doc.autoTable).toHaveBeenCalledTimes(1);
        expect(doc.save).toHaveBeenCalledTimes(1);
        expect(doc.putTotalPages).toHaveBeenCalled();
        const opts = doc.autoTable.mock.calls[0][0];
        expect(opts.head[0]).toEqual(["Id", "Name", "Other"]);
        expect(opts.columnStyles.id.halign).toBe("right");
        expect(opts.columnStyles.name.halign).toBe("center");
    });

    it("skips datetime header when printDateTimeHeader is false", () => {
        PdfExportHelper.exportToPdf({ grid: makeDataGrid(), printDateTimeHeader: false });
        const doc = FakeJsPDF.last;
        expect(doc.autoTable).toHaveBeenCalledTimes(1);
    });

    it("writes output to new window without saving a file", () => {
        PdfExportHelper.exportToPdf({ grid: makeDataGrid(), output: "newwindow" });
        const doc = FakeJsPDF.last;
        expect(doc.output).toHaveBeenCalledWith("dataurlnewwindow");
        expect(doc.save).not.toHaveBeenCalled();
    });

    it("maps window output and autoPrint", () => {
        PdfExportHelper.exportToPdf({ grid: makeDataGrid(), output: "window", autoPrint: true });
        const doc = FakeJsPDF.last;
        expect(doc.autoPrint).toHaveBeenCalled();
        expect(doc.output).toHaveBeenCalledWith("dataurlnewwindow");
    });

    it("passes through other output types", () => {
        PdfExportHelper.exportToPdf({ grid: makeDataGrid(), output: "blob" });
        expect(FakeJsPDF.last.output).toHaveBeenCalledWith("dataurlnewwindow");
    });

    it("uses custom title, file name and column titles", () => {
        PdfExportHelper.exportToPdf({
            grid: makeDataGrid(),
            reportTitle: "Custom Title",
            fileName: "report_{0}_{1}.pdf",
            columnTitles: { name: "The Name" }
        });
        const doc = FakeJsPDF.last;
        expect(doc.save).toHaveBeenCalledWith(expect.stringContaining("report_My Report_"));
        const opts = doc.autoTable.mock.calls[0][0];
        expect(opts.head[0]).toEqual(["Id", "The Name", "Other"]);
    });

    it("filters out select columns", () => {
        const grid = makeDataGrid();
        grid.columns = [{ id: "__select__", field: "__select__" }, ...grid.columns];
        PdfExportHelper.exportToPdf({ grid });
        expect(FakeJsPDF.last.save).toHaveBeenCalled();
    });

    it("renders formatter results that are DOM nodes", () => {
        const span = document.createElement("span");
        span.textContent = "Rendered";
        const grid = makeDataGrid();
        grid.sleekGrid = makeSleekGrid(grid.columns, () => span) as any;
        PdfExportHelper.exportToPdf({ grid });
        const opts = FakeJsPDF.last.autoTable.mock.calls[0][0];
        expect(opts.body[0][0]).toBe("Rendered");
    });

    it("renders select formatter results", () => {
        const select = document.createElement("select");
        const option = document.createElement("option");
        option.setAttribute("selected", "");
        option.textContent = "Selected";
        select.appendChild(option);
        const grid = makeDataGrid();
        grid.sleekGrid = makeSleekGrid(grid.columns, () => select) as any;
        PdfExportHelper.exportToPdf({ grid });
        const opts = FakeJsPDF.last.autoTable.mock.calls[0][0];
        expect(opts.body[0][0]).toBe("Selected");
    });

    it("renders input formatter results", () => {
        const input = document.createElement("input");
        input.value = "InputValue";
        const grid = makeDataGrid();
        grid.sleekGrid = makeSleekGrid(grid.columns, () => input) as any;
        PdfExportHelper.exportToPdf({ grid });
        const opts = FakeJsPDF.last.autoTable.mock.calls[0][0];
        expect(opts.body[0][0]).toBe("InputValue");
    });

    it("handles a missing entity list", () => {
        vi.spyOn(corelib, "serviceCall").mockImplementation((opt: any) => {
            opt.onSuccess?.({});
            return { then: () => { } } as any;
        });
        PdfExportHelper.exportToPdf({ grid: makeDataGrid() });
        const opts = FakeJsPDF.last.autoTable.mock.calls[0][0];
        expect(opts.body).toEqual([]);
    });

    it("reports an error when jsPDF cannot be loaded", async () => {
        delete (globalThis as any).jsPDF;
        const errorSpy = vi.spyOn(corelib, "notifyError");
        PdfExportHelper.exportToPdf({ grid: makeDataGrid() });
        await vi.waitFor(() => expect(errorSpy).toHaveBeenCalled());
    });
});
