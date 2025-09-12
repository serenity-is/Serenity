import { extend } from "../../compat";
import { EditorTypeRegistry } from "../../types/editortyperegistry";
import { QuickFilter } from "../datagrid/quickfilter";
import { Widget } from "../widgets/widget";
import { BaseEditorFiltering } from "./baseeditorfiltering";
import { FilterOperator, FilterOperators } from "./filteroperator";

export class EditorFiltering extends BaseEditorFiltering<Widget<any>> {
    static override typeInfo = this.classTypeInfo("Serenity.EditorFiltering");

    constructor(public readonly props: { editorType?: string, useRelative?: boolean, useLike?: boolean } = {}) {
        super(Widget);
        this.props ??= {};
    }

    get editorType() { return this.props.editorType }
    set editorType(value) { this.props.editorType = value }

    get useRelative() { return this.props.useRelative }
    set useRelative(value) { this.props.useRelative = value }

    get useLike() { return this.props.useLike }
    set useLike(value) { this.props.useLike = value }

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

    protected useEditor() {
        var op = this.get_operator().key;

        return op === FilterOperators.EQ ||
            op === FilterOperators.NE ||
            (this.useRelative && (
                op === FilterOperators.LT ||
                op === FilterOperators.LE ||
                op === FilterOperators.GT ||
                op === FilterOperators.GE));
    }

    getEditorOptions() {
        var opt = super.getEditorOptions();
        if (this.useEditor() && this.editorType === (this.get_field().editorType ?? 'String')) {
            opt = extend(opt, this.get_field().editorParams);
        }

        return opt;
    }

    createEditor() {
        if (this.useEditor()) {
            var editorType = EditorTypeRegistry.get(this.editorType) as typeof Widget<{}>;

            this.editor = new editorType({
                element: el => this.get_container().append(el),
                ...this.getEditorOptions()
            });

            return;
        }

        super.createEditor();
    }

    protected useIdField(): boolean {
        return this.useEditor();
    }

    initQuickFilter(filter: QuickFilter<Widget<any>, any>) {
        super.initQuickFilter(filter);

        filter.type = EditorTypeRegistry.get(this.editorType);
    }
}
