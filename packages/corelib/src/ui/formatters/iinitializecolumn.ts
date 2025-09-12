import { Column } from "@serenity-is/sleekgrid";
import { interfaceTypeInfo, registerType } from "../../base";

export interface IInitializeColumn {
    initializeColumn(column: Column): void;
}

export class IInitializeColumn {
    static typeInfo = interfaceTypeInfo("Serenity.IInitializeColumn"); static { registerType(this); }
}
