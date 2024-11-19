import { DeleteRequest, DeleteResponse, Fluent, RetrieveRequest, RetrieveResponse, SaveRequest, SaveResponse, ServiceOptions, UndeleteRequest, UndeleteResponse, confirmDialog, getInstanceType, getTypeFullName, localText, notifySuccess, serviceCall, stringFormat, tryGetText, type PropertyItem, type PropertyItemsData } from "../../base";
import { IEditDialog, IReadOnly } from "../../interfaces";
import { Authorization, Exception, ScriptData, ValidationHelper, extend, getFormData, getFormDataAsync, replaceAll, validatorAbortHandler } from "../../q";
import { DataChangeInfo } from "../../types";
import { EntityTypeAttribute, FormKeyAttribute, IdPropertyAttribute, IsActivePropertyAttribute, ItemNameAttribute, LocalTextPrefixAttribute, NamePropertyAttribute, ServiceAttribute } from "../../types/attributes";
import { Decorators } from "../../types/decorators";
import { IRowDefinition } from "../datagrid/irowdefinition";
import { EditorUtils } from "../editors/editorutils";
import { SubDialogHelper } from "../helpers/subdialoghelper";
import { TabsExtensions } from "../helpers/tabsextensions";
import { PropertyGrid, PropertyGridMode, PropertyGridOptions } from "../widgets/propertygrid";
import { ToolButton } from "../widgets/toolbar";
import { Widget, WidgetProps } from "../widgets/widget";
import { BaseDialog } from "./basedialog";
import { EntityLocalizer, EntityLocalizerOptions } from "./entitylocalizer";
import { SaveInitiator, applyChangesToolButton, cloneToolButton, deleteToolButton, editToolButton, localizationToolButton, saveAndCloseToolButton, undeleteToolButton } from "./entitytoolbuttons";

@Decorators.registerClass('Serenity.EntityDialog', [IEditDialog, IReadOnly])
@Decorators.panel(true)
export class EntityDialog<TItem, P = {}> extends BaseDialog<P> implements IEditDialog, IReadOnly {

    declare private _entity: TItem;
    declare private _entityId: any;

    declare protected propertyItemsData: PropertyItemsData;
    declare protected propertyGrid: PropertyGrid;

    declare protected saveAndCloseButton: Fluent;
    declare protected applyChangesButton: Fluent;
    declare protected deleteButton: Fluent;
    declare protected undeleteButton: Fluent;
    declare protected cloneButton: Fluent;
    declare protected editButton: Fluent;

    declare protected localizer: EntityLocalizer;
    declare protected localizerButton: Fluent;

    static defaultLanguageList: () => string[][];

    constructor(props?: WidgetProps<P>) {
        super(props);

        this.syncOrAsyncThen(this.getPropertyItemsData, this.getPropertyItemsDataAsync, itemsData => {
            this.propertyItemsReady(itemsData);
            this.afterInit();
        });
    }

    protected propertyItemsReady(itemsData: PropertyItemsData) {
        this.propertyItemsData = itemsData;
        this.initPropertyGrid();
        this.initLocalizer();
    }

    protected afterInit() {
    }

    protected useAsync() {
        return false;
    }

    destroy(): void {

        if (this.propertyGrid) {
            this.propertyGrid.destroy();
            this.propertyGrid = null;
        }

        if (this.localizer) {
            this.localizer.destroy();
            this.localizer = null;
        }

        this.undeleteButton = null;
        this.applyChangesButton = null;
        this.deleteButton = null;
        this.saveAndCloseButton = null;
        this.editButton = null;
        this.cloneButton = null;
        this.toolbar = null;

        super.destroy();
    }

    get entity(): TItem {
        return this._entity;
    }

    protected set entity(value: TItem) {
        this._entity = value || new Object() as any;
    }

    /** @deprecated use entityId */
    protected get_entityId(): any {
        return this.entityId;
    }

    get entityId(): any {
        return this._entityId;
    }

