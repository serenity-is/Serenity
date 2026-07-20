import { interfaceTypeInfo, nsSerenity, registerType, RetrieveResponse } from "../base";

export abstract class IEditDialog {
    static [Symbol.typeInfo] = interfaceTypeInfo(nsSerenity); static { registerType(this); }
}

export interface IEditDialog {
    load(entityOrId: any, done: () => void, fail?: (p1: any) => void): PromiseLike<RetrieveResponse<any>>;
}
