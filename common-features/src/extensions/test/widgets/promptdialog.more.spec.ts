import { Dialog, StringEditor } from "@serenity-is/corelib";
import { PromptDialog } from "../../Modules/Widgets/PromptDialog";

afterEach(() => {
    vi.useRealTimers();
    vi.restoreAllMocks();
    document.body.innerHTML = "";
});

describe("PromptDialog options and value", () => {
    it("applies a css class to the dialog node", () => {
        const dlg = new PromptDialog({ required: false, cssClass: "my-prompt", validateValue: () => true });
        expect(dlg.domNode.classList.contains("my-prompt")).toBe(true);
        dlg.destroy();
    });

    it("renders a message element", () => {
        const dlg = new PromptDialog({ required: false, message: "Hello", validateValue: () => true });
        expect(dlg.domNode.querySelector(".message")).toBeTruthy();
        expect(dlg.domNode.querySelector(".message").textContent).toBe("Hello");
        dlg.destroy();
    });

    it("renders an html message when isHtml is set", () => {
        const dlg = new PromptDialog({ required: false, message: "<b>Hi</b>", isHtml: true, validateValue: () => true } as any);
        expect(dlg.domNode.querySelector(".message").innerHTML).toBe("<b>Hi</b>");
        dlg.destroy();
    });

    it("sets a custom title", () => {
        const dlg = new PromptDialog({ required: false, title: "Custom", validateValue: () => true });
        expect(dlg.dialogTitle).toBe("Custom");
        dlg.destroy();
    });

    it("exposes closeOnEscape in dialog options", () => {
        const dlg = new PromptDialog({ required: false, closeOnEscape: false, validateValue: () => true });
        expect(dlg["getDialogOptions"]().closeOnEscape).toBe(false);
        dlg.destroy();
    });

    it("returns property items for the value", () => {
        const dlg = new PromptDialog({ required: true, validateValue: () => true });
        const items = dlg["getPropertyItems"]();
        expect(items[0].name).toBe("Value");
        expect(items[0].required).toBe(true);
        dlg.destroy();
    });

    it("uses the specified editor type", () => {
        const dlg = new PromptDialog({ required: false, editorType: StringEditor, editorOptions: { maxLength: 5 }, validateValue: () => true });
        const items = dlg["getPropertyItems"]();
        expect(items[0].editorParams).toEqual({ maxLength: 5 });
        dlg.destroy();
    });

    it("gets and sets the value", () => {
        const dlg = new PromptDialog({ required: false, value: "initial", validateValue: () => true });
        expect(dlg.value).toBe("initial");
        dlg.value = "updated";
        expect(dlg.value).toBe("updated");
        dlg.destroy();
    });

    it("returns the value editor widget", () => {
        const dlg = new PromptDialog({ required: false, validateValue: () => true });
        const editor = dlg.getEditor(StringEditor);
        expect(editor).toBeTruthy();
        dlg.destroy();
    });

    it("submits on enter when enabled", async () => {
        vi.useFakeTimers();
        const dlg = new PromptDialog({ required: false, submitOnEnter: true, validateValue: () => true });
        dlg.dialogOpen();
        const inst = Dialog.getInstance(dlg.element);
        const footer = inst.getFooterNode();
        const okButton = footer.querySelector("button");
        const clickSpy = vi.spyOn(okButton, "click");
        const valueEl = document.getElementById(dlg.idPrefix + "Value");
        valueEl.dispatchEvent(new KeyboardEvent("keydown", { key: "Enter", bubbles: true }));
        expect(clickSpy).toHaveBeenCalled();
        await vi.runOnlyPendingTimersAsync();
        dlg.destroy();
    });

    it("static prompt opens a dialog and invokes the validator", () => {
        let captured: PromptDialog;
        const openSpy = vi.spyOn(PromptDialog.prototype, "dialogOpen").mockImplementation(function (this: PromptDialog) {
            captured = this;
        });
        let validatorCalled = false;
        PromptDialog.prompt("Title", "Message", "value", v => {
            validatorCalled = true;
            return true;
        });
        expect(openSpy).toHaveBeenCalled();
        expect(captured).toBeTruthy();
        vi.spyOn(captured as any, "validateForm").mockReturnValue(true);
        captured["getDialogButtons"]()[0].click({ preventDefault: () => { } } as any);
        expect(validatorCalled).toBe(true);
        captured.destroy();
    });

    it("prevents default on ok click when validation fails", () => {
        const dlg = new PromptDialog({ required: false, validateValue: () => true });
        vi.spyOn(dlg as any, "validateForm").mockReturnValue(false);
        const event = { preventDefault: vi.fn() };
        dlg["getDialogButtons"]()[0].click(event as any);
        expect(event.preventDefault).toHaveBeenCalled();
        dlg.destroy();
    });
});
