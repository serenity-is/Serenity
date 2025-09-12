import { IntegerEditor } from "../editors/integereditor";
import { BaseEditorFiltering } from "./baseeditorfiltering";
import { FilterOperator } from "./filteroperator";

export class IntegerFiltering extends BaseEditorFiltering<IntegerEditor> {
    static override typeInfo = this.registerClass("Serenity.IntegerFiltering");

    constructor() {
        super(IntegerEditor);
    }

    getOperators(): FilterOperator[] {
        return this.appendNullableOperators(this.appendComparisonOperators([]));
    }
}