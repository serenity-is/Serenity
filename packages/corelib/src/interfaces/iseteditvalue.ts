import { interfaceTypeInfo, nsSerenity, PropertyItem, registerType } from "../base";

export abstract class ISetEditValue {
    static [Symbol.typeInfo] = interfaceTypeInfo(nsSerenity); static { registerType(this); }
}

export interface ISetEditValue {
    setEditValue(source: any, property: PropertyItem): void;
}