    protected set entityId(value: any) {
        this._entityId = value;
    }

    protected getEntityNameFieldValue(): any {
        return ((this.entity as any)[this.getNameProperty()] ?? '').toString();
    }

    protected getEntityTitle(): string {
        if (!this.isEditMode()) {
            return stringFormat(localText('Controls.EntityDialog.NewRecordTitle'), this.getEntitySingular());
        }
        else {
            var titleFormat = (this.isViewMode() || this.readOnly || !this.hasSavePermission()) ?
                localText('Controls.EntityDialog.ViewRecordTitle') : localText('Controls.EntityDialog.EditRecordTitle');
            var title = this.getEntityNameFieldValue() ?? '';
            return stringFormat(titleFormat,
                this.getEntitySingular(), !title ? '' : (' (' + title + ')'));
        }
    }

    protected updateTitle(): void {
        this.dialogTitle = this.getEntityTitle();
    }

    protected isCloneMode(): boolean {
        return false;
    }

    protected isEditMode(): boolean {
        return this.entityId != null && !this.isCloneMode();
    }

    protected isDeleted(): boolean {
        if (this.entityId == null) {
            return false;
        }

        var isDeletedProperty = this.getIsDeletedProperty();
        if (isDeletedProperty) {
            return !!(this.entity as any)[isDeletedProperty];
        }

        var value = (this.entity as any)[this.getIsActiveProperty()];
        if (value == null) {
            return false;
        }

        return value < 0;
    }

    protected isNew(): boolean {
        return this.entityId == null;
    }

    protected isNewOrDeleted(): boolean {
        return this.isNew() || this.isDeleted();
    }

    protected getDeleteOptions(callback: (response: DeleteResponse) => void): ServiceOptions<DeleteResponse> {
        return {};
    }

    protected deleteHandler(options: ServiceOptions<DeleteResponse>, callback: (response: DeleteResponse) => void): void {
        serviceCall(options);
    }

    protected getDeleteServiceMethod() {
        return this.getService() + '/Delete';
    }

    protected doDelete(callback: (response: DeleteResponse) => void): void {
        var self = this;

        var request: DeleteRequest = {
            EntityId: this.entityId
        };

        var baseOptions: ServiceOptions<DeleteResponse> = {
            service: this.getDeleteServiceMethod(),
            request: request,
            onSuccess: response => {
                self.onDeleteSuccess(response);
                if (callback != null) {
                    callback(response);
                }
                Fluent.trigger(this.domNode, "ondatachange", {
                    entityId: request.EntityId,
                    entity: this.entity,
                    operationType: 'delete'
                } satisfies Partial<DataChangeInfo>);
            }
        };

        var thisOptions = this.getDeleteOptions(callback);
        var finalOptions = extend(baseOptions, thisOptions);

        this.deleteHandler(finalOptions, callback);
    }

    protected onDeleteSuccess(response: DeleteResponse): void {
    }

    protected getRowDefinition(): IRowDefinition {
        return null;
    }

    declare private _entityType: string;

    protected getEntityType(): string {

        if (this._entityType != null)
            return this._entityType;

        var attr = this.getCustomAttribute(EntityTypeAttribute);
        if (attr)
            return (this._entityType = attr.value);

        // remove global namespace
        var name = getTypeFullName(getInstanceType(this));
        var px = name.indexOf('.');
        if (px >= 0)
            name = name.substring(px + 1);

        // don't like this kind of convention, make it obsolete soon...
        if (name.endsWith('Dialog') || name.endsWith('Control'))
            name = name.substring(0, name.length - 6);
        else if (name.endsWith('Panel'))
            name = name.substring(0, name.length - 5);

        return (this._entityType = name);
    }

    declare private _formKey: string;

    protected getFormKey(): string {
        if (this._formKey != null)
            return this._formKey;

        var attr = this.getCustomAttribute(FormKeyAttribute);
        if (attr)
            return (this._formKey = attr.value);

        return (this._formKey = this.getEntityType());
    }

