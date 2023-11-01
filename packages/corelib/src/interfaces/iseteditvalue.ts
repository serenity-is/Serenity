import { Decorators } from "../decorators";
import { PropertyItem } from "../q";

@Decorators.registerInterface("Serenity.ISetEditValue")
export class ISetEditValue {
}

export interface ISetEditValue {
    setEditValue(source: any, property: PropertyItem): void;
}