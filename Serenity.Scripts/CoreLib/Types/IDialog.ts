namespace Serenity {

    @Decorators.registerInterface('Serenity.IDialog')
    export class IDialog {
    }

    export interface IDialog {
        dialogOpen(asPanel?: boolean): void;
    }

}