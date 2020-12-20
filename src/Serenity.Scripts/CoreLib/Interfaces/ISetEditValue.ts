import { registerInterface } from "../Decorators";

@registerInterface()
export class ISetEditValue {
}

export interface ISetEditValue {
    setEditValue(source: any, property: Serenity.PropertyItem): void;
}