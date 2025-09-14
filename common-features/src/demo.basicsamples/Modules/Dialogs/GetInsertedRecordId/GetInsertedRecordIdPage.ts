import { gridPageInit, notifyInfo, notifySuccess, SaveInitiator, SaveResponse } from "@serenity-is/corelib";
import { CategoryDialog, CategoryGrid, CategoryService } from "@serenity-is/demo.northwind";
import { nsDemoBasicSamples } from "../../ServerTypes/Namespaces";

export default () => gridPageInit(GetInsertedRecordIdGrid);

/**
 * Subclass of CategoryGrid to override dialog type to GetInsertedRecordIdDialog
 */
export class GetInsertedRecordIdGrid extends CategoryGrid {
    static [Symbol.typeInfo] = this.registerClass(nsDemoBasicSamples);

    protected override getDialogType() { return GetInsertedRecordIdDialog; }
}

export class GetInsertedRecordIdDialog extends CategoryDialog {
    static [Symbol.typeInfo] = this.registerClass(nsDemoBasicSamples);

    /**
     * This method is called after the save request to service
     * is completed succesfully. This can be an insert or update.
     *
     * @param response Response that is returned from server
     */
    protected override onSaveSuccess(response: SaveResponse, initiator: SaveInitiator): void {

        // check that this is an insert
        if (this.isNew()) {
            notifySuccess("Just inserted a category with ID: " + response.EntityId);

            // you could also open a new dialog
            // new CategoryDialog().loadByIdAndOpenDialog(response.EntityId);

            // but let's better load inserted record using Retrieve service
            CategoryService.Retrieve({
                EntityId: response.EntityId
            }, resp => notifyInfo("Looks like the category you added has name: " + resp.Entity.CategoryName));
        }
    }
}