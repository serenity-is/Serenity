import { PropertyItem } from "../base";
import { Decorators } from "../types/decorators";

@Decorators.registerInterface("Serenity.IGetEditValue")
export class IGetEditValue {
}

export interface IGetEditValue {
    getEditValue(property: PropertyItem, target: any): void;
}