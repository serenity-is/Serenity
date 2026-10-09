import { nsSerenity } from "../../base";
import { Int64Editor } from "../editors/int64editor";
import { BaseEditorFiltering } from "./baseeditorfiltering";
import { FilterOperator } from "./filteroperator";

/**
 * Filtering handler for Int64 (64-bit integer) fields using an Int64 editor.
 */
export class Int64Filtering extends BaseEditorFiltering<Int64Editor> {
    static override[Symbol.typeInfo] = this.registerClass(nsSerenity);

    /**
     * Creates an Int64 filtering handler.
     */
    constructor() {
        super(Int64Editor);
    }

    /**
     * Returns the operators supported by this filtering handler.
     * @returns The operators.
     */
    getOperators(): FilterOperator[] {
        return this.appendNullableOperators(this.appendComparisonOperators([]));
    }
}
