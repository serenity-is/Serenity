import { registerInterface } from "../Decorators";

export interface IReadOnly {
    get_readOnly(): boolean;
    set_readOnly(value: boolean): void;
}

@registerInterface('Serenity.IReadOnly')
export class IReadOnly {
}
