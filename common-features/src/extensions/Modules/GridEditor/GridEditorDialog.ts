import { Decorators, DeleteResponse, EntityDialog, SaveResponse, ServiceOptions } from "@serenity-is/corelib";

@Decorators.registerClass("Serenity.Extensions.GridEditorDialog")
@Decorators.panel(false)
export abstract class GridEditorDialog<TEntity, P = {}> extends EntityDialog<TEntity, P> {
    protected getIdProperty() { return "__id"; }

    public onSave: (options: ServiceOptions<SaveResponse>,
        callback: (response: SaveResponse) => void) => void;

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
        if (this.applyChangesButton) {
            this.applyChangesButton.hide();
        }
    }

    protected saveHandler(options: ServiceOptions<SaveResponse>,
        callback: (response: SaveResponse) => void): void {
        this.onSave && this.onSave(options, callback);
    }

    protected deleteHandler(options: ServiceOptions<DeleteResponse>,
        callback: (response: DeleteResponse) => void): void {
        this.onDelete && this.onDelete(options, callback);
    }
}