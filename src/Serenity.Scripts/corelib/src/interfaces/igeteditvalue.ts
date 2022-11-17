import { Decorators } from "../decorators";
import { PropertyItem } from "@serenity-is/corelib/q";

@Decorators.registerInterface()
export class IGetEditValue {
}

export interface IGetEditValue {
    getEditValue(property: PropertyItem, target: any): void;
}