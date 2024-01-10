import { Decorators } from "../types/decorators";

@Decorators.registerInterface("Serenity.IDoubleValue")
export class IDoubleValue {
}

export interface IDoubleValue {
    get_value(): any;
    set_value(value: any): void;
}