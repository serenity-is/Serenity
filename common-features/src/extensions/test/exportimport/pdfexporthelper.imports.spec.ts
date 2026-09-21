import * as corelib from "@serenity-is/corelib";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { PdfExportHelper } from "../../Modules/ExportImport/PdfExportHelper";

const state = vi.hoisted(() => ({
    hasJsPdf: true,
    hasApiAutoTable: false,
    applyPlugin: true,
    last: null as any
}));

vi.mock("jspdf", () => {
    class FakeJsPDF {
        constructor() {
            state.last = this;
        }
        internal = { pageSize: { width: 800, height: 600 } };
        setFontSize = vi.fn();
        setFont = vi.fn();
        autoTableText = vi.fn();
        putTotalPages = vi.fn();
        save = vi.fn();
        output = vi.fn();
        autoPrint = vi.fn();
        autoTable = vi.fn();
    }
    return {
        get jsPDF() {
            if (!state.hasJsPdf)
                return undefined;
            (FakeJsPDF as any).API = state.hasApiAutoTable ? { autoTable: {} } : undefined;
            return FakeJsPDF;
        }
    };
});

vi.mock("jspdf-autotable", () => ({
    get applyPlugin() {
        if (!state.applyPlugin)
            return undefined;
        return (jsPDF: any) => { jsPDF.API = { autoTable: {} }; };
    }
}));

function makeGrid() {
    return {
        prepareSubmit: () => true,
        view: { params: {}, sortBy: undefined, url: "Test/List" },
        columns: [{ id: "id", field: "Id", name: "Id" }],
        sleekGrid: {
            getFormatter: () => null,
            getFormatterContext: () => ({})
        },
        getTitle: () => "Report"
    } as any;
}

beforeEach(() => {
    delete (globalThis as any).jsPDF;
    state.hasJsPdf = true;
    state.hasApiAutoTable = false;
    state.applyPlugin = true;
    state.last = null;
    vi.spyOn(corelib, "serviceCall").mockImplementation((opt: any) => {
        opt.onSuccess?.({ Entities: [] });
        return { then: () => { } } as any;
    });
});

afterEach(() => {
    delete (globalThis as any).jsPDF;
    vi.restoreAllMocks();
});

describe("PdfExportHelper jsPDF import fallbacks", () => {
    it("loads jsPDF from the import map when it has autotable already", async () => {
        state.hasApiAutoTable = true;
        PdfExportHelper.exportToPdf({ grid: makeGrid() });
        await vi.waitFor(() => expect(state.last).toBeTruthy());
        expect(state.last.autoTable).toHaveBeenCalled();
    });

    it("loads jsPDF from the import map and applies the autotable plugin", async () => {
        state.hasApiAutoTable = false;
        state.applyPlugin = true;
        PdfExportHelper.exportToPdf({ grid: makeGrid() });
        await vi.waitFor(() => expect(state.last).toBeTruthy());
    });

    it("notifies an error when jsPDF is missing from the import map", async () => {
        state.hasJsPdf = false;
        const errorSpy = vi.spyOn(corelib, "notifyError");
        PdfExportHelper.exportToPdf({ grid: makeGrid() });
        await vi.waitFor(() => expect(errorSpy).toHaveBeenCalled());
    });

    it("notifies an error when the autotable plugin is missing", async () => {
        state.hasApiAutoTable = false;
        state.applyPlugin = false;
        const errorSpy = vi.spyOn(corelib, "notifyError");
        PdfExportHelper.exportToPdf({ grid: makeGrid() });
        await vi.waitFor(() => expect(errorSpy).toHaveBeenCalled());
    });
});

