import { interfaceTypeInfo, registerType } from "../base";

export class IStringValue {
    static typeInfo = interfaceTypeInfo("Serenity.IStringValue"); static { registerType(this); }
}

export interface IStringValue {
    get_value(): string;
    set_value(value: string): void;
}