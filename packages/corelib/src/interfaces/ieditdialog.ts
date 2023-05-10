import { Decorators } from "../decorators";

@Decorators.registerInterface('Serenity.IEditDialog')
export class IEditDialog {
}

export interface IEditDialog {
    load(entityOrId: any, done: () => void, fail?: (p1: any) => void): void;
}
