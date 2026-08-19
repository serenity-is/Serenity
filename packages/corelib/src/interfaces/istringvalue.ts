import { interfaceTypeInfo, nsSerenity, registerType } from "../base";

/**
 * Type token for editors that expose a string value.
 */
export abstract class IStringValue {
    static [Symbol.typeInfo] = interfaceTypeInfo(nsSerenity); static { registerType(this); }
}

export interface IStringValue {
    /** Gets the current string value. @returns Current value. */
    get_value(): string;
    /** Sets the string value. @param value - New value to assign. */
    set_value(value: string): void;
}