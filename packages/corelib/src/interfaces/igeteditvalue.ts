import { interfaceTypeInfo, PropertyItem, registerType } from "../base";

export class IGetEditValue {
    static typeInfo = interfaceTypeInfo("Serenity.IGetEditValue"); static { registerType(this); }
}

export interface IGetEditValue {
    getEditValue(property: PropertyItem, target: any): void;
}