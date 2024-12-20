import { Dialog, DialogTexts, Fluent } from "@serenity-is/corelib";
import { PromptDialog } from "../../Modules/Widgets/PromptDialog";

function clickButton(dlg: PromptDialog, text: string) {
    const inst = Dialog.getInstance(dlg.element);
    expect(inst).toBeTruthy();
    const footer = inst.getFooterNode();
    expect(footer).toBeTruthy();
    const button = Array.from(footer.querySelectorAll("button")).filter(x => x.textContent.trim() === text)[0];
    expect(button).toBeTruthy();
    Fluent(button).click();
}

function clickOkButton(dlg: PromptDialog) {
    clickButton(dlg, DialogTexts.OkButton);
}

beforeEach(() => {
    vi.useFakeTimers();
});

afterEach(() => {
    vi.useRealTimers();
    vi.restoreAllMocks();
});

describe("PromptDialog", () => {
    it("closes dialog if validateValue returns true", async () => {
        const dlg = new PromptDialog({
            required: false,
            validateValue: () => {
                return true;
            }
        });
        try {
            const closeSpy = vi.spyOn(Dialog.prototype, "close");
            dlg.dialogOpen();
            expect(closeSpy).not.toHaveBeenCalled();
            clickOkButton(dlg);
            await vi.runOnlyPendingTimersAsync();
            expect(closeSpy).toHaveBeenCalledTimes(1);
        }
        finally {
            dlg.destroy();
        }
    });

    it("does not allow closing dialog if validateValue returns false", async () => {
        const dlg = new PromptDialog({
            required: false,
            validateValue: () => {
                return false;
            }
        });
        try {
            const closeSpy = vi.spyOn(Dialog.prototype, "close");
            dlg.dialogOpen();
            expect(closeSpy).not.toHaveBeenCalled();
            clickOkButton(dlg);
            await vi.runOnlyPendingTimersAsync();
            expect(closeSpy).not.toHaveBeenCalled();
        }
        finally {
            dlg.destroy();
        }
    });
});