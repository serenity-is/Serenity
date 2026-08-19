import { interfaceTypeInfo, nsSerenity, registerType } from "../base";

/**
 * Type token for widgets/editors that expose a boolean value.
 * Implement {@link IBooleanValue.get_value} / {@link IBooleanValue.set_value} and register with the interface type system.
 */
export abstract class IBooleanValue {
    static [Symbol.typeInfo] = interfaceTypeInfo(nsSerenity); static { registerType(this); }
}

export interface IBooleanValue {
    /** Gets the current boolean value. @returns Current value. */
    get_value(): boolean;
    /** Sets the boolean value. @param value - New value to assign. */
    set_value(value: boolean): void;
}
