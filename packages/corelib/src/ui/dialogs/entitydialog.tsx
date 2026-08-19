import { Authorization, DeleteRequest, DeleteResponse, EntityDialogTexts, Fluent, LanguageList, RetrieveColumnSelection, RetrieveRequest, RetrieveResponse, SaveRequest, SaveResponse, ServiceOptions, TranslationConfig, UndeleteRequest, UndeleteResponse, confirmDialog, getInstanceType, getTypeFullName, localText, notifySuccess, nsSerenity, serviceCall, stringFormat, type PropertyItem, type PropertyItemsData } from "../../base";
import { ScriptData, ValidationHelper, getFormData, getFormDataAsync, replaceAll, validatorAbortHandler } from "../../compat";
import { IEditDialog, IReadOnly } from "../../interfaces";
import { DataChangeInfo } from "../../types";
import { Attributes } from "../../types/attributes";
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

/**
 * Base dialog for editing entities, integrating property grids, save/delete
 * operations, localization, and toolbar buttons.
 * @typeParam TItem - Entity row type.
 * @typeParam P - Widget props type.
 */
export class EntityDialog<TItem, P = {}> extends BaseDialog<P> implements IEditDialog, IReadOnly {

    static override[Symbol.typeInfo] = this.registerClass(nsSerenity, [IEditDialog, IReadOnly, Attributes.panel]);

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

    /**
     * Creates an entity dialog and loads property items.
     * @param props - Widget props forwarded to the base dialog.
     */
    constructor(props?: WidgetProps<P>) {
        super(props);

        this.syncOrAsyncThen(this.getPropertyItemsData, this.getPropertyItemsDataAsync, itemsData => {
            this.propertyItemsReady(itemsData);
            this.afterInit();
        });
    }

    /**
     * Called once property items are available; initializes the property grid and localizer.
     * @param itemsData - Property items data.
     */
    protected propertyItemsReady(itemsData: PropertyItemsData) {
        this.propertyItemsData = itemsData;
        this.initPropertyGrid();
        this.initLocalizer();
    }

    /**
     * Hook invoked after the dialog is initialized.
     */
    protected afterInit() {
    }

    /**
     * Whether property items should be loaded asynchronously.
     * @returns True when async loading is used.
     */
    protected useAsync() {
        return false;
    }

    /**
     * Cleans up the property grid, localizer, and toolbar buttons.
     */
    override destroy(): void {
        this.propertyGrid?.destroy();
        delete this.propertyGrid;
        this.localizer?.destroy();
        delete this.localizer;
        delete this.toolbar;
        Object.keys(this).filter(k => Object.prototype.hasOwnProperty.call(this, k) && k.endsWith("Button")).forEach(k => delete (this as any)[k]);
        super.destroy();
    }

    /**
     * Returns the current entity.
     * @returns The entity.
     */
    get entity(): TItem {
        return this._entity;
    }

    /** Sets the current entity. */
    protected set entity(value: TItem) {
        this._entity = value || new Object() as any;
    }

    /** @deprecated use entityId */
    protected get_entityId(): any {
        return this.entityId;
    }

    /**
     * Returns the current entity id.
     * @returns The entity id.
     */
    get entityId(): any {
        return this._entityId;
    }

    /** Sets the current entity id. */
    protected set entityId(value: any) {
        this._entityId = value;
    }

    /**
     * Returns the value of the entity name field.
     * @returns The name field value.
     */
    protected getEntityNameFieldValue(): any {
        return ((this.entity as any)[this.getNameProperty()] ?? '').toString();
    }

    /**
     * Returns the title for the dialog based on the current mode.
     * @returns The dialog title.
     */
    protected getEntityTitle(): string {
        if (!this.isEditMode())
            return stringFormat(EntityDialogTexts.NewRecordTitle, this.getEntitySingular());
        const titleFormat = (this.isViewMode() || this.readOnly || !this.hasSavePermission()) ?
            EntityDialogTexts.ViewRecordTitle : EntityDialogTexts.EditRecordTitle;
        const title = this.getEntityNameFieldValue() ?? '';
        return stringFormat(titleFormat, this.getEntitySingular(), !title ? '' : (' (' + title + ')'));
    }

