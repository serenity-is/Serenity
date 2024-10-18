import { Decorators, EditorProps, EntityGrid, IGetEditValue, ISetEditValue, SaveRequest, ServiceOptions, ServiceResponse, ToolButton, deepClone, indexOf } from "@serenity-is/corelib";
import { GridEditorDialog } from "./GridEditorDialog";

@Decorators.registerClass("Serenity.Extensions.GridEditorBase", [IGetEditValue, ISetEditValue])
@Decorators.editor()
@Decorators.element("<div/>")
export abstract class GridEditorBase<TEntity, P = {}> extends EntityGrid<TEntity, P>
    implements IGetEditValue, ISetEditValue {

    protected getIdProperty() { return "__id"; }

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

    protected save(opt: ServiceOptions<any>, callback: (r: ServiceResponse) => void) {
        var request = opt.request as SaveRequest<TEntity>;
        var row = deepClone(request.Entity);

        var id = this.id(row);
        if (id == null) {
            (row as any)[this.getIdProperty()] = this.getNextId();
        }

        if (!this.validateEntity(row, id)) {
            return;
        }

        var items = this.view.getItems().slice();
        if (id == null) {
            items.push(row);
        }
        else {
            var index = indexOf(items, x => this.id(x) === id);
            items[index] = deepClone({} as TEntity, items[index], row);
        }

        this.setEntities(items);
        callback({});
    }

    protected deleteEntity(id: number) {
        this.view.deleteItem(id);
        return true;
    }

    protected validateEntity(row: TEntity, id: number) {
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
                    var dialog = dlg as GridEditorDialog<TEntity>;
                    dialog.onSave = (opt, callback) => this.save(opt, callback);
                    this.transferDialogReadOnly(dialog);
                    dialog.loadEntityAndOpenDialog(this.getNewEntity());
                });
            }
        }

        return buttons.filter(x => x.action !== "refresh");
    }

    protected editItem(entityOrId: any): void {

        var id = entityOrId;
        var item = this.view.getItemById(id);
        this.createEntityDialog(this.getItemType(), dlg => {
            var dialog = dlg as GridEditorDialog<TEntity>;
            dialog.onDelete = (opt, callback) => {
                if (!this.deleteEntity(id)) {
                    return;
                }
                callback({});
            };
            this.transferDialogReadOnly(dialog);
            dialog.onSave = (opt, callback) => this.save(opt, callback);
            dialog.loadEntityAndOpenDialog(item);
        });;
    }

    public getEditValue(property, target) {
        target[property.name] = this.value;
    }

    public setEditValue(source, property) {
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
        return false;
    }

    protected usePager() {
        return false;
    }

    protected getInitialTitle() {
        return null;
    }

    protected createQuickSearchInput() {
    }
}