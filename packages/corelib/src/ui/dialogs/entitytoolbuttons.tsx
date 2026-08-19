import { EntityDialogTexts, faIcon } from "../../base";
import { ToolButton } from "../widgets/toolbar";

/**
 * Identifies how a save operation was initiated.
 */
export type SaveInitiator = "save-and-close" | "apply-changes";

/**
 * Creates a toolbar button that saves the entity and closes the dialog.
 * @param opt - Optional overrides merged into the button definition.
 * @returns Tool button definition.
 */
export function saveAndCloseToolButton(opt?: ToolButton): ToolButton {
    return {
        title: EntityDialogTexts.SaveButton,
        action: 'save-and-close',
        cssClass: 'save-and-close-button',
        icon: faIcon("check-circle", "purple"),
        hotkey: 'alt+s',
        ...opt
    }
}

/**
 * Creates a toolbar button that saves the entity and keeps the dialog open.
 * @param opt - Optional overrides merged into the button definition.
 * @returns Tool button definition.
 */
export function applyChangesToolButton(opt?: ToolButton): ToolButton {
    return {
        title: '',
        hint: EntityDialogTexts.ApplyChangesButton,
        action: 'apply-changes',
        cssClass: 'apply-changes-button',
        icon: faIcon("clipboard-check", "purple"),
        hotkey: 'alt+a',
        ...opt
    }
}

/**
 * Creates a toolbar button that deletes the entity.
 * @param opt - Optional overrides merged into the button definition.
 * @returns Tool button definition.
 */
export function deleteToolButton(opt?: ToolButton): ToolButton {
    return {
        title: EntityDialogTexts.DeleteButton,
        action: "delete",
        cssClass: 'delete-button',
        icon: faIcon("trash-o", "danger"),
        hotkey: 'alt+x',
        ...opt
    }
}

/**
 * Creates a toolbar button that undeletes a soft-deleted entity.
 * @param opt - Optional overrides merged into the button definition.
 * @returns Tool button definition.
 */
export function undeleteToolButton(opt?: ToolButton): ToolButton {
    return {
        title: EntityDialogTexts.UndeleteButton,
        action: 'undo-delete',
        cssClass: 'undo-delete-button',
        ...opt
    }
}

/**
 * Creates a toolbar button that switches the dialog to edit mode.
 * @param opt - Optional overrides merged into the button definition.
 * @returns Tool button definition.
 */
export function editToolButton(opt?: ToolButton): ToolButton {
    return {
        title: EntityDialogTexts.EditButton,
        action: 'edit',
        cssClass: 'edit-button',
        icon: faIcon("edit"),
        ...opt
    }
}

/**
 * Creates a toolbar button that toggles the localization editor.
 * @param opt - Optional overrides merged into the button definition.
 * @returns Tool button definition.
 */
export function localizationToolButton(opt?: ToolButton): ToolButton {
    return {
        title: EntityDialogTexts.LocalizationButton,
        action: 'localization',
        cssClass: 'localization-button',
        ...opt
    }
}

/**
 * Creates a toolbar button that clones the current entity.
 * @param opt - Optional overrides merged into the button definition.
 * @returns Tool button definition.
 */
export function cloneToolButton(opt?: ToolButton): ToolButton {
    return {
        title: EntityDialogTexts.CloneButton,
        action: 'clone',
        cssClass: 'clone-button',
        icon: faIcon("clone"),
        ...opt
    }
}