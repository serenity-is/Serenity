import { Decorators, EditorUtils, ToolButton, gridPageInit, indexOf } from "@serenity-is/corelib";
import { SupplierDialog, SupplierGrid } from "@serenity-is/demo.northwind";

export default () => gridPageInit(ReadOnlyGrid);

/**
 * A readonly grid that launches ReadOnlyDialog
 */
@Decorators.registerClass('Serenity.Demo.BasicSamples.ReadOnlyGrid')
export class ReadOnlyGrid extends SupplierGrid {

    protected getDialogType() { return ReadOnlyDialog; }

    /**
     * Removing add button from grid using its css class
     */
    protected getButtons(): ToolButton[] {
        var buttons = super.getButtons();
        buttons.splice(indexOf(buttons, x => x.action == "add"), 1);
        return buttons;
    }
}

@Decorators.registerClass('Serenity.Demo.BasicSamples.ReadOnlyDialog')
export class ReadOnlyDialog extends SupplierDialog {

    /**
     * This is the method that gets list of tool 
     * buttons to be created in a dialog.
     *
     * Here we'll remove save and close button, and
     * apply changes buttons. 
     */
    protected getToolbarButtons(): ToolButton[] {
        let buttons = super.getToolbarButtons();

        buttons.splice(indexOf(buttons, x => x.action == "save-and-close"), 1);
        buttons.splice(indexOf(buttons, x => x.action == "apply-changes"), 1);

        // We could also remove delete button here, but for demonstration 
        // purposes we'll hide it in another method (updateInterface)
        // buttons.splice(buttons.indexOf(x => x.action == "delete"), 1);

        return buttons;
    }

    /**
     * This method is a good place to update states of
     * interface elements. It is called after dialog
     * is initialized and an entity is loaded into dialog.
     * This is also called in new item mode.
     */
    protected updateInterface(): void {

        super.updateInterface();

        // finding all editor elements and setting their readonly attribute
        // note that this helper method only works with basic inputs, 
        // some editors require widget based set readonly overload (setReadOnly)
        EditorUtils.setReadonly(this.domNode.querySelectorAll('.editor'), true);

        // remove required asterisk (*)
        this.domNode.querySelectorAll('sup').forEach(el => el.style.display = 'none');

        // here is a way to locate a button by its css class
        // note that this method is not available in 
        // getToolbarButtons() because buttons are not 
        // created there yet!
        // 
        // this.toolbar.findButton('delete-button').hide();

        // entity dialog also has reference variables to
        // its default buttons, lets use them to hide delete button
        this.deleteButton.hide();

        // we could also hide save buttons just like delete button,
        // but they are null now as we removed them in getToolbarButtons()
        // if we didn't we could write like this:
        // 
        // this.applyChangesButton.hide();
        // this.saveAndCloseButton.hide();

        // instead of hiding, we could disable a button
        // 
        // this.deleteButton.toggleClass('disabled', true);
    }

    /**
     * This method is called when dialog title needs to be updated.
     * Base class returns something like 'Edit xyz' for edit mode,
     * and 'New xyz' for new record mode.
     * 
     * But our dialog is readonly, so we should change it to 'View xyz'
     */
    protected getEntityTitle(): string {

        if (!this.isEditMode()) {
            // we shouldn't hit here, but anyway for demo...
            return "How did you manage to open this dialog in new record mode?";
        }
        else {
            // entitySingular is type of record this dialog edits. something like 'Supplier'.
            // you could hardcode it, but this is for demonstration
            var entityType = super.getEntitySingular();

            // get name field value of record this dialog edits
            let name = this.getEntityNameFieldValue() || "";

            // you could use format with a local text, but again demo...
            return 'View ' + entityType + " (" + name + ")";
        }
    }

    /**
     * This method is actually the one that calls getEntityTitle()
     * and updates the dialog title. We could do it here too...
     */
    protected updateTitle(): void {
        super.updateTitle();

        // remove super.updateTitle() call above and uncomment 
        // below line if you'd like to use this version
        // 
        // this.dialogTitle = 'View Supplier (' + this.getEntityNameFieldValue() + ')';
    }

}