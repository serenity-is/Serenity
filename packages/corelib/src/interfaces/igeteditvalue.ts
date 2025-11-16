import { interfaceTypeInfo, nsSerenity, PropertyItem, registerType } from "../base";

export abstract class IGetEditValue {
    static [Symbol.typeInfo] = interfaceTypeInfo(nsSerenity); static { registerType(this); }
}

export interface IGetEditValue {
    getEditValue(property: PropertyItem, target: any): void;
}