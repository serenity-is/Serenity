import { interfaceTypeInfo, nsSerenity, registerType } from "../base";

/**
 * Type token for editors that expose a numeric (double) value.
 */
export abstract class IDoubleValue {
    static [Symbol.typeInfo] = interfaceTypeInfo(nsSerenity); static { registerType(this); }
}

export interface IDoubleValue {
    /** Gets the current numeric value. @returns Current value (number or null/undefined). */
    get_value(): any;
    /** Sets the numeric value. @param value - New value to assign. */
    set_value(value: any): void;
}