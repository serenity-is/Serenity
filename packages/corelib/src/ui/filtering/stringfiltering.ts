import { nsSerenity } from "../../base";
import { BaseFiltering } from "./basefiltering";
import { FilterOperator, FilterOperators } from "./filteroperator";

/**
 * Filtering handler for string fields.
 */
export class StringFiltering extends BaseFiltering {
    static override[Symbol.typeInfo] = this.registerClass(nsSerenity);

    /**
     * Returns the operators supported by this filtering handler.
     * @returns The operators.
     */
    getOperators(): FilterOperator[] {
        var ops = [
            { key: FilterOperators.contains },
            { key: FilterOperators.startsWith },
            { key: FilterOperators.EQ },
            { key: FilterOperators.NE }
        ];
        return this.appendNullableOperators(ops);
    }

    /**
     * Validates the editor value, allowing empty values.
     * @param value - The value to validate.
     * @returns The validated value.
     */
    override validateEditorValue(value: string) {
        if (value.length === 0) {
            return value;
        }

        return super.validateEditorValue(value);
    }
}
