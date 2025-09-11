import { interfaceTypeInfo, registerType } from "../base";

export class IDoubleValue {
    static typeInfo = interfaceTypeInfo("Serenity.IDoubleValue"); static { registerType(this); }
}

export interface IDoubleValue {
    get_value(): any;
    set_value(value: any): void;
}