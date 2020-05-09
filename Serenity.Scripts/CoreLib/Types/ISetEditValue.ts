namespace Serenity {

    @Serenity.Decorators.registerInterface()
    export class ISetEditValue {
    }

    export interface ISetEditValue {
        setEditValue(source: any, property: PropertyItem): void;
    }
}