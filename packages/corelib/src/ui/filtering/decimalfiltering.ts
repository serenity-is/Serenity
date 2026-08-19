import { nsSerenity } from "../../base";
import { DecimalEditor } from "../editors/decimaleditor";
import { BaseEditorFiltering } from "./baseeditorfiltering";
import { FilterOperator } from "./filteroperator";

/**
 * Filtering handler for decimal fields using a decimal editor.
 */
export class DecimalFiltering extends BaseEditorFiltering<DecimalEditor> {
    static override[Symbol.typeInfo] = this.registerClass(nsSerenity);

    /**
     * Creates a decimal filtering handler.
     */
    constructor() {
        super(DecimalEditor);
    }

    /**
     * Returns the operators supported by this filtering handler.
     * @returns The operators.
     */
    getOperators(): FilterOperator[] {
        return this.appendNullableOperators(
            this.appendComparisonOperators([]));
    }
}
