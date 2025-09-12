import { interfaceTypeInfo, nsSerenity, registerType } from "../base";

export class IStringValue {
    static typeInfo = interfaceTypeInfo(nsSerenity); static { registerType(this); }
}

export interface IStringValue {
    get_value(): string;
    set_value(value: string): void;
}