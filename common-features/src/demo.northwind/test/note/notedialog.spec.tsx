import { mockDynamicData } from "test-utils";
import { NoteDialog } from "../../Modules/Note/NoteDialog";

beforeAll(() => {
    mockDynamicData();
});

describe("NoteDialog", () => {
    it("text property can be set and get", () => {
        const dialog = new NoteDialog({});
        dialog.text = "test";
        expect(dialog.text).toBe("test");
        dialog.destroy();
    });

    it("renders a text editor", () => {
        const dialog = new NoteDialog({});
        expect(dialog["textEditor"]).toBeTruthy();
        dialog.destroy();
    });

    it("provides ok and cancel dialog buttons", () => {
        const dialog = new NoteDialog({});
        const buttons = dialog["getDialogButtons"]();
        expect(buttons.length).toBe(2);
        dialog.destroy();
    });

    it("invokes okClick when the form is valid", () => {
        const dialog = new NoteDialog({});
        dialog.text = "Note text";
        const okClick = vi.fn();
        dialog.okClick = okClick;
        const buttons = dialog["getDialogButtons"]();
        buttons[0].click({ preventDefault: vi.fn() } as any);
        expect(okClick).toHaveBeenCalled();
        dialog.destroy();
    });

    it("prevents the ok button when the form is invalid", () => {
        const dialog = new NoteDialog({});
        dialog.text = "";
        vi.spyOn(dialog as any, "validateForm").mockReturnValue(false);
        const okClick = vi.fn();
        dialog.okClick = okClick;
        const preventDefault = vi.fn();
        const buttons = dialog["getDialogButtons"]();
        buttons[0].click({ preventDefault } as any);
        expect(preventDefault).toHaveBeenCalled();
        expect(okClick).not.toHaveBeenCalled();
        dialog.destroy();
    });
});
