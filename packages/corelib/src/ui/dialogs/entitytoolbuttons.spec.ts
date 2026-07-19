import { describe, it, expect } from "vitest";
import { EntityDialogTexts } from "../../base";
import {
    saveAndCloseToolButton,
    applyChangesToolButton,
    deleteToolButton,
    undeleteToolButton,
    editToolButton,
    localizationToolButton,
    cloneToolButton
} from "./entitytoolbuttons";

describe("saveAndCloseToolButton", () => {
    it("returns a ToolButton with save-and-close config", () => {
        const btn = saveAndCloseToolButton();
        expect(btn.title).toBe(EntityDialogTexts.SaveButton);
        expect(btn.action).toBe("save-and-close");
        expect(btn.cssClass).toBe("save-and-close-button");
        expect(btn.hotkey).toBe("alt+s");
        expect(btn.icon).toBeTruthy();
    });

    it("merges with provided options", () => {
        const btn = saveAndCloseToolButton({ title: "Custom Save", cssClass: "custom" });
        expect(btn.title).toBe("Custom Save");
        expect(btn.cssClass).toBe("custom");
        expect(btn.action).toBe("save-and-close");
    });
});

describe("applyChangesToolButton", () => {
    it("returns a ToolButton with apply-changes config", () => {
        const btn = applyChangesToolButton();
        expect(btn.title).toBe("");
        expect(btn.hint).toBe(EntityDialogTexts.ApplyChangesButton);
        expect(btn.action).toBe("apply-changes");
        expect(btn.cssClass).toBe("apply-changes-button");
        expect(btn.hotkey).toBe("alt+a");
        expect(btn.icon).toBeTruthy();
    });

    it("merges with provided options", () => {
        const btn = applyChangesToolButton({ title: "Custom Apply", cssClass: "custom" });
        expect(btn.title).toBe("Custom Apply");
        expect(btn.cssClass).toBe("custom");
        expect(btn.action).toBe("apply-changes");
    });
});

describe("deleteToolButton", () => {
    it("returns a ToolButton with delete config", () => {
        const btn = deleteToolButton();
        expect(btn.title).toBe(EntityDialogTexts.DeleteButton);
        expect(btn.action).toBe("delete");
        expect(btn.cssClass).toBe("delete-button");
        expect(btn.hotkey).toBe("alt+x");
    });

    it("merges with provided options", () => {
        const btn = deleteToolButton({ title: "Custom Delete", cssClass: "custom" });
        expect(btn.title).toBe("Custom Delete");
        expect(btn.cssClass).toBe("custom");
        expect(btn.action).toBe("delete");
    });
});

describe("undeleteToolButton", () => {
    it("returns a ToolButton with undo-delete config", () => {
        const btn = undeleteToolButton();
        expect(btn.title).toBe(EntityDialogTexts.UndeleteButton);
        expect(btn.action).toBe("undo-delete");
        expect(btn.cssClass).toBe("undo-delete-button");
    });

    it("merges with provided options", () => {
        const btn = undeleteToolButton({ title: "Custom Undelete" });
        expect(btn.title).toBe("Custom Undelete");
        expect(btn.action).toBe("undo-delete");
    });
});

describe("editToolButton", () => {
    it("returns a ToolButton with edit config", () => {
        const btn = editToolButton();
        expect(btn.title).toBe(EntityDialogTexts.EditButton);
        expect(btn.action).toBe("edit");
        expect(btn.cssClass).toBe("edit-button");
    });

    it("merges with provided options", () => {
        const btn = editToolButton({ title: "Custom Edit" });
        expect(btn.title).toBe("Custom Edit");
        expect(btn.action).toBe("edit");
    });
});

describe("localizationToolButton", () => {
    it("returns a ToolButton with localization config", () => {
        const btn = localizationToolButton();
        expect(btn.title).toBe(EntityDialogTexts.LocalizationButton);
        expect(btn.action).toBe("localization");
        expect(btn.cssClass).toBe("localization-button");
    });

    it("merges with provided options", () => {
        const btn = localizationToolButton({ title: "Custom Loc" });
        expect(btn.title).toBe("Custom Loc");
        expect(btn.action).toBe("localization");
    });
});

describe("cloneToolButton", () => {
    it("returns a ToolButton with clone config", () => {
        const btn = cloneToolButton();
        expect(btn.title).toBe(EntityDialogTexts.CloneButton);
        expect(btn.action).toBe("clone");
        expect(btn.cssClass).toBe("clone-button");
    });

    it("merges with provided options", () => {
        const btn = cloneToolButton({ title: "Custom Clone" });
        expect(btn.title).toBe("Custom Clone");
        expect(btn.action).toBe("clone");
    });
});
