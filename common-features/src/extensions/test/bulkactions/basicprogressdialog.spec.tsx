import { Dialog, DialogTexts } from "@serenity-is/corelib";
import { describe, expect, it, vi } from "vitest";
import { BasicProgressDialog } from "../../Modules/BulkActions/BasicProgressDialog";
import { BasicProgressDialogTexts } from "../../Modules/ServerTypes/Texts";

describe("BasicProgressDialog", () => {
    it("sets the default title from texts", () => {
        const dlg = new BasicProgressDialog({});
        expect(dlg.title).toBe(BasicProgressDialogTexts.PleaseWait);
        dlg.destroy();
    });

    it("has a 600px width dialog option", () => {
        const dlg = new BasicProgressDialog({});
        expect(dlg["getDialogOptions"]().width).toBe(600);
        dlg.destroy();
    });

    it("gets and sets max value through the progress bar", () => {
        const dlg = new BasicProgressDialog({});
        dlg.max = 50;
        expect(dlg.max).toBe(50);
        dlg.max = 0;
        expect(dlg.max).toBe(100);
        dlg.destroy();
    });

    it("gets and sets value and updates the progress bar styles", () => {
        const dlg = new BasicProgressDialog({});
        dlg.max = 200;
        dlg.value = 50;
        expect(dlg.value).toBe(50);
        const bar = (dlg as any).progressBar as HTMLElement;
        expect(bar.style.width).toBe("25%");
        expect(bar.textContent).toBe("50 / 200");
        dlg.value = 0;
        expect(dlg.value).toBe(0);
        dlg.destroy();
    });

    it("sets the dialog title", () => {
        const dlg = new BasicProgressDialog({});
        dlg.title = "Custom";
        expect(dlg.title).toBe("Custom");
        dlg.destroy();
    });

    it("marks cancelled and uses the default cancel title on cancel click", () => {
        const dlg = new BasicProgressDialog({});
        const buttons = dlg["getDialogButtons"]();
        buttons[0].click();
        expect(dlg.cancelled).toBe(true);
        expect(dlg.title).toBe(BasicProgressDialogTexts.CancelTitle);
        dlg.destroy();
    });

    it("uses a custom cancel title on cancel click", () => {
        const dlg = new BasicProgressDialog({});
        dlg.cancelTitle = "Cancelling";
        const buttons = dlg["getDialogButtons"]();
        buttons[0].click();
        expect(dlg.title).toBe("Cancelling");
        dlg.destroy();
    });

    it("disables footer buttons on cancel click when a dialog instance exists", () => {
        const dlg = new BasicProgressDialog({});
        const footer = document.createElement("div");
        const btn = document.createElement("button");
        footer.appendChild(btn);
        vi.spyOn(Dialog, "getInstance").mockReturnValue({ getFooterNode: () => footer } as any);
        dlg["getDialogButtons"]()[0].click();
        expect(btn.getAttribute("disabled")).toBe("disabled");
        vi.restoreAllMocks();
        dlg.destroy();
    });

    it("hides the titlebar close button on initDialog", () => {
        const wrapper = document.createElement("div");
        wrapper.className = "ui-dialog";
        const close = document.createElement("div");
        close.className = "ui-dialog-titlebar-close";
        wrapper.appendChild(close);
        const dlg = new BasicProgressDialog({ element: wrapper });
        dlg["initDialog"]();
        expect(close.hidden).toBe(true);
        dlg.destroy();
    });

    it("cancels the dialog through the cancel button text", () => {
        const dlg = new BasicProgressDialog({});
        const inst = Dialog.getInstance(dlg.element);
        if (inst) {
            const footer = inst.getFooterNode();
            const button = Array.from(footer.querySelectorAll("button")).find(x => x.textContent.trim() === DialogTexts.CancelButton);
            expect(button).toBeTruthy();
        }
        dlg.destroy();
    });
});
