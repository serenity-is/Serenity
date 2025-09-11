import { interfaceTypeInfo, PropertyItem, registerType } from "../base";

export class ISetEditValue {
    static typeInfo = interfaceTypeInfo("Serenity.ISetEditValue"); static { registerType(this); }
}

export interface ISetEditValue {
    setEditValue(source: any, property: PropertyItem): void;
}