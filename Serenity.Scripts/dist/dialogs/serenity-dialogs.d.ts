/// <reference types="jquery.validation" />
/// <reference types="jquery" />
/// <reference types="jqueryui" />
declare namespace Q.Router {
    let enabled: boolean;
    function navigate(hash: string, tryBack?: boolean, silent?: boolean): void;
    function replace(hash: string, tryBack?: boolean): void;
    function replaceLast(hash: string, tryBack?: boolean): void;
    function dialog(owner: JQuery, element: JQuery, hash: () => string): void;
    function resolve(hash?: string): void;
}
declare namespace Q {
    function autoFullHeight(element: JQuery): void;
    function initFullHeightGridPage(gridDiv: JQuery): void;
    function layoutFillHeightValue(element: JQuery): number;
    function layoutFillHeight(element: JQuery): void;
    function setMobileDeviceMode(): void;
    function triggerLayoutOnShow(element: JQuery): void;
    function centerDialog(el: JQuery): void;
}
declare namespace Serenity.DialogExtensions {
    function dialogResizable(dialog: JQuery, w?: any, h?: any, mw?: any, mh?: any): JQuery;
    function dialogMaximizable(dialog: JQuery): JQuery;
}
declare namespace Serenity {
    class TemplatedDialog<TOptions> extends TemplatedWidget<TOptions> {
        protected tabs: JQuery;
        protected toolbar: Serenity.Toolbar;
        protected validator: JQueryValidation.Validator;
        constructor(options?: TOptions);
        private get isMarkedAsPanel();
        private get isResponsive();
        private static getCssSize;
        private static applyCssSizes;
        destroy(): void;
        protected initDialog(): void;
        protected initToolbar(): void;
        protected getToolbarButtons(): ToolButton[];
        protected getValidatorOptions(): JQueryValidation.ValidationOptions;
        protected initValidator(): void;
        protected resetValidation(): void;
        protected validateForm(): boolean;
        dialogOpen(asPanel?: boolean): void;
        static openPanel(element: JQuery, uniqueName: string): void;
        static closePanel(element: JQuery, e?: JQueryEventObject): void;
        protected onDialogOpen(): void;
        protected arrange(): void;
        protected onDialogClose(): void;
        protected addCssClass(): void;
        protected getDialogOptions(): JQueryUI.DialogOptions;
        protected getDialogTitle(): string;
        dialogClose(): void;
        get dialogTitle(): string;
        private setupPanelTitle;
        set dialogTitle(value: string);
        set_dialogTitle(value: string): void;
        protected initTabs(): void;
        protected handleResponsive(): void;
    }
}
declare namespace Serenity {
    type DialogButton = JQueryUI.DialogButtonOptions;
    class PropertyDialog<TItem, TOptions> extends TemplatedDialog<TOptions> {
        protected _entity: TItem;
        protected _entityId: any;
        constructor(opt?: TOptions);
        destroy(): void;
        protected getDialogOptions(): JQueryUI.DialogOptions;
        protected getDialogButtons(): JQueryUI.DialogButtonOptions[];
        protected okClick(): void;
        protected okClickValidated(): void;
        protected cancelClick(): void;
        protected initPropertyGrid(): void;
        protected getFormKey(): string;
        protected getPropertyGridOptions(): PropertyGridOptions;
        protected getPropertyItems(): PropertyItem[];
        protected getSaveEntity(): TItem;
        protected loadInitialEntity(): void;
        protected get_entity(): TItem;
        protected set_entity(value: TItem): void;
        protected get_entityId(): any;
        protected set_entityId(value: any): void;
        protected validateBeforeSave(): boolean;
        protected updateTitle(): void;
        protected propertyGrid: Serenity.PropertyGrid;
    }
}
declare namespace Serenity {
    class EntityDialog<TItem, TOptions> extends TemplatedDialog<TOptions> implements IEditDialog, IReadOnly {
        protected entity: TItem;
        protected entityId: any;
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
        constructor(opt?: TOptions);
        destroy(): void;
        protected get_entity(): TItem;
        protected set_entity(entity: any): void;
        protected get_entityId(): any;
        protected set_entityId(value: any): void;
        protected getEntityNameFieldValue(): any;
        protected getEntityTitle(): string;
        protected updateTitle(): void;
        protected isCloneMode(): boolean;
        protected isEditMode(): boolean;
        protected isDeleted(): boolean;
        protected isNew(): boolean;
        protected isNewOrDeleted(): boolean;
        protected getDeleteOptions(callback: (response: DeleteResponse) => void): ServiceOptions<DeleteResponse>;
        protected deleteHandler(options: ServiceOptions<DeleteResponse>, callback: (response: DeleteResponse) => void): void;
        protected doDelete(callback: (response: DeleteResponse) => void): void;
        protected onDeleteSuccess(response: DeleteResponse): void;
        protected attrs<TAttr>(attrType: {
            new (...args: any[]): TAttr;
        }): TAttr[];
        private entityType;
        protected getEntityType(): string;
        private formKey;
        protected getFormKey(): string;
        private localTextDbPrefix;
        protected getLocalTextDbPrefix(): string;
        protected getLocalTextPrefix(): string;
        private entitySingular;
        protected getEntitySingular(): string;
        private nameProperty;
        protected getNameProperty(): string;
        private idProperty;
        protected getIdProperty(): string;
        protected isActiveProperty: string;
        protected getIsActiveProperty(): string;
        protected getIsDeletedProperty(): string;
        protected service: string;
        protected getService(): string;
        load(entityOrId: any, done: () => void, fail: (ex: Q.Exception) => void): void;
        loadNewAndOpenDialog(asPanel?: boolean): void;
        loadEntityAndOpenDialog(entity: TItem, asPanel?: boolean): void;
        protected loadResponse(data: any): void;
        protected loadEntity(entity: TItem): void;
        protected beforeLoadEntity(entity: TItem): void;
        protected afterLoadEntity(): void;
        loadByIdAndOpenDialog(entityId: any, asPanel?: boolean): void;
        protected onLoadingData(data: RetrieveResponse<TItem>): void;
        protected getLoadByIdOptions(id: any, callback: (response: RetrieveResponse<TItem>) => void): ServiceOptions<RetrieveResponse<TItem>>;
        protected getLoadByIdRequest(id: any): RetrieveRequest;
        protected reloadById(): void;
        loadById(id: any, callback?: (response: RetrieveResponse<TItem>) => void, fail?: () => void): void;
        protected loadByIdHandler(options: ServiceOptions<RetrieveResponse<TItem>>, callback: (response: RetrieveResponse<TItem>) => void, fail: () => void): void;
        protected initLocalizationGrid(): void;
        protected initLocalizationGridCommon(pgOptions: PropertyGridOptions): void;
        protected isLocalizationMode(): boolean;
        protected isLocalizationModeAndChanged(): boolean;
        protected localizationButtonClick(): void;
        protected getLanguages(): any[];
        private getLangs;
        protected loadLocalization(): void;
        protected setLocalizationGridCurrentValues(): void;
        protected getLocalizationGridValue(): any;
        protected getPendingLocalizations(): any;
        protected initPropertyGrid(): void;
        protected getPropertyItems(): PropertyItem[];
        protected getPropertyGridOptions(): PropertyGridOptions;
        protected validateBeforeSave(): boolean;
        protected getSaveOptions(callback: (response: SaveResponse) => void): ServiceOptions<SaveResponse>;
        protected getSaveEntity(): TItem;
        protected getSaveRequest(): SaveRequest<TItem>;
        protected onSaveSuccess(response: SaveResponse): void;
        protected save_submitHandler(callback: (response: SaveResponse) => void): void;
        protected save(callback?: (response: SaveResponse) => void): void | boolean;
        protected saveHandler(options: ServiceOptions<SaveResponse>, callback: (response: SaveResponse) => void): void;
        protected initToolbar(): void;
        protected showSaveSuccessMessage(response: SaveResponse): void;
        protected getToolbarButtons(): ToolButton[];
        protected getCloningEntity(): TItem;
        protected updateInterface(): void;
        protected getUndeleteOptions(callback?: (response: UndeleteResponse) => void): ServiceOptions<UndeleteResponse>;
        protected undeleteHandler(options: ServiceOptions<UndeleteResponse>, callback: (response: UndeleteResponse) => void): void;
        protected undelete(callback?: (response: UndeleteResponse) => void): void;
        private _readonly;
        get readOnly(): boolean;
        set readOnly(value: boolean);
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
        protected getInsertPermission(): string;
        protected getUpdatePermission(): string;
        protected getDeletePermission(): string;
        protected hasDeletePermission(): boolean;
        protected hasInsertPermission(): boolean;
        protected hasUpdatePermission(): boolean;
        protected hasSavePermission(): boolean;
        protected editClicked: boolean;
        protected isViewMode(): boolean;
        protected useViewMode(): boolean;
    }
}
