import { interfaceTypeInfo, registerType } from "../base";

export class IValidateRequired {
    static typeInfo = interfaceTypeInfo("Serenity.IValidateRequired"); static { registerType(this); }
}

export interface IValidateRequired {
    get_required(): boolean;
    set_required(value: boolean): void;
}
