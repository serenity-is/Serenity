import { interfaceTypeInfo, registerType } from "../base";

export interface IReadOnly {
    get_readOnly(): boolean;
    set_readOnly(value: boolean): void;
}

export class IReadOnly {
    static typeInfo = interfaceTypeInfo("Serenity.IReadOnly"); static { registerType(this); }
}
