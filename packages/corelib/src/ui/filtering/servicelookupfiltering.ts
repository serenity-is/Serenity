import { nsSerenity } from "../../base";
import { ServiceLookupEditor } from "../editors/servicelookupeditor";
import { BaseEditorFiltering } from "./baseeditorfiltering";
import { FilterOperator, FilterOperators } from "./filteroperator";

/**
 * Filtering handler for service lookup fields using a service lookup editor.
 */
export class ServiceLookupFiltering extends BaseEditorFiltering<ServiceLookupEditor> {
    static override[Symbol.typeInfo] = this.registerClass(nsSerenity);

    /**
     * Creates a service lookup filtering handler.
     */
    constructor() {
        super(ServiceLookupEditor);
    }

    /**
     * Returns the operators supported by this filtering handler.
     * @returns The operators.
     */
    getOperators(): FilterOperator[] {
        var ops = [{ key: FilterOperators.EQ }, { key: FilterOperators.NE }, { key: FilterOperators.contains }, { key: FilterOperators.startsWith }]
        return this.appendNullableOperators(ops);
    }

    /**
     * Whether the current operator uses an editor.
     * @returns True when eq or ne.
     */
    protected override useEditor(): boolean {
        var op = this.get_operator().key;
        return op == FilterOperators.EQ || op == FilterOperators.NE;
    }

    /**
     * Whether to use the id field for the criteria.
     * @returns True when an editor is used.
     */
    protected override useIdField(): boolean {
        return this.useEditor();
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
