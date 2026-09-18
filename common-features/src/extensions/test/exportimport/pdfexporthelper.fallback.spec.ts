import * as corelib from "@serenity-is/corelib";
import { PdfExportHelper } from "../../Modules/ExportImport/PdfExportHelper";

const state = vi.hoisted(() => ({
    applyPlugin: true,
    last: null as any
}));

vi.mock("jspdf", () => {
    throw new Error("jspdf is not available from the import map");
});

vi.mock("jspdf-autotable", () => {
    throw new Error("jspdf-autotable is not available from the import map");
});

vi.mock("virtual-jspdf-asset", () => {
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
            (FakeJsPDF as any).API = undefined;
            return FakeJsPDF;
        },
        get applyPlugin() {
            if (!state.applyPlugin)
                return undefined;
            return (jsPDF: any) => { jsPDF.API = { autoTable: {} }; };
        }
    };
});

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
    state.applyPlugin = true;
    state.last = null;
    vi.spyOn(corelib, "resolveUrl").mockReturnValue("virtual-jspdf-asset");
    vi.spyOn(corelib, "serviceCall").mockImplementation((opt: any) => {
        opt.onSuccess?.({ Entities: [] });
        return { then: () => { } } as any;
    });
});

afterEach(() => {
    delete (globalThis as any).jsPDF;
    vi.restoreAllMocks();
});

describe("PdfExportHelper asset fallbacks", () => {
    it("loads jsPDF and autotable from the fallback asset bundle", async () => {
        PdfExportHelper.exportToPdf({ grid: makeGrid() });
        await vi.waitFor(() => expect(state.last).toBeTruthy());
        expect(state.last.autoTable).toHaveBeenCalled();
    });

    it("notifies an error when the fallback asset has no autotable plugin", async () => {
        state.applyPlugin = false;
        const errorSpy = vi.spyOn(corelib, "notifyError");
        PdfExportHelper.exportToPdf({ grid: makeGrid() });
        await vi.waitFor(() => expect(errorSpy).toHaveBeenCalled());
    });
});
