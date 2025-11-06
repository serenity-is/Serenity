import { EntityDialog } from "@serenity-is/corelib";
import { SupplierForm, SupplierRow, SupplierService } from "../ServerTypes/Demo";
import { nsDemoNorthwind } from "../ServerTypes/Namespaces";

export class SupplierDialog<P = {}> extends EntityDialog<SupplierRow, P> {
    static override[Symbol.typeInfo] = this.registerClass(nsDemoNorthwind);

    protected override getFormKey() { return SupplierForm.formKey; }
    protected override getRowDefinition() { return SupplierRow; }
    protected override getService() { return SupplierService.baseUrl; }

    protected form = new SupplierForm(this.idPrefix);
}
