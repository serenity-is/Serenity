import { interfaceTypeInfo, nsSerenity, registerType } from "../base";

/**
 * Type token for dialog widgets. Implemented by dialogs that can be opened as modal or panel.
 */
export abstract class IDialog {
    static [Symbol.typeInfo] = interfaceTypeInfo(nsSerenity); static { registerType(this); }
}

export interface IDialog {
    /**
     * Opens the dialog.
     * @param asPanel - When true, opens as an in-page panel instead of a modal dialog.
     */
    dialogOpen(asPanel?: boolean): void;
}