import { DecimalEditor } from "../editors/decimaleditor";
import { BaseEditorFiltering } from "./baseeditorfiltering";
import { FilterOperator } from "./filteroperator";

export class DecimalFiltering extends BaseEditorFiltering<DecimalEditor> {
    static override typeInfo = this.registerClass("Serenity.DecimalFiltering");

    constructor() {
        super(DecimalEditor);
    }

    getOperators(): FilterOperator[] {
        return this.appendNullableOperators(
            this.appendComparisonOperators([]));
    }
}
