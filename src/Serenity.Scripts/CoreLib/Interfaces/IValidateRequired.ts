import { registerInterface } from "../Decorators";

export interface IValidateRequired {
    get_required(): boolean;
    set_required(value: boolean): void;
}

@registerInterface('Serenity.IValidateRequired')
export class IValidateRequired {
}