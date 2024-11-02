import { DataChangeInfo, Decorators, DeleteResponse, DialogType, EditorProps, EntityDialog, EntityGrid, IGetEditValue, ISetEditValue, PropertyItem, SaveRequest, SaveResponse, ServiceOptions, ServiceResponse, ToolButton, deepClone, getInstanceType, getTypeFullName, indexOf, serviceCall } from "@serenity-is/corelib";
import { GridEditorDialog } from "./GridEditorDialog";

@Decorators.registerClass("Serenity.Extensions.GridEditorBase", [IGetEditValue, ISetEditValue])
@Decorators.editor()
@Decorators.element("<div/>")
export abstract class GridEditorBase<TEntity, P = {}> extends EntityGrid<TEntity, P>
    implements IGetEditValue, ISetEditValue {

    /** Gets the id property name. Returns it from getRowDefinition() if available, or the default __id.
     * For connected mode, this should be the actual id property name of the entity, or getRowDefinition
     * should be implemented.
     */
    protected getIdProperty() { return this.getRowDefinition()?.idProperty ?? "__id"; }

    /**
     * Gets the dialog type to be used for editing entities. This method must be overridden in the grid editor class to return
     * the dialog type that extends GridEditorDialog<TEntity> and has the same TEntity type with the grid editor.
     */
    protected getDialogType(): DialogType | PromiseLike<DialogType> {
        throw new Error(`Please override getDialogType method in the grid editor class (${getTypeFullName(getInstanceType(this))}),` + 
            ` and return the correct dialog type which extends GridEditorDialog!`);
    }

    /** Next ID value for in memory editing mode */
    protected nextId = 1;

    constructor(props: EditorProps<P>) {
        super(props);
    }
    
    /**
     * Gets the id value of the entity. If the entity is null, returns null.
     * @param entity 
     * @deprecated Use itemId method instead
     */
    protected id(entity: TEntity) {
        return this.itemId(entity);
    }

    /**
     * Gets next id value for in-memory editing mode. It is a number prefixed with a backtick.
     */
    getNextId() {
        return "`" + this.nextId++;
    }

    /**
     * Sets the id value of the entity for in-memory editing mode.
     * @param entity Entity to set the id value
     */
    setNewId(entity: TEntity): TEntity {
        entity[this.getIdProperty()] = this.getNextId();
        return entity;
    }

    /**
     * This is called from the editor dialog's save handler to save the entity.
     * @param opt Save options
     * @param callback An optional callback to call after the entity is saved, usually same with the opt.onSuccess
     */
    protected async save(opt: ServiceOptions<any>, callback: (r: ServiceResponse) => void): Promise<void> {
        const request = opt.request as SaveRequest<TEntity>;
        let row = request.Entity;
        let id = this.itemId(row);

        if (this.connectedMode) {
            if (!(await this.validateEntity(row, request?.EntityId)))
                return;
    
            const response = await serviceCall(opt);
            id = response?.EntityId ?? id;
        }
        else {

            row = deepClone(row);

            if (id == null) {
                (row as any)[this.getIdProperty()] = this.getNextId();
            }

            if (!(await this.validateEntity(row, request?.EntityId)))
                return;

            const items = this.view.getItems().slice();
            if (id == null) {
                items.push(row);
            }
            else {
                const index = indexOf(items, x => this.itemId(x) === id);
                items[index] = row = deepClone({} as TEntity, items[index], row);
            }

            callback?.({});
        }

        this.element.trigger("change", { 
            operationType: id == null ? "create" : "update", 
            entityId: id,
            entity: row
        } satisfies Partial<DataChangeInfo>);
    }

    /**
     * This is called from the editor dialog's delete handler to delete the entity.
     * @param opt Delete service call options
     * @param callback An optional callback to call after the entity is deleted, usually same with the opt.onSuccess
     * @returns 
     */
    protected async delete(opt: ServiceOptions<any>, callback: (r: ServiceResponse) => void): Promise<void> {
        const id = opt?.request?.EntityId;
        const row = this.view.getItemById(id);

        if (!(await this.deleteEntity(id)))
            return;

        if (this.connectedMode) {
            await serviceCall(opt);
            this.element.trigger("change", { 
                operationType: "delete", 
                entityId: id,
                entity: row
            } satisfies Partial<DataChangeInfo>);
        }
        else {
            callback?.({});
        }

        this.element.trigger("change", { 
            operationType: "delete",
            entityId: id,
            entity: row
        } satisfies Partial<DataChangeInfo>);
    }

    protected deleteEntity(id: any): (boolean | Promise<boolean>) {
        if (!this.connectedMode)
            this.view.deleteItem(id);
        return true;
    }

    /** 
     * Called before saving an entity from the dialog. If the function returns false,
     * the entity will not be saved. Row is the entity to be saved and id is the id of the entity.
     * If the id is null, it is a new entity (insert mode), otherwise it is an existing entity (update mode).
     */
    protected validateEntity(row: TEntity, id: any): (boolean | Promise<boolean>) {
        return true;
    }

    /**
     * Gets a new entity instance to be used for creating new entities.
     */
    protected getNewEntity(): TEntity {
        return {} as TEntity;
    }

    protected getButtons(): ToolButton[] {
        const buttons = super.getButtons();
        const addButton = buttons.find(x => x.action === 'add');
        if (addButton) {
            addButton.onClick = () => {
                this.createEntityDialog(this.getItemType(), dlg => {
                    const dialog = this.checkDialogType(dlg);
                    this.transferDialogReadOnly(dialog);
                    dialog.onSave = (opt, callback) => this.save(opt, callback);
                    dialog.loadEntityAndOpenDialog(this.getNewEntity());
                });
            }
        }

        return buttons;
    }

    /**
     * This method is overridden to intercept the dialog creation and pass the handlers for save/delete operations.
     * @param entityOrId Entity or id of the entity to be edited
     */
    protected editItem(entityOrId: any): void {

        const scriptType = typeof (entityOrId);
        let id: any;
        const byId = scriptType === 'string' || scriptType === 'number';
        if (byId)
            id = entityOrId;
        else if (entityOrId != null)
            id = this.itemId(entityOrId);

        this.createEntityDialog(this.getItemType(), dlg => {
            const dialog = this.checkDialogType(dlg);
            this.transferDialogReadOnly(dialog);
            dialog.onDelete = this.delete.bind(this);
            dialog.onSave = this.save.bind(this);

            if (this.connectedMode) {
                if (byId) {
                    dialog.loadByIdAndOpenDialog(id);
                }
                else {
                    dialog.loadEntityAndOpenDialog(entityOrId);
                }
            }
            else {
                const item = (byId ? this.view.getItemById(id) : entityOrId) ?? entityOrId;
                dialog.loadEntityAndOpenDialog(item);
            }
        });
    }

    /**
     * Sets the editor value in target object. If connected mode is on the value is not set.
     * @param property Property item
     * @param target Target object
     */
    public getEditValue(property: PropertyItem, target: any) {
        if (!this.connectedMode)
            target[property.name] = this.value;
    }

    /**
     * Gets the editor value from source object. If connected mode is on the value is not read.
     */
    public setEditValue(source: any, property: PropertyItem) {
        if (!this.connectedMode)
            this.value = source[property.name];
    }

    /**
     * Gets the value of the grid editor. Unlike getEditValue, this method returns the actual array of entities
     * whether connected mode is on or off.
     */
    public get value(): TEntity[] {
        const p = this.getIdProperty();
        return this.view.getItems().map(x => {
            const y = deepClone(x);
            const id = y[p];
            if (id && id.toString().charAt(0) == '`')
                delete y[p];
            return y;
        });
    }

    /**
     * Sets the value of the grid editor. Unlike setEditValue, this method sets entities
     * whether connected mode is on or off.
     */
    public set value(value: TEntity[]) {
        const p = this.getIdProperty();
        this.view.setItems((value || []).map(x => {
            const y = deepClone(x);
            if ((y as any)[p] == null)
                (y as any)[p] = this.getNextId();
            return y;
        }), true);
    }

    /**
     * Returns true only in connected mode, otherwise false.
     */
    protected getGridCanLoad() {
        return super.getGridCanLoad() && !!this._connectedMode;
    }

    /**
     * As grid editor works in memory editing mode by default, it does not use pager.
     */
    protected usePager() {
        return false;
    }

    /**
     * No title by default for grid editors
     */
    protected getInitialTitle() {
        return null;
    }

    private _connectedMode = false;

    get connectedMode() {
        return this._connectedMode;
    }

    /**
     * Sets the connected mode of the grid editor. By default it is false, e.g. it is disconnected/in-memory editing mode.
     * Connected mode should only be enabled when the dialog containing grid editor is in edit mode, e.g. 
     * a master entity ID is available. In connected mode, the grid editor will load and save data from/to 
     * services directly, instead of in-memory editing, and validateEntity, deleteEntity, save, delete methods
     * will still be called but they should check the connectedMode property before trying to perform in-memory logic.
     * In the form, set [MinSelectLevel(SelectLevel.Explicit)] attribute to the grid editor property so that it is not
     * loaded by MasterDetailRelation behavior when the master dialog is in edit mode.
     */
    set connectedMode(value) {
        if (!!value !== !!this._connectedMode) {

            if (value)
                this.checkConnectedModeSupport();

            this._connectedMode = !!value;

            this.connectedModeChanged();
            this.updateInterface();
        }
    }

    /**
     * Override this method to perform additional operations when connected mode is switched on/off.
     * By default it sets the items to empty array when connected mode is switched on/off.
     */
    protected connectedModeChanged() {
        this.view.setItems([]);
    }

    /**
     * Update UI elements based on the connected mode. In connected mode, quick search and refresh buttons are visible,
     * and the grid editor has "connected-mode" css class.
     */
    override updateInterface() {
        super.updateInterface();

        this.element.toggleClass("connected-mode", !!this.connectedMode);
        this.toolbar.findButton(".refresh-button").toggle(this.connectedMode);
        this.byId("QuickSearchInput").closest("s-QuickSearchBar").toggle(!!this.connectedMode);
    }

    /**
     * Validates the connected mode support for the grid editor. Checks that the id property is not "__id" and
     * one of getService, getServiceMethod, getServiceUrl methods are overridden to return the service URL of the entity.
     */
    protected checkConnectedModeSupport() {
        if (this.getIdProperty() == "__id") {
            throw new Error(`To switch to connected mode, getIdProperty method of the grid editor (${getTypeFullName(getInstanceType(this))})` + 
                ` and its dialog class must return the actual id property name of the entity,` + 
                ` (e.g. getIdProperty() { return XYZRow.idProperty }), not the default '__id' property name!`);
        }

        if (this.getService === EntityGrid.prototype["getService"] && 
            this.getServiceMethod === EntityGrid.prototype["getServiceMethod"] &&
            this.getServiceUrl === EntityGrid.prototype["getServiceUrl"]) {
            throw new Error(`To switch to connected mode, getService method of the grid editor (${getTypeFullName(getInstanceType(this))})` + 
             ` and its dialog class must be overridden to return the service URL of the entity.`);
        }
    }


    /**
     * Validates the dialog type returned from getDialogType method of the grid editor.
     * It must be a subclass of GridEditorDialog<TEntity>, its id property must match with the grid editor's id property, and
     * its getService() method must return the same service URL with the grid editor's getService() method.
     * 
     * @param dlg Dialog
     */
    protected checkDialogType(dlg: any): GridEditorDialog<TEntity> {
        if (!(dlg instanceof GridEditorDialog)) {
            throw new Error(`The dialog type (${getTypeFullName(getInstanceType(dlg))}) returned from getDialogType` + 
                ` method of ${getTypeFullName(getInstanceType(this))} must be a subclass of GridEditorDialog!`);
        }

        if (dlg["getIdProperty"]() !== this.getIdProperty()) {
            throw new Error(`The id property (${dlg["getIdProperty"]()}) returned from ${getTypeFullName(getInstanceType(dlg))}`
                + ` does not match the id property (${this.getIdProperty()}) its grid editor type ${getTypeFullName(getInstanceType(this))} returns!`
                + ` Please make them both return the same property name or remove getIdProperty() method from both` 
                + ` classes to use the default '__id' property name!`);
        }

        
        if (this.connectedMode && dlg["getService"]?.() !== this.getService?.()) {
            throw new Error(`To use connected mode, getService method of the grid editor dialog class (${getTypeFullName(getInstanceType(dlg))})` + 
             ` must return the same service URL with the grid editor from its getService() method.`);
        }

        return dlg as GridEditorDialog<TEntity>;
    }    
}