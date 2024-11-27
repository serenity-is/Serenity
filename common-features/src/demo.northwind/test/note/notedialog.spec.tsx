import { NoteDialog } from '../../Modules/Note/NoteDialog';

describe("NoteDialog", () => {
    it("text property can be set and get", () => {
        const dialog = new NoteDialog({});
        dialog.text = "test";
        expect(dialog.text).toBe("test");
    });

});