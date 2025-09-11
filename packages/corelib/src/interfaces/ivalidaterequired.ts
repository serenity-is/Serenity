import { interfaceTypeInfo, registerType } from "../base";

export interface IValidateRequired {
    get_required(): boolean;
    set_required(value: boolean): void;
}

export class IValidateRequired {
    static typeInfo = interfaceTypeInfo("Serenity.IValidateRequired"); static { registerType(this); }
}