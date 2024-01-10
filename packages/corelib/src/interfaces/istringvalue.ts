import { Decorators } from "../types/decorators";

@Decorators.registerInterface("Serenity.IStringValue")
export class IStringValue {
}

export interface IStringValue {
    get_value(): string;
    set_value(value: string): void;
}