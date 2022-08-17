import { Decorators } from "../decorators";
import { PropertyItem } from "../q";

@Decorators.registerInterface()
export class IGetEditValue {
}

export interface IGetEditValue {
    getEditValue(property: PropertyItem, target: any): void;
}