    declare private _localTextDbPrefix: string;

    protected getLocalTextDbPrefix(): string {
        if (this._localTextDbPrefix != null)
            return this._localTextDbPrefix;

        this._localTextDbPrefix = this.getLocalTextPrefix() ?? '';

        if (this._localTextDbPrefix.length > 0 && !this._localTextDbPrefix.endsWith('.'))
            this._localTextDbPrefix = 'Db.' + this._localTextDbPrefix + '.';

        return this._localTextDbPrefix;
    }

    protected getLocalTextPrefix(): string {
        var rowDefinition = this.getRowDefinition();
        if (rowDefinition)
            return rowDefinition.localTextPrefix;

        var attr = this.getCustomAttribute(LocalTextPrefixAttribute);
        if (attr)
            return attr.value;

        return this.getEntityType();
    }

    declare private _entitySingular: string;

    protected getEntitySingular(): string {
        if (this._entitySingular != null)
            return this._entitySingular;

        var attr = this.getCustomAttribute(ItemNameAttribute);
        return (this._entitySingular = attr ? localText(attr.value, attr.value) :
            tryGetText(this.getLocalTextDbPrefix() + 'EntitySingular') ?? this.getEntityType());
    }

    declare private _nameProperty: string;

    protected getNameProperty(): string {
        if (this._nameProperty != null)
            return this._nameProperty;

        var rowDefinition = this.getRowDefinition();
        if (rowDefinition)
            return this._nameProperty = rowDefinition.nameProperty ?? '';

        var attr = this.getCustomAttribute(NamePropertyAttribute);

        if (attr)
            return this._nameProperty = attr.value ?? '';

        return this._nameProperty = 'Name';
    }

    declare private _idProperty: string;

    protected getIdProperty(): string {
        if (this._idProperty != null)
            return this._idProperty;

        var rowDefinition = this.getRowDefinition();
        if (rowDefinition)
            return this._idProperty = rowDefinition.idProperty ?? '';

        var attr = this.getCustomAttribute(IdPropertyAttribute);
        if (attr)
            return this._idProperty = attr.value ?? '';

        return this._idProperty = 'ID';
    }

    declare private _isActiveProperty: string;

    protected getIsActiveProperty(): string {
        if (this._isActiveProperty != null)
            return this._isActiveProperty;

        var rowDefinition = this.getRowDefinition();
        if (rowDefinition)
            return this._isActiveProperty = rowDefinition.isActiveProperty ?? '';

        var attr = this.getCustomAttribute(IsActivePropertyAttribute);
        if (attr)
            return this._isActiveProperty = attr.value ?? '';

        return this._isActiveProperty = '';
    }

    protected getIsDeletedProperty(): string {
        return this.getRowDefinition()?.isDeletedProperty;
    }

    declare private _service: string;

    protected getService() {
        if (this._service != null)
            return this._service;

        var attr = this.getCustomAttribute(ServiceAttribute);
        if (attr)
            this._service = attr.value;
        else
            this._service = replaceAll(this.getEntityType(), '.', '/');

        return this._service;
    }

    load(entityOrId: any, done: () => void, fail?: (ex: Exception) => void): void {

        var action = () => {

            if (entityOrId == null) {
                this.loadResponse({});
                done && done();
                return;
            }

            var scriptType = typeof (entityOrId);
            if (scriptType === 'string' || scriptType === 'number') {
                var entityId = entityOrId;

                this.loadById(entityId, function (response) {
                    if (done)
                        window.setTimeout(done, 0);
                }, null);

                return;
            }

            var entity = entityOrId || new Object();
            this.loadResponse({ Entity: entity });
            done && done();
        };

        if (fail == null) {
            action();
            return;
        }

        try {
            action();
        }
        catch (ex1) {
            var ex = (Exception as any).wrap(ex1);
            fail(ex);
        }
    }

    loadNewAndOpenDialog(asPanel?: boolean): void {
        this.loadResponse({});
        this.dialogOpen(asPanel);
    }

