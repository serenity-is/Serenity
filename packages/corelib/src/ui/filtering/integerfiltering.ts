import { nsSerenity } from "../../base";
import { IntegerEditor } from "../editors/integereditor";
import { BaseEditorFiltering } from "./baseeditorfiltering";
import { FilterOperator } from "./filteroperator";

/**
 * Filtering handler for integer fields using an integer editor.
 */
export class IntegerFiltering extends BaseEditorFiltering<IntegerEditor> {
    static override[Symbol.typeInfo] = this.registerClass(nsSerenity);

    /**
     * Creates an integer filtering handler.
     */
    constructor() {
        super(IntegerEditor);
    }

    /**
     * Returns the operators supported by this filtering handler.
     * @returns The operators.
     */
    getOperators(): FilterOperator[] {
        return this.appendNullableOperators(this.appendComparisonOperators([]));
    }
}