import { interfaceTypeInfo, nsSerenity, PropertyItem, registerType } from "../base";

/**
 * Type token for editors that can write their value into a target object.
 */
export abstract class IGetEditValue {
    static [Symbol.typeInfo] = interfaceTypeInfo(nsSerenity); static { registerType(this); }
}

export interface IGetEditValue {
    /**
     * Writes the editor value into the target object.
     * @param property - Property metadata for the field.
     * @param target - Object to populate.
     */
    getEditValue(property: PropertyItem, target: any): void;
}