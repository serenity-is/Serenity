import { EntityDialog } from "@serenity-is/corelib";
import { ShipperForm, ShipperRow, ShipperService } from "../ServerTypes/Demo";
import { nsDemoNorthwind } from "../ServerTypes/Namespaces";

export class ShipperDialog<P = {}> extends EntityDialog<ShipperRow, P> {
    static override[Symbol.typeInfo] = this.registerClass(nsDemoNorthwind);

    protected override getFormKey() { return ShipperForm.formKey; }
    protected override getRowDefinition() { return ShipperRow; }
    protected override getService() { return ShipperService.baseUrl; }

    protected form = new ShipperForm(this.idPrefix);
}