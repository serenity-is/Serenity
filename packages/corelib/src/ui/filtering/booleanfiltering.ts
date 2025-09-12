import { nsSerenity } from "../../base";
import { BaseFiltering } from "./basefiltering";
import { FilterOperators } from "./filteroperator";

export class BooleanFiltering extends BaseFiltering {
    static override typeInfo = this.registerClass(nsSerenity);

    getOperators() {
        return this.appendNullableOperators([
            { key: FilterOperators.isTrue },
            { key: FilterOperators.isFalse }
        ]);
    }
}
