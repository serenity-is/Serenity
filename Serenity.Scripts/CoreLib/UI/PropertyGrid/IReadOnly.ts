namespace Serenity {

    export interface IReadOnly {
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
    }

    @Serenity.Decorators.registerInterface('Serenity.IReadOnly')
    export class IReadOnly {
    }
}