import { interfaceTypeInfo, nsSerenity, registerType } from "../base";

/**
 * Type token for editors that support a required-field flag.
 */
export abstract class IValidateRequired {
    static [Symbol.typeInfo] = interfaceTypeInfo(nsSerenity); static { registerType(this); }
}

export interface IValidateRequired {
    /** Gets whether a value is required. @returns True if required. */
    get_required(): boolean;
    /** Sets whether a value is required. @param value - True to require a value. */
    set_required(value: boolean): void;
}
