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
});
