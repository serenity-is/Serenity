import { Decorators } from "../types/decorators";

export interface IValidateRequired {
    get_required(): boolean;
    set_required(value: boolean): void;
}

@Decorators.registerInterface('Serenity.IValidateRequired')
export class IValidateRequired {
}