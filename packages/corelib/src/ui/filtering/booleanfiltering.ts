import { nsSerenity } from "../../base";
import { BaseFiltering } from "./basefiltering";
import { FilterOperators } from "./filteroperator";

/**
 * Filtering handler for boolean fields, supporting is-true and is-false operators.
 */
export class BooleanFiltering extends BaseFiltering {
    static override[Symbol.typeInfo] = this.registerClass(nsSerenity);

    /**
     * Returns the operators supported by this filtering handler.
     * @returns The operators.
     */
    getOperators() {
        return this.appendNullableOperators([
            { key: FilterOperators.isTrue },
            { key: FilterOperators.isFalse }
        ]);
    }
}
