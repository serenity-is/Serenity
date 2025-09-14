import { nsSerenity } from "../../base";
import { ServiceLookupEditor } from "../editors/servicelookupeditor";
import { BaseEditorFiltering } from "./baseeditorfiltering";
import { FilterOperator, FilterOperators } from "./filteroperator";

export class ServiceLookupFiltering extends BaseEditorFiltering<ServiceLookupEditor> {
    static [Symbol.typeInfo] = this.registerClass(nsSerenity);

    constructor() {
        super(ServiceLookupEditor);
    }

    getOperators(): FilterOperator[] {
        var ops = [{ key: FilterOperators.EQ }, { key: FilterOperators.NE }, { key: FilterOperators.contains }, { key: FilterOperators.startsWith }]
        return this.appendNullableOperators(ops);
    }

    protected useEditor(): boolean {
        var op = this.get_operator().key;
        return op == FilterOperators.EQ || op == FilterOperators.NE;
    }

    protected useIdField(): boolean {
        return this.useEditor();
    }

    getEditorText(): string {
        if (this.useEditor()) {
            return this.editor.text;
        }

        return super.getEditorText();
    }
}
