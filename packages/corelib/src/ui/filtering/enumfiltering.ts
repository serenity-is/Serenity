import { nsSerenity } from "../../base";
import { EnumEditor } from "../editors/enumeditor";
import { BaseEditorFiltering } from "./baseeditorfiltering";
import { FilterOperators } from "./filteroperator";

/**
 * Filtering handler for enum fields using an enum editor.
 */
export class EnumFiltering extends BaseEditorFiltering<EnumEditor> {
    static override[Symbol.typeInfo] = this.registerClass(nsSerenity);

    /**
     * Creates an enum filtering handler.
     */
    constructor() {
        super(EnumEditor);
    }

    /**
     * Returns the operators supported by this filtering handler.
     * @returns The operators.
     */
    getOperators() {
        var op = [{ key: FilterOperators.EQ }, { key: FilterOperators.NE }];
        return this.appendNullableOperators(op);
    }

    /**
     * Returns the display text of the current editor value.
     * @returns The editor text.
     */
    override getEditorText(): string {
        if (this.useEditor()) {
            return this.editor.text;
        }

        return super.getEditorText();
    }
}