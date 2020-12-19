import { registerInterface } from "../Decorators";

@registerInterface()
export class IBooleanValue {
}

export interface IBooleanValue {
    get_value(): boolean;
    set_value(value: boolean): void;
}
