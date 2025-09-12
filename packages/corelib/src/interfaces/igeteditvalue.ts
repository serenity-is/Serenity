import { interfaceTypeInfo, nsSerenity, PropertyItem, registerType } from "../base";

export class IGetEditValue {
    static typeInfo = interfaceTypeInfo(nsSerenity); static { registerType(this); }
}

export interface IGetEditValue {
    getEditValue(property: PropertyItem, target: any): void;
}