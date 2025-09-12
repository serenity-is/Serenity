import { nsSerenity } from "../../base";
import { BaseFiltering } from "./basefiltering";
import { FilterOperator, FilterOperators } from "./filteroperator";

export class StringFiltering extends BaseFiltering {
    static override typeInfo = this.registerClass(nsSerenity);

    getOperators(): FilterOperator[] {
        var ops = [
            { key: FilterOperators.contains },
            { key: FilterOperators.startsWith },
            { key: FilterOperators.EQ },
            { key: FilterOperators.NE }
        ];
        return this.appendNullableOperators(ops);
    }

    validateEditorValue(value: string) {
        if (value.length === 0) {
            return value;
        }

        return super.validateEditorValue(value);
    }
}
