import * as corelib from "@serenity-is/corelib";
import { mockFetch, unmockFetch } from "test-utils";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { ReportDialog } from "../../Modules/Reporting/ReportDialog";
import { ReportHelper } from "../../Modules/Reporting/ReportHelper";

function reportData(overrides: any = {}) {
    return {
        ReportKey: "Some.Report",
        Title: "My Report Title",
        Properties: [],
        InitialSettings: {},
        IsDataOnlyReport: false,
        IsExternalReport: false,
        ...overrides
    };
}

function makeDialog(data = reportData()) {
    vi.spyOn(corelib, "serviceCall").mockImplementation((opt: any) => {
        opt.onSuccess?.(data);
        return { then: () => { } } as any;
    });
    return new ReportDialog({ reportKey: "Some.Report" });
}

beforeEach(() => {
    mockFetch({ "*": () => ({}) });
});

afterEach(() => {
    unmockFetch();
    vi.restoreAllMocks();
});

describe("ReportDialog", () => {
    it("loads the report and sets the dialog title", () => {
        const dialog = makeDialog();
        expect(dialog.dialogTitle).toBe("My Report Title");
        expect(dialog["propertyGrid"]).toBeTruthy();
        expect(dialog["report"].ReportKey).toBe("Some.Report");
        dialog.destroy();
    });

    it("returns null dialog buttons", () => {
        const dialog = makeDialog();
        expect(dialog["getDialogButtons"]()).toBeNull();
        dialog.destroy();
    });

    it("exposes toolbar buttons with click handlers", () => {
        const dialog = makeDialog();
        const buttons = dialog["getToolbarButtons"]();
        expect(buttons.length).toBe(4);
        expect(buttons.map(b => b.cssClass)).toEqual([
            "print-preview-button",
            "run-button",
            "export-pdf-button",
            "export-xlsx-button"
        ]);
        dialog.destroy();
    });

    it("executes a report using the property grid values", () => {
        const dialog = makeDialog();
        vi.spyOn(dialog as any, "validateForm").mockReturnValue(true);
        const executeSpy = vi.spyOn(ReportHelper, "execute").mockImplementation(() => { });
        dialog.executeReport("_blank", "pdf", true);
        expect(executeSpy).toHaveBeenCalledTimes(1);
        expect((executeSpy.mock.calls[0][0] as any).reportKey).toBe("Some.Report");
        expect((executeSpy.mock.calls[0][0] as any).download).toBe(true);
        dialog.destroy();
    });

    it("does not execute a report when validation fails", () => {
        const dialog = makeDialog();
        vi.spyOn(dialog as any, "validateForm").mockReturnValue(false);
        const executeSpy = vi.spyOn(ReportHelper, "execute").mockImplementation(() => { });
        dialog.executeReport("_blank", "pdf", false);
        expect(executeSpy).not.toHaveBeenCalled();
        dialog.destroy();
    });

    it("executes a report from the toolbar buttons", () => {
        const dialog = makeDialog();
        vi.spyOn(dialog as any, "validateForm").mockReturnValue(true);
        const executeSpy = vi.spyOn(ReportHelper, "execute").mockImplementation(() => { });
        const buttons = dialog["getToolbarButtons"]();
        buttons.forEach(button => (button.onClick as any)({}));
        expect(executeSpy).toHaveBeenCalledTimes(buttons.length);
        expect((executeSpy.mock.calls[1][0] as any).extension).toBeNull();
        expect((executeSpy.mock.calls[2][0] as any).extension).toBe("pdf");
        expect((executeSpy.mock.calls[3][0] as any).extension).toBe("xlsx");
        dialog.destroy();
    });

    it("toggles toolbar buttons based on the report type", () => {
        const dialog = makeDialog(reportData({ IsDataOnlyReport: true }));
        expect(dialog["report"].IsDataOnlyReport).toBe(true);
        dialog["updateInterface"]();
        dialog.destroy();
    });

    it("toggles toolbar buttons for external reports", () => {
        const dialog = makeDialog(reportData({ IsExternalReport: true }));
        dialog["updateInterface"]();
        dialog.destroy();
    });
});
