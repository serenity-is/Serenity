import { DataChangeInfo, Decorators, DeleteResponse, DialogType, EditorProps, EntityDialog, EntityGrid, IGetEditValue, ISetEditValue, PropertyItem, SaveRequest, SaveResponse, ServiceOptions, ServiceResponse, ToolButton, deepClone, getInstanceType, getTypeFullName, indexOf, serviceCall } from "@serenity-is/corelib";
import { GridEditorDialog } from "./GridEditorDialog";

@Decorators.registerClass("Serenity.Extensions.GridEditorBase", [IGetEditValue, ISetEditValue])
@Decorators.editor()
@Decorators.element("<div/>")
export abstract class GridEditorBase<TEntity, P = {}> extends EntityGrid<TEntity, P>
    implements IGetEditValue, ISetEditValue {

    protected getIdProperty() { return this.getRowDefinition()?.idProperty ?? "__id"; }

    protected getDialogType(): DialogType | PromiseLike<DialogType> {
        throw new Error(`Please override getDialogType method in the grid editor class (${getTypeFullName(getInstanceType(this))}),` + 
            ` and return the correct dialog type which extends GridEditorDialog!`);
    }

    protected nextId = 1;

    constructor(props: EditorProps<P>) {
        super(props);
    }

    protected id(entity: TEntity) {
        return (entity as any)[this.getIdProperty()];
    }

    protected getNextId() {
        return "`" + this.nextId++;
    }

    protected setNewId(entity: TEntity) {
        entity[this.getIdProperty()] = this.getNextId();
    }

    protected async save(opt: ServiceOptions<any>, callback: (r: ServiceResponse) => void) {
        const request = opt.request as SaveRequest<TEntity>;
        let row = request.Entity;
        let id = this.id(row);

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

            var items = this.view.getItems().slice();
            if (id == null) {
                items.push(row);
            }
            else {
                var index = indexOf(items, x => this.id(x) === id);
                items[index] = row = deepClone({} as TEntity, items[index], row);
            }

            this.setEntities(items);
            callback?.({});
        }

        this.element.trigger("change", { 
            operationType: id == null ? "create" : "update", 
            entityId: id,
            entity: row
        } satisfies Partial<DataChangeInfo>);
    }

    protected async delete(opt: ServiceOptions<any>, callback: (r: ServiceResponse) => void) {
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

    protected validateEntity(row: TEntity, id: any): (boolean | Promise<boolean>) {
        return true;
    }

    protected setEntities(items: TEntity[]) {
        this.view.setItems(items, true);
    }

    protected getNewEntity(): TEntity {
        return {} as TEntity;
    }

    protected getButtons(): ToolButton[] {
        var buttons = super.getButtons();
        var addButton = buttons.find(x => x.action === 'add');
        if (addButton) {
            addButton.onClick = () => {
                this.createEntityDialog(this.getItemType(), dlg => {
                    var dialog = this.checkDialogType(dlg);
                    this.transferDialogReadOnly(dialog);
                    dialog.onSave = (opt, callback) => this.save(opt, callback);
                    dialog.loadEntityAndOpenDialog(this.getNewEntity());
                });
            }
        }

        return buttons;
    }

    protected editItem(entityOrId: any): void {

        var scriptType = typeof (entityOrId);
        let id: any;
        const byId = scriptType === 'string' || scriptType === 'number';
        if (byId)
            id = entityOrId;
        else if (entityOrId != null)
            id = this.id(entityOrId);

        this.createEntityDialog(this.getItemType(), dlg => {
            var dialog = this.checkDialogType(dlg);
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
                var item = (byId ? this.view.getItemById(id) : entityOrId) ?? entityOrId;
                dialog.loadEntityAndOpenDialog(item);
            }
        });
    }

    public getEditValue(property: PropertyItem, target: any) {
        if (!this.connectedMode)
            target[property.name] = this.value;
    }

    public setEditValue(source: any, property: PropertyItem) {
        if (!this.connectedMode)
            this.value = source[property.name];
    }

    public get value(): TEntity[] {
        var p = this.getIdProperty();
        return this.view.getItems().map(x => {
            var y = deepClone(x);
            var id = y[p];
            if (id && id.toString().charAt(0) == '`')
                delete y[p];
            return y;
        });
    }

    public set value(value: TEntity[]) {
        var p = this.getIdProperty();
        this.view.setItems((value || []).map(x => {
            var y = deepClone(x);
            if ((y as any)[p] == null)
                (y as any)[p] = this.getNextId();
            return y;
        }), true);
    }

    protected getGridCanLoad() {
        return super.getGridCanLoad() && !!this._connectedMode;
    }

    protected usePager() {
        return false;
    }

    protected getInitialTitle() {
        return null;
    }

    protected createQuickSearchInput() {
    }

    private _connectedMode = false;

    get connectedMode() {
        return this._connectedMode;
    }

    protected connectedModeChanged() {
        // clear items when connected mode is switched on/off to avoid errors
        // caller that sets connectedMode should also refresh the grid if needed
        this.view.setItems([]);
    }

    override updateInterface() {
        super.updateInterface();

        this.element.toggleClass("connected-mode", !!this.connectedMode);
        this.toolbar.findButton(".refresh-button").toggle(!this.connectedMode);
    }

    protected checkConnectedModeSupport() {
        if (this.getIdProperty() == "__id") {
            throw new Error(`To switch to connected mode, getIdProperty method of the grid editor (${getTypeFullName(getInstanceType(this))})` + 
                ` and its dialog class must return the actual id property name of the entity,` + 
                ` (e.g. getIdProperty() { return XYZRow.idProperty }), not the default '__id' property name!`);
        }

        if (this.getService === EntityDialog.prototype["getService"]) {
            throw new Error(`To switch to connected mode, getService method of the grid editor (${getTypeFullName(getInstanceType(this))})` + 
             ` and its dialog class must be overridden to return the service URL of the entity.`);
        }
    }


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

        
        if (this.connectedMode && dlg["getService"]() !== this.getService()) {
            throw new Error(`To use connected mode, getService method of the grid editor dialog class (${getTypeFullName(getInstanceType(dlg))})` + 
             ` must return the same service URL with the grid editor from its getService() method.`);
        }

        return dlg as GridEditorDialog<TEntity>;
    }    

    /**
     * Sets the connected mode of the grid editor. By default it is false, e.g. in-memory editing mode.
     * Connected mode should only be enabled when the dialog containing grid editor is in edit mode, e.g. 
     * a master entity ID is available. In connected mode, the grid editor will load and save data from/to 
     * services directly, instead of in-memory editing, and validateEntity, deleteEntity, save, delete methods
     * will still be called but they should check the connectedMode property before trying to perform in-memory logic.
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
}