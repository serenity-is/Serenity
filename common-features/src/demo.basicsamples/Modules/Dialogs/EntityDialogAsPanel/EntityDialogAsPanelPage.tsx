import { Attributes, notifySuccess, PanelAttribute, resolveUrl, SaveInitiator, SaveResponse } from "@serenity-is/corelib";
import { OrderDialog, type OrderRow } from "@serenity-is/demo.northwind";
import { SampleInfo } from "../../sample-info";
import { nsDemoBasicSamples } from "../../ServerTypes/Namespaces";
import "./EntityDialogAsPanelPage.css";

export default (model: number | OrderRow) => {

    // first create a new dialog, store it in globalThis, e.g. window so that sample can access it from outsite the module
    var myDialogAsPanel = new EntityDialogAsPanel();

    // load a new entity if url doesn't contain an ID, or load order with ID specified in page URL
    // here we use done event in second parameter, to be sure operation succeeded before showing the panel
    myDialogAsPanel.load(model || {}, function () {
        // if we didn't reach here, probably there is no order with specified ID in url
        myDialogAsPanel.domNode.hidden = false;
        document.querySelector('#PanelDiv').append(myDialogAsPanel.domNode);
        myDialogAsPanel.arrange();
    });

    return <SampleInfo sources={[".css"]}>
        <p>This sample demonstrates how to use an entity dialog as an inline panel. Try operations below:</p>
        <br />
        <ul>
            <li><a href={resolveUrl("~/BasicSamples/EntityDialogAsPanel/")}>Switch to new record mode by changing URL</a></li>
            <li><a href={resolveUrl("~/BasicSamples/EntityDialogAsPanel/11068")}>Load Order with ID 11068 by changing URL</a></li>
            <li><a id="SwitchToNewRecordMode" href="javascript:;" onClick={() => {
                myDialogAsPanel.load({}, function () { notifySuccess('Switched to new record mode'); });
            }}>Switch to new record mode with Javascript</a></li>
            <li><a id="LoadEntityWithId" href="javascript:;" onClick={() => {
                myDialogAsPanel.load({ OrderID: 11048 }, function () { notifySuccess('Loaded order with ID 11048'); });
            }}>Load Order with ID 11048 with javascript</a></li>
            <li><a href={resolveUrl("~/BasicSamples/EntityDialogAsPanel/11077")}>Back to Order with ID 11077 by changing URL</a></li>
        </ul>
    </SampleInfo>
}

/**
 * A version of order dialog converted to a panel by adding PanelAttribute.
 */
export class EntityDialogAsPanel extends OrderDialog {
    static override[Symbol.typeInfo] = this.registerClass(nsDemoBasicSamples, [Attributes.panel]);

    protected override updateInterface() {
        super.updateInterface();

        this.deleteButton.hide();
        this.applyChangesButton.hide();
    }

    protected override onSaveSuccess(response: SaveResponse, initiator: SaveInitiator) {
        this.showSaveSuccessMessage(response, initiator);
    }
}