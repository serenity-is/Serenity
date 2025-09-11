import { EntityDialog } from "@serenity-is/corelib";
import { SupplierForm, SupplierRow, SupplierService } from "../ServerTypes/Demo";

export class SupplierDialog<P = {}> extends EntityDialog<SupplierRow, P> {
    static override typeInfo = this.classTypeInfo("Serenity.Demo.Northwind.SupplierDialog");

    protected getFormKey() { return SupplierForm.formKey; }
    protected getRowDefinition() { return SupplierRow; }
    protected getService() { return SupplierService.baseUrl; }

    protected form = new SupplierForm(this.idPrefix);
}
