import { interfaceTypeInfo, nsSerenity, registerType } from "../base";

export class IBooleanValue {
    static typeInfo = interfaceTypeInfo(nsSerenity); static { registerType(this); }
}

export interface IBooleanValue {
    get_value(): boolean;
    set_value(value: boolean): void;
}