    loadEntityAndOpenDialog(entity: TItem, asPanel?: boolean): void {
        this.loadResponse({ Entity: entity });
        this.dialogOpen(asPanel);
    }

    protected loadResponse(data: any): void {
        this.init();
        data = data || {};
        this.onLoadingData(data);
        var entity = data.Entity || new Object();
        this.beforeLoadEntity(entity);
        this.loadEntity(entity);
        this.entity = entity;
        this.afterLoadEntity();
    }

    protected loadEntity(entity: TItem): void {
        var idField = this.getIdProperty();

        if (idField != null)
            this.entityId = ((entity as any)[idField]);

        this.entity = entity;

        if (this.propertyGrid != null) {
            this.propertyGrid.set_mode((this.isEditMode() ?
                PropertyGridMode.update : PropertyGridMode.insert));
            this.propertyGrid.load(entity);
        }
    }

    protected beforeLoadEntity(entity: TItem): void {
        this.localizer?.clearValue();
    }

    protected afterLoadEntity(): void {
        this.updateInterface();
        this.updateTitle();
    }

    public loadByIdAndOpenDialog(entityId: any, asPanel?: boolean, callback?: (response: RetrieveResponse<TItem>) => void, fail?: () => void): void {
        this.loadById(entityId,
            response => window.setTimeout(() => {
                this.dialogOpen(asPanel);
                callback?.(response);
            }, 0),
            () => {
                if (!Fluent.isVisibleLike(this.domNode)) {
                    this.domNode.remove();
                }
                fail?.();
            });
    }

    protected onLoadingData(data: RetrieveResponse<TItem>): void {
    }

    protected getLoadByIdOptions(id: any, callback: (response: RetrieveResponse<TItem>) => void): ServiceOptions<RetrieveResponse<TItem>> {
        return {};
    }

    protected getLoadByIdRequest(id: any): RetrieveRequest {
        var request: RetrieveRequest = {};
        request.EntityId = id;
        return request;
    }

    protected reloadById(): void {
        this.loadById(this.entityId);
    }

    protected getRetrieveServiceMethod() {
        return this.getService() + '/Retrieve';
    }

    loadById(id: any, callback?: (response: RetrieveResponse<TItem>) => void, fail?: () => void) {
        var baseOptions: ServiceOptions<RetrieveResponse<TItem>> = {
            service: this.getRetrieveServiceMethod(),
            blockUI: true,
            request: this.getLoadByIdRequest(id),
            onSuccess: response => {
                this.loadResponse(response);
                callback && callback(response);
            },
            onCleanup: () => {
                if (this.validator != null) {
                    validatorAbortHandler(this.validator);
                }
            }
        };

        var thisOptions = this.getLoadByIdOptions(id, callback);
        var finalOptions = extend(baseOptions, thisOptions);
        this.loadByIdHandler(finalOptions, callback, fail);
    }

    protected loadByIdHandler(options: ServiceOptions<RetrieveResponse<TItem>>, callback: (response: RetrieveResponse<TItem>) => void, fail: () => void): void {
        var request = serviceCall(options);
        fail && ((request as any)?.fail ? (request as any).fail(fail) : request.then(null, fail));
    }

    protected getLocalizerOptions(): EntityLocalizerOptions {
        return {
            idPrefix: this.idPrefix,
            getButton: () => this.localizerButton,
            getEntity: () => this.entity,
            getEntityId: () => this.entityId,
            getIdProperty: () => this.getIdProperty(),
            getLanguages: () => this.getLanguages(),
            getRetrieveServiceMethod: () => this.getRetrieveServiceMethod(),
            getToolButtons: () => this.toolbar.element.findAll(".tool-button"),
            isNew: () => this.isNew(),
            validateForm: () => this.validateForm(),
            byId: s => this.byId(s),
            getPropertyGrid: () => this.byId("PropertyGrid"),
            pgOptions: this.getPropertyGridOptions()
        };
    }

