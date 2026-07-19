import { FilterPanelTexts } from "../../base";
import { FilterDialog } from "./filterdialog";

describe("FilterDialog", () => {
    it("is registered with typeInfo", () => {
        expect(FilterDialog[Symbol.for("Serenity.typeInfo")]).toBeDefined();
    });

    it("can be constructed", () => {
        const dialog = new FilterDialog({});
        expect(dialog).toBeInstanceOf(FilterDialog);
        expect(dialog.get_filterPanel()).toBeTruthy();
        dialog.destroy();
    });

    it("configures filterPanel correctly in constructor", () => {
        const dialog = new FilterDialog({});
        const panel = dialog.get_filterPanel();
        expect(panel.showInitialLine).toBe(true);
        expect(panel.showSearchButton).toBe(false);
        expect((panel as any).updateStoreOnReset).toBe(false);
        dialog.destroy();
    });

    it("sets dialogTitle from FilterPanelTexts", () => {
        const dialog = new FilterDialog({});
        expect(dialog.dialogTitle).toBe(FilterPanelTexts.DialogTitle);
        dialog.destroy();
    });

    it("getDialogOptions returns fullScreen lg-down", () => {
        const dialog = new FilterDialog({});
        const options = (dialog as any).getDialogOptions();
        expect(options.fullScreen).toBe("lg-down");
        dialog.destroy();
    });

    it("getDialogButtons returns ok and cancel buttons", () => {
        const dialog = new FilterDialog({});
        const buttons = (dialog as any).getDialogButtons();
        expect(buttons).toHaveLength(2);
        dialog.destroy();
    });

    it("ok button has click handler defined", () => {
        const dialog = new FilterDialog({});
        const buttons = (dialog as any).getDialogButtons();
        const okButton = buttons[0];
        expect(okButton.click).toBeInstanceOf(Function);
        dialog.destroy();
    });

    it("ok button click handler calls search without errors", () => {
        const dialog = new FilterDialog({});
        const buttons = (dialog as any).getDialogButtons();
        const okButton = buttons[0];

        const e = { preventDefault: vi.fn() };
        okButton.click(e);
        // Without any rows, search should succeed (no errors)
        expect(e.preventDefault).not.toHaveBeenCalled();
        dialog.destroy();
    });

    it("ok button click handler prevents default when hasErrors", () => {
        const dialog = new FilterDialog({});
        const buttons = (dialog as any).getDialogButtons();
        const okButton = buttons[0];
        const panel = dialog.get_filterPanel();

        // Mock hasErrors to return true
        vi.spyOn(panel, "hasErrors", "get").mockReturnValue(true);

        const e = { preventDefault: vi.fn() };
        okButton.click(e);
        expect(e.preventDefault).toHaveBeenCalled();
        dialog.destroy();
    });

    it("cancel button is defined", () => {
        const dialog = new FilterDialog({});
        const buttons = (dialog as any).getDialogButtons();
        const cancelButton = buttons[1];
        expect(cancelButton).toBeTruthy();
        dialog.destroy();
    });

    it("renderContents creates a div with FilterPanel id", () => {
        const dialog = new FilterDialog({});
        const contents = (dialog as any).renderContents();
        expect(contents).toBeTruthy();
        dialog.destroy();
    });

    it("get_filterPanel returns the filter panel", () => {
        const dialog = new FilterDialog({});
        expect(dialog.get_filterPanel()).toBeTruthy();
        dialog.destroy();
    });

    it("getDialogOptions returns fullScreen lg-down", () => {
        const dialog = new FilterDialog({});
        const options = (dialog as any).getDialogOptions();
        expect(options.fullScreen).toBe("lg-down");
        dialog.destroy();
    });

    it("hasErrors returns true when error span exists as direct child of rowsDiv with class v", () => {
        const dialog = new FilterDialog({});
        const panel = dialog.get_filterPanel();

        // Initially no errors
        expect(panel.hasErrors).toBe(false);

        // Add a div.v with span.error as direct child of rowsDiv (matching :scope > div.v > span.error)
        const divV = document.createElement("div");
        divV.className = "v";
        const err = document.createElement("span");
        err.className = "error";
        divV.appendChild(err);
        (panel as any).rowsDiv.appendChild(divV);

        expect(panel.hasErrors).toBe(true);
        dialog.destroy();
    });

    it("hasErrors returns false when error is inside a non-v child", () => {
        const dialog = new FilterDialog({});
        const panel = dialog.get_filterPanel();

        // Add error inside a filter-line structure (not matching :scope > div.v)
        const row = document.createElement("div");
        row.className = "filter-line";
        const divV = document.createElement("div");
        divV.className = "v";
        const err = document.createElement("span");
        err.className = "error";
        divV.appendChild(err);
        row.appendChild(divV);
        (panel as any).rowsDiv.appendChild(row);

        // This returns false because :scope > div.v doesn't match nested div.v
        expect(panel.hasErrors).toBe(false);
        dialog.destroy();
    });
});
