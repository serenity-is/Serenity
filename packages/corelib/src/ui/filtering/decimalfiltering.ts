import { nsSerenity } from "../../base";
import { DecimalEditor } from "../editors/decimaleditor";
import { BaseEditorFiltering } from "./baseeditorfiltering";
import { FilterOperator } from "./filteroperator";

export class DecimalFiltering extends BaseEditorFiltering<DecimalEditor> {
    static [Symbol.typeInfo] = this.registerClass(nsSerenity);

    constructor() {
        super(DecimalEditor);
    }

    getOperators(): FilterOperator[] {
        return this.appendNullableOperators(
            this.appendComparisonOperators([]));
    }
}
