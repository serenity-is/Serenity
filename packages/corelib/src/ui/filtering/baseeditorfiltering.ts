import { nsSerenity } from "../../base";
import { deepClone } from "../../compat";
import { QuickFilter } from "../datagrid/quickfilter";
import { EditorUtils } from "../editors/editorutils";
import { Widget } from "../widgets/widget";
import { BaseFiltering } from "./basefiltering";

/**
 * Base filtering handler that uses an editor widget for comparison operators.
 * @typeParam TEditor - The editor widget type.
 */
export abstract class BaseEditorFiltering<TEditor extends Widget<any>> extends BaseFiltering {
    static override[Symbol.typeInfo] = this.registerClass(nsSerenity);
    /**
     * Creates a base editor filtering handler.
     * @param editorTypeRef - Constructor of the editor widget type.
     */
    constructor(public editorTypeRef: any) {
        super();
    }

    /**
     * Whether the current operator uses an editor.
     * @returns True when an editor is used.
     */
    protected useEditor() {
        switch (this.get_operator().key) {
            case 'eq':
            case 'ne':
            case 'lt':
            case 'le':
            case 'gt':
            case 'ge':
                return true;
        }
        return false;
    }

    declare protected editor: TEditor;

    /**
     * Creates the editor for the current operator.
     */
    override createEditor() {
        if (this.useEditor()) {
            this.editor = new (this.editorTypeRef as typeof Widget<{}>)({
                element: el => {
                    this.get_container().append(el);
                },
                ...this.getEditorOptions()
            }).init() as TEditor;
            return;
        }
        this.editor = null;
        super.createEditor();
    }

    /**
     * Whether to use the id field for the criteria.
     * @returns True when the id field is used.
     */
    protected useIdField() {
        return false;
    }

    /**
     * Returns the criteria field name, using the filtering id field when applicable.
     * @returns The criteria field name.
     */
    override getCriteriaField() {
        if (this.useEditor() &&
            this.useIdField() &&
            this.get_field().filteringIdField) {
            return this.get_field().filteringIdField;
        }

        return super.getCriteriaField();
    }

    /**
     * Returns the options for the editor widget.
     * @returns The editor options.
     */
    getEditorOptions() {
        var opt = deepClone(this.get_field().editorParams || {});
        delete opt['cascadeFrom'];
        // currently can't support cascadeFrom in filtering
        return Object.assign(opt, this.get_field().filteringParams);
    }

    /**
     * Loads persisted state into the editor.
     * @param state - The persisted state.
     */
    override loadState(state: any) {
        if (this.useEditor()) {
            if (state == null) {
                return;
            }

            EditorUtils.setValue(this.editor, state);
            return;
        }

        super.loadState(state);
    }

    /**
     * Saves the editor state for persistence.
     * @returns The saved state.
     */
    override saveState() {
        if (this.useEditor()) {
            return EditorUtils.getValue(this.editor);
        }

        return super.saveState();
    }

    /**
     * Returns the current editor value.
     * @returns The editor value.
     */
    override getEditorValue() {
        if (this.useEditor()) {
            var value = EditorUtils.getValue(this.editor);

            if (value == null || (typeof value == "string" && value.trim().length === 0))
                throw this.argumentNull();

            return value;
        }

        return super.getEditorValue();
    }

    /**
     * Initializes a quick filter using the editor type.
     * @param filter - The quick filter to initialize.
     */
    override initQuickFilter(filter: QuickFilter<Widget<any>, any>) {
        super.initQuickFilter(filter);

        filter.type = this.editorTypeRef;
        filter.options = Object.assign(Object.create(null), deepClone(this.getEditorOptions()), deepClone(this.get_field().quickFilterParams));
    }
}
