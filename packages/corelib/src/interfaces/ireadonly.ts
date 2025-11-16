import { interfaceTypeInfo, nsSerenity, registerType } from "../base";

export abstract class IReadOnly {
    static [Symbol.typeInfo] = interfaceTypeInfo(nsSerenity); static { registerType(this); }
}

export interface IReadOnly {
    get_readOnly(): boolean;
    set_readOnly(value: boolean): void;
}
