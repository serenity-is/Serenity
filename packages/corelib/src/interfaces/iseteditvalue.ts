import { PropertyItem } from "../base";
import { Decorators } from "../types/decorators";

@Decorators.registerInterface("Serenity.ISetEditValue")
export class ISetEditValue {
}

export interface ISetEditValue {
    setEditValue(source: any, property: PropertyItem): void;
}