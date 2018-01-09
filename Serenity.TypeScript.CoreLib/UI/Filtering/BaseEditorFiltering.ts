namespace Serenity {

    @Serenity.Decorators.registerClass('Serenity.BaseEditorFiltering')
    export abstract class BaseEditorFiltering<TEditor> extends BaseFiltering {
        constructor(public editorType: any) {
            super();
        }

        protected useEditor() {
            switch(this.get_operator().key) {
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

        protected editor: Serenity.Widget<any>;

        createEditor() {
            if (this.useEditor()) {
                this.editor = Serenity.Widget.create({
                    type: this.editorType,
                    container: this.get_container(),
                    options: this.getEditorOptions(),
                    init: null
                });
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
                !Q.isEmptyOrNull(this.get_field().filteringIdField)) {
                return this.get_field().filteringIdField;
            }

            return super.getCriteriaField();
        }

        getEditorOptions() {
            var opt = Q.deepClone({}, this.get_field().editorParams);
            delete opt['cascadeFrom'];
            // currently can't support cascadeFrom in filtering
            return Q.deepClone(opt, this.get_field().filteringParams);
        }

        loadState(state: any) {
            if (this.useEditor()) {
                if (state == null) {
                    return;
                }
                Serenity.EditorUtils.setValue(this.editor, state);
                return;
            }

            super.loadState(state);
        }

        saveState() {
            if (this.useEditor()) {
                return Serenity.EditorUtils.getValue(this.editor);
            }

            return super.saveState();
        }

        getEditorValue() {
            if (this.useEditor()) {
                var value = Serenity.EditorUtils.getValue(this.editor);

                if (value == null || (typeof value == "string" && value.trim().length === 0))
                    throw this.argumentNull();

                return value;
            }

            return super.getEditorValue();
        }

        initQuickFilter(filter: QuickFilter<Widget<any>, any>) {
            super.initQuickFilter(filter);

            filter.type = this.editorType;
            filter.options = Q.deepClone({},
                this.getEditorOptions(),
                this.get_field().quickFilterParams);
        }
    }

}