import { interfaceTypeInfo, nsSerenity, registerType } from "../base";

export class IDoubleValue {
    static [Symbol.typeInfo] = interfaceTypeInfo(nsSerenity); static { registerType(this); }
}

export interface IDoubleValue {
    get_value(): any;
    set_value(value: any): void;
}