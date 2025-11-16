import { interfaceTypeInfo, nsSerenity, registerType } from "../base";

export abstract class IBooleanValue {
    static [Symbol.typeInfo] = interfaceTypeInfo(nsSerenity); static { registerType(this); }
}

export interface IBooleanValue {
    get_value(): boolean;
    set_value(value: boolean): void;
}
