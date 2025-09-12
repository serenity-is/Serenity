import { interfaceTypeInfo, nsSerenity, registerType } from "../base";

export class IReadOnly {
    static typeInfo = interfaceTypeInfo(nsSerenity); static { registerType(this); }
}

export interface IReadOnly {
    get_readOnly(): boolean;
    set_readOnly(value: boolean): void;
}
