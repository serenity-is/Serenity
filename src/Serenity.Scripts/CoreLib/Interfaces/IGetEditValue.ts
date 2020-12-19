import { registerInterface } from "../Decorators";
import { PropertyItem } from "../Services/PropertyItem";

@registerInterface()
export class IGetEditValue {
}

export interface IGetEditValue {
    getEditValue(property: PropertyItem, target: any): void;
}