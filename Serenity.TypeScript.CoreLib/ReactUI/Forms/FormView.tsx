namespace Serenity.UI {    
    export interface FormViewProps<TEntity> extends FormDataModel<TEntity> {
        onClose?: () => void;
    }

    export class FormView<TEntity, TProps extends FormViewProps<TEntity> = FormViewProps<TEntity>, TState = any>
        extends React.Component<TProps, TState> {

        protected editors = new EditorRefs();

        canSave(): boolean {
            return this.props.onSave != null;
        }

        showSave(): boolean {
            return this.props.formMode == FormMode.Edit || this.props.formMode == FormMode.New;
        }

        canClose(): boolean {
            return this.props.onClose != null;
        }

        showApplyChanges(): boolean {
            return this.showSave() && this.canClose();
        }

        isUpdate(): boolean {
            return this.props.formMode == FormMode.Edit;
        }

        canDelete(): boolean {
            return this.props.onDelete != null;
        }

        showDelete(): boolean {
            return this.props.formMode == FormMode.Edit;
        }

        canUndelete(): boolean {
            return this.props.onUndelete != null;
        }

        showUndelete(): boolean {
            return this.props.formMode == FormMode.Deleted;
        }

        loadEntity(entity: TEntity) {
            this.editors.loadFrom(entity);
        }

        componentDidMount() {
            this.loadEntity(this.props.entity || Object.create(null));
        }

        componentWillReceiveProps(nextProps: TProps) {
            if (nextProps.entity !== null &&
                this.props.entity !== nextProps.entity) {
                this.loadEntity(nextProps.entity || Object.create(null));
            }
        }

        renderSaveButton() {
            return <SaveButton isUpdate={this.isUpdate()} disabled={!this.canSave()} onClick={() => this.save(true)} />;
        }

        renderApplyChangesButton() {
            return <ApplyChangesButton disabled={!this.canSave()} onClick={() => this.save(false)} />;
        }

        renderDeleteButton() {
            return <DeleteButton disabled={!this.canDelete()} />;
        }

        renderToolbar(children?: React.ReactNode) {
            return (
                <Toolbar>
                    {this.showSave() && this.renderSaveButton()}
                    {this.showApplyChanges() && this.renderApplyChangesButton()}
                    {this.showDelete() && this.renderDeleteButton()}
                    {children}
                </Toolbar>
            )
        }

        save(close?: boolean): PromiseLike<void> {
            if (this.props.onSave == null)
                return Promise.reject("No onSave handler!");

            var values: TEntity = Object.create(null);
            this.editors.saveTo(values);

            var promise = this.props.onSave(values);

            if (close) {
                if (this.props.onClose != null)
                    promise = promise.then(e => this.props.onClose != null && this.props.onClose());
            }
            else if (this.props.onReload != null) {
                promise = promise.then(e => this.props.onReload != null && this.props.onReload());
            }

            return promise;
        }
    }
}