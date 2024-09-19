import { Decorators } from "../types/decorators";

@Decorators.registerInterface("Serenity.IBooleanValue")
export class IBooleanValue {
}

export interface IBooleanValue {
    get_value(): boolean;
    set_value(value: boolean): void;
}
