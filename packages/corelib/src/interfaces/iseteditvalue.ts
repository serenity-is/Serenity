import { interfaceTypeInfo, nsSerenity, PropertyItem, registerType } from "../base";

/**
 * Type token for editors that can be populated from a source object.
 */
export abstract class ISetEditValue {
    static [Symbol.typeInfo] = interfaceTypeInfo(nsSerenity); static { registerType(this); }
}

export interface ISetEditValue {
    /**
     * Populates the editor from a source object.
     * @param source - Object containing property values.
     * @param property - Property metadata for the field.
     */
    setEditValue(source: any, property: PropertyItem): void;
}