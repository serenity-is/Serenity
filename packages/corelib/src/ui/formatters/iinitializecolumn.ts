import { Column } from "@serenity-is/sleekgrid";
import { interfaceTypeInfo, nsSerenity, registerType } from "../../base";

export abstract class IInitializeColumn {
    static [Symbol.typeInfo] = interfaceTypeInfo(nsSerenity); static { registerType(this); }
}

export interface IInitializeColumn {
    initializeColumn(column: Column): void;
}
