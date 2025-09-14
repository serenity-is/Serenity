import { Column } from "@serenity-is/sleekgrid";
import { interfaceTypeInfo, nsSerenity, registerType } from "../../base";

export interface IInitializeColumn {
    initializeColumn(column: Column): void;
}

export class IInitializeColumn {
    static [Symbol.typeInfo] = interfaceTypeInfo(nsSerenity); static { registerType(this); }
}
