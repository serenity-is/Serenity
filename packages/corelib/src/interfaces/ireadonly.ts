import { interfaceTypeInfo, nsSerenity, registerType } from "../base";

/**
 * Type token for widgets that support read-only mode.
 */
export abstract class IReadOnly {
    static [Symbol.typeInfo] = interfaceTypeInfo(nsSerenity); static { registerType(this); }
}

export interface IReadOnly {
    /** Gets whether the widget is read-only. @returns True if read-only. */
    get_readOnly(): boolean;
    /** Sets read-only state. @param value - True to make read-only, false to make editable. */
    set_readOnly(value: boolean): void;
}