    protected initLocalizer(): void {
        const pgDiv = this.findById('PropertyGrid');
        if (!pgDiv)
            return;

        this.localizer = new EntityLocalizer(this.getLocalizerOptions());
    }

    protected getLanguages(): any[] {
        if (EntityDialog.defaultLanguageList != null)
            return EntityDialog.defaultLanguageList() || [];

        return [];
    }

    protected initPropertyGrid(): void {
        var pgDiv = this.byId('PropertyGrid');
        if (pgDiv.length <= 0) {
            return;
        }
        var pgOptions = this.getPropertyGridOptions();
        this.propertyGrid = (new PropertyGrid({ element: pgDiv, ...pgOptions })).init();
    }

    protected getPropertyItems(): PropertyItem[] {
        return this.propertyItemsData?.items || [];
    }

    protected getPropertyItemsData(): PropertyItemsData {
        var formKey = this.getFormKey();

        if (this.getFormKey === EntityDialog.prototype.getFormKey &&
            this.getPropertyItems !== EntityDialog.prototype.getPropertyItems &&
            !ScriptData.canLoad('Form.' + formKey)) {
            return {
                items: this.getPropertyItems(),
                additionalItems: []
            }
        }

        if (formKey) {
            return getFormData(formKey);
        }

        return { items: [], additionalItems: [] };
    }

    protected async getPropertyItemsDataAsync(): Promise<PropertyItemsData> {
        var formKey = this.getFormKey();
        if (formKey) {
            return await getFormDataAsync(formKey);
        }

        return { items: [], additionalItems: [] };
    }

    protected getPropertyGridOptions(): PropertyGridOptions {
        return {
            idPrefix: this.idPrefix,
            items: this.getPropertyItems(),
            mode: PropertyGridMode.insert,
            localTextPrefix: 'Forms.' + this.getFormKey() + '.'
        };
    }

    protected validateBeforeSave(): boolean {
        return true;
    }

    protected getCreateServiceMethod() {
        return this.getService() + '/Create';
    }

    protected getUpdateServiceMethod() {
        return this.getService() + '/Update';
    }

    protected getSaveOptions(callback: (response: SaveResponse) => void, initiator?: SaveInitiator): ServiceOptions<SaveResponse> {

        var opt: ServiceOptions<SaveResponse> = {};

        opt.service = this.isEditMode() ? this.getUpdateServiceMethod() : this.getCreateServiceMethod();

        opt.onSuccess = response => {
            this.onSaveSuccess(response, initiator);

            callback && callback(response);

            var typ = (this.isEditMode() ? 'update' : 'create');

            var ent = opt.request == null ? null : opt.request.Entity;
            var eid: any = this.isEditMode() ? this.entityId :
                (response == null ? null : response.EntityId);

            var dci = {
                operationType: typ,
                entity: ent,
                entityId: eid
            } satisfies Partial<DataChangeInfo>;

            Fluent.trigger(this.domNode, "ondatachange", dci);
        };

        opt.onCleanup = () => {
            this.validator && validatorAbortHandler(this.validator);
        };

        opt.request = this.getSaveRequest();

        return opt;
    }

    protected getSaveEntity(): TItem {

        var entity: TItem = new Object() as any;

        if (this.propertyGrid != null) {
            this.propertyGrid.save(entity);
        }

        if (this.isEditMode()) {
            var idField = this.getIdProperty();
            if (idField != null && (entity as any)[idField] == null) {
                (entity as any)[idField] = this.entityId;
            }
        }

        return entity;
    }

    protected getSaveRequest(): SaveRequest<TItem> {

        var entity = this.getSaveEntity();
        var req: SaveRequest<TItem> = {};
        req.Entity = entity;

        if (this.isEditMode()) {
            var idField = this.getIdProperty();
            if (idField != null) {
                req.EntityId = this.entityId;
            }
        }

        this.localizer?.adjustSaveRequest(req);
        return req;
    }

