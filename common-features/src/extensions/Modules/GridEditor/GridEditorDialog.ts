import { DeleteResponse, EntityDialog, PanelAttribute, SaveInitiator, SaveResponse, ServiceOptions } from "@serenity-is/corelib";

export abstract class GridEditorDialog<TEntity, P = {}> extends EntityDialog<TEntity, P> {
    static override typeInfo = this.classTypeInfo("Serenity.Extensions.GridEditorDialog", [new PanelAttribute(false)]);

    protected getIdProperty() { return this.getRowDefinition()?.idProperty ?? "__id"; }

    public onSave: (options: ServiceOptions<SaveResponse>,
        callback: (response: SaveResponse) => void, initiator: SaveInitiator) => void;

    public onDelete: (options: ServiceOptions<DeleteResponse>,
        callback: (response: DeleteResponse) => void) => void;

    public destroy() {
        this.onSave = null;
        this.onDelete = null;
        super.destroy();
    }

    protected updateInterface() {
        super.updateInterface();

        // apply changes button doesn't work properly with in-memory grids yet
        this.applyChangesButton.hide();
    }

    protected override saveHandler(options: ServiceOptions<SaveResponse>,
        callback: (response: SaveResponse) => void, initiator: SaveInitiator): void {
        this.onSave?.(options, callback, initiator);
    }

    protected override deleteHandler(options: ServiceOptions<DeleteResponse>,
        callback: (response: DeleteResponse) => void): void {
        this.onDelete?.(options, callback);
    }
}