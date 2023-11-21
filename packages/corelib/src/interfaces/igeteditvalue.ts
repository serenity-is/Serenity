import { PropertyItem } from "@serenity-is/base";
import { Decorators } from "../decorators";

@Decorators.registerInterface("Serenity.IGetEditValue")
export class IGetEditValue {
}

export interface IGetEditValue {
    getEditValue(property: PropertyItem, target: any): void;
}