import { nsSerenity } from "../../base";
import { LookupEditor } from "../editors/lookupeditor";
import { BaseEditorFiltering } from "./baseeditorfiltering";
import { FilterOperator, FilterOperators } from "./filteroperator";

export class LookupFiltering extends BaseEditorFiltering<LookupEditor> {
    static override [Symbol.typeInfo] = this.registerClass(nsSerenity);

    constructor() {
        super(LookupEditor);
    }

    getOperators(): FilterOperator[] {
        var ops = [{ key: FilterOperators.EQ }, { key: FilterOperators.NE }, { key: FilterOperators.contains }, { key: FilterOperators.startsWith }]
        return this.appendNullableOperators(ops);
    }

    protected override useEditor(): boolean {
        var op = this.get_operator().key;
        return op == FilterOperators.EQ || op == FilterOperators.NE;
    }

    protected override useIdField(): boolean {
        return this.useEditor();
    }

    override getEditorText(): string {
        if (this.useEditor()) {
            return this.editor.text;
        }

        return super.getEditorText();
    }
}
