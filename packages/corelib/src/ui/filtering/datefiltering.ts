import { DateEditor } from "../editors/dateeditor";
import { BaseEditorFiltering } from "./baseeditorfiltering";
import { FilterOperator } from "./filteroperator";

export class DateFiltering extends BaseEditorFiltering<DateEditor> {
    static override typeInfo = this.classTypeInfo("Serenity.DateFiltering");

    constructor() {
        super(DateEditor)
    }

    getOperators(): FilterOperator[] {
        return this.appendNullableOperators(this.appendComparisonOperators([]));
    }
}
