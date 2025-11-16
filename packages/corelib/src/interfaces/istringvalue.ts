import { interfaceTypeInfo, nsSerenity, registerType } from "../base";

export abstract class IStringValue {
    static [Symbol.typeInfo] = interfaceTypeInfo(nsSerenity); static { registerType(this); }
}

export interface IStringValue {
    get_value(): string;
    set_value(value: string): void;
}