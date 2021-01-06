import { Decorators } from "../Decorators";

@Decorators.registerInterface()
export class IDoubleValue {
}

export interface IDoubleValue {
    get_value(): any;
    set_value(value: any): void;
}