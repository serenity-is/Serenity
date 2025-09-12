import { nsSerenity } from "../../base";
import { deepClone, extend } from "../../compat";
import { QuickFilter } from "../datagrid/quickfilter";
import { EditorUtils } from "../editors/editorutils";
import { Widget } from "../widgets/widget";
import { BaseFiltering } from "./basefiltering";

export abstract class BaseEditorFiltering<TEditor extends Widget<any>> extends BaseFiltering {
    static override typeInfo = this.registerClass(nsSerenity);
    constructor(public editorTypeRef: any) {
        super();
    }

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

    createEditor() {
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

    protected useIdField() {
        return false;
    }

    getCriteriaField() {
        if (this.useEditor() &&
            this.useIdField() &&
            this.get_field().filteringIdField) {
            return this.get_field().filteringIdField;
        }

        return super.getCriteriaField();
    }

    getEditorOptions() {
        var opt = deepClone(this.get_field().editorParams || {});
        delete opt['cascadeFrom'];
        // currently can't support cascadeFrom in filtering
        return extend(opt, this.get_field().filteringParams);
    }

    loadState(state: any) {
        if (this.useEditor()) {
            if (state == null) {
                return;
            }

            EditorUtils.setValue(this.editor, state);
            return;
        }

        super.loadState(state);
    }

    saveState() {
        if (this.useEditor()) {
            return EditorUtils.getValue(this.editor);
        }

        return super.saveState();
    }

    getEditorValue() {
        if (this.useEditor()) {
            var value = EditorUtils.getValue(this.editor);

            if (value == null || (typeof value == "string" && value.trim().length === 0))
                throw this.argumentNull();

            return value;
        }

        return super.getEditorValue();
    }

    initQuickFilter(filter: QuickFilter<Widget<any>, any>) {
        super.initQuickFilter(filter);

        filter.type = this.editorTypeRef;
        filter.options = extend(extend({}, deepClone(this.getEditorOptions())), deepClone(this.get_field().quickFilterParams));
    }
}