    /**
     * Updates the dialog title from the entity.
     */
    protected updateTitle(): void {
        this.dialogTitle = this.getEntityTitle();
    }

    /**
     * Whether the dialog is in clone mode.
     * @returns True when cloning.
     */
    protected isCloneMode(): boolean {
        return false;
    }

    /**
     * Whether the dialog is editing an existing entity.
     * @returns True when editing.
     */
    protected isEditMode(): boolean {
        return this.entityId != null && !this.isCloneMode();
    }

    /**
     * Whether the current entity is soft-deleted.
     * @returns True when deleted.
     */
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

    /**
     * Whether the dialog is creating a new entity.
     * @returns True when new.
     */
    protected isNew(): boolean {
        return this.entityId == null;
    }

    /**
     * Whether the entity is new or soft-deleted.
     * @returns True when new or deleted.
     */
    protected isNewOrDeleted(): boolean {
        return this.isNew() || this.isDeleted();
    }

    /**
     * Returns the delete request for the current entity.
     * @returns The delete request.
     */
    protected getDeleteRequest(): DeleteRequest {
        return {
            EntityId: this.entityId
        };
    }

    /**
     * Returns the options for the delete service call.
     * @param callback - Callback invoked on success.
     * @returns Service options.
     */
    protected getDeleteOptions(callback: (response: DeleteResponse) => void): ServiceOptions<DeleteResponse> {
        const request = this.getDeleteRequest();
        return {
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
        }
    }

    /**
     * Executes the delete service call.
     * @param options - Service options.
     * @param callback - Callback invoked on success.
     * @returns A promise resolving to the delete response.
     */
    protected deleteHandler(options: ServiceOptions<DeleteResponse>, callback: (response: DeleteResponse) => void): PromiseLike<DeleteResponse> {
        return serviceCall(options);
    }

    /**
     * Returns the delete service method name.
     * @returns The service method.
     */
    protected getDeleteServiceMethod() {
        return this.getService() + '/Delete';
    }

    /**
     * Deletes the current entity.
     * @param callback - Callback invoked on success.
     * @returns A promise resolving to the delete response.
     */
    protected doDelete(callback: (response: DeleteResponse) => void): PromiseLike<DeleteResponse> {
        const options = this.getDeleteOptions(callback);
        return this.deleteHandler(options, callback);
    }

    /**
     * Hook invoked after a successful delete.
     * @param response - The delete response.
     */
    protected onDeleteSuccess(response: DeleteResponse): void {
    }

    /**
     * Returns the row definition for this dialog.
     * @returns The row definition, or null.
     */
    protected getRowDefinition(): IRowDefinition {
        return null;
    }

    declare private _entityType: string;