    protected onSaveSuccess(response: SaveResponse, initiator?: SaveInitiator): void {
        initiator !== "save-and-close" && this.showSaveSuccessMessage(response, initiator);
    }

    protected save_submitHandler(callback: (response: SaveResponse) => void, initiator: SaveInitiator): void {
        var options = this.getSaveOptions(callback, initiator);
        this.saveHandler(options, callback, initiator);
    }

    protected save(callback?: (response: SaveResponse) => void, initiator?: SaveInitiator): void | boolean {
        return ValidationHelper.submit(this.byId('Form'),
            () => this.validateBeforeSave(),
            () => this.save_submitHandler(callback, initiator));
    }

    protected saveHandler(options: ServiceOptions<SaveResponse>, callback: (response: SaveResponse) => void, initiator: SaveInitiator): void {
        serviceCall(options);
    }

    protected showSaveSuccessMessage(response: SaveResponse, initiator?: SaveInitiator): void {
        notifySuccess(localText('Controls.EntityDialog.SaveSuccessMessage'), '', null);
    }

    protected getToolbarButtons(): ToolButton[] {
        return [
            saveAndCloseToolButton({
                onClick: () => this.save(() => this.dialogClose("save-and-close"), "save-and-close"),
                visible: () => !this.isDeleted() && !this.isViewMode(),
                disabled: () => !this.hasSavePermission() || this.readOnly,
                ref: el => this.saveAndCloseButton = Fluent(el)
            }),
            applyChangesToolButton({
                onClick: () => this.save(response => this.loadById(this.isEditMode() ? (response?.EntityId ?? this.entityId) : response?.EntityId), "apply-changes"),
                visible: () => !this.isDeleted() && !this.isViewMode(),
                disabled: () => !this.hasSavePermission() || this.readOnly,
                ref: el => this.applyChangesButton = Fluent(el)
            }),
            deleteToolButton({
                onClick: () => confirmDialog(localText('Controls.EntityDialog.DeleteConfirmation'),
                    () => this.doDelete(() => this.dialogClose("delete"))),
                visible: () => this.isEditMode() && !this.isDeleted() && !this.isViewMode(),
                disabled: () => !this.hasDeletePermission() || this.readOnly,
                ref: el => this.deleteButton = Fluent(el)
            }),
            undeleteToolButton({
                onClick: () => this.isDeleted() && confirmDialog(localText('Controls.EntityDialog.UndeleteConfirmation'), () =>
                    this.undelete(() => this.loadById(this.entityId))),
                visible: () => this.isEditMode() && this.isDeleted() && !this.isViewMode(),
                disabled: () => !this.hasDeletePermission() || this.readOnly,
                ref: el => this.undeleteButton = Fluent(el)
            }),
            editToolButton({
                onClick: () => {
                    if (!this.isEditMode())
                        return;
                    this.editClicked = true;
                    this.updateInterface();
                    this.updateTitle();
                },
                visible: () => this.isViewMode(),
                disabled: () => !this.hasSavePermission() || this.readOnly,
                ref: el => this.editButton = Fluent(el)
            }),
            localizationToolButton({
                onClick: () => this.localizer?.buttonClick(),
                ref: el => this.localizerButton = Fluent(el)
            }),
            cloneToolButton({
                onClick: () => {
                    if (!this.isEditMode())
                        return;
                    var cloneEntity = this.getCloningEntity();
                    var cloneDialog = Widget.create({ type: getInstanceType(this) })
                    SubDialogHelper.bubbleDataChange(SubDialogHelper.cascade(cloneDialog, this.domNode), this, true);
                    (cloneDialog as typeof this).loadEntityAndOpenDialog(cloneEntity, null);
                },
                visible: () => false,
                disabled: () => !this.hasInsertPermission() || this.readOnly,
                ref: el => this.cloneButton = Fluent(el)
            })
        ];
    }

