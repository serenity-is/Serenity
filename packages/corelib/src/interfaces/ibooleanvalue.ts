import { interfaceTypeInfo, registerType } from "../base";

export class IBooleanValue {
    static typeInfo = interfaceTypeInfo("Serenity.IBooleanValue"); static { registerType(this); }
}

registerType(IBooleanValue);

export interface IBooleanValue {
    get_value(): boolean;
    set_value(value: boolean): void;
}
