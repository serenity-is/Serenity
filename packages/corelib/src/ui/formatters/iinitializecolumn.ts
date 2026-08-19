import { Column } from "@serenity-is/sleekgrid";
import { interfaceTypeInfo, nsSerenity, registerType } from "../../base";

/**
 * Type token for formatters/editors that need to modify their grid column at setup time
 * (e.g. to declare `referencedFields`).
 */
export abstract class IInitializeColumn {
    static [Symbol.typeInfo] = interfaceTypeInfo(nsSerenity); static { registerType(this); }
}

export interface IInitializeColumn {
    /**
     * Called during column construction to allow the formatter to adjust column metadata.
     * @param column - Mutable column definition to initialize.
     */
    initializeColumn(column: Column): void;
}
