import { Authorization, DeleteRequest, DeleteResponse, Fluent, LanguageList, RetrieveColumnSelection, RetrieveRequest, RetrieveResponse, SaveRequest, SaveResponse, ServiceOptions, TranslationConfig, UndeleteRequest, UndeleteResponse, confirmDialog, getInstanceType, getTypeFullName, localText, notifySuccess, serviceCall, stringFormat, tryGetText, type PropertyItem, type PropertyItemsData } from "../../base";
import { Exception, ScriptData, ValidationHelper, extend, getFormData, getFormDataAsync, replaceAll, validatorAbortHandler } from "../../compat";
import { IEditDialog, IReadOnly } from "../../interfaces";
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
        this.propertyGrid?.destroy();
        delete this.propertyGrid;
        this.localizer?.destroy();
        delete this.localizer;
        delete this.toolbar;
        Object.keys(this).filter(k => Object.prototype.hasOwnProperty.call(this, k) && k.endsWith("Button")).forEach(k => delete (this as any)[k]);
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
        if (!this.isEditMode())
            return stringFormat(localText('Controls.EntityDialog.NewRecordTitle'), this.getEntitySingular());
        const titleFormat = (this.isViewMode() || this.readOnly || !this.hasSavePermission()) ?
            localText('Controls.EntityDialog.ViewRecordTitle') : localText('Controls.EntityDialog.EditRecordTitle');
        const title = this.getEntityNameFieldValue() ?? '';
        return stringFormat(titleFormat, this.getEntitySingular(), !title ? '' : (' (' + title + ')'));
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
        if (this.entityId == null)
            return false;

        const isDeletedProperty = this.getIsDeletedProperty();
        if (isDeletedProperty)
            return !!(this.entity as any)[isDeletedProperty];

        const value = (this.entity as any)[this.getIsActiveProperty()];
        if (value == null)
            return false;

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
        const request: DeleteRequest = {
            EntityId: this.entityId
        };

        const options: ServiceOptions<DeleteResponse> = extend({
            service: this.getDeleteServiceMethod(),
            request: request,
            onSuccess: response => {
                this.onDeleteSuccess(response);
                callback?.(response);
                Fluent.trigger(this.domNode, "ondatachange", {
                    entityId: request.EntityId,
                    entity: this.entity,
                    operationType: 'delete'
                } satisfies Partial<DataChangeInfo>);
            }
        }, this.getDeleteOptions(callback));

        this.deleteHandler(options, callback);
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

        const attr = this.getCustomAttribute(EntityTypeAttribute);
        if (attr)
            return (this._entityType = attr.value);

        let name = getTypeFullName(getInstanceType(this));
        const px = name.indexOf('.');
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

        const attr = this.getCustomAttribute(FormKeyAttribute);
        return this._formKey = attr ? attr.value : this.getEntityType();
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
        const rowDefinition = this.getRowDefinition();
        if (rowDefinition)
            return rowDefinition.localTextPrefix;
        const attr = this.getCustomAttribute(LocalTextPrefixAttribute);
        return attr ? attr.value : this.getEntityType();
    }

    declare private _entitySingular: string;

    protected getEntitySingular(): string {
        if (this._entitySingular != null)
            return this._entitySingular;

        const attr = this.getCustomAttribute(ItemNameAttribute);
        return (this._entitySingular = attr ? localText(attr.value, attr.value) :
            tryGetText(this.getLocalTextDbPrefix() + 'EntitySingular') ?? this.getEntityType());
    }

    declare private _nameProperty: string;

    protected getNameProperty(): string {
        if (this._nameProperty != null)
            return this._nameProperty;

        const rowDefinition = this.getRowDefinition();
        if (rowDefinition)
            return this._nameProperty = rowDefinition.nameProperty ?? '';

        const attr = this.getCustomAttribute(NamePropertyAttribute);
        return this._nameProperty = attr ? (attr.value ?? "") : 'Name';
    }

    declare private _idProperty: string;

    protected getIdProperty(): string {
        if (this._idProperty != null)
            return this._idProperty;

        const rowDefinition = this.getRowDefinition();
        if (rowDefinition)
            return this._idProperty = rowDefinition.idProperty ?? '';

        const attr = this.getCustomAttribute(IdPropertyAttribute);
        return this._idProperty = attr ? (attr.value ?? '') : 'ID';
    }

    declare private _isActiveProperty: string;

    protected getIsActiveProperty(): string {
        if (this._isActiveProperty != null)
            return this._isActiveProperty;

        const rowDefinition = this.getRowDefinition();
        return this._isActiveProperty = rowDefinition ? (rowDefinition.isActiveProperty ?? '') : this.getCustomAttribute(IsActivePropertyAttribute)?.value ?? '';
    }

    protected getIsDeletedProperty(): string {
        return this.getRowDefinition()?.isDeletedProperty;
    }

    declare private _service: string;

    protected getService() {
        if (this._service != null)
            return this._service;

        const attr = this.getCustomAttribute(ServiceAttribute);
        return this._service = attr ? attr.value : replaceAll(this.getEntityType(), '.', '/');
    }

    load(entityOrId: any, done: () => void, fail?: (ex: Exception) => void): void {

        const action = () => {

            if (entityOrId == null) {
                this.loadResponse({});
                done?.();
                return;
            }

            const scriptType = typeof (entityOrId);
            if (scriptType === 'string' || scriptType === 'number') {
                this.loadById(entityOrId, () => {
                    done && window.setTimeout(done, 0);
                }, null);
                return;
            }

            this.loadResponse({ Entity: entityOrId || new Object() });
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
            const ex = (Exception as any).wrap(ex1);
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
        const entity = data.Entity || new Object();
        this.beforeLoadEntity(entity);
        this.loadEntity(entity);
        this.entity = entity;
        this.afterLoadEntity();
    }

    protected loadEntity(entity: TItem): void {
        const idField = this.getIdProperty();
        if (idField != null)
            this.entityId = ((entity as any)[idField]);

        this.entity = entity;
        this.propertyGrid?.set_mode((this.isEditMode() ? PropertyGridMode.update : PropertyGridMode.insert));
        this.propertyGrid?.load(entity);
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
        return {
            blockUI: true,
            service: this.getRetrieveServiceMethod(),
            request: this.getLoadByIdRequest(id),
            onSuccess: response => {
                this.loadResponse(response);
                callback?.(response);
            },
            onCleanup: () => this.validator != null && validatorAbortHandler(this.validator)
        };
    }

    protected getLoadByIdRequest(id: any): RetrieveRequest {
        return {
            EntityId: id
        };
    }

    protected reloadById(): void {
        this.loadById(this.entityId);
    }

    protected getRetrieveServiceMethod() {
        return this.getService() + '/Retrieve';
    }

    loadById(id: any, callback?: (response: RetrieveResponse<TItem>) => void, fail?: () => void) {
        this.loadByIdHandler(this.getLoadByIdOptions(id, callback), callback, fail);
    }

    protected loadByIdHandler(options: ServiceOptions<RetrieveResponse<TItem>>, callback: (response: RetrieveResponse<TItem>) => void, fail: () => void): void {
        const request = serviceCall(options);
        fail && ((request as any)?.fail ? (request as any).fail(fail) : request.then(null, fail));
    }

    protected async retrieveLocalizations(): Promise<Record<string, Partial<TItem>>> {
        const opt: ServiceOptions<RetrieveResponse<TItem>> = {
            ...this.getLoadByIdOptions(this.entityId, null),
            onSuccess: void 0,
            onCleanup: void 0
        };

        opt.request = {
            ...opt.request,
            ColumnSelection: RetrieveColumnSelection.keyOnly,
            IncludeColumns: ['Localizations']
        };
        return (await serviceCall(opt)).Localizations;
    }

    protected getLocalizerOptions(): EntityLocalizerOptions {
        return {
            byId: id => this.byId(id),
            idPrefix: this.idPrefix,
            getButton: () => this.localizerButton,
            getEntity: () => this.entity,
            getLanguages: () => this.getLanguages(),
            getPropertyGrid: () => this.byId("PropertyGrid"),
            getToolButtons: () => this.toolbar.element.findAll(".tool-button"),
            isNew: () => this.isNew(),
            pgOptions: this.getPropertyGridOptions(),
            retrieveLocalizations: () => this.retrieveLocalizations(),
            validateForm: () => this.validateForm()
        };
    }

    protected initLocalizer(): void {
        if (this.findById('PropertyGrid'))
            this.localizer = new EntityLocalizer(this.getLocalizerOptions());
    }

    protected getLanguages(): LanguageList {
        return TranslationConfig.getLanguageList?.() || [];
    }

    protected initPropertyGrid(): void {
        const pgDiv = this.findById('PropertyGrid');
        if (!pgDiv)
            return;
        const pgOptions = this.getPropertyGridOptions();
        this.propertyGrid = (new PropertyGrid({ element: pgDiv, ...pgOptions })).init();
    }

    protected getPropertyItems(): PropertyItem[] {
        return this.propertyItemsData?.items || [];
    }

    protected getPropertyItemsData(): PropertyItemsData {
        const formKey = this.getFormKey();

        if (this.getFormKey === EntityDialog.prototype.getFormKey &&
            this.getPropertyItems !== EntityDialog.prototype.getPropertyItems &&
            !ScriptData.canLoad('Form.' + formKey)) {
            return {
                items: this.getPropertyItems(),
                additionalItems: []
            }
        }

        return formKey ? getFormData(formKey) : { items: [], additionalItems: [] };
    }

    protected async getPropertyItemsDataAsync(): Promise<PropertyItemsData> {
        const formKey = this.getFormKey();
        return formKey ? await getFormDataAsync(formKey) : { items: [], additionalItems: [] };
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
        const opt: ServiceOptions<SaveResponse> = {};
        opt.service = this.isEditMode() ? this.getUpdateServiceMethod() : this.getCreateServiceMethod();
        opt.onSuccess = response => {
            this.onSaveSuccess(response, initiator);
            callback?.(response);
            Fluent.trigger(this.domNode, "ondatachange", {
                operationType: this.isEditMode() ? 'update' : 'create',
                entity: opt.request == null ? null : opt.request.Entity,
                entityId: this.isEditMode() ? this.entityId : (response == null ? null : response.EntityId)
            } satisfies Partial<DataChangeInfo>);
        };
        opt.onCleanup = () => this.validator && validatorAbortHandler(this.validator);
        opt.request = this.getSaveRequest();
        return opt;
    }

    protected getSaveEntity(): TItem {
        const entity: TItem = new Object() as any;
        this.propertyGrid?.save(entity);

        if (this.isEditMode()) {
            const idField = this.getIdProperty();
            if (idField != null && (entity as any)[idField] == null)
                (entity as any)[idField] = this.entityId;
        }

        return entity;
    }

    protected getSaveRequest(): SaveRequest<TItem> {
        const entity = this.getSaveEntity();
        const req: SaveRequest<TItem> = { Entity: entity };

        if (this.isEditMode() && this.getIdProperty() != null)
            req.EntityId = this.entityId;

        this.localizer?.editSaveRequest(req);
        return req;
    }

    protected onSaveSuccess(response: SaveResponse, initiator?: SaveInitiator): void {
        initiator !== "save-and-close" && this.showSaveSuccessMessage(response, initiator);
    }

    protected save_submitHandler(callback: (response: SaveResponse) => void, initiator: SaveInitiator): void {
        const options = this.getSaveOptions(callback, initiator);
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
                visible: () => this.localizer?.isEnabled(),
                ref: el => this.localizerButton = Fluent(el)
            }),
            cloneToolButton({
                onClick: () => {
                    if (!this.isEditMode())
                        return;
                    const cloneEntity = this.getCloningEntity();
                    const cloneDialog = Widget.create({ type: getInstanceType(this) })
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

        const clone: any = extend(new Object(), this.entity);

        const idField = this.getIdProperty();
        if (idField)
            delete clone[idField];

        const isActiveField = this.getIsActiveProperty();
        if (isActiveField)
            delete clone[isActiveField];

        const isDeletedField = this.getIsDeletedProperty();
        if (isDeletedField)
            delete clone[isDeletedField];

        return clone;
    }

    protected updateInterface(): void {
        EditorUtils.setContainerReadOnly(this.byId('Form'), false);
        this.toolbar.updateInterface();
        TabsExtensions.setDisabled(this.tabs, 'Log', this.isNewOrDeleted());
        this.localizer?.updateInterface();
        if (!this.hasSavePermission() || this.isViewMode() || this.readOnly)
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
        const request: UndeleteRequest = { EntityId: this.entityId };
        const options: ServiceOptions<UndeleteResponse> = extend({
            service: this.getUndeleteServiceMethod(),
            request,
            onSuccess: response => {
                callback && callback(response);
                Fluent.trigger(this.domNode, "ondatachange", {
                    entityId: this.entityId,
                    entity: this.entity,
                    operationType: 'undelete'
                });
            }
        }, this.getUndeleteOptions(callback));

        this.undeleteHandler(options, callback);
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
        const deletePermission = this.getDeletePermission();
        return deletePermission == null || Authorization.hasPermission(deletePermission);
    }

    protected hasInsertPermission() {
        const insertPermission = this.getInsertPermission();
        return insertPermission == null || Authorization.hasPermission(insertPermission);
    }

    protected hasUpdatePermission() {
        const updatePermission = this.getUpdatePermission();
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

    static get defaultLanguageList(): string[][] {
        return TranslationConfig.getLanguageList?.().map(x => [x.id, x.text]);
    }

    static set defaultLanguageList(value: string[][]) {
        TranslationConfig.getLanguageList = () => value?.map(x => ({ id: x[0], text: x[1] }));
    }
}