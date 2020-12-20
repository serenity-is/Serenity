import { registerInterface } from "../Decorators";

@registerInterface()
export class IGetEditValue {
}

export interface IGetEditValue {
    getEditValue(property: Serenity.PropertyItem, target: any): void;
}