import { nsSerenity } from "../../base";
import { DateEditor } from "../editors/dateeditor";
import { BaseEditorFiltering } from "./baseeditorfiltering";
import { FilterOperator } from "./filteroperator";

/**
 * Filtering handler for date fields using a date editor.
 */
export class DateFiltering extends BaseEditorFiltering<DateEditor> {
    static override[Symbol.typeInfo] = this.registerClass(nsSerenity);

    /**
     * Creates a date filtering handler.
     */
    constructor() {
        super(DateEditor)
    }

    /**
     * Returns the operators supported by this filtering handler.
     * @returns The operators.
     */
    getOperators(): FilterOperator[] {
        return this.appendNullableOperators(this.appendComparisonOperators([]));
    }
}
