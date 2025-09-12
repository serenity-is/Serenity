import { interfaceTypeInfo, nsSerenity, registerType } from "../base";

export class IEditDialog {
    static typeInfo = interfaceTypeInfo(nsSerenity); static { registerType(this); }
}

export interface IEditDialog {
    load(entityOrId: any, done: () => void, fail?: (p1: any) => void): void;
}
