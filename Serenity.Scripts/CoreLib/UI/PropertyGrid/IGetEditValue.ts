namespace Serenity {

    @Serenity.Decorators.registerInterface()
    export class IGetEditValue {
    }

    export interface IGetEditValue {
        getEditValue(property: PropertyItem, target: any): void;
    }
}