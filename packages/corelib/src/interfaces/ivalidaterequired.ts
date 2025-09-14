import { interfaceTypeInfo, nsSerenity, registerType } from "../base";

export class IValidateRequired {
    static [Symbol.typeInfo] = interfaceTypeInfo(nsSerenity); static { registerType(this); }
}

export interface IValidateRequired {
    get_required(): boolean;
    set_required(value: boolean): void;
}
