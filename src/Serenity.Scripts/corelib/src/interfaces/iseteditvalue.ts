import { Decorators } from "../decorators";
import { PropertyItem } from "@serenity-is/corelib/q";

@Decorators.registerInterface()
export class ISetEditValue {
}

export interface ISetEditValue {
    setEditValue(source: any, property: PropertyItem): void;
}