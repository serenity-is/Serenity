import { nsSerenity } from "../../base";
import { EnumEditor } from "../editors/enumeditor";
import { BaseEditorFiltering } from "./baseeditorfiltering";
import { FilterOperators } from "./filteroperator";

export class EnumFiltering extends BaseEditorFiltering<EnumEditor> {
    static override[Symbol.typeInfo] = this.registerClass(nsSerenity);

    constructor() {
        super(EnumEditor);
    }

    getOperators() {
        var op = [{ key: FilterOperators.EQ }, { key: FilterOperators.NE }];
        return this.appendNullableOperators(op);
    }

    override getEditorText(): string {
        if (this.useEditor()) {
            return this.editor.text;
        }

        return super.getEditorText();
    }
}