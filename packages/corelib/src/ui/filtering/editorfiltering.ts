import { isPromiseLike, nsSerenity } from "../../base";
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
        const list = [];

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
        const op = this.get_operator().key;

        return op === FilterOperators.EQ ||
            op === FilterOperators.NE ||
            (this.useRelative && (
                op === FilterOperators.LT ||
                op === FilterOperators.LE ||
                op === FilterOperators.GT ||
                op === FilterOperators.GE));
    }

    /** Marks the place of the editor while its type is loading. It is gone if the container is emptied meanwhile. */
    declare private loadingPoint: Comment;
    /** The state loaded while the editor type is loading, which is applied once the editor is created. */
    declare private pendingState: any;

    /**
     * Whether the type of the editor for the current operator is still loading.
     * @returns True when it is loading.
     */
    private isEditorLoading() {
        return this.loadingPoint != null && this.useEditor();
    }

    /**
     * Creates the editor for the current operator. If its type has to be loaded, it is created when the
     * type is loaded, and until then the state methods work with the state that will be applied to it.
     */
    override createEditor() {
        this.loadingPoint?.remove();
        this.loadingPoint = null;
        this.pendingState = undefined;

        if (this.useEditor()) {
            const editorType = EditorTypeRegistry.getOrLoad(this.editorType ?? 'String');
            if (isPromiseLike(editorType)) {
                const loadingPoint = this.loadingPoint = document.createComment("Loading editor type...");
                this.editor = null;
                this.get_container().append(loadingPoint);
                editorType.then(type => this.editorTypeLoaded(type, loadingPoint));
                return;
            }

            this.editor = this.newEditor(editorType, el => this.get_container().append(el));
            return;
        }

        super.createEditor();
    }

    private newEditor(editorType: any, element: (el: HTMLElement) => void) {
        return new (editorType as typeof Widget<{}>)({
            element,
            ...this.getEditorOptions()
        }).init?.();
    }

    private editorTypeLoaded(editorType: any, loadingPoint: Comment) {
        // a later createEditor call replaced this one
        if (this.loadingPoint !== loadingPoint)
            return;

        this.loadingPoint = null;
        const state = this.pendingState;
        this.pendingState = undefined;

        // the container is emptied, e.g. the field or the operator of the row changed
        if (!loadingPoint.parentNode)
            return;

        this.editor = this.newEditor(editorType, el => loadingPoint.replaceWith(el));
        super.loadState(state);
    }

    /**
     * Loads persisted state into the editor, or keeps it until the editor is created.
     * @param state - The persisted state.
     */
    override loadState(state: any) {
        if (this.isEditorLoading())
            this.pendingState = state;
        else
            super.loadState(state);
    }

    /**
     * Saves the editor state for persistence.
     * @returns The saved state.
     */
    override saveState() {
        return this.isEditorLoading() ? this.pendingState : super.saveState();
    }

    /**
     * Returns the current editor value.
     * @returns The editor value.
     */
    override getEditorValue() {
        if (!this.isEditorLoading())
            return super.getEditorValue();

        const value = this.pendingState;
        if (value == null || (typeof value == "string" && value.trim().length === 0))
            throw this.argumentNull();

        return value;
    }

    /**
     * Returns the display text of the current editor value, which is the state while the editor is loading.
     * @returns The editor text.
     */
    override getEditorText(): string {
        return this.isEditorLoading() ? (this.pendingState?.toString() ?? "") : super.getEditorText();
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

        filter.type = EditorTypeRegistry.getOrLoad(this.editorType ?? 'String');
    }
}
