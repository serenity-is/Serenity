import { Decorators, EntityDialog } from "@serenity-is/corelib";
import { ShipperForm, ShipperRow, ShipperService } from "../ServerTypes/Demo";

@Decorators.registerClass()
export class ShipperDialog<P = {}> extends EntityDialog<ShipperRow, P> {
    protected getFormKey() { return ShipperForm.formKey; }
    protected getRowDefinition() { return ShipperRow; }
    protected getService() { return ShipperService.baseUrl; }

    protected form = new ShipperForm(this.idPrefix);
}