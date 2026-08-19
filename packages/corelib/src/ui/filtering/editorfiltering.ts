import { nsSerenity } from "../../base";
import { EditorTypeRegistry } from "../../types/editortyperegistry";
import { QuickFilter } from "../datagrid/quickfilter";
import { Widget } from "../widgets/widget";
import { BaseEditorFiltering } from "./baseeditorfiltering";
import { FilterOperator, FilterOperators } from "./filteroperator";

/**
 * A filtering handler that uses an editor type resolved from the editor type registry.
 */
export class EditorFiltering extends BaseEditorFiltering<Widget<any>> {
    static override[Symbol.typeInfo] = this.registerClass(nsSerenity);

    /**
     * Creates an editor filtering handler.
     * @param props - Options including the editor type and operator flags.
     */
    constructor(public readonly props: { editorType?: string, useRelative?: boolean, useLike?: boolean } = {}) {
        super(Widget);
        this.props ??= {};
    }

    /** The editor type key. */
    get editorType() { return this.props.editorType }
    /** Sets the editor type key. */
    set editorType(value) { this.props.editorType = value }

    /** Whether relative comparison operators are used. */
    get useRelative() { return this.props.useRelative }
    /** Sets whether relative comparison operators are used. */
    set useRelative(value) { this.props.useRelative = value }

    /** Whether like operators (contains, startsWith) are used. */
    get useLike() { return this.props.useLike }
    /** Sets whether like operators are used. */
    set useLike(value) { this.props.useLike = value }

    /**
     * Returns the operators supported by this filtering handler.
     * @returns The operators.
     */
    getOperators(): FilterOperator[] {
        var list = [];

        list.push({ key: FilterOperators.EQ });
        list.push({ key: FilterOperators.NE });

        if (this.useRelative) {
            list.push({ key: FilterOperators.LT });
            list.push({ key: FilterOperators.LE });
            list.push({ key: FilterOperators.GT });
            list.push({ key: FilterOperators.GE });
        }

        if (this.useLike) {
            list.push({ key: FilterOperators.contains });
            list.push({ key: FilterOperators.startsWith });
        }

        this.appendNullableOperators(list);

        return list;
    }

    /**
     * Whether the current operator uses an editor.
     * @returns True when an editor is used.
     */
    protected override useEditor() {
        var op = this.get_operator().key;

        return op === FilterOperators.EQ ||
            op === FilterOperators.NE ||
            (this.useRelative && (
                op === FilterOperators.LT ||
                op === FilterOperators.LE ||
                op === FilterOperators.GT ||
                op === FilterOperators.GE));
    }

    /**
     * Creates the editor for the current operator.
     */
    override createEditor() {
        if (this.useEditor()) {
            var editorType = EditorTypeRegistry.get(this.editorType ?? 'String') as typeof Widget<{}>;

            this.editor = new editorType({
                element: el => this.get_container().append(el),
                ...this.getEditorOptions()
            }).init?.();

            return;
        }

        super.createEditor();
    }

    /**
     * Whether to use the id field for the criteria.
     * @returns True when an editor is used.
     */
    protected override useIdField(): boolean {
        return this.useEditor();
    }

    /**
     * Initializes a quick filter using the editor type.
     * @param filter - The quick filter to initialize.
     */
    override initQuickFilter(filter: QuickFilter<Widget<any>, any>) {
        super.initQuickFilter(filter);

        filter.type = EditorTypeRegistry.get(this.editorType ?? 'String');
    }
}
