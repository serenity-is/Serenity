import { mockFetch, unmockFetch } from "test-utils";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { GridEditorDialog } from "../../Modules/GridEditor/GridEditorDialog";

class TestGridEditorDialog extends GridEditorDialog<any> {
}

class RowDefinitionDialog extends GridEditorDialog<any> {
    override getRowDefinition() { return { idProperty: "theRowId" } as any; }
}

beforeEach(() => {
    mockFetch({ "*": () => ({}) });
});

afterEach(() => {
    unmockFetch();
    vi.restoreAllMocks();
});

describe("GridEditorDialog", () => {
    it("returns __id by default", () => {
        const dialog = new TestGridEditorDialog({});
        expect(dialog["getIdProperty"]()).toBe("__id");
        dialog.destroy();
    });

    it("returns the id property from getRowDefinition", () => {
        const dialog = new RowDefinitionDialog({});
        expect(dialog["getIdProperty"]()).toBe("theRowId");
        dialog.destroy();
    });

    it("clears save and delete handlers on destroy", () => {
        const dialog = new TestGridEditorDialog({});
        dialog.onSave = vi.fn() as any;
        dialog.onDelete = vi.fn() as any;
        dialog.destroy();
        expect(dialog.onSave).toBeNull();
        expect(dialog.onDelete).toBeNull();
    });

    it("hides the apply changes button in updateInterface", () => {
        const dialog = new TestGridEditorDialog({});
        const button = dialog["applyChangesButton"];
        expect(button).toBeTruthy();
        const hideSpy = vi.spyOn(button, "hide");
        dialog["updateInterface"]();
        expect(hideSpy).toHaveBeenCalled();
        dialog.destroy();
    });

    it("saveHandler delegates to onSave", async () => {
        const dialog = new TestGridEditorDialog({});
        const response = { EntityId: 1 };
        dialog.onSave = vi.fn(() => Promise.resolve(response)) as any;
        const result = dialog["saveHandler"]({} as any, () => { }, "save" as any);
        expect(dialog.onSave).toHaveBeenCalled();
        await expect(result).resolves.toBe(response);
        dialog.destroy();
    });

    it("deleteHandler delegates to onDelete", async () => {
        const dialog = new TestGridEditorDialog({});
        const response = {};
        dialog.onDelete = vi.fn(() => Promise.resolve(response)) as any;
        const result = dialog["deleteHandler"]({} as any, () => { });
        expect(dialog.onDelete).toHaveBeenCalled();
        await expect(result).resolves.toBe(response);
        dialog.destroy();
    });

    it("saveHandler and deleteHandler return undefined without handlers", () => {
        const dialog = new TestGridEditorDialog({});
        expect(dialog["saveHandler"]({} as any, () => { }, "save" as any)).toBeUndefined();
        expect(dialog["deleteHandler"]({} as any, () => { })).toBeUndefined();
        dialog.destroy();
    });
});