    /**
     * Returns the entity type name derived from the dialog class name.
     * @returns The entity type.
     */
    protected getEntityType(): string {

        if (this._entityType != null)
            return this._entityType;

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

    /**
     * Returns the form key used to load property items.
     * @returns The form key.
     */
    protected getFormKey(): string {
        if (this._formKey != null)
            return this._formKey;

        return this._formKey = this.getEntityType();
    }

    declare private _localTextDbPrefix: string;

    /**
     * Returns the local text database prefix for this dialog.
     * @returns The local text db prefix.
     */
    protected getLocalTextDbPrefix(): string {
        if (this._localTextDbPrefix != null)
            return this._localTextDbPrefix;

        this._localTextDbPrefix = this.getLocalTextPrefix() ?? '';

        if (this._localTextDbPrefix.length > 0 && !this._localTextDbPrefix.endsWith('.'))
            this._localTextDbPrefix = 'Db.' + this._localTextDbPrefix + '.';

        return this._localTextDbPrefix;
    }

    /**
     * Returns the local text prefix for this dialog.
     * @returns The local text prefix.
     */
    protected getLocalTextPrefix(): string {
        const rowDefinition = this.getRowDefinition();
        if (rowDefinition)
            return rowDefinition.localTextPrefix;
        return this.getEntityType();
    }

    declare private _entitySingular: string;

    /**
     * Returns the localized singular name for the entity.
     * @returns The entity singular name.
     */
    protected getEntitySingular(): string {
        if (this._entitySingular != null)
            return this._entitySingular;

        return this._entitySingular = localText(this.getLocalTextDbPrefix() + 'EntitySingular', this.getEntityType());
    }

    declare private _nameProperty: string;

    /**
     * Returns the name property for the entity.
     * @returns The name property name.
     */
    protected getNameProperty(): string {
        if (this._nameProperty != null)
            return this._nameProperty;

        const rowDefinition = this.getRowDefinition();
        if (rowDefinition)
            return this._nameProperty = rowDefinition.nameProperty ?? '';

        return this._nameProperty = 'Name';
    }

    declare private _idProperty: string;

    /**
     * Returns the id property for the entity.
     * @returns The id property name.
     */
    protected getIdProperty(): string {
        if (this._idProperty != null)
            return this._idProperty;

        const rowDefinition = this.getRowDefinition();
        if (rowDefinition)
            return this._idProperty = rowDefinition.idProperty ?? '';

        return this._idProperty = 'ID';
    }

    declare private _isActiveProperty: string;

    /**
     * Returns the is-active property for the entity.
     * @returns The is-active property name.
     */
    protected getIsActiveProperty(): string {
        if (this._isActiveProperty != null)
            return this._isActiveProperty;

        const rowDefinition = this.getRowDefinition();
        return this._isActiveProperty = rowDefinition ? (rowDefinition.isActiveProperty ?? '') : '';
    }

    protected getIsDeletedProperty(): string {
        return this.getRowDefinition()?.isDeletedProperty;
    }

    declare private _service: string;

    protected getService() {
        if (this._service != null)
            return this._service;

        return this._service = replaceAll(this.getEntityType(), '.', '/');
    }

    /**
     * Loads an entity or id into the dialog.
     * @param entityOrId - Entity instance or identifier to load.
     * @param done - Callback invoked when loading completes.
     * @param fail - Optional callback invoked on failure.
     * @returns A promise resolving to the retrieve response.
     */
    load(entityOrId: any, done: () => void, fail?: (ex: any) => void): PromiseLike<RetrieveResponse<TItem>> {

        const action = () => {

            if (entityOrId == null) {
                this.loadResponse({});
                done?.();
                return Promise.resolve<RetrieveResponse<TItem>>(null);
            }

            const scriptType = typeof (entityOrId);
            if (scriptType === 'string' || scriptType === 'number') {
                return this.loadById(entityOrId, () => {
                    done?.();
                }, null);
            }

            this.loadResponse({ Entity: entityOrId || new Object() });
            done?.();
            return Promise.resolve<RetrieveResponse<TItem>>(null);
        };

        if (fail == null) {
            return action();
        }

        try {
            return action();
        }
        catch (ex1) {
            fail(ex1);
            return Promise.reject(ex1);
        }
    }

    /**
     * Loads a new empty entity and opens the dialog.
     * @param asPanel - When true, opens as a panel.
     */
    loadNewAndOpenDialog(asPanel?: boolean): void {
        this.loadResponse({});
        this.dialogOpen(asPanel);
    }

    /**
     * Loads an entity and opens the dialog.
     * @param entity - The entity to load.
     * @param asPanel - When true, opens as a panel.
     */
    loadEntityAndOpenDialog(entity: TItem, asPanel?: boolean): void {
        this.loadResponse({ Entity: entity });
        this.dialogOpen(asPanel);
    }

    /**
     * Loads a retrieve response into the dialog.
     * @param data - The retrieve response data.
     */
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

    /**
     * Loads an entity into the property grid.
     * @param entity - The entity to load.
     */
    protected loadEntity(entity: TItem): void {
        const idField = this.getIdProperty();
        if (idField != null)
            this.entityId = ((entity as any)[idField]);

        this.entity = entity;
        this.propertyGrid?.set_mode((this.isEditMode() ? PropertyGridMode.update : PropertyGridMode.insert));
        this.propertyGrid?.load(entity);
    }

    /**
     * Hook invoked before loading an entity; clears localization state.
     * @param entity - The entity being loaded.
     */
    protected beforeLoadEntity(entity: TItem): void {
        this.localizer?.clearValue();
    }

    /**
     * Hook invoked after loading an entity; updates the interface and title.
     */
    protected afterLoadEntity(): void {
        this.updateInterface();
        this.updateTitle();
    }

    /**
     * Loads an entity by id and opens the dialog.
     * @param entityId - The entity id to load.
     * @param asPanel - When true, opens as a panel.
     * @param callback - Optional callback invoked on success.
     * @param fail - Optional callback invoked on failure.
     * @returns A promise resolving to the retrieve response.
     */
    public loadByIdAndOpenDialog(entityId: any, asPanel?: boolean, callback?: (response: RetrieveResponse<TItem>) => void, fail?: () => void): PromiseLike<RetrieveResponse<TItem>> {
        return this.loadById(entityId,
            response => {
                this.dialogOpen(asPanel);
                callback?.(response);
            },
            () => {
                if (!Fluent.isVisibleLike(this.domNode)) {
                    this.domNode.remove();
                }
                fail?.();
            });
    }

    /**
     * Hook invoked when data starts loading.
     * @param data - The retrieve response data.
     */
    protected onLoadingData(data: RetrieveResponse<TItem>): void {
    }

    /**
     * Returns the options for the retrieve service call.
     * @param id - The entity id to load.
     * @param callback - Callback invoked on success.
     * @returns Service options.
     */
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

    /**
     * Returns the retrieve request for an entity id.
     * @param id - The entity id.
     * @returns The retrieve request.
     */
    protected getLoadByIdRequest(id: any): RetrieveRequest {
        return {
            EntityId: id
        };
    }

    /**
     * Reloads the current entity by id.
     */
    protected reloadById(): void {
        this.loadById(this.entityId);
    }

    /**
     * Returns the retrieve service method name.
     * @returns The service method.
     */
    protected getRetrieveServiceMethod() {
        return this.getService() + '/Retrieve';
    }

    /**
     * Loads an entity by id.
     * @param id - The entity id.
     * @param callback - Optional callback invoked on success.
     * @param fail - Optional callback invoked on failure.
     * @returns A promise resolving to the retrieve response.
     */
    loadById(id: any, callback?: (response: RetrieveResponse<TItem>) => void, fail?: () => void): PromiseLike<RetrieveResponse<TItem>> {
        return this.loadByIdHandler(this.getLoadByIdOptions(id, callback), callback, fail);
    }

    /**
     * Executes the retrieve service call.
     * @param options - Service options.
     * @param callback - Callback invoked on success.
     * @param fail - Callback invoked on failure.
     * @returns A promise resolving to the retrieve response.
     */
    protected loadByIdHandler(options: ServiceOptions<RetrieveResponse<TItem>>, callback: (response: RetrieveResponse<TItem>) => void, fail: () => void): PromiseLike<RetrieveResponse<TItem>> {
        const response = serviceCall<RetrieveResponse<TItem>>(options);
        fail && ((response as any)?.fail ? (response as any).fail(fail) : response.then(null, fail));
        return response;
    }

    /**
     * Retrieves existing localizations for the current entity.
     * @returns A promise resolving to the localizations keyed by language.
     */
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

    /**
     * Returns the options for the entity localizer.
     * @returns Localizer options.
     */
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

    /**
     * Initializes the entity localizer.
     */
    protected initLocalizer(): void {
        if (this.findById('PropertyGrid'))
            this.localizer = new EntityLocalizer(this.getLocalizerOptions());
    }

    /**
     * Returns the list of languages for localization.
     * @returns The language list.
     */
    protected getLanguages(): LanguageList {
        return TranslationConfig.getLanguageList?.() || [];
    }

    /**
     * Initializes the property grid from the PropertyGrid element.
     */
    protected initPropertyGrid(): void {
        const pgDiv = this.findById('PropertyGrid');
        if (!pgDiv)
            return;
        const pgOptions = this.getPropertyGridOptions();
        this.propertyGrid = (new PropertyGrid({ element: pgDiv, ...pgOptions })).init();
    }

    /**
     * Returns the property items for this dialog.
     * @returns The property items.
     */
    protected getPropertyItems(): PropertyItem[] {
        return this.propertyItemsData?.items || [];
    }

    /**
     * Loads the property items data, either from script data or local items.
     * @returns The property items data.
     */
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

    /**
     * Asynchronously loads the property items data.
     * @returns A promise resolving to the property items data.
     */
    protected async getPropertyItemsDataAsync(): Promise<PropertyItemsData> {
        const formKey = this.getFormKey();
        return formKey ? await getFormDataAsync(formKey) : { items: [], additionalItems: [] };
    }

    /**
     * Returns the options for the property grid.
     * @returns Property grid options.
     */
    protected getPropertyGridOptions(): PropertyGridOptions {
        return {
            idPrefix: this.idPrefix,
            items: this.getPropertyItems(),
            mode: PropertyGridMode.insert,
            localTextPrefix: 'Forms.' + this.getFormKey() + '.'
        };
    }

    /**
     * Commits pending edits in the property grid.
     * @returns True when the commit succeeds.
     */
    protected async commitEdits(): Promise<boolean> {
        if (this.propertyGrid && (await this.propertyGrid.commitEdits()) === false) {
            return false;
        }

        return true;
    }

    /**
     * Validates the form before saving.
     * @returns True when the form is valid.
     */
    protected validateBeforeSave(): boolean {
        return true;
    }

    /**
     * Returns the create service method name.
     * @returns The service method.
     */
    protected getCreateServiceMethod() {
        return this.getService() + '/Create';
    }

    /**
     * Returns the update service method name.
     * @returns The service method.
     */
    protected getUpdateServiceMethod() {
        return this.getService() + '/Update';
    }

    /**
     * Returns the options for the save service call.
     * @param callback - Callback invoked on success.
     * @param initiator - How the save was initiated.
     * @returns Service options.
     */
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

    /**
     * Returns the entity populated from the property grid.
     * @returns The saved entity.
     */
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

    /**
     * Returns the save request for the current entity.
     * @returns The save request.
     */
    protected getSaveRequest(): SaveRequest<TItem> {
        const entity = this.getSaveEntity();
        const req: SaveRequest<TItem> = { Entity: entity };

        if (this.isEditMode() && this.getIdProperty() != null)
            req.EntityId = this.entityId;

        this.localizer?.editSaveRequest(req);
        return req;
    }

    /**
     * Hook invoked after a successful save.
     * @param response - The save response.
     * @param initiator - How the save was initiated.
     */
    protected onSaveSuccess(response: SaveResponse, initiator?: SaveInitiator): void {
        initiator !== "save-and-close" && this.showSaveSuccessMessage(response, initiator);
    }

    /**
     * Submits the save after validation.
     * @param callback - Callback invoked on success.
     * @param initiator - How the save was initiated.
     * @returns A promise resolving to the save response.
     */
    protected save_submitHandler(callback: (response: SaveResponse) => void, initiator: SaveInitiator): PromiseLike<SaveResponse> {
        const options = this.getSaveOptions(callback, initiator);
        return this.saveHandler(options, callback, initiator);
    }

    /**
     * Validates and saves the entity.
     * @param callback - Optional callback invoked on success.
     * @param initiator - How the save was initiated.
     * @returns A promise resolving to the save response, or false when validation fails.
     */
    protected save(callback?: (response: SaveResponse) => void, initiator?: SaveInitiator): PromiseLike<SaveResponse> | false {
        if (!ValidationHelper.submit(this.byId('Form'), () => this.validateBeforeSave(), null))
            return false;
        return this.save_submitHandler(callback, initiator);
    }

    /**
     * Executes the save service call.
     * @param options - Service options.
     * @param callback - Callback invoked on success.
     * @param initiator - How the save was initiated.
     * @returns A promise resolving to the save response.
     */
    protected saveHandler(options: ServiceOptions<SaveResponse>, callback: (response: SaveResponse) => void, initiator: SaveInitiator): PromiseLike<SaveResponse> {
        return serviceCall(options);
    }

    /**
     * Shows a success message after saving.
     * @param response - The save response.
     * @param initiator - How the save was initiated.
     */
    protected showSaveSuccessMessage(response: SaveResponse, initiator?: SaveInitiator): void {
        notifySuccess(EntityDialogTexts.SaveSuccessMessage, '', null);
    }

    /**
     * Returns the toolbar buttons for the entity dialog.
     * @returns Tool button definitions.
     */
    protected override getToolbarButtons(): ToolButton[] {
        return [
            saveAndCloseToolButton({
                onClick: async () => {
                    if (!(await this.commitEdits()))
                        return;
                    this.save(() => this.dialogClose("save-and-close"), "save-and-close")
                },
                visible: () => !this.isDeleted() && !this.isViewMode(),
                disabled: () => !this.hasSavePermission() || this.readOnly,
                ref: el => this.saveAndCloseButton = Fluent(el)
            }),
            applyChangesToolButton({
                onClick: async () => {
                    if (!(await this.commitEdits()))
                        return;
                    this.save(response => this.loadById(this.isEditMode() ? (response?.EntityId ?? this.entityId) : response?.EntityId), "apply-changes")
                },
                visible: () => !this.isDeleted() && !this.isViewMode(),
                disabled: () => !this.hasSavePermission() || this.readOnly,
                ref: el => this.applyChangesButton = Fluent(el)
            }),
            deleteToolButton({
                onClick: () => confirmDialog(EntityDialogTexts.DeleteConfirmation,
                    () => this.doDelete(() => this.dialogClose("delete"))),
                visible: () => this.isEditMode() && !this.isDeleted() && !this.isViewMode(),
                disabled: () => !this.hasDeletePermission() || this.readOnly,
                ref: el => this.deleteButton = Fluent(el)
            }),
            undeleteToolButton({
                onClick: () => this.isDeleted() && confirmDialog(EntityDialogTexts.UndeleteConfirmation, () =>
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

    /**
     * Returns a clone of the current entity with identity and state fields removed.
     * @returns The cloning entity.
     */
    protected getCloningEntity(): TItem {

        const clone: any = Object.assign(Object.create(null), this.entity);

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

    /**
     * Updates the interface to reflect the current mode and permissions.
     */
    protected updateInterface(): void {
        EditorUtils.setContainerReadOnly(this.byId('Form'), false);
        this.toolbar.updateInterface();
        TabsExtensions.setDisabled(this.tabs, 'Log', this.isNewOrDeleted());
        this.localizer?.updateInterface();
        if (!this.hasSavePermission() || this.isViewMode() || this.readOnly)
            EditorUtils.setContainerReadOnly(this.byId("Form"), true);
    }

    /**
     * Returns the undelete request for the current entity.
     * @returns The undelete request.
     */
    protected getUndeleteRequest(): UndeleteRequest {
        return { EntityId: this.entityId };
    }

    /**
     * Returns the options for the undelete service call.
     * @param callback - Optional callback invoked on success.
     * @returns Service options.
     */
    protected getUndeleteOptions(callback?: (response: UndeleteResponse) => void): ServiceOptions<UndeleteResponse> {
        const request = this.getUndeleteRequest();

        return {
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
        }
    }

    /**
     * Executes the undelete service call.
     * @param options - Service options.
     * @param callback - Optional callback invoked on success.
     * @returns A promise resolving to the undelete response.
     */
    protected undeleteHandler(options: ServiceOptions<UndeleteResponse>, callback?: (response: UndeleteResponse) => void): PromiseLike<UndeleteResponse> {
        return serviceCall(options);
    }

    /**
     * Returns the undelete service method name.
     * @returns The service method.
     */
    protected getUndeleteServiceMethod() {
        return this.getService() + '/Undelete';
    }

    /**
     * Undeletes the current entity.
     * @param callback - Optional callback invoked on success.
     * @returns Void or a promise resolving to the undelete response.
     */
    protected undelete(callback?: (response: UndeleteResponse) => void): void | PromiseLike<UndeleteResponse> {
        const options = this.getUndeleteOptions(callback);
        return this.undeleteHandler(options, callback);
    }

    declare private _readonly: boolean;

    /** Whether the dialog is in read-only mode. */
    public get readOnly(): boolean {
        return this.get_readOnly();
    }

    /** Sets whether the dialog is in read-only mode. */
    public set readOnly(value: boolean) {
        this.set_readOnly(value);
    }

    /**
     * Returns whether the dialog is in read-only mode.
     * @returns True when read-only.
     */
    public get_readOnly() {
        return !!this._readonly;
    }

    /**
     * Sets whether the dialog is in read-only mode and updates the interface.
     * @param value - True to enable read-only mode.
     */
    public set_readOnly(value: boolean) {
        if (!!this._readonly != !!value) {
            this._readonly = !!value;
            this.updateInterface();
            this.updateTitle();
        }
    }

    /**
     * Returns the insert permission for the entity.
     * @returns The insert permission, or undefined.
     */
    protected getInsertPermission(): string {
        return this.getRowDefinition()?.insertPermission;
    }

    /**
     * Returns the update permission for the entity.
     * @returns The update permission, or undefined.
     */
    protected getUpdatePermission(): string {
        return this.getRowDefinition()?.updatePermission;
    }

    /**
     * Returns the delete permission for the entity.
     * @returns The delete permission, or undefined.
     */
    protected getDeletePermission(): string {
        return this.getRowDefinition()?.deletePermission;
    }

    /**
     * Whether the current user has delete permission.
     * @returns True when permitted.
     */
    protected hasDeletePermission() {
        const deletePermission = this.getDeletePermission();
        return deletePermission == null || Authorization.hasPermission(deletePermission);
    }

    /**
     * Whether the current user has insert permission.
     * @returns True when permitted.
     */
    protected hasInsertPermission() {
        const insertPermission = this.getInsertPermission();
        return insertPermission == null || Authorization.hasPermission(insertPermission);
    }

    /**
     * Whether the current user has update permission.
     * @returns True when permitted.
     */
    protected hasUpdatePermission() {
        const updatePermission = this.getUpdatePermission();
        return updatePermission == null || Authorization.hasPermission(updatePermission);
    }

    /**
     * Whether the current user has save permission (insert or update).
     * @returns True when permitted.
     */
    protected hasSavePermission(): boolean {
        return this.isNew() ? this.hasInsertPermission() : this.hasUpdatePermission();
    }

    declare protected editClicked: boolean;

    /**
     * Whether the dialog is in view mode (read-only display of an existing entity).
     * @returns True when in view mode.
     */
    protected isViewMode() {
        return this.useViewMode() && this.isEditMode() && !this.editClicked;
    }

    /**
     * Whether view mode is enabled for this dialog.
     * @returns True when view mode is used.
     */
    protected useViewMode() {
        return false;
    }

    /**
     * Renders the dialog contents with toolbar, form, and property grid.
     * @returns The rendered content.
     */
    protected override renderContents(): any {
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

    /**
     * Returns the default language list for localization.
     * @returns The default language list.
     */
    static get defaultLanguageList(): string[][] {
        return TranslationConfig.getLanguageList?.().map(x => [x.id, x.text]);
    }

    /** Sets the default language list for localization. */
    static set defaultLanguageList(value: string[][]) {
        TranslationConfig.getLanguageList = () => value?.map(x => ({ id: x[0], text: x[1] }));
    }
}