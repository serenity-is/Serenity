import { EntityDialog } from "@serenity-is/corelib";
import { ShipperForm, ShipperRow, ShipperService } from "../ServerTypes/Demo";
import { nsDemoNorthwind } from "../ServerTypes/Namespaces";

export class ShipperDialog<P = {}> extends EntityDialog<ShipperRow, P> {
    static override typeInfo = this.registerClass(nsDemoNorthwind);

    protected getFormKey() { return ShipperForm.formKey; }
    protected getRowDefinition() { return ShipperRow; }
    protected getService() { return ShipperService.baseUrl; }

    protected form = new ShipperForm(this.idPrefix);
}