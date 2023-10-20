import { any, Authorization, confirmDialog, DeleteRequest, DeleteResponse, endsWith, Exception, extend, format, getAttributes, getFormData, getFormDataAsync, getInstanceType, getTypeFullName, isEmptyOrNull, localText, LT, notifySuccess, PropertyItem, PropertyItemsData, replaceAll, RetrieveRequest, RetrieveResponse, safeCast, SaveRequest, SaveResponse, ScriptData, serviceCall, ServiceOptions, startsWith, tryGetText, UndeleteRequest, UndeleteResponse, validatorAbortHandler } from "../../q";
import { Decorators, EntityTypeAttribute, FormKeyAttribute, IdPropertyAttribute, IsActivePropertyAttribute, ItemNameAttribute, LocalTextPrefixAttribute, NamePropertyAttribute, ServiceAttribute } from "../../decorators";
import { IEditDialog, IReadOnly } from "../../interfaces";
import { IRowDefinition } from "../datagrid/irowdefinition";
import { EditorUtils } from "../editors/editorutils";
import { SubDialogHelper } from "../helpers/subdialoghelper";
import { TabsExtensions } from "../helpers/tabsextensions";
import { ValidationHelper } from "../helpers/validationhelper";
import { PropertyGrid, PropertyGridMode, PropertyGridOptions } from "../widgets/propertygrid";
import { Toolbar, ToolButton } from "../widgets/toolbar";
import { Widget } from "../widgets/widget";
import { TemplatedDialog } from "./templateddialog";

@Decorators.registerClass('Serenity.EntityDialog', [IEditDialog, IReadOnly])
export class EntityDialog<TItem, TOptions> extends TemplatedDialog<TOptions> implements IEditDialog, IReadOnly {

    protected entity: TItem;
    protected entityId: any;
    protected propertyItemsData: PropertyItemsData;
    protected propertyGrid: PropertyGrid;

    protected toolbar: Toolbar;
    protected saveAndCloseButton: JQuery;
    protected applyChangesButton: JQuery;
    protected deleteButton: JQuery;
    protected undeleteButton: JQuery;
    protected cloneButton: JQuery;
    protected editButton: JQuery;

    protected localizationGrid: PropertyGrid;
    protected localizationButton: JQuery;
    protected localizationPendingValue: any;
    protected localizationLastValue: any;

    static defaultLanguageList: () => string[][];

    constructor(opt?: TOptions) {
        super(opt);

        if (this.useAsync())
            this.initAsync();
        else 
            this.initSync();        
    }

    internalInit() {
        this.initPropertyGrid();
        this.initLocalizationGrid();
    }

    protected initSync() {
        this.propertyItemsData = this.getPropertyItemsData();
        this.internalInit();
        this.afterInit();
    }

