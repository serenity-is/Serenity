import { Decorators } from "../decorators";
import { PropertyItem } from "../q";

@Decorators.registerInterface()
export class ISetEditValue {
}

export interface ISetEditValue {
    setEditValue(source: any, property: PropertyItem): void;
}