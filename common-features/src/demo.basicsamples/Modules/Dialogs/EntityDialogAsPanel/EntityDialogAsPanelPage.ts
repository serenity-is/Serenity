import { Decorators, notifySuccess, SaveResponse } from "@serenity-is/corelib";
import { OrderDialog } from "@serenity-is/demo.northwind";
import "./EntityDialogAsPanelPage.css";

export default function pageInit(model: any) {

    // first create a new dialog, store it in globalThis, e.g. window so that sample can access it from outsite the module
    var myDialogAsPanel = new EntityDialogAsPanel();

    document.querySelector('#SwitchToNewRecordMode').addEventListener('click', () => {
        myDialogAsPanel.load({}, function() { notifySuccess('Switched to new record mode'); });
    });

    document.querySelector('#LoadEntityWithId').addEventListener('click', () => {
        myDialogAsPanel.load(11048, function() { notifySuccess('Loaded entity with ID 11048'); })
    });

    // load a new entity if url doesn't contain an ID, or load order with ID specified in page URL
    // here we use done event in second parameter, to be sure operation succeeded before showing the panel
    myDialogAsPanel.load(model || {}, function () {
        // if we didn't reach here, probably there is no order with specified ID in url
        myDialogAsPanel.domNode.classList.remove('hidden');
        document.querySelector('#DialogDiv').append(myDialogAsPanel.domNode);
        myDialogAsPanel.arrange();
    });
}


/**
 * A version of order dialog converted to a panel by adding Serenity.@Decorators.panel decorator.
 */
@Decorators.panel()
export class EntityDialogAsPanel extends OrderDialog {

    protected updateInterface() {
        super.updateInterface();

        this.deleteButton.hide();
        this.applyChangesButton.hide();
    }

    protected onSaveSuccess(response: SaveResponse) {
        this.showSaveSuccessMessage(response);
    }
}