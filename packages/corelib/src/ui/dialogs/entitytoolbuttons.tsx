import { EntityDialogTexts, faIcon } from "../../base";
import { ToolButton } from "../widgets/toolbar";

export type SaveInitiator = "save-and-close" | "apply-changes";

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

export function undeleteToolButton(opt?: ToolButton): ToolButton {
    return {
        title: EntityDialogTexts.UndeleteButton,
        action: 'undo-delete',
        cssClass: 'undo-delete-button',
        ...opt
    }
}

export function editToolButton(opt?: ToolButton): ToolButton {
    return {
        title: EntityDialogTexts.EditButton,
        action: 'edit',
        cssClass: 'edit-button',
        icon: faIcon("edit"),
        ...opt
    }
}

export function localizationToolButton(opt?: ToolButton): ToolButton {
    return {
        title: EntityDialogTexts.LocalizationButton,
        action: 'localization',
        cssClass: 'localization-button',
        ...opt
    }
}

export function cloneToolButton(opt?: ToolButton): ToolButton {
    return {
        title: EntityDialogTexts.CloneButton,
        action: 'clone',
        cssClass: 'clone-button',
        icon: faIcon("clone"),
        ...opt
    }
}