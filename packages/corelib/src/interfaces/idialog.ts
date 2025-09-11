import { interfaceTypeInfo, registerType } from "../base";

export class IDialog {
    static typeInfo = interfaceTypeInfo("Serenity.IDialog"); static { registerType(this); }
}

export interface IDialog {
    dialogOpen(asPanel?: boolean): void;
}