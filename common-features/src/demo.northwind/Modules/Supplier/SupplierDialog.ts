import { EntityDialog } from "@serenity-is/corelib";
import { SupplierForm, SupplierRow, SupplierService } from "../ServerTypes/Demo";
import { nsDemoNorthwind } from "../ServerTypes/Namespaces";

export class SupplierDialog<P = {}> extends EntityDialog<SupplierRow, P> {
    static override typeInfo = this.registerClass(nsDemoNorthwind);

    protected getFormKey() { return SupplierForm.formKey; }
    protected getRowDefinition() { return SupplierRow; }
    protected getService() { return SupplierService.baseUrl; }

    protected form = new SupplierForm(this.idPrefix);
}