    protected async initAsync() {
        this.propertyItemsData = await this.getPropertyItemsDataAsync();
        this.internalInit();
        this.afterInit();
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

        if (this.localizationGrid) {
            this.localizationGrid.destroy();
            this.localizationGrid = null;
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

    protected get_entity(): TItem {
        return this.entity;
    }

    protected set_entity(entity: any): void {
        this.entity = entity || new Object();
    }

    protected get_entityId(): any {
        return this.entityId;
    }

    protected set_entityId(value: any) {
        this.entityId = value;
    }

    protected getEntityNameFieldValue(): any {
        return ((this.get_entity() as any)[this.getNameProperty()] ?? '').toString();
    }

    protected getEntityTitle(): string {
        if (!this.isEditMode()) {
            return format(localText('Controls.EntityDialog.NewRecordTitle'), this.getEntitySingular());
        }
        else {
            var titleFormat = (this.isViewMode() || this.readOnly || !this.hasSavePermission()) ?
                localText('Controls.EntityDialog.ViewRecordTitle') : localText('Controls.EntityDialog.EditRecordTitle');
            var title = this.getEntityNameFieldValue() ?? '';
            return format(titleFormat,
                this.getEntitySingular(), (isEmptyOrNull(title) ? '' : (' (' + title + ')')));
        }
    }

    protected updateTitle(): void {
        this.dialogTitle = this.getEntityTitle();
    }

    protected isCloneMode(): boolean {
        return false;
    }

    protected isEditMode(): boolean {
        return this.get_entityId() != null && !this.isCloneMode();
    }

    protected isDeleted(): boolean {
        if (this.get_entityId() == null) {
            return false;
        }

        var isDeletedProperty = this.getIsDeletedProperty();
        if (isDeletedProperty) {
            return !!(this.get_entity() as any)[isDeletedProperty];
        }

        var value = (this.get_entity() as any)[this.getIsActiveProperty()];
        if (value == null) {
            return false;
        }

        return value < 0;
    }

    protected isNew(): boolean {
        return this.get_entityId() == null;
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

    protected doDelete(callback: (response: DeleteResponse) => void): void {
        var self = this;

        var request: DeleteRequest = {
            EntityId: this.get_entityId()
        };

        var baseOptions: ServiceOptions<DeleteResponse> = {
            service: this.getService() + '/Delete',
            request: request,
            onSuccess: response => {
                self.onDeleteSuccess(response);
                if (callback != null) {
                    callback(response);
                }
                self.element.triggerHandler('ondatachange', [{
                    entityId: request.EntityId,
                    entity: this.entity,
                    type: 'delete'
                }]);
            }
        };

        var thisOptions = this.getDeleteOptions(callback);
        var finalOptions = extend(baseOptions, thisOptions);

        this.deleteHandler(finalOptions, callback);
    }

    protected onDeleteSuccess(response: DeleteResponse): void {
    }

    protected attrs<TAttr>(attrType: { new(...args: any[]): TAttr }): TAttr[] {
        return getAttributes(getInstanceType(this), attrType, true);
    }

    protected getRowDefinition(): IRowDefinition {
        return null;
    }

    private _entityType: string;

    protected getEntityType(): string {

        if (this._entityType != null)
            return this._entityType;

        var typeAttributes = this.attrs(EntityTypeAttribute);

        if (typeAttributes.length === 1)
            return (this._entityType = typeAttributes[0].value);

        // remove global namespace
        var name = getTypeFullName(getInstanceType(this));
        var px = name.indexOf('.');
        if (px >= 0)
            name = name.substring(px + 1);

        // don't like this kind of convention, make it obsolete soon...
        if (endsWith(name, 'Dialog') || endsWith(name, 'Control'))
            name = name.substr(0, name.length - 6);
        else if (endsWith(name, 'Panel'))
            name = name.substr(0, name.length - 5);

        return (this._entityType = name);
    }

    private _formKey: string;

    protected getFormKey(): string {
        if (this._formKey != null)
            return this._formKey;

        var attributes = this.attrs(FormKeyAttribute);

        if (attributes.length >= 1)
            return (this._formKey = attributes[0].value);

        return (this._formKey = this.getEntityType());
    }

    private _localTextDbPrefix: string;

    protected getLocalTextDbPrefix(): string {
        if (this._localTextDbPrefix != null)
            return this._localTextDbPrefix;

        this._localTextDbPrefix = this.getLocalTextPrefix() ?? '';

        if (this._localTextDbPrefix.length > 0 && !endsWith(this._localTextDbPrefix, '.'))
            this._localTextDbPrefix = 'Db.' + this._localTextDbPrefix + '.';

        return this._localTextDbPrefix;
    }

    protected getLocalTextPrefix(): string {
        var rowDefinition = this.getRowDefinition();
        if (rowDefinition)
            return rowDefinition.localTextPrefix;

        var attr = this.attrs(LocalTextPrefixAttribute);

        if (attr.length >= 1)
            return attr[0].value;

        return this.getEntityType();
    }
   
    private _entitySingular: string;

    protected getEntitySingular(): string {
        if (this._entitySingular != null)
            return this._entitySingular;

        var attributes = this.attrs(ItemNameAttribute);

        if (attributes.length >= 1) {
            this._entitySingular = attributes[0].value;
            this._entitySingular = LT.getDefault(this._entitySingular, this._entitySingular);
        }
        else {
            var es = tryGetText(this.getLocalTextDbPrefix() + 'EntitySingular');
            if (es == null) 
                es = this.getEntityType();
            this._entitySingular = es;
        }

        return this._entitySingular;
    }

    private _nameProperty: string;

    protected getNameProperty(): string {
        if (this._nameProperty != null)
            return this._nameProperty;

        var rowDefinition = this.getRowDefinition();
        if (rowDefinition)
            return this._nameProperty = rowDefinition.nameProperty ?? '';

        var attributes = this.attrs(NamePropertyAttribute);

        if (attributes.length >= 1)
            return this._nameProperty = attributes[0].value ?? '';
            
        return this._nameProperty = 'Name';
    }

    private _idProperty: string;

    protected getIdProperty(): string {
        if (this._idProperty != null)
            return this._idProperty;

        var rowDefinition = this.getRowDefinition();
        if (rowDefinition)
            return this._idProperty = rowDefinition.idProperty ?? '';

        var attr = this.attrs(IdPropertyAttribute);
        if (attr.length === 1)
            return this._idProperty = attr[0].value ?? '';

        return this._idProperty = 'ID';
    }

    private _isActiveProperty: string;

    protected getIsActiveProperty(): string {
        if (this._isActiveProperty != null)
            return this._isActiveProperty;

        var rowDefinition = this.getRowDefinition();
        if (rowDefinition)
            return this._isActiveProperty = rowDefinition.isActiveProperty ?? '';

        var attr = this.attrs(IsActivePropertyAttribute);
        if (attr.length === 1)
            return this._isActiveProperty = attr[0].value ?? '';

        return this._isActiveProperty = '';
    }

    protected getIsDeletedProperty(): string {
        return this.getRowDefinition()?.isDeletedProperty;
    }

    private _service: string;

    protected getService() {
        if (this._service != null)
            return this._service;

        var attributes = this.attrs(ServiceAttribute);
        if (attributes.length >= 1)
            this._service = attributes[0].value;
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
        data = data || {};
        this.onLoadingData(data);
        var entity = data.Entity || new Object();
        this.beforeLoadEntity(entity);
        this.loadEntity(entity);
        this.set_entity(entity);
        this.afterLoadEntity();
    }

    protected loadEntity(entity: TItem): void {
        var idField = this.getIdProperty();

        if (idField != null)
            this.set_entityId((entity as any)[idField]);
        
        this.set_entity(entity);

        if (this.propertyGrid != null) {
            this.propertyGrid.set_mode((this.isEditMode() ?
                PropertyGridMode.update : PropertyGridMode.insert));
            this.propertyGrid.load(entity);
        }
    }

    protected beforeLoadEntity(entity: TItem): void {
        this.localizationPendingValue = null;
        this.localizationLastValue = null;
    }

    protected afterLoadEntity(): void {
        this.updateInterface();
        this.updateTitle();
    }

    public loadByIdAndOpenDialog(entityId: any, asPanel?: boolean) {
        this.loadById(entityId,
            response => window.setTimeout(() => this.dialogOpen(asPanel), 0),
            () => {
                if (!this.element.is(':visible')) {
                    this.element.remove();
                }
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
        this.loadById(this.get_entityId());
    }

    loadById(id: any, callback?: (response: RetrieveResponse<TItem>) => void, fail?: () => void) {
        var baseOptions: ServiceOptions<RetrieveResponse<TItem>> = {
            service: this.getService() + '/Retrieve',
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
        fail && request.fail(fail);
    }

    protected initLocalizationGrid(): void {
        var pgDiv = this.byId('PropertyGrid');
        if (pgDiv.length <= 0) {
            return;
        }
        var pgOptions = this.getPropertyGridOptions();
        this.initLocalizationGridCommon(pgOptions);
    }

    protected initLocalizationGridCommon(pgOptions: PropertyGridOptions) {
        var pgDiv = this.byId('PropertyGrid');

        if (!any(pgOptions.items, x => x.localizable === true))
            return;

        var localGridDiv = $('<div/>')
            .attr('id', this.idPrefix + 'LocalizationGrid')
            .hide().insertAfter(pgDiv);

        pgOptions.idPrefix = this.idPrefix + 'Localization_';

        var items: PropertyItem[] = [];
        for (var item1 of pgOptions.items) {
            var langs = null;

            if (item1.localizable === true) {
                var copy = extend({}, item1);
                copy.oneWay = true;
                copy.readOnly = true;
                copy.required = false;
                copy.defaultValue = null;
                items.push(copy);

                if (langs == null)
                    langs = this.getLangs();

                for (var lang of langs) {
                    copy = extend({}, item1);
                    copy.name = lang[0] + '$' + copy.name;
                    copy.title = lang[1];
                    copy.cssClass = [copy.cssClass, 'translation'].join(' ');
                    copy.insertable = true;
                    copy.updatable = true;
                    copy.oneWay = false;
                    copy.required = false;
                    copy.localizable = false;
                    copy.defaultValue = null;
                    items.push(copy);
                }
            }
        }

        pgOptions.items = items;

        this.localizationGrid = (new PropertyGrid(localGridDiv, pgOptions)).init(null);
        localGridDiv.addClass('s-LocalizationGrid');
    }

    protected isLocalizationMode(): boolean {
        return this.localizationButton != null && this.localizationButton.hasClass('pressed');
    }

    protected isLocalizationModeAndChanged(): boolean {
        if (!this.isLocalizationMode()) {
            return false;
        }

        var newValue = this.getLocalizationGridValue();
        return JSON.stringify(this.localizationLastValue) !=  JSON.stringify(newValue);
    }

    protected localizationButtonClick(): void {
        if (this.isLocalizationMode() && !this.validateForm()) {
            return;
        }

        if (this.isLocalizationModeAndChanged()) {
            var newValue = this.getLocalizationGridValue();
            this.localizationLastValue = newValue;
            this.localizationPendingValue = newValue;
        }

        this.localizationButton.toggleClass('pressed');
        this.updateInterface();
        if (this.isLocalizationMode()) {
            this.loadLocalization();
        }
    }

    protected getLanguages(): any[] {
        if (EntityDialog.defaultLanguageList != null)
            return EntityDialog.defaultLanguageList() || [];

        return [];
    }

    // for compatibility with older getLanguages methods written in Saltaralle
    private getLangs(): any {

        var langsTuple = this.getLanguages();
        var langs = safeCast(langsTuple, Array);
        if (langs == null || langs.length === 0 ||
            langs[0] == null || !Array.isArray(langs[0])) {
            langs = Array.prototype.slice.call(langsTuple.map(function (x: any) {
                return [x.item1, x.item2];
            }));
        }

        return langs;
    }

    protected loadLocalization(): void {
        if (this.localizationLastValue == null && this.isNew()) {
            this.localizationGrid.load({});
            this.setLocalizationGridCurrentValues();
            this.localizationLastValue = this.getLocalizationGridValue();
            return;
        }

        if (this.localizationLastValue != null) {
            this.localizationGrid.load(this.localizationLastValue);
            this.setLocalizationGridCurrentValues();
            return;
        }

        var opt = <ServiceOptions<any>>{
            service: this.getService() + '/Retrieve',
            blockUI: true,
            request: {
                EntityId: this.get_entityId(),
                ColumnSelection: 'keyOnly',
                IncludeColumns: ['Localizations']
            },
            onSuccess: response => {
                var copy = extend(new Object(), this.get_entity());
                if (response.Localizations) {
                    for (var language of Object.keys(response.Localizations)) {
                        var entity = response.Localizations[language];
                        for (var key of Object.keys(entity)) {
                            (copy as any)[language + '$' + key] = entity[key];
                        }
                    }
                }
                
                this.localizationGrid.load(copy);
                this.setLocalizationGridCurrentValues();
                this.localizationPendingValue = null;
                this.localizationLastValue = this.getLocalizationGridValue();
            }
        };

        serviceCall(opt);
    }

    protected setLocalizationGridCurrentValues(): void {
        var valueByName: Record<string, any> = {};

        this.localizationGrid.enumerateItems((item, widget) => {
            if (item.name.indexOf('$') < 0 && widget.element.is(':input')) {
                valueByName[item.name] = this.byId(item.name).val();
                widget.element.val(valueByName[item.name]);
            }
        });

        this.localizationGrid.enumerateItems((item1, widget1) => {
            var idx = item1.name.indexOf('$');
            if (idx >= 0 && widget1.element.is(':input')) {
                var hint = valueByName[item1.name.substr(idx + 1)];
                if (hint != null && hint.length > 0) {
                    widget1.element.attr('title', hint).attr('placeholder', hint);
                }
            }
        });
    }

    protected getLocalizationGridValue(): any {
        var value: any = {};
        this.localizationGrid.save(value);

        for (var k of Object.keys(value)) {
            if (k.indexOf('$') < 0) {
                delete value[k];
            }
        }

        return value;
    }

    protected getPendingLocalizations(): any {
        if (this.localizationPendingValue == null) {
            return null;
        }

        var result: { [key: string]: any } = {};
        var idField = this.getIdProperty();
        var langs = this.getLangs();

        for (var pair of langs) {
            var language = pair[0];
            var entity: any = {};

            if (idField != null) {
                entity[idField] = this.get_entityId();
            }

            var prefix = language + '$';

            for (var k of Object.keys(this.localizationPendingValue)) {
                if (startsWith(k, prefix)) 
                    entity[k.substr(prefix.length)] = this.localizationPendingValue[k];
            }

            result[language] = entity;
        }

        return result;
    }

    protected initPropertyGrid(): void {
        var pgDiv = this.byId('PropertyGrid');
        if (pgDiv.length <= 0) {
            return;
        }
        var pgOptions = this.getPropertyGridOptions();
        this.propertyGrid = (new PropertyGrid(pgDiv, pgOptions)).init(null);
        if (this.element.closest('.ui-dialog').hasClass('s-Flexify')) {
            this.propertyGrid.element.children('.categories').flexHeightOnly(1);
        }
    }

    protected getPropertyItems() {
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

        if (!isEmptyOrNull(formKey)) {
            return getFormData(formKey);
        }

        return { items: [], additionalItems: [] };
    }

    protected async getPropertyItemsDataAsync(): Promise<PropertyItemsData> {
        var formKey = this.getFormKey();
        if (!isEmptyOrNull(formKey)) {
            return await getFormDataAsync(formKey);
        }

        return { items: [], additionalItems: [] };
    }    

    protected getPropertyGridOptions(): PropertyGridOptions {
        return {
            idPrefix: this.idPrefix,
            items: this.getPropertyItems(),
            mode: PropertyGridMode.insert,
            localTextPrefix: 'Forms.' + this.getFormKey() + '.',
            useCategories: true
        };
    }

    protected validateBeforeSave(): boolean {
        return true;
    }

    protected getSaveOptions(callback: (response: SaveResponse) => void): ServiceOptions<SaveResponse> {

        var opt: ServiceOptions<SaveResponse> = {};

        opt.service = this.getService() + '/' + (this.isEditMode() ? 'Update' : 'Create'),

        opt.onSuccess = response => {
            this.onSaveSuccess(response);

            callback && callback(response);

            var typ = (this.isEditMode() ? 'update' : 'create');

            var ent = opt.request == null ? null : opt.request.Entity;
            var eid: any = this.isEditMode() ? this.get_entityId() :
                (response == null ? null : response.EntityId);

            var dci = {
                type: typ,
                entity: ent,
                entityId: eid
            };

            this.element.triggerHandler('ondatachange', [dci]);
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
                (entity as any)[idField] = this.get_entityId();
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
                req.EntityId = this.get_entityId();
            }
        }

        if (this.localizationPendingValue != null) {
            req.Localizations = this.getPendingLocalizations();
        }

        return req;
    }

    protected onSaveSuccess(response: SaveResponse): void {
    }

    protected save_submitHandler(callback: (response: SaveResponse) => void): void {
        var options = this.getSaveOptions(callback);
        this.saveHandler(options, callback);
    }

    protected save(callback?: (response: SaveResponse) => void): void | boolean {
        return ValidationHelper.submit(this.byId('Form'),
            () => this.validateBeforeSave(),
            () => this.save_submitHandler(callback));
    }

    protected saveHandler(options: ServiceOptions<SaveResponse>,
        callback: (response: SaveResponse) => void): void {
        serviceCall(options);
    }

    protected initToolbar(): void {

        super.initToolbar();

        if (!this.toolbar)
            return;

        this.saveAndCloseButton = this.toolbar.findButton('save-and-close-button');
        this.applyChangesButton = this.toolbar.findButton('apply-changes-button');
        this.deleteButton = this.toolbar.findButton('delete-button');
        this.undeleteButton = this.toolbar.findButton('undo-delete-button');
        this.editButton = this.toolbar.findButton('edit-button');
        this.cloneButton = this.toolbar.findButton('clone-button');
        this.localizationButton = this.toolbar.findButton('localization-button');
    }

    protected showSaveSuccessMessage(response: SaveResponse): void {
        notifySuccess(localText('Controls.EntityDialog.SaveSuccessMessage'), '', null);
    }

    protected getToolbarButtons(): ToolButton[] {
        var list: ToolButton[] = [];

        list.push({
            title: localText('Controls.EntityDialog.SaveButton'),
            action: 'save-and-close',
            cssClass: 'save-and-close-button',
            icon: 'fa-check-circle text-purple',
            hotkey: 'alt+s',
            onClick: () => {
                this.save(response => {
                    this.dialogClose();
                });
            },
            visible: () => !this.isDeleted() && !this.isViewMode(),
            disabled: () => !this.hasSavePermission() || this.readOnly
        });

        list.push({
            title: '',
            hint: localText('Controls.EntityDialog.ApplyChangesButton'),
            action: 'apply-changes',
            cssClass: 'apply-changes-button',
            icon: 'fa-clipboard-check text-purple',
            hotkey: 'alt+a',
            onClick: () => {
                this.save(response1 => {

                    if (this.isEditMode()) {
                        var id1 = response1.EntityId;
                        if (id1 == null) {
                            id1 = this.get_entityId();
                        }
                        this.loadById(id1);
                    }
                    else {
                        this.loadById(response1.EntityId);
                    }

                    this.showSaveSuccessMessage(response1);
                });
            },
            visible: () => !this.isDeleted() && !this.isViewMode(),
            disabled: () => !this.hasSavePermission() || this.readOnly
        });

        list.push({
            title: localText('Controls.EntityDialog.DeleteButton'),
            cssClass: 'delete-button',
            icon: 'fa-trash-o text-red',
            hotkey: 'alt+x',
            onClick: () => {
                confirmDialog(localText('Controls.EntityDialog.DeleteConfirmation'), () => {
                    this.doDelete(() => this.dialogClose());
                });
            },
            visible: () => this.isEditMode() && !this.isDeleted() && !this.isViewMode(),
            disabled: () => !this.hasDeletePermission() || this.readOnly
        });

        list.push({
            title: localText('Controls.EntityDialog.UndeleteButton'),
            action: 'undo-delete',
            cssClass: 'undo-delete-button',
            onClick: () => {
                if (this.isDeleted()) {
                    confirmDialog(localText('Controls.EntityDialog.UndeleteConfirmation'), () => {
                        this.undelete(() => this.loadById(this.get_entityId()));
                    });
                }
            },
            visible: () => this.isEditMode() && this.isDeleted() && !this.isViewMode(),
            disabled: () => !this.hasDeletePermission() || this.readOnly
        });

        if (this.useViewMode()) {
            list.push({
                title: localText('Controls.EntityDialog.EditButton'),
                action: 'delete',
                cssClass: 'edit-button',
                icon: 'fa-edit',
                onClick: () => {
                    if (!this.isEditMode())
                        return;

                    this.editClicked = true;
                    this.updateInterface();
                    this.updateTitle();
                },
                visible: () => this.isViewMode(),
                disabled: () => !this.hasSavePermission() || this.readOnly
            });
        }

        list.push({
            title: localText('Controls.EntityDialog.LocalizationButton'),
            action: 'localization',
            cssClass: 'localization-button',
            onClick: () => this.localizationButtonClick()
        });

        list.push({
            title: localText('Controls.EntityDialog.CloneButton'),
            action: 'clone',
            cssClass: 'clone-button',
            icon: 'fa-clone',
            onClick: () => {

                if (!this.isEditMode()) {
                    return;
                }

                var cloneEntity = this.getCloningEntity();

                Widget.create({
                    type: getInstanceType(this),
                    init: w => SubDialogHelper.bubbleDataChange(
                        SubDialogHelper.cascade(w, this.element), this, true)
                        .loadEntityAndOpenDialog(cloneEntity, null)
                });
            },
            visible: () => false,
            disabled: () => !this.hasInsertPermission() || this.readOnly
        });

        return list;
    }

    protected getCloningEntity(): TItem {

        var clone: any = new Object();
        clone = extend(clone, this.get_entity());

        var idField = this.getIdProperty();
        if (!isEmptyOrNull(idField)) {
            delete clone[idField];
        }

        var isActiveField = this.getIsActiveProperty();
        if (!isEmptyOrNull(isActiveField)) {
            delete clone[isActiveField];
        }

        var isDeletedField = this.getIsDeletedProperty();
        if (!isEmptyOrNull(isDeletedField)) {
            delete clone[isDeletedField];
        }

        return clone;
    }

    protected updateInterface(): void {

        EditorUtils.setContainerReadOnly(this.byId('Form'), false);

        var isLocalizationMode = this.isLocalizationMode();
        var hasSavePermission = this.hasSavePermission();
        var viewMode = this.isViewMode();
        var readOnly = this.readOnly;

        this.toolbar.updateInterface();

        if (this.tabs != null) {
            TabsExtensions.setDisabled(this.tabs, 'Log', this.isNewOrDeleted());
        }

        if (this.propertyGrid != null) {
            this.propertyGrid.element.toggle(!isLocalizationMode);
        }

        if (this.localizationGrid != null) {
            this.localizationGrid.element.toggle(isLocalizationMode);
        }

        if (this.localizationButton != null) {
            this.localizationButton.toggle(this.localizationGrid != null);
            this.localizationButton.find('.button-inner')
                .text((this.isLocalizationMode() ?
                    localText('Controls.EntityDialog.LocalizationBack') :
                    localText('Controls.EntityDialog.LocalizationButton')));
        }

        if (isLocalizationMode) {

            if (this.toolbar != null)
                this.toolbar.findButton('tool-button')
                    .not('.localization-hidden')
                    .addClass('.localization-hidden').hide();

            this.localizationButton && this.localizationButton.show();

            return;
        }

        this.toolbar.findButton('localization-hidden')
            .removeClass('localization-hidden').show();

        if (!hasSavePermission || viewMode || readOnly)
            EditorUtils.setContainerReadOnly(this.byId("Form"), true);
    }

    protected getUndeleteOptions(callback?: (response: UndeleteResponse) => void): ServiceOptions<UndeleteResponse> {
        return {}
    }

    protected undeleteHandler(options: ServiceOptions<UndeleteResponse>, callback: (response: UndeleteResponse) => void): void {
        serviceCall(options);
    }

    protected undelete(callback?: (response: UndeleteResponse) => void): void {
        var baseOptions: ServiceOptions<UndeleteResponse> = {};
        baseOptions.service = this.getService() + '/Undelete';

        var request: UndeleteRequest = {};
        request.EntityId = this.get_entityId();

        baseOptions.request = request;
        baseOptions.onSuccess = response => {
            callback && callback(response);
            this.element.triggerHandler('ondatachange', [{
                entityId: this.get_entityId(),
                entity: this.entity,
                type: 'undelete'
            }]);
        };

        var thisOptions = this.getUndeleteOptions(callback);
        var finalOptions = extend(baseOptions, thisOptions);
        this.undeleteHandler(finalOptions, callback);
    }

    private _readonly: boolean;

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

    protected editClicked: boolean;

    protected isViewMode() {
        return this.useViewMode() && this.isEditMode() && !this.editClicked;
    }

    protected useViewMode() {
        return false;
    }

    protected getFallbackTemplate() {
        return `<div class="s-DialogContent">
    <div id="~_Toolbar" class="s-DialogToolbar">
    </div>
    <div class="s-Form">
        <form id="~_Form" action="">
            <div class="fieldset">
                <div id="~_PropertyGrid"></div>
                <div class="clear"></div>
            </div>
        </form> 
    </div>
</div>`;
    }
}