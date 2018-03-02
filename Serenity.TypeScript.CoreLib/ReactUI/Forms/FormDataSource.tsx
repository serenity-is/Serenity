namespace Serenity.UI {   

    export interface FormDataSourceProps<TEntity> {
        service?: string;
        retrieveService?: string;
        createService?: string;
        updateService?: string;
        entityId?: any;
        entity?: TEntity;
        idProperty?: string;
        nameProperty?: string;
        isActiveProperty?: string;
        isDeletedProperty?: string;
        localTextPrefix?: string;
        readOnly?: boolean;
        view?: (model: FormDataModel<TEntity>) => React.ReactNode;
    }

    export interface FormDataSourceState<TEntity> {
        entity: TEntity;
        formMode: FormMode;
        formTitle: string;
        localizations?: any;
    }

    export class FormDataSource<TEntity> extends React.Component<FormDataSourceProps<TEntity>, FormDataSourceState<TEntity>> {

        private emptyEntity: TEntity = Object.create(null);
        private pendingEntity: TEntity;
        private canSetState: boolean;

        constructor(props: FormDataSourceProps<TEntity>, context?: any) {
            super(props, context);

            var entity = this.props.entity;

            var mode = this.modeFor(entity);
            this.state = {
                formMode: mode,
                formTitle: this.titleFor(entity, mode),
                entity: entity
            };

            if (this.props.entityId != null)
                this.loadById(this.props.entityId);

            this.delete = this.delete.bind(this);
            this.save = this.save.bind(this);        
        }

        componentWillReceiveProps(nextProps: FormDataSourceProps<TEntity>) {
            var nextEntityId = Q.coalesce(nextProps.entityId, null);
            var entityId = Q.coalesce(this.props.entityId, null);
            if (nextEntityId !== entityId) {
                if (nextEntityId === null) {
                    this.loadEntity(nextProps.entity || Object.create(null));
                }
                else {
                    this.loadById(nextProps.entityId);
                }
            }
            else if (nextProps.entity !== this.props.entity) {
                this.loadEntity(Q.coalesce(nextProps.entity, Object.create(null)));
            }
        }

        componentDidMount() {
            this.canSetState = true;
            if (this.pendingEntity !== undefined) {
                this.loadEntity(this.pendingEntity || Object.create(null));
            }
        }

        componentWillUnmount() {
            this.canSetState = false;
        }

        loadEntity(entity: TEntity) {
            if (this.canSetState) {
                var mode = this.modeFor(entity);
                this.setState({
                    formMode: mode,
                    formTitle: this.titleFor(entity, mode),
                    entity: entity
                });
                this.pendingEntity = undefined;
            }
            else {
                this.pendingEntity = entity || null;
            }
        }

        loadResponse(response: RetrieveResponse<TEntity>) {
            this.loadEntity(response.Entity);
        }

        getLoadByIdRequest(entityId: any): RetrieveRequest {
            return {
                EntityId: entityId
            }
        }

        getServiceFor(method: string): string {

            var service = this.props[method.charAt(0).toLowerCase() + method.substr(1) + "Service"];
            if (service != null)
                return service;

            return (this.props.service || "ServiceNotSet!") + "/" + method;
        }

        getLoadByIdOptions(entityId: any): ServiceOptions<RetrieveResponse<TEntity>> {
            return {
                service: this.getServiceFor("Retrieve"),
                request: this.getLoadByIdRequest(entityId)
            }
        }

        loadById(entityId: any): PromiseLike<RetrieveResponse<TEntity>> {
            var options = this.getLoadByIdOptions(entityId);
            return Q.serviceCall(options).then(response => {
                this.loadResponse(response);
                return response;
            });
        }

        isDeleted(entity: TEntity): boolean {
            if (this.props.isDeletedProperty && entity[this.props.isDeletedProperty])
                return true;

            if (this.props.isActiveProperty && entity[this.props.isActiveProperty] === -1)
                return true;
        }

        modeFor(entity: TEntity) {
            if (entity == null)
                return FormMode.Initial;

            if (this.props.readOnly)
                return FormMode.View;

            if (this.props.idProperty && entity[this.props.idProperty] != null)
                return FormMode.Edit;

            if (this.isDeleted(entity))
                return FormMode.Deleted;

            return FormMode.New;
        }

        protected getEntityName(): string {
            if (this.props.localTextPrefix) {
                var es = Q.tryGetText("Db." + this.props.localTextPrefix + '.EntitySingular');
                if (es)
                    return es;
            }

            return "Item";
        }

        protected getNameValue(entity: TEntity): any {
            if (!entity || !this.props.nameProperty)
                return "";

            return Q.coalesce(entity[this.props.nameProperty], '').toString();
        }

        titleFor(entity: TEntity, mode: FormMode) {

            if (mode == FormMode.New)
                return Q.format(Q.text('Controls.EntityDialog.NewRecordTitle'), this.getEntityName());

            var title = Q.coalesce(this.getNameValue(entity), '');
            return Q.format(Q.text('Controls.EntityDialog.EditRecordTitle'),
                this.getEntityName(), (Q.isEmptyOrNull(title) ? '' : (' (' + title + ')')));
        }

        isEditMode() {
            return this.state.formMode == FormMode.Edit;
        }

        getSaveEntity(values: TEntity): TEntity {
            return values;
        }

        getSaveOptions(values: TEntity): ServiceOptions<SaveResponse> {

            var opt: Q.ServiceOptions<SaveResponse> = {};
            opt.service = this.getServiceFor(this.isEditMode() ? "Update" : "Create");
            opt.request = this.getSaveRequest(values);

            return opt;
        }

        getIdProperty() {
            return this.props.idProperty || "NoIdProperty!";
        }

        protected getLanguages(): any[] {
            if (Serenity.EntityDialog.defaultLanguageList != null)
                return Serenity.EntityDialog.defaultLanguageList() || [];

            return [];
        }

        protected getPendingLocalizations(): any {
            if (this.state.localizations == null) {
                return null;
            }

            var result: Q.Dictionary<any> = {};
            var idField = this.getIdProperty();
            var langs = this.getLanguages();

            for (var pair of langs) {
                var language = pair[0];
                var entity: any = {};

                if (idField != null) {
                    entity[idField] = this.entity[this.getIdProperty()];
                }

                var prefix = language + '$';

                for (var k of Object.keys(this.state.localizations)) {
                    if (Q.startsWith(k, prefix))
                        entity[k.substr(prefix.length)] = this.state.localizations[k];
                }

                result[language] = entity;
            }

            return result;
        }

        getSaveRequest(values: TEntity) {
            var entity = this.getSaveEntity(values);
            var req: SaveRequest<TEntity> = {};
            req.Entity = entity;

            if (this.isEditMode()) {
                var idField = this.getIdProperty();
                if (idField != null) {
                    req.EntityId = this.entity[this.props.idProperty];
                }
            }

            if (this.state.localizations != null) {
                req.Localizations = this.getPendingLocalizations();
            }

            return req;
        }

        save(values: TEntity): PromiseLike<void> {
            var options = this.getSaveOptions(values);
            return Q.serviceCall(options);
        }

        delete(): PromiseLike<void> {
            return null;
        }

        undelete(): PromiseLike<void> {
            return null;
        }

        getDataModel(): FormDataModel<TEntity> {
            return {
                entity: this.state.entity || this.emptyEntity,
                formMode: this.state.formMode,
                formTitle: this.state.formTitle,
                onSave: (this.state.formMode == FormMode.Edit || this.state.formMode == FormMode.New) ? this.save : null,
                onDelete: this.state.formMode == FormMode.Edit ? this.delete : null,
                onUndelete: this.state.formMode == FormMode.Deleted ? this.undelete : null,
            };
        }

        get dataModel() {
            return this.getDataModel();
        }

        get entity() {
            return this.pendingEntity !== undefined ? this.pendingEntity : this.state.entity;
        }

        render() {
            return this.props.view(this.getDataModel());
        }
    }
}