import * as corelib from "@serenity-is/corelib";
import { mockAdmin, mockDynamicData } from "test-utils";
import { afterEach, beforeAll, describe, expect, it, vi } from "vitest";
import { NoteDialog } from "../../Modules/Note/NoteDialog";
import { NotesEditor } from "../../Modules/Note/NotesEditor";

beforeAll(() => {
    mockDynamicData();
    mockAdmin();
});

afterEach(() => {
    vi.restoreAllMocks();
    document.body.innerHTML = "";
});

describe("NotesEditor", () => {
    it("stores and returns the note list value", () => {
        const editor = new NotesEditor({});
        const notes = [{ Text: "One" }];
        editor.value = notes;
        expect(editor.value).toEqual(notes);
        editor.value = null;
        expect(editor.value).toEqual([]);
        editor.destroy();
    });

    it("reads and writes edit values through property items", () => {
        const editor = new NotesEditor({});
        editor.value = [];
        const target: any = {};
        editor.getEditValue({ name: "NoteList" } as any, target);
        expect(target.NoteList).toEqual([]);

        editor.setEditValue({ NoteList: [{ Text: "Two" }] }, { name: "NoteList" } as any);
        expect(editor.value).toEqual([{ Text: "Two" }]);
        editor.destroy();
    });

    it("tracks the dirty flag", () => {
        const editor = new NotesEditor({});
        editor["set_isDirty"](true);
        expect(editor["get_isDirty"]()).toBe(true);
        editor.destroy();
    });

    it("opens a note dialog from the toolbar add button", async () => {
        const editor = new NotesEditor({});
        const addClick = vi.spyOn(editor as any, "addClick").mockImplementation(() => { });
        await vi.waitFor(() => expect(editor.element.findFirst(".add-button").length).toBeTruthy());
        editor.element.findFirst(".add-button").click();
        expect(addClick).toHaveBeenCalled();
        editor.destroy();
    });

    it("adds a new note through the note dialog", () => {
        let captured: any;
        vi.spyOn(Object.getPrototypeOf(NoteDialog.prototype), "dialogOpen").mockImplementation(function (this: any) { captured = this; });
        const editor = new NotesEditor({});
        const changed = vi.fn();
        editor.onChange = changed;
        editor["addClick"]();
        captured.text = "A new note";
        captured.okClick();
        expect(editor.value.length).toBe(1);
        expect(editor.value[0].Text).toBe("A new note");
        expect(changed).toHaveBeenCalled();
        editor.destroy();
    });

    it("ignores empty notes on add", () => {
        let captured: any;
        vi.spyOn(Object.getPrototypeOf(NoteDialog.prototype), "dialogOpen").mockImplementation(function (this: any) { captured = this; });
        const editor = new NotesEditor({});
        editor["addClick"]();
        captured.text = "   ";
        captured.okClick();
        expect(editor.value).toBeUndefined();
        editor.destroy();
    });

    it("edits an existing note through the note dialog", () => {
        let captured: any;
        vi.spyOn(Object.getPrototypeOf(NoteDialog.prototype), "dialogOpen").mockImplementation(function (this: any) { captured = this; });
        const editor = new NotesEditor({});
        editor.value = [{ Text: "Old" }];
        editor["editClick"]({ preventDefault: vi.fn(), target: { dataset: { index: "0" } } });
        captured.text = "Updated";
        captured.okClick();
        expect(editor.value[0].Text).toBe("Updated");
        editor.destroy();
    });

    it("ignores empty notes on edit", () => {
        let captured: any;
        vi.spyOn(Object.getPrototypeOf(NoteDialog.prototype), "dialogOpen").mockImplementation(function (this: any) { captured = this; });
        const editor = new NotesEditor({});
        editor.value = [{ Text: "Old" }];
        editor["editClick"]({ preventDefault: vi.fn(), target: { dataset: { index: "0" } } });
        captured.text = "   ";
        captured.okClick();
        expect(editor.value[0].Text).toBe("Old");
        editor.destroy();
    });

    it("deletes a note after confirmation", () => {
        vi.spyOn(corelib, "confirmDialog").mockImplementation((_msg: any, onYes: any) => { onYes(); return null as any; });
        const editor = new NotesEditor({});
        editor.value = [{ Text: "One" }, { Text: "Two" }];
        editor.deleteClick({ preventDefault: vi.fn(), target: { dataset: { index: "0" } } } as any);
        expect(editor.value.length).toBe(1);
        expect(editor.value[0].Text).toBe("Two");
        editor.destroy();
    });
});
