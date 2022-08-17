import { Decorators } from "../decorators";

@Decorators.registerInterface()
export class IStringValue {
}

export interface IStringValue {
    get_value(): string;
    set_value(value: string): void;
}