    protected getCloningEntity(): TItem {

        var clone: any = new Object();
        clone = extend(clone, this.entity);

        var idField = this.getIdProperty();
        if (idField) {
            delete clone[idField];
        }

        var isActiveField = this.getIsActiveProperty();
        if (isActiveField) {
            delete clone[isActiveField];
        }

        var isDeletedField = this.getIsDeletedProperty();
        if (isDeletedField) {
            delete clone[isDeletedField];
        }

        return clone;
    }

    protected updateInterface(): void {

        EditorUtils.setContainerReadOnly(this.byId('Form'), false);

        var hasSavePermission = this.hasSavePermission();
        var viewMode = this.isViewMode();
        var readOnly = this.readOnly;

        this.toolbar.updateInterface();

        TabsExtensions.setDisabled(this.tabs, 'Log', this.isNewOrDeleted());

        this.localizer?.updateInterface();

        if (!hasSavePermission || viewMode || readOnly)
            EditorUtils.setContainerReadOnly(this.byId("Form"), true);
    }

    protected getUndeleteOptions(callback?: (response: UndeleteResponse) => void): ServiceOptions<UndeleteResponse> {
        return {}
    }

    protected undeleteHandler(options: ServiceOptions<UndeleteResponse>, callback: (response: UndeleteResponse) => void): void {
        serviceCall(options);
    }

    protected getUndeleteServiceMethod() {
        return this.getService() + '/Undelete';
    }

    protected undelete(callback?: (response: UndeleteResponse) => void): void {
        var baseOptions: ServiceOptions<UndeleteResponse> = {};
        baseOptions.service = this.getUndeleteServiceMethod();

        var request: UndeleteRequest = {};
        request.EntityId = this.entityId;

        baseOptions.request = request;
        baseOptions.onSuccess = response => {
            callback && callback(response);
            Fluent.trigger(this.domNode, "ondatachange", {
                entityId: this.entityId,
                entity: this.entity,
                operationType: 'undelete'
            });
        };

        var thisOptions = this.getUndeleteOptions(callback);
        var finalOptions = extend(baseOptions, thisOptions);
        this.undeleteHandler(finalOptions, callback);
    }

    declare private _readonly: boolean;

    public get readOnly(): boolean {
        return this.get_readOnly();
    }

    public set readOnly(value: boolean) {
        this.set_readOnly(value);
    }

    public get_readOnly() {
        return !!this._readonly;
    }

    public set_readOnly(value: boolean) {
        if (!!this._readonly != !!value) {
            this._readonly = !!value;
            this.updateInterface();
            this.updateTitle();
        }
    }

    protected getInsertPermission(): string {
        return this.getRowDefinition()?.insertPermission;
    }

    protected getUpdatePermission(): string {
        return this.getRowDefinition()?.updatePermission;
    }

    protected getDeletePermission(): string {
        return this.getRowDefinition()?.deletePermission;
    }

    protected hasDeletePermission() {
        var deletePermission = this.getDeletePermission();
        return deletePermission == null || Authorization.hasPermission(deletePermission);
    }

    protected hasInsertPermission() {
        var insertPermission = this.getInsertPermission();
        return insertPermission == null || Authorization.hasPermission(insertPermission);
    }

    protected hasUpdatePermission() {
        var updatePermission = this.getUpdatePermission();
        return updatePermission == null || Authorization.hasPermission(updatePermission);
    }

    protected hasSavePermission(): boolean {
        return this.isNew() ? this.hasInsertPermission() : this.hasUpdatePermission();
    }

    declare protected editClicked: boolean;

    protected isViewMode() {
        return this.useViewMode() && this.isEditMode() && !this.editClicked;
    }

    protected useViewMode() {
        return false;
    }

    protected renderContents(): any {
        if (this.legacyTemplateRender())
            return void 0;

        const id = this.useIdPrefix();
        return (<>
            <div id={id.Toolbar} />
            <div class="s-Form">
                <form id={id.Form} action="">
                    <div id={id.PropertyGrid} />
                </form>
            </div>
        </>);
    }
}