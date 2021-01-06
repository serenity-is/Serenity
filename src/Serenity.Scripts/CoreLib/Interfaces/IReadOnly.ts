import { Decorators } from "../Decorators";

export interface IReadOnly {
    get_readOnly(): boolean;
    set_readOnly(value: boolean): void;
}

@Decorators.registerInterface('Serenity.IReadOnly')
export class IReadOnly {
}
