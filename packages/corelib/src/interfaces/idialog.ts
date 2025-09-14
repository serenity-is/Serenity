import { interfaceTypeInfo, nsSerenity, registerType } from "../base";

export class IDialog {
    static [Symbol.typeInfo] = interfaceTypeInfo(nsSerenity); static { registerType(this); }
}

export interface IDialog {
    dialogOpen(asPanel?: boolean): void;
}