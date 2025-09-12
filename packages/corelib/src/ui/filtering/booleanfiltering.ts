import { BaseFiltering } from "./basefiltering";
import { FilterOperators } from "./filteroperator";

export class BooleanFiltering extends BaseFiltering {
    static override typeInfo = this.classTypeInfo("Serenity.BooleanFiltering");

    getOperators() {
        return this.appendNullableOperators([
            { key: FilterOperators.isTrue },
            { key: FilterOperators.isFalse }
        ]);
    }
}
