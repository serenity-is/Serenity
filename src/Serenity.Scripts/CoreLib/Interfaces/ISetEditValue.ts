import { registerInterface } from "../Decorators";
import { PropertyItem } from "../Services/PropertyItem";

@registerInterface()
export class ISetEditValue {
}

export interface ISetEditValue {
    setEditValue(source: any, property: PropertyItem): void;
}