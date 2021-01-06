import { Decorators } from "../Decorators";

@Decorators.registerInterface()
export class IGetEditValue {
}

export interface IGetEditValue {
    getEditValue(property: Serenity.PropertyItem, target: any): void;
}